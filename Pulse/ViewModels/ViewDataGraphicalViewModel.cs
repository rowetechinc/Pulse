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
 * 06/17/2013      RC          3.0.1      Initial coding
 * 06/26/2013      RC          3.0.2      Use EventAggregator to receive the ensembles.
 * 08/12/2013      RC          3.0.7      Handle ProjectEvent.  Save options to the Project DB.
 * 08/13/2013      RC          3.0.7      Pass ensemble to ContourPlot instead of VelocityVectors.
 * 09/03/2013      RC          3.0.9      Check if ensemble numbers are jumping around in CheckData() to clear the plots.
 * 09/30/2013      RC          3.1.5      Changed VelPlot to a BinPlot3D.
 * 12/06/2013      RC          3.2.0      Handle SelectedEnsembleEvent.
 * 01/14/2014      RC          3.2.3      Made DisplayData() and DisplayBulkData() work async.  The data is buffered when it is recieved so the event handler can return immediately.
 * 01/16/2014      RC          3.2.3      Removed the plot color properties.  Convert the option's colors from strings to OxyColor.
 * 05/07/2014      RC          3.2.4      In CheckData(), fixed checking if the ensembles are out of sync when playing back.
 * 06/27/2014      RC          3.4.0      Check for new sources of data from averaging.
 * 07/14/2014      RC          3.4.0      Moved TextEnsembleVM from the base view to each view.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 08/18/2014      RC          4.0.0      Changed the returns to continue in DisplayDataExecute().
 * 08/20/2014      RC          4.0.1      Removed AdcpPlotEvent.  Added CloseVMCommand.
 * 01/19/2015      RC          4.1.0      Added plot types to Contour plot.
 * 01/20/2015      RC          4.1.0      Display the selected ensemble in the text area.
 * 03/23/2015      RC          4.1.2      Added TimeSeriesOptions.
 * 08/13/2015      RC          4.1.5      Fixed bug with CheckData() not checking for bad ensembles.
 * 11/16/2016      RC          4.3.1      Added a thread.
 * 08/02/2016      RC          4.4.12     Removed contour plot and replaced with heatmap plot.
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
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// View the data graphically.  This will create all the
    /// objects to view the data graphically.  This includes
    /// plots, time series and contour plots.
    /// </summary>
    public class ViewDataGraphicalViewModel : DisplayViewModel, IHandle<ProjectEvent>, IHandle<SelectedEnsembleEvent>
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
        /// Used to determine if 
        /// any settings have changed.
        /// This monitors the previous size
        /// of a bin.  For the bin size to change,
        /// the ADCP must stop pinging and the setting
        /// changed.
        /// </summary>
        private int _prevBinSize;

        /// <summary>
        /// Used to determine if any settings
        /// have changed.  This monitors the 
        /// previous number of bins.  For the
        /// number of bins to change, the ADCP must
        /// stop pinging and the setting changed.
        /// </summary>
        private int _prevNumBins;

        /// <summary>
        /// Previous Ensemble to keep track if the user is jumping around.
        /// If the user is jumping around, clear the plots.
        /// </summary>
        private int _prevEnsNum;

        /// <summary>
        /// Store a list of the last MAX_DATASET vectors
        /// representing velocites for each bin.
        /// </summary>
        private LimitedList<DataSet.Ensemble> _ensembleHistory;

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
        ///// Limit how many times the display is refreshed.
        ///// </summary>
        ////private int _displayCounter;

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

        #region Plots

        #region Profile Plots

        /// <summary>
        /// Correlation plot.
        /// </summary>
        public ProfilePlotViewModel CorrPlot { get; set; }

        /// <summary>
        /// Amplitude plot.
        /// </summary>
        public ProfilePlotViewModel AmpPlot { get; set; }

        /// <summary>
        /// Velocity Profile plot.
        /// </summary>
        public ProfilePlotViewModel VelProfilePlot { get; set; }

        /// <summary>
        /// Velocity plot.
        /// </summary>
        //public ProfilePlotViewModel VelPlot { get; set; }
        public BinPlot3D VelPlot { get; set; }

        /// <summary>
        /// Display the 3D Velocity display.
        /// Set this to true will display the 3D plot.
        /// </summary>
        public bool IsDisplay3DVelocity
        {
            get { return _options.IsDisplay3DVelocity; }
            set
            {
                _options.IsDisplay3DVelocity = value;
                this.NotifyOfPropertyChange(() => this.IsDisplay3DVelocity);
                this.NotifyOfPropertyChange(() => this.IsDisplay2DVelocity);

                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Display the 2D Velocity display.
        /// Set this to true will display the 2D plot.
        /// </summary>
        public bool IsDisplay2DVelocity
        {
            get { return !_options.IsDisplay3DVelocity; }
            set
            {
                _options.IsDisplay3DVelocity = !value;
                this.NotifyOfPropertyChange(() => this.IsDisplay3DVelocity);
                this.NotifyOfPropertyChange(() => this.IsDisplay2DVelocity);

                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Contour Plot

        /// <summary>
        /// Colormap brush chosen.
        /// </summary>
        public ColormapBrush.ColormapBrushEnum ColormapBrushSelection
        {
            get { return _options.PlotColorMap; }
            set
            {
                _options.PlotColorMap = value;
                VelPlot.ColormapBrushSelection = value;

                this.NotifyOfPropertyChange(() => this.ColormapBrushSelection);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// List of all the possible colors for the velocity plots.
        /// </summary>
        public List<ColormapBrush.ColormapBrushEnum> ColormapList
        {
            get
            {
                return ColormapBrush.GetColormapList();
            }
        }

        /// <summary>
        /// Min velocity displayed in the Velocity ListBox.
        /// </summary>
        public double ContourMinValue
        {
            get { return _options.ContourMinimumValue; }
            set
            {
                _options.ContourMinimumValue = value;
                VelPlot.MinVelocity = value;

                this.NotifyOfPropertyChange(() => this.ContourMinValue);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

        /// <summary>
        /// Max velocity displayed in the Velocity ListBox.
        /// </summary>
        public double ContourMaxValue
        {
            get { return _options.ContourMaximumValue; }
            set
            {
                _options.ContourMaximumValue = value;
                VelPlot.MaxVelocity = value;

                this.NotifyOfPropertyChange(() => this.ContourMaxValue);

                // Update the database
                UpdateDatabaseOptions();
            }
        }

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
                _ensembleHistory.Limit = _options.DisplayMaxEnsembles;

                // Update the database with the latest options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Heatmap Plot

        /// <summary>
        /// Heatmap Plot View Model for Magnitude Velocity.
        /// </summary>
        public HeatmapPlotViewModel HeatmapPlot { get; set; }

        #endregion

        #region Selected Plot

        /// <summary>
        /// Selected bin and ensemble from the VelocityPlot.
        /// When the user clicks on the VelocityPlot,
        /// the Selected Bin and Selected Ensemble
        /// will update the BinSeries and EnsembleSeries.
        /// </summary>
        private ContourPlotMouseSelection _velocityPlotSelection;
        /// <summary>
        /// Selected bin and ensemble from the VelocityPlot.
        /// When the user clicks on the VelocityPlot,
        /// the Selected Bin and Selected Ensemble
        /// will update the BinSeries and EnsembleSeries.
        /// </summary>
        public ContourPlotMouseSelection VelocityPlotSelection
        {
            get { return _velocityPlotSelection; }
            set
            {
                _velocityPlotSelection = value;
                this.NotifyOfPropertyChange(() => this.VelocityPlotSelection);

                if (VelocityPlotSelection != null)
                {
                    //_selectedEnsemblePlot.SelectedBin = _velocityPlotSelection.BinNumber;
                    //_selectedEnsemblePlot.AddIncomingData(_ensembleVectors[_velocityPlotSelection.Index]);

                    //// Create an array with only the selected bins as the magnitude and direction
                    //// Then plot the data
                    //_binHistoryPlot.AddIncomingData(GenerateBinHistoryVectors());
                }
            }
        }

        #endregion

        #region Time Series Plot

        /// <summary>
        /// Default Earth Velocity plot.
        /// </summary>
        public TimeSeriesPlotViewModel TimeSeries1Plot { get; set; }

        /// <summary>
        /// Default Earth Velocity plot.
        /// </summary>
        public TimeSeriesPlotViewModel TimeSeries2Plot { get; set; }

        /// <summary>
        /// Default Earth Velocity plot.
        /// </summary>
        public TimeSeriesPlotViewModel TimeSeries3Plot { get; set; }

        #endregion

        #endregion

        #region Plot Sizes

        /// <summary>
        /// Height of the Contour plot.
        /// </summary>
        public int PlotSize2D
        {
            get { return _options.PlotSize2D; }
            set
            {
                _options.PlotSize2D = value;
                this.NotifyOfPropertyChange(() => this.PlotSize2D);

                // Update the database with the latest options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Transform

        /// <summary>
        /// Selection of what coordinate transform to display.
        /// </summary>
        public Core.Commons.Transforms SelectedVelocityPlotTransform
        {
            get { return _options.SelectedVelocityPlotTransform; }
            set
            {
                _options.SelectedVelocityPlotTransform = value;
                this.NotifyOfPropertyChange(() => this.SelectedVelocityPlotTransform);

                // Update the database
                //UpdateDatabaseOptions();
            }
        }

        #endregion

        #region IsLoading

        /// <summary>
        /// Is Loading flag to know when a big process is loading.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Is Loading flag to know when a big process is loading.
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

        #endregion

        #region Commands

        /// <summary>
        /// Command to clear all the plots.
        /// </summary>
        public ReactiveCommand<object> ClearPlotCommand { get; protected set; }

        /// <summary>
        /// Command to close this VM.
        /// </summary>
        public ReactiveCommand<object> CloseVMCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// <param name="config">Configuration containing data source and SubsystemConfiguration.</param>
        /// </summary>
        public ViewDataGraphicalViewModel(SubsystemDataConfig config) 
            : base("ViewDataGraphicalViewModel")
        {
            // Set Subsystem 
            _Config = config;
            //_displayCounter = 0;

            // Get the Event Aggregator
            _events = IoC.Get<IEventAggregator>();

            // Get PulseManager
            _pm = IoC.Get<PulseManager>();
            _pm.RegisterDisplayVM(this);

            // Get the options from the database
            GetOptionsFromDatabase();

            // Create the Text view
            TextEnsembleVM = new ViewDataTextEnsembleViewModel(config);

            // Initialize values
            _prevBinSize = DEFAULT_PREV_BIN_SIZE;
            _prevNumBins = DEFAULT_PREV_NUM_BIN;
            _prevEnsNum = 0;
            IsLoading = false;
            //_maxEnsembles = DEFAULT_MAX_ENSEMBLES;                                  // Max ensembles to use to store data
            //_isProcessingBuffer = false;
            _buffer = new ConcurrentQueue<DataSet.Ensemble>();

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = string.Format("Graphical View: {0}", config.DescString());
            _processDataThread.Start();


            // Setup the plots
            SetupPlots();

            // Clear Plot command
            //ClearPlotCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ClearPlots()));
            ClearPlotCommand = ReactiveCommand.Create();
            ClearPlotCommand.Subscribe(_ => ClearPlots());

            // Close the VM
            CloseVMCommand = ReactiveCommand.Create();
            CloseVMCommand.Subscribe(_ => _events.PublishOnUIThread(new CloseVmEvent(_Config)));

            _events.Subscribe(this);
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            // Wake up the thread to process data
            _continue = false;
            _eventWaitData.Set();

            AmpPlot.Dispose();
            CorrPlot.Dispose();

            TextEnsembleVM.Dispose();

            HeatmapPlot.Dispose();

            if (TimeSeries1Plot != null)
            {
                TimeSeries1Plot.AddSeriesEvent -= TimeSeries1Plot_AddSeriesEvent;
                TimeSeries1Plot.RemoveSeriesEvent -= TimeSeries1Plot_RemoveSeriesEvent;
            }

            if (TimeSeries2Plot != null)
            {
                TimeSeries2Plot.AddSeriesEvent -= TimeSeries2Plot_AddSeriesEvent;
                TimeSeries2Plot.RemoveSeriesEvent -= TimeSeries2Plot_RemoveSeriesEvent;
            }

            if(TimeSeries3Plot != null)
            {
                TimeSeries3Plot.AddSeriesEvent -= TimeSeries3Plot_AddSeriesEvent;
                TimeSeries3Plot.RemoveSeriesEvent -= TimeSeries3Plot_RemoveSeriesEvent;
            }
        }

        #region Plots

        #region Create Plots

        /// <summary>
        /// Create all the plots.
        /// </summary>
        private void SetupPlots()
        {
            CorrPlot = new ProfilePlotViewModel(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation));
            CorrPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Correlation), 0, OxyColor.Parse(_options.Beam0Color));
            CorrPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Correlation), 1, OxyColor.Parse(_options.Beam1Color));
            CorrPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Correlation), 2, OxyColor.Parse(_options.Beam2Color));
            CorrPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Correlation), 3, OxyColor.Parse(_options.Beam3Color));

            AmpPlot = new ProfilePlotViewModel(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude));
            AmpPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Amplitude), 0, OxyColor.Parse(_options.Beam0Color));
            AmpPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Amplitude), 1, OxyColor.Parse(_options.Beam1Color));
            AmpPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Amplitude), 2, OxyColor.Parse(_options.Beam2Color));
            AmpPlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Amplitude), 3, OxyColor.Parse(_options.Beam3Color));

            VelProfilePlot = new ProfilePlotViewModel(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam));
            VelProfilePlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Velocity_BEAM), 0, OxyColor.Parse(_options.Beam0Color));
            VelProfilePlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Velocity_BEAM), 1, OxyColor.Parse(_options.Beam1Color));
            VelProfilePlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Velocity_BEAM), 2, OxyColor.Parse(_options.Beam2Color));
            VelProfilePlot.AddSeries(new ProfileType(ProfileType.eProfileType.WP_Velocity_BEAM), 3, OxyColor.Parse(_options.Beam3Color));

            VelPlot = new BinPlot3D();
            VelPlot.CylinderRadius = 0;
            VelPlot.ColormapBrushSelection = ColormapBrushSelection;
            VelPlot.MinVelocity = ContourMinValue;
            VelPlot.MaxVelocity = ContourMaxValue;

            HeatmapPlot = new HeatmapPlotViewModel(HeatmapPlotSeries.HeatmapPlotType.Earth_Velocity_Magnitude, _options.HeatmapOptions);
            HeatmapPlot.Plot.LegendPosition = LegendPosition.TopCenter;
            HeatmapPlot.AddSeries(_options.HeatmapOptions);
            HeatmapPlot.AddBtSeries();
            HeatmapPlot.UpdateOptionsEvent += HeatmapPlot_UpdateOptionsEvent;                                       // Options changed event

            _ensembleHistory = new LimitedList<DataSet.Ensemble>(DisplayMaxEnsembles);                              // Ensemble History

            #region Time Series 1 Plot

            // TimeSeries 1 Plot
            // Default Bottom Track Range Data
            if (_options.TimeSeries1Options.Count > 0)
            {
                var sourceSeries1 = new DataSource(_options.TimeSeries1Options.First().Source);
                var typeSeries1 = new BaseSeriesType(_options.TimeSeries1Options.First().Type);
                TimeSeries1Plot = new TimeSeriesPlotViewModel(new SeriesType(sourceSeries1, typeSeries1));
                for (int x = 0; x < _options.TimeSeries1Options.Count; x++)
                {
                    sourceSeries1 = new DataSource(_options.TimeSeries1Options[x].Source);
                    typeSeries1 = new BaseSeriesType(_options.TimeSeries1Options[x].Type);
                    TimeSeries1Plot.AddSeries(sourceSeries1, typeSeries1, _options.TimeSeries1Options[x].Beam, _options.TimeSeries1Options[x].Bin, OxyColor.Parse(_options.TimeSeries1Options[x].Color));
                }
                TimeSeries1Plot.Plot.TitlePadding = 0;
                TimeSeries1Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
                TimeSeries1Plot.AddSeriesEvent += TimeSeries1Plot_AddSeriesEvent;                                       // Put after adding series so they do not get added twice
                TimeSeries1Plot.RemoveSeriesEvent += TimeSeries1Plot_RemoveSeriesEvent;
            }
            else
            {
                var source1 = new DataSource(DataSource.eSource.WaterProfile);
                var type1 = new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam);
                TimeSeries1Plot = new TimeSeriesPlotViewModel(new SeriesType(source1, type1));
                TimeSeries1Plot.AddSeriesEvent += TimeSeries1Plot_AddSeriesEvent;                                       // Put before adding series so they will get added to the options
                TimeSeries1Plot.RemoveSeriesEvent += TimeSeries1Plot_RemoveSeriesEvent;
                TimeSeries1Plot.AddSeries(source1, type1, 0, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0));
                TimeSeries1Plot.AddSeries(source1, type1, 1, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_1));
                TimeSeries1Plot.AddSeries(source1, type1, 2, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_2));
                TimeSeries1Plot.AddSeries(source1, type1, 3, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_3));
                TimeSeries1Plot.Plot.TitlePadding = 0;
                TimeSeries1Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
            }

            #endregion

            #region Time Series 2 Plot

            // TimeSeries 2 Plot
            // Default Bottom Track Velocity data
            if (_options.TimeSeries2Options.Count > 0)
            {
                var sourceSeries2 = new DataSource(_options.TimeSeries2Options.First().Source);
                var typeSeries2 = new BaseSeriesType(_options.TimeSeries2Options.First().Type);
                TimeSeries2Plot = new TimeSeriesPlotViewModel(new SeriesType(sourceSeries2, typeSeries2));
                for (int x = 0; x < _options.TimeSeries2Options.Count; x++)
                {
                    sourceSeries2 = new DataSource(_options.TimeSeries2Options[x].Source);
                    typeSeries2 = new BaseSeriesType(_options.TimeSeries2Options[x].Type);
                    TimeSeries2Plot.AddSeries(sourceSeries2, typeSeries2, _options.TimeSeries2Options[x].Beam, _options.TimeSeries2Options[x].Bin, OxyColor.Parse(_options.TimeSeries2Options[x].Color));
                }
                TimeSeries2Plot.Plot.TitlePadding = 0;
                TimeSeries2Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
                TimeSeries2Plot.AddSeriesEvent += TimeSeries2Plot_AddSeriesEvent;                                       // Put after adding series so they do not get added twice
                TimeSeries2Plot.RemoveSeriesEvent += TimeSeries2Plot_RemoveSeriesEvent;
            }
            else
            {
                var source2 = new DataSource(DataSource.eSource.BottomTrack);
                var type2 = new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam);
                TimeSeries2Plot = new TimeSeriesPlotViewModel(new SeriesType(source2, type2));
                TimeSeries2Plot.AddSeriesEvent += TimeSeries2Plot_AddSeriesEvent;                                       // Put before adding series so they will get added to the options
                TimeSeries2Plot.RemoveSeriesEvent += TimeSeries2Plot_RemoveSeriesEvent;
                TimeSeries2Plot.AddSeries(source2, type2, 0, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0));
                TimeSeries2Plot.AddSeries(source2, type2, 1, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_1));
                TimeSeries2Plot.AddSeries(source2, type2, 2, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_2));
                TimeSeries2Plot.AddSeries(source2, type2, 3, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_3));
                TimeSeries2Plot.Plot.TitlePadding = 0;
                TimeSeries2Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
            }

            #endregion

            #region Time Series 3 Plot

            // TimeSeries 3 Plot
            // Default Bottom Track Range Data
            if (_options.TimeSeries3Options.Count > 0)
            {
                var sourceSeries3 = new DataSource(_options.TimeSeries3Options.First().Source);
                var typeSeries3 = new BaseSeriesType(_options.TimeSeries3Options.First().Type);
                TimeSeries3Plot = new TimeSeriesPlotViewModel(new SeriesType(sourceSeries3, typeSeries3));
                for(int x = 0; x < _options.TimeSeries3Options.Count; x++)
                {
                    sourceSeries3 = new DataSource(_options.TimeSeries3Options[x].Source);
                    typeSeries3 = new BaseSeriesType(_options.TimeSeries3Options[x].Type);
                    TimeSeries3Plot.AddSeries(sourceSeries3, typeSeries3, _options.TimeSeries3Options[x].Beam, _options.TimeSeries3Options[x].Bin, OxyColor.Parse(_options.TimeSeries3Options[x].Color));
                }
                TimeSeries3Plot.Plot.TitlePadding = 0;
                TimeSeries3Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
                TimeSeries3Plot.AddSeriesEvent += TimeSeries3Plot_AddSeriesEvent;                                       // Put after adding series so they do not get added twice
                TimeSeries3Plot.RemoveSeriesEvent += TimeSeries3Plot_RemoveSeriesEvent;
            }
            else
            {
                var source3 = new DataSource(DataSource.eSource.BottomTrack);
                var type3 = new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range);
                TimeSeries3Plot = new TimeSeriesPlotViewModel(new SeriesType(source3, type3));
                TimeSeries3Plot.AddSeriesEvent += TimeSeries3Plot_AddSeriesEvent;                                       // Put before adding series so they will get added to the options
                TimeSeries3Plot.RemoveSeriesEvent += TimeSeries3Plot_RemoveSeriesEvent;
                TimeSeries3Plot.AddSeries(source3, type3, 0, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_0));
                TimeSeries3Plot.AddSeries(source3, type3, 1, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_1));
                TimeSeries3Plot.AddSeries(source3, type3, 2, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_2));
                TimeSeries3Plot.AddSeries(source3, type3, 3, 0, OxyColor.Parse(BeamColor.DEFAULT_COLOR_BEAM_3));
                TimeSeries3Plot.Plot.TitlePadding = 0;
                TimeSeries3Plot.Plot.Padding = new OxyPlot.OxyThickness(0);
            }

            #endregion
        }



        /// <summary>
        /// Create a plotmodel with the following settings.
        /// </summary>
        /// <param name="name">Name of the plot.</param>
        /// <returns>A plotmodel setup.</returns>
        private PlotModel CreatePlot(string name)
        {
            var temp = new PlotModel() { Title = name };

            // Create line series and add to plot
            LineSeries beam1 = new LineSeries();
            LineSeries beam2 = new LineSeries();
            LineSeries beam3 = new LineSeries();
            LineSeries beam4 = new LineSeries();

            //add series to plot
            temp.Series.Add(beam1);
            temp.Series.Add(beam2);
            temp.Series.Add(beam3);
            temp.Series.Add(beam4);

            // Do not change margins
            //temp.AutoAdjustPlotMargins = false;

            // No legend
            //temp.IsLegendVisible = false;

            // No spacing around graph
            //temp.PlotMargins = new OxyThickness(0);
            //temp.Padding = new OxyThickness(35, 10, 5, 10);
            temp.Background = OxyColors.Black;
            temp.TextColor = OxyColors.White;
            temp.PlotAreaBorderColor = OxyColors.White;

            return temp;
        }


        #endregion

        #region Update Plots

        #region Data

        ///<summary>
        /// Update all the series to display
        /// the graph.
        /// Set the isUpdatingDisplay if you are updating the plot with new data.
        /// This will update the timeseries and contour plot with new data.  If
        /// you displaying a selected ensemble, do not add to the timeseries or
        /// contour plot.
        ///</summary>
        ///<param name="ensemble">Ensembles to update the plots.</param>
        ///<param name="isUpdatingDisplay">Updating the display flag.</param>
        private async void AddSeries(DataSet.Ensemble ensemble, bool isUpdatingDisplay = true)
        {
            // Verify the ensemble is good
            if (ensemble != null)
            {
                // Updating the display needs to update the timeseries and contour plot
                // Displaying the selected ensemble, does not need to update these plots
                if (isUpdatingDisplay)
                {
                    // Time series plot
                    await TimeSeries1Plot.AddIncomingData(ensemble, DisplayMaxEnsembles);
                    await TimeSeries2Plot.AddIncomingData(ensemble, DisplayMaxEnsembles);
                    await TimeSeries3Plot.AddIncomingData(ensemble, DisplayMaxEnsembles);

                    try
                    {
                        await HeatmapPlot.AddIncomingData(ensemble, DisplayMaxEnsembles);
                    }
                    catch (Exception e)
                    {
                        log.Error("Error adding ensemble to Heatmap plots.", e);
                    }
                }

                // Verify the data is available before trying to draw
                if (ensemble.IsCorrelationAvail)
                {
                    // Correlation plot
                    await CorrPlot.AddIncomingData(ensemble, ensemble.EnsembleData.NumBins);
                }
                if (ensemble.IsAmplitudeAvail)
                {
                    // Amplitude plot
                    await AmpPlot.AddIncomingData(ensemble, ensemble.EnsembleData.NumBins);
                }
                if (ensemble.IsBeamVelocityAvail || ensemble.IsEarthVelocityAvail || ensemble.IsInstrumentVelocityAvail)
                {
                    // Velocity Profile plot
                    await VelProfilePlot.AddIncomingData(ensemble, ensemble.EnsembleData.NumBins);
                }

                if (ensemble.IsEarthVelocityAvail)
                {
                    if (!ensemble.EarthVelocityData.IsVelocityVectorAvail)
                    {
                        // Add Velocity Vectors
                        DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);
                    }

                    // Velocity 3D plot
                    await VelPlot.AddIncomingData(DataSet.VelocityVectorHelper.GetEarthVelocityVectors(ensemble));

                    //VelPlot.AddIncomingData(DataSet.VelocityVectorHelper.GetInstrumentVelocityVectors(ensemble));
                }
            }
        }

        /// <summary>
        /// Add the data is bulk.  This will reduce the number of screen updates.
        /// </summary>
        /// <param name="ensembles">Ensembles to display.</param>
        /// <param name="maxEnsembles">Max Ensembles to display.</param>
        private void AddSeriesBulk(Cache<long, DataSet.Ensemble> ensembles, int maxEnsembles)
        {
            IsLoading = true;

            // Time series plot
            TimeSeries1Plot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);
            TimeSeries2Plot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);
            TimeSeries3Plot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);

            try
            {
                HeatmapPlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);
            }
            catch (Exception e)
            {
                log.Error("Error adding ensemble to Heatmap plots.", e);
            }

            // Correlation plot
            CorrPlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config);

            // Amplitude plot
            AmpPlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config);

            // Velocity Profile plot
            VelProfilePlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config);

            // Only the last ensemble needs to be displayed
            if (ensembles.Count() > 0)
            {

                // Get the last ensemble
                var ensemble = ensembles.Get(ensembles.Count()- 1);

                if (ensemble != null)
                {
                    if (ensemble.IsEarthVelocityAvail)
                    {
                        if (!ensemble.EarthVelocityData.IsVelocityVectorAvail)
                        {
                            // Add Velocity Vectors
                            DataSet.VelocityVectorHelper.CreateVelocityVector(ref ensemble);
                        }

                        // Velocity 3D plot
                        VelPlot.AddIncomingData(DataSet.VelocityVectorHelper.GetEarthVelocityVectors(ensemble));

                        //VelPlot.AddIncomingData(DataSet.VelocityVectorHelper.GetInstrumentVelocityVectors(ensemble));
                    }
                }
            }

            IsLoading = false;
        }

        #endregion

        #region Correlation

        ///// <summary>
        ///// Add the correlation series to the graph.
        ///// </summary>
        ///// <param name="adcpData">Latest Ensemble data.</param>
        //private void AddCorrelationSeries(DataSet.Ensemble adcpData)
        //{
        //    // Get the LineSeries data for Correlation
        //    // Use a scale factor of 100 to turn decimal value into a percent (ex: 1.0 = 100% and 0.5 = 50%)
        //    LineSeriesValues lsv = GenerateLineSeries(adcpData.CorrelationData.CorrelationData,                 // Correlation Data
        //                                                adcpData.CorrelationData.NumElements,                   // Number of bins
        //                                                adcpData.AncillaryData.FirstBinRange,                   // Depth of first Bin
        //                                                adcpData.AncillaryData.BinSize,                         // Size of a Bin
        //                                                100);                                                   // Scale factor to convert from decimal to percent

        //    // Use dispatcher to update data on plot
        //    try
        //    {
        //        Application.Current.Dispatcher.BeginInvoke(new System.Action(() => UpdateCorrelationPlot(lsv)));
        //    }
        //    catch (Exception ex)
        //    {
        //        // When shutting down, can get a null reference
        //        log.Debug("Error updating Correlation Plot", ex);
        //    }
        //}

        ///// <summary>
        ///// Update the Correlation Plot with the latest data.
        ///// This method must be called by the dispatcher.
        ///// Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateCorrelationPlot(lsv)));
        ///// </summary>
        ///// <param name="lsv">Latest Series to add to the plot.</param>
        //private void UpdateCorrelationPlot(LineSeriesValues lsv)
        //{
        //    CorrPlot.Series.Clear();

        //    // Add series to plot
        //    CorrPlot.Series.Add(lsv.Beam0Series);
        //    CorrPlot.Series.Add(lsv.Beam1Series);
        //    CorrPlot.Series.Add(lsv.Beam2Series);
        //    CorrPlot.Series.Add(lsv.Beam3Series);

        //    UpdatePlot(new AdcpPlotEvent(AdcpPlotEvent.AdcpPlotTypes.PLOT_CORRELATION));
        //}

        #endregion

        #region Amplitude

        ///// <summary>
        ///// Add the amplitude series to the graph.
        ///// </summary>
        ///// <param name="adcpData">Latest Ensemble data.</param>
        //private void AddAmplitudeSeries(DataSet.Ensemble adcpData)
        //{
        //    // Get the LineSeries data for Amplitude
        //    LineSeriesValues lsv = GenerateLineSeries(adcpData.AmplitudeData.AmplitudeData,         // Amplitude data 
        //                                                adcpData.AmplitudeData.NumElements,             // Number of bins
        //                                                adcpData.AncillaryData.FirstBinRange,       // Depth for first Bin
        //                                                adcpData.AncillaryData.BinSize);           // Size of each Bin

        //    // Use dispatcher to update data on the plot
        //    try
        //    {
        //        Application.Current.Dispatcher.BeginInvoke(new System.Action(() => UpdateAmplitudePlot(lsv)));
        //    }
        //    catch (Exception ex)
        //    {
        //        // When shutting down, can get a null reference
        //        log.Debug("Error updating Amplitude Plot", ex);
        //    }
        //}

        ///// <summary>
        ///// Update the Amplitude plot with the latest data.
        ///// This method must be called by the dispatcher.
        ///// Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateAmplitudePlot(lsv)));
        ///// </summary>
        ///// <param name="lsv">Latest data to update plot.</param>
        //private void UpdateAmplitudePlot(LineSeriesValues lsv)
        //{
        //    AmpPlot.Series.Clear();

        //    // Add series to plot
        //    AmpPlot.Series.Add(lsv.Beam0Series);
        //    AmpPlot.Series.Add(lsv.Beam1Series);
        //    AmpPlot.Series.Add(lsv.Beam2Series);
        //    AmpPlot.Series.Add(lsv.Beam3Series);

        //    UpdatePlot(new AdcpPlotEvent(AdcpPlotEvent.AdcpPlotTypes.PLOT_AMPLITUDE));
        //}

        #endregion

        #region Velocity

        ///// <summary>
        ///// Add the velocity data to the plot.  This will determine which transformation
        ///// is selected to determine which data it should use for the plot.
        ///// </summary>
        ///// <param name="adcpData">Latest Ensemble data.</param>
        //private void AddVelocitySeries(DataSet.Ensemble adcpData)
        //{
        //    LineSeriesValues lsv = null;

        //    // Determine which transform is selected to set the line series
        //    switch (SelectedVelocityPlotTransform)
        //    {
        //        case Core.Commons.Transforms.EARTH:
        //            if (adcpData.IsEarthVelocityAvail)
        //            {
        //                // Get the LineSeries data for Earth Velocity data
        //                lsv = GenerateLineSeries(adcpData.EarthVelocityData.EarthVelocityData,                  // Earth Velocity data 
        //                                                            adcpData.EarthVelocityData.NumElements,     // Number of bins
        //                                                            adcpData.AncillaryData.FirstBinRange,       // Depth for first Bin
        //                                                            adcpData.AncillaryData.BinSize);            // Size of each Bin
        //            }
        //            break;
        //        case Core.Commons.Transforms.INSTRUMENT:
        //            if (adcpData.IsInstrumentVelocityAvail)
        //            {
        //                // Get the LineSeries data for Instrument Velocity data
        //                lsv = GenerateLineSeries(adcpData.InstrumentVelocityData.InstrumentVelocityData,             // Instrument Velocity data 
        //                                                            adcpData.InstrumentVelocityData.NumElements,     // Number of bins
        //                                                            adcpData.AncillaryData.FirstBinRange,       // Depth for first Bin
        //                                                            adcpData.AncillaryData.BinSize);            // Size of each Bin
        //            }
        //            break;
        //        case Core.Commons.Transforms.BEAM:
        //            if (adcpData.IsBeamVelocityAvail)
        //            {
        //                // Get the LineSeries data for Beam Velocity data
        //                lsv = GenerateLineSeries(adcpData.BeamVelocityData.BeamVelocityData,                    // Beam Velocity data 
        //                                                            adcpData.BeamVelocityData.NumElements,      // Number of bins
        //                                                            adcpData.AncillaryData.FirstBinRange,       // Depth for first Bin
        //                                                            adcpData.AncillaryData.BinSize);            // Size of each Bin
        //            }
        //            break;
        //    }

        //    if (lsv != null)
        //    {
        //        // Use dispatcher to update data on the plot
        //        try
        //        {
        //            Application.Current.Dispatcher.BeginInvoke(new System.Action(() => UpdateVelocityPlot(lsv)));
        //        }
        //        catch (Exception ex)
        //        {
        //            // When shutting down, can get a null reference
        //            log.Debug("Error updating Velocity Plot", ex);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Update the Velocity plot with the latest data.
        ///// This method must be called by the dispatcher.
        ///// Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateVelocityPlot(lsv)));
        ///// </summary>
        ///// <param name="lsv">Latest data to update plot.</param>
        //private void UpdateVelocityPlot(LineSeriesValues lsv)
        //{
        //    VelPlot.Series.Clear();

        //    // Add series to plot
        //    VelPlot.Series.Add(lsv.Beam0Series);
        //    VelPlot.Series.Add(lsv.Beam1Series);
        //    VelPlot.Series.Add(lsv.Beam2Series);
        //    VelPlot.Series.Add(lsv.Beam3Series);

        //    UpdatePlot(new AdcpPlotEvent(AdcpPlotEvent.AdcpPlotTypes.PLOT_VELOCITY));
        //}

        #endregion

        #endregion

        #region Clear Plots

        /// <summary>
        /// Clear all the values for the plots.
        /// </summary>
        public void ClearPlots()
        {
            // Use a negative number
            // to know if the value has
            // changed or never been set
            _prevBinSize = DEFAULT_PREV_BIN_SIZE;
            _prevNumBins = DEFAULT_PREV_NUM_BIN;

            _prevEnsNum = 0;

            // Clear plots
            CorrPlot.ClearIncomingData();
            AmpPlot.ClearIncomingData();
            VelPlot.ClearIncomingData();
            _ensembleHistory.Clear();
            TimeSeries1Plot.ClearIncomingData();
            TimeSeries2Plot.ClearIncomingData();
            TimeSeries3Plot.ClearIncomingData();
            HeatmapPlot.ClearIncomingData();
        }

        #endregion

        #endregion

        #region Database

        /// <summary>
        /// Get the options for this subsystem display
        /// from the database.  If the options have not
        /// been set to the database yet, default values 
        /// will be used.
        /// </summary>
        private void GetOptionsFromDatabase()
        {
            //var ssConfig = new SubsystemConfiguration(_Config.SubSystem, _Config.CepoIndex, _Config.SubsystemConfigIndex);
            //_options = _pm.AppConfiguration.GetGraphicalOptions(ssConfig);

            //// Notify all the properties
            //NotifyOptionPropertyChange();

            _options = _pm.GetGraphicalViewOptions();

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
            //this.NotifyOfPropertyChange(() => this.Beam0Color);
            //this.NotifyOfPropertyChange(() => this.Beam0ColorStr);
            //this.NotifyOfPropertyChange(() => this.Beam1Color);
            //this.NotifyOfPropertyChange(() => this.Beam1ColorStr);
            //this.NotifyOfPropertyChange(() => this.Beam2Color);
            //this.NotifyOfPropertyChange(() => this.Beam2ColorStr);
            //this.NotifyOfPropertyChange(() => this.Beam3Color);
            //this.NotifyOfPropertyChange(() => this.Beam3ColorStr);
            this.NotifyOfPropertyChange(() => this.PlotSize2D);
            this.NotifyOfPropertyChange(() => this.SelectedVelocityPlotTransform);
            this.NotifyOfPropertyChange(() => this.DisplayMaxEnsembles);
            this.NotifyOfPropertyChange(() => this.ColormapBrushSelection);
            this.NotifyOfPropertyChange(() => this.ContourMinValue);
            this.NotifyOfPropertyChange(() => this.ContourMaxValue);
            this.NotifyOfPropertyChange(() => this.HeatmapPlot);

        }

        /// <summary>
        /// Update the database with the latest options.
        /// </summary>
        private void UpdateDatabaseOptions()
        {
            //// SubsystemDataConfig needs to be converted to a SubsystemConfiguration
            //// because the SubsystemConfig will be compared in AppConfiguration to determine
            //// where to save the settings.  Because SubsystemDataConfig and SubsystemConfiguration
            //// are not the same type, it will not pass Equal()
            //var ssConfig = new SubsystemConfiguration(_Config.SubSystem, _Config.CepoIndex, _Config.SubsystemConfigIndex);

            //_pm.AppConfiguration.SaveGraphicalOptions(ssConfig, _options);

            _pm.UpdateGraphicalViewOptions(_options);
        }

        #endregion

        #region Check Data

        /// <summary>
        /// Check if the dataset change settings.  If so, clear the ContourPlot and
        /// the bottom track plots.  The Binsize is set to an int because the binsize decimal value
        /// bounces around.
        /// </summary>
        /// <param name="ensemble">Adcp data to check against previous dataset.</param>
        private void CheckData(DataSet.Ensemble ensemble)
        {
            if (ensemble == null || !ensemble.IsEnsembleAvail || !ensemble.IsAncillaryAvail)
            {
                return;
            }

            // If the Bin size or number of bins has changed, clear data
            if (_prevBinSize != (int)ensemble.AncillaryData.BinSize ||
                _prevNumBins != ensemble.EnsembleData.NumBins)
            {
                // If the prev bin and number of bins are negative
                // they have just never been set and we do not
                // need to clear
                if (_prevBinSize >= 0 && _prevNumBins >= 0)
                {
                    // Clear the plots
                    ClearPlots();
                }

                _prevBinSize = (int)ensemble.AncillaryData.BinSize;
                _prevNumBins = ensemble.EnsembleData.NumBins;

                // Set the new values for the axis
                //SetupAxis(ensemble);
            }

            // Check if the previous index was greater or less than the new one
            // If the previous index is greater than new one, then the user has moved
            // back in the data, the time series plots will be messed up and should be cleared
            //
            // The ensembles may be out of order, subtracting by 15 will catch big jumps in ensemble
            // numbers.  15 is chosen, because there can be up to 12 subsystems so 12 different types
            // of ensembles output for any system.
            if (_prevEnsNum - 15 > ensemble.EnsembleData.EnsembleNumber)
            {
                ClearPlots();
            }

            _prevEnsNum = ensemble.EnsembleData.EnsembleNumber;
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
                            // Check the data for changes
                            CheckData(ensemble);

                            // Update Plots
                            AddSeries(ensemble);

                            // Add the ensemble to ensemble history
                            _ensembleHistory.Add(ensemble);

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
        /// <param name="maxEnsembles">Maximum ensembles to display.</param>
        public void DisplayBulkData(Cache<long, DataSet.Ensemble> ensembles, int maxEnsembles)
        {
            IsLoading = true;

            // Check the data for changes
            //CheckData(ensemble);

            // Update Plots
            //await Task.Run(() => AddSeriesBulk(ensembles, maxEnsembles));
            AddSeriesBulk(ensembles, maxEnsembles);

            // Add the ensemble to ensemble history
            //_ensembleHistory.Add(ensemble);

            if (ensembles.Count() > 0)
            {
                // Get the last ensemble
                var ensemble = ensembles.IndexValue(ensembles.Count() - 1);

                // Add the data to the Text Display
                TextEnsembleVM.ReceiveEnsemble(ensemble);
            }

            IsLoading = false;

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
            }

            // Set selected ensemble
            AddSeries(ensEvent.Ensemble, false);

            // Add the data to the Text Display
            TextEnsembleVM.ReceiveEnsemble(ensEvent.Ensemble);
        }

        /// <summary>
        /// Receive event when a new project has been selected.
        /// Then clear all the data in the view.
        /// </summary>
        /// <param name="prjEvent">Project Event received.</param>
        public void Handle(ProjectEvent prjEvent)
        {
            // Clear the display
            SetupPlots();

            // Get the new options for this project from the database
            GetOptionsFromDatabase();
        }

        /// <summary>
        /// Eventhandler when the contour plot is clicked by the left mouse button.
        /// This will give the ensemble and bin selected in the contour plot.
        /// Publish this information.
        /// </summary>
        /// <param name="ms">Mouse selection.</param>
        void _ContourPlot_LeftMouseButtonEvent(ContourPlotMouseSelection ms)
        {
            // Get the ensemble and bin selected
            if (_pm.IsProjectSelected && ms != null)
            {
                // Ensure the selected area actually contained data
                if (ms.Index < _ensembleHistory.Count)
                {
                    //DataSet.Ensemble ens = _pm.SelectedProject.GetEnsemble(ms.Index);
                    DataSet.Ensemble ens = _ensembleHistory[ms.Index];

                    // Publish the selected ensemble
                    _events.PublishOnUIThread(new SelectedEnsembleEvent(ens, EnsembleSource.Playback, EnsembleType.Single, ms));
                }
            }
        }

        #region Time Series Options

        #region Time Series Plot 1

        /// <summary>
        /// Add the series to the options.
        /// </summary>
        /// <param name="source">Data Source.</param>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries1Plot_AddSeriesEvent(DataSource.eSource source, BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            bool seriesExist = false;
            for (int x = 0; x < _options.TimeSeries1Options.Count; x++)
            {
                if (_options.TimeSeries1Options[x].Source == source &&
                    _options.TimeSeries1Options[x].Type == type &&
                    _options.TimeSeries1Options[x].Beam == beam &&
                    _options.TimeSeries1Options[x].Bin == bin &&
                    _options.TimeSeries1Options[x].Color.Equals(color))
                {
                    seriesExist = true;
                }
            }

            // If the series exist, do not add it to the list in options
            if (seriesExist)
            {
                return;
            }

            var option = new TimeSeriesOptions(source, type, beam, bin, color);
            _options.TimeSeries1Options.Add(option);

            // Update the options
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Remove the series from the options.
        /// </summary>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries1Plot_RemoveSeriesEvent(BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            // Find the series
            int index = -1;
            for (int x = 0; x < _options.TimeSeries1Options.Count; x++)
            {
                if (_options.TimeSeries1Options[x].Type == type &&
                    _options.TimeSeries1Options[x].Beam == beam &&
                    _options.TimeSeries1Options[x].Bin == bin &&
                    _options.TimeSeries1Options[x].Color.Equals(color))
                {
                    index = x;
                }
            }

            // Remove the series at the index if found
            if (index >= 0)
            {
                _options.TimeSeries1Options.RemoveAt(index);

                // Update the options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Time Series Plot 2

        /// <summary>
        /// Add the series to the options.
        /// </summary>
        /// <param name="source">Data Source.</param>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries2Plot_AddSeriesEvent(DataSource.eSource source, BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            bool seriesExist = false;
            for (int x = 0; x < _options.TimeSeries2Options.Count; x++)
            {
                if (_options.TimeSeries2Options[x].Source == source &&
                    _options.TimeSeries2Options[x].Type == type &&
                    _options.TimeSeries2Options[x].Beam == beam &&
                    _options.TimeSeries2Options[x].Bin == bin &&
                    _options.TimeSeries2Options[x].Color.Equals(color))
                {
                    seriesExist = true;
                }
            }

            // If the series exist, do not add it to the list in options
            if (seriesExist)
            {
                return;
            }

            var option = new TimeSeriesOptions(source, type, beam, bin, color);
            _options.TimeSeries2Options.Add(option);

            // Update the options
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Remove the series from the options.
        /// </summary>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries2Plot_RemoveSeriesEvent(BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            // Find the series
            int index = -1;
            for (int x = 0; x < _options.TimeSeries2Options.Count; x++)
            {
                if (_options.TimeSeries2Options[x].Type == type &&
                    _options.TimeSeries2Options[x].Beam == beam &&
                    _options.TimeSeries2Options[x].Bin == bin &&
                    _options.TimeSeries2Options[x].Color.Equals(color))
                {
                    index = x;
                }
            }

            // Remove the series at the index if found
            if (index >= 0)
            {
                _options.TimeSeries2Options.RemoveAt(index);

                // Update the options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #region Time Series Plot 3

        /// <summary>
        /// Add the series to the options.
        /// </summary>
        /// <param name="source">Data Source.</param>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries3Plot_AddSeriesEvent(DataSource.eSource source, BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            bool seriesExist = false;
            for (int x = 0; x < _options.TimeSeries3Options.Count; x++)
            {
                if (_options.TimeSeries3Options[x].Source == source &&
                    _options.TimeSeries3Options[x].Type == type &&
                    _options.TimeSeries3Options[x].Beam == beam &&
                    _options.TimeSeries3Options[x].Bin == bin &&
                    _options.TimeSeries3Options[x].Color.Equals(color))
                {
                    seriesExist = true;
                }
            }

            // If the series exist, do not add it to the list in options
            if(seriesExist)
            {
                return;
            }

            var option = new TimeSeriesOptions(source, type, beam, bin, color);
            _options.TimeSeries3Options.Add(option);

            // Update the options
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Remove the series from the options.
        /// </summary>
        /// <param name="type">Series type.</param>
        /// <param name="beam">Beam number.</param>
        /// <param name="bin">Bin number.</param>
        /// <param name="color">Series colors.</param>
        void TimeSeries3Plot_RemoveSeriesEvent(BaseSeriesType.eBaseSeriesType type, int beam, int bin, string color)
        {
            // Find the series
            int index = -1;
            for(int x = 0; x < _options.TimeSeries3Options.Count; x++)
            {
                if(_options.TimeSeries3Options[x].Type == type && 
                    _options.TimeSeries3Options[x].Beam == beam && 
                    _options.TimeSeries3Options[x].Bin == bin && 
                    _options.TimeSeries3Options[x].Color.Equals(color))
                {
                    index = x;
                }
            }

            // Remove the series at the index if found
            if(index >= 0)
            {
                _options.TimeSeries3Options.RemoveAt(index);

                // Update the options
                UpdateDatabaseOptions();
            }
        }

        #endregion

        #endregion

        #region Heatmap Options

        /// <summary>
        /// Update the options with the latest Heatmap options.
        /// </summary>
        /// <param name="options">Options used.</param>
        void HeatmapPlot_UpdateOptionsEvent(HeatmapSeriesOptions options)
        {
            // Set the options
            _options.HeatmapOptions = options;

            // Update the database with the latest options
            UpdateDatabaseOptions();
        }

        #endregion

        #endregion
    }
}
