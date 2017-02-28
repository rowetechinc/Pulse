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
 * 10/15/2013      RC          3.2.0      Initial coding
 * 01/02/2014      RC          3.2.2      Stop clearing the ProjectList in ClearProjectList().  Set the IsSelected property for the selected project.
 * 04/04/2014      RC          3.2.4      If the file is not a DB file, then consider it a binary file in ImportFiles().
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 08/13/2014      RC          4.0.0      Load projects async.
 * 08/15/2014      RC          4.0.0      In ImportDb() and ImportBinary, refresh the image on the UI thread.
 * 08/21/2014      RC          4.0.1      Fixed importing the data and loading the image in sync.
 * 01/16/2015      RC          4.1.0      Changed the importer to process the raw data into ensembles. 
 * 04/08/2015      RC          4.1.3      Changed the import to record the data without publishing it.
 * 07/27/2015      RC          4.1.5      Changed importing a file to FilePlayback.
 * 11/25/2015      RC          4.3.1      Select ENS and BIN as default options for playback files.
 * 02/27/2017      RC          4.5.1      Improved the performance of loading a project in ScanProjectAsync().
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
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Allow the user to select a project from the list of project
    /// in the Pulse database.  Also allow the user to import a project.
    /// </summary>
    public class LoadProjectsViewModel : PulseViewModel, IActivate, IDeactivate
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
        /// Import Project name given by the user.
        /// </summary>
        private string _importProjectName;

        /// <summary>
        /// Default file image.
        /// It is blank so no image will be displayed.
        /// </summary>
        private string DEFAULT_IMAGE_PATH = @"pack://application:,,,/"
                                                 + Assembly.GetExecutingAssembly().GetName().Name
                                                 + ";component/"
                                                 + "Images/blank_projectimage.png";

        /// <summary>
        /// Event to cause the thread
        /// to go to sleep or wakeup.
        /// </summary>
        private EventWaitHandle _eventWaitImport;

        #endregion

        #region Properties

        /// <summary>
        /// List of all the projects created.
        /// </summary>
        public ReactiveList<ProjectListItemViewModel> ProjectList { get; set; }

        /// <summary>
        /// Selected Project ListItem VM.
        /// </summary>
        private ProjectListItemViewModel _SelectedProjectVM;
        /// <summary>
        /// Selected Project ListItem VM.
        /// </summary>
        public ProjectListItemViewModel SelectedProjectVM
        {
            get { return _SelectedProjectVM; }
            set
            {
                if (value != null)
                {
                    // Project is changing
                    // So shutdown the previous selection
                    if (value != _SelectedProjectVM)
                    {
                        if (_SelectedProjectVM != null)
                        {
                            _SelectedProjectVM.IsSelected = false;
                            _SelectedProjectVM.Dispose();
                        }
                    }

                    //Set the selected project
                    _pm.SelectedProject = _pm.GetProject(value.ProjectName);
                }

                // Set the value
                _SelectedProjectVM = value;

                // Set IsSelected if its not null
                if (_SelectedProjectVM != null)
                {
                    _SelectedProjectVM.IsSelected = true;
                }

                this.NotifyOfPropertyChange(() => this.SelectedProjectVM);
            }
        }

        #region Import

        /// <summary>
        /// Set flag if the projects are loading.
        /// This is true when we are importing or removing a project.
        /// </summary>
        private bool _IsProjectLoading;
        /// <summary>
        /// Set flag if the projects are loading.
        /// This is true when we are importing or removing a project.
        /// </summary>
        public bool IsProjectLoading
        {
            get { return _IsProjectLoading; }
            set
            {
                _IsProjectLoading = value;
                this.NotifyOfPropertyChange(() => this.IsProjectLoading);
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
        /// Command to import data.
        /// </summary>
        public ReactiveCommand<object> ImportDataCommand { get; protected set; }

        /// <summary>
        /// Command to import RTB data.
        /// </summary>
        public ReactiveCommand<object> ImportRtbDataCommand { get; protected set; }


        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public LoadProjectsViewModel()
            : base("Load Projects")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Get the singleton ADCP connection
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Wait for decoding to be complete
            _eventWaitImport = new EventWaitHandle(false, EventResetMode.AutoReset);

            // Intialize values
            InitValues();

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedProjectVM, x => x.Value != null));
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpConfigurationView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Dialog to import data
            ImportDataCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsProjectLoading, x => !x.Value));
            ImportDataCommand.Subscribe(_ => ImportData());

            // Dialog to import RTB data
            ImportRtbDataCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsProjectLoading, x => !x.Value));
            ImportRtbDataCommand.Subscribe(_ => ImportRTB());

            // Scan for the projects
            Task.Run(() => ScanProjectAsync());
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            ClearProjectList();
        }

        #region Initialize Value

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void InitValues()
        {
            // Set value
            IsProjectLoading = false;
        }

        #endregion

        #region Load Projects

        /// <summary>
        /// Run this method async to get the list of all
        /// the projects.
        /// </summary>
        /// <returns>List of all the projects.</returns>
        private async Task ScanProjectAsync()
        {
            IsProjectLoading = true;

            ProjectList = new ReactiveList<ProjectListItemViewModel>();

            var list = await Task.Run(() => _pm.GetProjectList());
            foreach (var prj in list)
            {
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    // Create a project list item and add it to the list
                    var prjVM = new ProjectListItemViewModel(prj, this);
                    ProjectList.Add(prjVM);

                    // Check if the project image exist
                    if(prjVM.ProjectImage == null)
                    {
                        // Get the project for the image name
                        Project project = _pm.GetProject(prjVM.ProjectName);

                        // If the project image does not exist, create the image now
                        if (!File.Exists(project.GetProjectImagePath()))
                        {
                            Task.Run(() =>
                            {
                                prjVM.RefreshDisplay();
                                this.NotifyOfPropertyChange(() => this.ProjectList);
                            });
                        }
                    }

                    // Set the last selected project
                    if (prj.ProjectID == _pm.GetSelectedProjectID())
                    {
                        if (_pm.IsProjectSelected)
                        {
                            _SelectedProjectVM = prjVM;
                        }
                        else
                        {
                            SelectedProjectVM = prjVM;
                        }
                        _SelectedProjectVM.IsSelected = true;
                        this.NotifyOfPropertyChange(() => this.SelectedProjectVM);
                    }

                    prj.Dispose();
                }));
            }

            IsProjectLoading = false;
        }

        /// <summary>
        /// Clear the project list.
        /// 
        /// Removed ProjectList.Clear() because it would cause an InvalidOperationException about
        /// the border brush color.  I assume this is because the entry is removed from the list 
        /// so it can not set the border brush color.
        /// </summary>
        private void ClearProjectList()
        {
            if (ProjectList != null)
            {
                foreach (ProjectListItemViewModel prj in ProjectList)
                {
                    //prj.IsSelected = false;
                    prj.Dispose();
                }

                //ProjectList.Clear();
            }
        }

        #endregion

        #region Delete Project

        /// <summary>
        /// Delete the project.  This will remove the project and
        /// the project's view model.
        /// </summary>
        /// <param name="projectVM">Project to remove.</param>
        public void DeleteProject(ProjectListItemViewModel projectVM)
        {
            if (projectVM != null)
            {
                string dir = projectVM.ProjectFolderPath;
                string prjName = projectVM.ProjectName;

                // Unselect the project VM
                if (_SelectedProjectVM == projectVM)
                {
                    _pm.SelectedProject.Dispose();
                    SelectedProjectVM.IsSelected = false;
                    SelectedProjectVM.Dispose();
                    SelectedProjectVM = null;
                }

                // Delete the project from the database
                _pm.RemoveProject(projectVM.ProjectName);

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    // Dispose of the VM
                    projectVM.Dispose();

                    var tempPrjList = new ReactiveList<ProjectListItemViewModel>(ProjectList);
                    tempPrjList.Remove(projectVM);

                    ProjectList = tempPrjList;
                    this.NotifyOfPropertyChange(() => this.ProjectList);

                    // Delete the selected project from the list
                    //_ProjectList.Remove(projectVM);
                }));


                // Prompt to delete the folder content also
                this.PromptToPermenatelyDelete(dir, prjName);
            }
        }

        /// <summary>
        /// Permenately delete the project by deleting the folder and 
        /// all the content within the folder.  Prompt the user what
        /// they want to do.
        /// </summary>
        private MessageBoxResult PromptToPermenatelyDelete(string directory, string projectName)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(string.Format("Do you want to permenantely delete the project {0}?", projectName), "Permenately Delete", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        // Delete the Project folder and all files within
                        Directory.Delete(directory, true);
                    }
                }
                catch (IOException ex)
                {
                    log.Error("Exception peremantely deleting project.", ex);
                    System.Windows.MessageBox.Show("Error Deleting project.  Project file in use.");
                    return result;
                }
            }

            return result;
        }

        #endregion

        #region Import Data

        /// <summary>
        /// Ask the user for which files to import.
        /// Then being the import process.
        /// </summary>
        private async void ImportData()
        {
            try
            {
                // Show the FolderBrowserDialog.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Ensemble files (*.bin, *.ens)|*.bin; *.ens|BIN files (*.bin)|*.bin|ENS files (*.ens)|*.ens|DB files (*.db)|*.db|All files (*.*)|*.*";
                dialog.Multiselect = true;
                dialog.InitialDirectory = Pulse.Commons.DEFAULT_RECORD_DIR;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Get the files selected
                    string[] files = dialog.FileNames;

                    // Get the import Project name
                    bool importPrjNameResult = GetImportProjectName(files);

                    if (importPrjNameResult)
                    {
                        // Import the files
                        await Task.Run(() => ImportFiles(files));
                    }
                }
            }
            catch (AccessViolationException ae)
            {
                log.Error("Error trying to open file", ae);
            }
            catch (Exception e)
            {
                log.Error("Error trying to open file", e);
            }
        }

        private async void ImportRTB()
        {
            try
            {
                // Show the FolderBrowserDialog.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Ensemble files (*.bin, *.ens)|*.bin; *.ens|BIN files (*.bin)|*.bin|ENS files (*.ens)|*.ens|DB files (*.db)|*.db|All files (*.*)|*.*";
                dialog.Multiselect = true;
                //dialog.InitialDirectory = Pulse.Commons.DEFAULT_RECORD_DIR;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Get the files selected
                    string[] files = dialog.FileNames;

                    // Get the import Project name
                    bool importPrjNameResult = GetImportProjectName(files);

                    if (importPrjNameResult)
                    {
                        // Import the files
                        await Task.Run(() => ImportRTB(files));
                    }
                }
            }
            catch (AccessViolationException ae)
            {
                log.Error("Error trying to open file", ae);
            }
            catch (Exception e)
            {
                log.Error("Error trying to open file", e);
            }
        }


        /// <summary>
        /// Get the Import Project name from the user.  This will display a 
        /// dialog for the project name for the imported files.  If the files found
        /// is a database, then it will not ask for a project name.
        /// </summary>
        /// <param name="files">Files importing.</param>
        private bool GetImportProjectName(string[] files)
        {
            // Try to get a default project name
            _importProjectName = DateTime.Today.ToString("yyyymmddhhmmss");
            if (files.Length > 0)
            {
                FileInfo finfo = new FileInfo(files[0]);
                if (finfo.Extension.ToLower() != ".db")
                {
                    _importProjectName = Path.GetFileNameWithoutExtension(finfo.Name);

                    // Ask the user if they want to use this project name
                    ImportNameDialogViewModel vm = new ImportNameDialogViewModel();
                    vm.ImportName = _importProjectName;
                    IoC.Get<IWindowManager>().ShowDialog(vm);

                    // Set the project name based off the user
                    if (vm.IsOk)
                    {
                        _importProjectName = vm.ImportName;
                    }

                    // If the user presses cancel, then stop now
                    if (vm.IsCancel)
                    {
                        return false;
                    }

                    // Check if the project exist
                    if (IsProjectExist(_importProjectName))
                    {
                        // Check with the user to merge the projects
                        var msgBxResult = System.Windows.Forms.MessageBox.Show(string.Format("Project {0} already exist.  Do you want to merge the projects?", _importProjectName), "Project Exist Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        // If the user does not want to merge, return false
                        if (msgBxResult == DialogResult.No)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Import all the files selected to a new project.
        /// The project will be based off the first imported file.
        /// This will determine what type of files are being imported
        /// and import them properly.
        /// </summary>
        /// <param name="files">Files to import.</param>
        private void ImportFiles(object files)
        {
            // Cast the parameter to a string array
            string[] importFiles = files as string[];

            // Determine the type of files
            // If there are different types, the import process cannot
            // continue.  The user can either choose a DB file for
            // binary files
            bool dbFile = false;
            bool binaryFile = false;
            foreach (string fileStr in importFiles)
            {
                FileInfo finfo = new FileInfo(fileStr);
                if (finfo.Extension.ToLower() == ".db")
                {
                    dbFile = true;
                }

                if (finfo.Extension.ToLower() == ".ens" || finfo.Extension.ToLower() == ".bin")
                {
                    binaryFile = true;
                }
            }

            // If both types were found,
            // give a warning and stop here
            if (dbFile && binaryFile)
            {
                System.Windows.Forms.MessageBox.Show("Either Binary files (.BIN or .ENS) or a Project file (.db) can be imported, but both types cannot be imported at the same time.  Please import again and select one file type.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start the import process
            if (importFiles != null)
            {
                // Determine the file type
                // Either a binary file recorded from the ADCP
                // or a Database file recorded from Pulse.
                List<string> binaryFileList = new List<string>();
                foreach (string fileStr in importFiles)
                {
                    FileInfo finfo = new FileInfo(fileStr);
                    if (finfo.Extension.ToLower() == ".db")
                    {
                        ImportDb(fileStr);
                    }
                    else
                    {
                        // Add all other files as a binary file
                        binaryFileList.Add(fileStr);
                    }

                    //// Get a list of all the binary files
                    //if (finfo.Extension.ToLower() == ".ens" || finfo.Extension.ToLower() == ".bin")
                    //{
                    //    binaryFileList.Add(fileStr);
                    //}
                }

                // If there were binary files
                // import the binary files
                if (binaryFileList.Count > 0)
                {
                    ImportBinary(binaryFileList);
                }
            }

            return;
        }

        /// <summary>
        /// Import the binary files.
        /// </summary>
        /// <param name="files">Files to import to a project.</param>
        private async void ImportBinary(List<string> files)
        {
            // If the project does not exist in the list, create it now
            if (!IsProjectExist(_importProjectName))
            {
                // Create the new project based off
                // the project name and project directory
                Project prj = new Project(_importProjectName, RTI.Pulse.Commons.GetProjectDefaultFolderPath(), null);

                // Create the new project based off
                // the project name and project directory
                //System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddProject(prj)));
                await AddProject(prj);
                prj.Dispose();
            }

            //// Create the importer and an eventhandler to handle the imported data
            //AdcpImportBinaryFile importer = new AdcpImportBinaryFile();
            ////importer.ReceiveBinaryDataEvent += new AdcpImportBinaryFile.ReceiveBinaryDataEventHandler(importer_ReceiveBinaryDataEvent);
            //importer.CompleteEvent += new AdcpImportBinaryFile.CompleteEventHandler(importer_CompleteEvent);

            // Set flag to turn on recording and importing
            // This will tell the recorder to record but since we
            // are importing, do not update all the playback displays
            IsProjectLoading = true;
            _adcpConnection.IsRecording = true;
            _adcpConnection.IsImporting = true;
            IoC.Get<PlaybackViewModel>().IsRecordEnabled = true;        // Call this to update the display

            // Create the file playback based off the selected file
            List<RTI.FilePlayback.EnsembleData> ensList = new List<FilePlayback.EnsembleData>();
            using(FilePlayback fp = new FilePlayback())
            {
                fp.FindEnsembles(files.ToArray());
                ensList = fp.GetEnsembleDataList();
            }

            // Publish all the ensembles found from the importer
            //foreach(var ens in list)
            foreach(var ens in ensList)
            {
                //_adcpConnection.PublishEnsembleData(ens.RawData, ens.Ensemble);
                // Check if the serial number is set for the project
                if (_pm.SelectedProject.SerialNumber.IsEmpty())
                {
                    if (ens.Ensemble.IsEnsembleAvail)
                    {
                        _pm.SelectedProject.SerialNumber = ens.Ensemble.EnsembleData.SysSerialNumber;
                    }
                }

                // Record the data
                _pm.SelectedProject.RecordBinaryEnsemble(ens.RawData);
                _pm.SelectedProject.RecordDbEnsemble(ens.Ensemble);
            }

            // Set the Project Image
            if (_SelectedProjectVM != null)
            {
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    _SelectedProjectVM.RefreshDisplay();
                    this.NotifyOfPropertyChange(() => this.ProjectList);
                }));
            }

            // Turn off recording and importing
            _adcpConnection.IsRecording = false;
            _adcpConnection.IsImporting = false;
            IoC.Get<PlaybackViewModel>().IsRecordEnabled = false;        // Call this to update the display
            IsProjectLoading = false;
        }

        /// <summary>
        /// Import the RoweTech binary files.
        /// This is optimized for RoweTech Binary files.
        /// </summary>
        /// <param name="files">Files to import to a project.</param>
        private async void ImportRTB(string[] files)
        {
            // If the project does not exist in the list, create it now
            if (!IsProjectExist(_importProjectName))
            {
                // Create the new project based off
                // the project name and project directory
                Project prj = new Project(_importProjectName, RTI.Pulse.Commons.GetProjectDefaultFolderPath(), null);

                // Create the new project based off
                // the project name and project directory
                //System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddProject(prj)));
                await AddProject(prj);
                prj.Dispose();
            }

            //// Create the importer and an eventhandler to handle the imported data
            //AdcpImportBinaryFile importer = new AdcpImportBinaryFile();
            ////importer.ReceiveBinaryDataEvent += new AdcpImportBinaryFile.ReceiveBinaryDataEventHandler(importer_ReceiveBinaryDataEvent);
            //importer.CompleteEvent += new AdcpImportBinaryFile.CompleteEventHandler(importer_CompleteEvent);

            // Set flag to turn on recording and importing
            // This will tell the recorder to record but since we
            // are importing, do not update all the playback displays
            IsProjectLoading = true;
            _adcpConnection.IsRecording = true;
            _adcpConnection.IsImporting = true;
            IoC.Get<PlaybackViewModel>().IsRecordEnabled = true;        // Call this to update the display

            // Create the file playback based off the selected file
            List<RTI.FilePlayback.EnsembleData> ensList = new List<FilePlayback.EnsembleData>();
            using (FilePlayback fp = new FilePlayback())
            {
                fp.FindRtbEnsembles(files);
                ensList = fp.GetEnsembleDataList();
            }

            // Publish all the ensembles found from the importer
            //foreach(var ens in list)
            foreach (var ens in ensList)
            {
                //_adcpConnection.PublishEnsembleData(ens.RawData, ens.Ensemble);
                // Check if the serial number is set for the project
                if (_pm.SelectedProject.SerialNumber.IsEmpty())
                {
                    if (ens.Ensemble.IsEnsembleAvail)
                    {
                        _pm.SelectedProject.SerialNumber = ens.Ensemble.EnsembleData.SysSerialNumber;
                    }
                }

                // Record the data
                _pm.SelectedProject.RecordBinaryEnsemble(ens.RawData);
                _pm.SelectedProject.RecordDbEnsemble(ens.Ensemble);
            }

            // Set the Project Image
            if (_SelectedProjectVM != null)
            {
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    _SelectedProjectVM.RefreshDisplay();
                    this.NotifyOfPropertyChange(() => this.ProjectList);
                }));
            }

            // Turn off recording and importing
            _adcpConnection.IsRecording = false;
            _adcpConnection.IsImporting = false;
            IoC.Get<PlaybackViewModel>().IsRecordEnabled = false;        // Call this to update the display
            IsProjectLoading = false;
        }

        /// <summary>
        /// EventHandler from the importer.  This will pass data
        /// the ADCP connections binary codec.
        /// </summary>
        /// <param name="data">Data to import.</param>
        private void importer_ReceiveBinaryDataEvent(byte[] data)
        {
            _adcpConnection.ReceiveAdcpSerialData(data);
        }

        /// <summary>
        /// Set flag when importing is complete.
        /// </summary>
        void importer_CompleteEvent()
        {
            IsProjectLoading = false; ;
            _eventWaitImport.Set();
        }

        /// <summary>
        /// Import a project database.
        /// </summary>
        /// <param name="dbPath">Project to import.</param>
        private async void ImportDb(string dbPath)
        {
            // Verify a good file path was given
            if (File.Exists(dbPath))
            {
                // Try to use the file name being imported as the project name
                // This will get the first file in the list
                // It will then use the filename without the extension as the project name
                FileInfo finfo = new FileInfo(dbPath);
                string name = Path.GetFileNameWithoutExtension(finfo.Name);

                // Create the new project based off
                // the project name and project directory
                Project prj = new Project(name, RTI.Pulse.Commons.GetProjectDefaultFolderPath(), null, dbPath);

                // Add the project to the application
                await AddProject(prj);
                prj.Dispose();

                // Set the Project Image
                if (_SelectedProjectVM != null)
                {
                    await System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        _SelectedProjectVM.RefreshDisplay();
                        this.NotifyOfPropertyChange(() => this.ProjectList);
                    }));
                }
            }
        }

        /// <summary>
        /// Check if the project already exist in the list.  The list is the
        /// list of all the projects that Pulse is maintaining.
        /// </summary>
        /// <param name="prjName">Project name.</param>
        /// <returns>TRUE = Project exist in list.</returns>
        public bool IsProjectExist(string prjName)
        {
            // Check if the project name already exist
            foreach (var prjVM in ProjectList)
            {
                // Check if the project names match any project
                if (prjVM.ProjectName == prjName)
                {
                    return true;
                }

            }
            return false;
        }

        #endregion

        #region Add/Remove Project

        /// <summary>
        /// Add a project to the list of projects.
        /// This will add the project to the database.
        /// It will also set the new project as the 
        /// selected project.  If the project already
        /// exist, it will display a messagebox warning.
        /// </summary>
        /// <param name="prj">Project to add.</param>
        public async Task AddProject(Project prj)
        {
            // Add project to DB
            _pm.AddNewProject(prj);

            await System.Windows.Application.Current.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                // Create the VM
                ProjectListItemViewModel newPrjVm = new ProjectListItemViewModel(prj, this);

                // Add project VM to the list
                ProjectList.Insert(0, newPrjVm);

                // Set the new project as the selected project
                SelectedProjectVM = newPrjVm;

                _pm.SelectedProject = prj;
            }));


        }

        #endregion

        #region IDectivate

        /// <summary>
        /// Shutdown the VM.
        /// </summary>
        /// <param name="close"></param>
        void IDeactivate.Deactivate(bool close)
        {
            Dispose();
        }

        #endregion
    }
}
