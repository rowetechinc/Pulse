/*
 * Copyright © 2011 
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
 * 09/01/2011      RC                     Initial coding
 * 10/06/2011      RC                     Default settings for theme
 * 10/18/2011      RC                     Added small plots for some ranges and
 *                                         list of previous ranges.
 * 10/19/2011      RC                     Observer removed, now data received through event.
 * 10/25/2011      RC                     Receive reference of current dataset from CurrentDataSetManager.
 *                                         Removed the list and update the series.
 * 11/10/2011      RC                     Added Display options for Measurement standard.
 *                                         Removed datagrids and created a bin list.
 * 11/16/2011      RC                     Use a background worker to update the display.
 * 11/29/2011      RC                     Update the lineseries and plot all in the same method using the dispatcher.
 * 12/06/2011      RC          1.08       Added Page_Load and Unload to stop updating display when data arrives if page is not viewed.
 * 12/07/2011      RC          1.08       Rewrote UpdateBinList to create a single string for each entry.   
 * 12/08/2011      RC          1.09       Changed name from STANDARD to IMPERIAL.
 * 12/12/2011      RC          1.09       Remove passing dataset as reference.
 * 12/15/2011      RC          1.09       Clear the text data in UpdateBinList() when data does not contain velocity data.
 * 12/19/2011      RC          1.10       Added GPS data to display.
 * 01/03/2012      RC          1.11       Set a FontSize property.  Allow changing in setting.  Changed spacing for amplitude data.
 * 01/26/2012      RC          1.14       Receive EventAggregator in constructor and use it to subscribe to receive ensembles.
 * 01/27/2012      RC          1.14       Went from using page load and unload to navigation aware interface.
 * 02/07/2012      RC          2.00       Get and Set the font size to the database.  
 *                                         Added logger to log errors.
 * 02/22/2012      RC          2.03       Moved the coordinate transform enum to RTI.Commons.cs
 * 02/23/2012      RC          2.03       Subscribe to receive Selected Project.
 *                                         Get options from the database and store to object.
 * 02/29/2012      RC          2.04       Clear the display when a new project is selected.
 * 03/05/2012      RC          2.05       Added Magnitude and Direction to the text output.
 * 03/06/2012      RC          2.05       Added SetDegreeValue() for degree values to be displayed.
 * 04/24/2012      RC          2.10       Check if bottom track data exist before displaying.
 * 09/12/2012      RC          2.15       In ReceiveCurrentDataSetFilter(), if the subsystem is set to empty, display all incoming data.
 * 09/18/2012      RC          2.15       Filter ensemble data to display based Subsystem and SubsystemConfiguration.
 * 11/26/2012      RC          2.16       In UpdateBinList(), changed Magnitude and Direction title to ENU Mag and ENU Direction.
 * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
 * 01/16/2013      RC          2.17       Changed _options from TextSubsystemOptions to TextSubsystemConfigOptions.
 * 01/23/2013      RC          2.17       In UpdateBinList(), check if the bin number is above the array size because a project was selected with number of bins different from live data.
 * 03/07/2013      RC          2.18       Check if the received ensemble is valid and can be displayed DisplayEnsembleEvent event filter.
 * 03/28/2013      RC          2.19       In ReceiveCurrentEnsembleFilter(), allow an ensemble to pass through if a serial number is not set.
 * 05/01/2013      RC          2.19       Add ability to handle single beam data. 
 * 08/12/2013      RC          3.0.7      Save options to the Project DB.
 * 08/27/2013      RC          3.0.8      Changed _subsystemConfig from SubsystemConfiguration to SubsystemDataConfig so the data source is known.
 * 08/29/2013      RC          3.0.8      Fixed bug in SaveOptionsToDatabase() where the SubsystemDataConfig was passed to AppConfiguration when it should be a SubsystemConfiguration.
 * 01/14/2014      RC          3.2.3      Maded DisplayData() work async.  The data is buffered when it is recieved so the event handler can return immediately.
 * 04/15/2014      RC          3.2.4      Allow display of Single beam ADCPs.      
 * 05/01/2014      RC          3.2.4      Check for any exceptions in UpdateBinList().
 * 05/07/2014      RC          3.2.4      In UpdateBinList(), display the data even when datasets are missing.
 * 06/27/2014      RC          3.4.0      Check for new sources of data from averaging.
 * 07/14/2014      RC          3.4.0      Moved TextEnsembleVM from the base view to each view.
 * 07/29/2014      RC          3.4.0      Added Water Track data.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 08/20/2014      RC          4.0.1      Added CloseVMCommand.
 * 11/13/2015      RC          4.3.1      Added RangeTracking info.
 * 11/16/2016      RC          4.3.1      Added a thread.
 * 
 */

