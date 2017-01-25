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
 * 10/03/2013      RC          3.2.0      Initial coding
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 09/17/2014      RC          4.1.0      Added DvlSetup.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ReactiveUI;
    using Caliburn.Micro;
    using AutoUpdaterDotNET;

    /// <summary>
    /// Home page for the application.
    /// This will be the place where the user decides what
    /// they want to do with the softare.
    /// </summary>
    public class HomeViewModel : PulseViewModel
    {
        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        #endregion

        #region Commands

        /// <summary>
        /// Command to start pulse in acquire mode.
        /// </summary>
        public ReactiveCommand<object> AcquireModeCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Playback mode.
        /// </summary>
        public ReactiveCommand<object> PlaybackModeCommand { get; protected set; }

        /// <summary>
        /// Command to to the Adcp Test.
        /// </summary>
        public ReactiveCommand<object> AdcpUtilitiesCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Data Export mode.
        /// </summary>
        public ReactiveCommand<object> DataExportCommand { get; protected set; }

        /// <summary>
        /// Command to go to the DVL Setup.
        /// </summary>
        public ReactiveCommand<object> DvlSetupCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Waves.
        /// </summary>
        public ReactiveCommand<object> WavesCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Prediction Model.
        /// </summary>
        public ReactiveCommand<object> PredictionModelCommand { get; protected set; }

        #endregion

        #region Properties

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
            IsCheckingForUpdates = true;
            //AutoUpdater.Start("http://66.147.244.164/~rowetech/pulse/Pulse_AppCast.xml");
            //AutoUpdater.Start("http://www.rowetechinc.com/pulse/Pulse_AppCast.xml");
            AutoUpdater.Start("http://www.rowetechinc.co/pulse/Pulse_AppCast.xml");
            AutoUpdater.CheckForUpdateEvent += new AutoUpdater.CheckForUpdateEventHandler(AutoUpdater_AutoUpdaterEvent);
        }

        /// <summary>
        /// Event handler for the AutoUpdater.   This will get if an update is available
        /// and if so, which version is available.
        /// </summary>
        /// <param name="args">Results for checking if an update exist.</param>
        void AutoUpdater_AutoUpdaterEvent(UpdateInfoEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            if (!args.IsUpdateAvailable)
            {
                PulseVersionUpdateToDate = string.Format("Pulse is up to date");
            }
            else
            {
                PulseVersionUpdateToDate = string.Format("Pulse version {0} is available", args.CurrentVersion);
            }

            // Unsubscribe
            AutoUpdater.CheckForUpdateEvent -= AutoUpdater_AutoUpdaterEvent;
            IsCheckingForUpdates = false;
        }

        #endregion


        /// <summary>
        /// Initialize the values.
        /// </summary>
        public HomeViewModel()
            : base("Home")
        {

            PulseVersion = Pulse.Version.VERSION + Pulse.Version.VERSION_ADDITIONAL;
            PulseDisplayVersion = PulseDisplay.Version.VERSION + Pulse.Version.VERSION_ADDITIONAL;
            RtiVersion = Core.Commons.VERSION + Core.Commons.RTI_VERSION_ADDITIONAL;
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();

            // Load all previous Pulse options
            LoadPreviousPulseOptions();

            // Acquire Mode command
            AcquireModeCommand = ReactiveCommand.Create();
            AcquireModeCommand.Subscribe(_ => AcquireMode());

            // Playback Mode command
            PlaybackModeCommand = ReactiveCommand.Create();
            PlaybackModeCommand.Subscribe(_ => PlaybackMode());

            // Adcp Test command
            AdcpUtilitiesCommand = ReactiveCommand.Create();
            AdcpUtilitiesCommand.Subscribe(_ => AdcpUtilities());

            // Data Export command
            DataExportCommand = ReactiveCommand.Create();
            DataExportCommand.Subscribe(_ => DataExport());

            // Data Export command
            DvlSetupCommand = ReactiveCommand.Create();
            DvlSetupCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DvlSetupView)));

            // Waves command
            WavesCommand = ReactiveCommand.Create();
            WavesCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.WavesView)));

            // Prediction Model command
            PredictionModelCommand = ReactiveCommand.Create();
            PredictionModelCommand.Subscribe(_ => PredictionModel());

            CheckForUpdates();
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            AutoUpdater.CheckForUpdateEvent -= AutoUpdater_AutoUpdaterEvent;
        }

        #region Load Pulse Options

        private void LoadPreviousPulseOptions()
        {

        }

        #endregion

        #region Acquire Mode

        /// <summary>
        /// Start pulse in Acquire mode.
        /// This mode is used to start receiving data
        /// from the ADCP.
        /// </summary>
        private void AcquireMode()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ProjectView));
        }

        #endregion

        #region Playback Mode

        /// <summary>
        /// Go to the Playback view.
        /// </summary>
        private void PlaybackMode()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SelectPlaybackView));
        }

        #endregion

        #region Adcp Test

        /// <summary>
        /// Go to the Adcp Test view.
        /// </summary>
        private void AdcpUtilities()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpUtilitiesView));
        }

        #endregion

        #region Data Export

        /// <summary>
        /// Go to the Data Export view.
        /// </summary>
        private void DataExport()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ExportDataView));
        }

        #endregion

        #region Prediction Model

        /// <summary>
        /// Go to the Prediction Model view.
        /// </summary>
        private void PredictionModel()
        {
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpPredictionModelView));
        }

        #endregion

    }
}
