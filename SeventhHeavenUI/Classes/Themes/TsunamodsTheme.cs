﻿using SeventhHeaven.ViewModels;
using SeventhHeavenUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SeventhHeaven.Classes.Themes
{
    public class TsunamodsTheme : ITheme
    {
        public string Name { get => "Tsunamods"; }

        public string PrimaryAppBackground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsBackgroundColor") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string SecondaryAppBackground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsSecondaryBackgroundColor") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlBackground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlBackground") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlForeground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlForeground") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlPressed
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlPressed") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlMouseOver
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlMouseOver") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlDisabledBackground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlDisabledBackground") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string PrimaryControlDisabledForeground
        {
            get
            {
                Color? colorResource = App.Current.TryFindResource("TsunamodsControlDisabledForeground") as Color?;
                return ThemeSettings.ColorToHexString(colorResource.Value);
            }
        }

        public string BackgroundImageName { get => "Tsunamods Logo"; }
    }
}