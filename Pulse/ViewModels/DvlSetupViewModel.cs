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
 * 09/17/2014      RC          4.1.0       Initial coding
 * 11/19/2015      RC          4.3.1       Added timer to update terminal display.
 * 
 */

using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{

    /// <summary>
    /// Setup a DVL.
    /// </summary>
    public class DvlSetupViewModel : PulseViewModel
    {
        #region Variable

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Connection to the ADCP.
        /// </summary>
        private AdcpConnection _adcpConn;

        /// <summary>
        /// Timer to reduce the number of update calls the terminal window.
        /// </summary>
        private System.Timers.Timer _displayTimer;

        #endregion

        #region Properties

        #region Subsystem

        /// <summary>
        /// List of all the DVL Subsystem Configurations.
        /// </summary>
        public ReactiveList<DvlSubsystemConfigurationViewModel> SubsystemConfigList { get; set; }

        /// <summary>
        /// List of all the subsystems with a description.
        /// Used to populate the combobox.
        /// </summary>
        public SubsystemList ListOfSubsystems { get; set; }

        /// <summary>
        /// Selected Subsystem from the combobox.
        /// </summary>
        private RTI.SubsystemList.SubsystemCodeDesc _selectedSubsystem;
        /// <summary>
        /// Selected Subsystem from the combobox.
        /// </summary>
        public RTI.SubsystemList.SubsystemCodeDesc SelectedSubsystem
        {
            get { return _selectedSubsystem; }
            set
            {
                _selectedSubsystem = value;
                this.NotifyOfPropertyChange(() => this.SelectedSubsystem);
            }
        }

        #endregion

        #region ADCP Configuration

        /// <summary>
        /// The ADCP Configuration.
        /// </summary>
        private AdcpConfiguration _AdcpConfig;
        /// <summary>
        /// The ADCP Configuration.
        /// </summary>
        public AdcpConfiguration AdcpConfig
        {
            get { return _AdcpConfig; }
            set
            {
                _AdcpConfig = value;
                this.NotifyOfPropertyChange(() => this.AdcpConfig);
            }
        }

        /// <summary>
        /// The ADCP command set.
        /// </summary>
        private string _AdcpCommandSet;
        /// <summary>
        /// The ADCP command set.
        /// </summary>
        public string AdcpCommandSet
        {
            get { return _AdcpCommandSet; }
            set
            {
                _AdcpCommandSet = value;
                this.NotifyOfPropertyChange(() => this.AdcpCommandSet);
            }
        }

        #endregion

        #region Communication

        /// <summary>
        /// Display the receive buffer from the connected ADCP serial port.
        /// </summary>
        public string AdcpReceiveBuffer
        {
            get { return _adcpConn.ReceiveBufferString; }
        }

        #endregion

        #region ADCP Send Commands

        /// <summary>
        /// History of all the previous ADCP commands.
        /// </summary>
        private ObservableCollection<string> _AdcpCommandHistory;
        /// <summary>
        /// History of all the previous ADCP commands.
        /// </summary>
        public IEnumerable AdcpCommandHistory
        {
            get { return _AdcpCommandHistory; }
        }

        /// <summary>
        /// Command currently selected.
        /// </summary>
        private string _SelectedAdcpCommand;
        /// <summary>
        /// Command currently selected.
        /// </summary>
        public string SelectedAdcpCommand
        {
            get { return _SelectedAdcpCommand; }
            set
            {
                _SelectedAdcpCommand = value;
                this.NotifyOfPropertyChange(() => this.SelectedAdcpCommand);
                this.NotifyOfPropertyChange(() => this.NewAdcpCommand);
            }
        }

        /// <summary>
        /// New command entered by the user.
        /// This will be called when the user enters
        /// in a new command to send to the ADCP.
        /// It will update the list and set the SelectedCommand.
        /// </summary>
        public string NewAdcpCommand
        {
            get { return _SelectedAdcpCommand; }
            set
            {
                //if (_SelectedAdcpCommand != null)
                //{
                //    return;
                //}
                if (!string.IsNullOrEmpty(value))
                {
                    _AdcpCommandHistory.Insert(0, value);
                    SelectedAdcpCommand = value;
                }
            }
        }

        #endregion

        #region Hardware Connection

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _HardwareConnection;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string HardwareConnection
        {
            get { return _HardwareConnection; }
            set
            {
                _HardwareConnection = value;
                this.NotifyOfPropertyChange(() => this.HardwareConnection);
            }
        }

        #endregion

        #region CEOUTPUT

        /// <summary>
        /// CEOUTPUT description.
        /// </summary>
        public string CEOUTPUT_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetCeoutputDesc();
            }
        }

        /// <summary>
        /// CEOUTPUT list.
        /// </summary>
        public List<string> CeoutputList { get; private set; }

        /// <summary>
        /// CEOUTPUT list.
        /// </summary>
        public List<string> Pd0TransformList { get; private set; }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedCeoutput;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedCeoutput
        {
            get { return _SelectedCeoutput; }
            set
            {
                _SelectedCeoutput = value;

                // Set the value to the config
                _AdcpConfig.Commands.CEOUTPUT = (Commands.AdcpCommands.AdcpOutputMode)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpOutputMode), _SelectedCeoutput);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedCeoutput);
                this.NotifyOfPropertyChange(() => this.IsDisplaySelectedTransform);
                this.NotifyOfPropertyChange(() => this.IsDisplayOceanServerProfileBin);
            }
        }

        #region PD0 Transform

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if PD0 i selected.
        /// </summary>
        public bool IsDisplaySelectedTransform
        {
            get
            {
                // If PD0 is selected
                if(_AdcpConfig.Commands.CEOUTPUT == Commands.AdcpCommands.AdcpOutputMode.PD0 ||
                    _AdcpConfig.Commands.CEOUTPUT == Commands.AdcpCommands.AdcpOutputMode.PD3 ||
                    _AdcpConfig.Commands.CEOUTPUT == Commands.AdcpCommands.AdcpOutputMode.PD4 ||
                    _AdcpConfig.Commands.CEOUTPUT == Commands.AdcpCommands.AdcpOutputMode.PD5)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedTransform;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedTransform
        {
            get { return _SelectedTransform; }
            set
            {
                _SelectedTransform = value;
                this.NotifyOfPropertyChange(() => this.SelectedTransform);

                // Set the value to the config
                _AdcpConfig.Commands.CEOUTPUT_PdCoordinateTransform = (RTI.Core.Commons.Transforms)System.Enum.Parse(typeof(RTI.Core.Commons.Transforms), _SelectedTransform);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #region Ocean Server Profile Bin

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if Ocean Server is selected.
        /// </summary>
        public bool IsDisplayOceanServerProfileBin
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.CEOUTPUT == Commands.AdcpCommands.AdcpOutputMode.OceanServer)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        private ushort _SelectedProfileBin;
        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        public ushort SelectedProfileBin
        {
            get { return _SelectedProfileBin; }
            set
            {
                _SelectedProfileBin = value;

                // Set the value to the config
                _AdcpConfig.Commands.CEOUTPUT_OceanServerProfileBin = value;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedProfileBin);
            }
        }

        #endregion

        #endregion

        #region CERECORD

        /// <summary>
        /// CERECORD description.
        /// </summary>
        public string CERECORD_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetCerecordDesc();
            }
        }

        /// <summary>
        /// CEOUTPUT list.
        /// </summary>
        public List<string> CerecordList { get; private set; }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedCerecord_EnsemblePing;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedCerecord_EnsemblePing
        {
            get { return _SelectedCerecord_EnsemblePing; }
            set
            {
                _SelectedCerecord_EnsemblePing = value;

                // Set the value to the config
                _AdcpConfig.Commands.CERECORD_EnsemblePing = (Commands.AdcpCommands.AdcpRecordOptions)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpRecordOptions), _SelectedCerecord_EnsemblePing);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedCerecord_EnsemblePing);
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private bool _SelectedCerecord_SinglePing;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public bool SelectedCerecord_SinglePing
        {
            get { return _SelectedCerecord_SinglePing; }
            set
            {
                _SelectedCerecord_SinglePing = value;

                // Set the value to the config
                _AdcpConfig.Commands.CERECORD_SinglePing = value;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedCerecord_SinglePing);
            }
        }

        #endregion

        #region CEI

        /// <summary>
        /// CEI description.
        /// </summary>
        public string CEI_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetCeiDesc();
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public int CEI
        {
            get { return _AdcpConfig.Commands.CEI.ToSeconds(); }
            set
            {
                // Set the value to the config
                _AdcpConfig.Commands.CEI = new RTI.Commands.TimeValue(value);

                // Update the command set.
                UpdateCommandSet();

                // Display Time span
                DisplayTimeSpan(value);

                this.NotifyOfPropertyChange(() => this.SelectedCerecord_EnsemblePing);
                this.NotifyOfPropertyChange(() => this.CEI_Timespan);
            }
        }

        /// <summary>
        /// Time span as a string
        /// </summary>
        private string _CEI_Timespan;
        /// <summary>
        /// Time span as a string
        /// </summary>
        public string CEI_Timespan
        {
            get { return _CEI_Timespan; }
            set
            {
                _CEI_Timespan = value;
                this.NotifyOfPropertyChange(() => this.CEI_Timespan);
            }
        }

        #endregion

        #region CTRIG

        /// <summary>
        /// CTRIG description.
        /// </summary>
        public string CTRIG_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetCtrigDesc();
            }
        }

        /// <summary>
        /// CEOUTPUT list.
        /// </summary>
        public List<string> CtrigList { get; private set; }

        /// <summary>
        /// Selected External ADCP trigger.
        /// </summary>
        private string _SelectedCtrig;
        /// <summary>
        /// Selected External ADCP trigger.
        /// </summary>
        public string SelectedCtrig
        {
            get { return _SelectedCtrig; }
            set
            {
                _SelectedCtrig = value;
                this.NotifyOfPropertyChange(() => this.SelectedCtrig);

                // Set the value to the config
                _AdcpConfig.Commands.CTRIG = (Commands.AdcpCommands.AdcpExternalTrigger)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpExternalTrigger), _SelectedCtrig);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #region Serial Output

        /// <summary>
        /// Serial Baud rate list.
        /// </summary>
        public List<string> BaudList { get; private set; }

        /// <summary>
        /// C232B, C422B and C485B description.
        /// </summary>
        public string Baud_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetBaudDesc();
            }
        }

        #region 232

        /// <summary>
        /// C232B output description.
        /// </summary>
        public string C232out_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetC232outDesc();
            }
        }

        /// <summary>
        /// Baudrate for the RS-232 serial port.
        /// </summary>
        private string _SelectedC232B;
        /// <summary>
        /// Baudrate for the RS-232 serial port.
        /// </summary>
        public string SelectedC232B
        {
            get { return _SelectedC232B; }
            set
            {
                _SelectedC232B = value;

                // Set the value to the config
                _AdcpConfig.Commands.C232B = (RTI.Commands.Baudrate)System.Enum.Parse(typeof(RTI.Commands.Baudrate), _SelectedC232B); ;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC232B);
            }
        }

        /// <summary>
        /// Output for the RS-232 serial port.
        /// </summary>
        private string _SelectedC232OUT;
        /// <summary>
        /// Output for the RS-232 serial port.
        /// </summary>
        public string SelectedC232OUT
        {
            get { return _SelectedC232OUT; }
            set
            {
                _SelectedC232OUT = value;

                // Set the value to the config
                _AdcpConfig.Commands.C232OUT = (Commands.AdcpCommands.AdcpOutputMode)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpOutputMode), _SelectedC232OUT);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC232OUT);
                this.NotifyOfPropertyChange(() => this.IsDisplayC232OUT_SelectedTransform);
                this.NotifyOfPropertyChange(() => this.IsDisplayC232OUT_OceanServerProfileBin);
            }
        }

        #region PD0 Transform

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if PD i selected.
        /// </summary>
        public bool IsDisplayC232OUT_SelectedTransform
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C232OUT == Commands.AdcpCommands.AdcpOutputMode.PD0 ||
                    _AdcpConfig.Commands.C232OUT == Commands.AdcpCommands.AdcpOutputMode.PD3 ||
                    _AdcpConfig.Commands.C232OUT == Commands.AdcpCommands.AdcpOutputMode.PD4 ||
                    _AdcpConfig.Commands.C232OUT == Commands.AdcpCommands.AdcpOutputMode.PD5)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedC232OUT_Transform;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedC232OUT_Transform
        {
            get { return _SelectedC232OUT_Transform; }
            set
            {
                _SelectedC232OUT_Transform = value;
                this.NotifyOfPropertyChange(() => this.SelectedC232OUT_Transform);

                // Set the value to the config
                _AdcpConfig.Commands.C232OUT_PdCoordinateTransform = (RTI.Core.Commons.Transforms)System.Enum.Parse(typeof(RTI.Core.Commons.Transforms), _SelectedC232OUT_Transform);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #region Ocean Server Profile Bin

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if Ocean Server is selected.
        /// </summary>
        public bool IsDisplayC232OUT_OceanServerProfileBin
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C232OUT == Commands.AdcpCommands.AdcpOutputMode.OceanServer)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        private ushort _SelectedC232OUT_ProfileBin;
        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        public ushort SelectedC232OUT_ProfileBin
        {
            get { return _SelectedC232OUT_ProfileBin; }
            set
            {
                _SelectedC232OUT_ProfileBin = value;

                // Set the value to the config
                _AdcpConfig.Commands.C232OUT_OceanServerProfileBin = value;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC232OUT_ProfileBin);
            }
        }

        #endregion

        #endregion

        #region 485

        /// <summary>
        /// C485B output description.
        /// </summary>
        public string C485out_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetC485outDesc();
            }
        }

        /// <summary>
        /// Baudrate for the RS-485 serial port.
        /// </summary>
        private string _SelectedC485B;
        /// <summary>
        /// Baudrate for the RS-485 serial port.
        /// </summary>
        public string SelectedC485B
        {
            get { return _SelectedC485B; }
            set
            {
                _SelectedC485B = value;

                // Set the value to the config
                _AdcpConfig.Commands.C485B = (RTI.Commands.Baudrate)System.Enum.Parse(typeof(RTI.Commands.Baudrate), _SelectedC485B); ;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC485B);
            }
        }

        /// <summary>
        /// Output for the RS-232 serial port.
        /// </summary>
        private string _SelectedC485OUT;
        /// <summary>
        /// Output for the RS-232 serial port.
        /// </summary>
        public string SelectedC485OUT
        {
            get { return _SelectedC485OUT; }
            set
            {
                _SelectedC485OUT = value;

                // Set the value to the config
                _AdcpConfig.Commands.C485OUT = (Commands.AdcpCommands.AdcpOutputMode)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpOutputMode), _SelectedC485OUT);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC485OUT);
                this.NotifyOfPropertyChange(() => this.IsDisplayC485OUT_SelectedTransform);
                this.NotifyOfPropertyChange(() => this.IsDisplayC485OUT_OceanServerProfileBin);
            }
        }

        #region PD0 Transform

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if PD i selected.
        /// </summary>
        public bool IsDisplayC485OUT_SelectedTransform
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C485OUT == Commands.AdcpCommands.AdcpOutputMode.PD0 ||
                    _AdcpConfig.Commands.C485OUT == Commands.AdcpCommands.AdcpOutputMode.PD3 ||
                    _AdcpConfig.Commands.C485OUT == Commands.AdcpCommands.AdcpOutputMode.PD4 ||
                    _AdcpConfig.Commands.C485OUT == Commands.AdcpCommands.AdcpOutputMode.PD5)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedC485OUT_Transform;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedC485OUT_Transform
        {
            get { return _SelectedC485OUT_Transform; }
            set
            {
                _SelectedC485OUT_Transform = value;
                this.NotifyOfPropertyChange(() => this.SelectedC485OUT_Transform);

                // Set the value to the config
                _AdcpConfig.Commands.C485OUT_PdCoordinateTransform = (RTI.Core.Commons.Transforms)System.Enum.Parse(typeof(RTI.Core.Commons.Transforms), _SelectedC485OUT_Transform);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #region Ocean Server Profile Bin

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if Ocean Server is selected.
        /// </summary>
        public bool IsDisplayC485OUT_OceanServerProfileBin
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C485OUT == Commands.AdcpCommands.AdcpOutputMode.OceanServer)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        private ushort _SelectedC485OUT_ProfileBin;
        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        public ushort SelectedC485OUT_ProfileBin
        {
            get { return _SelectedC485OUT_ProfileBin; }
            set
            {
                _SelectedC485OUT_ProfileBin = value;

                // Set the value to the config
                _AdcpConfig.Commands.C485OUT_OceanServerProfileBin = value;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC485OUT_ProfileBin);
            }
        }

        #endregion

        #endregion

        #region 422

        /// <summary>
        /// C422B output description.
        /// </summary>
        public string C422out_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetC422outDesc();
            }
        }

        /// <summary>
        /// Baudrate for the RS-422 serial port.
        /// </summary>
        private string _SelectedC422B;
        /// <summary>
        /// Baudrate for the RS-422 serial port.
        /// </summary>
        public string SelectedC422B
        {
            get { return _SelectedC422B; }
            set
            {
                _SelectedC422B = value;

                // Set the value to the config
                _AdcpConfig.Commands.C422B = (RTI.Commands.Baudrate)System.Enum.Parse(typeof(RTI.Commands.Baudrate), _SelectedC422B); ;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC422B);
            }
        }

        /// <summary>
        /// Output for the RS-422 serial port.
        /// </summary>
        private string _SelectedC422OUT;
        /// <summary>
        /// Output for the RS-422 serial port.
        /// </summary>
        public string SelectedC422OUT
        {
            get { return _SelectedC485OUT; }
            set
            {
                _SelectedC422OUT = value;

                // Set the value to the config
                _AdcpConfig.Commands.C422OUT = (Commands.AdcpCommands.AdcpOutputMode)System.Enum.Parse(typeof(Commands.AdcpCommands.AdcpOutputMode), _SelectedC422OUT);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC422OUT);
                this.NotifyOfPropertyChange(() => this.IsDisplayC422OUT_SelectedTransform);
                this.NotifyOfPropertyChange(() => this.IsDisplayC422OUT_OceanServerProfileBin);
            }
        }

        #region PD0 Transform

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if PD i selected.
        /// </summary>
        public bool IsDisplayC422OUT_SelectedTransform
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C422OUT == Commands.AdcpCommands.AdcpOutputMode.PD0 ||
                    _AdcpConfig.Commands.C422OUT == Commands.AdcpCommands.AdcpOutputMode.PD3 ||
                    _AdcpConfig.Commands.C422OUT == Commands.AdcpCommands.AdcpOutputMode.PD4 ||
                    _AdcpConfig.Commands.C422OUT == Commands.AdcpCommands.AdcpOutputMode.PD5)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        private string _SelectedC422OUT_Transform;
        /// <summary>
        /// Hardware connection used to communicate with
        /// the ADCP to configure it.
        /// </summary>
        public string SelectedC422OUT_Transform
        {
            get { return _SelectedC422OUT_Transform; }
            set
            {
                _SelectedC422OUT_Transform = value;
                this.NotifyOfPropertyChange(() => this.SelectedC422OUT_Transform);

                // Set the value to the config
                _AdcpConfig.Commands.C422OUT_PdCoordinateTransform = (RTI.Core.Commons.Transforms)System.Enum.Parse(typeof(RTI.Core.Commons.Transforms), _SelectedC422OUT_Transform);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #region Ocean Server Profile Bin

        /// <summary>
        /// Set a flag to display the selected 
        /// transform item if Ocean Server is selected.
        /// </summary>
        public bool IsDisplayC422OUT_OceanServerProfileBin
        {
            get
            {
                // If PD0 is selected
                if (_AdcpConfig.Commands.C422OUT == Commands.AdcpCommands.AdcpOutputMode.OceanServer)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        private ushort _SelectedC422OUT_ProfileBin;
        /// <summary>
        /// Profile bin for the Ocean Server output.
        /// </summary>
        public ushort SelectedC422OUT_ProfileBin
        {
            get { return _SelectedC422OUT_ProfileBin; }
            set
            {
                _SelectedC422OUT_ProfileBin = value;

                // Set the value to the config
                _AdcpConfig.Commands.C422OUT_OceanServerProfileBin = value;

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedC422OUT_ProfileBin);
            }
        }

        #endregion

        #endregion

        #endregion

        #region UDP

        /// <summary>
        /// IP description.
        /// </summary>
        public string IP_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetIpDesc() + "\n" + Commands.AdcpCommands.GetCemacDesc();
            }
        }

        /// <summary>
        /// UDPPORT description.
        /// </summary>
        public string UDPPORT_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetUdpportDesc();
            }
        }

        /// <summary>
        /// CUDPOUT description.
        /// </summary>
        public string CUDPOUT_Desc
        {
            get
            {
                return Commands.AdcpCommands.GetCudpoutDesc();
            }
        }

        /// <summary>
        /// CUDPOUT list.
        /// </summary>
        public List<string> CudpOutputList { get; private set; }

        /// <summary>
        /// Output format for the UDP port.
        /// </summary>
        private string _SelectedCudpout;
        /// <summary>
        /// Output format for the UDP port.
        /// </summary>
        public string SelectedCudpout
        {
            get { return _SelectedCudpout; }
            set
            {
                _SelectedCudpout = value;

                // Set the value to the config
                _AdcpConfig.Commands.CUDPOUT = (Commands.AdcpCommands.UdpOutputMode)System.Enum.Parse(typeof(Commands.AdcpCommands.UdpOutputMode), _SelectedCudpout);

                // Update the command set.
                UpdateCommandSet();

                this.NotifyOfPropertyChange(() => this.SelectedCudpout);
            }
        }

        /// <summary>
        /// Selected ADCP Ethernet Address A.
        /// </summary>
        public uint EtherAddressA
        {
            get
            {
                return _AdcpConfig.Commands.IP.IpAddrA;
            }
            set
            {
                _AdcpConfig.Commands.IP.IpAddrA = value;
                this.NotifyOfPropertyChange(() => this.EtherAddressA);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        /// <summary>
        /// Selected ADCP Ethernet Address B.
        /// </summary>
        public uint EtherAddressB
        {
            get
            {
                return _AdcpConfig.Commands.IP.IpAddrB;
            }
            set
            {
                _AdcpConfig.Commands.IP.IpAddrB = value;
                this.NotifyOfPropertyChange(() => this.EtherAddressB);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        /// <summary>
        /// Selected ADCP Ethernet Address C.
        /// </summary>
        public uint EtherAddressC
        {
            get
            {
                return _AdcpConfig.Commands.IP.IpAddrC;
            }
            set
            {
                _AdcpConfig.Commands.IP.IpAddrC = value;
                this.NotifyOfPropertyChange(() => this.EtherAddressC);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        /// <summary>
        /// Selected ADCP Ethernet Address D.
        /// </summary>
        public uint EtherAddressD
        {
            get
            {
                return _AdcpConfig.Commands.IP.IpAddrD;
            }
            set
            {
                _AdcpConfig.Commands.IP.IpAddrD = value;
                this.NotifyOfPropertyChange(() => this.EtherAddressD);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        /// <summary>
        /// Ethernet Port
        /// </summary>
        public uint EtherPort
        {
            get
            {
                return _AdcpConfig.Commands.IP.Port;
            }
            set
            {
                _AdcpConfig.Commands.IP.Port = value;
                this.NotifyOfPropertyChange(() => this.EtherPort);

                // Update the command set.
                UpdateCommandSet();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to send a BREAK.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SendBreakCommand { get; protected set; }

        /// <summary>
        /// Command to stop the DVL.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> StopDvlCommand { get; protected set; }

        /// <summary>
        /// Command to read the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ReadAdcpCommand { get; protected set; }

        /// <summary>
        /// Command to send a command to the terminal.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SendCommand { get; protected set; }

        /// <summary>
        /// Command to clear the terminal.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ClearTerminalCommand { get; protected set; }

        /// <summary>
        /// Command to clear the command set.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ClearCommandSetCommand { get; protected set; }

        /// <summary>
        /// Command to set the ADCP time.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SetAdcpTimeCommand { get; protected set; }

        /// <summary>
        /// Command to send the command set.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SendCommandSetCommand { get; protected set; }

        /// <summary>
        /// Command to send the START DVL.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SendDvlStartCommand { get; protected set; }

        /// <summary>
        /// Command to save the command set to a file.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SaveCommandSetCommand { get; protected set; }

        /// <summary>
        /// Command to import a command set.
        /// </summary>
        public ReactiveCommand<object> ImportCommandSetCommand { get; protected set; }

        /// <summary>
        /// Command to add a subsystem.
        /// </summary>
        public ReactiveCommand<object> AddSubsystemCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public DvlSetupViewModel()
            : base("DVL Setup")
        {
            // Initialize values
            _adcpConn = IoC.Get<AdcpConnection>();
            _adcpConn.ReceiveDataEvent += new AdcpConnection.ReceiveDataEventHandler(_adcpConnection_ReceiveDataEvent);

            // Update the display
            _displayTimer = new System.Timers.Timer(500);
            _displayTimer.Elapsed += _displayTimer_Elapsed;
            _displayTimer.AutoReset = true;
            _displayTimer.Enabled = true;

            SubsystemConfigList = new ReactiveList<DvlSubsystemConfigurationViewModel>();

            _AdcpCommandHistory = new ObservableCollection<string>();

            _AdcpConfig = new AdcpConfiguration();

            // Display Time span
            DisplayTimeSpan(_AdcpConfig.Commands.CEI.ToSeconds());

            // Initialize list
            Init();

            // Send BREAK command
            SendBreakCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SendBreak()));

            // Send BREAK command
            StopDvlCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => StopDvl()));

            // Read the ADCP command
            ReadAdcpCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ReadAdcp()));

            // Send command to the terminal
            SendCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SendTerminal()));

            // Clear the terminal
            ClearTerminalCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ClearTerminal()));

            // Clear the command set
            ClearCommandSetCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ClearCommandSet()));

            // Set the ADCP time
            SetAdcpTimeCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SetAdcpTime()));

            // Send the command set to the ADCP
            SendCommandSetCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SendCommandSet()));

            // Send the command start the DVL
            SendDvlStartCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SendDvlStart()));

            // Save the command set to a file
            SaveCommandSetCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => SaveCommandSet()));

            // Add Subsystem
            AddSubsystemCommand = ReactiveCommand.Create();
            AddSubsystemCommand.Subscribe(_ => AddSubsystem());

            // Import an ADCP command set
            ImportCommandSetCommand = ReactiveCommand.Create();
            ImportCommandSetCommand.Subscribe(_ => ImportCommandSet());
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            if (_adcpConn != null)
            {
                _adcpConn.ReceiveDataEvent -= _adcpConnection_ReceiveDataEvent;
            }
        }

        #region Init

        /// <summary>
        /// Initialize all the list.
        /// </summary>
        private void Init()
        {
            // CEOUTPUT
            CeoutputList = new List<string>();
            // Get the names from the AdcpOutputMode enum
            Array ceoutput = System.Enum.GetValues(typeof(Commands.AdcpCommands.AdcpOutputMode));
            foreach (var output in ceoutput)
            {
                CeoutputList.Add(Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), output));
            }
            SelectedCeoutput = Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), _AdcpConfig.Commands.CEOUTPUT);

            // PD0 Transform
            Pd0TransformList = new List<string>();
            // Get the names from the AdcpOutputMode enum
            Array pd0Transform = System.Enum.GetValues(typeof(RTI.Core.Commons.Transforms));
            foreach (var transform in pd0Transform)
            {
                Pd0TransformList.Add(Enum.GetName(typeof(RTI.Core.Commons.Transforms), transform));
            }
            SelectedTransform = Enum.GetName(typeof(RTI.Core.Commons.Transforms), _AdcpConfig.Commands.CEOUTPUT_PdCoordinateTransform);
            SelectedProfileBin = _AdcpConfig.Commands.CEOUTPUT_OceanServerProfileBin;

            // CERECORD
            CerecordList = new List<string>();
            Array cerecord = System.Enum.GetValues(typeof(Commands.AdcpCommands.AdcpRecordOptions));
            foreach (var output in cerecord)
            {
                CerecordList.Add(Enum.GetName(typeof(Commands.AdcpCommands.AdcpRecordOptions), output));
            }
            SelectedCerecord_EnsemblePing = Enum.GetName(typeof(Commands.AdcpCommands.AdcpRecordOptions), _AdcpConfig.Commands.CERECORD_EnsemblePing);
            SelectedCerecord_SinglePing = _AdcpConfig.Commands.CERECORD_SinglePing;

            // CTRIG
            CtrigList = new List<string>();
            // Get the names from the AdcpOutputMode enum
            Array trigs = System.Enum.GetValues(typeof(Commands.AdcpCommands.AdcpExternalTrigger));
            foreach (var trig in trigs)
            {
                CtrigList.Add(Enum.GetName(typeof(Commands.AdcpCommands.AdcpExternalTrigger), trig));
            }
            SelectedCtrig = Enum.GetName(typeof(Commands.AdcpCommands.AdcpExternalTrigger), _AdcpConfig.Commands.CTRIG);

            // Serial
            BaudList = new List<string>(); // Get the names from the AdcpOutputMode enum
            Array bauds = System.Enum.GetValues(typeof(Commands.Baudrate));
            foreach (var baud in bauds)
            {
                BaudList.Add(Enum.GetName(typeof(Commands.Baudrate), baud));
            }
            SelectedC232OUT = Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), _AdcpConfig.Commands.C232OUT);
            SelectedC232OUT_Transform = Enum.GetName(typeof(RTI.Core.Commons.Transforms), _AdcpConfig.Commands.C232OUT_PdCoordinateTransform);
            SelectedC232OUT_ProfileBin = _AdcpConfig.Commands.C232OUT_OceanServerProfileBin;

            SelectedC485OUT = Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), _AdcpConfig.Commands.C485OUT);
            SelectedC485OUT_Transform = Enum.GetName(typeof(RTI.Core.Commons.Transforms), _AdcpConfig.Commands.C485OUT_PdCoordinateTransform);
            SelectedC485OUT_ProfileBin = _AdcpConfig.Commands.C485OUT_OceanServerProfileBin;

            SelectedC422OUT = Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), _AdcpConfig.Commands.C422OUT);
            SelectedC422OUT_Transform = Enum.GetName(typeof(RTI.Core.Commons.Transforms), _AdcpConfig.Commands.C422OUT_PdCoordinateTransform);
            SelectedC422OUT_ProfileBin = _AdcpConfig.Commands.C422OUT_OceanServerProfileBin;
            
            SelectedC232B = Enum.GetName(typeof(Commands.Baudrate), _AdcpConfig.Commands.C232B);
            SelectedC485B = Enum.GetName(typeof(Commands.Baudrate), _AdcpConfig.Commands.C485B);
            SelectedC422B = Enum.GetName(typeof(Commands.Baudrate), _AdcpConfig.Commands.C422B);

            // UDP
            CudpOutputList = new List<string>();
            // Get the names from the AdcpOutputMode enum
            Array cudpouts = System.Enum.GetValues(typeof(Commands.AdcpCommands.UdpOutputMode));
            foreach (var output in cudpouts)
            {
                CudpOutputList.Add(Enum.GetName(typeof(Commands.AdcpCommands.UdpOutputMode), output));
            }
            SelectedCudpout = Enum.GetName(typeof(Commands.AdcpCommands.UdpOutputMode), _AdcpConfig.Commands.CUDPOUT);
            
            // Update the subsystem config list
            var configs = _AdcpConfig.SubsystemConfigDict.Values.ToArray();
            for (int x = 0; x < configs.Length; x++)
            {
                SubsystemConfigList.Add(new DvlSubsystemConfigurationViewModel(ref configs[x], this));
            }
            //foreach (var config in _AdcpConfig.SubsystemConfigDict.Values)
            //{
            //    SubsystemConfigList.Add(new DvlSubsystemConfigurationViewModel(config, this));
            //}

            // Create the Subsystem List
            ListOfSubsystems = new SubsystemList();
        }

        #endregion

        #region Update Display

        /// <summary>
        /// Update all the display.
        /// </summary>
        private void UpdateDisplay()
        {
            SelectedCeoutput = Enum.GetName(typeof(Commands.AdcpCommands.AdcpOutputMode), _AdcpConfig.Commands.CEOUTPUT);
        }

        #endregion


        #region Update Terminal Display

        /// <summary>
        /// Reduce the number of times the display is updated.
        /// This will update the display based off the timer and not
        /// based off when data is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.NotifyOfPropertyChange(() => this.AdcpReceiveBuffer);
        }

        #endregion

        #region Date Time Display

        /// <summary>
        /// Create a pretty timespan for the given seconds.
        /// </summary>
        /// <param name="seconds">Number of seconds.</param>
        private void DisplayTimeSpan(int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, (int)seconds);
            CEI_Timespan = MathHelper.TimeSpanPrettyFormat(ts);
        }

        #endregion

        #region Command Set

        /// <summary>
        /// Get the command set from the configuration created.
        /// </summary>
        /// <returns>List of all the commands.</returns>
        private List<string> GetCommandSet()
        {
            List<string> commands = new List<string>();

            if (_AdcpConfig != null)
            {
                // Add the system commands
                commands.AddRange(_AdcpConfig.Commands.GetDvlCommandList());

                // Add the subsystem commands
                foreach (var subConfig in _AdcpConfig.SubsystemConfigDict.Values)
                {
                    commands.AddRange(subConfig.Commands.GetDvlCommandList());
                }
            }

            // Add the CSAVE command
            commands.Add(RTI.Commands.AdcpCommands.CMD_CSAVE);

            return commands;
        }

        /// <summary>
        /// Update the command set with the latest information.
        /// </summary>
        public void UpdateCommandSet()
        {
            // Get the command set from the configuration
            List<string> commands = GetCommandSet();

            // Update the string
            UpdateCommandSetStr(commands);
        }

        /// <summary>
        /// Create a string of all the commands.
        /// </summary>
        /// <param name="commands">Commands to create the string.</param>
        private void UpdateCommandSetStr(List<string> commands)
        {
            // Go through all the commands
            StringBuilder sb = new StringBuilder();
            foreach(var cmd in commands)
            {
                sb.AppendLine(cmd);
            }

            // Update the string
            AdcpCommandSet = sb.ToString();
        }

        #endregion

        #region Configurations

        /// <summary>
        /// Setup the configurations based off the
        /// currently set AdcpConfig.
        /// </summary>
        private void SetupConfiguration()
        {
            HardwareConnection = AdcpConfig.EngPort;

            // Update the command set.
            UpdateCommandSet();

            // Update the display
            UpdateDisplay();

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                // Add VM and update subsystem list
                ListOfSubsystems.Clear();
                var configs = _AdcpConfig.SubsystemConfigDict.Values.ToArray();
                for (int x = 0; x < configs.Length; x++)
                {
                    // Add VM
                    AddVM(ref configs[x]);

                    // Update Subsystem List
                    ListOfSubsystems.Add(new SubsystemList.SubsystemCodeDesc(configs[x].SubsystemConfig.SubSystem.Code));

                    // Have it select the first option by default
                    if (ListOfSubsystems.Count >= 1)
                    {
                        SelectedSubsystem = ListOfSubsystems[0];
                    }
                }
            }));
        }

        /// <summary>
        /// Add a configuration to the list.
        /// This will add the configuration to the ADCP Config and 
        /// also create a VM to update its values.
        /// </summary>
        /// <param name="code"></param>
        private void AddConfiguration(byte code)
        {
            AdcpSubsystemConfig config = null;
            _AdcpConfig.AddConfiguration(new Subsystem(code), out config);

            // Update the command set.
            UpdateCommandSet();

            // Add the VM
            AddVM(ref config);
        }

        /// <summary>
        /// Add the VM to the list.
        /// </summary>
        /// <param name="config">Subsystem config.</param>
        private void AddVM(ref AdcpSubsystemConfig config)
        {
            // Create the VM
            var dvlSubConfig = new DvlSubsystemConfigurationViewModel(ref config, this);

            System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() => SubsystemConfigList.Add(dvlSubConfig)));
        }

        /// <summary>
        /// Remove the view model.
        /// </summary>
        /// <param name="vm">Viewmodel to remove.</param>
        public void RemoveVM(DvlSubsystemConfigurationViewModel vm)
        {
            // Remove the subsystem config from the config
            _AdcpConfig.RemoveAdcpSubsystemConfig(vm.AdcpSubConfig);

            // Remove the vm from the list
            SubsystemConfigList.Remove(vm);

            // Dispose of the VM
            vm.Dispose();

            // Update the commandset
            UpdateCommandSet();
        }

        /// <summary>
        /// Clear the configuration.
        /// </summary>
        private void ClearConfiguration()
        {
            // Clear the command set
            ClearCommandSet();

            // Remove the old configuraon
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() => SubsystemConfigList.Clear()));
        }

        #endregion

        #region Commands

        /// <summary>
        /// Send a BREAK statement.
        /// </summary>
        private void SendBreak()
        {
            // Ensure the serial port is open
            if (_adcpConn.AdcpSerialPort.IsOpen())
            {
                _adcpConn.AdcpSerialPort.SendBreak();
            }
        }

        /// <summary>
        /// Send a STOP command to the DVL.
        /// </summary>
        private void StopDvl()
        {
            // Ensure the serial port is open
            if (_adcpConn.AdcpSerialPort.IsOpen())
            {
                _adcpConn.AdcpSerialPort.StopPinging();
            }
        }

        /// <summary>
        /// Read the settings from the ADCP.
        /// </summary>
        private void ReadAdcp()
        {
            if(_adcpConn.AdcpSerialPort.IsOpen())
            {
                // Clear configuration
                ClearConfiguration();

                // Get the configruation
                 AdcpConfig = _adcpConn.GetAdcpConfiguration();

                // Setup the configuraion
                SetupConfiguration();
            }            
        }

        /// <summary>
        /// Send a command to the terminal.
        /// </summary>
        private void SendTerminal()
        {
            // Send the command
            _adcpConn.SendDataWaitReply(SelectedAdcpCommand);

            // Clear the command
            SelectedAdcpCommand = "";
        }

        /// <summary>
        /// Clear the terminal.
        /// </summary>
        private void ClearTerminal()
        {
            _adcpConn.ReceiveBufferString = "";
            this.NotifyOfPropertyChange(() => this.AdcpReceiveBuffer);
        }

        /// <summary>
        /// Clear the Command set.
        /// </summary>
        private void ClearCommandSet()
        {
            AdcpCommandSet = "";
        }

        /// <summary>
        /// Set the ADCP time.
        /// </summary>
        private void SetAdcpTime()
        {
            _adcpConn.SetLocalSystemDateTime();
        }

        /// <summary>
        /// Send the command set to the ADCP.
        /// </summary>
        private void SendCommandSet()
        {
            // Get the command set from the configuration
            List<string> commands = GetCommandSet();

            // Send the commands to the ADCP
            _adcpConn.SendCommands(commands);
        }

        /// <summary>
        /// Send the command start pinging the DVL.
        /// </summary>
        private void SendDvlStart()
        {
            _adcpConn.StartPinging(true);
        }

        /// <summary>
        /// Save the command set to a file.
        /// </summary>
        private void SaveCommandSet()
        {
            try
            {
                // Get the project dir
                // Create the file name
                string prjDir = @"c:\RTI_Configuration_Files";
                System.IO.Directory.CreateDirectory(prjDir);

                DateTime now = DateTime.Now;
                string year = now.Year.ToString("0000");
                string month = now.Month.ToString("00");
                string day = now.Day.ToString("00");
                string hours = now.Hour.ToString("00");
                string minutes = now.Minute.ToString("00");
                string seconds = now.Second.ToString("00");
                string fileName = string.Format("Commands_{0}{1}{2}{3}{4}{5}.txt", year, month, day, hours, minutes, seconds);
                string cmdFilePath = prjDir + @"\" + fileName;

                // Get the commands
                string[] lines = GetCommandSet().ToArray();

                // Create a text file in the project
                System.IO.File.WriteAllLines(cmdFilePath, lines);

                // Set the filepath to the console output
                _adcpConn.ReceiveBufferString = "";
                _adcpConn.ReceiveBufferString = "File saved to: " + cmdFilePath;
                this.NotifyOfPropertyChange(() => this.AdcpReceiveBuffer);
            }
            catch (Exception e)
            {
                log.Error("Error writing configuration file.", e);
            }
        }

        /// <summary>
        /// Import a command set from a file.
        /// </summary>
        private void ImportCommandSet()
        {
            string fileName = "";
            try
            {
                // Show the FolderBrowserDialog.
                System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "All files (*.*)|*.*";
                dialog.Multiselect = false;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the files selected
                    fileName = dialog.FileName;

                    // Clear configuration
                    ClearConfiguration();

                    // Set the command set
                    AdcpCommandSet = File.ReadAllText(fileName);

                    // Decode the command set to apply to the configuration
                    //AdcpConfig = RTI.Commands.AdcpCommands.DecodeCSHOW(AdcpCommandSet, new SerialNumber());
                    AdcpConfig = RTI.Commands.AdcpCommands.DecodeCommandSet(AdcpCommandSet, new SerialNumber());

                    // Setup the configuraion
                    SetupConfiguration();
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error reading command set from {0}", fileName), e);
            }
        }

        /// <summary>
        /// Add a subsystem to the configuration and add a display.
        /// </summary>
        private void AddSubsystem()
        {
            if (_selectedSubsystem != null)
            {
                // Check serial
                if(_AdcpConfig !=null && _AdcpConfig.SerialNumber != null && _AdcpConfig.SerialNumber.IsEmpty())
                {
                    _AdcpConfig.SerialNumber.AddSubsystem(new Subsystem(_selectedSubsystem.Code));
                }

                // Add config
                AddConfiguration(_selectedSubsystem.Code);
            }
        }

        #endregion

        #region EventHandler

        /// <summary>
        /// Event handler when receiving serial data.
        /// </summary>
        /// <param name="data">Data received from the serial port.</param>
        private void _adcpConnection_ReceiveDataEvent(byte[] data)
        {
            //this.NotifyOfPropertyChange(() => this.AdcpReceiveBuffer);
        }

        #endregion

    }
}
