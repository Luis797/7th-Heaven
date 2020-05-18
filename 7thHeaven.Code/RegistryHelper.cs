﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace _7thHeaven.Code
{
    public enum FF7RegKey
    {
        SteamKeyPath,
        RereleaseKeyPath,
        SquareSoftKeyPath,
        FF7AppKeyPath,
        VirtualStoreKeyPath
    }

    public static class RegistryHelper
    {
        public const string SteamKeyPath64Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 39140";

        public const string SteamKeyPath32Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 39140";

        public const string RereleaseKeyPath64Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{141B8BA9-BFFD-4635-AF64-078E31010EC3}_is1";

        public const string RereleaseKeyPath32Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{141B8BA9-BFFD-4635-AF64-078E31010EC3}_is1";

        public const string SquareSoftKeyPath64Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Square Soft, Inc.";

        public const string SquareSoftKeyPath32Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Square Soft, Inc.";

        public const string FF7AppKeyPath64Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\App Paths\FF7app.exe";

        public const string FF7AppKeyPath32Bit = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\FF7app.exe";

        public const string VirtualStoreKeyPath64Bit = @"HKEY_CURRENT_USER\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE\Wow6432Node\Square Soft, Inc.";

        public const string VirtualStoreKeyPath32Bit = @"HKEY_CURRENT_USER\SOFTWARE\Classes\VirtualStore\MACHINE\SOFTWARE\Square Soft, Inc.";

        /// <summary>
        /// Returns the path to the Registry Key associated with the given <paramref name="regKey"/> for the correct OS Bitness.
        /// </summary>
        public static string GetKeyPath(FF7RegKey regKey)
        {
            return GetKeyPath(regKey, use32BitKey: !Environment.Is64BitOperatingSystem);
        }

        /// <summary>
        /// Returns the path to the Registry Key associated with the given <paramref name="regKey"/>.
        /// The returned path can be for 32-bit or 64-bit OS depending on <paramref name="use32BitKey"/>
        /// </summary>
        public static string GetKeyPath(FF7RegKey regKey, bool use32BitKey)
        {
            if (regKey == FF7RegKey.SteamKeyPath)
            {
                if (use32BitKey)
                {
                    return SteamKeyPath32Bit;
                }
                else
                {
                    return SteamKeyPath64Bit;
                }
            }

            if (regKey == FF7RegKey.RereleaseKeyPath)
            {
                if (use32BitKey)
                {
                    return RereleaseKeyPath32Bit;
                }
                else
                {
                    return RereleaseKeyPath64Bit;
                }
            }

            if (regKey == FF7RegKey.FF7AppKeyPath)
            {
                if (use32BitKey)
                {
                    return FF7AppKeyPath32Bit;
                }
                else
                {
                    return FF7AppKeyPath64Bit;
                }
            }

            if (regKey == FF7RegKey.SquareSoftKeyPath)
            {
                if (use32BitKey)
                {
                    return SquareSoftKeyPath32Bit;
                }
                else
                {
                    return SquareSoftKeyPath64Bit;
                }
            }

            if (regKey == FF7RegKey.VirtualStoreKeyPath)
            {
                if (use32BitKey)
                {
                    return VirtualStoreKeyPath32Bit;
                }
                else
                {
                    return VirtualStoreKeyPath64Bit;
                }
            }

            return "";
        }

        /// <summary>
        /// Returns the current user or local machine RegistryKey based on what the key path starts with.
        /// (Opens the key with the correct RegistryView based on the OS Bitness)
        /// </summary>
        private static RegistryKey GetBaseKey(string fullRegPath, bool force32View = false)
        {
            RegistryView view = RegistryView.Registry32;
            if (Environment.Is64BitOperatingSystem && !force32View)
            {
                view = RegistryView.Registry64;
            }

            if (fullRegPath.StartsWith("HKEY_LOCAL_MACHINE"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
            }
            else if (fullRegPath.StartsWith("HKEY_CURRENT_USER"))
            {
                return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view);
            }

            return null;
        }

        /// <summary>
        /// Removes 'HKEY_LOCAL_MACHINE\' or 'HKEY_CURRENT_USER\' from <paramref name="fullRegKeyPath"/>
        /// </summary>
        /// <returns> modified registry key path </returns>
        private static string RemoveBaseKeyFromPath(string fullRegKeyPath)
        {
            return fullRegKeyPath.Replace(@"HKEY_LOCAL_MACHINE\", "").Replace(@"HKEY_CURRENT_USER\", "");
        }

        public static object GetValue(FF7RegKey key, string valueName, object defaultVal = null)
        {
            return GetValue(GetKeyPath(key), valueName, defaultVal);
        }

        public static object GetValue(string key, string valueName, object defaultVal = null)
        {
            RegistryKey rootKey = GetBaseKey(key);

            if (rootKey == null)
            {
                return defaultVal;
            }

            // remove from key path if exists since not needed to open a Subkey
            string fullRegPath = RemoveBaseKeyFromPath(key);

            RegistryKey regKey = rootKey.OpenSubKey(fullRegPath, false);

            if (regKey == null)
            {
                return defaultVal;
            }

            return regKey.GetValue(valueName, defaultVal);
        }

        /// <summary>
        /// Sets the name/value pair in the specified Registry Key. Creates the Registry Key if it does not exist already.
        /// </summary>
        public static bool SetValue(string key, string valueName, object value, RegistryValueKind regKind = RegistryValueKind.String)
        {
            RegistryKey rootKey = GetBaseKey(key);

            if (rootKey == null)
            {
                return false;
            }

            // remove from key path if exists since not needed to open a Subkey
            string fullRegPath = RemoveBaseKeyFromPath(key);

            RegistryKey regKey = rootKey.CreateSubKey(fullRegPath);

            if (regKey == null)
            {
                return false;
            }

            regKey.SetValue(valueName, value, regKind);
            return true;
        }

        /// <summary>
        /// Uses reg.exe to export a registry key to specified .reg file
        /// </summary>
        /// <param name="regKey"> registry key to export </param>
        /// <param name="savePath"> full path to .reg file to save to </param>
        /// <returns></returns>
        public static bool ExportKey(string regKey, string savePath)
        {
            string bitness = "32";
            if (Environment.Is64BitOperatingSystem)
            {
                bitness = "64";
            }

            Process proc = new Process();

            try
            {
                proc.StartInfo.FileName = "reg.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc = Process.Start("reg.exe", $"export \"{regKey}\" \"{savePath}\" /reg:{bitness} /y");

                if (proc != null) proc.WaitForExit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (proc != null) proc.Dispose();
            }

        }

        /// <summary>
        /// Deletes a given Registry Key and any child subkeys
        /// </summary>
        public static bool DeleteKey(string keyPath)
        {
            RegistryKey rootKey = GetBaseKey(keyPath);

            if (rootKey == null)
            {
                return false;
            }


            try
            {
                string fullRegPath = RemoveBaseKeyFromPath(keyPath);
                rootKey.DeleteSubKeyTree(fullRegPath, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns list of Value Names of a Registry Key
        /// </summary>
        public static List<string> GetValueNamesFromKey(string keyPath)
        {
            RegistryKey rootKey = GetBaseKey(keyPath);

            if (rootKey == null)
            {
                return new List<string>();
            }

            try
            {
                string fullRegPath = RemoveBaseKeyFromPath(keyPath);
                RegistryKey regKey = rootKey.OpenSubKey(fullRegPath, false);

                return regKey.GetValueNames().ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Deletes a specified Value from a Registry Key
        /// </summary>
        public static bool DeleteValueFromKey(string keyPath, string valueName)
        {
            RegistryKey rootKey = GetBaseKey(keyPath);

            if (rootKey == null)
            {
                return false;
            }


            try
            {
                string fullRegPath = RemoveBaseKeyFromPath(keyPath);
                RegistryKey regKey = rootKey.OpenSubKey(fullRegPath, true);

                if (regKey == null)
                {
                    // check if subkey can be opened in 32-bit view
                    rootKey = GetBaseKey(keyPath, force32View: true);
                    regKey = rootKey?.OpenSubKey(fullRegPath, true);

                    if (regKey == null)
                    {
                        return false;
                    }
                }

                regKey.DeleteValue(valueName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Update Registry with new value if it has changed from the current value in the Registry.
        /// Returns true if changed.
        /// </summary>
        public static bool SetValueIfChanged(string regKeyPath, string regValueName, object newValue, RegistryValueKind valueKind = RegistryValueKind.String)
        {
            object currentValue = GetValue(regKeyPath, regValueName, null);
            bool isValuesEqual;

            if (newValue is byte[])
            {
                string currentConverted = currentValue == null ? "" : BitConverter.ToString(currentValue as byte[]);
                string newConverted = newValue == null ? "" : BitConverter.ToString(newValue as byte[]);

                isValuesEqual = currentConverted != null && currentConverted.Equals(newConverted);
            }
            else
            {
                isValuesEqual = currentValue != null && currentValue.Equals(newValue);
            }

            if (!isValuesEqual)
            {
                SetValue(regKeyPath, regValueName, newValue, valueKind);
                return true;
            }

            return false;
        }
    }
}
