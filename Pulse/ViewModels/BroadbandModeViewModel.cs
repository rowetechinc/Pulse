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
 * 11/01/2013      RC          3.2.0      Initial coding
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;
    using ReactiveUI;

    /// <summary>
    /// Select the broadband mode for the ADCP subsystem configuration.
    /// </summary>
    public class BroadbandModeViewModel: PulseViewModel
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

        /// <summary>
        /// Pulse manager.
        /// </summary>
        private readonly PulseManager _pm;

        #endregion

        #region Class and Enums

        /// <summary>
        /// Broadband modes.
        /// </summary>
        public class BroadbandModes
        {
            /// <summary>
            /// Mode type.
            /// </summary>
            public Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType ModeType { get; set; }

            /// <summary>
            /// Title for the mode.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Description of the mode.
            /// </summary>
            public string Desc { get; set; }

            /// <summary>
            /// Initialize the object.
            /// </summary>
            /// <param name="modeType">Mode type.</param>
            /// <param name="title">Mode title.</param>
            /// <param name="desc">Descpition of the mode.</param>
            public BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType modeType, string title, string desc)
            {
                ModeType = modeType;
                Title = title;
                Desc = desc;
            }
        }


        #endregion

        #region Properties

        /// <summary>
        /// Adcp Subsystem Configuration key to use in the selected projects
        /// dictionary.
        /// </summary>
        public string ConfigKey { get; protected set; }

        /// <summary>
        /// A list of all the modes.
        /// </summary>
        public ReactiveList<BroadbandModes> ModeList { get; set; }

        /// <summary>
        /// Selected Broadband mode.
        /// </summary>
        private BroadbandModes _SelectedMode;
        /// <summary>
        /// Selected Broadband mode.
        /// </summary>
        public BroadbandModes SelectedMode
        {
            get { return _SelectedMode; }
            set
            {
                if (value == null)
                {
                    value = new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, "Broadband", "Coded Broadband");
                }

                // Set the value to the config
                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_TransmitPulseType = value.ModeType;
                    _pm.SelectedProject.Save();
                }

                _SelectedMode = value;
                this.NotifyOfPropertyChange(() => this.SelectedMode);
            }
        }

        /// <summary>
        /// Lag Length.
        /// </summary>
        private float _LagLength;
        /// <summary>
        /// Lag Length.
        /// </summary>
        public float LagLength
        {
            get { return _LagLength; }
            set
            {
                _LagLength = value;
                this.NotifyOfPropertyChange(() => this.LagLength);

                // Set the value to the config
                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_LagLength = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Move to the next screen.
        /// </summary>
        public ReactiveCommand<object> NextCommand { get; protected set; }

        /// <summary>
        /// Go back a screen.
        /// </summary>
        public ReactiveCommand<object> BackCommand { get; protected set; }

        /// <summary>
        /// Exit the wizard.
        /// </summary>
        public ReactiveCommand<object> ExitCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public BroadbandModeViewModel()
            :base("Broadband Mode ViewModel")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            ConfigKey = "";

            // Create the list
            CreateList();

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedMode,                                  // Ensure something is selected
                                                    x => x.Value != null));
            NextCommand.Subscribe(_ => OnNextCommand());

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));
        }


        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        /// <summary>
        /// Set the configuration key.
        /// </summary>
        /// <param name="key">Key to convert to a string.</param>
        public void SetConfigKey(object key)
        {
            string configKey = key as string;
            if (configKey != null)
            {
                ConfigKey = configKey;

                // Initialize the values
                InitializeSelectedMode();
                InitializeLagLength();
            }
        }

        #region Initialize

        /// <summary>
        /// Create the list and populate it.
        /// </summary>
        private void CreateList()
        {
            ModeList = new ReactiveList<BroadbandModes>();

            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND, "Narrowband", "Non-coded Narrowband"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, "Broadband", "Coded Broadband"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE, "Non-Coded Pulse-to-Pulse", "Narrowband Non-Coded Pulse-to-Pulse"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, "Broadband Pulse-to-Pulse", "Broadband Coded Pulse-to-Pulse"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_BROADBAND_PULSE_TO_PULSE, "Non-Coded Broadband Pulse-to-Pulse", "Non-Coded Broadband Pulse-to-Pulse"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_AMBIGUITY_RESOLVER, "Broadband Ambiguity Resolver", "Broadband Ambiguity Resolver"));
            ModeList.Add(new BroadbandModes(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_P2P_AMBIGUITY_RESOLVER, "Broadband Pulse-to-Pulse and Ambiguity Resolver", "Broadband Pulse-to-Pulse and Ambiguity Resolver"));

            //InitializeSelectedMode();
        }

        /// <summary>
        /// Chose the selected Mode based off the CWPBB command in the selected project.
        /// </summary>
        private void InitializeSelectedMode()
        {
            // Get the current value from the project
            RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType type = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_TransmitPulseType;

            foreach (BroadbandModes mode in ModeList)
            {
                if (mode.ModeType == type)
                {
                    SelectedMode = mode;
                }
            }
        }

        /// <summary>
        /// Initialize the lag length.
        /// </summary>
        private void InitializeLagLength()
        {
            LagLength = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_LagLength;
        }

        #endregion

        #region Next Command

        /// <summary>
        /// Save the selected Broadband mode.
        /// This move to the next view.
        /// </summary>
        private void OnNextCommand()
        {
            //// Set the value to the config
            //_pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_TransmitPulseType = SelectedMode.ModeType;
            //_pm.SelectedProject.Save();

            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BottomTrackOnView, ConfigKey));
        }

        #endregion
    }
}
