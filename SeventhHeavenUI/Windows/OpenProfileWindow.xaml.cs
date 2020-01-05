﻿using SeventhHeaven.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeventhHeaven.Windows
{
    /// <summary>
    /// Interaction logic for OpenProfileWindow.xaml
    /// </summary>
    public partial class OpenProfileWindow : Window
    {
        public OpenProfileViewModel ViewModel { get; set; }

        public OpenProfileWindow()
        {
            InitializeComponent();

            ViewModel = new OpenProfileViewModel();
            this.DataContext = ViewModel;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void menuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!IsProfileSelected())
            {
                return;
            }

            string selected = (string)lstProfiles.SelectedItem;

            if (MessageDialogWindow.Show($"Are you sure you want to delete the selected profile ({selected})?", "Delete Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning).Result == MessageBoxResult.Yes)
            {
                ViewModel.DeleteProfile(selected);
            }
        }

        private void menuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!IsProfileSelected())
            {
                return;
            }

            ViewModel.CopyProfile((string)lstProfiles.SelectedItem);
        }

        private void lstProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedProfile = (string)lstProfiles.SelectedItem;
        }

        private bool IsProfileSelected()
        {
            if (lstProfiles.SelectedItem == null)
            {
                MessageDialogWindow.Show("Select a Profile first.", "No Profile Selected", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void menuItemNew_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateNewProfile();
            lstProfiles.SelectedItem = ViewModel.SelectedProfile;
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CloseWindow(true);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedProfile = null;
            CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            this.DialogResult = dialogResult;
            this.Close();
        }

        private void menuItemDetails_Click(object sender, RoutedEventArgs e)
        {
            if (!IsProfileSelected())
            {
                return;
            }

            ViewModel.ViewProfileDetails((string)lstProfiles.SelectedItem);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.AttemptDeleteTempFiles();
        }
    }
}
