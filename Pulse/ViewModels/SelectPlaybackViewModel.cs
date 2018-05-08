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
 * 09/03/2014      RC          4.0.3      Initial coding
 * 11/24/2015      RC          4.3.1      Select ENS and BIN as default options for playback files.
 * 10/23/2017      RC          4.6.1      Made File playback match NavBarViewModel's file playback.
 * 05/01/2018      RC          4.11.0     Moved loading data with CreateProject() for playback to PulseManager.
 * 
 * 
 */

using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTI
{


    /// <summary>
    /// Select a way to playback data.
    /// </summary>
    class SelectPlaybackViewModel : PulseViewModel
    {

        #region Variables

        // Setup logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Project manager.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        #endregion

        #region Properties

        /// <summary>
        /// Set flag if the files are loading.
        /// This is true when we are importing files.
        /// </summary>
        private bool _IsLoading;
        /// <summary>
        /// Set flag if the files are loading.
        /// This is true when we are importing or removing files.
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
        /// Command to select a file to playback.
        /// </summary>
        public ReactiveCommand<object> FilePlaybackCommand { get; protected set; }

        /// <summary>
        /// Go back a screen.
        /// </summary>
        public ReactiveCommand<object> ProjectPlaybackCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Select whether to playback a file or a project.
        /// </summary>
        public SelectPlaybackViewModel()
            : base("Select Playback")
        {
            // Initialize values
            _pm = IoC.Get<PulseManager>();
            _events = IoC.Get<IEventAggregator>();

            // Initialize values
            IsLoading = false;

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Select a file to playback
            FilePlaybackCommand = ReactiveCommand.Create();
            FilePlaybackCommand.Subscribe(_ => PlaybackFile());

            // Project coommand
            ProjectPlaybackCommand = ReactiveCommand.Create();
            ProjectPlaybackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.LoadProjectsView)));
        }

        /// <summary>
        /// Dispose of the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        /// <summary>
        /// Have the user select a file to playback.  Then set the 
        /// playback to the playback base in AdcpConnection.
        /// </summary>
        private async void PlaybackFile()
        {
            try
            {
                // Show the FolderBrowserDialog.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Ensemble files (*.bin, *.ens)|*.bin; *.ens|BIN files (*.bin)|*.bin|ENS files (*.ens)|*.ens|All files (*.*)|*.*";
                dialog.Multiselect = true;
                dialog.InitialDirectory = Pulse.Commons.DEFAULT_RECORD_DIR;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Load the file
                    await Task.Run(() => LoadFiles(dialog.FileNames));

                    // Go to the view page
                    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView));
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
        /// Load the files.
        /// </summary>
        /// <param name="files">Files selected.</param>
        private void LoadFiles(string[] files)
        {
            // Set flag
            IsLoading = true;

            //// Create the file playback based off the selected file
            //FilePlayback fp = new FilePlayback();
            //fp.FindEnsembles(files);

            //// Wait for ensembles to be added
            //int timeout = 10;
            //while (fp.TotalEnsembles < 0 && timeout >= 0)
            //{
            //    System.Threading.Thread.Sleep(250);
            //    timeout--;
            //}

            //// Set the selected playback to the pulsemanager
            //_pm.SelectedPlayback = fp;

            if (files.Length > 0)
            {
                // Create the file playback based off the selected file
                // Try to optimize and first load the file into the Binary only codec
                // If this does not work, then try all the codecs
                FilePlayback fp = new FilePlayback();
                fp.FindRtbEnsembles(files);

                // Wait for ensembles to be added
                int timeout = 10;
                while (fp.TotalEnsembles < 0 && timeout >= 0)
                {
                    System.Threading.Thread.Sleep(250);
                    timeout--;
                }

                // Check if any ensembles were found
                if (fp.TotalEnsembles > 0)
                {
                    // Add the ensembles to the project
                    // Create a project if new, or load if old
                    var project = CreateProject(files[0], fp.GetAllEnsembles());

                    // Set the selected playback to the pulsemanager
                    _pm.SelectedProject = project;
                    //_pm.SelectedPlayback = fp;
                }
                else
                {
                    // Find the ensembles using all the codecs
                    fp.FindEnsembles(files);

                    var project = CreateProject(files[0], fp.GetAllEnsembles());

                    // Set the selected playback to the pulsemanager
                    _pm.SelectedProject = project;
                    //_pm.SelectedPlayback = fp;

                }
            }

            // Reset flag
            IsLoading = false;
        }

        #region Project

        /// <summary>
        /// Create a new project with the file name as the project name.
        /// Add all the ensembles to the project.
        /// If the project exist, and the same number of ensembles are in the 
        /// project, it will return the project.
        /// If the project exist and there is a different number of ensembles, 
        /// a new project will be created with file name and the date and time
        /// added to the end of the file name.
        /// </summary>
        /// <param name="filepath">File name to use as the project name.</param>
        /// <param name="ensembles">Ensembles to add to the project.</param>
        /// <returns>Project with ensemble data.</returns>
        private Project CreateProject(string filepath, Cache<long, DataSet.Ensemble> ensembles)
        {
            // Create a project or reload an old project
            return _pm.CreateProject(filepath, ensembles);
        }

        #endregion

    }
}
