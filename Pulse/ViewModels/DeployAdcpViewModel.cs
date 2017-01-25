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
 * 11/15/2013      RC          3.2.0      Initial coding
 * 06/02/2014      RC          3.3.0      Added saving the commands to a text file. 
 * 06/11/2014      RC          3.3.1      In SaveCommandsToFile(), include the date and time to the command text file.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
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
    using System.Windows;
    using System.Threading.Tasks;

    /// <summary>
    /// Allow the user to deploy the ADCP with the set commands.
    /// </summary>
    public class DeployAdcpViewModel : PulseViewModel
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
        /// Connection to the ADCP.
        /// </summary>
        private AdcpConnection _adcpConnection;

        #endregion

        #region Properties

        /// <summary>
        /// Additional commands the user might want to pass to the ADCP.
        /// </summary>
        private string _AdditionalCommands;
        /// <summary>
        /// Additional commands the user might want to pass to the ADCP.
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

        /// <summary>
        /// Flag to know if the scan is still loading.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Flag to know if the scan is still loading.
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
        /// Command to send all the commands to the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> SendCommandsCommand { get; protected set; }

        /// <summary>
        /// COmmand to view all the commands that will be send to the ADCP.
        /// </summary>
        public ReactiveCommand<object> ViewCommandsCommand { get; protected set; }

        /// <summary>
        /// Save the commands to a text file.
        /// </summary>
        public ReactiveCommand<object> SaveCmdsCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view.
        /// </summary>
        public DeployAdcpViewModel()
            : base("Deploy ADCP")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();
            _adcpConnection = IoC.Get<AdcpConnection>();
            AdditionalCommands = "";
            IsLoading = false;

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsLoading, x => !x.Value));
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView)));

            // Back command
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit command
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Command to send commands to ADCP
            SendCommandsCommand = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => ConfigureAdcp()));

            // Command to view all the commands
            ViewCommandsCommand = ReactiveCommand.Create();
            ViewCommandsCommand.Subscribe(_ => ShowCommands());

            // Save the commands to a text file
            SaveCmdsCommand = ReactiveCommand.Create();
            SaveCmdsCommand.Subscribe(_ => SaveCommandsToFile());
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Configure Adcp

        /// <summary>
        /// Get all the commands that are necessary to configure the
        /// ADCP.  Validate the additional commands given.
        /// Then pass all the commands to the ADCP.
        /// </summary>
        private void ConfigureAdcp()
        {
            if (_adcpConnection.IsAdcpSerialConnected())
            {
                IsLoading = true;

                // Get all the commands
                List<string> commands = GetCommands();

                // Start the ADCP pinging
                //commands.Add(Commands.AdcpCommands.CMD_START_PINGING);

                // Save the commands to the file
                SaveCommandsToFile();

                // Send all the commands to the ADCP
                _adcpConnection.SendCommands(commands);

                // Decode CSHOW again to get all the additional commands given
                //CheckAllOptionsFromAdcp();

                // Start Pinging
                _adcpConnection.StartPinging();

                IsLoading = false;

                // Display message that is is ok to disconnect the ADCP
                MessageBox.Show("It is OK to disconnect the ADCP now.", "Configuration Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                // Change the view to the ViewData view
                //_events.Publish(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView));
            }
            else
            {
                // Display message that it could not send the commands
                MessageBox.Show("An ADCP is not connected.\nCannot send the commands.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Addtional Commands

        /// <summary>
        /// Verify each command in AdditionalCommands.
        /// This will break up each line into a command.
        /// It will then verify each command.  If the command
        /// is good, it will be added to the list.
        /// </summary>
        /// <returns>List of all the additional commands that were validated.</returns>
        private List<string> VerifyAdditionalCommands()
        {
            List<string> commands = new List<string>();
            // Break up the commands per line
            // verify each line

            string[] lines = AdditionalCommands.Split('\n');

            return lines.ToList();
        }

        #endregion

        #region Show Commands

        /// <summary>
        /// Show the deployment report about the ADCP.
        /// </summary>
        private bool ShowCommands()
        {
            // Show report of deployment based off the configuration
            ShowAdcpCommandsViewModel vm = new ShowAdcpCommandsViewModel(GetCommands());
            IoC.Get<IWindowManager>().ShowDialog(vm);

            if (vm.IsOk)
            {
                return true;
            }

            return false;
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
            commands.AddRange(VerifyAdditionalCommands());

            // Save the commands to the ADCP
            commands.Add(Commands.AdcpCommands.CMD_CSAVE);

            return commands;
        }

        #endregion

        #region Save Commands to File

        /// <summary>
        /// Save all the commands to a text file in the project.
        /// </summary>
        private void SaveCommandsToFile()
        {
            try
            {
                // Check if a project is selected
                if (_pm.IsProjectSelected)
                {
                    // Get the project dir
                    // Create the file name
                    string prjDir = _pm.SelectedProject.ProjectFolderPath;
                    DateTime now = DateTime.Now;
                    string year = now.Year.ToString("0000");
                    string month = now.Month.ToString("00");
                    string day = now.Day.ToString("00");
                    string hours = now.Hour.ToString("00");
                    string minutes = now.Minute.ToString("00");
                    string seconds = now.Second.ToString("00");
                    string fileName = string.Format("Commands_{0}{1}{2}{3}{4}{5}.txt", year, month, day, hours, minutes, seconds);
                    string cmdFilePath = prjDir + @"\" + fileName;

                    // Get the commands
                    string[] lines = GetCommands().ToArray();

                    // Create a text file in the project
                    System.IO.File.WriteAllLines(cmdFilePath, lines);
                }
            }
            catch (Exception) { }
        }

        #endregion

    }
}
