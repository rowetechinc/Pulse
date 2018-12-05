/*
 * Copyright © 2013 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 08/26/2013      RC          3.0.8       Initial coding
 * 08/27/2013      RC          3.0.8       Added IsCheckingForUpdates to know when checking for an update is complete.
 * 12/16/2013      RC          3.2.0       Added OpenUserGuideCommand to open the user guide.
 * 12/17/2013      RC          3.2.1       Updated the URL in CheckForUpdates() for the new pulse location.
 * 08/07/2014      RC          4.0.0       Updated ReactiveCommand to 6.0.
 * 10/27/2015      RC          4.3.1       Added version number for Pulse Display.
 * 09/11/2017      RC          4.5.4       Check if the website exists for AutoUpdate.
 * 01/17/2018      RC          4.7.2       Made messagebox pop up for AutoUpdater.  Add URL to update.
 * 
 */

using Caliburn.Micro;

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ReactiveUI;
    using System.IO;
    using AutoUpdaterDotNET;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Net;
    using System.Windows.Threading;
    using System.Windows;

    /// <summary>
    /// About the Pulse Software.
    /// </summary>
    public class AboutViewModel : Screen, IDisposable
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Licneses file.
        /// </summary>
        public const string PULSE_LICENSES_PATH = "Licenses.txt";

        /// <summary>
        /// End User Rights file.
        /// </summary>
        public const string PULSE_END_USER_RIGHTS_PATH = "EndUserRights.txt";

        /// <summary>
        /// Copyright file.
        /// </summary>
        public const string PULSE_COPY_RIGHT_PATH = "Copyright.txt";

        #endregion

        #region Properties

        #region Version

        /// <summary>
        /// Pulse version number.
        /// </summary>
        private string _PulseVersion;
        /// <summary>
        /// Pulse version number.
        /// </summary>
        public string PulseVersion
        {
            get { return _PulseVersion; }
            set
            {
                _PulseVersion = value;
                this.NotifyOfPropertyChange(() => this.PulseVersion);
            }
        }

        /// <summary>
        /// A string to nofity the user if the version is not update to date.
        /// </summary>
        private string _PulseVersionUpdateToDate;
        /// <summary>
        /// A string to nofity the user if the version is not update to date.
        /// </summary>
        public string PulseVersionUpdateToDate
        {
            get { return _PulseVersionUpdateToDate; }
            set
            {
                _PulseVersionUpdateToDate = value;
                this.NotifyOfPropertyChange(() => this.PulseVersionUpdateToDate);
            }
        }

        /// <summary>
        /// Pulse Display version number.
        /// </summary>
        private string _PulseDisplayVersion;
        /// <summary>
        /// Pulse Display version number.
        /// </summary>
        public string PulseDisplayVersion
        {
            get { return _PulseDisplayVersion; }
            set
            {
                _PulseDisplayVersion = value;
                this.NotifyOfPropertyChange(() => this.PulseDisplayVersion);
            }
        }

        /// <summary>
        /// RTI version number.
        /// </summary>
        private string _RtiVersion;
        /// <summary>
        /// RTI version number.
        /// </summary>
        public string RtiVersion
        {
            get { return _RtiVersion; }
            set
            {
                _RtiVersion = value;
                this.NotifyOfPropertyChange(() => this.RtiVersion);
            }
        }

        /// <summary>
        /// Flag to determine if we are looking for the update.
        /// </summary>
        private bool _IsCheckingForUpdates;
        /// <summary>
        /// Flag to determine if we are looking for the update.
        /// </summary>
        public bool IsCheckingForUpdates
        {
            get { return _IsCheckingForUpdates; }
            set
            {
                _IsCheckingForUpdates = value;
                this.NotifyOfPropertyChange(() => this.IsCheckingForUpdates);
            }
        }

        /// <summary>
        /// RTI Pulse Update URL.
        /// </summary>
        private string _PulseUpdateUrl;
        /// <summary>
        /// RTI Pulse Update URL.
        /// </summary>
        public string PulseUpdateUrl
        {
            get { return _PulseUpdateUrl; }
            set
            {
                _PulseUpdateUrl = value;
                this.NotifyOfPropertyChange(() => this.PulseUpdateUrl);
            }
        }

        #endregion

        #region Pulse Info

        /// <summary>
        /// Either the User Agreement or License information.
        /// </summary>
        private string _PulseInfo;
        /// <summary>
        /// Either the User Agreement or License information.
        /// </summary>
        public string PulseInfo
        {
            get { return _PulseInfo; }
            set
            {
                _PulseInfo = value;
                this.NotifyOfPropertyChange(() => this.PulseInfo);
            }
        }

        #endregion

        #region Copyright Info

        /// <summary>
        /// Copyright information.
        /// </summary>
        private string _Copyright;
        /// <summary>
        /// Copyright information.
        /// </summary>
        public string Copyright
        {
            get { return _Copyright; }
            set
            {
                _Copyright = value;
                this.NotifyOfPropertyChange(() => this.Copyright);
            }
        }

        #endregion

        #region Rendering Options

        /// <summary>
        /// The render tier of the system currently
        /// running the application.
        /// 
        /// Rendering Tier 0 No graphics hardware acceleration. All graphics features use software acceleration. The DirectX version level is less than version 9.0.
        /// Rendering Tier 1 Some graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// Rendering Tier 2 Most graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// </summary>
        private int _RenderTier;
        /// <summary>
        /// The render tier of the system currently
        /// running the application.
        /// 
        /// Rendering Tier 0 No graphics hardware acceleration. All graphics features use software acceleration. The DirectX version level is less than version 9.0.
        /// Rendering Tier 1 Some graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// Rendering Tier 2 Most graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// 
        /// http://msdn.microsoft.com/en-us/library/ms742196.aspx
        /// http://msdn.microsoft.com/en-us/library/system.windows.media.rendercapability.tier.aspx
        /// </summary>
        public int RenderTier
        {
            get { return _RenderTier; }
            set
            {
                _RenderTier = value;
                this.NotifyOfPropertyChange(() => this.RenderTier);
            }
        }


        /// <summary>
        /// ProcessRenderMode of the application.  When the application is
        /// started, this will state which mode the application was started in,
        /// Software or Hardware Rendering.
        /// </summary>
        private string _ProcessRenderMode;
        /// <summary>
        /// ProcessRenderMode of the application.  When the application is
        /// started, this will state which mode the application was started in,
        /// Software or Hardware Rendering.
        /// </summary>
        public string ProcessRenderMode
        {
            get { return _ProcessRenderMode; }
            set
            {
                _ProcessRenderMode = value;
                this.NotifyOfPropertyChange(() => this.ProcessRenderMode);
            }
        }

        /// <summary>
        /// The registry key value for Hardware Rendering for WPF 
        /// application Disabled.
        /// 
        /// HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics\DisableHWAcceleration != 0
        /// If the value is not set to 0, then hardware rendering is disabled.
        /// </summary>
        private int _RegKeyHwdrRenderDisabled;
        /// <summary>
        /// The registry key value for Hardware Rendering for WPF 
        /// application Disabled.
        /// 
        /// HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics\DisableHWAcceleration != 0
        /// If the value is not set to 0, then hardware rendering is disabled.
        /// </summary>
        public int RegKeyHwdrRenderDisabled
        {
            get { return _RegKeyHwdrRenderDisabled; }
            set
            {
                _RegKeyHwdrRenderDisabled = value;
                this.NotifyOfPropertyChange(() => this.RegKeyHwdrRenderDisabled);
            }
        }

        /// <summary>
        /// The rendering mode of the application based off the
        /// settings found on the system and application.  The
        /// rendering modes are either Software Rendering or
        /// Hardware Rendering.  Hardware Rendering is preferred
        /// because it used the GPU to do the rendering.  But
        /// system settings or hardware limitations can cause
        /// the application to use Software Rendering (CPU).
        /// 
        /// You can also verify if hardware rendering is being done by
        /// using the WPF Performance Profiling Tools from Microsoft.
        /// Check the "Draw software rendering with purple tint" checkbox.
        /// If anything shows up purple on the application, you then know
        /// that the application is using software rendering.
        /// http://msdn.microsoft.com/en-us/library/aa969767(v=vs.90)
        /// </summary>
        private string _RenderingMode;
        /// <summary>
        /// The rendering mode of the application based off the
        /// settings found on the system and application.  The
        /// rendering modes are either Software Rendering or
        /// Hardware Rendering.  Hardware Rendering is preferred
        /// because it used the GPU to do the rendering.  But
        /// system settings or hardware limitations can cause
        /// the application to use Software Rendering (CPU).
        /// 
        /// You can also verify if hardware rendering is being done by
        /// using the WPF Performance Profiling Tools from Microsoft.
        /// Check the "Draw software rendering with purple tint" checkbox.
        /// If anything shows up purple on the application, you then know
        /// that the application is using software rendering.
        /// http://msdn.microsoft.com/en-us/library/aa969767(v=vs.90)
        /// </summary>
        public string RenderingMode
        {
            get { return _RenderingMode; }
            set
            {
                _RenderingMode = value;
                this.NotifyOfPropertyChange(() => this.RenderingMode);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Load all the licenses for the projects used in Pulse.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> LoadLicensesCommand { get; protected set; }

        /// <summary>
        /// Load the End User Rights for Pulse.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> LoadEndUserRightsCommand { get; protected set; }

        /// <summary>
        /// Check if the application is using software rendering.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> RenderCheckCommand { get; protected set; }

        /// <summary>
        /// Load the error log.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> LoadErrorLogCommand { get; protected set; }

        /// <summary>
        /// Clear the error log.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ClearErrorLogCommand { get; protected set; }

        /// <summary>
        /// Open User Guide.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> OpenUserGuideCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public AboutViewModel()
        {
            PulseVersion = Pulse.Version.VERSION + Pulse.Version.VERSION_ADDITIONAL;
            PulseDisplayVersion = PulseDisplay.Version.VERSION + Pulse.Version.VERSION_ADDITIONAL;
            RtiVersion = Core.Commons.VERSION + Core.Commons.RTI_VERSION_ADDITIONAL;

            IsCheckingForUpdates = false;
            PulseVersionUpdateToDate = "Checking for an update...";
            PulseUpdateUrl = "";

            // Get the copyright info
            GetCopyrightInfo();

            // Load the license
            LoadLicensesCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => LoadLicenses()));

            // Load the license
            LoadEndUserRightsCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => LoadEndUserRights()));

            // Check the rendering options
            RenderCheckCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => CheckForSoftwareRendering()));

            // Load the error log
            LoadErrorLogCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => LoadErrorLog()));

            // Load the error log
            ClearErrorLogCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ClearErrorLog()));

            // Open User Guide
            OpenUserGuideCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => OpenUserGuide()));

            // Check for updates to the applications
            CheckForUpdates();
        }

        /// <summary>
        /// Shutdown this viewmodel.
        /// </summary>
        public void Dispose()
        {
            AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
        }

        #region License

        /// <summary>
        /// Load the file with the Licenses.
        /// </summary>
        private void LoadLicenses()
        {
            PulseInfo = File.ReadAllText(PULSE_LICENSES_PATH);
        }

        #endregion

        #region End User Rights

        /// <summary>
        /// Load the file for the End User Rights.
        /// </summary>
        private void LoadEndUserRights()
        {
            PulseInfo = File.ReadAllText(PULSE_END_USER_RIGHTS_PATH);
        }

        #endregion

        #region  Copyright

        /// <summary>
        /// Load the file for the Copyright.
        /// </summary>
        private void GetCopyrightInfo()
        {
            Copyright = File.ReadAllText(PULSE_COPY_RIGHT_PATH);
        }

        #endregion

        #region Auto Update

        /// <summary>
        /// Check for updates to the application.  This will download the version of the application from 
        /// website/pulse/Pulse_AppCast.xml.  It will then check the version against the verison of this application
        /// set in Properties->AssemblyInfo.cs.  If the one on the website is greater, it will display a message 
        /// to update the application.
        /// 
        /// Also subscribe to the event to determine if an update is necssary.
        /// </summary>
        private void CheckForUpdates()
        {
            string url = @"http://www.rowetechinc.com/swfw/latest/Pulse/Pulse_AppCast.xml";

            try
            {
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response != null && response.StatusCode == HttpStatusCode.OK && response.ResponseUri == new System.Uri(url))
                {
                    IsCheckingForUpdates = true;
                    AutoUpdater.Start(url);
                    AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                }
                response.Close();
            }
            catch(System.Net.WebException e)
            {
                // No Internet connection, so do nothing
                log.Error("No Internet connection to check for updates.", e);
            }
            catch(Exception e)
            {
                log.Error("Error checking for an update on the web.", e);
            }
        }

        /// <summary>
        /// Event handler for the AutoUpdater.   This will get if an update is available
        /// and if so, which version is available.
        /// </summary>
        /// <param name="args">Results for checking if an update exist.</param>
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (!args.IsUpdateAvailable)
                {
                    PulseVersionUpdateToDate = string.Format("Pulse is up to date");
                    PulseUpdateUrl = "";
                }
                else
                {
                    PulseVersionUpdateToDate = string.Format("Pulse version {0} is available", args.CurrentVersion);
                    PulseUpdateUrl = args.DownloadURL;
                }
                // Unsubscribe
                AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                IsCheckingForUpdates = false;


                if (args.IsUpdateAvailable)
                {
                    MessageBoxResult dialogResult;
                    if (args.Mandatory)
                    {
                        dialogResult =
                            MessageBox.Show(@"There is new version " + args.CurrentVersion + "  available. \nYou are using version " + args.InstalledVersion + ". \nThis is required update. \nPress Ok to begin updating the application.",
                                            @"Update Available",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                    }
                    else
                    {
                        dialogResult =
                            MessageBox.Show(
                                @"There is new version " + args.CurrentVersion + " available. \nYou are using version " + args.InstalledVersion + ".  \nDo you want to update the application now?",
                                @"Update Available",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Information);
                    }

                    if (dialogResult.Equals(MessageBoxResult.Yes))
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate())
                            {
                                //Application.Current.Exit();
                                System.Windows.Application.Current.Shutdown();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, 
                                exception.GetType().ToString(), 
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    //MessageBox.Show(@"There is no update available please try again later.", 
                    //                @"No update available",
                    //                MessageBoxButton.OK,
                    //                MessageBoxImage.Information);
                }
            }
            else
            {
                //MessageBox.Show(
                //        @"There is a problem reaching update server please check your internet connection and try again later.",
                //        @"Update check failed", 
                //        MessageBoxButton.OK,
                //        MessageBoxImage.Error);
            }
        }

        #endregion

        #region Software Rendering Check

        /// <summary>
        /// Check if software rendering is turned on in anyway.  There are many ways the
        /// application can be forced into software rendering.  These are some of the checks
        /// that can be done to verify if it is in software rendering mode.
        /// 
        /// 
        /// http://blogs.msdn.com/b/jgoldb/archive/2010/06/22/software-rendering-usage-in-wpf.aspx
        /// 
        /// Render Tier
        /// Rendering Tier 0 No graphics hardware acceleration. All graphics features use software acceleration. The DirectX version level is less than version 9.0.
        /// Rendering Tier 1 Some graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// Rendering Tier 2 Most graphics features use graphics hardware acceleration. The DirectX version level is greater than or equal to version 9.0.
        /// 
        /// Process Render Mode
        /// If the RenderMode is set to software only, then it will render in software only.
        /// 
        /// Registery Set to Disable Hardware Rendering
        /// HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics\DisableHWAcceleration != 0
        /// If the value is not set to 0, then hardware rendering is disabled.
        /// 
        /// </summary>
        private void CheckForSoftwareRendering()
        {
            int NO_HWDR_RENDERING = -1;

            // Render Tier
            // Anything less than 2 may cause software rendering
            RenderTier = System.Windows.Media.RenderCapability.Tier >> 16;

            // Process Render Mode
            ProcessRenderMode = System.Windows.Media.RenderOptions.ProcessRenderMode.ToString();

            // Registry key for Hardware Rendering Disabled is turned on or off
            // 0 means it is not disabled and will use Hardware Rendering if it can
            // HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics\DisableHWAcceleration
            object value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics", "DisableHWAcceleration", NO_HWDR_RENDERING);

            // Value is null if the key does not exist
            // Value is less than 0 if there is NO hardware rendering
            if (value == null || (int)value < 0)
            {
                RegKeyHwdrRenderDisabled = NO_HWDR_RENDERING;
            }
            else
            {
                RegKeyHwdrRenderDisabled = (int)value;
            }

            // Check the values to see if any caused software rendering
            if (RenderTier != 2 ||
                System.Windows.Media.RenderOptions.ProcessRenderMode == System.Windows.Interop.RenderMode.SoftwareOnly ||
                RegKeyHwdrRenderDisabled != 0)
            {
                RenderingMode = "SOFTWARE Rendering";
            }
            else
            {
                RenderingMode = "HARDWARE Rendering";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Render Mode: \t\t{0}", RenderingMode));
            
            if (RenderTier != 2)
            {
                sb.AppendLine(string.Format("Render Tier: \t\t{0} \t[SOFTWARE]", RenderTier));
            }
            else
            {
                sb.AppendLine(string.Format("Render Tier: \t\t{0} \t[HARDWARE]", RenderTier));
            }

            if (System.Windows.Media.RenderOptions.ProcessRenderMode == System.Windows.Interop.RenderMode.SoftwareOnly)
            {
                sb.AppendLine(string.Format("ProcessRenderMode: \t{0} \t[SOFTWARE]", ProcessRenderMode));
            }
            else
            {
                sb.AppendLine(string.Format("ProcessRenderMode: \t{0} \t[HARDWARE]", ProcessRenderMode));
            }

            if (RegKeyHwdrRenderDisabled != 0)
            {
                sb.AppendLine(string.Format("Registry Hdwr Disable: \t{0} \t[SOFTWARE]", RegKeyHwdrRenderDisabled));
            }
            else
            {
                sb.AppendLine(string.Format("Registry Hdwr Disable: \t{0} \t[HARDWARE]", RegKeyHwdrRenderDisabled));
            }

            // Set the info
            PulseInfo = sb.ToString();
        }

        #endregion

        #region Error Log

        /// <summary>
        /// Read the error log.
        /// </summary>
        private void LoadErrorLog()
        {
            PulseInfo = File.ReadAllText(Pulse.Commons.GetErrorLogPath());
        }

        /// <summary>
        /// Clear the error log.
        /// </summary>
        private void ClearErrorLog()
        {
            try
            {
                using (FileStream stream = new FileStream(Pulse.Commons.GetErrorLogPath(), FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("");
                    }
                }

                // Load the cleared error log
                LoadErrorLog();
            }
            catch (Exception e)
            {
                log.Error("Error clearing the ErrorLog.", e);
            }
        }

        #endregion

        #region User Guide

        /// <summary>
        /// Open the user guide for user to view.
        /// When running this in Visual Studio, you must ensure you put
        /// the user guide in the same place as Pulse.exe.  It will look for
        /// the user guide there.
        /// </summary>
        private void OpenUserGuide()
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string userGuide = "RTI - Pulse User Guide.pdf";

            try
            {
                Process proc = new Process();
                proc.StartInfo = new ProcessStartInfo()
                {
                    FileName = basePath + "\\" + userGuide
                };
                proc.Start();
            }
            catch (Exception e)
            {
                log.Error("Error loading the Pulse User Guide.  " + basePath + "\\" + userGuide, e);
            }
        }

        #endregion

    }
}
