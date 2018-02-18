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
 * 10/02/2014      RC          4.1.0      Initial coding
 * 07/17/2015      RC          4.1.5      Commented out the calibration results for performance for Paul.
 *
 */

using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace RTI
{
    /// <summary>
    /// RTI custom compass calibration.
    /// 
    /// http://www.camelsoftware.com/firetail/blog/uavs/3-axis-magnetometer-calibration-a-simple-technique-for-hard-soft-errors/
    /// </summary>
    class RtiCompassCalViewModel : PulseViewModel
    {
        #region Variables

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
        private PniPrimeCompassBinaryCodec _compassCodec;

        /// <summary>
        /// Binary writer for raw Compass data.
        /// </summary>
        private StreamWriter _rawCompassRecordWriter;

        /// <summary>
        /// Lock for the raw ADCP file.
        /// </summary>
        private object _rawCompassRecordFileLock = new object();

        /// <summary>
        /// Buffer the incoming data.
        /// </summary>
        private ConcurrentQueue<RTI.PniPrimeCompassBinaryCodec.PniDataResponse> _buffer;

        /// <summary>
        /// Flag for processing buffer.
        /// </summary>
        private bool _isProcessingBuffer;

        #endregion

        #region Properties

        #region Connection

        /// <summary>
        /// Flag if the ADCP is connected in compass mode.
        /// 
        /// This is mainly used to set the buttons on and off.
        /// The mode should be checked in the AdcpSerialPort if it
        /// is set correctly.
        /// AdcpSerialPort.IsCompassMode
        /// </summary>
        public bool IsCompassConnected
        {
            get
            {
                if (_adcpConn == null)
                {
                    return false;
                }

                if (_adcpConn.AdcpSerialPort == null)
                {
                    return false;
                }

                return _adcpConn.AdcpSerialPort.IsCompassMode;
            }
        }

        #endregion

        #region Record Data

        /// <summary>
        /// Turn on or off the compass recording.
        /// </summary>
        private bool _IsCompassRecording;
        /// <summary>
        /// Turn on or off the compass recording.
        /// </summary>
        public bool IsCompassRecording
        {
            get { return _IsCompassRecording; }
            set
            {
                _IsCompassRecording = value;
                this.NotifyOfPropertyChange(() => this.IsCompassRecording);

                // Turn on or off recording
                RecordData();
            }
        }

        /// <summary>
        /// Bytes written to the recording.
        /// </summary>
        private long _RawCompassBytesWritten;
        /// <summary>
        /// Bytes written to the recording.
        /// </summary>
        public long RawCompassBytesWritten
        {
            get { return _RawCompassBytesWritten; }
            set
            {
                _RawCompassBytesWritten = value;
                this.NotifyOfPropertyChange(() => this.RawCompassBytesWritten);
                this.NotifyOfPropertyChange(() => this.RawCompassByteWrittenStr);
            }
        }

        /// <summary>
        /// Compass bytes written to a string.
        /// </summary>
        public string RawCompassByteWrittenStr
        {
            get
            {
                return MathHelper.MemorySizeString(RawCompassBytesWritten);
            }
        }

        /// <summary>
        /// File name for the compass record.
        /// </summary>
        private string _RawCompassRecordFileName;
        /// <summary>
        /// File name for the compass record.
        /// </summary>
        public string RawCompassRecordFileName
        {
            get { return _RawCompassRecordFileName; }
            set
            {
                _RawCompassRecordFileName = value;
                this.NotifyOfPropertyChange(() => this.RawCompassRecordFileName);
            }
        }

        #endregion

        #region Sample Data

        /// <summary>
        /// Sample data from the ADCP compass.  This will be the
        /// heading, pitch and roll data.
        /// </summary>
        private string _SampleData;
        /// <summary>
        /// Sample data from the ADCP compass.  This will be the
        /// heading, pitch and roll data.
        /// </summary>
        public string SampleData
        {
            get { return _SampleData; }
            set
            {
                _SampleData = value;
                this.NotifyOfPropertyChange(() => this.SampleData);
            }
        }

        #endregion

        #region Plots

        /// <summary>
        /// Raw Points plot.
        /// </summary>
        public CompassCalScatterPlot3D RawPlot { get; set; }

        ///// <summary>
        ///// Hard Iron Calibration Points plot.
        ///// </summary>
        //public CompassCalScatterPlot3D HardIronCalPlot { get; set; }

        ///// <summary>
        ///// Soft Iron Calibration Points plot.
        ///// </summary>
        //public CompassCalScatterPlot3D SoftIronCalPlot { get; set; }

        /// <summary>
        /// Light for the plot.
        /// </summary>
        public Model3DGroup Lights
        {
            get
            {
                var group = new Model3DGroup();
                group.Children.Add(new AmbientLight(System.Windows.Media.Colors.White));
                return group;
            }
        }

        #endregion

        //#region Hard Iron Points

        ///// <summary>
        ///// Minimum X alignment value.
        ///// </summary>
        //private double _MinX;
        ///// <summary>
        ///// Minimum X alignment value.
        ///// </summary>
        //public double MinX
        //{
        //    get { return _MinX; }
        //    set
        //    {
        //        _MinX = value;
        //        this.NotifyOfPropertyChange(() => this.MinX);
        //    }
        //}

        ///// <summary>
        ///// Maximum X alignment value.
        ///// </summary>
        //private double _MaxX;
        ///// <summary>
        ///// Maximum X alignment value.
        ///// </summary>
        //public double MaxX
        //{
        //    get { return _MaxX; }
        //    set
        //    {
        //        _MaxX = value;
        //        this.NotifyOfPropertyChange(() => this.MaxX);
        //    }
        //}

        ///// <summary>
        ///// Average of the Minimum and Maximum X alignment value.
        ///// </summary>
        //private double _AvgMinMaxX;
        ///// <summary>
        ///// Average of the Minimum and Maximum X alignment value.
        ///// </summary>
        //public double AvgMinMaxX
        //{
        //    get { return _AvgMinMaxX; }
        //    set
        //    {
        //        _AvgMinMaxX = value;
        //        this.NotifyOfPropertyChange(() => this.AvgMinMaxX);
        //    }
        //}

        ///// <summary>
        ///// Minimum Y alignment value.
        ///// </summary>
        //private double _MinY;
        ///// <summary>
        ///// Minimum Y alignment value.
        ///// </summary>
        //public double MinY
        //{
        //    get { return _MinY; }
        //    set
        //    {
        //        _MinY = value;
        //        this.NotifyOfPropertyChange(() => this.MinY);
        //    }
        //}

        ///// <summary>
        ///// Maximum Y alignment value.
        ///// </summary>
        //private double _MaxY;
        ///// <summary>
        ///// Maximum Y alignment value.
        ///// </summary>
        //public double MaxY
        //{
        //    get { return _MaxY; }
        //    set
        //    {
        //        _MaxY = value;
        //        this.NotifyOfPropertyChange(() => this.MaxY);
        //    }
        //}

        ///// <summary>
        ///// Average of the Minimum and Maximum Y alignment value.
        ///// </summary>
        //private double _AvgMinMaxY;
        ///// <summary>
        ///// Average of the Minimum and Maximum Y alignment value.
        ///// </summary>
        //public double AvgMinMaxY
        //{
        //    get { return _AvgMinMaxY; }
        //    set
        //    {
        //        _AvgMinMaxY = value;
        //        this.NotifyOfPropertyChange(() => this.AvgMinMaxY);
        //    }
        //}

        ///// <summary>
        ///// Minimum Z alignment value.
        ///// </summary>
        //private double _MinZ;
        ///// <summary>
        ///// Minimum Z alignment value.
        ///// </summary>
        //public double MinZ
        //{
        //    get { return _MinZ; }
        //    set
        //    {
        //        _MinZ = value;
        //        this.NotifyOfPropertyChange(() => this.MinZ);
        //    }
        //}

        ///// <summary>
        ///// Maximum Z alignment value.
        ///// </summary>
        //private double _MaxZ;
        ///// <summary>
        ///// Maximum Z alignment value.
        ///// </summary>
        //public double MaxZ
        //{
        //    get { return _MaxZ; }
        //    set
        //    {
        //        _MaxZ = value;
        //        this.NotifyOfPropertyChange(() => this.MaxZ);
        //    }
        //}

        ///// <summary>
        ///// Average of the Minimum and Maximum Z alignment value.
        ///// </summary>
        //private double _AvgMinMaxZ;
        ///// <summary>
        ///// Average of the Minimum and Maximum Z alignment value.
        ///// </summary>
        //public double AvgMinMaxZ
        //{
        //    get { return _AvgMinMaxZ; }
        //    set
        //    {
        //        _AvgMinMaxZ = value;
        //        this.NotifyOfPropertyChange(() => this.AvgMinMaxZ);
        //    }
        //}

        //#endregion

        //#region Soft Iron Points

        //#region X

        ///// <summary>
        ///// Minimum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMinX;
        ///// <summary>
        ///// Minimum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMinX
        //{
        //    get { return _VMinX; }
        //    set
        //    {
        //        _VMinX = value;
        //        this.NotifyOfPropertyChange(() => this.VMinX);
        //    }
        //}

        ///// <summary>
        ///// Maximum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMaxX;
        ///// <summary>
        ///// Maximum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMaxX
        //{
        //    get { return _VMaxX; }
        //    set
        //    {
        //        _VMaxX = value;
        //        this.NotifyOfPropertyChange(() => this.VMaxX);
        //    }
        //}

        ///// <summary>
        ///// Average of the Mininum and Maximum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _AvgVMinMaxX;
        ///// <summary>
        ///// Average of the Mininum and Maximum X alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double AvgVMinMaxX
        //{
        //    get { return _AvgVMinMaxX; }
        //    set
        //    {
        //        _AvgVMinMaxX = value;
        //        this.NotifyOfPropertyChange(() => this.AvgVMinMaxX);
        //    }
        //}

        ///// <summary>
        ///// Soft Iron Scale Factor X.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the X value.
        ///// </summary>
        //private double _SoftIronScaleFactorX;
        ///// <summary>
        ///// Soft Iron Scale Factor X.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the X value.
        ///// </summary>
        //public double SoftIronScaleFactorX
        //{
        //    get { return _SoftIronScaleFactorX; }
        //    set
        //    {
        //        _SoftIronScaleFactorX = value;
        //        this.NotifyOfPropertyChange(() => this.SoftIronScaleFactorX);
        //    }
        //}

        //#endregion

        //#region Y

        ///// <summary>
        ///// Minimum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMinY;
        ///// <summary>
        ///// Minimum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMinY
        //{
        //    get { return _VMinY; }
        //    set
        //    {
        //        _VMinY = value;
        //        this.NotifyOfPropertyChange(() => this.VMinY);
        //    }
        //}

        ///// <summary>
        ///// Maximum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMaxY;
        ///// <summary>
        ///// Maximum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMaxY
        //{
        //    get { return _VMaxY; }
        //    set
        //    {
        //        _VMaxY = value;
        //        this.NotifyOfPropertyChange(() => this.VMaxY);
        //    }
        //}

        ///// <summary>
        ///// Average of the Mininum and Maximum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _AvgVMinMaxY;
        ///// <summary>
        ///// Average of the Mininum and Maximum Y alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double AvgVMinMaxY
        //{
        //    get { return _AvgVMinMaxY; }
        //    set
        //    {
        //        _AvgVMinMaxY = value;
        //        this.NotifyOfPropertyChange(() => this.AvgVMinMaxY);
        //    }
        //}

        ///// <summary>
        ///// Soft Iron Scale Factor Y.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the Y value.
        ///// </summary>
        //private double _SoftIronScaleFactorY;
        ///// <summary>
        ///// Soft Iron Scale Factor Y.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the Y value.
        ///// </summary>
        //public double SoftIronScaleFactorY
        //{
        //    get { return _SoftIronScaleFactorY; }
        //    set
        //    {
        //        _SoftIronScaleFactorY = value;
        //        this.NotifyOfPropertyChange(() => this.SoftIronScaleFactorY);
        //    }
        //}

        //#endregion

        //#region Z

        ///// <summary>
        ///// Minimum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMinZ;
        ///// <summary>
        ///// Minimum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMinZ
        //{
        //    get { return _VMinZ; }
        //    set
        //    {
        //        _VMinZ = value;
        //        this.NotifyOfPropertyChange(() => this.VMinZ);
        //    }
        //}

        ///// <summary>
        ///// Maximum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _VMaxZ;
        ///// <summary>
        ///// Maximum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double VMaxZ
        //{
        //    get { return _VMaxZ; }
        //    set
        //    {
        //        _VMaxZ = value;
        //        this.NotifyOfPropertyChange(() => this.VMaxZ);
        //    }
        //}

        ///// <summary>
        ///// Average of the Mininum and Maximum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //private double _AvgVMinMaxZ;
        ///// <summary>
        ///// Average of the Mininum and Maximum Z alignment value with the averaged min/max is removed.
        ///// </summary>
        //public double AvgVMinMaxZ
        //{
        //    get { return _AvgVMinMaxZ; }
        //    set
        //    {
        //        _AvgVMinMaxZ = value;
        //        this.NotifyOfPropertyChange(() => this.AvgVMinMaxZ);
        //    }
        //}

        ///// <summary>
        ///// Soft Iron Scale Factor Z.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the Z value.
        ///// </summary>
        //private double _SoftIronScaleFactorZ;
        ///// <summary>
        ///// Soft Iron Scale Factor Z.
        ///// Subtract out the Hard Iron Scale Factor and multiply it to the Z value.
        ///// </summary>
        //public double SoftIronScaleFactorZ
        //{
        //    get { return _SoftIronScaleFactorZ; }
        //    set
        //    {
        //        _SoftIronScaleFactorZ = value;
        //        this.NotifyOfPropertyChange(() => this.SoftIronScaleFactorZ);
        //    }
        //}

        //#endregion

        ///// <summary>
        ///// Average radius by averaging the values for that axis.
        ///// </summary>
        //private double _AvgRad;
        ///// <summary>
        ///// Average radius by averaging the values for that axis.
        ///// </summary>
        //public double AvgRad
        //{
        //    get { return _AvgRad; }
        //    set
        //    {
        //        _AvgRad = value;
        //        this.NotifyOfPropertyChange(() => this.AvgRad);
        //    }
        //}

        //#endregion

        #region Points

        /// <summary>
        /// List of all the points.
        /// </summary>
        public List<Point3D> Points { get; set;}

        /// <summary>
        /// Lock for Points list.
        /// </summary>
        public object PointsLock = new object();

        ///// <summary>
        ///// List of all the points with the hard iron calibration.
        ///// </summary>
        //public List<Point3D> HardIronCalPoints { get; set; }

        ///// <summary>
        ///// List of all the points with the soft iron calibration.
        ///// </summary>
        //public List<Point3D> SoftIronCalPoints { get; set; }

        #endregion

        #endregion

        #region Command

        /// <summary>
        /// Connect the ADCP as Compass mode.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> CompassConnectCommand { get; protected set; }

        /// <summary>
        /// Connect the ADCP as Compass mode.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> CompassDisconnectCommand { get; protected set; }

        /// <summary>
        /// Command to start or stop the Sample data.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SampleDataCommand { get; protected set; }

        /// <summary>
        /// Command to get data from the compass.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> GetDataCommand { get; protected set; }

        /// <summary>
        /// Command to set data output to all values for the compass.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SetAllDataOutputCommand { get; protected set; }

        /// <summary>
        /// Command to set data output to Heading, Pitch and Roll values for the compass.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SetHprDataOutputCommand { get; protected set; }

        /// <summary>
        /// Command to set factory calibration.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> FactoryCalCommand { get; protected set; }

        /// <summary>
        /// Command to clear data.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ClearCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize value.
        /// </summary>
        public RtiCompassCalViewModel()
            : base("RTI CompassCal")
        {
            // Initialize values
            _eventAggregator = IoC.Get<IEventAggregator>();
            _adcpConn = IoC.Get<AdcpConnection>();
            _compassCodec = new RTI.PniPrimeCompassBinaryCodec();
            IsCompassRecording = false;
            RawCompassBytesWritten = 0;
            RawCompassRecordFileName = "";
            _isProcessingBuffer = false;
            _buffer = new ConcurrentQueue<RTI.PniPrimeCompassBinaryCodec.PniDataResponse>();

            Points = new List<Point3D>();
            //HardIronCalPoints = new List<Point3D>();
            //SoftIronCalPoints = new List<Point3D>();

            // Init plots
            RawPlot = new CompassCalScatterPlot3D();
            //HardIronCalPlot = new CompassCalScatterPlot3D();
            //SoftIronCalPlot = new CompassCalScatterPlot3D();

            // Setup the compass event subscriptions
            SubscribeCompassEvents();

            // Commands
            // Command to connect to compass mode
            CompassConnectCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => !x.Value),
                                                                                _ => Task.Run(() => CompassConnect()));

            // Command to disconnect from compass mode
            CompassDisconnectCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => CompassDisconnect()));

            SampleDataCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                                    param => Task.Run(() => SetSampleData(param)));

            GetDataCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                                                _ => Task.Run(() => GetData()));

            SetAllDataOutputCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                                    _ => SetAllData());

            SetHprDataOutputCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                                                _ => SetHPRData());

            FactoryCalCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                                    _ => Task.Run(() => OnSetDefaultCompassCalMag()));

            ClearCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsCompassConnected, x => x.Value),
                                                        _ => Task.Run(() => ClearData()));

            // Set compass connected for button activation
            this.NotifyOfPropertyChange(() => this.IsCompassConnected);
        }

        /// <summary>
        /// Dispose view model.
        /// </summary>
        public override void Dispose()
        {
            // Unsubscribe
            UnsubscribeCompassEvents();

            // Stop Recording
            StopRecording();

            // Dispose codec
            _compassCodec.Dispose();
        }

        #region Event Subscriptions

        /// <summary>
        /// Create new serial ports with the options
        /// from the serial options.
        /// This will also subscribe to receive events
        /// from the serial ports and clear the buffers.
        /// </summary>
        private void SubscribeCompassEvents()
        {
            // If the serial port was previous connected, 
            // Unsubscribe events.
            if (_adcpConn.AdcpSerialPort != null)
            {
                UnsubscribeCompassEvents();

                // Subscribe to receive event when compass data received
                _adcpConn.AdcpSerialPort.ReceiveCompassSerialDataEvent += new RTI.AdcpSerialPort.ReceiveCompassSerialDataEventHandler(On_ReceiveCompassSerialDataEvent);

                // Wait for incoming cal samples
                _compassCodec.CompassEvent += new RTI.PniPrimeCompassBinaryCodec.CompassEventHandler(CompassCodecEventHandler);
            }
        }

        /// <summary>
        /// Unsubscribe from the ADCP serial port events.
        /// </summary>
        private void UnsubscribeCompassEvents()
        {
            _adcpConn.AdcpSerialPort.ReceiveCompassSerialDataEvent -= On_ReceiveCompassSerialDataEvent;

            _compassCodec.CompassEvent -= CompassCodecEventHandler;
        }

        #endregion

        #region Connect Disconnect Compass

        /// <summary>
        /// Make the ADCP go into compasss mode.
        /// The ADCP will then only work for compass commands.
        /// The ADCP must be disconnected from the compass when complete.
        /// </summary>
        /// <returns>TRUE = Connected.</returns>
        private void CompassConnect()
        {
            // Send the commands
            // Put ADCP in Compass mode
            // Set the serial port to COMPASS mode to decode compass data
            if (!_adcpConn.AdcpSerialPort.StartCompassMode())
            {
                //SetStatusBar(new StatusEvent("Compass Issue.  Could not connect to compass.", MessageBoxImage.Error));
            }

            Thread.Sleep(RTI.AdcpSerialPort.WAIT_STATE);                  // Delay for 485 response

            // Clear the buffer of any data from last calibration
            _compassCodec.ClearIncomingData();

            // Set compass connected for button activation
            this.NotifyOfPropertyChange(() => this.IsCompassConnected);
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

            // Set status that ADCP is connected
            //SetAdcpStatus(new AdcpStatus(eAdcpStatus.Connected));

            // Set compass connected for button activation
            this.NotifyOfPropertyChange(() => this.IsCompassConnected);
        }

        #endregion

        #region Compass Cal

        /// <summary>
        /// Compass cal data.
        /// 
        /// Hard and Soft Iron Calibration found here
        /// http://www.camelsoftware.com/firetail/blog/uavs/3-axis-magnetometer-calibration-a-simple-technique-for-hard-soft-errors/
        /// </summary>
        /// <param name="data">Compass data.</param>
        public void CompassCalData(RTI.PniPrimeCompassBinaryCodec.PniDataResponse data)
        {
            // Create a point based off latest data
            Point3D point3d = new Point3D(data.xAligned, data.yAligned, data.zAligned);

            // Add it the list of points
            Point3D[] pointArray = null;
            lock (PointsLock)
            {
                // List of all the points
                Points.Add(point3d);

                // Create an array of the points
                pointArray = Points.ToArray();
            }

            if (pointArray != null)
            {
                //***************************
                //***************************
                //
                // Commmented out to improve performance
                //
                //***************************
                //***************************
                //// Find the min and max of the values
                //MinX = pointArray.Min(p => p.X);
                //MaxX = pointArray.Max(p => p.X);
                //MinY = pointArray.Min(p => p.Y);
                //MaxY = pointArray.Max(p => p.Y);
                //MinZ = pointArray.Min(p => p.Z);
                //MaxZ = pointArray.Max(p => p.Z);

                //// Average Min and Max value
                //AvgMinMaxX = (_MinX + _MaxX) / 2.0;
                //AvgMinMaxY = (_MinY + _MaxY) / 2.0;
                //AvgMinMaxZ = (_MinZ + _MaxZ) / 2.0;

                //// Find Min Max with the averaged min max removed
                //// The average distance from ths center is calculated.
                //VMinX = _MinX - _AvgMinMaxX;
                //VMaxX = _MaxX - _AvgMinMaxX;
                //VMinY = _MinY - _AvgMinMaxY;
                //VMaxY = _MaxY - _AvgMinMaxY;
                //VMinZ = _MinZ - _AvgMinMaxZ;
                //VMaxZ = _MaxZ - _AvgMinMaxZ;

                //// Average of the VMin VMax values
                //// We want to know how far from the center, so the negative values are inverted.
                //AvgVMinMaxX = (VMaxX + (VMinX * -1.0)) / 2.0;           // Multiply by -1 to make the negative values positive
                //AvgVMinMaxY = (VMaxY + (VMinY * -1.0)) / 2.0;           // Multiply by -1 to make the negative values positive
                //AvgVMinMaxZ = (VMaxZ + (VMinZ * -1.0)) / 2.0;           // Multiply by -1 to make the negative values positive

                //// The components are now averaged out to get the average radius
                //AvgRad = (AvgVMinMaxX + AvgVMinMaxY + AvgVMinMaxZ) / 3.0;

                //// Finally calculate the scale factor by dividing average radius by average value for that axis
                //SoftIronScaleFactorX = AvgRad / AvgVMinMaxX;
                //SoftIronScaleFactorY = AvgRad / AvgVMinMaxY;
                //SoftIronScaleFactorZ = AvgRad / AvgVMinMaxZ;

                //// Remove the min max average from the point
                //// To calibrate out the hard iron 
                //Point3D hardIronPoint = new Point3D(data.xAligned - AvgMinMaxX,                                 // X point with averaged min/max removed
                //                                    data.yAligned - AvgMinMaxY,                                 // Y point with averaged min/max removed
                //                                    data.zAligned - AvgMinMaxZ);                                // Z point with averaged min/max removed

                //Point3D softIronPoint = new Point3D((data.xAligned - AvgMinMaxX) * SoftIronScaleFactorX,        // X point with averaged min/max removed and Soft Iron Scale factor multiplied in
                //                                    (data.yAligned - AvgMinMaxY) * SoftIronScaleFactorY,        // Y point with averaged min/max removed and Soft Iron Scale factor multiplied in
                //                                    (data.zAligned - AvgMinMaxZ) * SoftIronScaleFactorZ);       // Z point with averaged min/max removed and Soft Iron Scale factor multiplied in

                //// Set the Average radius
                //RawPlot.AverageRadius = _AvgRad;
                //HardIronCalPlot.AverageRadius = _AvgRad;
                //SoftIronCalPlot.AverageRadius = _AvgRad;

                //Add points to plot
                //HardIronCalPlot.AddIncomingData(hardIronPoint);
                //SoftIronCalPlot.AddIncomingData(softIronPoint);
                
                // Draw the plot since we already have all the points
                RawPlot.DrawPlot(pointArray);
            }

        }

        #endregion

        #region Clear Data

        /// <summary>
        /// Clear all the data.
        /// </summary>
        private void ClearData()
        {
            //HardIronCalPlot.ClearIncomingData();
            //SoftIronCalPlot.ClearIncomingData();
            RawPlot.ClearIncomingData();

            Points.Clear();

            //MinX = 0.0;
            //MaxX = 0.0;
            //MinY = 0.0;
            //MaxY = 0.0;
            //MinZ = 0.0;
            //MaxZ = 0.0;
            //AvgMinMaxX = 0.0;
            //AvgMinMaxY = 0.0;
            //AvgMinMaxZ = 0.0;
            //VMinX = 0.0;
            //VMaxX = 0.0;
            //VMinY = 0.0;
            //VMaxY = 0.0;
            //VMinZ = 0.0;
            //VMaxZ = 0.0;
            //AvgVMinMaxX = 0.0;
            //AvgVMinMaxY = 0.0;
            //AvgVMinMaxZ = 0.0;
            //AvgRad = 0.0;
            //SoftIronScaleFactorX = 0.0;
            //SoftIronScaleFactorY = 0.0;
            //SoftIronScaleFactorZ = 0.0;
        }

        #endregion


        #region Sample Data

        /// <summary>
        /// Send the command to stop or stop the interval mode
        /// based off the param given.
        /// The params will be a string of either "start" or
        /// "stop".
        /// </summary>
        /// <param name="param">Param to start or stop the interval mode.</param>
        private void SetSampleData(object param)
        {
            var paramStr = param as string;
            if (paramStr != null)
            {
                switch (paramStr)
                {
                    case "start":
                        _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.StartIntervalModeCommand());
                        break;
                    case "stop":
                        _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.StopIntervalModeCommand());
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Get Data

        /// <summary>
        ///  Send a command to get Data from the ADCP compass.
        /// </summary>
        private void GetData()
        {
            // Clear buffer
            SampleData = "";

            // Send command to get data
            _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.GetDataCommand());
        }

        /// <summary>
        /// Send a command to get all Data from the ADCP compass.
        /// This will get all the possible values from the compass.
        ///  
        /// kHeading
        /// kPAngle
        /// kRAngle
        /// 
        /// </summary>
        private async Task SetHPRData()
        {
            // Send command to get data
            await Task.Run(() => _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.SetHPRDataComponentsCommands()));
        }

        /// <summary>
        /// Send a command to get all Data from the ADCP compass.
        /// This will get all the possible values from the compass.
        ///  
        /// kHeading
        /// kPAngle
        /// kRAngle
        /// kDistortion
        /// kCalStatus
        /// kPAligned
        /// kRAligned
        /// kIZAligned
        /// kXAligned
        /// kYAligned
        /// kZAligned
        /// 
        /// </summary>
        private async Task SetAllData()
        {
            // Send command to get data
            await Task.Run(() => _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.SetAllDataComponentsCommand()));
        }

        #endregion

        #region Display Data

        /// <summary>
        /// Display the data
        /// </summary>
        /// <param name="compassDataResponse"></param>
        private void DisplayData(RTI.PniPrimeCompassBinaryCodec.PniDataResponse compassDataResponse)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Heading: {0} Deg", compassDataResponse.Heading));
            sb.AppendLine(string.Format("Pitch:   {0} Deg", compassDataResponse.Pitch));
            sb.AppendLine(string.Format("Roll:    {0} Deg", compassDataResponse.Roll));

            if (compassDataResponse.CalStatus)
            {
                sb.AppendLine(string.Format("Cal Status: Calibrated"));
            }
            else
            {
                sb.AppendLine(string.Format("Cal Status: Not Calibrated"));
            }

            if (compassDataResponse.Distortion)
            {
                sb.AppendLine(string.Format("Distoriation: Distoriation"));
            }
            else
            {
                sb.AppendLine(string.Format("Distoriation: No Distoriation"));
            }

            sb.AppendLine(string.Format("pAligned:  {0} G", compassDataResponse.pAligned));
            sb.AppendLine(string.Format("rAligned:  {0} G", compassDataResponse.rAligned));
            sb.AppendLine(string.Format("izAligned: {0} G", compassDataResponse.izAligned));
            sb.AppendLine(string.Format("xAligned:  {0} uT", compassDataResponse.xAligned));
            sb.AppendLine(string.Format("yAligned:  {0} uT", compassDataResponse.yAligned));
            sb.AppendLine(string.Format("zAligned:  {0} uT", compassDataResponse.zAligned));

            SampleData = sb.ToString();
        }

        #endregion

        #region Process DAta

        /// <summary>
        /// Buffer the incoming data to process.
        /// </summary>
        /// <param name="data">PNI compass data.</param>
        public void ProcessCompassData(RTI.PniPrimeCompassBinaryCodec.PniDataResponse data)
        {
            _buffer.Enqueue(data);

            //if (_buffer.Count % 10 == 0)
            //{
                // Execute async
                if (!_isProcessingBuffer)
                {
                    // Execute async
                    Task.Run(() => ProcessCompassDataExecute());
                }
            //}
        }

        /// <summary>
        /// Record the compass data.
        /// </summary>
        private void ProcessCompassDataExecute()
        {
            while (!_buffer.IsEmpty)
            {
                _isProcessingBuffer = true;

                try
                {
                    // Get the latest data from the buffer
                    RTI.PniPrimeCompassBinaryCodec.PniDataResponse data = null;
                    if (_buffer.TryDequeue(out data))
                    {
                        // Display the data
                        DisplayData(data);

                        // Use the data for compass calibration
                        Task.Run(() => CompassCalData(data));

                        // Verify recording is turned on
                        if (_IsCompassRecording)
                        {

                            // Verify writer is created
                            if (_rawCompassRecordWriter != null)
                            {
                                // Seen thread exceptions for trying to have
                                // multiple threads write at the same time.
                                // The serial data is coming in and it is not writing fast enough
                                lock (_rawCompassRecordFileLock)
                                {
                                    string compassData = GetCompassData(data);

                                    // Write the data to the file
                                    _rawCompassRecordWriter.Write(compassData);

                                    // Accumulate the number of bytes written
                                    RawCompassBytesWritten += compassData.Length;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // Error writing lake test data
                    log.Error("Error raw ADCP data..", e);
                }
            }

            // Turn off processing
            _isProcessingBuffer = false;
        }

        #endregion

        #region Record Data

        /// <summary>
        /// Turn on recording.
        /// This will create the binary writer.
        /// </summary>
        private void RecordData()
        {
            if (_IsCompassRecording)
            {
                StartRecording(Pulse.Commons.DEFAULT_RECORD_DIR);
            }
            else
            {
                StopRecording();
            }
        }

        /// <summary>
        /// Create the recorder.
        /// </summary>
        /// <param name="dir">Directory to write the data to.</param>
        private void StartRecording(string dir)
        {
            // Create the writer if it does not exist
            if (_rawCompassRecordWriter == null)
            {
                // Create a file name
                DateTime currDateTime = DateTime.Now;

                string filename = string.Format("RawCompass_{0:yyyyMMddHHmmss}.csv", currDateTime);
                string filePath = string.Format("{0}\\{1}", dir, filename);

                try
                {
                    // Open the binary writer
                    _rawCompassRecordWriter = new StreamWriter(File.Open(filePath, FileMode.Create, FileAccess.Write));

                    // Set the raw ADCP file name
                    RawCompassRecordFileName = filePath;

                    // Reset the number of bytes written
                    RawCompassBytesWritten = 0;

                    // Write the data to the file
                    _rawCompassRecordWriter.Write(GetCompassDataHeader());
                }
                catch (Exception e)
                {
                    log.Error("Error creating the raw ADCP file.", e);
                }
            }
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        private void StopRecording()
        {
            try
            {
                if (_rawCompassRecordWriter != null)
                {
                    // Flush and close the writer
                    _rawCompassRecordWriter.Flush();
                    _rawCompassRecordWriter.Close();
                    _rawCompassRecordWriter.Dispose();
                    _rawCompassRecordWriter = null;
                }
            }
            catch (Exception e)
            {
                // Log error
                log.Error("Error closing Raw Compass Record.", e);
            }
        }



        /// <summary>
        /// Get the compass data header.
        /// </summary>
        /// <returns>Header for the data file.</returns>
        private string GetCompassDataHeader()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Heading (Deg),");
            sb.Append("Pitch (Deg),");
            sb.Append("Roll (Deg),");
            sb.Append("pAligned (G),");
            sb.Append("rAligned (G),");
            sb.Append("izAligned (G),");
            sb.Append("xAligned (uT),");
            sb.Append("yAligned (uT),");
            sb.Append("zAligned (uT),");
            sb.Append("CalStatus,");
            sb.Append("Distortion,");
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Create a CSV of the compass data.
        /// </summary>
        /// <param name="data">Data to decode.</param>
        private string GetCompassData(RTI.PniPrimeCompassBinaryCodec.PniDataResponse data)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(data.Heading + ",");
            sb.Append(data.Pitch + ",");
            sb.Append(data.Roll + ",");
            sb.Append(data.pAligned + ",");
            sb.Append(data.rAligned + ",");
            sb.Append(data.izAligned + ",");
            sb.Append(data.xAligned + ",");
            sb.Append(data.yAligned + ",");
            sb.Append(data.zAligned + ",");

            if (data.CalStatus)
            {
                sb.Append(1 + ",");
            }
            else
            {
                sb.Append(0 + ",");
            }

            if (data.Distortion)
            {
                sb.Append(1);
            }
            else
            {
                sb.Append(0);
            }

            sb.AppendLine();

            return sb.ToString();
        }

        #endregion

        #region Set Compass Cal Mag Default

        /// <summary>
        /// Set the Compass Calibration Accelerator default settings.
        /// This will use the factory calibration.
        /// </summary>
        private void OnSetDefaultCompassCalMag()
        {

            // Connect the compass
            //CompassConnect();

            // Send command to Read Compass data
            _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.GetDefaultCompassCalMagCommand());


            // Send command to Read Compass data
            _adcpConn.AdcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.GetDefaultCompassCalAccelCommand());

            // Now wait for the event that the command is complete
            // Then save the compass cal settings using kSave
            // When kSaveDone is received, disconnect from the Compass back to ADCP mode
        }

        #endregion

        #region Event Handlers

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
        /// When the serial port receives a new cal sample
        /// from the Compass calibration, set the new sample value.
        /// </summary>
        /// <param name="data">Data from the compass.</param>
        public void CompassCodecEventHandler(PniPrimeCompassBinaryCodec.CompassEventArgs data)
        {
            //// Declination
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kDeclination)
            //{
            //    var val = (float)data.Value;
            //    ShowConfig += "Declination: " + val + "\n";
            //}

            //// True North
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kTrueNorth || data.EventType == PniPrimeCompassBinaryCodec.ID.kModInfoResp)
            //{
            //    var param = data.Value as PniPrimeCompassBinaryCodec.PniModInfo;
            //    if (param != null)
            //    {
            //        DisplayModInfo(param);
            //        return;
            //    }

            //    try
            //    {
            //        var val = (bool)data.Value;
            //        ShowConfig += "True North: " + MathHelper.BoolToOnOffStr(val) + "\n";
            //    }
            //    catch (Exception) { }
            //}

            //// kBigEndian
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kBigEndian)
            //{
            //    var val = (bool)data.Value;
            //    ShowConfig += "Big Endian: " + MathHelper.BoolToOnOffStr(val) + "\n";
            //}

            //// kMountingRef
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kMountingRef)
            //{
            //    var val = (int)data.Value;
            //    ShowConfig += "Mounting Ref: " + PniPrimeCompassBinaryCodec.PniConfiguration.MountingRefToString((PniPrimeCompassBinaryCodec.PniConfiguration.PniMountingRef)val) + "\n";
            //}

            //// kUserCalStableCheck
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kUserCalStableCheck)
            //{
            //    var val = (bool)data.Value;
            //    ShowConfig += "User Calibration Stable Check: " + MathHelper.BoolToOnOffStr(val) + "\n";
            //}

            //// kUserCalNumPoints
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kUserCalNumPoints)
            //{
            //    var val = (uint)data.Value;
            //    ShowConfig += "User Calibration Number of Samples: " + val + "\n";
            //}

            //// kUserCalAutoSampling
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kUserCalAutoSampling)
            //{
            //    var val = (bool)data.Value;
            //    ShowConfig += "User Calibration Auto Sample: " + MathHelper.BoolToOnOffStr(val) + "\n";
            //}

            //// kParamResp or kBaudRate
            //// They have the same value
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kBaudRate || data.EventType == PniPrimeCompassBinaryCodec.ID.kParamResp)
            //{
            //    // kParamResp
            //    // Try as kParamResp
            //    var param = data.Value as PniPrimeCompassBinaryCodec.PniParameters;
            //    if (param != null)
            //    {
            //        DisplayParamResponse(param);
            //        return; // Stop here
            //    }

            //    // kBaudRate
            //    // Try as kBaudRate
            //    try
            //    {
            //        var val = (int)data.Value;
            //        ShowConfig += "BaudRate: " + PniPrimeCompassBinaryCodec.PniConfiguration.BaudRateToString(val) + "\n";
            //    }
            //    catch (InvalidCastException) { return; }    // If fails do nothing
            //}

            //// kAcqParamsResp
            //if (data.EventType == PniPrimeCompassBinaryCodec.ID.kAcqParamsResp)
            //{
            //    var param = data.Value as PniPrimeCompassBinaryCodec.PniAcqParam;
            //    if (param != null)
            //    {
            //        DisplayAcqParam(param);
            //    }
            //}

            // kDataResp
            if (data.EventType == RTI.PniPrimeCompassBinaryCodec.ID.kDataResp)
            {
                RTI.PniPrimeCompassBinaryCodec.PniDataResponse compassDataResponse = data.Value as RTI.PniPrimeCompassBinaryCodec.PniDataResponse;
                if (compassDataResponse != null)
                {
                    //Record the data
                    Task.Run(() => ProcessCompassData(compassDataResponse));
                }
            }


        }

        #endregion


    }
}
