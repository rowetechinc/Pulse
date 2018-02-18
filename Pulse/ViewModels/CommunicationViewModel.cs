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
 * 10/08/2013      RC          3.2.0      Initial coding
 * 10/14/2013      RC          3.2.0      Got connections to work with ADCP serial port.
 * 10/15/2013      RC          3.2.0      Save the ADCP serial port options to the Pulse Database.
 * 12/12/2013      RC          3.2.0      Reset the CommPortList in ScanForAdcp().
 * 04/09/2014      RC          3.2.4      Set the Selected ADCP port and baudrate from the current connection.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
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
    /// TODO: Update summary.
    /// </summary>
    public class CommunicationViewModel : PulseViewModel
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

        #endregion

        #region Properties

        #region Lists

        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        public List<string> CommPortList { get; set; }

        /// <summary>
        /// List of all the baud rate options.
        /// </summary>
        public List<int> BaudRateList { get; set; }

        #endregion

        #region Selected Items

        #region ADCP

        /// <summary>
        /// Selected ADCP Comm Port.
        /// </summary>
        private string _SelectedAdcpCommPort;
        /// <summary>
        /// Selected ADCP Comm Port.
        /// </summary>
        public string SelectedAdcpCommPort
        {
            get { return _SelectedAdcpCommPort; }
            set
            {
                _SelectedAdcpCommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedAdcpCommPort);
                this.NotifyOfPropertyChange(() => this.IsSetAdcpConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.AdcpSerialOptions.SerialOptions.Port = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectAdcpSerial(_pm.SelectedProject.Configuration.AdcpSerialOptions.SerialOptions);
                }

                // Update the pulse database for latest valeus
                _pm.UpdateAdcpSerialCommPort(value);
            }
        }

        /// <summary>
        /// Selected ADCP Baud Rate.
        /// </summary>
        private int _SelectedAdcpBaudRate;
        /// <summary>
        /// Selected ADCP Baud Rate.
        /// </summary>
        public int SelectedAdcpBaudRate
        {
            get { return _SelectedAdcpBaudRate; }
            set
            {
                _SelectedAdcpBaudRate = value;
                this.NotifyOfPropertyChange(() => this.SelectedAdcpBaudRate);
                this.NotifyOfPropertyChange(() => this.IsSetAdcpConnAvail);

                // Set the value to the project
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    _pm.SelectedProject.Configuration.AdcpSerialOptions.SerialOptions.BaudRate = value;

                    // Reconnect the serial port
                    _adcpConnection.ReconnectAdcpSerial(_pm.SelectedProject.Configuration.AdcpSerialOptions.SerialOptions);
                }

                // Update the pulse database for latest values
                _pm.UpdateAdcpSerialBaudRate(value);
            }
        }

        /// <summary>
        /// Set if ADCP connection is available.
        /// </summary>
        public bool IsSetAdcpConnAvail
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedAdcpCommPort) && SelectedAdcpBaudRate > 0;
            }
        }

        #endregion

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
            }
        }

        ///// <summary>
        ///// Set if GPS1 connection is available.
        ///// </summary>
        //public bool IsSetGps1ConnAvail
        //{
        //    get
        //    {
        //        return !string.IsNullOrEmpty(SelectedGps1CommPort) && SelectedGps1BaudRate > 0;
        //    }
        //}

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

        #endregion

        #region Visibility

        /// <summary>
        /// Is GPS2 visibile.  GPS2 is visible for VM systems.
        /// </summary>
        private bool _IsGps2Visible;
        /// <summary>
        /// Is GPS2 visibile.  GPS2 is visible for VM systems.
        /// </summary>
        public bool IsGps2Visible
        {
            get { return _IsGps2Visible; }
            set
            {
                _IsGps2Visible = value;
                this.NotifyOfPropertyChange(() => this.IsGps2Visible);
            }
        }

        /// <summary>
        /// Is Nmea1 visible.  NMEA1 is visible for VM systems.
        /// </summary>
        private bool _IsNmea1Visible;
        /// <summary>
        /// Is Nmea1 visible.  NMEA1 is visible for VM systems.
        /// </summary>
        public bool IsNmea1Visible
        {
            get { return _IsNmea1Visible; }
            set
            {
                _IsNmea1Visible = value;
                this.NotifyOfPropertyChange(() => this.IsNmea1Visible);
            }
        }

        /// <summary>
        /// Is Nmea2 visible.  NMEA2 is visible for VM systems.
        /// </summary>
        private bool _IsNmea2Visible;
        /// <summary>
        /// Is Nmea2 visible.  NMEA2 is visible for VM systems.
        /// </summary>
        public bool IsNmea2Visible
        {
            get { return _IsNmea2Visible; }
            set
            {
                _IsNmea2Visible = value;
                this.NotifyOfPropertyChange(() => this.IsNmea2Visible);
            }
        }

        #endregion

        #region ADCP Image

        /// <summary>
        /// ADCP Image source.
        /// </summary>
        private string _AdcpImageSrc;
        /// <summary>
        /// ADCP Image source.
        /// </summary>
        public string AdcpImageSrc
        {
            get { return _AdcpImageSrc; }
            set
            {
                _AdcpImageSrc = value;
                this.NotifyOfPropertyChange(() => this.AdcpImageSrc);
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

        /// <summary>
        /// Command to scan for available ADCP.
        /// </summary>
        public ReactiveCommand<object> ScanAdcpCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Intialize the view model.
        /// </summary>
        public CommunicationViewModel()
            : base("Communications")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Get the singleton ADCP connection
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Initialize the values
            InitValues();

            // Set the list
            CommPortList = SerialOptions.PortOptions;
            BaudRateList = SerialOptions.BaudRateOptions;

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ScanAdcpView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Scan for ADCP command
            ScanAdcpCommand = ReactiveCommand.Create();
            ScanAdcpCommand.Subscribe(_ => ScanForAdcp());
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Init

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void InitValues()
        {
            IsGps2Visible = false;
            IsNmea1Visible = false;
            IsNmea2Visible = false;

            if (_pm.IsProjectSelected)
            {
                // Get the previous options if they have already been set
                SelectedAdcpCommPort = _adcpConnection.AdcpSerialPort.SerialOptions.Port;
                SelectedAdcpBaudRate = _adcpConnection.AdcpSerialPort.SerialOptions.BaudRate;

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

                // Set the product image
                AdcpImageSrc = ProductImage.GetProductImage(_pm.SelectedProject);

                // Based off mode, it will determine what is visible
                switch (_pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode)
                {
                    case DeploymentOptions.AdcpDeploymentMode.VM:
                        IsGps2Visible = true;
                        IsNmea1Visible = true;
                        IsNmea2Visible = true;
                        break;
                    default:
                        IsGps2Visible = false;
                        IsNmea1Visible = false;
                        IsNmea2Visible = false;
                        break;
                }

            }
        }

        #endregion

        #region Scan For ADCP

        /// <summary>
        /// Scan for any available ADCP and set the selected ADCP options.
        /// </summary>
        private void ScanForAdcp()
        {
            CommPortList = SerialOptions.PortOptions;

            List<AdcpSerialPort.AdcpSerialOptions> serialConnOptions = _adcpConnection.ScanSerialConnection();

            // Set the first serial port to the ADCP selected options
            if (serialConnOptions.Count > 0)
            {
                SelectedAdcpCommPort = serialConnOptions[0].SerialOptions.Port;
                SelectedAdcpBaudRate = serialConnOptions[0].SerialOptions.BaudRate;

                // Set the ADCP serial options
                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.AdcpSerialOptions = serialConnOptions[0];
                }
            }
        }





        #endregion
    }
}
