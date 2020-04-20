﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS4Windows;

namespace SeventhHeaven.Classes
{
    internal class DS4ControllerService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static DS4ControllerService instance;

        internal ControlService RootHub { get; set; }

        internal bool IsRunning { get => RootHub != null && RootHub.IsRunning; }

        /// <summary>
        /// static instance that can be accessed from anywhere in app such as game launcher or controls window
        /// </summary>
        internal static DS4ControllerService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DS4ControllerService();
                }

                return instance;
            }
        }


        public DS4ControllerService()
        {
            RootHub = new ControlService();
        }

        public void StartService()
        {
            if (IsRunning)
            {
                return;
            }

            if (RootHub == null)
            {
                RootHub = new ControlService();
            }

            RootHub.Debug += RootHub_LogDebug;
            DS4Windows.Global.UseExclusiveMode = true;
            RootHub.Start(showlog: true);
        }

        public void StopService()
        {
            if (!IsRunning)
            {
                return;
            }

            RootHub.Stop(showlog: true);
            RootHub.Debug -= RootHub_LogDebug;
            RootHub = null;
        }


        private void RootHub_LogDebug(object sender, DebugEventArgs e)
        {
            if (e.Warning)
            {
                Logger.Warn(e.Data);
            }
            else
            {
                Logger.Info(e.Data);
            }
        }
    }
}
