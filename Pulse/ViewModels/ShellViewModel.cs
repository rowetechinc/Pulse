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
 * 04/23/2013      RC          3.0.0       Initial coding
 * 07/10/2013      RC          3.0.3       Added ErrorLog setup. 
 * 07/11/2013      RC          3.0.4       Added CompassCalView to ViewNavEvent.
 * 07/29/2013      RC          3.0.6       Added DownloadDataView to ViewNavEvent.
 * 07/30/2013      RC          3.0.6       Added UpdateFirmwareViewModel to ViewNavEvent.
 * 08/26/2013      RC          3.0.8       Added ExportDataViewModel to ViewNavEvent.
 * 12/02/2013      RC          3.2.0       Added DeactivateItem(ActiveItem, true) to Deactivate to ensure the last activated item gets deactivated.
 * 02/12/2014      RC          3.2.3       Added VesselMountViewModel.
 * 07/29/2014      RC          3.4.0       Fixed a bug with shutdown the application and put the AdcpConnection as the last singleton to shutdown.
 * 08/07/2014      RC          4.0.0       Updated ReactiveCommand to 6.0.
 * 08/20/2014      RC          4.0.1       Check for updates on startup.
 * 09/17/2014      RC          4.1.0       Added DVL Setup.
 * 10/02/2014      RC          4.1.0       Added RtiCompassCalView.
 * 07/09/2015      RC          0.0.5       Added Environment.Exit(Environment.ExitCode) to shutdown all threads.
 * 08/28/2017      RC          4.5.2       Added DataOutputView.
 * 
 */

