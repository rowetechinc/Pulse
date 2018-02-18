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
 * 10/04/2013      RC          3.2.0      Initial coding
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
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Create a new project.  The project will include the
    /// commands and the data stored from the ADCP.
    /// This will ask for the project name and a directory to
    /// store the project.
    /// </summary>
    public class NewProjectViewModel : PulseViewModel
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
        /// Index to create a default project name.
        /// </summary>
        private int _projectNameIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Serial number generator.
        /// </summary>
        public SerialNumberGeneratorViewModel SerialNumberGeneratorVM { get; protected set; }

        /// <summary>
        /// Project name.
        /// </summary>
        private string _ProjectName;
        /// <summary>
        /// Project name.
        /// </summary>
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                this.NotifyOfPropertyChange(() => this.ProjectName);
            }
        }

        /// <summary>
        /// Project directory
        /// </summary>
        private string _ProjectDirectory;
        /// <summary>
        /// Project directory
        /// </summary>
        public string ProjectDir
        {
            get { return _ProjectDirectory; }
            set
            {
                _ProjectDirectory = value;
                this.NotifyOfPropertyChange(() => this.ProjectDir);
            }
        }

        /// <summary>
        /// Additional Commands to automatically add to the project.
        /// </summary>
        private string _AdditionalCommands;
        /// <summary>
        /// Additional Commands to automatically add to the project.
        /// </summary>
        public string AdditionalCommands
        {
            get { return _AdditionalCommands; }
            set
            {
                _AdditionalCommands = value;
                this.NotifyOfPropertyChange(() => this.AdditionalCommands);
            }
        }

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
        /// Command to load any commands you would like to use with the project and ADCP.
        /// </summary>
        public ReactiveCommand<object> LoadCommandsCommand { get; protected set; }

        /// <summary>
        /// Command to browse for the project folder.
        /// </summary>
        public ReactiveCommand<object> BrowseProjectFolderCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public NewProjectViewModel()
            : base("New Project")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Serial Number Generator view model
            SerialNumberGeneratorVM = IoC.Get<SerialNumberGeneratorViewModel>();

            // Initialize the values
            _projectNameIndex = 1;
            AdditionalCommands = "";

            // Get a default project name
            ProjectDir = Pulse.Commons.GetProjectDefaultFolderPath();
            ProjectName = GetNewDefaultProjectName();

            // Load Project command
            LoadCommandsCommand = ReactiveCommand.Create();
            LoadCommandsCommand.Subscribe(_ => LoadCommands());

            // Browse Project Folder command
            BrowseProjectFolderCommand = ReactiveCommand.Create();
            BrowseProjectFolderCommand.Subscribe(_ => BrowseProjectFolder());

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => CreateProject());

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            
        }

        #region Create Project

        /// <summary>
        /// Create a project with the given project name
        /// and project folder.
        /// </summary>
        private void CreateProject()
        {
            // Create the project
            Project prj = new Project(_ProjectName, _ProjectDirectory, SerialNumberGeneratorVM.SerialNumber.ToString());

            // Set it to PulseManager
            _pm.AddNewProject(prj);
            _pm.SelectedProject = prj;

            // Process additional commands

            // Create the ADCP configuration for the project


            // Move to the next section
            _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.CommunicationsView));
        }

        /// <summary>
        /// Generate a new default project name.
        /// This will use the project default name and
        /// add an index to the end.
        /// </summary>
        /// <returns>A new project name.</returns>
        private string GetNewDefaultProjectName()
        {
            // Generate a folder name
            string fileName = string.Format("{0}{1}", Pulse.Commons.DEFAULT_PROJECT_NAME, _projectNameIndex++);

            // Check if folder exist
            while (Directory.Exists(string.Format(@"{0}\{1}", ProjectDir, fileName)))
            {
                fileName = string.Format("{0}{1}", Pulse.Commons.DEFAULT_PROJECT_NAME, _projectNameIndex++);
            }

            return fileName;
        }


        #endregion


        #region Additional Commands

        /// <summary>
        /// Use the commands with this project.
        /// This will be a string of commands the user will give
        /// that will be used with the project.
        /// </summary>
        private void LoadCommands()
        {
            string fileName = "";
            try
            {
                // Show the FolderBrowserDialog.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All files (*.*)|*.*";
                dialog.Multiselect = false;

                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Get the files selected
                    fileName = dialog.FileName;

                    // Set the command set
                    AdditionalCommands = File.ReadAllText(fileName);

                    // Scan for a CEPO command to set the serial number
                    ScanAdditionalCommands();
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error reading command set from {0}", fileName), e);
            }
        }

        /// <summary>
        /// Scan the additional commands to determine the commands given by the user.
        /// This will help in setting some default values.
        /// 
        /// Check for th CEPO command to set the serial number.
        /// </summary>
        private void ScanAdditionalCommands()
        {
            // Go through each line in the commands
            using (StringReader reader = new StringReader(AdditionalCommands))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Look for the CEPO command
                    if (line.Contains(Commands.AdcpCommands.CMD_CEPO))
                    {
                        PassCepoToSerialNumberGen(line);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Get the CEPO command from the string.
        /// Then pass each value to the serial number generator as
        /// a subsystem.
        /// </summary>
        /// <param name="line">Line containing the CEPO command.</param>
        private void PassCepoToSerialNumberGen(string line)
        {
            // Seperate CEPO command
            string[] tokens = line.Split(new string[] { Commands.AdcpCommands.CMD_CEPO }, StringSplitOptions.None);
            if (tokens.Length > 1)
            {
                // Get the list of subsystems from the CEPO command
                string subsystems = tokens[1];

                // Create a subsystem and pass it to Serial number generator
                for (ushort x = 0; x < subsystems.Length; x++)
                {
                    Subsystem ss = new Subsystem(Convert.ToByte(subsystems[x]), x);
                    SerialNumberGeneratorVM.AddSubsystem(ss);
                }

            }

        }

        #endregion

        #region Browse Project Folder

        /// <summary>
        /// Browse for the user to give a folder path for the project.
        /// </summary>
        private void BrowseProjectFolder()
        {
            // Show the FolderBrowserDialog.
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = ProjectDir;
            dialog.Description = "Choose a folder to save project data.";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ProjectDir = dialog.SelectedPath;

                // Store the path to the database to retrieve next time
            }
        }

        #endregion

    }
}
