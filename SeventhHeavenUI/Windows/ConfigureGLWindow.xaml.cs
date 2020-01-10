﻿using Iros._7th;
using Iros._7th.Workshop;
using SeventhHeaven.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SeventhHeaven.Windows
{
    /// <summary>
    /// Interaction logic for ConfigureGLWindow.xaml
    /// </summary>
    public partial class ConfigureGLWindow : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private Iros._7th.Workshop.ConfigSettings.Settings _settings;
        private Iros._7th.Workshop.ConfigSettings.ConfigSpec _spec;
        private string _file;

        private List<GLSettingViewModel> ViewModels { get; set; }

        private Button btnClearTextureCache;

        public ConfigureGLWindow()
        {
            InitializeComponent();

            ViewModels = new List<GLSettingViewModel>();
        }

        public void SetStatusMessage(string message)
        {
            txtStatusMessage.Text = message;
        }


        public void Init(string cfgSpec, string cfgFile)
        {
            try
            {
                _settings = new Iros._7th.Workshop.ConfigSettings.Settings(System.IO.File.ReadAllLines(cfgFile));
                _spec = Util.Deserialize<Iros._7th.Workshop.ConfigSettings.ConfigSpec>(cfgSpec);
                _file = cfgFile;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                MessageDialogWindow.Show("Failed to read cfg/spec file. Closing window.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

            Dictionary<string, int> tabOrders = new Dictionary<string, int>()
            {
                {"Graphics", 0},
                {"Rendering", 1},
                {"Advanced", 3}
            };

            foreach (var items in _spec.Settings.GroupBy(s => s.Group)
                                                .Select(g => new { settingGroup = g, SortOrder = tabOrders[g.Key] })
                                                .OrderBy(g => g.SortOrder)
                                                .Select(g => g.settingGroup))
            {
                TabItem tab = new TabItem()
                {
                    Header = items.Key,
                };

                StackPanel stackPanel = new StackPanel()
                {
                    Margin = new Thickness(0, 5, 0, 0)
                };

                ScrollViewer scrollViewer = new ScrollViewer()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                foreach (Iros._7th.Workshop.ConfigSettings.Setting setting in items)
                {
                    GLSettingViewModel settingViewModel = new GLSettingViewModel(setting, _settings);

                    ContentControl settingControl = new ContentControl();
                    settingControl.DataContext = settingViewModel;
                    settingControl.MouseEnter += SettingControl_MouseEnter;

                    ViewModels.Add(settingViewModel);
                    stackPanel.Children.Add(settingControl);
                }

                if (items.Key == "Advanced")
                {
                    // add clear texture cache button
                    btnClearTextureCache = new Button()
                    {
                        Content = "Clear Texture Cache",
                        ToolTip = @"Will delete everything under 'modpath' (in cfg) \Textures\cache\*.* path",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    btnClearTextureCache.Click += BtnClearTextureCache_Click;

                    stackPanel.Children.Add(btnClearTextureCache);
                }


                scrollViewer.Content = stackPanel;
                tab.Content = scrollViewer;
                tabCtrlMain.Items.Add(tab);
            }
        }

        private void BtnClearTextureCache_Click(object sender, RoutedEventArgs e)
        {
            ClearTextureCache();
        }

        private void ClearTextureCache()
        {
            string pathToCache = Path.Combine(Sys.Settings.AaliFolder, "cache");

            if (!Directory.Exists(pathToCache))
            {
                return;
            }

            try
            {
                foreach (string file in Directory.GetFiles(pathToCache))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException ex)
                    {
                        Logger.Warn(ex); // add warning to log if fail to delete file but continue deleting other files
                    }
                }

                txtStatusMessage.Text = "Texture cache cleared!";
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                txtStatusMessage.Text = $"Failed to clear texture cache: {ex.Message}";
            }
        }

        private void SettingControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            GLSettingViewModel viewModel = ((ContentControl)sender)?.DataContext as GLSettingViewModel;

            if (viewModel == null)
            {
                return;
            }

            SetStatusMessage(viewModel.Description);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (GLSettingViewModel item in ViewModels)
                {
                    item.Save(_settings);
                }

                System.IO.File.WriteAllLines(_file, _settings.GetOutput());

                Sys.Message(new WMessage("Game Driver settings saved!"));
                this.Close();
            }
            catch (UnauthorizedAccessException)
            {
                MessageDialogWindow.Show("Could not write to ff7_opengl.cfg file. Check that it is not set to read only, and that FF7 is installed in a folder you have full write access to.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                SetStatusMessage("Unknown error while saving. error has been logged.");
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModels)
            {
                item.ResetToDefault(_settings);
            }

            SetStatusMessage("Game Driver settings reset to defaults. Click 'Save' to save changes.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (btnClearTextureCache != null)
            {
                btnClearTextureCache.Click -= BtnClearTextureCache_Click;
            }
        }
    }
}
