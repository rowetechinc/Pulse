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
 * 09/02/2017      RC          4.5.3      Initial coding
 * 09/05/2017      RC          4.5.3.1    Remove Ship Speed.
 * 
 * 
 * 
 */

using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Data Output.  This will take the incoming data.
    /// Reprocess the data.  Then output the data in the 
    /// new format selected.  The data will come out on the serial port.
    /// </summary>
    public class WpMagDirOutputViewModel : PulseViewModel, IHandle<EnsembleRawEvent>
    {

        #region Variables

        /// <summary>
        /// Event Aggregator to receive the latest ensembles.
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Pulse manager to manage the application.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Serial port.
        /// </summary>
        private AdcpSerialPort _serialPort;

        /// <summary>
        /// Serial Port options.
        /// </summary>
        private SerialOptions _serialOptions;

        /// <summary>
        /// Write to a file.
        /// </summary>
        private BinaryWriter _binaryWriter;

        /// <summary>
        /// Default recording directory.
        /// </summary>
        public const string DEFAULT_RECORD_DIR = @"C:\RTI_Capture";

        /// <summary>
        /// Previous Ship Speed East.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedEast = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed North.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedNorth = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Vertical.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedVert = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed X.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedX = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Y.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedY = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Z.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedZ = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Transverse.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedTransverse = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Longitundial.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedLongitudinal = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// Previous Ship Speed Normal.  Used to remove ship speed from Velocity data.
        /// </summary>
        private float _prevShipSpeedNormal = DataSet.Ensemble.BAD_VELOCITY;

        /// <summary>
        /// DataOutputViewModel options.
        /// </summary>
        private WpMagDirOutputViewOptions _options;

        #endregion

        #region Properties

        /// <summary>
        /// Water Direction and Magitude Tooltip.
        /// </summary>
        public string WaterDirMagTooltip
        {
            get
            {
                return "This is not a standard PD6 output.\nThis will give the Water magnitude and direction.\n";
            }
        }

        #region Data Output

        /// <summary>
        /// Flag to turn on this feature.
        /// </summary>
        public bool IsOutputEnabled
        {
            get { return _options.IsOutputEnabled; }
            set
            {
                _options.IsOutputEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsOutputEnabled);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Minimum Bin Tooltip.
        /// </summary>
        public string DataOutputTooltip
        {
            get
            {
                return "Output the reprocessed data to the serial port.  Select a format to output the data.";
            }
        }

        /// <summary>
        /// Data Output.
        /// </summary>
        private string _DataOutput;
        /// <summary>
        /// Data Output.
        /// </summary>
        public string DataOutput
        {
            get { return _DataOutput; }
            set
            {
                _DataOutput = value;
                this.NotifyOfPropertyChange(() => this.DataOutput);
            }
        }

        /// <summary>
        /// Status Output.
        /// </summary>
        private string _StatusOutput;
        /// <summary>
        /// Status Output.
        /// </summary>
        public string StatusOutput
        {
            get { return _StatusOutput; }
            set
            {
                _StatusOutput = value;
                this.NotifyOfPropertyChange(() => this.StatusOutput);
            }
        }

        #endregion

        #region Ports

        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        private List<string> _CommPortList;
        /// <summary>
        /// List of all the comm ports on the computer.
        /// </summary>
        public List<string> CommPortList
        {
            get { return _CommPortList; }
            set
            {
                _CommPortList = value;
                this.NotifyOfPropertyChange(() => this.CommPortList);
            }
        }

        /// <summary>
        /// List of all the baud rate options.
        /// </summary>
        public List<int> BaudRateList { get; set; }

        /// <summary>
        /// Selected COMM Port.
        /// </summary>
        public string SelectedCommPort
        {
            get { return _options.SelectedCommPort; }
            set
            {
                _options.SelectedCommPort = value;
                this.NotifyOfPropertyChange(() => this.SelectedCommPort);

                // Set the serial options
                _serialOptions.Port = value;

                // Reconnect the ADCP
                ReconnectSerial(_serialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Selected baud rate.
        /// </summary>
        public int SelectedBaud
        {
            get { return _options.SelectedBaud; }
            set
            {
                _options.SelectedBaud = value;
                this.NotifyOfPropertyChange(() => this.SelectedBaud);

                // Set the serial options
                _serialOptions.BaudRate = value;

                // Reconnect the ADCP
                ReconnectSerial(_serialOptions);

                // Reset check to update
                //this.NotifyOfPropertyChange(() => this.CanUpdate);

                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Output Coordinate Transform

        /// <summary>
        /// Selected Format.
        /// </summary>
        public string SelectedTransform
        {
            get { return _options.SelectedTransform; }
            set
            {
                _options.SelectedTransform = value;
                this.NotifyOfPropertyChange(() => this.SelectedTransform);

                UpdateDatabaseOptions();
            }
        }

                /// <summary>
        /// Retransform Tooltip.
        /// </summary>
        public string SelectedTransformTooltip
        {
            get
            {
                return "Select the coordinate transform to output the data.\nShip velocity is orientated with the ship.\nEarth velocity is orientated with the compass.\nInstrument velocity is orientated with X,Y,Z.";
            }
        }

        /// <summary>
        /// List of all the format options.
        /// </summary>
        public List<string> FormatList { get; set; }

        #endregion

        #region Retransform Data

        /// <summary>
        /// Flag if the data should be retransformed.
        /// </summary>
        public bool IsRetransformData
        {
            get { return _options.IsRetransformData; }
            set
            {
                _options.IsRetransformData = value;
                this.NotifyOfPropertyChange(() => this.IsRetransformData);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Retransform Tooltip.
        /// </summary>
        public string RetransformTooltip
        {
            get
            {
                return "Retransform the data using the new heading value.  If the heading is bad or an offset applied, the data needs to be reprocessed with a replacement heading.";
            }
        }

        #endregion

        #region Heading

        /// <summary>
        /// Flag if the retransformed data and output data should use the GPS or Gyro incoming data.
        /// </summary>
        public bool IsUseGpsHeading
        {
            get { return _options.IsUseGpsHeading; }
            set
            {
                _options.IsUseGpsHeading = value;
                this.NotifyOfPropertyChange(() => this.IsUseGpsHeading);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// List of available heading sources.
        /// </summary>
        private List<Transform.HeadingSource> _HeadingSourceList;
        /// <summary>
        /// List of available heading sources.
        /// </summary>
        public List<Transform.HeadingSource> HeadingSourceList
        {
            get { return _HeadingSourceList; }
            set
            {
                _HeadingSourceList = value;
                this.NotifyOfPropertyChange(() => this.HeadingSourceList);
            }
        }

        /// <summary>
        /// Selected heading sources.
        /// </summary>
        public Transform.HeadingSource SelectedHeadingSource
        {
            get { return _options.SelectedHeadingSource; }
            set
            {
                _options.SelectedHeadingSource = value;
                this.NotifyOfPropertyChange(() => this.SelectedHeadingSource);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Gps Heading Tooltip.
        /// </summary>
        public string GpsHeadingTooltip
        {
            get
            {
                return "Replace the Ancillary and Bottom Track heading with the GPS/Gyro heading.\nIt will use the HDT message for the heading value.\nThe data will then be retransformed so the Earth data is using the new heading value.";
            }
        }

        /// <summary>
        /// Heading offset value in degrees.
        /// </summary>
        public float HeadingOffset
        {
            get { return _options.HeadingOffset; }
            set
            {
                _options.HeadingOffset = value;
                this.NotifyOfPropertyChange(() => this.HeadingOffset);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Heading Offset Tooltip.
        /// </summary>
        public string HeadingOffsetTooltip
        {
            get
            {
                return "Add this heading offset to the current heading value.\nThe data will then be retransformed so the Earth data is using the new heading value.\nThis is typicially used to account for magnetic interference of declination.";
            }
        }


        #endregion

        #region Ship Transducer Offset

        /// <summary>
        /// The offset from the tranducer and the ship.  This is need to calculate the Ship coordinate transform.
        /// </summary>
        public float ShipXdcrOffset
        {
            get { return _options.ShipXdcrOffset; }
            set
            {
                _options.ShipXdcrOffset = value;
                this.NotifyOfPropertyChange(() => this.ShipXdcrOffset);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Ship Transducer Heading offset Tooltip.
        /// </summary>
        public string ShipXdcrOffsetTooltip
        {
            get
            {
                return "Beam 0 of the ADCP should be pointed forward.\nIf the ADCP is not pointed forward, use this value to account for the ADCP offset.\nThis offset is used for Ship Coordinate Transform.\nThis is the not the same as Heading offset.\nHeading offset is used for magnetic interference or distortition.\nThis is for physical orientation offset.";
            }
        }

        #endregion

        #region Recording

        /// <summary>
        /// Is Recording data.
        /// </summary>
        public bool IsRecording
        {
            get { return _options.IsRecording; }
            set
            {
                _options.IsRecording = value;
                this.NotifyOfPropertyChange(() => this.IsRecording);

                if (!_options.IsRecording)
                {
                    StopRecord();
                }
                else
                {
                    StartRecord();
                }

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Bytes written.
        /// </summary>
        private long _BytesWritten;
        /// <summary>
        /// Bytes written.
        /// </summary>
        public long BytesWritten
        {
            get { return _BytesWritten; }
            set
            {
                _BytesWritten = value;
                this.NotifyOfPropertyChange(() => this.BytesWritten);

                // Set the string for the file size
                FileSize = MathHelper.MemorySizeString(_BytesWritten);
            }
        }

        /// <summary>
        /// Bytes written as a string.
        /// </summary>
        private string _FileSize;
        /// <summary>
        /// Bytes written as a string.
        /// </summary>
        public string FileSize
        {
            get { return _FileSize; }
            set
            {
                _FileSize = value;
                this.NotifyOfPropertyChange(() => this.FileSize);
            }
        }

        #endregion

        #region Selected Bin

        /// <summary>
        /// Select the bins that will output the water direction and magnitude.
        /// </summary>
        public string SelectedBins
        {
            get { return _options.SelectedBins; }
            set
            {
                _options.SelectedBins = value;
                this.NotifyOfPropertyChange(() => this.SelectedBins);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Add Water Direction and Magitude Tooltip.
        /// </summary>
        public string SelectedBinsTooltip
        {
            get
            {
                return "Select the bins that will output the magnitude and direction of the water.\nThis is a comma seperated list.\nAll listed bins will be in one message.";
            }
        }

        #endregion

        #region Remove Ship Speed

        /// <summary>
        /// Flag to remove the ship speed from the velocity data.
        /// </summary>
        public bool IsRemoveShipSpeed
        {
            get { return _options.IsRemoveShipSpeed; }
            set
            {
                _options.IsRemoveShipSpeed = value;
                this.NotifyOfPropertyChange(() => this.IsRemoveShipSpeed);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Flag if can use Bottom Track to remove ship speed.
        /// </summary>
        public bool CanUseBottomTrackVel
        {
            get { return _options.CanUseBottomTrackVel; }
            set
            {
                _options.CanUseBottomTrackVel = value;
                this.NotifyOfPropertyChange(() => this.CanUseBottomTrackVel);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Flag if can use GPS velocity to remove the ship speed.
        /// This is used as a backup.
        /// </summary>
        public bool CanUseGpsVel
        {
            get { return _options.CanUseGpsVel; }
            set
            {
                _options.CanUseGpsVel = value;
                this.NotifyOfPropertyChange(() => this.CanUseGpsVel);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Remove Ship Speed Tooltip.
        /// </summary>
        public string RemoveShipSpeedTooltip
        {
            get
            {
                return "Remove the ship speed from the velocity data.\nThis will remove the velocity of the ship from the water velocity data.\nUse Bottom Track by default and GPS as a backup for the ship speed.";
            }
        }


        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to scan for available ADCP.
        /// </summary>
        public ReactiveCommand<object> ScanCommand { get; protected set; }

        /// <summary>
        /// Connect the serial port.
        /// </summary>
        public ReactiveCommand<object> ConnectCommand { get; protected set; }

        /// <summary>
        /// Disconnect the serial port.
        /// </summary>
        public ReactiveCommand<object> DisconnectCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        public WpMagDirOutputViewModel()
            : base("Water Profile Direction and Magnitude Data Output")
        {
            base.DisplayName = "Data Output";

            // Subscribe to receive ensembles
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);

            // Get PulseManager
            _pm = IoC.Get<PulseManager>();

            // Get the options from the database
            GetOptionsFromDatabase();

            FormatList = new List<string>();
            FormatList.Add(WpMagDirOutputViewOptions.TRANSFORM_SHIP);
            FormatList.Add(WpMagDirOutputViewOptions.TRANSFORM_EARTH);
            FormatList.Add(WpMagDirOutputViewOptions.TRANSFORM_INSTRUMENT);

            HeadingSourceList = Enum.GetValues(typeof(Transform.HeadingSource)).Cast<Transform.HeadingSource>().ToList();

            _serialOptions = new SerialOptions();
            CommPortList = SerialOptions.PortOptions;
            BaudRateList = SerialOptions.BaudRateOptions;

            DataOutput = "";

            // Scan for ADCP command
            ScanCommand = ReactiveUI.ReactiveCommand.Create();
            ScanCommand.Subscribe(_ => ScanForSerialPorts());

            // Disconnect Serial
            ConnectCommand = ReactiveUI.ReactiveCommand.Create();
            ConnectCommand.Subscribe(_ => ConnectAdcpSerial());

            // Disconnect Serial
            DisconnectCommand = ReactiveUI.ReactiveCommand.Create();
            DisconnectCommand.Subscribe(_ => DisconnectSerial());
        }

        /// <summary>
        /// Dispose of the ViewModel.
        /// </summary>
        public override void Dispose()
        {
            if (_serialPort != null)
            {
                DisconnectSerial();
            }

            //_adcpCodec.ProcessDataEvent -= _adcpCodec_ProcessDataEvent;
            //_adcpCodec.Dispose();

            if (_options.IsRecording)
            {
                StopRecord();
            }
        }

        #region Database

        /// <summary>
        /// Get the options for this subsystem display
        /// from the database.  If the options have not
        /// been set to the database yet, default values 
        /// will be used.
        /// </summary>
        private void GetOptionsFromDatabase()
        {
            _options = _pm.GetWpMagDirOutputViewOptions();

            // Notify all the properties
            NotifyOptionPropertyChange();
        }

        /// <summary>
        /// Notify all the properties of a change
        /// when a new option object is set.
        /// </summary>
        private void NotifyOptionPropertyChange()
        {
            //Notify property of changes
            this.NotifyOfPropertyChange(null);
        }

        /// <summary>
        /// Update the database with the latest options.
        /// </summary>
        private void UpdateDatabaseOptions()
        {
            _pm.UpdateWpMagDirOutputViewOptions(_options);
        }

        #endregion

        #region Status

        /// <summary>
        /// Display the status.
        /// </summary>
        /// <param name="status"></param>
        private void DisplayStatus(string status)
        {
            StatusOutput += status + "\n";
            if (_StatusOutput.Length > 400)
            {
                StatusOutput = _StatusOutput.Substring(_StatusOutput.Length - 400);
            }
        }

        #endregion

        #region Serial Connection

        /// <summary>
        /// Connect the ADCP Serial port.
        /// </summary>
        public void ConnectAdcpSerial()
        {
            ConnectSerial(_serialOptions);
        }

        /// <summary>
        /// Create a connection to the ADCP serial port with
        /// the given options.  If no options are given, return null.
        /// </summary>
        /// <param name="options">Options to connect to the serial port.</param>
        /// <returns>Adcp Serial Port based off the options</returns>
        public AdcpSerialPort ConnectSerial(SerialOptions options)
        {
            // If there is a connection, disconnect
            if (_serialPort != null)
            {
                DisconnectSerial();
            }

            if (options != null)
            {
                // Set the connection
                //Status.Status = eAdcpStatus.Connected;

                // Create the connection and connect
                _serialPort = new AdcpSerialPort(options);
                _serialPort.Connect();


                // Publish that the ADCP serial port is new
                //PublishAdcpSerialConnection();

                DisplayStatus(string.Format("Connect Serial: {0}", _serialPort.ToString()));

                // Set flag
                //IsAdcpFound = true;

                return _serialPort;
            }

            return null;
        }

        /// <summary>
        /// Shutdown the ADCP serial port.
        /// This will stop all the read threads
        /// for the ADCP serial port.
        /// </summary>
        public void DisconnectSerial()
        {
            try
            {
                if (_serialPort != null)
                {
                    DisplayStatus(string.Format("Disconnect Serial: {0}", _serialPort.ToString()));

                    // Disconnect the serial port
                    _serialPort.Disconnect();


                    // Publish that the ADCP serial conneciton is disconnected
                    //PublishAdcpSerialDisconnection();

                    // Shutdown the serial port
                    _serialPort.Dispose();
                }
                //Status.Status = eAdcpStatus.NotConnected;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error disconnecting the serial port.", e);
            }
        }

        /// <summary>
        /// Disconnect then connect with the new options given.
        /// </summary>
        /// <param name="options">Options to connect the ADCP serial port.</param>
        public void ReconnectSerial(SerialOptions options)
        {
            // Disconnect
            DisconnectSerial();

            // Wait for Disconnect to finish
            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);

            // Connect
            ConnectSerial(options);
        }

        /// <summary>
        /// Return if the Adcp Serial port is open and connected.
        /// </summary>
        /// <returns>TRUE = Is connected.</returns>
        public bool IsSerialConnected()
        {
            // See if the connection is open
            if (_serialPort != null && _serialPort.IsOpen())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Send data to the serial port.
        /// </summary>
        /// <param name="data">Data to send.</param>
        public void SendDataToSerial(string data)
        {
            // Verify connection is open then send data
            if (IsSerialConnected())
            {
                // Add carrage return, line feed to the end
                _serialPort.SendData(data + "\r\n", false);
            }
        }

        /// <summary>
        /// Scan for available serial ports.
        /// </summary>
        private void ScanForSerialPorts()
        {
            CommPortList = SerialOptions.PortOptions;
        }


        ///// <summary>
        ///// Send the command to the serial port.
        ///// </summary>
        //private void SendAdcpCommand()
        //{
        //    _serialPort.SendDataWaitReply(_SerialCmd);

        //    DisplayStatus("Send Command: " + _SerialCmd);
        //}

        #endregion

        #region File Record

        /// <summary>
        /// Set the directory for the raw recording results and
        /// turn on the flag.
        /// </summary>
        public void StartRecord()
        {
            // Stop the recording if on
            if (_binaryWriter != null)
            {
                StopRecord();
            }

            // Create a file name
            DateTime currDateTime = DateTime.Now;
            string filename = string.Format("RTI_{0:yyyyMMddHHmmss}.bin", currDateTime);
            string filePath = string.Format("{0}\\{1}", DEFAULT_RECORD_DIR, filename);

            try
            {
                // Writer
                _binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Append, FileAccess.Write));

                //IsRecording = true;

                DisplayStatus("Recording start for file: " + filePath);
            }
            catch (Exception e)
            {
                DisplayStatus("Error creating a record.  " + e.ToString());
            }
        }

        /// <summary>
        /// Stop writing data to the file.
        /// Close the file
        /// </summary>
        public void StopRecord()
        {
            // Set flag
            //_IsRecording = false;

            try
            {
                if (_binaryWriter != null)
                {
                    // Flush and close the writer
                    _binaryWriter.Flush();
                    _binaryWriter.Close();
                    _binaryWriter.Dispose();
                    _binaryWriter = null;

                    DisplayStatus("Stop recording file.");
                }
            }
            catch (Exception e)
            {
                // Log error
                DisplayStatus("Error closing Record. " + e.ToString());
            }
        }

        /// <summary>
        /// Verify the writer is created.  If it is not turned on,
        /// craete the writer.  Then write the data.
        /// Write the raw data to the file.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        private void WriteData(byte[] data)
        {
            // Verify recording is turned on
            if (IsRecording)
            {
                // Create the writer if it does not exist
                if (_binaryWriter == null)
                {
                    // Create writer
                    StartRecord();
                }

                // Verify writer is created
                if (_binaryWriter != null)
                {
                    try
                    {
                        // Write the data to the file
                        _binaryWriter.Write(data);

                        // Accumulate the number of bytes written
                        BytesWritten += data.Length;
                    }
                    catch (Exception e)
                    {
                        // Error writing data
                        DisplayStatus("Error writing data.." + e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Verify the writer is created.  If it is not turned on,
        /// craete the writer.  Then write the data.
        /// Write the raw data to the file.
        /// </summary>
        /// <param name="data">Data to write to the file.</param>
        private void WriteData(string data)
        {
            // Verify recording is turned on
            if (IsRecording)
            {
                // Create the writer if it does not exist
                if (_binaryWriter == null)
                {
                    // Create writer
                    StartRecord();
                }

                // Verify writer is created
                if (_binaryWriter != null)
                {
                    try
                    {
                        // Write the data to the file
                        _binaryWriter.Write(data);

                        // Accumulate the number of bytes written
                        BytesWritten += data.Length;
                    }
                    catch (Exception e)
                    {
                        // Error writing data
                        DisplayStatus("Error writing data.." + e.ToString());
                    }
                }
            }
        }

        #endregion

        #region Bin Selection

        /// <summary>
        /// Verify the bin selections are good.
        /// Then return them as a list.
        /// </summary>
        /// <returns>List of selected bins.</returns>
        private List<int> VerifyBinSelection(DataSet.Ensemble ens)
        {
            List<int> result = new List<int>();

            int numBins = 0;
            // Get the number of bins
            if (ens.IsEnsembleAvail)
            {
                numBins = ens.EnsembleData.NumBins;
            }
            else
            {
                return new List<int>();
            }

            // Create the list of from the user input
            string[] strList = SelectedBins.Split(',');

            // Parse the string list to ints
            for (int x = 0; x < strList.Length; x++ )
            {
                int val = -1;
                if( int.TryParse(strList[x], out val))
                {
                    // Check if negative
                    // Check if number greater than number of bins
                    if(val >= 0 && val < numBins )
                    {
                        result.Add(val);
                    }
                }
            }
            
            return result;
        }

        #endregion

        #region Encode

        /// <summary>
        /// Encode the velocity data.
        /// 
        /// Encoding
        /// $CUR – name of the message, all in one line followed by [CR][LF] (keep it as those 3 letters “CUR” so we can filter to look for those type of messages)
        /// CC – channel number, so the depth to which the depth applies
        /// AA.AA – ENU MAG (as I can see on RTI screens, that’s the value Navigators are looking at), the magnitude of the current
        /// BBB.BB – DIR – the direction of the current
        /// 
        /// 
        /// Ex:
        /// $CUR,04,0.44,272.33,08,0.64,263.33,12,1.64,163.33
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <returns>String message.</returns>
        private string EncodeVelocity(DataSet.Ensemble ens)
        {
            string result = "";

            // Get the Selected bins
            // If none are found, return nothing
            List<int> selectedBins = VerifyBinSelection(ens);
            if(selectedBins.Count <= 0)
            {
                return "";
            }

            // SHIP
            if(_options.SelectedTransform == WpMagDirOutputViewOptions.TRANSFORM_SHIP)
            {
                if(ens.IsShipVelocityAvail)
                {
                    return EncodeVelcoity(ens.ShipVelocityData.ShipVelocityData, selectedBins);
                }
            }

            // EARTH
            if(_options.SelectedTransform == WpMagDirOutputViewOptions.TRANSFORM_EARTH)
            {
                if(ens.IsEarthVelocityAvail)
                {
                    return EncodeVelcoity(ens.EarthVelocityData.EarthVelocityData, selectedBins);
                }
            }

            // Instrument
            if (_options.SelectedTransform == WpMagDirOutputViewOptions.TRANSFORM_INSTRUMENT)
            {
                if (ens.IsInstrumentVelocityAvail)
                {
                    return EncodeVelcoity(ens.InstrumentVelocityData.InstrumentVelocityData, selectedBins);
                }
            }

            return result;
        }


        /// <summary>
        /// Using the velocity data, convert the selected bin data to magnitude and direction,
        /// then add it to the result.
        /// </summary>
        /// <param name="vel">Float array containing velocities for each bin and beam.</param>
        /// <param name="selectedBins">Selected bins.</param>
        /// <returns>String of all the selected bin's mag and direction.</returns>
        private string EncodeVelcoity(float[,] vel, List<int> selectedBins)
        {
            StringBuilder result = new StringBuilder();

            // Add the ID
            result.Append("$CUR");

            foreach(var bin in selectedBins)
            {
                double mag = 0.0;
                double dir = 0.0;

                // Verify the data is good
                if (vel[bin, 0] != DataSet.Ensemble.BAD_VELOCITY &&
                   vel[bin, 1] != DataSet.Ensemble.BAD_VELOCITY &&
                   vel[bin, 2] != DataSet.Ensemble.BAD_VELOCITY)
                {
                    // Calculate magnitude
                    mag = MathHelper.CalculateMagnitude(vel[bin, 0],
                                                        vel[bin, 1],
                                                        0);
                    
                    // Calculate direciton
                    // East, North
                    // X, Y
                    // Traverse, Longitudinal
                    dir = MathHelper.CalculateDirection(vel[bin, 0], vel[bin, 1]);

                    result.Append(",");
                    // Add the channel to the message
                    result.Append(string.Format("{0, 3}", bin.ToString("0")));
                    result.Append(",");

                    // Add Magnitude
                    result.Append(string.Format("{0, 5}", mag.ToString("0.00")));
                    result.Append(",");
 
                    // Add Direction
                    result.Append(string.Format("{0, 6}", dir.ToString("0.00")));
                }
            }

            // Add <CR><LF>
            result.Append("\r\n");

            return result.ToString();
        }

        #endregion

        #region Remove Ship Speed

        /// <summary>
        /// Remove the ship speed.
        /// This will remove the ship speed in the Earth, Instrument and Ship velocities.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        private void RemoveShipSpeed(ref DataSet.Ensemble ens)
        {

            // Remove the Ship speed from the data
            // Remove Ship Speed
            if (_options.IsRemoveShipSpeed)
            {
                ScreenData.RemoveShipSpeed.RemoveVelocity(ref ens, _prevShipSpeedEast, _prevShipSpeedNorth, _prevShipSpeedVert, _options.CanUseBottomTrackVel, _options.CanUseGpsVel, _options.HeadingOffset);
                ScreenData.RemoveShipSpeed.RemoveVelocityInstrument(ref ens, _prevShipSpeedX, _prevShipSpeedY, _prevShipSpeedZ, _options.CanUseBottomTrackVel, _options.CanUseGpsVel, _options.HeadingOffset);
                ScreenData.RemoveShipSpeed.RemoveVelocityShip(ref ens, _prevShipSpeedTransverse, _prevShipSpeedLongitudinal, _prevShipSpeedNormal, _options.CanUseBottomTrackVel, _options.CanUseGpsVel, _options.HeadingOffset);
            }

            // EARTH
            // Record the Bottom for previous values
            float[] prevShipSpeed = ScreenData.RemoveShipSpeed.GetPreviousShipSpeed(ens, HeadingOffset, _options.CanUseBottomTrackVel, _options.CanUseGpsVel);
            _prevShipSpeedEast = prevShipSpeed[0];
            _prevShipSpeedNorth = prevShipSpeed[1];
            _prevShipSpeedVert = prevShipSpeed[2];

            // Instrument
            // Record the Bottom for previous values
            float[] prevShipSpeedInstrument = ScreenData.RemoveShipSpeed.GetPreviousShipSpeedInstrument(ens, HeadingOffset, _options.CanUseBottomTrackVel, _options.CanUseGpsVel);
            _prevShipSpeedX = prevShipSpeedInstrument[0];
            _prevShipSpeedY = prevShipSpeedInstrument[1];
            _prevShipSpeedZ = prevShipSpeedInstrument[2];

            // Ship
            // Record the Bottom for previous values
            float[] prevShipSpeedShip = ScreenData.RemoveShipSpeed.GetPreviousShipSpeedShip(ens, HeadingOffset, _options.CanUseBottomTrackVel, _options.CanUseGpsVel);
            _prevShipSpeedTransverse = prevShipSpeedShip[0];
            _prevShipSpeedLongitudinal = prevShipSpeedShip[1];
            _prevShipSpeedNormal = prevShipSpeedShip[2];
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Receive an ensemble.  
        /// </summary>
        /// <param name="message">Ensemble message</param>
        public void Handle(EnsembleRawEvent message)
        {
            // If turned off, do not process the data
            if (!_options.IsOutputEnabled)
            {
                return;
            }

            // Check if the ensemble is good
            if (message.Ensemble == null)
            {
                return;
            }

            if(message.Source == EnsembleSource.Serial ||
                message.Source == EnsembleSource.Playback ||
                message.Source == EnsembleSource.Ethernet)
            {
                // Make a copy of the ensemble, because the data will be modified
                ReceiveEnsemble(message.Ensemble.Clone(), message.OrigDataFormat);
            }
        }

        /// <summary>
        /// Receive the ensemble and decode it to output the data.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <param name="origDataFormat">Original Data format.</param>
        public void ReceiveEnsemble(DataSet.Ensemble ens, AdcpCodec.CodecEnum origDataFormat)
        {
            // Set the buffer size to display data
            // To much data will make the system run slower
            int dataOutputMax = 5000;

            // If the HeadingOffset is set or
            // If they want to retransform the data or
            // They are replacing the heading with GPS heading,
            // The data needs to be retransformed to use the new heading.
            // Retransform the data with the new heading
            // Apply HDT heading if requried and available
            // This will also apply the heading offset
            if (_options.IsRetransformData || _options.IsUseGpsHeading || _options.HeadingOffset != 0)
            {
                // Retransform the Profile datas
                Transform.ProfileTransform(ref ens, origDataFormat, 0.25f, _options.SelectedHeadingSource, _options.HeadingOffset);

                // Retransform the Bottom Track data
                // This will also create the ship data
                Transform.BottomTrackTransform(ref ens, origDataFormat, 0.90f, 10.0f, _options.SelectedHeadingSource, _options.HeadingOffset);

                // WaterMass transform data
                // This will also create the ship data
                if (ens.IsInstrumentWaterMassAvail)
                {
                    Transform.WaterMassTransform(ref ens, origDataFormat, 0.90f, 10.0f, _options.SelectedHeadingSource, _options.HeadingOffset, _options.ShipXdcrOffset);
                }
            }

            // Remove the ship speed
            RemoveShipSpeed(ref ens);

            // Encode the data
            string output = EncodeVelocity(ens);

            // Display data
            DataOutput += output;
            
            // Send data to serial port
            SendDataToSerial(output);

            // Write data to file if turned on
            WriteData(output);

            // Keep the Buffer to a set limit
            if (DataOutput.Length > dataOutputMax)
            {
                DataOutput = DataOutput.Substring(DataOutput.Length - dataOutputMax);
            }
        }

        #endregion

    }
}
