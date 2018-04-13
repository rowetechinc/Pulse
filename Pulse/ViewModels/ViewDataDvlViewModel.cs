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
 * 09/30/2014      RC          4.1.0      Initial coding
 * 11/16/2016      RC          4.3.1      Added a thread.
 * 05/19/2016      RC          4.4.7      Added GPS Heading.
 * 02/13/2017      RC          4.5.1      Fixed Display All data.
 * 08/31/2017      RC          4.5.2      Added Ship Velocity to Bottom Track and Water Track.
 * 
 */
namespace RTI
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using OxyPlot;
    using OxyPlot.Series;
    using System;
    using System.Collections.Concurrent;
    using ReactiveUI;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using OxyPlot.Axes;
    using System.Threading;
    using System.ComponentModel;

    /// <summary>
    /// View the data DVL.  This will create all the
    /// objects to view the data graphically.
    /// </summary>
    public class ViewDataDvlViewModel : DisplayViewModel, IHandle<ProjectEvent>, IHandle<SelectedEnsembleEvent>
    {
        #region Class and Enums

        /// <summary>
        /// Struct to hold all 4 LineSeries
        /// so only 1 loop will need to be done
        /// to generate all ranges for one dataset.
        /// </summary>
        private class LineSeriesValues
        {
            public LineSeries Beam0Series { get; set; }
            public LineSeries Beam1Series { get; set; }
            public LineSeries Beam2Series { get; set; }
            public LineSeries Beam3Series { get; set; }
        }

        #endregion

        #region Defaults

        /// <summary>
        /// Default previous bin size.
        /// Number is negative so we know it has not been
        /// initialized.
        /// </summary>
        private const int DEFAULT_PREV_BIN_SIZE = -1;

        /// <summary>
        /// Default previous number of bins.
        /// Number is negative so we know it has not been
        /// initialized.
        /// </summary>
        private const int DEFAULT_PREV_NUM_BIN = -1;

        /// <summary>
        /// Maximun number of ensembles
        /// to store.
        /// </summary>
        private const int DEFAULT_MAX_ENSEMBLES = 1;

        #endregion

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Pulse manager to manage the application.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Options for this ViewModel.
        /// </summary>
        private ViewDataGraphicalOptions _options;

        /// <summary>
        /// DVL View Model.
        /// </summary>
        public ViewDataTextEnsembleViewModel TextEnsembleVM { get; set; }

        /// <summary>
        /// Compass Rose view model.
        /// </summary>
        public CompassRoseViewModel CompassRoseVM { get; set; }

        /// <summary>
        /// Buffer the incoming data.
        /// </summary>
        private ConcurrentQueue<DataSet.Ensemble> _buffer;

        /// <summary>
        /// Thread to decode incoming data.
        /// </summary>
        private Thread _processDataThread;

        /// <summary>
        /// Flag used to stop the thread.
        /// </summary>
        private bool _continue;

        /// <summary>
        /// Event to cause the thread
        /// to go to sleep or wakeup.
        /// </summary>
        private EventWaitHandle _eventWaitData;

        ///// <summary>
        ///// Limit how often the display updates.
        ///// </summary>
        ////private int _displayCounter;

        /// <summary>
        /// Value used to convert meters to feet.
        /// </summary>
        private const float METERS_TO_FEET = 3.2808399f;

        #endregion

        #region Properties

        #region Configuration

        /// <summary>
        /// Subsystem Data Configuration for this view.
        /// </summary>
        private SubsystemDataConfig _Config;
        /// <summary>
        /// Subsystem Data Configuration for this view.
        /// </summary>
        public SubsystemDataConfig Config
        {
            get { return _Config; }
            set
            {
                _Config = value;
                this.NotifyOfPropertyChange(() => this.Config);
                this.NotifyOfPropertyChange(() => this.IsPlayback);
            }
        }

        #endregion

        #region Display

        /// <summary>
        /// Display the CEPO index to describe this view model.
        /// </summary>
        public string Display
        {
            get
            {
                return _Config.IndexCodeString();
            }
        }

        /// <summary>
        /// Display the CEPO index to describe this view model.
        /// </summary>
        public string Title
        {
            get 
            {
                return string.Format("[{0}]{1}", _Config.CepoIndex.ToString(), _Config.SubSystem.CodedDescString()); 
            }
        }

        /// <summary>
        /// Flag if this view will display playback or live data.
        /// TRUE = Playback Data
        /// </summary>
        public bool IsPlayback
        {
            get
            {
                if (_Config.Source == EnsembleSource.Playback)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Flag if the data came from the serial port.
        /// </summary>
        public bool IsSerial
        {
            get
            {
                if (_Config.Source == EnsembleSource.Serial)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Flag if the data came from the Long Term Average.
        /// </summary>
        public bool IsLta
        {
            get
            {
                if (_Config.Source == EnsembleSource.LTA)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Flag if the data came from the Short Term Average.
        /// </summary>
        public bool IsSta
        {
            get
            {
                if (_Config.Source == EnsembleSource.STA)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Is Loading

        /// <summary>
        /// Flag for loading.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Flag for loading.
        /// </summary>
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                _IsLoading = value;
                this.NotifyOfPropertyChange(() => this.IsLoading);
            }
        }

        #endregion

        #region HPR

        /// <summary>
        /// Heading in degrees.
        /// </summary>
        private string _Heading;
        /// <summary>
        /// Heading in degrees.
        /// </summary>
        public string Heading
        {
            get { return _Heading; }
            set
            {
                _Heading = value;
                this.NotifyOfPropertyChange(() => this.Heading);
            }
        }

        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        private string _Pitch;
        /// <summary>
        /// Pitch in degrees.
        /// </summary>
        public string Pitch
        {
            get { return _Pitch; }
            set
            {
                _Pitch = value;
                this.NotifyOfPropertyChange(() => this.Pitch);
            }
        }

        /// <summary>
        /// Roll in degrees.
        /// </summary>
        private string _Roll;
        /// <summary>
        /// Roll in degrees.
        /// </summary>
        public string Roll
        {
            get { return _Roll; }
            set
            {
                _Roll = value;
                this.NotifyOfPropertyChange(() => this.Roll);
            }
        }

        #endregion

        #region Date and Time

        /// <summary>
        /// Date.
        /// </summary>
        private string _Date;
        /// <summary>
        /// Date.
        /// </summary>
        public string Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                this.NotifyOfPropertyChange(() => this.Date);
            }
        }

        /// <summary>
        /// Time.
        /// </summary>
        private string _Time;
        /// <summary>
        /// Time.
        /// </summary>
        public string Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                this.NotifyOfPropertyChange(() => this.Time);
            }
        }

        #endregion

        #region Voltage

        /// <summary>
        /// Voltage in volts.
        /// </summary>
        private string _Voltage;
        /// <summary>
        /// Voltage in volts.
        /// </summary>
        public string Voltage
        {
            get { return _Voltage; }
            set
            {
                _Voltage = value;
                this.NotifyOfPropertyChange(() => this.Voltage);
            }
        }

        #endregion

        #region Status

        /// <summary>
        /// Status.
        /// </summary>
        private string _Status;
        /// <summary>
        /// Status.
        /// </summary>
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                this.NotifyOfPropertyChange(() => this.Status);
            }
        }

        /// <summary>
        /// Bottom Track Status.
        /// </summary>
        private string _BtStatus;
        /// <summary>
        /// Bottom Track Status.
        /// </summary>
        public string BtStatus
        {
            get { return _BtStatus; }
            set
            {
                _BtStatus = value;
                this.NotifyOfPropertyChange(() => this.BtStatus);
            }
        }

        #endregion

        #region Colors

        /// <summary>
        /// List of all the color options.
        /// </summary>
        private List<OxyColor> _beamColorList;
        /// <summary>
        /// List of all the color options.
        /// </summary>
        public List<OxyColor> BeamColorList
        {
            get { return _beamColorList; }
            set
            {
                _beamColorList = value;
                this.NotifyOfPropertyChange(() => this.BeamColorList);
            }
        }

        /// <summary>
        /// Color for Beam 0 plots property.
        /// </summary>
        private OxyColor _beam0Color;
        /// <summary>
        /// Color for Beam 0 plots property.
        /// </summary>
        public OxyColor Beam0Color
        {
            get { return _beam0Color; }
            set
            {
                _beam0Color = value;
                this.NotifyOfPropertyChange(() => this.Beam0Color);
                this.NotifyOfPropertyChange(() => this.Beam0ColorStr);
            }
        }

        /// <summary>
        /// String for the Beam 0 Color to display as a background color.
        /// </summary>
        public string Beam0ColorStr
        {
            get { return "#" + BeamColor.ColorValue(_beam0Color); }
        }

        /// <summary>
        /// Color for Beam 1 plots property.
        /// </summary>
        private OxyColor _beam1Color;
        /// <summary>
        /// Color for Beam 1 plots property.
        /// </summary>
        public OxyColor Beam1Color
        {
            get { return _beam1Color; }
            set
            {
                _beam1Color = value;
                this.NotifyOfPropertyChange(() => this.Beam1Color);
                this.NotifyOfPropertyChange(() => this.Beam1ColorStr);
            }
        }

        /// <summary>
        /// String for the Beam 1 Color to display as a background color.
        /// </summary>
        public string Beam1ColorStr
        {
            get { return "#" + BeamColor.ColorValue(_beam1Color); }
        }

        /// <summary>
        /// Color for Beam 2 plots property.
        /// </summary>
        private OxyColor _beam2Color;
        /// <summary>
        /// Color for Beam 2 plots property.
        /// </summary>
        public OxyColor Beam2Color
        {
            get { return _beam2Color; }
            set
            {
                _beam2Color = value;
                this.NotifyOfPropertyChange(() => this.Beam2Color);
                this.NotifyOfPropertyChange(() => this.Beam2ColorStr);
            }
        }

        /// <summary>
        /// String for the Beam 2 Color to display as a background color.
        /// </summary>
        public string Beam2ColorStr
        {
            get { return "#" + BeamColor.ColorValue(_beam2Color); }
        }

        /// <summary>
        /// Color for Beam 4 plots property.
        /// </summary>
        private OxyColor _beam3Color;
        /// <summary>
        /// Color for Beam 3 plots property.
        /// </summary>
        public OxyColor Beam3Color
        {
            get { return _beam3Color; }
            set
            {
                _beam3Color = value;
                this.NotifyOfPropertyChange(() => this.Beam3Color);
                this.NotifyOfPropertyChange(() => this.Beam3ColorStr);
            }
        }

        /// <summary>
        /// String for the Beam 3 Color to display as a background color.
        /// </summary>
        public string Beam3ColorStr
        {
            get { return "#" + BeamColor.ColorValue(_beam3Color); }
        }

        #endregion

        #region Range Plot

        /// <summary>
        /// Range in meters.
        /// </summary>
        private string _Range;
        /// <summary>
        /// Range in meters.
        /// </summary>
        public string Range
        {
            get { return _Range; }
            set
            {
                _Range = value;
                this.NotifyOfPropertyChange(() => this.Range);
            }
        }

        /// <summary>
        /// Bottom Track Range plot.
        /// </summary>
        public TimeSeriesPlotViewModel BottomTrackRangePlot { get; set; }

        #endregion

        #region Velocity Plot

        /// <summary>
        /// Speed in meters per second.
        /// </summary>
        private string _BtSpeed;
        /// <summary>
        /// Speed in meters per second.
        /// </summary>
        public string BtSpeed
        {
            get { return _BtSpeed; }
            set
            {
                _BtSpeed = value;
                this.NotifyOfPropertyChange(() => this.BtSpeed);
            }
        }

        /// <summary>
        /// Bottom Track Velocity plot.
        /// </summary>
        public TimeSeriesPlotViewModel BottomTrackSpeedPlot { get; set; }

        #endregion 

        #region DMG Plot

        /// <summary>
        /// Distance Made Good Plot.
        /// </summary>
        private DmgPlotViewModel _DmgPlot;
        /// <summary>
        /// Distance Made Good Plot.
        /// </summary>
        public DmgPlotViewModel DmgPlot
        {
            get { return _DmgPlot; }
            set
            {
                _DmgPlot = value;
                this.NotifyOfPropertyChange(() => this.DmgPlot);
            }
        }

        /// <summary>
        /// Text from the calculation of
        /// Distance made good and Course made good.
        /// </summary>
        private ProjectReportText _reportText;
        /// <summary>
        /// Text from the calculation of
        /// Distance made good and Course made good.
        /// </summary>
        public ProjectReportText ReportText
        {
            get { return _reportText; }
            set
            {
                _reportText = value;
                this.NotifyOfPropertyChange(() => this.ReportText);
            }
        }

        #endregion

        #region Max Ensembles

        /// <summary>
        /// Maximum ensembles to display set by the user.
        /// </summary>
        public int DisplayMaxEnsembles
        {
            get { return _options.DisplayMaxEnsembles; }
            set
            {
                _options.DisplayMaxEnsembles = value;
                this.NotifyOfPropertyChange(() => this.DisplayMaxEnsembles);

                // Update the plots
                //_ContourPlot.MaxEnsembles = _options.DisplayMaxEnsembles;
                //_ensembleHistory.Limit = _options.DisplayMaxEnsembles;

                // Update the database with the latest options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region GPS

        /// <summary>
        /// Latitude.
        /// </summary>
        private string _Lat;
        /// <summary>
        /// Latitude.
        /// </summary>
        public string Lat
        {
            get { return _Lat; }
            set
            {
                _Lat = value;
                this.NotifyOfPropertyChange(() => this.Lat);
            }
        }

        /// <summary>
        /// Longitude.
        /// </summary>
        private string _Lon;
        /// <summary>
        /// Longitude.
        /// </summary>
        public string Lon
        {
            get { return _Lon; }
            set
            {
                _Lon = value;
                this.NotifyOfPropertyChange(() => this.Lon);
            }
        }

        /// <summary>
        /// GPS Altitude.
        /// </summary>
        private string _Alt;
        /// <summary>
        /// GPS Altitude.
        /// </summary>
        public string Alt
        {
            get { return _Alt; }
            set
            {
                _Alt = value;
                this.NotifyOfPropertyChange(() => this.Alt);
            }
        }

        /// <summary>
        /// GPS fix status.
        /// </summary>
        private string _GpsFix;
        /// <summary>
        /// GPS fix status.
        /// </summary>
        public string GpsFix
        {
            get { return _GpsFix; }
            set
            {
                _GpsFix = value;
                this.NotifyOfPropertyChange(() => this.GpsFix);
            }
        }

        /// <summary>
        /// GPS Satellite count.
        /// </summary>
        private int _SatCount;
        /// <summary>
        /// GPS Satellite count.
        /// </summary>
        public int SatCount
        {
            get { return _SatCount; }
            set
            {
                _SatCount = value;
                this.NotifyOfPropertyChange(() => this.SatCount);
            }
        }

        /// <summary>
        /// GPS Satellite view.
        /// </summary>
        private int _SatView;
        /// <summary>
        /// GPS Satellite view.
        /// </summary>
        public int SatView
        {
            get { return _SatView; }
            set
            {
                _SatView = value;
                this.NotifyOfPropertyChange(() => this.SatView);
            }
        }

        /// <summary>
        /// GPS Satellite speed.
        /// </summary>
        private string _SatSpeed;
        /// <summary>
        /// GPS Satellite speed.
        /// </summary>
        public string SatSpeed
        {
            get { return _SatSpeed; }
            set
            {
                _SatSpeed = value;
                this.NotifyOfPropertyChange(() => this.SatSpeed);
            }
        }

        /// <summary>
        /// GPS Satellite heading.
        /// </summary>
        private string _SatHeading;
        /// <summary>
        /// GPS Satellite heading.
        /// </summary>
        public string SatHeading
        {
            get { return _SatHeading; }
            set
            {
                _SatHeading = value;
                this.NotifyOfPropertyChange(() => this.SatHeading);
            }
        }

        #endregion

        #region Display Ensemble Data

        /// <summary>
        /// Current ensemble to display.
        /// </summary>
        private DataSet.Ensemble _DisplayEnsemble;
        /// <summary>
        /// Current ensemble to display.
        /// </summary>
        public DataSet.Ensemble DisplayEnsemble
        {
            get { return _DisplayEnsemble; }
            set
            {
                _DisplayEnsemble = value;
                this.NotifyOfPropertyChange(() => this.DisplayEnsemble);

                // Update the rounded values
                //UpdateDisplay();
            }
        }

        #endregion

        #region BT

        /// <summary>
        /// Get the Bottom Track Range for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtRangeB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_0_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Range for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtRangeB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_1_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Range for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtRangeB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_2_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Range for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtRangeB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.Range[DataSet.Ensemble.BEAM_3_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtBeamVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_0_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtBeamVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_1_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtBeamVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_2_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtBeamVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.BeamVelocity[DataSet.Ensemble.BEAM_3_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity Good Ping for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodBeamB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_0_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity Good Ping for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodBeamB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_1_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity Good Ping for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodBeamB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_2_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Beam Velocity Good Ping for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodBeamB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.BeamGood[DataSet.Ensemble.BEAM_3_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtInstrVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_0_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtInstrVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_1_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtInstrVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_2_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtInstrVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.InstrumentVelocity[DataSet.Ensemble.BEAM_3_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity Good Ping for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodInstrB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_0_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity Good Ping for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodInstrB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_1_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity Good Ping for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodInstrB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_2_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Instrument Velocity Good Ping for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodInstrB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.InstrumentGood[DataSet.Ensemble.BEAM_3_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtEarthVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_0_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtEarthVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_1_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtEarthVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_2_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtEarthVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.EarthVelocity[DataSet.Ensemble.BEAM_3_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity Good Ping for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodEarthB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_0_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity Good Ping for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodEarthB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_1_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity Good Ping for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodEarthB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_2_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Earth Velocity Good Ping for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtGoodEarthB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.EarthGood[DataSet.Ensemble.BEAM_3_INDEX]).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Ship Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtShipVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_0_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Ship Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtShipVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_1_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Ship Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtShipVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_2_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Ship Velocity for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtShipVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return SetMeasurementValue(_DisplayEnsemble.BottomTrackData.ShipVelocity[DataSet.Ensemble.BEAM_3_INDEX], "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Signal To Noise Ratio for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtSnrB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_0_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Signal To Noise Ratio for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtSnrB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_1_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Signal To Noise Ratio for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtSnrB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_2_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Signal To Noise Ratio for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtSnrB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.SNR[DataSet.Ensemble.BEAM_3_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Amplitude Ratio for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtAmpB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_0_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Amplitude Ratio for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtAmpB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_1_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Amplitude Ratio for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtAmpB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_2_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Amplitude Ratio for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtAmpB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.Amplitude[DataSet.Ensemble.BEAM_3_INDEX]).ToString("0.0");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Correlation Ratio for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtCorrB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams >= 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_0_INDEX]).ToString("0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Correlation Ratio for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtCorrB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 1)
                {
                    return (_DisplayEnsemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_1_INDEX]).ToString("0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Correlation Ratio for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtCorrB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 2)
                {
                    return (_DisplayEnsemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_2_INDEX]).ToString("0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Bottom Track Correlation Ratio for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string BtCorrB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsBottomTrackAvail && _DisplayEnsemble.BottomTrackData.NumBeams > 3)
                {
                    return (_DisplayEnsemble.BottomTrackData.Correlation[DataSet.Ensemble.BEAM_3_INDEX]).ToString("0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Water Track

        /// <summary>
        /// Water Track Instrument Depth Layer.
        /// </summary>
        public string WtInstrumentDepthLayer
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsInstrumentWaterMassAvail)
                {
                    return _DisplayEnsemble.InstrumentWaterMassData.WaterMassDepthLayer.ToString("0"); ;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Water Track Earth Depth Layer.
        /// </summary>
        public string WtEarthDepthLayer
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsEarthWaterMassAvail)
                {
                    return _DisplayEnsemble.EarthWaterMassData.WaterMassDepthLayer.ToString("0"); ;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Instrument Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtInstrVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsInstrumentWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.InstrumentWaterMassData.VelocityX, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Instrument Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtInstrVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsInstrumentWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.InstrumentWaterMassData.VelocityY, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Instrument Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtInstrVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsInstrumentWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.InstrumentWaterMassData.VelocityZ, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Instrument Velocity for Beam 3.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtInstrVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsInstrumentWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.InstrumentWaterMassData.VelocityQ, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Earth Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtEarthVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsEarthWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.EarthWaterMassData.VelocityEast, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Earth Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtEarthVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsEarthWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.EarthWaterMassData.VelocityNorth, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Earth Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtEarthVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsEarthWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.EarthWaterMassData.VelocityVertical, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Ship Velocity for Beam 0.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtShipVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsShipWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.ShipWaterMassData.VelocityTransverse, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Ship Velocity for Beam 1.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtShipVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsShipWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.ShipWaterMassData.VelocityLongitudinal, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Water Track Ship Velocity for Beam 2.
        /// Round to 3 decimal places.
        /// </summary>
        public string WtShipVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsShipWaterMassAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.ShipWaterMassData.VelocityNormal, "0.000");
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region DVL

        #region Instrument Velocity

        /// <summary>
        /// Bottom Track Instrument X Velocity.
        /// </summary>
        public string BtXVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtXVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Instrument Y Velocity.
        /// </summary>
        public string BtYVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtYVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Instrument Z Velocity.
        /// </summary>
        public string BtZVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtZVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Instrument Error Velocity.
        /// </summary>
        public string BtErrorVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtErrorVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Ship Velocity

        /// <summary>
        /// Bottom Track Ship X Velocity.
        /// </summary>
        public string BtTransverseVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtTransverseVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Ship Y Velocity.
        /// </summary>
        public string BtLongitudinalVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtLongitudinalVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Ship Z Velocity.
        /// </summary>
        public string BtNormalVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtNormalVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Ship Error Velocity.
        /// </summary>
        public string BtShipErrorVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtShipErrorVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Earth Velocity

        /// <summary>
        /// Bottom Track Earth East Velocity.
        /// </summary>
        public string BtEastVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtEastVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Earth North Velocity.
        /// </summary>
        public string BtNorthVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtNorthVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Earth Upward Velocity.
        /// </summary>
        public string BtUpwardVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtUpwardVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Earth Velocity

        /// <summary>
        /// Bottom Track Earth East Distance.
        /// </summary>
        public string BtEastDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtEastDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Earth North Distance.
        /// </summary>
        public string BtNorthDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtNorthDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Earth Upward Distance.
        /// </summary>
        public string BtUpwardDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.BtUpwardDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Earth Center.
        /// </summary>
        public string BtEarthRangeToWaterMassCenter
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.BtRangeToBottom.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Range

        /// <summary>
        /// Bottom Track Range Beam 0.
        /// </summary>
        public string RangeBeam0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.RangeBeam0, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Range Beam 1.
        /// </summary>
        public string RangeBeam1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.RangeBeam1, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Range Beam 2.
        /// </summary>
        public string RangeBeam2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.RangeBeam2, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Bottom Track Range Beam 3.
        /// </summary>
        public string RangeBeam3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.RangeBeam3, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region DMG Earth

        /// <summary>
        /// DMG Earth East.
        /// </summary>
        public string DmgEast
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgEast, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Earth North.
        /// </summary>
        public string DmgNorth
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgNorth, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Earth Upward.
        /// </summary>
        public string DmgUpward
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgUpward, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Earth Error.
        /// </summary>
        public string DmgError
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgError, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }


        #endregion

        #region HPR

        /// <summary>
        /// DVL Sample Number.
        /// </summary>
        public string DvlSampleNumber
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.SampleNumber.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Heading.
        /// </summary>
        public string DvlHeading
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.Heading.ToString("0.00") + " °";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Pitch.
        /// </summary>
        public string DvlPitch
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.Pitch.ToString("0.00") + " °";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Roll.
        /// </summary>
        public string DvlRoll
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.Roll.ToString("0.00") + " °";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Date and Time.
        /// </summary>
        public string DvlDateAndTime
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.DateAndTime.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Salinity.
        /// </summary>
        public string DvlSalinity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.Salinity.ToString("0.00") + " ppt";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Water Temperature.
        /// </summary>
        public string DvlWaterTemp
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.WaterTemp.ToString("0.00") + " C";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Transducer Depth.
        /// </summary>
        public string DvlTransducerDepth
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.TransducerDepth.ToString("0.00") + " m";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Speed of Sound.
        /// </summary>
        public string DvlSpeedOfSound
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.SpeedOfSound.ToString("0.00") + " m/s";
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL BIT.
        /// </summary>
        public string DvlBit
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.BIT.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DVL Pressure.
        /// </summary>
        public string DvlPressure
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.Pressure.ToString("0.00") + " Pa";
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Reference Layer

        /// <summary>
        /// Reference Layer Min Layer.
        /// </summary>
        public string RefLayerMin
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.RefLayerMin.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Max Layer.
        /// </summary>
        public string RefLayerMax
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return _DisplayEnsemble.DvlData.RefLayerMax.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Reference Layer Instrument Velocity

        /// <summary>
        /// Reference Layer Instrument X Velocity.
        /// </summary>
        public string WmXVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmXVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Instrument Y Velocity.
        /// </summary>
        public string WmYVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmYVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Instrument Z Velocity.
        /// </summary>
        public string WmZVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmZVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Instrument Error Velocity.
        /// </summary>
        public string WmErrorVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmErrorVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Reference Layer Ship Velocity

        /// <summary>
        /// Reference Layer Ship X Velocity.
        /// </summary>
        public string WmTransverseVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmTransverseVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Ship Y Velocity.
        /// </summary>
        public string WmLongitudinalVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmLongitudinalVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Ship Z Velocity.
        /// </summary>
        public string WmNormalVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmNormalVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Reference Layer Earth Velocity

        /// <summary>
        /// Reference Layer Earth East Velocity.
        /// </summary>
        public string WmEastVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmEastVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Earth North Velocity.
        /// </summary>
        public string WmNorthVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmNorthVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Earth Upward Velocity.
        /// </summary>
        public string WmUpwardVelocity
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmUpwardVelocity, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Reference Layer Earth Distance

        /// <summary>
        /// Reference Layer Earth East Distance.
        /// </summary>
        public string WmEastDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmEastDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Earth North Distance.
        /// </summary>
        public string WmNorthDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmNorthDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Earth Upward Distance.
        /// </summary>
        public string WmUpwardDistance
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmUpwardDistance, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference Layer Earth Center.
        /// </summary>
        public string WmEarthRangeToWaterMassCenter
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.WmEarthRangeToWaterMassCenter, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region DMG Reference Layer Earth

        /// <summary>
        /// DMG Reference Layer Earth East.
        /// </summary>
        public string DmgRefEast
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgRefEast, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Reference Layer Earth North.
        /// </summary>
        public string DmgRefNorth
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgRefNorth, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Reference Layer Earth Upward.
        /// </summary>
        public string DmgRefUpward
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgRefUpward, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DMG Reference Layer Earth Error.
        /// </summary>
        public string DmgRefError
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return SetMeasurementValue(_DisplayEnsemble.DvlData.DmgRefError, "0.000");
                }
                else
                {
                    return "";
                }
            }
        }


        #endregion

        #endregion

        #region Measurement Standard

        /// <summary>
        /// List of possible measurement standard types.
        /// </summary>
        private BindingList<Core.Commons.MeasurementStandards> _measurementStandardList;
        /// <summary>
        /// List of possible measurement standard types.
        /// </summary>
        public BindingList<Core.Commons.MeasurementStandards> MeasurementStandardList
        {
            get { return _measurementStandardList; }
            set
            {
                _measurementStandardList = value;
                this.NotifyOfPropertyChange(() => this.MeasurementStandardList);
            }
        }

        /// <summary>
        /// Set which measurement standard to use.
        /// </summary>
        public Core.Commons.MeasurementStandards MeasurementStandard
        {
            get { return _options.MeasurementStandard; }
            set
            {
                _options.MeasurementStandard = value;
                this.NotifyOfPropertyChange(() => this.MeasurementStandard);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to clear the plots.
        /// This will clear all the buffers.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ClearPlotsCommand { get; protected set; }

        /// <summary>
        /// Command to close this VM.
        /// </summary>
        public ReactiveCommand<object> CloseVMCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// <param name="config">Configuration containing data source and SubsystemConfiguration.</param>
        /// </summary>
        public ViewDataDvlViewModel(SubsystemDataConfig config) 
            : base("ViewDataDvlViewModel")
        {
            try
            {
                // Set Subsystem 
                _Config = config;
                //_displayCounter = 0;

                IsLoading = false;

                // Get the Event Aggregator
                _events = IoC.Get<IEventAggregator>();

                // Get PulseManager
                _pm = IoC.Get<PulseManager>();
                _pm.RegisterDisplayVM(this);

                //_isProcessingBuffer = false;
                _buffer = new ConcurrentQueue<DataSet.Ensemble>();

                // Initialize the thread
                _continue = true;
                _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
                _processDataThread = new Thread(ProcessDataThread);
                _processDataThread.Name = string.Format("DVL View: {0}", config.DescString());
                _processDataThread.Start();

                // Get the options from the database
                GetOptionsFromDatabase();

                // Plots
                DisplayMaxEnsembles = 1;
                SetupPlots();
                Beam0Color = OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0);
                Beam1Color = OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_1);
                Beam2Color = OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_2);
                Beam3Color = OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_3);

                _reportText = new ProjectReportText();

                // Distance Made Good Plot
                DmgPlot = new DmgPlotViewModel();

                // Create the Text view
                TextEnsembleVM = new ViewDataTextEnsembleViewModel(config);

                // Create the Text view
                CompassRoseVM = new CompassRoseViewModel();

                // Create a command to clear plots
                ClearPlotsCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => Clear()));

                // Close the VM
                CloseVMCommand = ReactiveCommand.Create();
                CloseVMCommand.Subscribe(_ => _events.PublishOnUIThread(new CloseVmEvent(_Config)));

                _events.Subscribe(this);
            }
            catch(Exception e)
            {
                log.Error("Error opening DVL VM.", e);
            }
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            // Wake up the thread to process data
            _continue = false;
            _eventWaitData.Set();
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
            var ssConfig = new SubsystemConfiguration(_Config.SubSystem, _Config.CepoIndex, _Config.SubsystemConfigIndex);
            _options = _pm.AppConfiguration.GetGraphicalOptions(ssConfig);

            // Notify all the properties
            NotifyOptionPropertyChange();
        }

        /// <summary>
        /// Notify all the properties of a change
        /// when a new option object is set.
        /// </summary>
        private void NotifyOptionPropertyChange()
        {

        }

        /// <summary>
        /// Update the database with the latest options.
        /// </summary>
        private void UpdateDatabaseOptions()
        {
            // SubsystemDataConfig needs to be converted to a SubsystemConfiguration
            // because the SubsystemConfig will be compared in AppConfiguration to determine
            // where to save the settings.  Because SubsystemDataConfig and SubsystemConfiguration
            // are not the same type, it will not pass Equal()
            var ssConfig = new SubsystemConfiguration(_Config.SubSystem, _Config.CepoIndex, _Config.SubsystemConfigIndex);

            _pm.AppConfiguration.SaveGraphicalOptions(ssConfig, _options);
        }

        #endregion

        #region Display Data

        /// <summary>
        /// Display the given ensemble.
        /// </summary>
        /// <param name="ensemble">Ensemble to display.</param>
        public void DisplayData(DataSet.Ensemble ensemble)
        {
            _buffer.Enqueue(ensemble);

            //if ((++_displayCounter % 5) == 0)
            //{
                // Wake up the thread to process data
                _eventWaitData.Set();

                //_displayCounter = 0;
            //}
        }

        /// <summary>
        /// Execute the displaying of the data async.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                // Wakeup the thread with a signal
                // Have a 2 second timeout to see if we need to shutdown the thread
                _eventWaitData.WaitOne(2000);

                while (!_buffer.IsEmpty)
                {
                    //_isProcessingBuffer = true;

                    // Get the latest data from the buffer
                    DataSet.Ensemble ensemble = null;
                    if (_buffer.TryDequeue(out ensemble))
                    {
                        // Verify the ensemble is good
                        if (ensemble == null || ensemble.EnsembleData == null || !ensemble.IsEnsembleAvail)
                        {
                            //_isProcessingBuffer = false;
                            continue;
                        }

                        // If no subsystem is given, then a project is not selected
                        // So receive all data and display
                        // If the serial number is not set, this may be an old ensemble
                        // Try to display it anyway
                        if (!_Config.SubSystem.IsEmpty() && !ensemble.EnsembleData.SysSerialNumber.IsEmpty())
                        {
                            // Verify the subsystem matches this viewmodel's subystem.
                            if ((_Config.SubSystem != ensemble.EnsembleData.GetSubSystem())        // Check if Subsystem matches 
                                    || (_Config != ensemble.EnsembleData.SubsystemConfig))         // Check if Subsystem Config matches
                            {
                                //_isProcessingBuffer = false;
                                continue;
                            }
                        }

                        try
                        {
                            // Update Plots
                            AddSeries(ensemble);

                            // Display the data
                            DisplayDvlData(ensemble);

                            // Add the data to the Text Display
                            TextEnsembleVM.ReceiveEnsemble(ensemble);
                        }
                        catch (Exception e)
                        {
                            log.Error("Error adding ensemble to plots.", e);
                        }
                    }
                }
            }

            //_isProcessingBuffer = false;

            return;
        }

        /// <summary>
        /// Only update the contour plot and timeseries.  This will need each ensemble.
        /// The profile plots only need the last ensemble. 
        /// </summary>
        /// <param name="ensembles">Event that contains the Ensembles to display.</param>
        /// <param name="maxEnsembles">Maximum Ensembles to display.</param>
        public async void DisplayBulkData(Cache<long, DataSet.Ensemble> ensembles, int maxEnsembles = 0)
        {
            await Task.Run(() => AddSeriesBulk(ensembles, maxEnsembles));
        }

        #endregion

        #region Display DVL data

        /// <summary>
        /// Display the data.  Use Bottom Track data
        /// as the primary source.
        /// </summary>
        /// <param name="ensemble">Ensemble to display.</param>
        private void DisplayDvlData(DataSet.Ensemble ensemble)
        {
            // Display Ensemble
            DisplayEnsemble = ensemble;

            // HPR and Speed
            if(ensemble.IsBottomTrackAvail)
            {
                _Heading = ensemble.BottomTrackData.Heading.ToString("0.000") + "°";
                _Pitch = ensemble.BottomTrackData.Pitch.ToString("0.000") + "°";
                _Roll = ensemble.BottomTrackData.Roll.ToString("0.000") + "°";
                _Range = ensemble.BottomTrackData.GetAverageRange().ToString("0.000") + " m";
                _BtSpeed = ensemble.BottomTrackData.GetVelocityMagnitude().ToString("0.000") + " m/s";
                _BtStatus = ensemble.BottomTrackData.Status.ToString();
            }
            else if (ensemble.IsAncillaryAvail)
            {
                _Heading = ensemble.AncillaryData.Heading.ToString("0.000") + "°";
                _Pitch = ensemble.AncillaryData.Pitch.ToString("0.000") + "°";
                _Roll = ensemble.AncillaryData.Roll.ToString("0.000") + "°";
            }
            else if(ensemble.IsNmeaAvail)
            {
                if(ensemble.NmeaData.IsGphdtAvail())
                {
                    _Heading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees.ToString() + "°";
                }
                //if(ensemble.NmeaData.IsGpvtgAvail())
                //{
                //    _BtSpeed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().ToString() + " m/s";
                //}
            }
            else if (ensemble.IsDvlDataAvail)
            {
                _Heading = ensemble.DvlData.Heading.ToString("0.000") + "°";
                _Pitch = ensemble.DvlData.Pitch.ToString("0.000") + "°";
                _Roll = ensemble.DvlData.Roll.ToString("0.000") + "°";
            }

            // Date and Time
            if(ensemble.IsEnsembleAvail)
            {
                _Date = ensemble.EnsembleData.EnsDateTime.ToShortDateString();
                _Time = ensemble.EnsembleData.EnsDateTime.ToLongTimeString();
                _Status = ensemble.EnsembleData.Status.ToString();
            }

            // Voltage
            if(ensemble.IsSystemSetupAvail)
            {
                _Voltage = ensemble.SystemSetupData.Voltage.ToString("0.000") + " v";
            }

            // GPS
            if (ensemble.IsNmeaAvail)
            {
                if (ensemble.NmeaData.IsGpggaAvail())
                {
                    _Lat = ensemble.NmeaData.GPGGA.Position.Latitude.ToString();
                    _Lon = ensemble.NmeaData.GPGGA.Position.Longitude.ToString();
                    _GpsFix = ensemble.NmeaData.GPGGA.FixQuality.ToString();
                    _Alt = ensemble.NmeaData.GPGGA.Altitude.ToMeters().Value.ToString("0.000") + " m";
                }
                if(ensemble.NmeaData.IsGpgsaAvail())
                {
                    _SatCount = ensemble.NmeaData.GPGSA.FixedSatellites.Count;
                }
                if(ensemble.NmeaData.IsGpgsvAvail())
                {
                    _SatView = ensemble.NmeaData.GPGSV.SatellitesInView;
                }
                if(ensemble.NmeaData.IsGpvtgAvail())
                {
                    _SatSpeed = ensemble.NmeaData.GPVTG.Speed.ToMetersPerSecond().Value.ToString("0.000") + " m/s";
                }
                if (ensemble.NmeaData.IsGphdtAvail())
                {
                    _SatHeading = ensemble.NmeaData.GPHDT.Heading.DecimalDegrees.ToString() + "°";
                }
            }

            // Update the display
            this.NotifyOfPropertyChange(null);
        }

        #endregion

        #region Clear

        /// <summary>
        /// Clear the plots and text.
        /// </summary>
        private void Clear()
        {
            // Clear text
            Heading = "0°";
            Pitch = "0°";
            Roll = "0°";
            Range = "0.0 m";
            BtSpeed = "0.0 m/s";
            BtStatus = "";
            Date = "";
            Time = "";
            Status = "";
            Lat = "";
            Lon = "";
            GpsFix = "";
            Voltage = "";

            DisplayMaxEnsembles = 1;

            // Clear the Report text
            ReportText.Clear();

            // Clear plots
            ClearPlots();
        }

        #endregion

        #region Plots

        #region Setup Plots

        /// <summary>
        /// Setup all the plots.
        /// </summary>
        private void SetupPlots()
        {
            // Bottom Track Range plot
            var btRangeSource = new DataSource(DataSource.eSource.BottomTrack);
            var btRangeType = new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range);
            BottomTrackRangePlot = new TimeSeriesPlotViewModel(new SeriesType(btRangeSource, btRangeType));
            BottomTrackRangePlot.AddSeries(btRangeSource, btRangeType, 0, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0));
            BottomTrackRangePlot.AddSeries(btRangeSource, btRangeType, 1, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_1));
            BottomTrackRangePlot.AddSeries(btRangeSource, btRangeType, 2, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_2));
            BottomTrackRangePlot.AddSeries(btRangeSource, btRangeType, 3, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_3));
            BottomTrackRangePlot.Plot.PlotMargins = new OxyPlot.OxyThickness(20, 0, 0, 0);
            BottomTrackRangePlot.Plot.TitlePadding = 0;
            BottomTrackRangePlot.Plot.Padding = new OxyPlot.OxyThickness(0);

            // Bottom Track Speed Plot
            var btVelSource = new DataSource(DataSource.eSource.AncillaryBottomTrack);
            var btVelType = new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Speed);
            BottomTrackSpeedPlot = new TimeSeriesPlotViewModel(new SeriesType(btVelSource, btVelType));
            BottomTrackSpeedPlot.AddSeries(btVelSource, btVelType, 0, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0));
            BottomTrackSpeedPlot.Plot.PlotMargins = new OxyPlot.OxyThickness(20, 0, 0, 0);
            BottomTrackSpeedPlot.Plot.TitlePadding = 0;
            BottomTrackSpeedPlot.Plot.Padding = new OxyPlot.OxyThickness(0);
        }

        #endregion

        #region Update Plots

        /// <summary>
        /// Add the data to the plots.
        /// </summary>
        /// <param name="ensemble">Ensemble to get the data.</param>
        private async void AddSeries(DataSet.Ensemble ensemble)
        {
            if (ensemble.IsBottomTrackAvail) // Bottom Track plots
            {
                // Increase Max Ensembles
                DisplayMaxEnsembles++;

                // Bottom Track Range
                await BottomTrackRangePlot.AddIncomingData(ensemble, DisplayMaxEnsembles);

                // Bottom Track Velocity
                await BottomTrackSpeedPlot.AddIncomingData(ensemble, DisplayMaxEnsembles);

                // Accumulate Report data
                //await Task.Run(() => _reportText.AddIncomingData(ensemble));
                _reportText.AddIncomingData(ensemble);

                // Distance Made Good
                await AddDistanceMadeGoodSeries(_reportText);

                // Update compass rose
                UpdateCompassRose(ensemble.BottomTrackData.Heading, ensemble.BottomTrackData.Pitch, ensemble.BottomTrackData.Roll);
            }
            // Update Compass Rose
            else if (ensemble.IsAncillaryAvail)
            {
                UpdateCompassRose(ensemble.AncillaryData.Heading, ensemble.AncillaryData.Pitch, ensemble.AncillaryData.Roll);
            }
            else if (ensemble.IsDvlDataAvail)
            {
                UpdateCompassRose(ensemble.DvlData.Heading, ensemble.DvlData.Pitch, ensemble.DvlData.Roll);
            }
        }

        /// <summary>
        /// Add the bulk data to the plots.
        /// </summary>
        /// <param name="ensembles">Ensembles to get the data.</param>
        /// <param name="maxEnsembles">Maximum ensembles to display.</param>
        private async void AddSeriesBulk(Cache<long, DataSet.Ensemble> ensembles, int maxEnsembles = 0)
        {
            // Set a flag for loading
            IsLoading = true;

            // Bottom Track Range
            BottomTrackRangePlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);

            // Bottom Track Velocity
            BottomTrackSpeedPlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);

            // Accumulate Report data
            await Task.Run(() => _reportText.LoadData(ensembles, _Config.SubSystem, _Config));

            // Distance Made Good
            await AddDistanceMadeGoodSeries(_reportText);


            if(ensembles.Count() > 0)
            {
                // Get the last ensemble
                var ensemble = ensembles.IndexValue(ensembles.Count() - 1);

                if (ensemble.IsBottomTrackAvail) // Bottom Track plots
                {

                    // Update compass rose
                    UpdateCompassRose(ensemble.BottomTrackData.Heading, ensemble.BottomTrackData.Pitch, ensemble.BottomTrackData.Roll);
                }
                // Update Compass Rose
                else if (ensemble.IsAncillaryAvail)
                {
                    UpdateCompassRose(ensemble.AncillaryData.Heading, ensemble.AncillaryData.Pitch, ensemble.AncillaryData.Roll);
                }
                else if (ensemble.IsDvlDataAvail)
                {
                    UpdateCompassRose(ensemble.DvlData.Heading, ensemble.DvlData.Pitch, ensemble.DvlData.Roll);
                }

                // Display the data
                DisplayDvlData(ensemble);

                // Add the data to the Text Display
                TextEnsembleVM.ReceiveEnsemble(ensemble);
            }

            // Set a flag for loading
            IsLoading = false;

        }

        #region Distance Made Good Plot

        /// <summary>
        /// Add the Distance Made Good values to the plot.
        /// This will keep a list of the last MAX_DATASETS ranges
        /// and plot them.
        /// </summary>
        /// <param name="prt">Get the latest data.</param>
        private async Task AddDistanceMadeGoodSeries(ProjectReportText prt)
        {
            // Copy the data needed by the DMG plot
            DmgPlotViewModel.DmgPlotData data = new DmgPlotViewModel.DmgPlotData();
            data.AddData(prt.GpsPoints, prt.BtEarthPoints);

            await DmgPlot.AddIncomingData(data);
        }

        #endregion

        #endregion

        #region Clear Plots

        /// <summary>
        /// Clear all the values for the plots.
        /// </summary>
        public void ClearPlots()
        {
            // Clear plots
            BottomTrackRangePlot.ClearIncomingData();
            BottomTrackSpeedPlot.ClearIncomingData();
            DmgPlot.ClearPlot();

        }

        #endregion

        #endregion

        #region Compass Rose

        /// <summary>
        /// Update the compass rose.
        /// </summary>
        /// <param name="heading">Heading.</param>
        /// <param name="pitch">Pitch</param>
        /// <param name="roll">Roll.</param>
        private void UpdateCompassRose(float heading, float pitch, float roll)
        {
            // Convert the roll from -180 to 180 to 0-360
            float rollConv = 180.0f + roll;
            //float pitchConv = 90.0f + pitch; 

            CompassRoseVM.AddIncomingData(heading, pitch, rollConv);
        }

        #endregion

        #region Measurement Standard

        /// <summary>
        /// Depending on the measurement standard set, convert
        /// the value given from meters to feet.
        /// </summary>
        /// <param name="value">Value to convert if set to Standard.</param>
        /// <param name="converter">String used in ToString to set number of decimal places.</param>
        /// <returns>String of the value.</returns>
        private string SetMeasurementValue(float value, string converter)
        {
            // Check for a bad value
            if (value == DataSet.Ensemble.BAD_VELOCITY)
            {
                return "  -   ";
                //return DataSet.BeamVelocityDataSet.BAD_VELOCITY_PLACEHOLDER;
            }

            string result = "";
            if (MeasurementStandard == Core.Commons.MeasurementStandards.METRIC)
            {
                result = value.ToString(converter);
            }
            else
            {
                // Convert meters to feet
                value *= METERS_TO_FEET;
                result = value.ToString(converter);
            }

            return result;
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Eventhandler for the latest ensemble data.
        /// This will filter the ensembles based off the subsystem type.
        /// It will set the max ensemble and then check the data and display the data.
        /// </summary>
        /// <param name="ensEvent">Ensemble event.</param>
        public override void Handle(EnsembleEvent ensEvent)
        {
            // Check if source matches this display
            if (_Config.Source != ensEvent.Source || ensEvent.Ensemble == null)
            {
                return;
            }

            // Display the data
            //Task.Run(() => DisplayData(ensEvent.Ensemble));
            DisplayData(ensEvent.Ensemble);
        }

        /// <summary>
        /// Bulk Ensemble event.
        /// </summary>
        /// <param name="ensEvent"></param>
        public override void Handle(BulkEnsembleEvent ensEvent)
        {
            // DO NOTHING
        }

        /// <summary>
        /// Update the velocity, correlaton and amplitude plot when
        /// an ensemble is selected.
        /// </summary>
        /// <param name="ensEvent">Selected Ensemble event.</param>
        public void Handle(SelectedEnsembleEvent ensEvent)
        {
            // Verify the ensemble is good
            if (ensEvent.Ensemble == null || ensEvent.Ensemble.EnsembleData == null || !ensEvent.Ensemble.IsEnsembleAvail)
            {
                return;
            }

            // If no subsystem is given, then a project is not selected
            // So receive all data and display
            // If the serial number is not set, this may be an old ensemble
            // Try to display it anyway
            if (!_Config.SubSystem.IsEmpty() && !ensEvent.Ensemble.EnsembleData.SysSerialNumber.IsEmpty())
            {
                // Verify the subsystem matches this viewmodel's subystem.
                if ((_Config.SubSystem != ensEvent.Ensemble.EnsembleData.GetSubSystem())        // Check if Subsystem matches 
                        || (_Config != ensEvent.Ensemble.EnsembleData.SubsystemConfig))         // Check if Subsystem Config matches
                {
                    return;
                }

                // Display the data
                //Task.Run(() => DisplayData(ensEvent.Ensemble));
                DisplayData(ensEvent.Ensemble);

            }
        }

        /// <summary>
        /// Receive event when a new project has been selected.
        /// Then clear all the data in the view.
        /// </summary>
        /// <param name="prjEvent">Project Event received.</param>
        public void Handle(ProjectEvent prjEvent)
        {
            // Get the new options for this project from the database
            GetOptionsFromDatabase();
        }

        #endregion
    }
}
