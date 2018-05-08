﻿/*
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
 * 08/19/2013      RC          3.0.7      Initial coding
 * 08/23/2013      RC          3.0.7      Added ScreenDataCommand.
 * 02/12/2014      RC          3.2.3      Added VesselMountCommand.
 * 07/09/2014      RC          3.4.0      Added AveragingCommand.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 11/24/2015      RC          4.3.1      Select ENS and BIN as default options for playback files.
 * 12/03/2015      RC          4.4.0      Added record button and recording to file to the record button.
 * 10/23/2017      RC          4.6.1      In LoadFiles() first check if its a binary file, then try all codec types. 
 * 05/01/2018      RC          4.11.0     Moved loading data with CreateProject() for playback to PulseManager.
 * 
 */

using Caliburn.Micro;
using ReactiveUI;

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Navigation bar.
    /// </summary>
    public class NavBarViewModel : PulseViewModel
    {

        #region Variables

        /// <summary>
        ///  Setup logger
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Project manager.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// Event aggregator.
        /// </summary>
        private readonly IEventAggregator _events;

        /// <summary>
        /// Connection to the ADCP.
        /// </summary>
        private AdcpConnection _adcpConn;

        /// <summary>
        /// Timer to display updated recording size.
        /// </summary>
        private System.Timers.Timer _recorderTimer;

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

        #region Test Flag

        /// <summary>
        /// Flag True when testing is in progress.
        /// This start to calculate the DMG.
        /// </summary>
        public bool IsTesting
        {
            get { return _adcpConn.IsValidationTestRecording; }
        }

        /// <summary>
        /// Check the toggle button.
        /// </summary>
        private bool _IsRecording;
        /// <summary>
        /// Check the toggle button.
        /// </summary>
        public bool IsRecording
        {
            get { return _IsRecording; }
            set
            {
                _IsRecording = value;
                this.NotifyOfPropertyChange(() => this.IsRecording);

                if(value)
                {
                    // Record data
                    StartTestingCommand.Execute(null);
                }
                else
                {
                    // Stop Recording data
                    StopTestingCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Validation Test recording size.
        /// </summary>
        public string RecordingSize
        {
            get { return MathHelper.MemorySizeString(_adcpConn.ValidationTestBytesWritten); }
        }

        #endregion

        /// <summary>
        /// String for the image to display for the
        /// record button.  The button can show a record
        /// on or off and a blink.  On is red, off is black.
        /// Blink is blue.
        /// </summary>
        private string _RecordImage;
        /// <summary>
        /// Record Button image.
        /// </summary>
        public string RecordImage
        {
            get { return _RecordImage; }
            set
            {
                _RecordImage = value;
                this.NotifyOfPropertyChange(() => this.RecordImage);
            }
        }

        #endregion 

        #region Commands

        /// <summary>
        /// Command to go back in the application.
        /// </summary>
        public ReactiveCommand<object> BackCommand { get; protected set; }

        /// <summary>
        /// Command to go Home View.
        /// </summary>
        public ReactiveCommand<object> HomeCommand { get; protected set; }

        /// <summary>
        /// Command to go Configure View.
        /// </summary>
        public ReactiveCommand<object> ConfigureCommand { get; protected set; }

        /// <summary>
        /// Command to go ViewData View.
        /// </summary>
        public ReactiveCommand<object> ViewDataCommand { get; protected set; }

        /// <summary>
        /// Command to go playback.
        /// </summary>
        public ReactiveCommand<object> PlaybackCommand { get; protected set; }

        /// <summary>
        /// Command to go ScreenData View.
        /// </summary>
        public ReactiveCommand<object> ScreenDataCommand { get; protected set; }

        /// <summary>
        /// Command to go ScreenData View.
        /// </summary>
        public ReactiveCommand<object> ProjectCommand { get; protected set; }

        /// <summary>
        /// Command to go Vessel Mount Options View.
        /// </summary>
        public ReactiveCommand<object> VmOptionsCommand { get; protected set; }

        /// <summary>
        /// Command to go Data Format View.
        /// </summary>
        public ReactiveCommand<object> DataFormatCommand { get; protected set; }

        /// <summary>
        /// Command to go Averaging view.
        /// </summary>
        public ReactiveCommand<object> AveragingCommand { get; protected set; }

        /// <summary>
        /// Command to start the testing.
        /// This will begin the recording process.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> StartTestingCommand { get; protected set; }

        /// <summary>
        /// Command to stop the testing.
        /// This will stop the recording process.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> StopTestingCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initalize values.
        /// </summary>
        public NavBarViewModel()
            : base("Nav")
        {
            _pm = IoC.Get<PulseManager>();
            _events = IoC.Get<IEventAggregator>();
            _adcpConn = IoC.Get<AdcpConnection>();
            _IsRecording = false;
            this.NotifyOfPropertyChange(() => this.IsRecording);

            // Set the record image
            SetRecorderImage();

            // Warning timer
            _recorderTimer = new System.Timers.Timer();
            _recorderTimer.Interval = 2000;                // 2 seconds.
            _recorderTimer.Elapsed += _recorderTimer_Elapsed;
            _recorderTimer.AutoReset = true;
            _recorderTimer.Start();

            // Command to go back a view
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Command to go to Home View
            HomeCommand = ReactiveCommand.Create();
            HomeCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Command to go to SmartPage View
            ConfigureCommand = ReactiveCommand.Create();
            ConfigureCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SmartPageView)));

            // Command to go to ViewData View
            ViewDataCommand = ReactiveCommand.Create();
            ViewDataCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView)));

            //// Command to go to Playback data
            //PlaybackCommand = ReactiveCommand.Create();
            //PlaybackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SelectPlaybackView)));

            // Select a file to playback
            PlaybackCommand = ReactiveCommand.Create();
            PlaybackCommand.Subscribe(_ => PlaybackFile());

            // Command to go to ScreenData View
            ScreenDataCommand = ReactiveCommand.Create();
            ScreenDataCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ScreenDataView)));

            // Command to go to Project View
            ProjectCommand = ReactiveCommand.Create();
            ProjectCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ProjectView)));

            // Command to go to VesselMount Options View
            VmOptionsCommand = ReactiveCommand.Create();
            VmOptionsCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.VesselMountOptionsView)));

            // Command to go to Data Format View
            DataFormatCommand = ReactiveCommand.Create();
            DataFormatCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DataFormatView)));

            // Command to go to VesselMount Options View
            AveragingCommand = ReactiveCommand.Create();
            AveragingCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AveragingView)));

            // Set the Clock time to Local System time on the ADCP
            StartTestingCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsTesting, x => !x.Value),
                                                                    _ => Task.Run(() => On_StartTesting()));

            // Create a command to stop testing
            StopTestingCommand = ReactiveCommand.CreateAsyncTask(this.WhenAny(x => x.IsTesting, x => x.Value),
                                                                _ => Task.Run(() => On_StopTesting()));
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            if(_IsRecording)
            {
                IsRecording = false;
            }
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
                //dialog.InitialDirectory = Pulse.Commons.DEFAULT_RECORD_DIR;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Load the file
                    await Task.Run(() => LoadFiles(dialog.FileNames));

                    // Go to the view page
                    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ViewDataView));
                }
            }
            catch(AccessViolationException ae)
            {
                log.Error("Error trying to open file", ae);
            }
            catch(Exception e)
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

            if(files.Length > 0)
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

        #region Start Testing

        /// <summary>
        /// Start Testing.
        /// </summary>
        private void On_StartTesting()
        {
            // Set flag to start recording
            //IsTesting = true;

            // Start recording
            _adcpConn.StartValidationTest(Pulse.Commons.DEFAULT_RECORD_DIR);

            SetRecorderImage();
        }



        #endregion

        #region Stop Testing

        /// <summary>
        /// Stop Testing.
        /// Record the results.
        /// </summary>
        private void On_StopTesting()
        {
            // Set flag that we are recording
            //IsTesting = false;

            // Stop recording
            _adcpConn.StopValidationTest();

            SetRecorderImage();
        }

        #endregion

        #region Recording Size

        /// <summary>
        /// Update the recording size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _recorderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(_adcpConn.IsValidationTestRecording != _IsRecording)
            {
                _IsRecording = _adcpConn.IsValidationTestRecording;
                this.NotifyOfPropertyChange(() => this.IsRecording);

                // Set the image
                SetRecorderImage();
            }

            this.NotifyOfPropertyChange(() => this.RecordingSize);
        }

        #endregion

        #region Record

        /// <summary>
        /// Set the recorder image based off 
        /// IsRecordEnabled set or not.
        /// </summary>
        private void SetRecorderImage()
        {
            // Set Image
            if (_IsRecording)
            {
                RecordImage = Pulse.Commons.RECORD_IMAGE_ON;
            }
            else
            {
                RecordImage = Pulse.Commons.RECORD_IMAGE_OFF;
            }
        }

        #endregion

    }
}
