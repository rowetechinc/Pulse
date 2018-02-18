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
 * 11/13/2013      RC          3.2.0      Added DVL and VM modes.
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
    /// Select the mode of the ADCP.
    /// </summary>
    public class ModeViewModel : PulseViewModel, IDeactivate
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

        #region Classes  and Enum

        /// <summary>
        /// Class to hold the mode option.
        /// </summary>
        public class ModeOption
        {
            /// <summary>
            /// Mode type.
            /// </summary>
            public RTI.DeploymentOptions.AdcpDeploymentMode ModeType { get; set; }

            /// <summary>
            /// Set if this mode is selected.
            /// </summary>
            public bool IsSelected { get; set; }

            /// <summary>
            /// Title line 1 for this mode.
            /// </summary>
            public string TitleLine1 { get; set; }

            /// <summary>
            /// Title line 2 for this mode.
            /// </summary>
            public string TitleLine2 { get; set; }

            /// <summary>
            /// Title line 3 for this mode.
            /// </summary>
            public string TitleLine3 { get; set; }

            /// <summary>
            /// Image for the mode.
            /// </summary>
            public string Image { get; set; }

            /// <summary>
            /// Initialize the values.
            /// </summary>
            /// <param name="type">Mode type.</param>
            public ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode type)
            {
                ModeType = type;
                IsSelected = false;
                TitleLine1 = "";
                TitleLine2 = "";
                TitleLine3 = "";
                Image = "";
            }
        }

        #endregion

        #region Properties

        #region Modes

        /// <summary>
        /// List of all the mode options.
        /// </summary>
        public List<ModeOption> ModeOptionsList { get; set; }

        /// <summary>
        /// Selected Mode Option.
        /// </summary>
        private ModeOption _SelectedModeOption;
        /// <summary>
        /// Selected Mode Option.
        /// </summary>
        public ModeOption SelectedModeOption
        {
            get { return _SelectedModeOption; }
            set
            {
                _SelectedModeOption = value;
                this.NotifyOfPropertyChange(() => this.SelectedModeOption);

                // Set the value for the selected project
                SetSelectedModeOption(value);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Move to the next screen.
        /// System.Reactive.Unit is equavalient to null.
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
        /// Initialize the values.
        /// </summary>
        public ModeViewModel() 
            : base("Mode")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            ModeOptionsList = new List<ModeOption>();
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.DirectReading) { TitleLine1 = "Direct", TitleLine2 = "Reading\n  Mode", Image = "../Images/SeaProfiler.png" });
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.SelfContained) { TitleLine1 = "Self", TitleLine2 = "Contained\n    Mode", Image = "../Images/SeaWatch.png" });
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.Waves) { TitleLine1 = "Waves", TitleLine2 = "Mode", Image = "../Images/SeaWave.png" });
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.River) { TitleLine1 = "River", TitleLine2 = "Mode", Image = "../Images/RiverProfiler.png" });
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.Dvl) { TitleLine1 = "DVL", TitleLine2 = "Mode", Image = "../Images/SeaPilot.png" });
            ModeOptionsList.Add(new ModeOption(RTI.DeploymentOptions.AdcpDeploymentMode.VM) { TitleLine1 = "Vessel Mount", TitleLine2 = "Mode", Image = "../Images/SeaTrak.png" });

            // Get the Mode option
            GetSelectedModeOption();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.StorageView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            
        }

        #region Selected Mode Option

        /// <summary>
        /// Get the CERECORD value from the selected project.
        /// I can only know now if it is on or off.  This will not tell me
        /// if it is rivers or waves also.
        /// </summary>
        private void GetSelectedModeOption()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    switch (_pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode)
                    {
                        case DeploymentOptions.AdcpDeploymentMode.DirectReading:
                            SelectedModeOption = ModeOptionsList[0];
                            break;
                        case DeploymentOptions.AdcpDeploymentMode.SelfContained:
                            SelectedModeOption = ModeOptionsList[1];
                            break;
                        case DeploymentOptions.AdcpDeploymentMode.Waves:
                            SelectedModeOption = ModeOptionsList[2];
                            break;
                        case DeploymentOptions.AdcpDeploymentMode.River:
                            SelectedModeOption = ModeOptionsList[3];
                            break;
                        case DeploymentOptions.AdcpDeploymentMode.Dvl:
                            SelectedModeOption = ModeOptionsList[4];
                            break;
                        case DeploymentOptions.AdcpDeploymentMode.VM:
                            SelectedModeOption = ModeOptionsList[5];
                            break;
                        default:
                            break;
                    }

                    // Need more hints from the project to know if it is Waves or River
                }
            }
        }

        /// <summary>
        /// Determine what to do based off the selected mode option.
        /// </summary>
        /// <param name="option">Mode option selected.</param>
        private void SetSelectedModeOption(ModeOption option)
        {
            if (option != null)
            {
                switch (option.ModeType)
                {
                    case RTI.DeploymentOptions.AdcpDeploymentMode.DirectReading:
                        DirectReadingMode();
                        break;
                    case RTI.DeploymentOptions.AdcpDeploymentMode.SelfContained:
                        SelfContainedMode();
                        break;
                    case RTI.DeploymentOptions.AdcpDeploymentMode.Waves:
                        WavesMode();
                        break;
                    case RTI.DeploymentOptions.AdcpDeploymentMode.River:
                        RiverMode();
                        break;
                    case RTI.DeploymentOptions.AdcpDeploymentMode.Dvl:
                        DvlMode();
                        break;
                    case RTI.DeploymentOptions.AdcpDeploymentMode.VM:
                        VmMode();
                        break;
                    default:
                        break;
                }
            }

            if (_pm.IsProjectSelected)
            {
                // Save the options to the project
                _pm.SelectedProject.Save();
            }
        }

        #endregion

        #region Direct Reading Mode

        /// <summary>
        /// Go to the Direct reading mode.
        /// This will turn off CERECORD.
        /// </summary>
        private void DirectReadingMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing  = Commands.AdcpCommands.AdcpRecordOptions.Disable;            // Turn off recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.DirectReading;        // Set the mode to DR
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }

        #endregion

        #region Self Contained mode

        /// <summary>
        /// Go to the self contained mode.
        /// This will turn on CERECORD.
        /// </summary>
        private void SelfContainedMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing  = Commands.AdcpCommands.AdcpRecordOptions.Enable;             // Turn on recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.SelfContained;        // Set the mode to SC
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }

        #endregion

        #region Wave Mode

        /// <summary>
        /// Go to the waves mode.
        /// This will turn off CERECORD.
        /// </summary>
        private void WavesMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing = Commands.AdcpCommands.AdcpRecordOptions.Enable;              // Turn on recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.Waves;                // Set the mode to Waves
                    _pm.SelectedProject.Configuration.Commands.CWS = RTI.Commands.AdcpCommands.DEFAULT_SALINITY_VALUE_SALT;                         // Set the salinity to salt
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }

        #endregion

        #region River Mode

        /// <summary>
        /// Go to the river mode.
        /// This will turn off CERECORD.
        /// </summary>
        private void RiverMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing = Commands.AdcpCommands.AdcpRecordOptions.Disable;             // Turn off recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.River;                // Set the mode to River
                    _pm.SelectedProject.Configuration.Commands.CWS = RTI.Commands.AdcpCommands.DEFAULT_SALINITY_VALUE_FRESH;                        // Set the salinity to fresh
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }

        #endregion

        #region DVL mode

        /// <summary>
        /// Set the intial values for the DVL mode.
        /// </summary>
        private void DvlMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing = Commands.AdcpCommands.AdcpRecordOptions.Disable;             // Turn off recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.Dvl;                  // Set the mode to DVL
                    _pm.SelectedProject.Configuration.Commands.CWS = RTI.Commands.AdcpCommands.DEFAULT_SALINITY_VALUE_SALT;                         // Set the salinity to salt
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }


        #endregion

        #region VM Mode

        /// <summary>
        /// Set the intial values for the VM mode.
        /// </summary>
        private void VmMode()
        {
            // Check if the project is selected and a configuration exist
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing = Commands.AdcpCommands.AdcpRecordOptions.Disable;             // Turn off recording internally
                    _pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode = DeploymentOptions.AdcpDeploymentMode.VM;                   // Set the mode to VM
                    _pm.SelectedProject.Configuration.Commands.CWS = RTI.Commands.AdcpCommands.DEFAULT_SALINITY_VALUE_SALT;                         // Set the salinity to salt
                    _pm.SelectedProject.Save();                                                                                                     // Save the settings
                }
            }
        }

        #endregion

        #region IDeactivate

        event EventHandler<DeactivationEventArgs> IDeactivate.AttemptingDeactivation
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        void IDeactivate.Deactivate(bool close)
        {
            Dispose();
        }

        event EventHandler<DeactivationEventArgs> IDeactivate.Deactivated
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        #endregion
    }
}
