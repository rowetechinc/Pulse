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
 * 11/16/2015      RC          4.3.1      Added a thread.
 * 07/26/2016      RC          4.4.12     Removed 3 plots and left only 1.
 * 
 */ 


using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Backscatter display to show the heatmaps for the 
    /// velocities and backscatter.
    /// </summary>
    public class BackscatterViewModel : DisplayViewModel
    {

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
        /// Pulse manager to manage the application.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Options for this ViewModel.
        /// </summary>
        private BackscatterOptions _options;

        ///// <summary>
        ///// Limit how often the display will be updated.
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

        #region Plot

        /// <summary>
        /// The plot for the view model.  This will be the plot
        /// that will be updated by the user.
        /// </summary>
        private PlotModel _plot;
        /// <summary>
        /// The plot for the view model.  This will be the plot
        /// that will be updated by the user.
        /// </summary>
        public PlotModel Plot
        {
            get { return _plot; }
            set
            {
                _plot = value;
                this.NotifyOfPropertyChange(() => this.Plot);
            }
        }

        /// <summary>
        /// Heatmap Plot View Model for Magnitude Velocity.
        /// </summary>
        public HeatmapPlotViewModel AmpltiduePlot { get; set; }

        #endregion

        #region Max Ensembles

        /// <summary>
        /// Maximum number of ensembles to display.
        /// </summary>
        public int MaxEnsembles
        {
            get { return _options.MaxEnsembles; }
            set
            {
                _options.MaxEnsembles = value;
                this.NotifyOfPropertyChange(() => this.MaxEnsembles);

                UpdateDatabaseOptions();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to close this VM.
        /// </summary>
        public ReactiveCommand<object> CloseVMCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public BackscatterViewModel(SubsystemDataConfig config)
            : base("Backscatter")
        {
            // Set Subsystem 
            _Config = config;
            //_displayCounter = 0;

            // Get PulseManager
            _pm = IoC.Get<PulseManager>();
            _pm.RegisterDisplayVM(this);

            // Get the Event Aggregator
            _events = IoC.Get<IEventAggregator>();

            _buffer = new ConcurrentQueue<DataSet.Ensemble>();

            // Initialize the thread
            _continue = true;
            _eventWaitData = new EventWaitHandle(false, EventResetMode.AutoReset);
            _processDataThread = new Thread(ProcessDataThread);
            _processDataThread.Name = string.Format("Backscatter View: {0}", config.DescString());
            _processDataThread.Start();

            // Get the options from the database
            GetOptionsFromDatabase();

            AmpltiduePlot = new HeatmapPlotViewModel(HeatmapPlotSeries.HeatmapPlotType.Amplitude, _options.AmplitudeSeriesOptions);
            AmpltiduePlot.AddSeries(_options.AmplitudeSeriesOptions);
            AmpltiduePlot.UpdateOptionsEvent += AmplitudeVelPlot_UpdateOptionsEvent;

            // Close the VM
            CloseVMCommand = ReactiveCommand.Create();
            CloseVMCommand.Subscribe(_ => _events.PublishOnUIThread(new CloseVmEvent(_Config)));

            _events.Subscribe(this);
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {
            // Wake up the thread to process data
            _continue = false;
            _eventWaitData.Set();

            // Unsubscribe
            AmpltiduePlot.UpdateOptionsEvent -= AmplitudeVelPlot_UpdateOptionsEvent;
        }

        #region Options

        /// <summary>
        /// Get the options for this subsystem display
        /// from the database.  If the options have not
        /// been set to the database yet, default values 
        /// will be used.
        /// </summary>
        private void GetOptionsFromDatabase()
        {
            _options = _pm.GetBackscatterOptions();

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
            this.NotifyOfPropertyChange(() => this.MaxEnsembles);

            //// Pass the Contour Plot options to the contour plot
            //if (_ContourPlot != null)
            //{
            //    _ContourPlot.ColormapBrushSelection = _options.PlotColorMap;
            //    _ContourPlot.MaxEnsembles = _options.DisplayMaxEnsembles;
            //    _ContourPlot.MinValue = _options.ContourMinimumValue;
            //    _ContourPlot.MaxValue = _options.ContourMaximumValue;
            //}
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
            _pm.UpdateBackscatterOptions(_options);
        }

        #endregion

        #region Plots

        /// <summary>
        /// Clear the plot.
        /// </summary>
        public void ClearPlots()
        {
            AmpltiduePlot.ClearIncomingData();
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

            // Limit how often the dsiplay updates
            //if ((++_displayCounter % 5) == 0)
            //{
                // Wake up the thread to process data
                _eventWaitData.Set();

                //_displayCounter = 0;
            //}
        }

        /// <summary>
        /// Only update the contour plot and timeseries.  This will need each ensemble.
        /// The profile plots only need the last ensemble. 
        /// </summary>
        /// <param name="ensembles">Event that contains the Ensembles to display.</param>
        /// <param name="maxEnsembles">Maximum ensembles to display.</param>
        public void DisplayBulkData(Cache<long, DataSet.Ensemble> ensembles, int maxEnsembles)
        {
            //Task.Run(() => DisplayData(ensemble));
            //DisplayData(ensemble);
            try
            {
                AmpltiduePlot.AddIncomingDataBulk(ensembles, _Config.SubSystem, _Config, maxEnsembles);
            }
            catch (Exception e)
            {
                log.Error("Error adding ensemble to Heatmap plots.", e);
            }
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
                            Task.Run(() => AmpltiduePlot.AddIncomingData(ensemble, MaxEnsembles));
                        }
                        catch (Exception e)
                        {
                            log.Error("Error adding ensemble to plots.", e);
                        }
                    }
                }

            }

            return;
        }

        /// <summary>
        /// Only update the contour plot and timeseries.  This will need each ensemble.
        /// The profile plots only need the last ensemble. 
        /// </summary>
        /// <param name="ensemble">Ensemble to display.</param>
        public void DisplayBulkData(DataSet.Ensemble ensemble, int maxEnsembles)
        {
            //MaxEnsembles = maxEnsembles;
            ////Task.Run(() => DisplayData(ensemble));
            DisplayData(ensemble);
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
        /// Bulk ensemble event.
        /// </summary>
        /// <param name="ensEvent"></param>
        public override void Handle(BulkEnsembleEvent ensEvent)
        {
            // Do Nothing
        }

        /// <summary>
        /// Update the options.
        /// </summary>
        /// <param name="options">Options to update.</param>
        void EastVelPlot_UpdateOptionsEvent(HeatmapSeriesOptions options)
        {
            // Update the options
            _options.EastSeriesOptions = options;
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Update the options.
        /// </summary>
        /// <param name="options">Options to update.</param>
        void NorthVelPlot_UpdateOptionsEvent(HeatmapSeriesOptions options)
        {
            // Update the options
            _options.NorthSeriesOptions = options;
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Update the options.
        /// </summary>
        /// <param name="options">Options to update.</param>
        void VerticalVelPlot_UpdateOptionsEvent(HeatmapSeriesOptions options)
        {
            // Update the options
            _options.VerticalSeriesOptions = options;
            UpdateDatabaseOptions();
        }

        /// <summary>
        /// Update the options.
        /// </summary>
        /// <param name="options">Options to update.</param>
        void AmplitudeVelPlot_UpdateOptionsEvent(HeatmapSeriesOptions options)
        {
            // Update the options
            _options.AmplitudeSeriesOptions = options;
            UpdateDatabaseOptions();
        }

        #endregion
    }
}