using System.ComponentModel.Composition;
using System.Diagnostics;
using Caliburn.Micro;
using ReactiveUI;

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using log4net.Repository.Hierarchy;
    using log4net.Appender;
    using log4net.Layout;
    using System.IO;
    using AutoUpdaterDotNET;

    /// <summary>
    /// Interface for the ShellViewModel.
    /// Empty.
    /// </summary>
    public interface IShellViewModel
    {
    }


    /// <summary>
    /// Create the shell for the application.  This is
    /// the top level of the application.
    /// 
    /// Set ActiveItem to the view that you want to display.
    /// This will use IHandle from the EventAgrregator to 
    /// receive all events with a ViewNavEvent.  These
    /// events are published to change the view.  Set 
    /// ActiveItem to the view you based off the event
    /// parameters.
    /// </summary>
    public class ShellViewModel : Conductor<object>, IShellViewModel, IDeactivate, IHandle<ViewNavEvent>
    {
        #region Variables

        /// <summary>
        /// Event Aggregator.
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Used for a back button.
        /// This will allow the user to navigate
        /// back.  It will keep track of all the page visits.
        /// A limit will be set number of view events it will store.
        /// </summary>
        private Stack<ViewNavEvent> _backStack;

        /// <summary>
        /// Store the _prevViewNavEvent so that the Back Stack
        /// gets the latest view to go back to.
        /// </summary>
        private ViewNavEvent _prevViewNavEvent;

        #endregion

        #region Properties

        #region Navigation Bar

        ///// <summary>
        ///// Only one property can be ActiveItem.  This will
        ///// be a second section withing the display for navigation
        ///// purposes.
        ///// </summary>
        //private Screen _navigationBar;
        ///// <summary>
        ///// Only one property can be ActiveItem.  This will
        ///// be a second section withing the display for navigation
        ///// purposes.
        ///// </summary>
        //public Screen NavigationBar
        //{
        //    get { return _navigationBar; }
        //    set
        //    {
        //        _navigationBar = value;
        //        NotifyOfPropertyChange(() => NavigationBar);
        //    }
        //}

        /// <summary>
        /// Playback View Model.
        /// </summary>
        public NavBarViewModel NavBarVM { get; set; }

        /// <summary>
        /// Set flag if the navigation bar should be visible.
        /// </summary>
        private bool _IsNavBarEnabled;
        /// <summary>
        /// Set flag if the navigation bar should be visible.
        /// </summary>
        public bool IsNavBarEnabled
        {
            get { return _IsNavBarEnabled; }
            set
            {
                _IsNavBarEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsNavBarEnabled);
            }
        }

        #endregion

        #region Playback

        /// <summary>
        /// Playback View Model.
        /// </summary>
        public PlaybackViewModel PlaybackVM { get; set; }

        /// <summary>
        /// Set flag if the playback controls should be visible.
        /// </summary>
        private bool _IsPlaybackEnabled;
        /// <summary>
        /// Set flag if the playback controls should be visible.
        /// </summary>
        public bool IsPlaybackEnabled
        {
            get { return _IsPlaybackEnabled; }
            set
            {
                _IsPlaybackEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsPlaybackEnabled);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to go to the home view.
        /// </summary>
        public ReactiveCommand<object> HomeViewCommand { get; protected set; }

        /// <summary>
        /// Command to go to the terminal view.
        /// </summary>
        public ReactiveCommand<object> TerminalViewCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Settings view.
        /// </summary>
        public ReactiveCommand<object> SettingsViewCommand { get; protected set; }

        /// <summary>
        /// Command to go back in the application.
        /// </summary>
        public ReactiveCommand<object> BackCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Validation Test view.
        /// </summary>
        public ReactiveCommand<object> ValidationTestViewCommand { get; protected set; }

        /// <summary>
        /// Command to go to the About Test view.
        /// </summary>
        public ReactiveCommand<object> AboutViewCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Load the home view.
        /// </summary>
        public ShellViewModel(IEventAggregator events)
        {
            Debug.Print("Shell Open");
            // To set the Window title
            // http://stackoverflow.com/questions/4615467/problem-with-binding-title-of-wpf-window-on-property-in-shell-view-model-class
            base.DisplayName = "Pulse";

            // Initialize the values
            _events = events;
            events.Subscribe(this);

            // Setup ErrorLog
            SetupErrorLog();

            // Set a size of 10 views
            _backStack = new Stack<ViewNavEvent>(20);
            _prevViewNavEvent = null;

            // Set the Navigation bar viewmodel
            NavBarVM = IoC.Get<NavBarViewModel>();
            IsNavBarEnabled = false;

            // Set the Playback viewmodel
            PlaybackVM = IoC.Get<PlaybackViewModel>();
            IsPlaybackEnabled = false;

            // Command to view the Home view
            HomeViewCommand = ReactiveCommand.Create();
            HomeViewCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Command to view the Terimal view
            TerminalViewCommand = ReactiveCommand.Create();
            TerminalViewCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.TerminalView)));

            // Command to view the Settings view
            SettingsViewCommand = ReactiveCommand.Create();
            SettingsViewCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SettingsView)));

            // Command to view the Settings view
            AboutViewCommand = ReactiveCommand.Create();
            AboutViewCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AboutView)));

            // Command to view the Settings view
            ValidationTestViewCommand = ReactiveCommand.Create();
            ValidationTestViewCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ValidationTestView)));

            // Command to go back a view
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Display the HomeViewModel
            Handle(new ViewNavEvent(ViewNavEvent.ViewId.HomeView));

            // Check for updates to the applications
            CheckForUpdates();
            Debug.Print("Shell Complete");
        }

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
            //AutoUpdater.Start("http://66.147.244.164/~rowetech/pulse/Pulse_AppCast.xml");
            AutoUpdater.Start("http://www.rowetechinc.com/pulse/Pulse_AppCast.xml");
        }

        #endregion

        #region Error Logger

        /// <summary>
        /// Setup the error log.
        /// </summary>
        private void SetupErrorLog()
        {
            Hierarchy hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            //RollingFileAppender rollingFileAppender = new RollingFileAppender();
            //rollingFileAppender.AppendToFile = true;
            //rollingFileAppender.LockingModel = new FileAppender.MinimalLock();
            //rollingFileAppender.File = Pulse.Commons.GetErrorLogPath();
            //rollingFileAppender.MaxFileSize = 1048576 * 1;  // 1mb
            //rollingFileAppender.MaxSizeRollBackups = 1;
            //rollingFileAppender.StaticLogFileName = true;
            //PatternLayout pl = new PatternLayout();
            //pl.ConversionPattern = "%-5level %date [%thread] %-22.22c{1} - %m%n";
            //pl.ActivateOptions();
            //rollingFileAppender.Layout = pl;
            //rollingFileAppender.ActivateOptions();
            //log4net.Config.BasicConfigurator.Configure(rollingFileAppender);

            FileAppender fileAppender = new FileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = Pulse.Commons.GetErrorLogPath();
            PatternLayout pl = new PatternLayout();
            string pulseVer = Pulse.Version.VERSION + Pulse.Version.VERSION_ADDITIONAL;
            string pulseDisplayVer = PulseDisplay.Version.VERSION + PulseDisplay.Version.VERSION_ADDITIONAL;
            string rtiVer = Core.Commons.VERSION + Core.Commons.RTI_VERSION_ADDITIONAL;
            pl.ConversionPattern = "%d [%2%t] %-5p [%-10c] Pulse:" + pulseVer + " RTI:" + rtiVer + "   %m%n%n";
            pl.ActivateOptions();

            // If not Admin
            // Only log Error and Fatal errors
            if (!Pulse.Commons.IsAdmin())
            {
                fileAppender.AddFilter(new log4net.Filter.LevelMatchFilter() { LevelToMatch = log4net.Core.Level.Error });          // Log Error
                fileAppender.AddFilter(new log4net.Filter.LevelMatchFilter() { LevelToMatch = log4net.Core.Level.Fatal });          // Log Fatal
                fileAppender.AddFilter(new log4net.Filter.DenyAllFilter());                                                         // Reject all other errors
            }

            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(fileAppender);
        }

        /// <summary>
        /// Clear the Error Log.
        /// </summary>
        public void ClearErrorLog()
        {
            using (FileStream stream = new FileStream(Pulse.Commons.GetErrorLogPath(), FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("");
                }
            }
        }

        #endregion

        #region Deactivate

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        /// <param name="close"></param>
        void IDeactivate.Deactivate(bool close)
        {
            // Shutdown the pulse manager
            PulseManager pm = IoC.Get<PulseManager>();
            if (pm != null)
            {
                pm.Dispose();
            }

            // Shutdown the singleton HomeViewModels
            HomeViewModel homeVM = IoC.Get<HomeViewModel>();
            if (homeVM != null)
            {
                homeVM.Dispose();
            }

            // Shutdown the singleton ViewDataBaseViewModel
            ViewDataBaseViewModel viewDataVM = IoC.Get<ViewDataBaseViewModel>();
            if (viewDataVM != null)
            {
                viewDataVM.Dispose();
            }

            // Shutdown the singleton PlaybackViewModel
            PlaybackViewModel playbackVM = IoC.Get<PlaybackViewModel>();
            if (playbackVM != null)
            {
                playbackVM.Dispose();
            }

            // Shutdown the singleton ValidationTestViewModel
            ValidationTestViewModel vtVM = IoC.Get<ValidationTestViewModel>();
            if (vtVM != null)
            {
                vtVM.Dispose();
            }

            // Shutdown the singleton CompassCalViewModel
            CompassCalViewModel ccVM = IoC.Get<CompassCalViewModel>();
            if (ccVM != null)
            {
                ccVM.Dispose();
            }

            // Shutdown the singleton ViewDataBaseGraphicalViewModel
            ViewDataBaseGraphicalViewModel vdGraph = IoC.Get<ViewDataBaseGraphicalViewModel>();
            if (vdGraph != null)
            {
                vdGraph.Dispose();
            }

            // Shutdown the singleton ViewDataBaseTextViewModel
            ViewDataBaseTextViewModel vdText = IoC.Get<ViewDataBaseTextViewModel>();
            if (vdText != null)
            {
                vdText.Dispose();
            }

            // Shutdown the singleton ScreenDataBaseViewModel
            ScreenDataBaseViewModel sdVM = IoC.Get<ScreenDataBaseViewModel>();
            if (sdVM != null)
            {
                sdVM.Dispose();
            }

            // Shutdown the singleton AveragingBaseViewModel
            AveragingBaseViewModel abVM = IoC.Get<AveragingBaseViewModel>();
            if (abVM != null)
            {
                abVM.Dispose();
            }

            // Shutdown the singleton ValidationTestBaseViewModel
            ValidationTestBaseViewModel vtbVM = IoC.Get<ValidationTestBaseViewModel>();
            if (vtbVM != null)
            {
                vtbVM.Dispose();
            }

            // Shutdown the singleton DownloadDataViewModel
            DownloadDataViewModel ddVM = IoC.Get<DownloadDataViewModel>();
            if (ddVM != null)
            {
                ddVM.Dispose();
            }

            // Shutdown the singleton UpdateFirmwareViewModel
            UpdateFirmwareViewModel ufVM = IoC.Get<UpdateFirmwareViewModel>();
            if (ufVM != null)
            {
                ufVM.Dispose();
            }

            // Shutdown the singleton CompassUtilityViewModel
            CompassUtilityViewModel cuVM = IoC.Get<CompassUtilityViewModel>();
            if (cuVM != null)
            {
                cuVM.Dispose();
            }

            // Shutdown the singleton RtiCompassCalViewModel
            RtiCompassCalViewModel rtiCcVM = IoC.Get<RtiCompassCalViewModel>();
            if (rtiCcVM != null)
            {
                rtiCcVM.Dispose();
            }

            //// Shutdown the singleton TerminalViewModel
            //TerminalViewModel termVM = IoC.Get<TerminalViewModel>();
            //if (termVM != null)
            //{
            //    termVM.Dispose();
            //}

            //// Shutdown the singleton TerminalAdcpViewModel
            //TerminalAdcpViewModel tadcpVM = IoC.Get<TerminalAdcpViewModel>();
            //if (tadcpVM != null)
            //{
            //    tadcpVM.Dispose();
            //}

            //// Shutdown the singleton TerminalNavViewModel
            //TerminalNavViewModel tnavVM = IoC.Get<TerminalNavViewModel>();
            //if (tnavVM != null)
            //{
            //    tnavVM.Dispose();
            //}

            // Shutdown the singleton AboutViewModel
            AboutViewModel aboutVM = IoC.Get<AboutViewModel>();
            if (aboutVM != null)
            {
                aboutVM.Dispose();
            }

            // Shutdown the singleton VesselMountViewModel
            VesselMountViewModel vmVM = IoC.Get<VesselMountViewModel>();
            if (vmVM != null)
            {
                vmVM.Dispose();
            }

            // Shutdown the singleton VesselMountViewModel
            DataOutputViewModel vmDO = IoC.Get<DataOutputViewModel>();
            if (vmDO != null)
            {
                vmDO.Dispose();
            }

            // Shutdown the singleton VesselMountViewModel
            WpMagDirOutputViewModel vmWMD = IoC.Get<WpMagDirOutputViewModel>();
            if (vmWMD != null)
            {
                vmWMD.Dispose();
            }

            // Shutdown the last active item
            DeactivateItem(ActiveItem, true);

            // MAKE THIS THE LAST THING TO SHUTDOWN
            // Shutdown the ADCP connection
            AdcpConnection adcp = IoC.Get<AdcpConnection>();
            if (adcp != null)
            {
                adcp.Dispose();
            }

            // Shutdown the applicaton and all the threads
            Environment.Exit(Environment.ExitCode);
        }

        #endregion

        #region ViewNavEvent

        /// <summary>
        /// Event handler for the ViewNavEvent.
        /// </summary>
        /// <param name="navEvent">Message for the event.</param>
        public void Handle(ViewNavEvent navEvent)
        {
            // Check if its a back event or a new view
            // to display
            if (navEvent.ID == ViewNavEvent.ViewId.Back)
            {
                if (_backStack.Count > 0)
                {
                    _prevViewNavEvent = _backStack.Pop();

                    NavigateToView(_prevViewNavEvent);
                }
            }
            else
            {
                // Show the view
                NavigateToView(navEvent);

                // Set the preview view to the back stack
                // and store the current view to put on the stack later next time
                if (_prevViewNavEvent != null)
                {
                    _backStack.Push(_prevViewNavEvent);
                }

                // Store the event if it is not back
                _prevViewNavEvent = navEvent;
            }

        }

        /// <summary>
        /// Navigate to the view based off the
        /// event given.
        /// </summary>
        /// <param name="message"></param>
        private void NavigateToView(ViewNavEvent message)
        {
            switch (message.ID)
            {
                case ViewNavEvent.ViewId.HomeView:
                    var vmHome = IoC.Get<HomeViewModel>();
                    //MenuLinks = vmHome.GetMenu();
                    ActivateItem(vmHome);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.AboutView:
                    var aboutHome = IoC.Get<AboutViewModel>();
                    ActivateItem(aboutHome);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.TerminalView:
                    var adcpConn = IoC.Get<AdcpConnection>();
                    ActivateItem(adcpConn.TerminalVM);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ViewDataView:
                    var vmViewData = IoC.Get<ViewDataBaseViewModel>();
                    ActivateItem(vmViewData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = true;
                    break;
                case ViewNavEvent.ViewId.ValidationTestView:
                    var vtData = IoC.Get<ValidationTestBaseViewModel>();
                    ActivateItem(vtData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = true;
                    break;
                case ViewNavEvent.ViewId.CompassCalView:
                    var ccData = IoC.Get<CompassCalViewModel>();
                    ActivateItem(ccData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ScreenDataView:
                    var sdData = IoC.Get<ScreenDataBaseViewModel>();
                    ActivateItem(sdData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.DownloadDataView:
                    var dlData = IoC.Get<DownloadDataViewModel>();
                    ActivateItem(dlData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.UpdateFirmwareView:
                    var ufData = IoC.Get<UpdateFirmwareViewModel>();
                    ActivateItem(ufData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.CompassUtilityView:
                    var cuData = IoC.Get<CompassUtilityViewModel>();
                    ActivateItem(cuData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ExportDataView:
                    var edData = IoC.Get<ExportDataViewModel>();
                    ActivateItem(edData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ModeView:
                    var modeData = IoC.Get<ModeViewModel>();
                    ActivateItem(modeData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ProjectView:
                    var prjData = IoC.Get<ProjectViewModel>();
                    ActivateItem(prjData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.NewProjectView:
                    var newPrjData = IoC.Get<NewProjectViewModel>();
                    ActivateItem(newPrjData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.CommunicationsView:
                    var commData = IoC.Get<CommunicationViewModel>();
                    ActivateItem(commData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.StorageView:
                    var strgData = IoC.Get<StorageViewModel>();
                    ActivateItem(strgData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.LoadProjectsView:
                    var loadPrjData = IoC.Get<LoadProjectsViewModel>();
                    ActivateItem(loadPrjData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.AdcpConfigurationView:
                    var adcpConfigData = IoC.Get<AdcpConfigurationViewModel>();
                    ActivateItem(adcpConfigData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.AveragingView:
                    var avgData = IoC.Get<AveragingBaseViewModel>();
                    ActivateItem(avgData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.BottomTrackOnView:
                    var btOnData = IoC.Get<BottomTrackOnViewModel>();
                    btOnData.SetConfigKey(message.Param);
                    ActivateItem(btOnData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.BinsView:
                    var binData = IoC.Get<BinsViewModel>();
                    binData.SetConfigKey(message.Param);
                    ActivateItem(binData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.BurstModeView:
                    var burstData = IoC.Get<BurstModeViewModel>();
                    burstData.SetConfigKey(message.Param);
                    ActivateItem(burstData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.PingTimingView:
                    var pingTimingData = IoC.Get<PingTimingViewModel>();
                    if (pingTimingData.SetConfigKey(message.Param))
                    {
                        ActivateItem(pingTimingData);
                    }
                    else
                    {
                        var adcpConfigData1 = IoC.Get<AdcpConfigurationViewModel>();
                        ActivateItem(adcpConfigData1);
                    }
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.FrequencyView:
                    var freqData = IoC.Get<FrequencyViewModel>();
                    ActivateItem(freqData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.BroadbandModeView:
                    var bbData = IoC.Get<BroadbandModeViewModel>();
                    bbData.SetConfigKey(message.Param);
                    ActivateItem(bbData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.NavSourcesView:
                    var navSrcData = IoC.Get<VesselMountViewModel>();
                    ActivateItem(navSrcData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.SaveAdcpConfigurationView:
                    var saveAdcpConfigData = IoC.Get<SaveAdcpConfigurationViewModel>();
                    ActivateItem(saveAdcpConfigData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.SalinityView:
                    var salData = IoC.Get<SalinityViewModel>();
                    ActivateItem(salData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.TimeView:
                    var timeData = IoC.Get<TimeViewModel>();
                    ActivateItem(timeData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.EnsembleIntervalView:
                    var ceiData = IoC.Get<EnsembleIntervalViewModel>();
                    ActivateItem(ceiData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.SimpleCompassCalView:
                    var simpleCCData = IoC.Get<SimpleCompassCalViewModel>();
                    ActivateItem(simpleCCData);
                    IsNavBarEnabled = false;
                    bool? flag = message.Param as bool?;
                    if (flag.HasValue)
                    {
                        IsNavBarEnabled = flag.Value;
                    }
                    else
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.SimpleCompassCalWizardView:
                    var simpleCCWizData = IoC.Get<SimpleCompassCalWizardViewModel>();
                    ActivateItem(simpleCCWizData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ZeroPressureSensorView:
                    var zeroPSData = IoC.Get<ZeroPressureSensorViewModel>();
                    ActivateItem(zeroPSData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.DeployAdcpView:
                    var deployAdcpData = IoC.Get<DeployAdcpViewModel>();
                    ActivateItem(deployAdcpData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.ScanAdcpView:
                    var scanAdcpData = IoC.Get<ScanAdcpViewModel>();
                    ActivateItem(scanAdcpData);
                    IsNavBarEnabled = false;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.AdcpUtilitiesView:
                    var adcpUtilData = IoC.Get<AdcpUtilitiesViewModel>();
                    ActivateItem(adcpUtilData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.AdcpPredictionModelView:
                    var predData = IoC.Get<AdcpPredictionModelViewModel>();
                    ActivateItem(predData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.VesselMountOptionsView:
                    var vmData = IoC.Get<VesselMountViewModel>();
                    ActivateItem(vmData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.SelectPlaybackView:
                    var selPlaybackData = IoC.Get<SelectPlaybackViewModel>();
                    ActivateItem(selPlaybackData);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.DvlSetupView:
                    var dvlSetup = IoC.Get<DvlSetupViewModel>();
                    ActivateItem(dvlSetup);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.RtiCompassCalView:
                    var rtiCompassCal = IoC.Get<RtiCompassCalViewModel>();
                    ActivateItem(rtiCompassCal);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.BackscatterView:
                    var backScat = IoC.Get<BackscatterViewModel>();
                    ActivateItem(backScat);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = true;
                    break;
                case ViewNavEvent.ViewId.DiagnosticView:
                    var diag = IoC.Get<DiagnosticsBaseViewModel>();
                    ActivateItem(diag);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = false;
                    break;
                case ViewNavEvent.ViewId.DataOutputView:
                    var dataOut = IoC.Get<BaseDataOutputViewModel>();
                    ActivateItem(dataOut);
                    IsNavBarEnabled = true;
                    IsPlaybackEnabled = true;
                    break;

                //case ViewNavEvent.ViewId.SettingsView:
                //    var vmSettings = IoC.Get<SettingsViewModel>();
                //    ActivateItem(vmSettings);
                //break;
                default:
                    break;
            }
        }

        #endregion

    }
}
