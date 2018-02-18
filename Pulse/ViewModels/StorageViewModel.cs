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
 * 10/10/2013      RC          3.2.0      Initial coding
 * 10/14/2013      RC          3.2.0      Added Backup location and added button get to the ADCP internal storage.
 * 12/12/2013      RC          3.2.0      Added FormatInternalStorageCommand to format the internal storage.
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
    using ReactiveUI;
    using System.Windows.Forms;
    using System.Threading.Tasks;

    /// <summary>
    /// Model to set the storage options.
    /// </summary>
    public class StorageViewModel : PulseViewModel
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

        /// <summary>
        /// Used for a parameter when browsing for a folder path
        /// to know which folder path to set.
        /// Disk 1.
        /// </summary>
        public const string DISK_1 = "DISK_1";

        /// <summary>
        /// Used for a parameter when browsing for a folder path
        /// to know which folder path to set.
        /// Disk 2.
        /// </summary>
        public const string DISK_2 = "DISK_2";

        #endregion

        #region Properties

        #region External Storage

        /// <summary>
        /// Maximum File size for external storage.
        /// </summary>
        private long _MaxFileSize;
        /// <summary>
        /// Maximum File size for external storage.
        /// </summary>
        public long MaxFileSize
        {
            get { return _MaxFileSize; }
            set
            {
                _MaxFileSize = value;
                this.NotifyOfPropertyChange(() => this.MaxFileSize);

                if (_pm.IsProjectSelected)
                {
                    // Convert the value to bytes
                    // and set to the project
                    _pm.SelectedProject.SetMaxBinaryFileSize(value * MathHelper.MB_TO_BYTES);

                    // Save the values
                    _pm.SelectedProject.Save();
                }
            }
        }

        #region Disk 1

        /// <summary>
        /// Disk 1 folder path.
        /// </summary>
        private string _Disk1FolderPath;
        /// <summary>
        /// Disk 1 folder path.
        /// </summary>
        public string Disk1FolderPath
        {
            get { return _Disk1FolderPath; }
            set
            {
                _Disk1FolderPath = value;
                this.NotifyOfPropertyChange(() => this.Disk1FolderPath);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.ProjectDir = value;

                    // Save the value
                    _pm.SelectedProject.Save();
                }
            }
        }

        #endregion

        #region Disk 2

        /// <summary>
        /// Enable Disk 2 storage.
        /// </summary>
        private bool _IsDisk2Enabled;
        /// <summary>
        /// Enable Disk 2 storage.
        /// </summary>
        public bool IsDisk2Enabled
        {
            get { return _IsDisk2Enabled; }
            set
            {
                _IsDisk2Enabled = value;

                this.NotifyOfPropertyChange(() => this.IsDisk2Enabled);
                this.NotifyOfPropertyChange(() => this.IsNextAvail);

                // If set false, remove the backup from the project
                if (!value && _pm.IsProjectSelected)
                {
                    _pm.SelectedProject.RemoveBackupWriter();
                    _Disk2FolderPath = null;
                    this.NotifyOfPropertyChange(() => this.Disk2FolderPath);
                    this.NotifyOfPropertyChange(() => this.IsNextAvail);
                }

                // If enabled, browse automatically
                if (value)
                {
                    //BrowseProjectFolderCommand.Execute(DISK_2);
                    BrowseProjectFolder(DISK_2);
                }
            }
        }

        /// <summary>
        /// Disk 2 Folder Path.
        /// </summary>
        private string _Disk2FolderPath;
        /// <summary>
        /// Disk 2 Folder Path.
        /// </summary>
        public string Disk2FolderPath
        {
            get { return _Disk2FolderPath; }
            set
            {
                _Disk2FolderPath = value;

                // If a project is selected, set the value
                if (_pm.IsProjectSelected && !string.IsNullOrEmpty(value))
                {
                    _pm.SelectedProject.CreateBackupWriter(_pm.SelectedProject.ProjectName, value, _pm.SelectedProject.SerialNumber.SerialNumberString, _MaxFileSize);

                    // Save the value
                    _pm.SelectedProject.Save();
                }

                // If the string is cleared, remove the backup writer
                if (_pm.IsProjectSelected && string.IsNullOrEmpty(value))
                {
                    _pm.SelectedProject.RemoveBackupWriter();
                }

                this.NotifyOfPropertyChange(() => this.Disk2FolderPath);
                this.NotifyOfPropertyChange(() => this.IsNextAvail);
            }
        }

        /// <summary>
        /// Used to enable and disable the next button based off the options
        /// selected.
        /// </summary>
        public bool IsNextAvail
        {
            get
            {
                return !(IsDisk2Enabled && string.IsNullOrEmpty(Disk2FolderPath));
            }
        }

        #endregion

        #endregion

        #region Internal Storage

        /// <summary>
        /// Enable Internal Storage.
        /// </summary>
        private bool _IsInternalStorageEnabled;
        /// <summary>
        /// Enable Internal Storage.
        /// </summary>
        public bool IsInternalStorageEnabled
        {
            get { return _IsInternalStorageEnabled; }
            set
            {
                _IsInternalStorageEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsInternalStorageEnabled);

                // Enable/Disable CERECORD
                if (_pm.IsProjectSelected)
                {
                    if (value)
                    {
                        _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing  = Commands.AdcpCommands.AdcpRecordOptions.Enable;
                    }
                    else
                    {
                        _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing = Commands.AdcpCommands.AdcpRecordOptions.Disable;
                    }

                    // Save the values
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// ADCP internal storage size.
        /// </summary>
        private string _AdcpInternalStorageSize;
        /// <summary>
        /// ADCP internal storage size.
        /// </summary>
        public string AdcpInternalStorageSize
        {
            get { return _AdcpInternalStorageSize; }
            set
            {
                _AdcpInternalStorageSize = value;
                this.NotifyOfPropertyChange(() => this.AdcpInternalStorageSize);
            }
        }

        /// <summary>
        /// ADCP internal storage used spaced.
        /// </summary>
        private string _AdcpInternalStorageUsed;
        /// <summary>
        /// ADCP internal storage used spaced.
        /// </summary>
        public string AdcpInternalStorageUsed
        {
            get { return _AdcpInternalStorageUsed; }
            set
            {
                _AdcpInternalStorageUsed = value;
                this.NotifyOfPropertyChange(() => this.AdcpInternalStorageUsed);
            }
        }

        #endregion

        #region Image

        /// <summary>
        /// Image of the ADCP.
        /// </summary>
        private string _ImageSrc;
        /// <summary>
        /// Image of the ADCP.
        /// </summary>
        public string ImageSrc
        {
            get { return _ImageSrc; }
            set
            {
                _ImageSrc = value;
                this.NotifyOfPropertyChange(() => this.ImageSrc);
            }
        }

        #endregion

        #region IsLoading

        /// <summary>
        /// Set flag for IsLoading.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Set flag for IsLoading.
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
        /// Command to browse for the project recording folder.
        /// </summary>
        public ReactiveCommand<object> BrowseProjectFolderCommand { get; protected set; }

        /// <summary>
        /// Refresh the amount of internal storage in the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> RefreshAdcpInternalStorageCommand { get; protected set; }

        /// <summary>
        /// Format the internal SD card of the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> FormatInternalStorageCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public StorageViewModel()
            : base("Storage")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Get the singleton ADCP connection
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Initialize values
            IsLoading = false;

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsNextAvail, x => x.Value));                           // Check if available
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SalinityView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Browse Project Folder command
            BrowseProjectFolderCommand = ReactiveCommand.Create();
            BrowseProjectFolderCommand.Subscribe(param => BrowseProjectFolder(param));

            // Refresh Internal ADCP storage coommand
            RefreshAdcpInternalStorageCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => CheckMemoryCard()));

            // Format SD Card
            FormatInternalStorageCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => FormatInternalStorage()));

            // Initialize values
            InitializeValues();
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Init

        /// <summary>
        /// Intialize the values for the view model.
        /// </summary>
        private void InitializeValues()
        {
            // If project is selected, initalize the values
            if (_pm.IsProjectSelected)
            {
                // Set the max file size but do not change it in the project
                _MaxFileSize = _pm.SelectedProject.GetMaxBinaryFileSize() / MathHelper.MB_TO_BYTES;
                this.NotifyOfPropertyChange(() => this.MaxFileSize);

                // Set the Disk1 folder path
                _Disk1FolderPath = _pm.SelectedProject.ProjectFolderPath;
                this.NotifyOfPropertyChange(() => this.Disk2FolderPath);

                _Disk2FolderPath = _pm.SelectedProject.BackupProjectFolderPath;
                this.NotifyOfPropertyChange(() => this.Disk2FolderPath);

                // Set if the disk is enabled based off if a value has been set for the path
                _IsDisk2Enabled = !string.IsNullOrEmpty(_Disk2FolderPath);
                this.NotifyOfPropertyChange(() => this.IsDisk2Enabled);

                // Internal recording
                if (_pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.Enable || _pm.SelectedProject.Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.BT_Eng)
                {
                    _IsInternalStorageEnabled = true;
                }
                else
                {
                    _IsInternalStorageEnabled = false;
                }
                this.NotifyOfPropertyChange(() => this.IsInternalStorageEnabled);

                // Memory card
                AdcpInternalStorageSize = MathHelper.MemorySizeString(_pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardTotalSize);
                AdcpInternalStorageUsed = MathHelper.MemorySizeString(_pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardUsed);

                // Set the product image
                ImageSrc = ProductImage.GetProductImage(_pm.SelectedProject);
            }
        }

        #endregion

        #region Disk Storage

        /// <summary>
        /// Check the memory card for its usage.
        /// This will set how full the memory card is
        /// and how much is required for the deployment.
        /// </summary>
        /// <returns>TRUE = Status was good.</returns>
        private void CheckMemoryCard()
        {
            IsLoading = true;

            // Get the memory usasge
            RTI.Commands.AdcpDirListing listing = _adcpConnection.GetDirectoryListing();

            // Set the total memory card usage
            if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
            {
                _pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardUsed = (long)Math.Round(listing.UsedSpace * MathHelper.MB_TO_BYTES);
                _pm.SelectedProject.Configuration.DeploymentOptions.InternalMemoryCardTotalSize = (long)Math.Round(listing.TotalSpace * MathHelper.MB_TO_BYTES);

                // Save the values
                _pm.SelectedProject.Save();
            }

            // Memory card
            AdcpInternalStorageSize = MathHelper.MemorySizeString((long)Math.Round(listing.TotalSpace * MathHelper.MB_TO_BYTES));
            AdcpInternalStorageUsed = MathHelper.MemorySizeString((long)Math.Round(listing.UsedSpace * MathHelper.MB_TO_BYTES));

            IsLoading = false;
        }

        #endregion

        #region Browse

        /// <summary>
        /// Browse for the user to give a folder path for the project.
        /// </summary>
        private void BrowseProjectFolder(object param)
        {
            // Try to convert the disk option to a string
            // If this fails, later it will not be used.
            string diskOption = param as string;

            string folderPath = "";

            // Show the FolderBrowserDialog.
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = folderPath;
            dialog.Description = "Choose a folder to save project data.";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;

                // Set the folder path based off the parameter given
                if(diskOption != null)
                {
                    switch(diskOption)
                    {  
                        case DISK_1:
                            Disk1FolderPath = folderPath;
                            break;
                        case DISK_2:
                            Disk2FolderPath = folderPath;
                            break;
                        default:
                            break;
                    }
                }

                // Store the path to the database to retrieve next time
            }
        }

        #endregion

        #region Format Internal Storage

        /// <summary>
        /// Format the internal SD memory card in the ADCP.
        /// Give a warning before formating.
        /// </summary>
        private void FormatInternalStorage()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                // Check if the user is ok with formating
                System.Windows.MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Are you sure you would like to format the ADCP's internal SD card?\nThe data can not be recovered if formated.\nEnsure you have downloaded all the data from the ADCP before proceeding.", "Format Warning", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // Send the command to format the SD card
                    _adcpConnection.SendData(Commands.AdcpCommands.CMD_DSFORMAT);

                    // Refresh the internal storage usage
                    CheckMemoryCard();
                }
            }));
        }

        #endregion

    }
}
