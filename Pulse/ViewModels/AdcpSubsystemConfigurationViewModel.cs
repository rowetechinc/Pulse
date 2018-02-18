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
 * 10/29/2013      RC          3.2.0      Initial coding
 * 12/02/2013      RC          3.2.0      Added UpdateBatteryType().
 * 05/23/2014      RC          3.2.4      Fixed a bug updating the Predictor.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 10/19/2016      RC          4.4.13     Made the default BT mode Narrowband in prediction model.
 * 03/30/2017      RC          4.5.2      Fixed data size in wizard for burst deployments.
 * 01/18/2017      RC          4.7.2      Updated the Prediction Model.
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
    /// TODO: Update summary.
    /// </summary>
    public class AdcpSubsystemConfigurationViewModel : PulseViewModel
    {

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Pulse manager.
        /// </summary>
        private readonly PulseManager _pm;

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        /// <summary>
        /// Used to relay data back to the parent VM.
        /// </summary>
        private AdcpConfigurationViewModel ConfigVM;


        #endregion

        #region Properties

        /// <summary>
        /// String that represents the AdcpSubsystemConfig in the
        /// Configuration dictionary.  This is used to lookup the configuration
        /// from the selected project.
        /// </summary>
        public string ConfigKey { get; protected set; }

        #region CWPP

        /// <summary>
        /// Number of Pings. CWPP>
        /// </summary>
        public ushort CWPP
        {
            get 
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPP;
                }

                return 0;
            }
        }

        #endregion

        #region CWPTBP

        /// <summary>
        /// Time Between Pings.  CWPTBP.  In seconds.
        /// </summary>
        public float CWPTBP
        {
            get 
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                { 
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPTBP; 
                }

                return 0;
            }
        }

        #endregion

        #region CWPBS

        /// <summary>
        /// Bin size in meters.
        /// </summary>
        public float CWPBS
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBS;
                }

                return 0;
            }
        }

        #endregion

        #region CWPBN

        /// <summary>
        /// Number of bins.
        /// </summary>
        public float CWPBN
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBN;
                }

                return 0;
            }
        }

        #endregion

        #region CWPBL

        /// <summary>
        /// Blank.
        /// </summary>
        public float CWPBL
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBL;
                }

                return 0;
            }
        }

        #endregion

        #region CWPBB_TransmitPulseType

        /// <summary>
        /// Broandband pulse type.
        /// </summary>
        public string CWPBB_TransmitPulseType
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.GetCwpbbTransmitPulseType();
                }

                return "";
            }
        }

        #endregion

        #region CWPBB_LagLength

        /// <summary>
        /// Broandband lag length.
        /// </summary>
        public float CWPBB_LagLength
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_LagLength;
                }

                return 0;
            }
        }

        #endregion

        #region CBTON

        /// <summary>
        /// Bottom Track On or off.
        /// </summary>
        public string CBTON
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return(MathHelper.BoolToOnOffStr(_pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON));
                }

                return "OFF";
            }
        }

        #endregion

        #region CBI

        /// <summary>
        /// Time period for the burst interval.
        /// </summary>
        public RTI.Commands.TimeValue CBI_BurstInterval
        {
            get
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval;
                }

                return new RTI.Commands.TimeValue();
            }
        }

        /// <summary>
        /// Number of ensembles within a burst.
        /// </summary>
        public ushort CBI_NumEnsembles
        {
            get 
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_NumEnsembles;
                }

                return 0;
            }
        }

        /// <summary>
        /// Interleave the burst interval subsystem configurations flag.
        /// </summary>
        public string CBI_BurstPairFlag
        {
            get 
            {
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    return MathHelper.BoolToOnOffStr(_pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstPairFlag);
                }

                return "OFF"; 
            }
        }

        #endregion

        #region RangeVM 

        /// <summary>
        /// ViewModel for the ADCP Range Planner
        /// </summary>
        public AdcpRangePlannerViewModel RangeVM { get; set; }

        #endregion

        #region Predictor

        /// <summary>
        /// Number of bytes converted to best scale for the amount
        /// of data that will be written for the given deployment.
        /// </summary>
        public string DataSize
        {
            get
            {
                return MathHelper.MemorySizeString(GetDataSize());
            }
        }

        /// <summary>
        /// Predicted Bottom Track Range truncated.
        /// </summary>
        private double _PredictedBottomRange;
        /// <summary>
        /// Predicted Bottom Track Range truncated.
        /// </summary>
        public double PredictedBottomRange
        {
            get
            {
                return _PredictedBottomRange;
            }
            set
            {
                _PredictedBottomRange = value;
                this.NotifyOfPropertyChange(() => this.PredictedBottomRange);
                this.NotifyOfPropertyChange(() => this.PredictedBottomRangeStr);
            }
        }

        /// <summary>
        /// Predicted Bottom Track Range truncated.
        /// </summary>
        public string PredictedBottomRangeStr
        {
            get
            {
                return _PredictedBottomRange.ToString("0.0000"); ;
            }
        }

        /// <summary>
        /// Predicted Profile Range truncated.
        /// </summary>
        private double _PredictedProfileRange;
        /// <summary>
        /// Predicted Profile Range truncated.
        /// </summary>
        public double PredictedProfileRange
        {
            get
            {
                return _PredictedProfileRange;
            }
            set
            {
                _PredictedProfileRange = value;
                this.NotifyOfPropertyChange(() => this.PredictedProfileRange);
                this.NotifyOfPropertyChange(() => this.PredictedProfileRangeStr);
            }
        }

        /// <summary>
        /// Predicted Bottom Track Range truncated.
        /// </summary>
        public string PredictedProfileRangeStr
        {
            get
            {
                return _PredictedProfileRange.ToString("0.0000"); ;
            }
        }

        /// <summary>
        /// Predicted Maximum Velocity truncated.
        /// </summary>
        private string _MaximumVelocity;
        /// <summary>
        /// Predicted Maximum Velocity truncated.
        /// </summary>
        public string MaximumVelocity
        {
            get
            {
                return _MaximumVelocity;
            }
            set
            {
                _MaximumVelocity = value;
                this.NotifyOfPropertyChange(() => this.MaximumVelocity);
            }
        }

        /// <summary>
        /// Predicted Standard Deviation truncated.
        /// </summary>
        private string _StandardDeviation;
        /// <summary>
        /// Predicted Standard Deviation truncated.
        /// </summary>
        public string StandardDeviation
        {
            get
            {
                return _StandardDeviation;
            }
            set
            {
                _StandardDeviation = value;
                this.NotifyOfPropertyChange(() => this.StandardDeviation);
            }
        }

        /// <summary>
        /// Predicted First Bin Position truncated.
        /// </summary>
        private string _ProfileFirstBinPosition;
        /// <summary>
        /// Predicted First Bin Position truncated.
        /// </summary>
        public string ProfileFirstBinPosition
        {
            get
            {
                return _ProfileFirstBinPosition;
            }
            set
            {
                _ProfileFirstBinPosition = value;
                this.NotifyOfPropertyChange(() => this.ProfileFirstBinPosition);
            }
        }

        /// <summary>
        /// Predicted Number of Battery Packs truncated.
        /// </summary>
        private double _NumberBatteryPacks;
        /// <summary>
        /// Predicted Number of Battery Packs truncated.
        /// </summary>
        public double NumberBatteryPacks
        {
            get
            {
                return _NumberBatteryPacks;
            }
            set
            {
                _NumberBatteryPacks = value;
                this.NotifyOfPropertyChange(() => this.NumberBatteryPacks);
                this.NotifyOfPropertyChange(() => this.NumberBatteryPackStr);
            }
        }

        /// <summary>
        /// Predicted Number of Battery Packs truncated.
        /// </summary>
        public string NumberBatteryPackStr
        {
            get
            {
                return _NumberBatteryPacks.ToString("0.0000");
            }
        }

        /// <summary>
        /// Predicted Number of bytes in the deployment.
        /// </summary>
        private long _NumberBytes;
        /// <summary>
        /// Predicted Number of bytes in the deployment.
        /// </summary>
        public long NumberBytes
        {
            get
            {
                return _NumberBytes;
            }
            set
            {
                _NumberBytes = value;
                this.NotifyOfPropertyChange(() => this.NumberBytes);
            }
        }

        

        #endregion

        #endregion

        #region Commands

        /// <summary>
        ///  Command to remove the Subsystem configuration.
        /// </summary>
        public ReactiveCommand<object> RemoveCommand { get; protected set; }

        /// <summary>
        ///  Command to Edit the Subsystem configuration.
        /// </summary>
        public ReactiveCommand<object> EditCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <param name="config">AdcpSubsystemConfig for this view model.</param>
        /// <param name="configVM">Adcp Configuration view model.</param>
        public AdcpSubsystemConfigurationViewModel(AdcpSubsystemConfig config, AdcpConfigurationViewModel configVM)
            : base(string.Format("Subsystem Configuration {0}", config.ToString() ))
        {
            ConfigKey = config.ToString();
            ConfigVM = configVM;

            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Get the predictor from the selected project subsystem configuration
            // Create the VM
            //Predictor = config.Predictor as AdcpPredictor;
            //Predictor = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Predictor as AdcpPredictor;
            CalcPrediction();
            RangeVM = new AdcpRangePlannerViewModel(PredictedProfileRange, PredictedBottomRange);

            // Update the properties with the latest values
            UpdateProperties();

            // Remove configuration command
            RemoveCommand = ReactiveCommand.Create();
            RemoveCommand.Subscribe(_ => OnRemoveCommand());

            // Edit the configuration command
            EditCommand = ReactiveCommand.Create();
            EditCommand.Subscribe(param => OnEditCommand(param));
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Initialize

        /// <summary>
        /// Initialize the values from the Subsystem configuration.
        /// </summary>
        private void UpdateProperties()
        {
            // This will update all the properties
            this.NotifyOfPropertyChange(null);

            //this.NotifyOfPropertyChange(() => this.CWPP);
            //this.NotifyOfPropertyChange(() => this.CWPTBP);
            //this.NotifyOfPropertyChange(() => this.CWPBS);
            //this.NotifyOfPropertyChange(() => this.CWPBN);
            //this.NotifyOfPropertyChange(() => this.CWPBL);
            //this.NotifyOfPropertyChange(() => this.CWPBB_TransmitPulseType);
            //this.NotifyOfPropertyChange(() => this.CWPBB_LagLength);
            //this.NotifyOfPropertyChange(() => this.CBTON);
            //this.NotifyOfPropertyChange(() => this.CBI_BurstInterval);

            //this.NotifyOfPropertyChange(() => this.DataSize);
            //this.NotifyOfPropertyChange(() => this.NumberBatteryPackStr);
            //this.NotifyOfPropertyChange(() => this.PredictedBottomRangeStr);
            //this.NotifyOfPropertyChange(() => this.PredictedProfileRangeStr);
            //this.NotifyOfPropertyChange(() => this.MaximumVelocity);
            //this.NotifyOfPropertyChange(() => this.StandardDeviation);
            //this.NotifyOfPropertyChange(() => this.ProfileFirstBinPosition);

        }

        ///// <summary>
        ///// Update all the properties for the RangeVM.
        ///// </summary>
        //private void UpdateRangeVMProperties()
        //{
        //    if (_pm.IsProjectSelected)
        //    {
        //        // Get the predictor from the selected project subsystem configuration
        //        AdcpPredictor predictor = new AdcpPredictor(new AdcpPredictorUserInput());

        //        // Update all the properites
        //        RangeVM.WaterProfileRange = predictor.PredictedProfileRange;
        //        RangeVM.BottomTrackRange = predictor.PredictedBottomRange;
        //        RangeVM.WpBlank = predictor.CWPBL;
        //        RangeVM.WpFirstBinRange = predictor.ProfileFirstBinPosition;
        //        if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
        //        {
        //            RangeVM.DepthToBottom = _pm.SelectedProject.Configuration.Commands.SPOS_WaterDepth;
        //        }

        //    }
        //}

        #endregion

        #region Predictor

        /// <summary>
        /// Update the number of deployment days.  Then update all the properties
        /// to display the new values.
        /// </summary>
        /// <param name="days">New deployment days.</param>
        public void UpdateDeploymentDays(uint days)
        {
            //Predictor.DeploymentDuration = days;
            _pm.SelectedProject.Configuration.DeploymentOptions.Duration = days;

            //// This will update all the properties
            //UpdateProperties();

            // Calculate the latest prediction
            CalcPrediction();
        }

        /// <summary>
        /// Update the battery type.  Then update the all the properties 
        /// to display the new values.
        /// </summary>
        /// <param name="battType">New battery type.</param>
        public void UpdateBatteryType(DeploymentOptions.AdcpBatteryType battType)
        {
            //Predictor.BatteryType = battType;
            _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType = battType;

            //// This will update all the properties
            //UpdateProperties();

            // Calculate the latest prediction
            CalcPrediction();
        }

        /// <summary>
        /// Get the total data size in bytes.
        /// </summary>
        /// <returns>Deployment size in bytes.</returns>
        public long GetDataSize()
        {
            // Calculate the latest prediction
            CalcPrediction();

            return NumberBytes;
        }

        /// <summary>
        /// Setup the predictor.
        /// </summary>
        public void CalcPrediction()
        {
            PredictionModelInput predInput = new PredictionModelInput(_pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].SubsystemConfig.SubSystem);

            if (_pm.SelectedProject.Configuration.DeploymentOptions.Duration <= 0)
            {
                predInput.DeploymentDuration = 1;
            }
            else
            {
                predInput.DeploymentDuration = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
            }

            // Absorption
            predInput.Temperature = _pm.SelectedProject.Configuration.Commands.CWT;
            predInput.Salinity = _pm.SelectedProject.Configuration.Commands.CWS;
            predInput.XdcrDepth = _pm.SelectedProject.Configuration.Commands.CTD;

            predInput.CEI = _pm.SelectedProject.Configuration.Commands.CEI.ToSecondsD();

            predInput.CWPON = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPON;
            predInput.CBTON = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON;

            predInput.CBTTBP = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTTBP;
            predInput.CBTBB_TransmitPulseType = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTBB_Mode;
            //predInput.CBTBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;   // DEFAULT to narrowband because it autoswitches now

            predInput.CWPTBP = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPTBP;
            predInput.CWPBN = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBN;
            predInput.CWPBS = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBS;
            predInput.CWPBL = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBL;
            predInput.CWPBB_TransmitPulseType = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_TransmitPulseType;
            predInput.CWPBB_LagLength = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBB_LagLength;
            predInput.CWPP = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPP;

            predInput.BatteryType = _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType;

            predInput.CBI_BurstInterval = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval.ToSecondsD();
            predInput.CBI_SamplesPerBurst = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_NumEnsembles;
            predInput.CBI_IsInterleaved = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstPairFlag;

            // Create Prediction model
            PredictionModel Predictor = new PredictionModel();

            // Set the prediction values
            PredictionModel.PredictedRanges ranges = Predictor.GetPredictedRange(predInput);
            PredictedBottomRange = ranges.BottomTrack;
            PredictedProfileRange = ranges.WaterProfile;
            ProfileFirstBinPosition = ranges.FirstBinPosition.ToString("0.0000");

            double maxVel = Predictor.GetMaxVelocity(predInput);
            MaximumVelocity = maxVel.ToString("0.0000");

            double std = Predictor.GetStandardDeviation(predInput);
            StandardDeviation = std.ToString("0.0000");

            double numBatts = Predictor.BatteryUsage(predInput);
            NumberBatteryPacks = numBatts;

            if (CBI_NumEnsembles > 0)
            {
                NumberBytes = Predictor.GetDataStorageBurst(predInput);
            }
            else
            {
                NumberBytes = Predictor.GetDataStorage(predInput);
            }

            if (RangeVM != null)
            {
                // Update all the properites
                RangeVM.WaterProfileRange = ranges.WaterProfile;
                RangeVM.BottomTrackRange = ranges.BottomTrack;
                RangeVM.WpBlank = predInput.CWPBL;
                RangeVM.WpFirstBinRange = ranges.FirstBinPosition;
                if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
                {
                    RangeVM.DepthToBottom = predInput.XdcrDepth;
                }
            }

            //this.NotifyOfPropertyChange(() => this.CWPP);
            //this.NotifyOfPropertyChange(() => this.CWPTBP);
            //this.NotifyOfPropertyChange(() => this.CWPBS);
            //this.NotifyOfPropertyChange(() => this.CWPBN);
            //this.NotifyOfPropertyChange(() => this.CWPBL);
            //this.NotifyOfPropertyChange(() => this.CWPBB_TransmitPulseType);
            //this.NotifyOfPropertyChange(() => this.CWPBB_LagLength);
            //this.NotifyOfPropertyChange(() => this.CBTON);
            //this.NotifyOfPropertyChange(() => this.CBI_BurstInterval);

            //this.NotifyOfPropertyChange(() => this.DataSize);
            //this.NotifyOfPropertyChange(() => this.NumberBatteryPackStr);
            //this.NotifyOfPropertyChange(() => this.PredictedBottomRangeStr);
            //this.NotifyOfPropertyChange(() => this.PredictedProfileRangeStr);
            //this.NotifyOfPropertyChange(() => this.ProfileFirstBinPosition);
            //this.NotifyOfPropertyChange(() => this.MaximumVelocity);
            //this.NotifyOfPropertyChange(() => this.StandardDeviation);
        }

        #endregion

        #region OnRemoveCommand

        /// <summary>
        /// Remove the subsystem configuration from the project and
        /// from the UI.
        /// </summary>
        private void OnRemoveCommand()
        {
            // Ensure a project is selected and if the key exist in the dictionary
            if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
            {
                AdcpSubsystemConfig ssConfig = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey];

                _pm.SelectedProject.Configuration.RemoveAdcpSubsystemConfig(ssConfig);

                ConfigVM.RemoveConfiguration(this);
            }
        }

        #endregion

        #region OnEditCommand

        /// <summary>
        /// Move to the correct view based off the parameter given.
        /// </summary>
        /// <param name="param">Command to know which view to go to.</param>
        private void OnEditCommand(object param)
        {
            string cmd = param as string;
            if(cmd != null)
            {
                switch(cmd)
                {
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPBB:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BroadbandModeView, ConfigKey));
                        break;
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CBTON:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BottomTrackOnView, ConfigKey));
                        break;
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPBL:
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPBS:
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPBN:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BinsView, ConfigKey));
                        break;
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPP:
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CWPTBP:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.PingTimingView, ConfigKey));
                        break;
                    case RTI.Commands.AdcpSubsystemCommands.CMD_CBI:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BurstModeView, ConfigKey));
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
