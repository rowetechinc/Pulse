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
 * 08/26/2013      RC          3.0.8       Initial coding
 * 04/16/2014      RC          3.2.4       Allow the user to set which datasets, bins and ensembles to export.
 *                                          Export PD0.
 * 08/07/2014      RC          4.0.0       Updated ReactiveCommand to 6.0.
 * 02/13/2015      RC          4.1.0       Fixed some bugs with missing flags.
 * 03/18/2015      RC          4.1.2       Added GageHeight dataset.
 * 10/27/2015      RC          4.3.1       Fixed setting the min and max ensemble index.
 * 05/18/2016      RC          4.4.7       When exporting data, use the VM and Screening options on the data.
 * 06/16/2016      RC          4.4.10      Check if the data is valid before converting.
 * 09/28/2016      RC          4.4.13      Added export of Velocity Vectors in CSV.
 * 02/08/2017      RC          4.5.0       Fix bug when new project is selected an no ensembles are in the project.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using ReactiveUI;
    using Caliburn.Micro;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Export a project to the selected format.
    /// </summary>
    public class ExportDataViewModel : PulseViewModel, IHandle<ProjectEvent>
    {

        #region Variables

        // Setup logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// EventAggregator to handle passing events.
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Pulse manager.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// ViewModel to screen the data.
        /// </summary>
        private ScreenDataBaseViewModel _screenDataVM;

        /// <summary>
        /// Beam coordiante transform.
        /// </summary>
        private const string XFORM_BEAM = "BEAM";

        /// <summary>
        /// Instrument coordiante transform.
        /// </summary>
        private const string XFORM_INSTRUMENT = "Instrument";

        /// <summary>
        /// Earth coordiante transform.
        /// </summary>
        private const string XFORM_EARTH = "Earth";

        /// <summary>
        /// Number of ensembles in the project.
        /// </summary>
        private int _numEns;
        #endregion

        #region Enum

        /// <summary>
        /// Export options.
        /// </summary>
        private enum Exporters
        {
            /// <summary>
            /// Export ensembles to CSV
            /// </summary>
            CSV,

            /// <summary>
            /// Export ensembles to matlab.
            /// </summary>
            Matlab,

            /// <summary>
            /// Export ensembles to PD0.
            /// </summary>
            PD0, 

            /// <summary>
            /// Export ensembles to Waves Matlab file.
            /// </summary>
            Waves
        }

        #endregion

        #region Properties

        /// <summary>
        /// Export options.
        /// </summary>
        private ExportOptions _Options;
        /// <summary>
        /// Export options.
        /// </summary>
        public ExportOptions Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                this.NotifyOfPropertyChange(() => this.Options);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Flag to know if the exporting is still running.
        /// </summary>
        private bool _IsExporting;
        /// <summary>
        /// Flag to know if the exporting is still running.
        /// </summary>
        public bool IsExporting
        {
            get { return _IsExporting; }
            set
            {
                _IsExporting = value;
                this.NotifyOfPropertyChange(() => this.IsExporting);
            }
        }

        #region Bins

        /// <summary>
        /// Minimum Bin.
        /// </summary>
        private int _MinimumBin;
        /// <summary>
        /// Minimum Bin.
        /// </summary>
        public int MinimumBin
        {
            get { return _MinimumBin; }
            set
            {
                _MinimumBin = value;
                this.NotifyOfPropertyChange(() => this.MinimumBin);
            }
        }

        /// <summary>
        /// Maximum Bin.
        /// </summary>
        private int _MaximumBin;
        /// <summary>
        /// Maximum Bin.
        /// </summary>
        public int MaximumBin
        {
            get { return _MaximumBin; }
            set
            {
                _MaximumBin = value;
                this.NotifyOfPropertyChange(() => this.MaximumBin);
            }
        }

        #endregion

        #region Ensemble Numbers

        /// <summary>
        /// Minimum Ensemble Number.
        /// </summary>
        public uint MinEnsembleNumber
        {
            get { return _Options.MinEnsembleNumber; }
            set
            {
                _Options.MinEnsembleNumber = value;
                this.NotifyOfPropertyChange(() => this.MinEnsembleNumber);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Maximum Ensemble Number.
        /// </summary>
        public uint MaxEnsembleNumber
        {
            get { return _Options.MaxEnsembleNumber; }
            set
            {
                _Options.MaxEnsembleNumber = value;
                this.NotifyOfPropertyChange(() => this.MaxEnsembleNumber);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Minimum Ensemble Number Entry.
        /// </summary>
        private uint _MinEnsembleNumberEntry;
        /// <summary>
        /// Minimum Ensemble Number Entry.
        /// </summary>
        public uint MinEnsembleNumberEntry
        {
            get { return _MinEnsembleNumberEntry; }
            set
            {
                _MinEnsembleNumberEntry = value;
                this.NotifyOfPropertyChange(() => this.MinEnsembleNumberEntry);
            }
        }

        /// <summary>
        /// Maximum Ensemble Number Entry.
        /// </summary>
        private uint _MaxEnsembleNumberEntry;
        /// <summary>
        /// Maximum Ensemble Number Entry.
        /// </summary>
        public uint MaxEnsembleNumberEntry
        {
            get { return _MaxEnsembleNumberEntry; }
            set
            {
                _MaxEnsembleNumberEntry = value;
                this.NotifyOfPropertyChange(() => this.MaxEnsembleNumberEntry);
            }
        }

        #endregion

        #region Amplitude

        /// <summary>
        /// Amplitude enabled/disabled.
        /// </summary>
        public bool IsAmplitudeDataSetOn
        {
            get { return _Options.IsAmplitudeDataSetOn; }
            set
            {
                _Options.IsAmplitudeDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsAmplitudeDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Amplitude minimum bin.
        /// </summary>
        public int AmplitudeMinBin
        {
            get { return _Options.AmplitudeMinBin; }
            set
            {
                _Options.AmplitudeMinBin = value;
                this.NotifyOfPropertyChange(() => this.AmplitudeMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Amplitude maximum bin.
        /// </summary>
        public int AmplitudeMaxBin
        {
            get { return _Options.AmplitudeMaxBin; }
            set
            {
                _Options.AmplitudeMaxBin = value;
                this.NotifyOfPropertyChange(() => this.AmplitudeMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Beam Velocity

        /// <summary>
        /// Beam Velocity enabled/disabled.
        /// </summary>
        public bool IsBeamVelocityDataSetOn
        {
            get { return _Options.IsBeamVelocityDataSetOn; }
            set
            {
                _Options.IsBeamVelocityDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsBeamVelocityDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Beam Velocity minimum bin.
        /// </summary>
        public int BeamMinBin
        {
            get { return _Options.BeamMinBin; }
            set
            {
                _Options.BeamMinBin = value;
                this.NotifyOfPropertyChange(() => this.BeamMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Beam Velocity maximum bin.
        /// </summary>
        public int BeamMaxBin
        {
            get { return _Options.BeamMaxBin; }
            set
            {
                _Options.BeamMaxBin = value;
                this.NotifyOfPropertyChange(() => this.BeamMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Instrument Velocity

        /// <summary>
        /// Instrument Velocity enabled/disabled.
        /// </summary>
        public bool IsInstrumentVelocityDataSetOn
        {
            get { return _Options.IsInstrumentVelocityDataSetOn; }
            set
            {
                _Options.IsInstrumentVelocityDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsInstrumentVelocityDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Instrument Velocity minimum bin.
        /// </summary>
        public int InstrumentMinBin
        {
            get { return _Options.InstrumentMinBin; }
            set
            {
                _Options.InstrumentMinBin = value;
                this.NotifyOfPropertyChange(() => this.InstrumentMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Instrument Velocity maximum bin.
        /// </summary>
        public int InstrumentMaxBin
        {
            get { return _Options.InstrumentMaxBin; }
            set
            {
                _Options.InstrumentMaxBin = value;
                this.NotifyOfPropertyChange(() => this.InstrumentMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Earth Velocity

        /// <summary>
        /// Earth Velocity enabled/disabled.
        /// </summary>
        public bool IsEarthVelocityDataSetOn
        {
            get { return _Options.IsEarthVelocityDataSetOn; }
            set
            {
                _Options.IsEarthVelocityDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsEarthVelocityDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Earth Velocity minimum bin.
        /// </summary>
        public int EarthMinBin
        {
            get { return _Options.EarthMinBin; }
            set
            {
                _Options.EarthMinBin = value;
                this.NotifyOfPropertyChange(() => this.EarthMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Earth Velocity maximum bin.
        /// </summary>
        public int EarthMaxBin
        {
            get { return _Options.EarthMaxBin; }
            set
            {
                _Options.EarthMaxBin = value;
                this.NotifyOfPropertyChange(() => this.EarthMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Velocity Vector

        /// <summary>
        /// Velocity Vector enabled/disabled.
        /// </summary>
        public bool IsVelocityVectorDataSetOn
        {
            get { return _Options.IsVelocityVectorDataSetOn; }
            set
            {
                _Options.IsVelocityVectorDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsVelocityVectorDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Velocity Vector minimum bin.
        /// </summary>
        public int VelVectorMinBin
        {
            get { return _Options.VelVectorMinBin; }
            set
            {
                _Options.VelVectorMinBin = value;
                this.NotifyOfPropertyChange(() => this.VelVectorMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Velocity Vector maximum bin.
        /// </summary>
        public int VelVectorMaxBin
        {
            get { return _Options.VelVectorMaxBin; }
            set
            {
                _Options.VelVectorMaxBin = value;
                this.NotifyOfPropertyChange(() => this.VelVectorMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Coordinate Transform

        /// <summary>
        /// Coordinate Transform.
        /// </summary>
        private string _CoordinateTransform;
        /// <summary>
        /// Coordinate Transform.
        /// </summary>
        public string CoordinateTransform
        {
            get { return _CoordinateTransform; }
            set
            {
                _CoordinateTransform = value;
                this.NotifyOfPropertyChange(() => this.CoordinateTransform);

                // Beam
                if (_CoordinateTransform == XFORM_BEAM)
                {
                    _Options.CoordinateTransform = PD0.CoordinateTransforms.Coord_Beam;
                }

                // Instrument
                if (_CoordinateTransform == XFORM_INSTRUMENT)
                {
                    _Options.CoordinateTransform = PD0.CoordinateTransforms.Coord_Instrument;
                }

                // Earth
                if (_CoordinateTransform == XFORM_EARTH)
                {
                    _Options.CoordinateTransform = PD0.CoordinateTransforms.Coord_Earth;
                }

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// List of all the coordinate Transforms to choose.
        /// </summary>
        public List<string> CoordinateTransformList { get; set; }

        #endregion

        #region Correlation

        /// <summary>
        /// Correlation enabled/disabled.
        /// </summary>
        public bool IsCorrelationDataSetOn
        {
            get { return _Options.IsCorrelationDataSetOn; }
            set
            {
                _Options.IsCorrelationDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsCorrelationDataSetOn);
                
                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Correlation minimum bin.
        /// </summary>
        public int CorrelationMinBin
        {
            get { return _Options.CorrelationMinBin; }
            set
            {
                _Options.CorrelationMinBin = value;
                this.NotifyOfPropertyChange(() => this.CorrelationMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Correlation maximum bin.
        /// </summary>
        public int CorrelationMaxBin
        {
            get { return _Options.CorrelationMaxBin; }
            set
            {
                _Options.CorrelationMaxBin = value;
                this.NotifyOfPropertyChange(() => this.CorrelationMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Good Beam

        /// <summary>
        /// Good Beam enabled/disabled.
        /// </summary>
        public bool IsGoodBeamDataSetOn
        {
            get { return _Options.IsGoodBeamDataSetOn; }
            set
            {
                _Options.IsGoodBeamDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsGoodBeamDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Good Beam minimum bin.
        /// </summary>
        public int GoodBeamMinBin
        {
            get { return _Options.GoodBeamMinBin; }
            set
            {
                _Options.GoodBeamMinBin = value;
                this.NotifyOfPropertyChange(() => this.GoodBeamMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Good Beam maximum bin.
        /// </summary>
        public int GoodBeamMaxBin
        {
            get { return _Options.GoodBeamMaxBin; }
            set
            {
                _Options.GoodBeamMaxBin = value;
                this.NotifyOfPropertyChange(() => this.GoodBeamMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Good Earth

        /// <summary>
        /// Good Earth enabled/disabled.
        /// </summary>
        public bool IsGoodEarthDataSetOn
        {
            get { return _Options.IsGoodEarthDataSetOn; }
            set
            {
                _Options.IsGoodEarthDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsGoodEarthDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Good Earth minimum bin.
        /// </summary>
        public int GoodEarthMinBin
        {
            get { return _Options.GoodEarthMinBin; }
            set
            {
                _Options.GoodEarthMinBin = value;
                this.NotifyOfPropertyChange(() => this.GoodEarthMinBin);

                // Save Options
                SaveOptions();
            }
        }

        /// <summary>
        /// Good Earth maximum bin.
        /// </summary>
        public int GoodEarthMaxBin
        {
            get { return _Options.GoodEarthMaxBin; }
            set
            {
                _Options.GoodEarthMaxBin = value;
                this.NotifyOfPropertyChange(() => this.GoodEarthMaxBin);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Bottom Track

        /// <summary>
        /// Bottom Track enabled/disabled.
        /// </summary>
        public bool IsBottomTrackDataSetOn
        {
            get { return _Options.IsBottomTrackDataSetOn; }
            set
            {
                _Options.IsBottomTrackDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsBottomTrackDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Earth Water Mass

        /// <summary>
        /// Earth Water Mass enabled/disabled.
        /// </summary>
        public bool IsEarthWaterMassDataSetOn
        {
            get { return _Options.IsEarthWaterMassDataSetOn; }
            set
            {
                _Options.IsEarthWaterMassDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsEarthWaterMassDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Instrument Water Mass

        /// <summary>
        /// Instrument Water Mass enabled/disabled.
        /// </summary>
        public bool IsInstrumentWaterMassDataSetOn
        {
            get { return _Options.IsInstrumentWaterMassDataSetOn; }
            set
            {
                _Options.IsInstrumentWaterMassDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsInstrumentWaterMassDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Range Tracking

        /// <summary>
        /// Range Tracking enabled/disabled.
        /// </summary>
        public bool IsRangeTrackingDataSetOn
        {
            get { return _Options.IsRangeTrackingDataSetOn; }
            set
            {
                _Options.IsRangeTrackingDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsRangeTrackingDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Gage Height

        /// <summary>
        /// Gage Height enabled/disabled.
        /// </summary>
        public bool IsGageHeightDataSetOn
        {
            get { return _Options.IsGageHeightDataSetOn; }
            set
            {
                _Options.IsGageHeightDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsGageHeightDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region NMEA

        /// <summary>
        /// NMEA enabled/disabled.
        /// </summary>
        public bool IsNmeaDataSetOn
        {
            get { return _Options.IsNmeaDataSetOn; }
            set
            {
                _Options.IsNmeaDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsNmeaDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Profile Engineering

        /// <summary>
        /// Profile Engineering enabled/disabled.
        /// </summary>
        public bool IsProfileEngineeringDataSetOn
        {
            get { return _Options.IsProfileEngineeringDataSetOn; }
            set
            {
                _Options.IsProfileEngineeringDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsProfileEngineeringDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region Bottom Track Engineering

        /// <summary>
        /// Bottom Track Engineering enabled/disabled.
        /// </summary>
        public bool IsBottomTrackEngineeringDataSetOn
        {
            get { return _Options.IsBottomTrackEngineeringDataSetOn; }
            set
            {
                _Options.IsBottomTrackEngineeringDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsBottomTrackEngineeringDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region System Setup

        /// <summary>
        /// System Setup enabled/disabled.
        /// </summary>
        public bool IsSystemSetupDataSetOn
        {
            get { return _Options.IsSystemSetupDataSetOn; }
            set
            {
                _Options.IsSystemSetupDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsSystemSetupDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region ADCP GPS Data

        /// <summary>
        /// ADCP GPS Data enabled/disabled.
        /// </summary>
        public bool IsAdcpGpsDataSetOn
        {
            get { return _Options.IsAdcpGpsDataSetOn; }
            set
            {
                _Options.IsAdcpGpsDataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsAdcpGpsDataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region GPS 1

        /// <summary>
        /// GPS 1 Data enabled/disabled.
        /// </summary>
        public bool IsGps1DataSetOn
        {
            get { return _Options.IsGps1DataSetOn; }
            set
            {
                _Options.IsGps1DataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsGps1DataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region GPS 2

        /// <summary>
        /// GPS 2 Data enabled/disabled.
        /// </summary>
        public bool IsGps2DataSetOn
        {
            get { return _Options.IsGps2DataSetOn; }
            set
            {
                _Options.IsGps2DataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsGps2DataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region NMEA 1

        /// <summary>
        /// NMEA 1 Data enabled/disabled.
        /// </summary>
        public bool IsNmea1DataSetOn
        {
            get { return _Options.IsNmea1DataSetOn; }
            set
            {
                _Options.IsNmea1DataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsNmea1DataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #region NMEA 2

        /// <summary>
        /// NMEA 2 Data enabled/disabled.
        /// </summary>
        public bool IsNmea2DataSetOn
        {
            get { return _Options.IsNmea2DataSetOn; }
            set
            {
                _Options.IsNmea2DataSetOn = value;
                this.NotifyOfPropertyChange(() => this.IsNmea2DataSetOn);

                // Save Options
                SaveOptions();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to export the given project to a CSV file.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ExportCsvCommand { get; protected set; }

        /// <summary>
        /// Command to export the given project to a Matlab file.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ExportMatlabCommand { get; protected set; }

        /// <summary>
        /// Command to export the given project to a PD0 file.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ExportPd0Command { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public ExportDataViewModel()
            : base ("ExportViewModel")
        {
            // Initialize values
            _pm = IoC.Get<PulseManager>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.Subscribe(this);
            _screenDataVM = IoC.Get<ScreenDataBaseViewModel>();                 // Get ScreenData VM

            IsExporting = false;

            _numEns = 0;
            if (_pm.IsProjectSelected)
            {
                ProjectName = _pm.SelectedProject.ProjectName;
                _numEns = _pm.SelectedProject.GetNumberOfEnsembles();
            }

            // Bins
            MinimumBin = 0;
            if (_pm.IsProjectSelected && _numEns > 0)
            {
                MaximumBin = _pm.SelectedProject.GetFirstEnsemble().EnsembleData.NumBins - 1;
            }
            else
            {
                MaximumBin = DataSet.Ensemble.MAX_NUM_BINS;
            }

            // Ensemble Numbers
            MinEnsembleNumberEntry = 0;
            if (_pm.IsProjectSelected)
            {
                MaxEnsembleNumberEntry = (uint)_numEns - 1;
            }
            else
            {
                MaxEnsembleNumberEntry = 0;
            }

            // Coordinate Transform list
            CoordinateTransformList = new List<string>();
            CoordinateTransformList.Add(XFORM_BEAM);
            CoordinateTransformList.Add(XFORM_INSTRUMENT);
            CoordinateTransformList.Add(XFORM_EARTH);

            // Set the options
            _Options = _pm.AppConfiguration.GetExportDataOptions();
            UpdateProperties();

            // Check if options are set
            CheckOptions();

            // Export to CSV Command
            ExportCsvCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x._numEns, x => x.Value > 0),
                                                                    _ => ExportCsv());         // Load if there are ensembles in the project

            // Export to Matlab command
            ExportMatlabCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x._numEns, x => x.Value > 0),
                                                                    _ => ExportMatlab());      // Load if there are ensembles in the project

            // Export to PD0 command
            ExportPd0Command = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x._numEns, x => x.Value > 0),
                                                                    _ => ExportPd0());         // Load if there are ensembles in the project
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {

        }

        #region CSV

        /// <summary>
        /// Execute the export async.
        /// </summary>
        private async Task ExportCsv()
        {
            await Task.Run(() => ExecuteExportCsv());
        }

        /// <summary>
        /// Export in CSV format.
        /// </summary>
        private void ExecuteExportCsv()
        {
            // Save the current options
            _pm.AppConfiguration.SaveExportDataOptions(_Options);
            
            // Start the export process
            // Export in CSV format
            Export(Exporters.CSV);
        }

        #endregion

        #region Matlab

        /// <summary>
        /// Execute the export async.
        /// </summary>
        private async Task ExportMatlab()
        {
            await Task.Run(() => ExecuteExportMatlab());
        }

        /// <summary>
        /// Export the selected project as matlab files.
        /// </summary>
        private void ExecuteExportMatlab()
        {
            // Save the current options
            _pm.AppConfiguration.SaveExportDataOptions(_Options);

            // Start the export process
            Export(Exporters.Matlab);
        }

        #endregion

        #region PD0

        /// <summary>
        /// Execute the export async.
        /// </summary>
        private async Task ExportPd0()
        {
            await Task.Run(() => ExecuteExportPd0());
        }

        /// <summary>
        /// Export the selected project as PD0 files.
        /// </summary>
        private void ExecuteExportPd0()
        {
            // Save the current options
            _pm.AppConfiguration.SaveExportDataOptions(_Options);

            // Start the export process
            Export(Exporters.PD0);
        }

        #endregion

        #region Export

        /// <summary>
        /// Export the the data based off the exporter chosen.
        /// </summary>
        /// <param name="exporter">Exporter chosen.</param>
        private void Export(Exporters exporter)
        {
            // No project was selected
            if (_pm.IsProjectSelected)
            {
                IsExporting = true;

                string dir = _pm.SelectedProject.ProjectFolderPath + @"\";
                string filename = "";
                //string file = dir + filename;

                // Determine which exporter to use
                IExporterWriter writer = null;
                switch(exporter)
                {
                    case Exporters.CSV:
                        writer = new CsvExporterWriter();
                        filename = _pm.SelectedProject.ProjectName + "_export.csv";
                        break;
                    case Exporters.Matlab:
                        writer = new MatlabExporterWriter();
                        filename = _pm.SelectedProject.ProjectName;
                        break;
                    case Exporters.PD0:
                        writer = new Pd0ExporterWriter();
                        filename = _pm.SelectedProject.ProjectName + "_export.pd0";
                        break;
                    default:
                        break;
                }


                if (writer != null)
                {
                    AdcpDatabaseReader dbReader = new AdcpDatabaseReader();
                    int size = dbReader.GetNumberOfEnsembles(_pm.SelectedProject);

                    uint x = 0;
                    try
                    {
                        // Display the busy indicator
                        //IsBusyIndicator = true;
                        // Create the directory if it does not exist
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        // Open the file
                        writer.Open(dir, filename, _Options);

                        // Check the max value
                        if (MaxEnsembleNumber > _pm.SelectedProject.GetNumberOfEnsembles())
                        {
                            MaxEnsembleNumber = (uint)_pm.SelectedProject.GetNumberOfEnsembles();
                        }
                        // Check the min value
                        uint count = (MaxEnsembleNumber - MinEnsembleNumber) + 1;
                        if(count < 0)
                        {
                            count = 0;
                        }

                        for (x = MinEnsembleNumber; x < count; x++)
                        {
                            // Get the data from the reader
                            DataSet.Ensemble data = dbReader.GetEnsemble(_pm.SelectedProject, x);
                            if (data != null)
                            {
                                // Vessel Mount Options
                                VesselMountScreen(ref data);

                                // Screen the data
                                _screenDataVM.ScreenData(ref data, AdcpCodec.CodecEnum.Binary);

                                // Verify go data
                                if (data.IsEnsembleAvail && data.EnsembleData.EnsembleNumber != 0)
                                {
                                    try
                                    {
                                        // Write the data to the file
                                        writer.Write(data);
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error(string.Format("Error writing file {0} {1}", dir + filename, x), e);
                                    }
                                }
                            }
                        }

                        // Close the writer and save the latest options
                        _Options = writer.Close();
                        //SaveDatabaseOptions();

                        // Remove the busy indicator
                        IsExporting = false;

                    }
                    catch(IOException ex)
                    {
                        System.Windows.MessageBox.Show("Export file is open or check file permissions for the folder.\n" + string.Format("{0}", dir + filename), "Export IO Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        log.Error(string.Format("IO Error exporting file {0} {1}", dir + filename, x), ex);
                        if (writer != null)
                        {
                            writer.Close();
                            IsExporting = false;
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("Error exporting file {0} {1}", dir + filename, x), e);
                        if (writer != null)
                        {
                            writer.Close();
                            IsExporting = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region Vessel Mount Screen Data

        /// <summary>
        /// Screen the ensemble with the given options.
        /// </summary>
        /// <param name="ensemble">Ensemble to screen.</param>
        private void VesselMountScreen(ref DataSet.Ensemble ensemble)
        {
            // Vessel Mount Options
            if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.VesselMountOptions != null)
            {
                VesselMount.VesselMountScreen.Screen(ref ensemble, _pm.SelectedProject.Configuration.VesselMountOptions);
            }
            else
            {
                VesselMount.VesselMountScreen.Screen(ref ensemble, _pm.AppConfiguration.GetVesselMountOptions());
            }

        }

        #endregion

        #region Update Properties

        /// <summary>
        /// Update all the properties.
        /// </summary>
        private void UpdateProperties()
        {
            this.NotifyOfPropertyChange();
        }

        #endregion

        #region Database

        /// <summary>
        /// Save the Export Data Options to the project.
        /// </summary>
        private void SaveDatabaseOptions()
        {
            _pm.AppConfiguration.SaveExportDataOptions(_Options);
            UpdateProperties();
        }

        #endregion

        #region Options

        /// <summary>
        /// Save the options.
        /// </summary>
        private void SaveOptions()
        {
            _pm.AppConfiguration.SaveExportDataOptions(_Options);
        }

        /// <summary>
        /// Check if any options have been set.  This
        /// will set all the options to the project settings.
        /// </summary>
        private void CheckOptions()
        {
            // Check if the options were set for Max Ensemble
            if (MaxEnsembleNumber <= 0)
            {
                MaxEnsembleNumber = MaxEnsembleNumberEntry;
            }

            if (AmplitudeMaxBin <= 0)
            {
                AmplitudeMaxBin = MaximumBin;
            }

            if (BeamMaxBin <= 0)
            {
                BeamMaxBin = MaximumBin;
            }

            if (InstrumentMaxBin <= 0)
            {
                InstrumentMaxBin = MaximumBin;
            }

            if (EarthMaxBin <= 0)
            {
                EarthMaxBin = MaximumBin;
            }

            if (VelVectorMaxBin <= 0)
            {
                VelVectorMaxBin = MaximumBin;
            }

            if (CorrelationMaxBin <= 0)
            {
                CorrelationMaxBin = MaximumBin;
            }

            if (GoodBeamMaxBin <= 0)
            {
                GoodBeamMaxBin = MaximumBin;
            }

            if (GoodEarthMaxBin <= 0)
            {
                GoodEarthMaxBin = MaximumBin;
            }

            // Coordinate Transform
            switch(_Options.CoordinateTransform)
            {
                case PD0.CoordinateTransforms.Coord_Beam:
                    CoordinateTransform = XFORM_BEAM;
                    break;
                case PD0.CoordinateTransforms.Coord_Instrument:
                    CoordinateTransform = XFORM_INSTRUMENT;
                    break;
                case PD0.CoordinateTransforms.Coord_Earth:
                    CoordinateTransform = XFORM_EARTH;
                    break;
                default:
                    CoordinateTransform = XFORM_EARTH;
                    break;
            }
        }


        #endregion

        #region EventHandler

        /// <summary>
        /// EventHandler for receiving selected project
        /// changes.
        /// </summary>
        /// <param name="message">Event containing the new selected project.</param>
        public void Handle(ProjectEvent message)
        {
            _Options = _pm.AppConfiguration.GetExportDataOptions();

            // Get the number of ensembles
            MaxEnsembleNumberEntry = (uint)message.Project.GetNumberOfEnsembles();

            if (MaxEnsembleNumberEntry > 0)
            {
                // Get the maximum number of bins
                MaximumBin = message.Project.GetFirstEnsemble().EnsembleData.NumBins;
            }
        }

        #endregion
    }
}
