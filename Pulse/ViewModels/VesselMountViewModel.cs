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
    /// Set the Vessel Mount options.
    /// </summary>
    public class VesselMountViewModel : PulseViewModel, IHandle<ProjectEvent>
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

        /// <summary>
        /// Singleton object to communication with the ADCP.
        /// </summary>
        private AdcpConnection _adcpConnection;

        /// <summary>
        /// Vessel Mount Options.
        /// </summary>
        private VesselMountOptions _vmOptions;

        #endregion

        #region Properties

        #region GPS 1

        /// <summary>
        /// Selected GPS 1 Comm Port.
        /// </summary>
        private string _SelectedGps1CommPort;
        /// <summary>
        /// Selected GPS 1 Comm Port.
        /// </summary>
        public string SelectedGps1CommPort
        {
            get { return _SelectedGps1CommPort; }
            set
            {
                _SelectedGps1CommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedGps1CommPort);
                //this.NotifyOfPropertyChange(() => this.IsSetGps1ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Gps1SerialOptions.Port = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectGps1Serial(_pm.SelectedProject.Configuration.Gps1SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateGps1SerialCommPort(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        /// <summary>
        /// Selected GPS 1 Baud Rate.
        /// </summary>
        private int _SelectedGps1BaudRate;
        /// <summary>
        /// Selected GPS 1 Baud Rate.
        /// </summary>
        public int SelectedGps1BaudRate
        {
            get { return _SelectedGps1BaudRate; }
            set
            {
                _SelectedGps1BaudRate = value;
                this.NotifyOfPropertyChange(() => this.SelectedGps1BaudRate);
                //this.NotifyOfPropertyChange(() => this.IsSetGps1ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Gps1SerialOptions.BaudRate = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectGps1Serial(_pm.SelectedProject.Configuration.Gps1SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateGps1SerialBaudRate(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        #endregion

        #region GPS 2

        /// <summary>
        /// Selected GPS 2 Comm Port.
        /// </summary>
        private string _SelectedGps2CommPort;
        /// <summary>
        /// Selected GPS 2 Comm Port.
        /// </summary>
        public string SelectedGps2CommPort
        {
            get { return _SelectedGps2CommPort; }
            set
            {
                _SelectedGps2CommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedGps2CommPort);
                this.NotifyOfPropertyChange(() => this.IsSetGps2ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Gps2SerialOptions.Port = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectGps2Serial(_pm.SelectedProject.Configuration.Gps2SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateGps2SerialCommPort(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        /// <summary>
        /// Selected GPS 2 Baud Rate.
        /// </summary>
        private int _SelectedGps2BaudRate;
        /// <summary>
        /// Selected GPS 2 Baud Rate.
        /// </summary>
        public int SelectedGps2BaudRate
        {
            get { return _SelectedGps2BaudRate; }
            set
            {
                _SelectedGps2BaudRate = value;
                this.NotifyOfPropertyChange(() => this.SelectedGps2BaudRate);
                this.NotifyOfPropertyChange(() => this.IsSetGps2ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Gps2SerialOptions.BaudRate = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectGps2Serial(_pm.SelectedProject.Configuration.Gps2SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateGps2SerialBaudRate(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        /// <summary>
        /// Set if GPS 2 connection is available.
        /// </summary>
        public bool IsSetGps2ConnAvail
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedGps2CommPort) && SelectedGps2BaudRate > 0;
            }
        }

        #endregion

        #region NMEA 1

        /// <summary>
        /// Selected NMEA 1 Comm Port.
        /// </summary>
        private string _SelectedNmea1CommPort;
        /// <summary>
        /// Selected NMEA 1 Comm Port.
        /// </summary>
        public string SelectedNmea1CommPort
        {
            get { return _SelectedNmea1CommPort; }
            set
            {
                _SelectedNmea1CommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedNmea1CommPort);
                this.NotifyOfPropertyChange(() => this.IsSetNmea1ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Nmea1SerialOptions.Port = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectNmea1Serial(_pm.SelectedProject.Configuration.Nmea1SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateNmea1SerialCommPort(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        /// <summary>
        /// Selected NMEA 1 Baud Rate.
        /// </summary>
        private int _SelectedNmea1BaudRate;
        /// <summary>
        /// Selected NMEA 1 Baud Rate.
        /// </summary>
        public int SelectedNmea1BaudRate
        {
            get { return _SelectedNmea1BaudRate; }
            set
            {
                _SelectedNmea1BaudRate = value;
                this.NotifyOfPropertyChange(() => this.SelectedNmea1BaudRate);
                this.NotifyOfPropertyChange(() => this.IsSetNmea1ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Nmea1SerialOptions.BaudRate = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectNmea1Serial(_pm.SelectedProject.Configuration.Nmea1SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateNmea1SerialBaudRate(value);
            }
        }

        /// <summary>
        /// Set if NMEA 1 connection is available.
        /// </summary>
        public bool IsSetNmea1ConnAvail
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedNmea1CommPort) && SelectedNmea1BaudRate > 0;
            }
        }

        #endregion

        #region NMEA 2

        /// <summary>
        /// Selected NMEA 2 Comm Port.
        /// </summary>
        private string _SelectedNmea2CommPort;
        /// <summary>
        /// Selected NMEA 2 Comm Port.
        /// </summary>
        public string SelectedNmea2CommPort
        {
            get { return _SelectedNmea2CommPort; }
            set
            {
                _SelectedNmea2CommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedNmea2CommPort);
                this.NotifyOfPropertyChange(() => this.IsSetNmea2ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Nmea2SerialOptions.Port = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectNmea2Serial(_pm.SelectedProject.Configuration.Nmea2SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateNmea2SerialCommPort(value);

                // Update Pulse
                _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
            }
        }

        /// <summary>
        /// Selected NMEA 2 Baud Rate.
        /// </summary>
        private int _SelectedNmea2BaudRate;
        /// <summary>
        /// Selected NMEA 2 Baud Rate.
        /// </summary>
        public int SelectedNmea2BaudRate
        {
            get { return _SelectedNmea2BaudRate; }
            set
            {
                _SelectedNmea2BaudRate = value;
                this.NotifyOfPropertyChange(() => this.SelectedNmea2BaudRate);
                this.NotifyOfPropertyChange(() => this.IsSetNmea2ConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.Nmea2SerialOptions.BaudRate = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectNmea2Serial(_pm.SelectedProject.Configuration.Nmea2SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateNmea2SerialBaudRate(value);
            }
        }

        /// <summary>
        /// Flag to state whether the NMEA 2 is connected or not.
        /// </summary>
        private bool _IsNmea2Set;
        /// <summary>
        /// Flag to state whether the NMEA 2 is connected or not.
        /// </summary>
        public bool IsNmea2Set
        {
            get { return _IsNmea2Set; }
            set
            {
                _IsNmea2Set = value;
                this.NotifyOfPropertyChange(() => this.IsNmea2Set);
            }
        }

        /// <summary>
        /// Set if NMEA 2 connection is available.
        /// </summary>
        public bool IsSetNmea2ConnAvail
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedNmea2CommPort) && SelectedNmea2BaudRate > 0;
            }
        }

        #endregion

        #region Lists

        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        public List<string> CommPortList { get; set; }

        /// <summary>
        /// List of all the baud rate options.
        /// </summary>
        public List<int> BaudRateList { get; set; }

        /// <summary>
        /// List of all the Heading Sources.
        /// </summary>
        public List<string> HeadingSourceList { get; set; }

        /// <summary>
        /// List of all the Tilt Sources.
        /// </summary>
        public List<string> TiltSourceList { get; set; }

        #endregion

        #region Heading Source

        /// <summary>
        /// Heading source.
        /// </summary>
        public string SelectedHeadingSource
        {
            get { return _vmOptions.HeadingSource; }
            set
            {
                _vmOptions.HeadingSource = value;
                this.NotifyOfPropertyChange(() => this.SelectedHeadingSource);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Magnetic Heading Offset.
        /// </summary>
        public float MagHeadingOffset
        {
            get { return _vmOptions.HeadingOffsetMag; }
            set
            {
                _vmOptions.HeadingOffsetMag = value;
                this.NotifyOfPropertyChange(() => this.MagHeadingOffset);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Alignment Heading offset.
        /// </summary>
        public float AlignmentHeadingOffset
        {
            get { return _vmOptions.HeadingOffsetAlignment; }
            set
            {
                _vmOptions.HeadingOffsetAlignment = value;
                this.NotifyOfPropertyChange(() => this.AlignmentHeadingOffset);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Fixed heading value in degrees.
        /// </summary>
        public float FixedHeading
        {
            get { return _vmOptions.FixedHeading; }
            set
            {
                _vmOptions.FixedHeading = value;
                this.NotifyOfPropertyChange(() => this.FixedHeading);

                // Save the options to the project
                Save();
            }
        }

        #endregion

        #region Tilt Source

        /// <summary>
        /// Tilt source.
        /// </summary>
        public string SelectedTiltSource
        {
            get { return _vmOptions.TiltSource; }
            set
            {
                _vmOptions.TiltSource = value;
                this.NotifyOfPropertyChange(() => this.SelectedTiltSource);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Pitch Offset.
        /// </summary>
        public float PitchOffset
        {
            get { return _vmOptions.PitchOffset; }
            set
            {
                _vmOptions.PitchOffset = value;
                this.NotifyOfPropertyChange(() => this.PitchOffset);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Roll Offset.
        /// </summary>
        public float RollOffset
        {
            get { return _vmOptions.RollOffset; }
            set
            {
                _vmOptions.RollOffset = value;
                this.NotifyOfPropertyChange(() => this.RollOffset);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Flag if a fixed Pitch should be used..
        /// </summary>
        public bool IsPitchFixed
        {
            get { return _vmOptions.IsPitchFixed; }
            set
            {
                _vmOptions.IsPitchFixed = value;
                this.NotifyOfPropertyChange(() => this.IsPitchFixed);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Pitch fixed.
        /// </summary>
        public float PitchFixed
        {
            get { return _vmOptions.PitchFixed; }
            set
            {
                _vmOptions.PitchFixed = value;
                this.NotifyOfPropertyChange(() => this.PitchFixed);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Flag if a fixed Pitch should be used..
        /// </summary>
        public bool IsRollFixed
        {
            get { return _vmOptions.IsRollFixed; }
            set
            {
                _vmOptions.IsRollFixed = value;
                this.NotifyOfPropertyChange(() => this.IsRollFixed);

                // Save the options to the project
                Save();
            }
        }

        /// <summary>
        /// Roll fixed.
        /// </summary>
        public float RollFixed
        {
            get { return _vmOptions.RollFixed; }
            set
            {
                _vmOptions.RollFixed = value;
                this.NotifyOfPropertyChange(() => this.RollFixed);

                // Save the options to the project
                Save();
            }
        }

        #endregion

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
        public VesselMountViewModel()
            :base("Vessel Mount ViewModel")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _events.Subscribe(this);
            _pm = IoC.Get<PulseManager>();

            // Get the singleton ADCP connection
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Get Options
            GetOptions();

            // Set the list
            CommPortList = SerialOptions.PortOptions;
            BaudRateList = SerialOptions.BaudRateOptions;

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AveragingView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Set the lists
            InitLists();
            InitValues();
        }


        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Screen Data

        /// <summary>
        /// Screen the ensemble with the given options.
        /// </summary>
        /// <param name="ensemble">Ensemble to screen.</param>
        public void Screen(ref DataSet.Ensemble ensemble)
        {
            // Vessel Mount Options
            //if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.VesselMountOptions != null)
            //{
                VesselMount.VesselMountScreen.Screen(ref ensemble, _vmOptions);
            //}
            
        }

        #endregion

        #region Vessel Mount Options

        /// <summary>
        /// Get the options from the project if a project is selected.
        /// If a project is not selected, use the default options.
        /// </summary>
        private void GetOptions()
        {
            // Check if a project is selected
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    // Check if the project has the options
                    if (_pm.SelectedProject.Configuration.VesselMountOptions != null)
                    {
                        _vmOptions = _pm.SelectedProject.Configuration.VesselMountOptions;
                    }
                    else
                    {
                        _vmOptions = new VesselMountOptions();
                    }
                }
            }
            else
            {
                // Get the pulse configuration
                _vmOptions = _pm.AppConfiguration.GetVesselMountOptions();

                // If no configuration has been create, then create a new one
                if (_vmOptions == null)
                {
                    _vmOptions = new VesselMountOptions();
                }
            }
        }

        /// <summary>
        /// Save the project options.
        /// </summary>
        private void Save()
        {
            // Save the options to the project
            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.VesselMountOptions = _vmOptions;
                    _pm.SelectedProject.Save();
                }
            }

            // Update Pulse
            _pm.AppConfiguration.SaveVesselMountOptions(_vmOptions);
        }

        #endregion

        #region Init

        /// <summary>
        /// Initialize the lists.
        /// </summary>
        private void InitLists()
        {
            if (_vmOptions != null)
            {
                HeadingSourceList = _vmOptions.HeadingSourceList;
                TiltSourceList = _vmOptions.TiltSourceList;
            }
        }

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void InitValues()
        {
            if (_pm.IsProjectSelected)
            {
                // Set the selected projects GPS1 options to the last GPS1 options
                // Then make the connection
                if (_adcpConnection.Gps1SerialPort != null)
                {
                    _pm.SelectedProject.Configuration.Gps1SerialOptions = _adcpConnection.Gps1SerialPort.SerialOptions;
                    _SelectedGps1CommPort = _adcpConnection.Gps1SerialPort.SerialOptions.Port;
                    _SelectedGps1BaudRate = _adcpConnection.Gps1SerialPort.SerialOptions.BaudRate;
                    if (_adcpConnection.IsGps1SerialPortEnabled)
                    {
                        _adcpConnection.ReconnectGps1Serial(_pm.SelectedProject.Configuration.Gps1SerialOptions);
                    }
                }

                // Set the selected projects GPS2 options to the last GPS2 options
                // Then make the connection
                if (_adcpConnection.Gps2SerialPort != null)
                {
                    _pm.SelectedProject.Configuration.Gps2SerialOptions = _adcpConnection.Gps2SerialPort.SerialOptions;
                    _SelectedGps2CommPort = _adcpConnection.Gps2SerialPort.SerialOptions.Port;
                    _SelectedGps2BaudRate = _adcpConnection.Gps2SerialPort.SerialOptions.BaudRate;
                    if (_adcpConnection.IsGps2SerialPortEnabled)
                    {
                        _adcpConnection.ReconnectGps2Serial(_pm.SelectedProject.Configuration.Gps2SerialOptions);
                    }
                }

                // Set the selected projects Nmea1 options to the last Nmea1 options
                // Then make the connection
                if (_adcpConnection.Nmea1SerialPort != null)
                {
                    _pm.SelectedProject.Configuration.Nmea1SerialOptions = _adcpConnection.Nmea1SerialPort.SerialOptions;
                    _SelectedNmea1CommPort = _adcpConnection.Nmea1SerialPort.SerialOptions.Port;
                    _SelectedNmea1BaudRate = _adcpConnection.Nmea1SerialPort.SerialOptions.BaudRate;
                    if (_adcpConnection.IsNmea1SerialPortEnabled)
                    {
                        _adcpConnection.ReconnectNmea1Serial(_pm.SelectedProject.Configuration.Nmea1SerialOptions);
                    }
                }

                // Set the selected projects Nmea2 options to the last Nmea2 options
                // Then make the connection
                if (_adcpConnection.Nmea2SerialPort != null)
                {
                    _pm.SelectedProject.Configuration.Nmea2SerialOptions = _adcpConnection.Nmea2SerialPort.SerialOptions;
                    _SelectedNmea2CommPort = _adcpConnection.Nmea2SerialPort.SerialOptions.Port;
                    _SelectedNmea2BaudRate = _adcpConnection.Nmea2SerialPort.SerialOptions.BaudRate;
                    if (_adcpConnection.IsNmea2SerialPortEnabled)
                    {
                        _adcpConnection.ReconnectNmea2Serial(_pm.SelectedProject.Configuration.Nmea2SerialOptions);
                    }
                }

            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// A new project was selected.  Get the new options.
        /// </summary>
        /// <param name="prjEvent">Project event.</param>
        public void Handle(ProjectEvent prjEvent)
        {
            // Get the options
            GetOptions();

            // Initialize the values from the new project
            InitValues();
        }

        #endregion
    }
}
