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
* 11/15/2013      RC          3.2.0      Initial coding
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
    using System.ComponentModel;
    using ReactiveUI;
    using System.Windows;
    using System.Threading;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple compass cal process.
    /// </summary>
    public class SimpleCompassCalViewModel : PulseViewModel, IDeactivate
    {
        #region Variables

        #region Defaults

        /// <summary>
        /// Default Point 1 location.
        /// </summary>
        public const double DEFAULT_POINT1_LOC = 0.0;

        /// <summary>
        /// Default Point 2 location.
        /// </summary>
        public const double DEFAULT_POINT2_LOC = 90.0;

        /// <summary>
        /// Default Point 3 location.
        /// </summary>
        public const double DEFAULT_POINT3_LOC = 180.0;

        /// <summary>
        /// Default Point 4 location.
        /// </summary>
        public const double DEFAULT_POINT4_LOC = 270.0;

        /// <summary>
        /// Default flag for validating the calibration score.
        /// </summary>
        public const bool DEFAULT_IS_VALIDATE_CAL_SCORE = false;

        /// <summary>
        /// Default Database server address to store the results.
        /// </summary>
        public const string DEFAULT_DB_SERVER = "192.168.1.73";

        /// <summary>
        /// Default port for the database.
        /// </summary>
        public const int DEFAULT_DB_PORT = 5984;

        /// <summary>
        /// Default Database database.
        /// </summary>
        public const string DEFAULT_DB_DATABASE = "adcp1";

        /// <summary>
        /// Default result text file name.
        /// </summary>
        public const string DEFAULT_RESULT_TXT = "CompassCalResults.csv";

        #endregion

        // Setup logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// EventAggregator to receive global events.
        /// This will be used to get ensembles for a serial
        /// number.
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Adcp Connection.
        /// </summary>
        private AdcpConnection _adcpConn;

        /// <summary>
        /// Codec to decode compass data.
        /// </summary>
        private RTI.PniPrimeCompassBinaryCodec _compassCodec;

        /// <summary>
        /// Data from the PNI compass.  This includes the heading, pitch and
        /// roll.
        /// </summary>
        private PniPrimeCompassBinaryCodec.PniDataResponse _compassDataResponse;

        #endregion

        #region Properties

        #region Status Bar

        /// <summary>
        /// Store event information.
        /// </summary>
        private StatusEvent _statusBarEvent;

        /// <summary>
        /// Text for the status bar.
        /// </summary>
        public string StatusBarText
        {
            get { return _statusBarEvent.Message; }
        }

        /// <summary>
        /// Background color of the status bar.
        /// </summary>
        public string StatusBarBackground
        {
            get { return _statusBarEvent.Color; }
        }

        /// <summary>
        /// The duration to show the status bar event.
        /// The duration is subtracted by 1 so the event
        /// can disappear for 1 second.
        /// </summary>
        public string StatusBarDurationStart
        {
            get
            {
                return "0:0:" + (_statusBarEvent.Duration - 1).ToString();
            }
        }

        /// <summary>
        /// The total duration of the event to be displayed.
        /// </summary>
        public string StatusBarDurationStop
        {
            get { return "0:0:" + (_statusBarEvent.Duration).ToString(); }
        }

        #endregion

        #region Compass Cal

        /// <summary>
        /// Enable or disable the compass cal button.
        /// </summary>
        private bool _CanStartCompassCal;
        /// <summary>
        /// Enable or disable the compass cal button.
        /// </summary>
        public bool CanStartCompassCal
        {
            get { return _CanStartCompassCal; }
            set
            {
                _CanStartCompassCal = value;
                this.NotifyOfPropertyChange(() => this.CanStartCompassCal);
            }
        }

        #endregion

        #region Cal Samples

        /// <summary>
        /// Value representing the number
        /// of sample completed in the compass
        /// cal.
        /// </summary>
        private UInt32 _calSamples;
        /// <summary>
        /// Value representing the number of 
        /// samples completed in the compass cal property.
        /// </summary>
        public UInt32 CalSamples
        {
            get { return _calSamples; }
            set
            {
                _calSamples = value;
                this.NotifyOfPropertyChange(() => this.CalSamples);
            }
        }

        /// <summary>
        /// String of the position to go to 
        /// next based off the current sample.
        /// </summary>
        private string _CalPosition;
        /// <summary>
        /// String of the position to go to 
        /// next based off the current sample.
        /// </summary>
        public string CalPosition
        {
            get { return _CalPosition; }
            set
            {
                _CalPosition = value;
                this.NotifyOfPropertyChange(() => this.CalPosition);
            }
        }

        #endregion

        #region Compass Cal Buttons

        /// <summary>
        /// Use the same button to preform two different
        /// operations based off the state.
        /// </summary>
        private string _compassCalButtonLabel;
        /// <summary>
        /// Use the same button to preform two different 
        /// operations based off the state. 
        /// Compass Cal Button Label property.
        /// </summary>
        public string CompassCalButtonLabel
        {
            get { return _compassCalButtonLabel; }
            set
            {
                _compassCalButtonLabel = value;
                this.NotifyOfPropertyChange(() => this.CompassCalButtonLabel);
            }
        }

        /// <summary>
        /// Flag if in Compass Cal mode or not.
        /// This will also change the button label.
        /// </summary>
        private bool _isCompassCal;
        /// <summary>
        /// Flag if in Compass Cal mode property.
        /// </summary>
        public bool IsCompassCal
        {
            get { return _isCompassCal; }
            set
            {
                _isCompassCal = value;

                if (_isCompassCal)
                {
                    CompassCalButtonLabel = "Stop";
                }
                else
                {
                    CompassCalButtonLabel = "Start";
                }

                this.NotifyOfPropertyChange(() => this.IsCompassCal);
                this.NotifyOfPropertyChange(() => this.CanTakeCompassCalSample);
            }
        }

        /// <summary>
        /// Flag if a sample can be taken.
        /// This will check if we are currently getting any points.
        /// </summary>
        public bool CanTakeCompassCalSample
        {
            get { return IsCompassCal; }
        }

        /// <summary>
        /// Use the same button to preform two different operations
        /// based off the state.
        /// </summary>
        private string _compassDataButtonLabel;
        /// <summary>
        /// Use the same button to preform two different operations
        /// base off the state.  Compass Data Button label property.
        /// </summary>
        public string CompassDataButtonLabel
        {
            get { return _compassDataButtonLabel; }
            set
            {
                _compassDataButtonLabel = value;
                this.NotifyOfPropertyChange(() => this.CompassDataButtonLabel);
            }
        }

        #endregion

        #region Compass Connected Flag

        /// <summary>
        /// Flag if in Compass Data collecting mode.
        /// This will also change the button label.
        /// </summary>
        private bool _isCompassConnected;
        /// <summary>
        /// Flag if in Compass Data collecting mode property.
        /// </summary>
        public bool IsCompassConnected
        {
            get { return _isCompassConnected; }
            set
            {
                _isCompassConnected = value;

                if (_isCompassConnected)
                {
                    CompassDataButtonLabel = "Disconnect";
                }
                else
                {
                    CompassDataButtonLabel = "Connect";
                }

                this.NotifyOfPropertyChange(() => this.IsCompassConnected);
            }
        }

        #endregion

        #region Admin

        /// <summary>
        /// Set a flag if the user is an Admin.
        /// </summary>
        private bool _isAdmin;
        /// <summary>
        /// Set a flag if the user is an Admin.
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                this.NotifyOfPropertyChange(() => this.IsAdmin);
            }
        }

        #endregion

        #region ADCP Config

        /// <summary>
        /// ADCP Configuration.
        /// </summary>
        private AdcpConfiguration _adcpConfig;

        /// <summary>
        /// ADCP Serial Number
        /// </summary>
        public string AdcpSerialNumber
        {
            get { return _adcpConfig.SerialNumber.ToString(); }
        }

        /// <summary>
        /// ADCP Firmware version.
        /// </summary>
        public string AdcpFirmware
        {
            get { return _adcpConfig.AdcpSerialOptions.Firmware.ToString(); }
        }

        #endregion

        #region Result File

        /// <summary>
        /// Result Text file.  The results will be stored to this
        /// file.
        /// </summary>
        private string _resultTxtFile;
        /// <summary>
        /// Result Text file.  The results will be stored to this
        /// file.
        /// </summary>
        public string ResultTextFile
        {
            get { return _resultTxtFile; }
            set
            {
                _resultTxtFile = value;
                this.NotifyOfPropertyChange(() => this.ResultTextFile);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to run the compass cal.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> CompassCalCommand { get; protected set; }

        /// <summary>
        /// Take a compass cal sample.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> TakeCompassCalSampleCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public SimpleCompassCalViewModel()
            : base("Simple Compass Cal")
        {
            // Initialize values
            _eventAggregator = IoC.Get<IEventAggregator>();
            _adcpConn = IoC.Get<AdcpConnection>();
            _compassCodec = new RTI.PniPrimeCompassBinaryCodec();
            _compassDataResponse = new PniPrimeCompassBinaryCodec.PniDataResponse();
            IsAdmin = Pulse.Commons.IsAdmin();                                                      // Set if the user is an admin
            ResultTextFile = RTI.Pulse.Commons.GetAppStorageDir() + @"\" + DEFAULT_RESULT_TXT;
            _statusBarEvent = new StatusEvent("");
            _adcpConfig = new AdcpConfiguration();
            CompassCalButtonLabel = "Start";

            // Start or stop compass calibration
            CompassCalCommand = ReactiveCommand.CreateAsyncTask(_ => _workerCompassCal_DoWork());

            // Create a command to take a sample 
            TakeCompassCalSampleCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.CanTakeCompassCalSample, x => x.Value), _ => OnTakeCompassCalSample());

            // Setup the serial port to receive serial port events
            SetupAdcpEvents();
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            // Remove subscriptions
            UnsubscribeAdcpEvents();

            // Dispose Codec
            _compassCodec.Dispose();
        }

        #region Initialize

        /// <summary>
        /// Initialize all the Compass Calibration
        /// ranges.
        /// </summary>
        private void InitializeCompassCalValues()
        {
            CalSamples = 0;
            CalPosition = "";
        }

        /// <summary>
        /// Create new serial ports with the options
        /// from the serial options.
        /// This will also subscribe to receive events
        /// from the serial ports and clear the buffers.
        /// </summary>
        private void SetupAdcpEvents()
        {
            // If the serial port was previous connected, 
            // Unsubscribe events.
            if (_adcpConn.AdcpSerialPort != null)
            {
                UnsubscribeAdcpEvents();

                // Subscribe to receive event when data received
                _adcpConn.AdcpSerialPort.ReceiveAdcpSerialDataEvent += new RTI.AdcpSerialPort.ReceiveAdcpSerialDataEventHandler(On_ReceiveAdcpSerialDataEvent);
                _adcpConn.AdcpSerialPort.ReceiveCompassSerialDataEvent += new RTI.AdcpSerialPort.ReceiveCompassSerialDataEventHandler(On_ReceiveCompassSerialDataEvent);
                _adcpConn.AdcpSerialPort.ReceiveSerialData += new RTI.SerialConnection.ReceiveSerialDataEventHandler(On_AdcpReceiveSerialData);

                // Wait for incoming cal samples
                InitializeCompassCalValues();
                _compassCodec.CompassEvent += new RTI.PniPrimeCompassBinaryCodec.CompassEventHandler(CompassCodecEventHandler);
            }
        }

        /// <summary>
        /// Unsubscribe from the ADCP serial port events.
        /// </summary>
        private void UnsubscribeAdcpEvents()
        {
            _adcpConn.AdcpSerialPort.ReceiveAdcpSerialDataEvent -= On_ReceiveAdcpSerialDataEvent;
            _adcpConn.AdcpSerialPort.ReceiveCompassSerialDataEvent -= On_ReceiveCompassSerialDataEvent;
            _compassCodec.CompassEvent -= CompassCodecEventHandler;
            _adcpConn.AdcpSerialPort.ReceiveSerialData -= On_AdcpReceiveSerialData;
        }

        #endregion

        #region Compass

        /// <summary>
        /// Set the ADCP to compass mode.  If we are currently doing a compass 
        /// calibration, send a warning.  Set the ADCP to compass mode.  Set 
        /// the ADCP status to compass mode.
        /// </summary>
        /// <returns>TRUE = Compass connected.</returns>
        private bool CompassConnect()
        {
            // If we are currently doing a compass cal,
            // give a warning and do not continue
            if (IsCompassCal)
            {
                SetStatusBar(new StatusEvent("Compass calibration in progress.  Stop the calibration first.", MessageBoxImage.Error));
                return false;
            }

            // Check if we need to be put in compass mode
            if (!IsCompassConnected)
            {
                // Stop ping just in case it is pinging
                // Send command to stop pinging
                // If the command is not sent properly,
                // send a message to the user that a connection
                // is probably not made to the ADCP.
                if (!_adcpConn.AdcpSerialPort.StopPinging())
                {
                    // Try to fix the issue with the ADCP
                    // or give a warning to the user
                    //if (!SerialCommunicationIssue())
                    //{
                    // Do not continue trying to setup the ADCP for compass mode
                    SetStatusBar(new StatusEvent("Compass Issue.  Could not connect to compass.", MessageBoxImage.Error));
                    return false;
                    //}
                }

                IsCompassConnected = true;

                // Send the commands
                // Put ADCP in Compass mode
                // Set the serial port to COMPASS mode to decode compass data
                if (!_adcpConn.AdcpSerialPort.StartCompassMode())
                {
                    SetStatusBar(new StatusEvent("Compass Issue.  Could not connect to compass.", MessageBoxImage.Error));
                    return false;
                }

                Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);                  // Delay for 485 response

                // Clear the buffer of any data from last calibration
                _compassCodec.ClearIncomingData();
            }

            return true;
        }

        /// <summary>
        /// Disconnect the ADCP from Compass mode.  This will
        /// send the command to disconnect the compass.  IT
        /// will then set the ADCP status to ADCP.
        /// </summary>
        private void CompassDisconnect()
        {
            // Turn on compass interval outputing
            // Stop ADCP from compass mode
            // Set the serial port to ADCP mode to decode ADCP data
            _adcpConn.AdcpSerialPort.StopCompassMode();

            // Set flag that disconnected
            IsCompassConnected = false;
        }

        /// <summary>
        /// Send a command an value to the ADCP.  This will
        /// send the command and if required a value to the adcp
        /// for the compass to process.
        /// </summary>
        /// <param name="id">Command ID.</param>
        /// <param name="value">Value for command if required.</param>
        private void SendCompassConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID id, object value)
        {
            // Connect the compass
            CompassConnect();

            // Send command to Read Compass data
            _adcpConn.AdcpSerialPort.SendCompassCommand(RTI.PniPrimeCompassBinaryCodec.SetConfigCommand(id, value));

            CompassDisconnect();
        }

        #endregion

        #region Compass Cal

        /// <summary>
        /// Work to do when the backgroundworker is started.  This will ask if a compass calibration wants to
        /// be started.  If the user presses OK, then the calibration will start.
        /// </summary>
        private async Task _workerCompassCal_DoWork()
        {
            if (IsCompassCal)
            {
                await Task.Run(() => StopCompassCal());
            }
            else
            {
                // Ask if they want to start a calibration
                MessageBoxResult result = MessageBox.Show("Begin compass calibration?", "Compass Calibration", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    // Run the Compass Cal
                    await Task.Run(() => StartCalibration());
                }
            }
        }

        /// <summary>
        /// Start the calibration process.
        /// This will get the system info, turn off
        /// auto sample and start to collect points.
        /// </summary>
        private void StartCalibration()
        {
            // Set button label
            CompassCalButtonLabel = "Stop";

            // Read Current Settings of the compass
            //OnReadCompass(null);
            ReadAdcpConfig();

            // Turn off Auto Sample
            SendCompassConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, false);

            //// Check if compass cal cancel is called
            //if (_workerCompassCal.CancellationPending)
            //{
            //    return;
            //}

            // Sleep to allow the auto sample to be set
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);

            // Run the Initial Points
            RunCompassCal();

            //// Check if compass cal cancel is called
            //if (_workerCompassCal.CancellationPending)
            //{
            //    return;
            //}
        }

        /// <summary>
        /// Start a compass calibration.  This will start the
        /// calibration test.
        /// </summary>
        private void RunCompassCal()
        {
            // Connect to the compass
            if (!CompassConnect())
            {
                // Could not get into compass mode
                return;
            }

            //// Check if compass cal cancel is called
            //if (_workerCompassCal.CancellationPending)
            //{
            //    return;
            //}

            // Reset the ranges
            InitializeCompassCalValues();

            // Start compass in calibration mode with acceleration calibration
            _adcpConn.AdcpSerialPort.StartCompassCal(false);

            // Set flag that doing a compass calibration
            IsCompassCal = true;
        }

        /// <summary>
        /// Stop a compass calibration.
        /// If the compass calibration is in process,
        /// this will stop the calibration process.
        /// </summary>
        private void StopCompassCal()
        {
            // Disable Compass Cal button
            CanStartCompassCal = false;

            // Set flag we are not compass calibrating
            IsCompassCal = false;

            // Stop Calibration mode
            _adcpConn.AdcpSerialPort.StopCompassCal();

            // Sleep to allow the compass cal to stop 
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);

            // Turn on Auto Sample
            SendCompassConfigCommand(RTI.PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling, true);

            // Sleep to allow the auto sample to be set
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);

            // Sleep to allow the auto sample to be set
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE * 2);

            // Disconnect from the compass
            CompassDisconnect();

            // Enable compass cal button
            CanStartCompassCal = true;
        }

        /// <summary>
        /// Complete the compass cal.  If the compass cal was successful,
        /// call this method to wrap up the compass cal.  This will record
        /// the results, and display compass cal is complete.
        /// </summary>
        private void CompleteCompassCal()
        {
            if (IsAdmin)
            {
                // Output Results to a file
                WriteResults();
            }

            // Write results to maintence log
            WriteResultsToMaintenceLog();

            SetStatusBar(new StatusEvent("Compass Calibration Complete."));
            CalSamples = 0;
            CalPosition = "Calibration Complete";

            // Set flag we are not compass calibrating
            IsCompassCal = false;
        }

        #endregion

        #region Take compass cal sample

        /// <summary>
        /// Take a compass cal sample.
        /// </summary>
        private async Task OnTakeCompassCalSample()
        {
            if (IsCompassCal)
            {
                // Take a sample
                await Task.Run(() => NextStep(CalSteps.CalPoint));
            }
            else
            {
                await Task.Run(() => SetStatusBar(new StatusEvent("Compass Cal has not started.  Start the compass cal first.", MessageBoxImage.Error)));

            }
        }

        #endregion

        #region Automata

        /// <summary>
        /// All the calibration steps.
        /// </summary>
        private enum CalSteps
        {

            /// <summary>
            /// This will tell the compass to take a calibration sample.
            /// </summary>
            CalPoint,

        }

        /// <summary>
        /// Handle all the steps in the calibration process.
        /// Give the step type and this will handle the process.
        /// </summary>
        /// <param name="step">Step to process.</param>
        private void NextStep(CalSteps step)
        {
            switch (step)
            {
                case CalSteps.CalPoint:
                    _adcpConn.AdcpSerialPort.SendCompassCommand(RTI.PniPrimeCompassBinaryCodec.GetTakeUserCalSampleCommand());   // Get a calibration sample
                    break;
            }
        }

        #endregion

        #region Status Bar

        /// <summary>
        /// Set the status bar with the latest message.
        /// </summary>
        /// <param name="statusEvent"></param>
        private void SetStatusBar(StatusEvent statusEvent)
        {
            _statusBarEvent = statusEvent;
            this.NotifyOfPropertyChange(() => this.StatusBarText);
        }

        #endregion

        #region ADCP Config

        /// <summary>
        /// Read the ADCP configuration.
        /// </summary>
        private void ReadAdcpConfig()
        {
            _adcpConfig = _adcpConn.AdcpSerialPort.GetAdcpConfiguration();
            this.NotifyOfPropertyChange(() => this.AdcpSerialNumber);
            this.NotifyOfPropertyChange(() => this.AdcpFirmware);
        }

        #endregion

        #region Write Results

        /// <summary>
        /// Create a string of all the results.
        /// </summary>
        /// <returns>String of the results of the calibration.</returns>
        private string ResultsString()
        {
            // Write the results
            var result = new StringBuilder();
            result.Append(AdcpSerialNumber + ", ");
            result.Append(AdcpFirmware);

            return result.ToString();
        }

        /// <summary>
        /// Write the result to a file.
        /// </summary>
        private void WriteResults()
        {
            StringBuilder result;

            // If the file does not exist
            // Add the header to the file
            if (!File.Exists(ResultTextFile))
            {
                result = new StringBuilder();
                result.Append("SerialNumber" + ", ");
                result.Append("Firmware");

                // Open write and write the line to the file
                using (StreamWriter w = File.AppendText(ResultTextFile))
                {
                    w.WriteLine(result.ToString());
                    w.Flush();
                    w.Close();
                }
            }

            // Open and write the line to the file
            using (StreamWriter w = File.AppendText(ResultTextFile))
            {
                w.WriteLine(ResultsString());
                w.Flush();
                w.Close();
            }
        }

        /// <summary>
        /// Create a maintence entry and add it to the
        /// maintence log.
        /// </summary>
        private void WriteResultsToMaintenceLog()
        {
            // Create an entry with the results
            // Add it to the list
            MaintenceEntry entry = new MaintenceEntry(MaintenceEntry.EntryId.UserCompassCal, ResultsString(), "");

            _adcpConn.AddMaintenceEntry(entry);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// When the serial port receives a new cal sample
        /// from the Compass calibration, set the new sample value.
        /// </summary>
        /// <param name="data">Data from the compass.</param>
        public void CompassCodecEventHandler(RTI.PniPrimeCompassBinaryCodec.CompassEventArgs data)
        {
            // Sample received
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kUserCalSampCount)
            {
                CalSamples = (UInt32)data.Value;
                CalPosition = RTI.PniPrimeCompassBinaryCodec.MagCalibrationPosition((UInt32)data.Value);
            }

            // Calibration Score received
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kUserCalScore)
            {
                // Save the compass cal automatically after getting a score
                _adcpConn.AdcpSerialPort.SaveCompassCal();

                //
                // Wait for kSaveDone event to be received to move on
                //
            }

            // Save complete
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kSaveDone)
            {
                SetStatusBar(new StatusEvent("Compass Calibration Value saved."));

                // Turn off compass cal
                // BY CALLING STOP HERE, THE START/STOP BUTTON WILL DISPLAY START EVEN
                // THOUGH THE POST POINTS STILL NEED TO BE COLLECTED
                StopCompassCal();
            }

            // Default Compass Cal Mag complete
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kFactoryUserCalDone)
            {
                // Send command to save the Compass cal
                _adcpConn.AdcpSerialPort.SaveCompassCal();
            }

            // Default Compass Cal Accel complete
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kFactoryInclCalDone)
            {
                // Send command to save the Compass cal
                _adcpConn.AdcpSerialPort.SendCompassCommand(RTI.PniPrimeCompassBinaryCodec.SaveCompassCalCommand());
            }
        }

        /// <summary>
        /// Pass data from the ADCP serial port to the
        /// RecorderManager to be parsed.
        /// </summary>
        /// <param name="data">Data received from the ADCP serial port in ADCP mode.</param>
        private void On_ReceiveAdcpSerialDataEvent(byte[] data)
        {

        }

        /// <summary>
        /// Pass data from the ADCP serial port to the 
        /// compass codec to be parsed.
        /// </summary>
        /// <param name="data">Data received from the ADCP serial port in Compass mode.</param>
        private void On_ReceiveCompassSerialDataEvent(byte[] data)
        {
            _compassCodec.AddIncomingData(data);
        }

        /// <summary>
        /// When data is received from the ADCP serial port, update the buffer display.
        /// </summary>
        /// <param name="data">Not Used.</param>
        public void On_AdcpReceiveSerialData(string data)
        {

        }

        #endregion

        #region Deactivate

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        /// <param name="close"></param>
        void IDeactivate.Deactivate(bool close)
        {
            Dispose();
        }

        #endregion
    }
}