using System;
using System.Windows.Media;
using System.ComponentModel;
using System.Text;
using Caliburn.Micro;
using ReactiveUI;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace RTI
{
    /// <summary>
    /// This will give the view data to display.  This will
    /// determine if the data that should be displayed from 
    /// a selected data set or the latest data set.  
    /// 
    /// Data is recevied from the ADCP by subscribing to
    /// CurrentDataSetManager.  Data will be received and
    /// stored as _latestDataSet.  
    /// 
    /// _selectedDataSet is a data set selected by the user.
    /// This data set is retrieve from the ProjectManager.
    /// The ProjectManager will query the ProjectManagerDatabaseWriter
    /// for the selected data.
    /// </summary>
    public class ViewDataTextViewModel : PulseViewModel, IHandle<EnsembleEvent>, IHandle<ProjectEvent>, IHandle<SelectedEnsembleEvent>
    {

        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Set if the page is being displayed.
        /// </summary>
        private bool _isDisplaying;
        /// <summary>
        /// Event Aggregator to receive the latest ensembles.
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Subsystem Configuration and source for this ViewModel.
        /// </summary>
        private SubsystemDataConfig _subsystemConfig;

        /// <summary>
        /// Pulse manager to manage the application.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Current number of bins in the latest dataset.
        /// </summary>
        private int _currentNumBins;

        /// <summary>
        /// All options for the view stored in
        /// an object.
        /// </summary>
        private TextSubsystemConfigOptions _options;

        /// <summary>
        /// Value used to convert meters to feet.
        /// </summary>
        private const float METERS_TO_FEET = 3.2808399f;

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

        /// <summary>
        /// Text Ensemble View Model.
        /// </summary>
        public ViewDataTextEnsembleViewModel TextEnsembleVM { get; set; }

        ///// <summary>
        ///// Limit how often the display will be updated.
        ///// </summary>
        ////private int _displayCounter;

        #region Colors

        /// <summary>
        /// Good Beam Color.
        /// </summary>
        private Color GOOD_BEAM_COLR = Colors.Green;

        /// <summary>
        /// Bad Beam color.
        /// </summary>
        private Color BAD_BEAM_COLOR = Colors.Red;

        #endregion

        #region BinDisplay

        /// <summary>
        /// All values needed to display 1 Bin of data.
        /// </summary>
        public class BinDisplay
        {
            /// <summary>
            /// Bin number.
            /// </summary>
            public int Bin { get; set; }

            /// <summary>
            /// Depth.
            /// </summary>
            public string Depth { get; set; }

            /// <summary>
            /// Beam 1 Velocity.
            /// </summary>
            public string VelBeam1 { get; set; }

            /// <summary>
            /// Beam 2 Velocity.
            /// </summary>
            public string VelBeam2 { get; set; }

            /// <summary>
            /// Beam 3 Velocity.
            /// </summary>
            public string VelBeam3 { get; set; }

            /// <summary>
            /// Beam 4 Velocity.
            /// </summary>
            public string VelBeam4 { get; set; }

            /// <summary>
            /// Beam 1 Good Ping.
            /// </summary>
            public string GoodPingBeam1 { get; set; }

            /// <summary>
            /// Beam 2 Good Ping.
            /// </summary>
            public string GoodPingBeam2 { get; set; }

            /// <summary>
            /// Beam 3 Good Ping.
            /// </summary>
            public string GoodPingBeam3 { get; set; }

            /// <summary>
            /// Beam 4 Good Ping.
            /// </summary>
            public string GoodPingBeam4 { get; set; }

            /// <summary>
            /// Beam 1 Good Ping Color.
            /// </summary>
            public Color GoodPingColorBeam1 { get; set; }

            /// <summary>
            /// Beam 2 Good Ping Color.
            /// </summary>
            public Color GoodPingColorBeam2 { get; set; }

            /// <summary>
            /// Beam 3 Good Ping Color.
            /// </summary>
            public Color GoodPingColorBeam3 { get; set; }

            /// <summary>
            /// Beam 4 Good Ping Color.
            /// </summary>
            public Color GoodPingColorBeam4 { get; set; }

            /// <summary>
            /// Beam 1 Amplitude.
            /// </summary>
            public string AmpBeam1 { get; set; }

            /// <summary>
            /// Beam 2 Amplitude.
            /// </summary>
            public string AmpBeam2 { get; set; }

            /// <summary>
            /// Beam 3 Amplitude.
            /// </summary>
            public string AmpBeam3 { get; set; }

            /// <summary>
            /// Beam 4 Amplitude.
            /// </summary>
            public string AmpBeam4 { get; set; }

            /// <summary>
            /// Beam 1 Correlation.
            /// </summary>
            public string CorrBeam1 { get; set; }

            /// <summary>
            /// Beam 2 Correaltion.
            /// </summary>
            public string CorrBeam2 { get; set; }

            /// <summary>
            /// Beam 3 Correlation.
            /// </summary>
            public string CorrBeam3 { get; set; }

            /// <summary>
            /// Beam 4 Correlation.
            /// </summary>
            public string CorrBeam4 { get; set; }

            /// <summary>
            /// Blank Distance.
            /// </summary>
            public string Blank { get { return ""; } }
        }

        #endregion

        #endregion

        #region Properties

        #region Display

        /// <summary>
        /// Display the CEPO index to describe this view model.
        /// </summary>
        public string Display
        {
            get { return _subsystemConfig.IndexCodeString(); }
        }

        /// <summary>
        /// Display the CEPO index to describe this view model.
        /// </summary>
        public string Title
        {
            get { return string.Format("[{0}]{1}", _subsystemConfig.CepoIndex.ToString(), _subsystemConfig.SubSystem.CodedDescString()); }
        }

        /// <summary>
        /// Ensemble source.
        /// </summary>
        public SubsystemDataConfig SubsystemConfig
        {
            get
            {
                return _subsystemConfig;
            }
        }

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

        /// <summary>
        /// Flag if this view will display playback or live data.
        /// TRUE = Playback Data
        /// </summary>
        public bool IsPlayback
        {
            get
            {
                if (_subsystemConfig.Source == EnsembleSource.Playback)
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
                if (_subsystemConfig.Source == EnsembleSource.Serial)
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
                if (_subsystemConfig.Source == EnsembleSource.LTA)
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
                if (_subsystemConfig.Source == EnsembleSource.STA)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion

        #region Subsystem

        /// <summary>
        /// Subsystem Index in the serial number.
        /// </summary>
        public string SubsystemIndex
        {
            get { return _subsystemConfig.SubSystem.Index.ToString(); }
            set
            {
                //_subsystemIndex = value;
                this.NotifyOfPropertyChange(() => this.SubsystemIndex);
            }
        }

        /// <summary>
        /// Subsystem Description.
        /// </summary>
        public string SubsystemDesc
        {
            get { return _subsystemConfig.SubSystem.DescString(); }
            set
            {
                //_subsystemDesc = value;
                this.NotifyOfPropertyChange(() => this.SubsystemDesc);
            }
        }

        /// <summary>
        /// Subsystem Code.
        /// </summary>
        public string SubsystemCode
        {
            get { return _subsystemConfig.SubSystem.CodeToString(); ; }
            set
            {
                //_subsystemCode = value;
                this.NotifyOfPropertyChange(() => this.SubsystemCode);
            }
        }

        #endregion

        #region Options

        /// <summary>
        /// Size of the font on the screen.
        /// </summary>
        public int FontSize
        {
            get { return _options.FontSize; }
            set
            {
                _options.FontSize = value;
                this.NotifyOfPropertyChange(() => this.FontSize);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

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

        /// <summary>
        /// Minimum Bin to display.
        /// </summary>
        private int _minBinDisplay;
        /// <summary>
        /// Minimum bin to display property.
        /// </summary>
        public int MinBinDisplay
        {
            get { return _minBinDisplay; }
            set
            {

                _minBinDisplay = SetMinBinDisplay(value);
                this.NotifyOfPropertyChange(() => this.MinBinDisplay);
            }
        }

        /// <summary>
        /// Maximum Bin to display
        /// </summary>
        private int _maxBinDisplay;
        /// <summary>
        /// Maximum bin to display property.
        /// </summary>
        public int MaxBinDisplay
        {
            get { return _maxBinDisplay; }
            set
            {
                _maxBinDisplay = SetMaxBinDisplay(value);
                this.NotifyOfPropertyChange(() => this.MaxBinDisplay);
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

        #endregion

        #region Range Tracking

        /// <summary>
        /// Range Track Instrument Number Beams.
        /// </summary>
        public string RtNumBeams
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    return _DisplayEnsemble.RangeTrackingData.NumBeams.ToString("0"); ;
                }
                else
                {
                    return "";
                }
            }
        }

        #region SNR

        /// <summary>
        /// Get the Range Tracking SNR for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtSnrB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        return _DisplayEnsemble.RangeTrackingData.SNR[0].ToString("0.00") + " dB";
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking SNR for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtSnrB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        return _DisplayEnsemble.RangeTrackingData.SNR[1].ToString("0.00") + " dB";
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking SNR for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtSnrB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        return _DisplayEnsemble.RangeTrackingData.SNR[2].ToString("0.00") + " dB";
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking SNR for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtSnrB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        return _DisplayEnsemble.RangeTrackingData.SNR[3].ToString("0.00") + " dB";
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Range

        /// <summary>
        /// Get the Range Tracking Range for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtRangeB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        return SetMeasurementValue(_DisplayEnsemble.RangeTrackingData.Range[0], "0.000");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Range for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtRangeB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        return SetMeasurementValue(_DisplayEnsemble.RangeTrackingData.Range[1], "0.000");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Range for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtRangeB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        return SetMeasurementValue(_DisplayEnsemble.RangeTrackingData.Range[2], "0.000");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Range for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtRangeB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        return SetMeasurementValue(_DisplayEnsemble.RangeTrackingData.Range[3], "0.000");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Number of Pings

        /// <summary>
        /// Get the Range Tracking Pings for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtPingsB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Pings[0].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Pings for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtPingsB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Pings[1].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Pings for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtPingsB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Pings[2].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Pings for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtPingsB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Pings[3].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Amplitude

        /// <summary>
        /// Get the Range Tracking Amplitude for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtAmplitudeB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Amplitude[0].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Amplitude for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtAmplitudeB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Amplitude[1].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Amplitude for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtAmplitudeB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Amplitude[2].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Amplitude for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtAmplitudeB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Amplitude[3].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Correlation

        /// <summary>
        /// Get the Range Tracking Correlation for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtCorrelationB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Correlation[0].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Correlation for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtCorrelationB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Correlation[1].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Correlation for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtCorrelationB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Correlation[2].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Correlation for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtCorrelationB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        return _DisplayEnsemble.RangeTrackingData.Correlation[3].ToString("0");
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Beam Velocity

        /// <summary>
        /// Get the Range Tracking Beam Velocity for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtBeamVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.BeamVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.BeamVelocity[0].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Beam Velocity for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtBeamVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.BeamVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.BeamVelocity[1].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Beam Velocity for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtBeamVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.BeamVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.BeamVelocity[2].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Beam Velocity for Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtBeamVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.BeamVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.BeamVelocity[3].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Instrument Velocity

        /// <summary>
        /// Get the Range Tracking Instrument Velocity for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtInstrVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.InstrumentVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.InstrumentVelocity[0].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Instrument Velocity for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtInstrVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.InstrumentVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.InstrumentVelocity[1].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Instrument Velocity for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtInstrVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.InstrumentVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.InstrumentVelocity[2].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Instrument Velocityfor Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtInstrVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.InstrumentVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.InstrumentVelocity[3].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #region Earth Velocity

        /// <summary>
        /// Get the Range Tracking Earth Velocity for Beam 0.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtEarthVelB0
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 1)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.EarthVelocity[0] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.EarthVelocity[0].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Earth Velocity for Beam 1.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtEarthVelB1
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 2)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.EarthVelocity[1] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.EarthVelocity[1].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Earth Velocity for Beam 2.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtEarthVelB2
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 3)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.EarthVelocity[2] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.EarthVelocity[2].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        /// <summary>
        /// Get the Range Tracking Earth Velocityfor Beam 3.
        /// Round to 2 decimal places.
        /// </summary>
        public string RtEarthVelB3
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsRangeTrackingAvail)
                {
                    if (_DisplayEnsemble.RangeTrackingData.NumBeams >= 4)
                    {
                        if (_DisplayEnsemble.RangeTrackingData.EarthVelocity[3] != DataSet.Ensemble.BAD_VELOCITY)
                        {
                            return _DisplayEnsemble.RangeTrackingData.EarthVelocity[3].ToString("0.00");
                        }
                        else
                        {
                            return "-";
                        }
                    }
                    else
                    {
                        return "-";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        #endregion

        #endregion

        #region Bin Properties

        /// <summary>
        /// Selection of what coordinate transform to display.
        /// </summary>
        public Core.Commons.Transforms SelectedTransform
        {
            get { return _options.Transform; }
            set
            {
                _options.Transform = value;
                this.NotifyOfPropertyChange(() => this.SelectedTransform);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// List of coordinate transforms.
        /// </summary>
        private BindingList<Core.Commons.Transforms> _transformList;
        /// <summary>
        /// List of coordinate transforms.
        /// </summary>
        public BindingList<Core.Commons.Transforms> TransformList
        {
            get { return _transformList; }
            set
            {
                _transformList = value;
                this.NotifyOfPropertyChange(() => this.TransformList);
            }
        }

        /// <summary>
        /// List of all the Bin data.
        /// </summary>
        private BindingList<string> _binList;
        /// <summary>
        /// List of all the Bin data property.
        /// </summary>
        public BindingList<string> BinList
        {
            get { return _binList; }
            set
            {
                _binList = value;
                this.NotifyOfPropertyChange(() => this.BinList);
            }
        }

        /// <summary>
        /// This is used to give a size to the list
        /// when displayed.  The font size and text
        /// dictates the size of the list displayed.
        /// </summary>
        private string _binListHeader;
        /// <summary>
        /// This is used to give a size to the list
        /// when displayed.  The font size and text
        /// dictates the size of the list displayed.
        /// </summary>
        public string BinListHeader
        {
            get { return _binListHeader; }
            set
            {
                _binListHeader = value;
                this.NotifyOfPropertyChange(() => this.BinListHeader);
            }
        }

        #endregion

        #endregion

        #region Command

        /// <summary>
        /// Command to set the default values.
        /// </summary>
        public ReactiveCommand<object> SetDefaultsCommand { get; protected set; }

        /// <summary>
        /// Command to close this VM.
        /// </summary>
        public ReactiveCommand<object> CloseVMCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// Register to start to receive live data
        /// from the CurrentDataSetManager.
        /// Start viewing live data.
        /// <param name="config">Configuration containing data source and SubsystemConfiguration.</param>
        /// </summary>
        public ViewDataTextViewModel(SubsystemDataConfig config)
            : base("Text")
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _subsystemConfig = config;
            _pm = IoC.Get<PulseManager>();
            _buffer = new ConcurrentQueue<DataSet.Ensemble>();
            //_displayCounter = 0;

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = string.Format("Text View: {0}", config.DescString());
            _processDataThread.Start();

            // Get the options from the database
            GetOptionsFromDatabase();

            // Create the Text view
            TextEnsembleVM = new ViewDataTextEnsembleViewModel(config);

            // Set if the page is being displayed
            _isDisplaying = true;

            _currentNumBins = 1;
            MinBinDisplay = 0;
            MaxBinDisplay = 1;

            // Subscribe to receive ensembles
            _eventAggregator.Subscribe(this);

            MeasurementStandardList = Core.Commons.GetMeasurementStandardList();
            TransformList = Core.Commons.GetTransformList();

            BinList = new BindingList<string>();
            BinListHeader = "";

            // Create a command to set default values
            SetDefaultsCommand = ReactiveCommand.Create();
            SetDefaultsCommand.Subscribe(_ => On_SetDefaults());

            // Close the VM
            CloseVMCommand = ReactiveCommand.Create();
            CloseVMCommand.Subscribe(_ => _eventAggregator.PublishOnUIThread(new CloseVmEvent(_subsystemConfig)));
        }

        /// <summary>
        /// Unsubscribe from the event aggregator when shutting down.
        /// </summary>
        public override void Dispose()
        {
            // Unsubscribe
            _eventAggregator.Unsubscribe(this);

            // Wake up the thread to process data
            _continue = false;
            _eventWaitData.Set();

            TextEnsembleVM.Dispose();
        }

        #region Display Data

        /// <summary>
        /// Buffer the incoming data so the event handler can return immediately.
        /// </summary>
        /// <param name="ensemble">Ensemble to display.</param>
        private void DisplayData(DataSet.Ensemble ensemble)
        {
            // Buffer the data
            _buffer.Enqueue(ensemble);

            // Limit how often the dsiplay updates
            //if ((++_displayCounter % 5) == 0)
            //{
                // Wake up the thread to process data
                _eventWaitData.Set();

                //_displayCounter = 0;
            //}
        }

        /// <summary>
        /// Display the data async.
        /// </summary>
        private void ProcessDataThread()
        {
            while (_continue)
            {
                // Wakeup the thread with a signal
                // Have a 2 second timeout to see if we need to shutdown the thread
                _eventWaitData.WaitOne(2000);

                // Continue processing until all the data has been displayed
                while (_buffer.Count > 0)
                {

                    // Get the data from the buffer
                    DataSet.Ensemble ensemble = null;
                    if (_buffer.TryDequeue(out ensemble))
                    {

                        // Update data if the page is being displayed
                        if (_isDisplaying)
                        {
                            // Set the latest data
                            DisplayEnsemble = ensemble;

                            // Check if the dataset changed
                            if (ensemble.EnsembleData.NumBins != _currentNumBins)
                            {
                                _currentNumBins = ensemble.EnsembleData.NumBins;
                                ResetMinMaxBinDisplay();
                            }

                            // Update the Bin List
                            UpdateBinList(ensemble);

                            // Report that the latest changed
                            UpdateDisplay();

                            // Add the data to the Text Display
                            TextEnsembleVM.ReceiveEnsemble(ensemble);
                        }
                    }
                }
            }
        }


        #endregion

        #region Update Display

        /// <summary>
        /// When a new dataset is received,
        /// notify that all these properties have
        /// changed.
        /// </summary>
        private void UpdateDisplay()
        {
            //#region BT

            //this.NotifyOfPropertyChange(() => this.BtRangeB0);
            //this.NotifyOfPropertyChange(() => this.BtRangeB1);
            //this.NotifyOfPropertyChange(() => this.BtRangeB2);
            //this.NotifyOfPropertyChange(() => this.BtRangeB3);

            //this.NotifyOfPropertyChange(() => this.BtBeamVelB0);
            //this.NotifyOfPropertyChange(() => this.BtBeamVelB1);
            //this.NotifyOfPropertyChange(() => this.BtBeamVelB2);
            //this.NotifyOfPropertyChange(() => this.BtBeamVelB3);

            //this.NotifyOfPropertyChange(() => this.BtGoodBeamB0);
            //this.NotifyOfPropertyChange(() => this.BtGoodBeamB1);
            //this.NotifyOfPropertyChange(() => this.BtGoodBeamB2);
            //this.NotifyOfPropertyChange(() => this.BtGoodBeamB3);

            //this.NotifyOfPropertyChange(() => this.BtInstrVelB0);
            //this.NotifyOfPropertyChange(() => this.BtInstrVelB1);
            //this.NotifyOfPropertyChange(() => this.BtInstrVelB2);
            //this.NotifyOfPropertyChange(() => this.BtInstrVelB3);

            //this.NotifyOfPropertyChange(() => this.BtGoodInstrB0);
            //this.NotifyOfPropertyChange(() => this.BtGoodInstrB1);
            //this.NotifyOfPropertyChange(() => this.BtGoodInstrB2);
            //this.NotifyOfPropertyChange(() => this.BtGoodInstrB3);

            //this.NotifyOfPropertyChange(() => this.BtEarthVelB0);
            //this.NotifyOfPropertyChange(() => this.BtEarthVelB1);
            //this.NotifyOfPropertyChange(() => this.BtEarthVelB2);
            //this.NotifyOfPropertyChange(() => this.BtEarthVelB3);

            //this.NotifyOfPropertyChange(() => this.BtGoodEarthB0);
            //this.NotifyOfPropertyChange(() => this.BtGoodEarthB1);
            //this.NotifyOfPropertyChange(() => this.BtGoodEarthB2);
            //this.NotifyOfPropertyChange(() => this.BtGoodEarthB3);

            //this.NotifyOfPropertyChange(() => this.BtSnrB0);
            //this.NotifyOfPropertyChange(() => this.BtSnrB1);
            //this.NotifyOfPropertyChange(() => this.BtSnrB2);
            //this.NotifyOfPropertyChange(() => this.BtSnrB3);

            //this.NotifyOfPropertyChange(() => this.BtAmpB0);
            //this.NotifyOfPropertyChange(() => this.BtAmpB1);
            //this.NotifyOfPropertyChange(() => this.BtAmpB2);
            //this.NotifyOfPropertyChange(() => this.BtAmpB3);

            //this.NotifyOfPropertyChange(() => this.BtCorrB0);
            //this.NotifyOfPropertyChange(() => this.BtCorrB1);
            //this.NotifyOfPropertyChange(() => this.BtCorrB2);
            //this.NotifyOfPropertyChange(() => this.BtCorrB3);

            //#endregion

            //this.NotifyOfPropertyChange(() => this.WtEarthDepthLayer);
            //this.NotifyOfPropertyChange(() => this.WtEarthVelB0);
            //this.NotifyOfPropertyChange(() => this.WtEarthVelB1);
            //this.NotifyOfPropertyChange(() => this.WtEarthVelB2);

            //this.NotifyOfPropertyChange(() => this.WtInstrumentDepthLayer);
            //this.NotifyOfPropertyChange(() => this.WtInstrVelB0);
            //this.NotifyOfPropertyChange(() => this.WtInstrVelB1);
            //this.NotifyOfPropertyChange(() => this.WtInstrVelB2);
            //this.NotifyOfPropertyChange(() => this.WtInstrVelB3);

            // Update all the displays
            this.NotifyOfPropertyChange(null);
        }

        /// <summary>
        /// Clear the display of all data.
        /// This is used when a new project is selected
        /// and the old data needs to be wiped from
        /// the screen.
        /// </summary>
        private void ClearDisplay()
        {
            // Clear the data
            _DisplayEnsemble = null;
            UpdateDisplay();

            // Clear the list of bin data
            BinList = null;
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

        /// <summary>
        /// Set the degree symbol at the end of the string.
        /// If the value is bad, put the place holder for a
        /// bad value.
        /// </summary>
        /// <param name="value">Value to create.</param>
        /// <param name="converter">Decimal places for the value.</param>
        /// <returns>String of the degree with the given decimal places and degree symbol.</returns>
        private string SetDegreeValue(double value, string converter)
        {
            // Check for a bad value
            if (value == DataSet.Ensemble.BAD_VELOCITY)
            {
                return "  -   ";
                //return DataSet.BeamVelocityDataSet.BAD_VELOCITY_PLACEHOLDER;
            }

            return string.Format("{0}°", value.ToString(converter));
        }

        /// <summary>
        /// Determine which label to add to the end of a value
        /// based off which measurement standard is selected.
        /// </summary>
        /// <returns></returns>
        private string MeasurementLabel()
        {
            switch (MeasurementStandard)
            {
                case Core.Commons.MeasurementStandards.METRIC:
                    return "m";
                case Core.Commons.MeasurementStandards.IMPERIAL:
                    return "ft";
            }

            return "m";
        }

        #endregion

        #region Options

        /// <summary>
        /// Get the options for this subsystem display
        /// from the database.  If the options have not
        /// been set to the database yet, default values 
        /// will be used.
        /// </summary>
        private void GetOptionsFromDatabase()
        {
            _options = _pm.AppConfiguration.GetTextOptions(_subsystemConfig);

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
            this.NotifyOfPropertyChange(() => this.MeasurementStandardList);
            this.NotifyOfPropertyChange(() => this.MeasurementStandard);
            this.NotifyOfPropertyChange(() => this.FontSize);
            this.NotifyOfPropertyChange(() => this.TransformList);
            this.NotifyOfPropertyChange(() => this.SelectedTransform);
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
            var ssConfig = new SubsystemConfiguration(_subsystemConfig.SubSystem, _subsystemConfig.CepoIndex, _subsystemConfig.SubsystemConfigIndex);

            _pm.AppConfiguration.SaveTextOptions(ssConfig, _options);
        }

        #endregion

        #region Min Max Bin Display

        /// <summary>
        /// Set the Minimum display bin value.  This
        /// is based off the Maximum value and the current
        /// dataset.
        /// </summary>
        /// <param name="value">Value to assign to min.</param>
        /// <returns>Value to assign to min.</returns>
        private int SetMinBinDisplay(int value)
        {
            // Ensure not greater than max
            if (value >= MaxBinDisplay)
            {
                value = MaxBinDisplay;
            }

            // Ensure within range of dataset
            if (value >= _currentNumBins)
            {
                value = 0;
            }

            // Ensure not less then 0
            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// Set the Maximum display bin value.  This
        /// is based off the Minimum value and the current
        /// dataset.
        /// </summary>
        /// <param name="value">Value to assign to max.</param>
        /// <returns>Value to assign to max.</returns>
        private int SetMaxBinDisplay(int value)
        {
            // Ensure not less than min
            if (value <= MinBinDisplay)
            {
                value = MinBinDisplay + 1;
            }

            // Ensure within range of dataset
            if (value >= _currentNumBins)
            {
                value = _currentNumBins - 1;
            }

            // Ensure not less then 0
            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// Reset the Minimum and Maximum display bin.
        /// </summary>
        private void ResetMinMaxBinDisplay()
        {
            MinBinDisplay = 0;
            MaxBinDisplay = _currentNumBins;
        }

        #endregion

        #region Update Bin List

        /// <summary>
        /// Create a list of all the values for each bin.  This includes
        /// velocity, good ping, amplitude and correlation.  A single velocity
        /// value is used and a single good ping value is used.  The user
        /// can choose which values to use; beam, earth or instrument.
        /// </summary>
        /// <param name="adcpData">DataSet to get data.</param>
        private void UpdateBinList(DataSet.Ensemble adcpData)
        {
            try
            {
                BindingList<string> list = new BindingList<string>();
                list.AllowEdit = false;
                list.AllowNew = false;
                list.AllowRemove = false;

                // Determine spacing of the items
                int DepthLabelPad = 10;
                int VelLabelPad = 26;
                int GPLabelPad = 21;
                int AmpLabelPad = 21;
                int CorrLabelPad = 30;
                int MagLabelPad = 19;
                int DirLabelPad = 10;
                int DepthPad = 10;
                int VelPad = 7;
                if (MeasurementStandard == Core.Commons.MeasurementStandards.IMPERIAL)
                {
                    DepthLabelPad = 9;
                    VelLabelPad = 29;
                    GPLabelPad = 24;
                    VelPad = 8;
                }

                // Add Top Label
                StringBuilder label = new StringBuilder();
                label.Append(("Bin").PadLeft(4) + "");
                label.Append(("Depth").PadLeft(DepthLabelPad) + "");
                label.Append((GetVelocityTitle()).PadLeft(VelLabelPad) + "");
                label.Append(("Good Ping").PadLeft(GPLabelPad) + "");
                label.Append(("Amplitude").PadLeft(AmpLabelPad) + "");
                label.Append(("Correlation").PadLeft(CorrLabelPad) + "");
                label.Append(("ENU Mag").PadLeft(MagLabelPad) + "");
                label.Append(("ENU Dir").PadLeft(DirLabelPad));
                //list.Add(label.ToString());
                BinListHeader = label.ToString();

                // Verify the minimum data is available
                if (adcpData == null || !adcpData.IsAncillaryAvail || !adcpData.IsEnsembleAvail)
                {
                    // Set at least the titles
                    BinList = list;

                    return;
                }

                // Check for Vertical beams to set the SelectedTransform
                CheckForVerticalBeam(adcpData);

                // Set the number of bins and the Bin size
                int numBins = adcpData.EnsembleData.NumBins;
                double binSize = adcpData.AncillaryData.BinSize;
                double firstBinDepth = adcpData.AncillaryData.FirstBinRange;

                // Set the Velocity data to the list
                float[,] velData = null;
                int[,] goodPingData = null;
                switch (SelectedTransform)
                {
                    case Core.Commons.Transforms.BEAM:
                        velData = SetBeamVelocityBinData(adcpData);
                        goodPingData = SetGoodBeamBinData(adcpData);
                        break;
                    case Core.Commons.Transforms.EARTH:
                        velData = SetEarthVelocityBinData(adcpData);
                        goodPingData = SetGoodEarthBinData(adcpData);
                        break;
                    case Core.Commons.Transforms.INSTRUMENT:
                        velData = SetInstrVelocityBinData(adcpData);
                        goodPingData = SetGoodBeamBinData(adcpData);
                        break;
                    default:
                        break;
                }

                // Verify we have good velocity data
                //if (velData == null)
                //{
                //    // Set at least the titles
                //    BinList = list;

                //    return;
                //}

                // Get the Amplitude data
                float[,] ampData = SetAmplitudeBinData(adcpData);
                //if (ampData == null)
                //{
                //    // Set at least the titles
                //    BinList = list;

                //    return;
                //}

                // Get the Correlation data
                float[,] corrData = SetCorrelationBinData(adcpData);
                //if (corrData == null)
                //{
                //    // Set at least the titles
                //    BinList = list;

                //    return;
                //}

                // Combine all the data and add to binding list
                for (int bin = MinBinDisplay; bin <= MaxBinDisplay; bin++)
                {
                    StringBuilder binData = new StringBuilder();
                    binData.Append((bin.ToString()).PadLeft(3) + "  ");

                    binData.Append((SetMeasurementValue((float)((bin * binSize) + firstBinDepth), "0.000") + MeasurementLabel()).PadLeft(DepthPad) + "    ");

                    //----------------------------------------------------------------------
                    // Velocity Beam 0
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_0_INDEX && velData != null)
                    {
                        binData.Append((SetMeasurementValue(velData[bin, DataSet.Ensemble.BEAM_0_INDEX], "0.000")).PadLeft(VelPad) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((VelPad)) + " "));
                    }

                    // Velocity Beam 1
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_1_INDEX && velData != null)
                    {
                        binData.Append(
                            (SetMeasurementValue(velData[bin, DataSet.Ensemble.BEAM_1_INDEX], "0.000")).PadLeft(VelPad) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((VelPad)) + " "));
                    }

                    // Velocity Beam 2
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_2_INDEX && velData != null)
                    {
                        binData.Append((SetMeasurementValue(velData[bin, DataSet.Ensemble.BEAM_2_INDEX], "0.000")).PadLeft(VelPad) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((VelPad)) + " "));
                    }

                    // Velocity Beam 3
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_3_INDEX && velData != null)
                    {
                        binData.Append((SetMeasurementValue(velData[bin, DataSet.Ensemble.BEAM_3_INDEX], "0.000")).PadLeft(VelPad) + "   ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((VelPad)) + "   "));
                    }

                    //----------------------------------------------------------------------
                    // Good Ping Beam 0
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_0_INDEX && goodPingData != null)
                    {
                        binData.Append(((goodPingData[bin, DataSet.Ensemble.BEAM_0_INDEX]).ToString()).PadLeft(1) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((1)) + " "));
                    }

                    // Good Ping Beam 1
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_1_INDEX && goodPingData != null)
                    {
                        binData.Append(((goodPingData[bin, DataSet.Ensemble.BEAM_1_INDEX]).ToString()).PadLeft(1) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((1)) + " "));
                    }

                    // Good Ping Beam 2
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_2_INDEX && goodPingData != null)
                    {
                        binData.Append(((goodPingData[bin, DataSet.Ensemble.BEAM_2_INDEX]).ToString()).PadLeft(1) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((1)) + " "));
                    }

                    // Good Ping Beam 3
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_3_INDEX && goodPingData != null)
                    {
                        binData.Append(((goodPingData[bin, DataSet.Ensemble.BEAM_3_INDEX]).ToString()).PadLeft(1) + "   ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((1)) + "   "));
                    }

                    //----------------------------------------------------------------------
                    // Amplitude Beam 0
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_0_INDEX && ampData != null)
                    {
                        binData.Append(((ampData[bin, DataSet.Ensemble.BEAM_0_INDEX]).ToString("0.0")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Amplitude Beam 1
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_1_INDEX && ampData != null)
                    {
                        binData.Append(((ampData[bin, DataSet.Ensemble.BEAM_1_INDEX]).ToString("0.0")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Amplitude Beam 2
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_2_INDEX && ampData != null)
                    {
                        binData.Append(((ampData[bin, DataSet.Ensemble.BEAM_2_INDEX]).ToString("0.0")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Amplitude Beam 3
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_3_INDEX && ampData != null)
                    {
                        binData.Append(((ampData[bin, DataSet.Ensemble.BEAM_3_INDEX]).ToString("0.0")).PadLeft(6) + "   ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + "   "));
                    }

                    //----------------------------------------------------------------------
                    // Correlation Beam 0
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_0_INDEX && corrData != null)
                    {
                        binData.Append(((corrData[bin, DataSet.Ensemble.BEAM_0_INDEX]).ToString("0.000")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Correlation Beam 1
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_1_INDEX && corrData != null)
                    {
                        binData.Append(((corrData[bin, DataSet.Ensemble.BEAM_1_INDEX]).ToString("0.000")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Correlation Beam 2
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_2_INDEX && corrData != null)
                    {
                        binData.Append(((corrData[bin, DataSet.Ensemble.BEAM_2_INDEX]).ToString("0.000")).PadLeft(6) + " ");
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6)) + " "));
                    }

                    // Correlation Beam 3
                    if (adcpData.EnsembleData.NumBeams > DataSet.Ensemble.BEAM_3_INDEX && corrData != null)
                    {
                        binData.Append(((corrData[bin, DataSet.Ensemble.BEAM_3_INDEX]).ToString("0.000")).PadLeft(6));
                    }
                    else
                    {
                        binData.Append(("-".PadLeft((6))));
                    }

                    if (adcpData.IsEarthVelocityAvail && adcpData.EarthVelocityData.IsVelocityVectorAvail)
                    {
                        binData.Append(SetMeasurementValue((float)adcpData.EarthVelocityData.VelocityVectors[bin].Magnitude, "0.00").PadLeft(9));
                        binData.Append(SetDegreeValue(adcpData.EarthVelocityData.VelocityVectors[bin].DirectionXNorth, "0.00").PadLeft(12));
                    }

                    // Add the string to the list
                    list.Add(binData.ToString());
                }

                // Set the new list
                BinList = list;
            }
            catch (System.IndexOutOfRangeException ex)
            {
                // This check is done if we are switching between live and playback data and
                // number of bins are not the same
                log.Error("Index for the number of bins error", ex);
            }
            catch (Exception e)
            {
                log.Error("Error Displaying the data.", e);
            }
        }

        /// <summary>
        /// If the ensemble is for a Vertical Beam,
        /// it can only have Beam data.  So set the selected
        /// transform to beam data.
        /// </summary>
        /// <param name="ensemble">Ensemble to get the subsystem type.</param>
        private void CheckForVerticalBeam(DataSet.Ensemble ensemble)
        {
            if(ensemble.IsEnsembleAvail)
            {
                // Check for vertical beam
                switch(ensemble.EnsembleData.SubsystemConfig.SubSystem.Code)
                {
                    case Subsystem.SUB_2MHZ_VERT_PISTON_9:
                    case Subsystem.SUB_1_2MHZ_VERT_PISTON_A:
                    case Subsystem.SUB_600KHZ_VERT_PISTON_B:
                    case Subsystem.SUB_300KHZ_VERT_PISTON_C:
                    case Subsystem.SUB_150KHZ_VERT_PISTON_D:
                    case Subsystem.SUB_75KHZ_VERT_PISTON_E:
                    case Subsystem.SUB_38KHZ_VERT_PISTON_F:
                    case Subsystem.SUB_20KHZ_VERT_PISTON_G:
                        // Set the selected transform
                        SelectedTransform = Core.Commons.Transforms.BEAM;
                        break;
                    default:
                        break;
                }

            }

        }

        /// <summary>
        /// Determine what the title should be for velocity.
        /// This wills state which transform is selected.
        /// </summary>
        /// <returns>Velocity label based off transform selected.</returns>
        private string GetVelocityTitle()
        {
            if (SelectedTransform == Core.Commons.Transforms.BEAM)
            {
                return "BEAM VELOCITY";
            }

            if (SelectedTransform == Core.Commons.Transforms.EARTH)
            {
                return "EARTH VELOCITY";
            }

            if (SelectedTransform == Core.Commons.Transforms.INSTRUMENT)
            {
                return "INSTRUMENT VELOCITY";
            }

            return "VELOCITY";
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private float[,] SetBeamVelocityBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsBeamVelocityAvail)
            {
                return adcpData.BeamVelocityData.BeamVelocityData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private float[,] SetEarthVelocityBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsEarthVelocityAvail)
            {
                return adcpData.EarthVelocityData.EarthVelocityData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private float[,] SetInstrVelocityBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsInstrumentVelocityAvail)
            {
                return adcpData.InstrumentVelocityData.InstrumentVelocityData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private int[,] SetGoodBeamBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsGoodBeamAvail)
            {
                return adcpData.GoodBeamData.GoodBeamData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private int[,] SetGoodEarthBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsGoodEarthAvail)
            {
                return adcpData.GoodEarthData.GoodEarthData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private float[,] SetAmplitudeBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsAmplitudeAvail)
            {
                return adcpData.AmplitudeData.AmplitudeData;
            }

            return null;
        }

        /// <summary>
        /// Verify that the dataset type exist.  If it does,
        /// return its data.  If it does not exist, return null.
        /// </summary>
        /// <param name="adcpData">Dataset containing the data.</param>
        /// <returns>Array containing the velocity data.  Null if the dataset type does not exist.</returns>
        private float[,] SetCorrelationBinData(DataSet.Ensemble adcpData)
        {
            if (adcpData.IsCorrelationAvail)
            {
                return adcpData.CorrelationData.CorrelationData;
            }

            return null;
        }


        #endregion

        #region Event Handler

        /// <summary>
        /// Check if the dataset's subsystem matches this
        /// viewmodel's subystem.
        /// </summary>
        /// <param name="ensemble"></param>
        /// <returns></returns>
        public bool ReceiveCurrentEnsembleFilter(DataSet.Ensemble ensemble)
        {
            // Check if the ensemble is good
            if (ensemble == null || ensemble.EnsembleData == null || !ensemble.IsEnsembleAvail)
            {
                return false;
            }

            // If no subsystem is given, then a project is not selected
            // So receive all data and display
            //if (_subsystem == Subsystem.Empty && _subsystemConfig == SubsystemConfiguration.Empty)
            if (_subsystemConfig.SubSystem.IsEmpty())
            {
                return true;
            }

            // If the serial number is not set, this may be an old ensemble
            // Try to display it anyway
            if (ensemble.EnsembleData.SysSerialNumber.IsEmpty())
            {
                return true;
            }

            // Verify the subsystem matches this viewmodel's subystem.
            return (_subsystemConfig.SubSystem == ensemble.EnsembleData.GetSubSystem()) && (_subsystemConfig == ensemble.EnsembleData.SubsystemConfig);
        }

        /// <summary>
        /// Receive a new dataset.  Display the data.  This will
        /// update the plots and the text data.
        /// </summary>
        /// <param name="ensEvent">Latest ensemble event to display.</param>
        public void Handle(EnsembleEvent ensEvent)
        {
            // Check if the ensemble is good
            if (ensEvent.Ensemble == null || ensEvent.Ensemble.EnsembleData == null || !ensEvent.Ensemble.IsEnsembleAvail)
            {
                return;
            }

            // If no subsystem is given, then a project is not selected
            // So receive all data and display
            // If the serial number is not set, this may be an old ensemble
            // Try to display it anyway
            if (!_subsystemConfig.SubSystem.IsEmpty() && !ensEvent.Ensemble.EnsembleData.SysSerialNumber.IsEmpty())
            {
                // Verify the subsystem matches this viewmodel's subystem.
                // Verify the subsystem matches this viewmodel's subystem.
                if ((_subsystemConfig.SubSystem != ensEvent.Ensemble.EnsembleData.GetSubSystem())        // Check if Subsystem matches 
                        || (_subsystemConfig != ensEvent.Ensemble.EnsembleData.SubsystemConfig)          // Check if Subsystem Config matches
                        || _subsystemConfig.Source != ensEvent.Source)                                   // Check if source matches
                {
                    return;
                }
            }

            // Display the data
            DisplayData(ensEvent.Ensemble);
        }

        /// <summary>
        /// Receive a selected ensemble.  Display the data.  This will
        /// update the plots and the text data.
        /// </summary>
        /// <param name="ensEvent">Selected ensemble event to display.</param>
        public void Handle(SelectedEnsembleEvent ensEvent)
        {
            // Check if the ensemble is good
            if (ensEvent.Ensemble == null || ensEvent.Ensemble.EnsembleData == null || !ensEvent.Ensemble.IsEnsembleAvail)
            {
                return;
            }

            // If no subsystem is given, then a project is not selected
            // So receive all data and display
            // If the serial number is not set, this may be an old ensemble
            // Try to display it anyway
            if (!_subsystemConfig.SubSystem.IsEmpty() && !ensEvent.Ensemble.EnsembleData.SysSerialNumber.IsEmpty())
            {
                // Verify the subsystem matches this viewmodel's subystem.
                // Verify the subsystem matches this viewmodel's subystem.
                if ((_subsystemConfig.SubSystem != ensEvent.Ensemble.EnsembleData.GetSubSystem())        // Check if Subsystem matches 
                        || (_subsystemConfig != ensEvent.Ensemble.EnsembleData.SubsystemConfig)          // Check if Subsystem Config matches
                        || _subsystemConfig.Source != ensEvent.Source)                                   // Check if source matches
                {
                    return;
                }
            }

            // Display the data
            DisplayData(ensEvent.Ensemble);
        }

        /// <summary>
        /// Receive event when a new project has been selected.
        /// Then clear all the data in the view.
        /// </summary>
        /// <param name="prjEvent">Project Event received.</param>
        public void Handle(ProjectEvent prjEvent)
        {
            // Clear the display
            ClearDisplay();

            // Get the new options for this project from the database
            GetOptionsFromDatabase();
        }

        #endregion

        #region Commands

        #region Set Defaults Command

        /// <summary>
        /// Set the default ranges based off the 
        /// frequency.
        /// </summary>
        private void On_SetDefaults()
        {
            _options.SetDefaults();

            // Update the database
            UpdateDatabaseOptions();

            // Notify all the properties
            NotifyOptionPropertyChange();
        }

        #endregion

        #endregion
    }
}