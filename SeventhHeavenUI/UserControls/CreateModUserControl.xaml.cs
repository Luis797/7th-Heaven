﻿using _7thHeaven.Code;
using SeventhHeaven.Classes;
using SeventhHeaven.Windows;
using SeventhHeavenUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SeventhHeaven.UserControls
{
    /// <summary>
    /// Interaction logic for CreateModUserControl.xaml
    /// </summary>
    public partial class CreateModUserControl : UserControl
    {
        ModCreationViewModel ViewModel { get; set; }
        public CreateModUserControl()
        {
            InitializeComponent();

            ViewModel = new ModCreationViewModel();
            this.DataContext = ViewModel;
        }

        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            string pathToFile = FileDialogHelper.BrowseForFile("*.png,*.jpg,*.jpeg|*.png;*.jpg;*.jpeg", "Select image to use");


            if (!string.IsNullOrEmpty(pathToFile))
            {
                ViewModel.PreviewImageInput = System.IO.Path.GetFileName(pathToFile);
                MessageDialogWindow.Show(string.Format(ResourceHelper.Get(StringKey.MakeSureToCopyToTheRootFolder), ViewModel.PreviewImageInput), ResourceHelper.Get(StringKey.Info), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GenerateModOutput();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string pathToFile = FileDialogHelper.OpenSaveDialog("mod .xml|*.xml", "Save mod xml");

            if (!string.IsNullOrEmpty(pathToFile))
            {
                ViewModel.SaveModXml(pathToFile);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string pathToFile = FileDialogHelper.BrowseForFile("mod .xml|*.xml", "Select mod.xml to Load");
        
            if (!string.IsNullOrEmpty(pathToFile))
            {
                ViewModel.LoadModXml(pathToFile);
            }
        }
    }
}
