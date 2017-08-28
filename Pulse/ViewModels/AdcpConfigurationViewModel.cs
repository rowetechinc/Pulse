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
 * 05/23/2014      RC          3.2.4      Fixed a bug updating the Predictor.
 * 06/02/2014      RC          3.3.0      Added saving the commands to a text file. 
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 02/17/2017      RC          4.5.1      Added Compass Calibration as a button.
 * 03/30/2017      RC          4.5.2      Fixed data size in wizard for burst deployments.
 * 
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
    using Xceed.Wpf.Toolkit;
    using System.Windows;
    using System.Threading.Tasks;

    /// <summary>
    /// Set the ADCP configuration for the selected project.
    /// </summary>
    public class AdcpConfigurationViewModel : PulseViewModel, IDeactivate
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

        /// <summary>
        /// List of all the Subsystem Configurations.
        /// </summary>
        public ReactiveList<AdcpSubsystemConfigurationViewModel> SubsystemConfigList { get; set; }

        #region Project


        /// <summary>
        /// Project Name.
        /// </summary>
        public string ProjectName
        {
            get 
            {
                if(_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.ProjectName; 
                }
                
                return "";
            }
        }

        #endregion

        #region CEPO

        /// <summary>
        /// CEPO command.  String
        /// </summary>
        public string CEPO
        {
            get 
            {
                if(_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.Commands.CEPO; 
                }
                
                return null;
            }
        }


        /// <summary>
        /// CEPO string.  This will describe the subsystems.
        /// </summary>
        public string CEPO_DescStr
        {
            get 
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.GetCepoDescString();
                }

                return "";
            }
        }

        #endregion

        #region CWS

        /// <summary>
        /// CWS command.  Salinity.
        /// </summary>
        public float CWS
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.Commands.CWS;
                }

                return 0;
            }
        }

        #endregion

        #region CHO

        /// <summary>
        /// CHO command.  Heading Offset or declination.
        /// </summary>
        public float CHO
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.Commands.CHO;
                }

                return 0;
            }
        }

        #endregion

        #region CEI

        /// <summary>
        /// CEI command.  Ensemble interval.  How often an ensemble will be output.
        /// </summary>
        public string CEI
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.Commands.CEI_ToString();
                }

                return "";
            }
        }

        #endregion

        #region CERECORD

        /// <summary>
        /// CERECORD command.
        /// </summary>
        public string CERECORD
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    if (_pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.Enable || _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.BT_Eng)
                    {
                        return "ON";
                    }
                    else
                    {
                        return "OFF";
                    }
                }

                return "";
            }
        }

        #endregion

        #region CETFP

        /// <summary>
        /// CETFP command.  Time of First ping.  Start time.
        /// </summary>
        public string CETFP
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.Commands.CETFP_ToString();
                }

                return "";
            }
        }

        #endregion

        #region Serial Number

        /// <summary>
        /// Serial Number stirng.
        /// </summary>
        public string SerialNumberStr
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.SerialNumber.ToString();
                }

                return "";
            }
        }

        #endregion

        #region Internal Storage

        /// <summary>
        /// Total size of the internal storage.
        /// </summary>
        public long InternalStorageTotal
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardTotalSize;
                }

                return 0;
            }
        }

        /// <summary>
        /// Amount of Internal Storage used.
        /// </summary>
        public long InternalStorageUsed
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardUsed;
                }

                return 0;
            }
        }

        /// <summary>
        /// String for the internal storage usage.
        /// </summary>
        public string InternalStorageStr
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return MathHelper.MemorySizeString(_pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardUsed) + " Used, " + MathHelper.MemorySizeString(_pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardTotalSize) + " Total";
                }

                return "";
            }
        }

        #endregion

        #region Storage

        /// <summary>
        /// Primary storage file path.
        /// </summary>
        public string PrimaryStoragePath
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.ProjectFolderPath;
                }

                return "";
            }
        }

        /// <summary>
        /// Backup storage file path.
        /// </summary>
        public string BackupStoragePath
        {
            get
            {
                if (_pm.IsProjectSelected)
                {
                    return _pm.SelectedProject.BackupProjectFolderPath;
                }

                return "";
            }
        }

        #endregion

        #region Scan

        /// <summary>
        /// Flag to know if the scan is still loading.
        /// </summary>
        private bool _IsScanning;
        /// <summary>
        /// Flag to know if the scan is still loading.
        /// </summary>
        public bool IsScanning
        {
            get { return _IsScanning; }
            set
            {
                _IsScanning = value;
                this.NotifyOfPropertyChange(() => this.IsScanning);
            }
        }

        #endregion

        #region Predictor

        /// <summary>
        /// Deployment days.
        /// </summary>
        private uint _DeploymentDays;
        /// <summary>
        /// Deployment days.
        /// </summary>
        public uint DeploymentDays
        {
            get { return _DeploymentDays; }
            set
            {
                _DeploymentDays = value;
                this.NotifyOfPropertyChange(() => this.DeploymentDays);

                // Update all the subsystems
                UpdateDeploymentDays(value);
            }
        }

        /// <summary>
        /// List of all the battery types.
        /// </summary>
        public List<DeploymentOptions.AdcpBatteryType> BatteryTypeList { get; set; }

        /// <summary>
        /// Battery Type Selected
        /// </summary>
        private DeploymentOptions.AdcpBatteryType _SelectedBatteryType;
        /// <summary>
        /// Battery Type Selected
        /// </summary>
        public DeploymentOptions.AdcpBatteryType SelectedBatteryType
        {
            get { return _SelectedBatteryType; }
            set
            {
                _SelectedBatteryType = value;
                this.NotifyOfPropertyChange(() => this.SelectedBatteryType);

                // Update all the subsystem configurations
                UpdateBatteryType(value);
            }
        }

        /// <summary>
        /// Number of battery packs for all the ADCP.
        /// </summary>
        private string _NumberBatteryPacks;
        /// <summary>
        /// Number of battery packs for all the ADCP.
        /// </summary>
        public string NumberBatteryPacks
        {
            get { return _NumberBatteryPacks; }
            set
            {
                _NumberBatteryPacks = value;
                this.NotifyOfPropertyChange(() => this.NumberBatteryPacks);
            }
        }

        /// <summary>
        /// Amount of data that will be stored with the deployment.
        /// </summary>
        private string _DataSize;
        /// <summary>
        /// Amount of data that will be stored with the deployment.
        /// </summary>
        public string DataSize
        {
            get { return _DataSize; }
            set
            {
                _DataSize = value;
                this.NotifyOfPropertyChange(() => this.DataSize);
            }
        }

        /// <summary>
        /// Predicted Storage used.
        /// </summary>
        private long _PredictedStorageUsed;
        /// <summary>
        /// Predicted Storage used.
        /// </summary>
        public long PredictedStorageUsed
        {
            get { return _PredictedStorageUsed; }
            set
            {
                _PredictedStorageUsed = value;
                this.NotifyOfPropertyChange(() => this.PredictedStorageUsed);
                this.NotifyOfPropertyChange(() => this.PredictedStorageStr);
            }
        }

        /// <summary>
        /// A string to state the used and total size of the internal storage based
        /// off the predictions.
        /// </summary>
        public string PredictedStorageStr
        {
            get
            {
                return string.Format("{0} of {1}", MathHelper.MemorySizeString(PredictedStorageUsed), MathHelper.MemorySizeString(InternalStorageTotal));
            }
        }

        #endregion

        #region Ping Model

        /// <summary>
        /// Ping Model VM.
        /// </summary>
        public PingModelViewModel PingModelVM { get; set; }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to scan the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ScanAdcpCommand { get; protected set; }

        /// <summary>
        /// Command to add a subsystem configuration to the ADCP configuration.
        /// </summary>
        public ReactiveCommand<object> AddSubsystemCommand { get; protected set; }

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
        /// Edit command to send to the proper page.
        /// </summary>
        public ReactiveCommand<object> EditCommand { get; protected set; }

        /// <summary>
        /// Save the commands to a text file.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SaveCmdsCommand { get; protected set; }

        /// <summary>
        /// Move to the Compass Cal Screen.
        /// </summary>
        public ReactiveCommand<object> CompassCalCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public AdcpConfigurationViewModel() 
            : base("Adcp Configuration")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();
            _adcpConnection = IoC.Get<AdcpConnection>();

            SubsystemConfigList = new ReactiveList<AdcpSubsystemConfigurationViewModel>();
            BatteryTypeList = DeploymentOptions.GetBatteryList();

            // Initialize the values
            InitializeValues();

            // Scan ADCP command
            ScanAdcpCommand = ReactiveCommand.CreateAsyncTask(_ => ScanConfiguration());

            // Add Subsystem Configuration
            AddSubsystemCommand = ReactiveCommand.Create();
            AddSubsystemCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.FrequencyView)));

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsScanning, x => !x.Value));
            NextCommand.Subscribe(_ => NextPage());

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Compass Cal coommand
            CompassCalCommand = ReactiveCommand.Create();
            CompassCalCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.CompassCalView)));

            // Edit the configuration command
            EditCommand = ReactiveCommand.Create();
            EditCommand.Subscribe(param => OnEditCommand(param));

            // Save the commands to a text file
            SaveCmdsCommand = ReactiveCommand.CreateAsyncTask(_ => SaveCommandsToFile());

            // Get the configuration from the project
            GetConfiguation();

            // Update the properites
            UpdateProperties();
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            ClearConfigurationList();
        }

        #region Next Page

        /// <summary>
        /// Determine which is the next page.
        /// </summary>
        private void NextPage()
        {
            // Based off mode, it will determine what is visible
            switch (_pm.SelectedProject.Configuration.DeploymentOptions.DeploymentMode)
            {
                case DeploymentOptions.AdcpDeploymentMode.VM:
                    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.NavSourcesView));
                    break;
                default:
                    //_events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SimpleCompassCalWizardView));
                    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DeployAdcpView));
                    break;
            }


        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void InitializeValues()
        {
            if (_pm.IsProjectSelected)
            {
                // Get the selected battery type
                SelectedBatteryType = _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType;
            }
            else
            {
                // Set default battery type
                SelectedBatteryType = DeploymentOptions.DEFAULT_BATTERY_TYPE;
            }

            if (_pm.SelectedProject.Configuration.DeploymentOptions.Duration <= 0)
            {
                DeploymentDays = 1;
            }
            else
            {
                DeploymentDays = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Get the configuration from the selected project.
        /// </summary>
        private void GetConfiguation()
        {
            if (_pm.IsProjectSelected)
            {
                // Create temp variables
                long dataSize = 0;
                double numberBattery = 0.0;

                foreach(AdcpSubsystemConfig ssCfg in _pm.SelectedProject.Configuration.SubsystemConfigDict.Values)
                {
                    // Create the VM and add it to the list
                    AdcpSubsystemConfigurationViewModel ssVM = new AdcpSubsystemConfigurationViewModel(ssCfg, this);
                    ssVM.Predictor.DeploymentDuration = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
                    ssVM.Predictor.BatteryType = _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType;

                    // Add the vm to the list
                    SubsystemConfigList.Add(ssVM);

                    // Accumluate the data sizes and number batteries for each subsystem configuration
                    dataSize += ssVM.GetDataSize();
                    numberBattery += ssVM.Predictor.NumberBatteryPacks;
                }

                // Set the combined values
                NumberBatteryPacks = numberBattery.ToString("0.00");
                PredictedStorageUsed = dataSize + InternalStorageUsed;
                DataSize = MathHelper.MemorySizeString(dataSize);
            }
        }

        /// <summary>
        /// Scan the ADCP for its current configuration.
        /// </summary>
        private async Task ScanConfiguration()
        {
            // Check if the user is ok with the data being replaced
            System.Windows.MessageBoxResult result =  Xceed.Wpf.Toolkit.MessageBox.Show("By scanning the ADCP, all values in the project will be replaced with values from the ADCP.\nAre you sure you want to continue?", "Project Warning", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // Set flag that it is scanning
                IsScanning = true;

                // Execute scan
                await Task.Run(() => ExecuteScanAdcp());
            }
        }

        /// <summary>
        /// Execute the scan of the ADCP.
        /// </summary>
        private void ExecuteScanAdcp()
        {
            if (_pm.IsProjectSelected)
            {
                _pm.SelectedProject = _adcpConnection.SetAdcpConfiguration(_pm.SelectedProject);
            }

            Application.Current.Dispatcher.BeginInvoke(new System.Action(() => 
            {
                // Set the deployment days if it has changed
                //DeploymentDays = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
                if (_pm.SelectedProject.Configuration.DeploymentOptions.Duration <= 0)
                {
                    DeploymentDays = 1;
                }
                else
                {
                    DeploymentDays = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
                }

                // Clear the previous list and then populate it
                SubsystemConfigList.Clear();

                // Create temp variables
                long dataSize = 0;
                double numberBattery = 0.0;

                foreach (AdcpSubsystemConfig ssCfg in _pm.SelectedProject.Configuration.SubsystemConfigDict.Values)
                {
                    // Create the VM and add it to the list
                    AdcpSubsystemConfigurationViewModel ssVM = new AdcpSubsystemConfigurationViewModel(ssCfg, this);
                    ssVM.Predictor.DeploymentDuration = _pm.SelectedProject.Configuration.DeploymentOptions.Duration;
                    ssVM.Predictor.BatteryType = _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType;

                    // Add the vm to the list
                    SubsystemConfigList.Add(ssVM);

                    // Accumluate the data sizes and number batteries for each subsystem configuration
                    dataSize += ssVM.GetDataSize();
                    numberBattery += ssVM.Predictor.NumberBatteryPacks;
                }

                // Set the combined values
                NumberBatteryPacks = numberBattery.ToString("0.00");
                PredictedStorageUsed = dataSize + InternalStorageUsed;
                DataSize = MathHelper.MemorySizeString(dataSize);

                UpdateProperties();

                // Turn off flag
                IsScanning = false;
            }));
        }

        /// <summary>
        /// Remove the configuration from the list.
        /// </summary>
        /// <param name="ssConfigVM">ViewModel to remove from the list.</param>
        public void RemoveConfiguration(AdcpSubsystemConfigurationViewModel ssConfigVM)
        {
            // Shutdown the view model
            ssConfigVM.Dispose();

            // Remove from the list
            SubsystemConfigList.Remove(ssConfigVM);

            if (_pm.IsProjectSelected)
            {
                // Remove the subsystem config from the project
                if (_pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ssConfigVM.ConfigKey))
                {
                    _pm.SelectedProject.Configuration.RemoveAdcpSubsystemConfig(_pm.SelectedProject.Configuration.SubsystemConfigDict[ssConfigVM.ConfigKey]);
                }

                // Save the new configuration
                _pm.SelectedProject.Save();
            }

            // Update the display
            this.NotifyOfPropertyChange(() => this.SubsystemConfigList);
            this.NotifyOfPropertyChange(() => this.CEPO);
            this.NotifyOfPropertyChange(() => this.CEPO_DescStr);
            this.NotifyOfPropertyChange(() => this.SerialNumberStr);

            // this will update the predictions
            UpdateDeploymentDays(_DeploymentDays);

            // Create a new Ping Model
            UpdatePingModel();
        }

        /// <summary>
        /// Clear the list of all the configurations.
        /// This will properly shutdown the view model
        /// and then clear the list.
        /// </summary>
        private void ClearConfigurationList()
        {
            // Shutdown all the view models
            for (int x = 0; x < SubsystemConfigList.Count; x++)
            {
                SubsystemConfigList[x].Dispose();
            }

            // Clear the list
            SubsystemConfigList.Clear();
        }
        

        #endregion

        #region Update Properties

        /// <summary>
        /// Update all the properties to see the latest changes.
        /// </summary>
        private void UpdateProperties()
        {
            // This will update all the properties
            this.NotifyOfPropertyChange();

            this.NotifyOfPropertyChange(() => this.CEPO);
            this.NotifyOfPropertyChange(() => this.CEPO_DescStr);
            this.NotifyOfPropertyChange(() => this.SerialNumberStr);
            this.NotifyOfPropertyChange(() => this.CETFP);

            // Update the ping model
            UpdatePingModel();
        }

        #endregion

        #region Predictor

        /// <summary>
        /// Update the deployment days to all the subsystem configuration
        /// predictors.  Then update all the properties here.
        /// </summary>
        /// <param name="days"></param>
        private void UpdateDeploymentDays(uint days)
        {
            // Update the project and save
            if (_pm.IsProjectSelected)
            {
                _pm.SelectedProject.Configuration.DeploymentOptions.Duration = days;
                _pm.SelectedProject.Save();
            }

            // Create temp variables
            long dataSize = 0;
            double numberBattery = 0.0;

            // Get the latest values
            foreach (var ssVM in SubsystemConfigList)
            {
                ssVM.UpdateDeploymentDays(days);

                dataSize += ssVM.GetDataSize();

                numberBattery += ssVM.Predictor.NumberBatteryPacks;
                
            }

            // Set the combined values
            NumberBatteryPacks = numberBattery.ToString("0.00");
            PredictedStorageUsed = dataSize + InternalStorageUsed;
            DataSize = MathHelper.MemorySizeString(dataSize);

        }

        /// <summary>
        /// Update the battery type to all the subsystem configuration
        /// predictors.  Then update all the properties here.
        /// </summary>
        /// <param name="battType">Battery Type.</param>
        private void UpdateBatteryType(DeploymentOptions.AdcpBatteryType battType)
        {
            // Update the project and save
            if (_pm.IsProjectSelected)
            {
                _pm.SelectedProject.Configuration.DeploymentOptions.BatteryType = battType;
                _pm.SelectedProject.Save();
            }

            // Create temp variables
            long dataSize = 0;
            double numberBattery = 0.0;

            // Get the latest values
            foreach (var ssVM in SubsystemConfigList)
            {
                ssVM.UpdateBatteryType(battType);

                dataSize += ssVM.GetDataSize();
                numberBattery += ssVM.Predictor.NumberBatteryPacks;

            }

            // Set the combined values
            NumberBatteryPacks = numberBattery.ToString("0.00");
            PredictedStorageUsed = dataSize + InternalStorageUsed;
            DataSize = MathHelper.MemorySizeString(dataSize);

        }

        private void UpdatePredictor()
        {

        }

        #endregion

        #region Ping Model

        /// <summary>
        /// Setup the Ping Model display.
        /// </summary>
        private void UpdatePingModel()
        {
            // Initialize the Ping Model
            PingModelVM = new PingModelViewModel();

            if (_pm.IsProjectSelected)
            {
                // Set the Ping Mode
                PingModelVM.AddConfiguration(_pm.SelectedProject.Configuration);
            }

            this.NotifyOfPropertyChange(() => this.PingModelVM);
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
                    case RTI.Commands.AdcpCommands.CMD_CWS:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SalinityView));
                        break;
                    case "AVG":
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView));
                        break;
                    case "NAV_SOURCES":
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.NavSourcesView));
                        break;
                    case "STORAGE":
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.StorageView));
                        break;
                    case RTI.Commands.AdcpCommands.CMD_CETFP:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.TimeView));
                        break;
                    case RTI.Commands.AdcpCommands.CMD_CEI:
                        _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.EnsembleIntervalView));
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Save Commands to File

                /// <summary>
        /// Scan the ADCP for its current configuration.
        /// </summary>
        private async Task SaveCommandsToFile()
        {
            await Task.Run(() => SaveCommandsToFileExec());
        }

        /// <summary>
        /// Save all the commands to a text file in the project.
        /// </summary>
        private void SaveCommandsToFileExec()
        {
            try
            {
                // Check if a project is selected
                if (_pm.IsProjectSelected)
                {
                    // Get the project dir
                    // Create the file name
                    string prjDir = _pm.SelectedProject.ProjectFolderPath;
                    string fileName = "Commands.txt";
                    string cmdFilePath = prjDir + @"\" + fileName;

                    // Get the commands
                    string[] lines = GetCommands().ToArray();

                    // Create a text file in the project
                    System.IO.File.WriteAllLines(cmdFilePath, lines);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Get all the commands to send to the ADCP.
        /// This will include all the additional commands.
        /// </summary>
        /// <returns>List of all the commands to send to the ADCP.</returns>
        private List<string> GetCommands()
        {
            List<string> commands = new List<string>();

            // Stop the ADCP pinging
            // The ADCP will not take commands if it is pinging
            commands.Add(Commands.AdcpCommands.CMD_STOP_PINGING);

            // Add the ADCP commands
            // This command must go first because the CEPO command will set all the configurations to default values
            commands.AddRange(_pm.SelectedProject.Configuration.Commands.GetDeploymentCommandList());

            // Add all Subsystem Commands
            foreach (var config in _pm.SelectedProject.Configuration.SubsystemConfigDict.Values)
            {
                commands.AddRange(config.Commands.GetDeploymentCommandList());
            }

            // Verify and add the additional commands to the list
            //commands.AddRange(VerifyAdditionalCommands());

            // Save the commands to the ADCP
            commands.Add(Commands.AdcpCommands.CMD_CSAVE);

            return commands;
        }

        #endregion

        #region IDeactivate

        /// <summary>
        /// Shutdown the viewmodel.
        /// </summary>
        /// <param name="close">Flag if closing.</param>
        void IDeactivate.Deactivate(bool close)
        {
            Dispose();
        }

        #endregion
    }
}
