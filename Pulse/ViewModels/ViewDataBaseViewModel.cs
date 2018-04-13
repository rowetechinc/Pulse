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
 * 06/26/2013      RC          3.0.2      Use EventAggregator to receive the ensembles.
 * 06/10/2014      RC          3.3.0      Added a warning if not recording live data.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 09/29/2015      RC          4.2.2      Update warning message using a timer.
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
    using System.Diagnostics;

    /// <summary>
    /// Base ViewModel to display the data to the user.
    /// </summary>
    public class ViewDataBaseViewModel : Conductor<object>, IHandle<EnsembleEvent>
    {
        #region Variables

        /// <summary>
        /// Project manager.
        /// </summary>
        private PulseManager _pm;

        /// <summary>
        /// EventsAggregator.
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Connection to the ADCP.
        /// </summary>
        private AdcpConnection _adcpConn;

        /// <summary>
        /// Timer to display a warning about recording.
        /// </summary>
        private System.Timers.Timer _warningRecordTimer;

        #endregion

        #region Properties

        #region Recording Warning

        /// <summary>
        /// A warning if data is not recording but is being received.
        /// </summary>
        private bool _IsDisplayRecordingWarning;
        /// <summary>
        /// A warning if data is not recording but is being received.
        /// </summary>
        public bool IsDisplayRecordingWarning
        {
            get { return _IsDisplayRecordingWarning; }
            set
            {
                _IsDisplayRecordingWarning = value;
                this.NotifyOfPropertyChange(() => this.IsDisplayRecordingWarning);
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to configure the selected ADCP.  This will require
        /// a ProjectName is given.
        /// </summary>
        public ReactiveCommand<object> HomeViewCommand { get; protected set; }

        /// <summary>
        /// Command to view graphical data.
        /// </summary>
        public ReactiveCommand<object> GraphicalViewCommand { get; protected set; }

        /// <summary>
        /// Commnad to view Text data.
        /// </summary>
        public ReactiveCommand<object> TextViewCommand { get; protected set; }

        /// <summary>
        /// Commnad to view DVL data.
        /// </summary>
        public ReactiveCommand<object> DvlViewCommand { get; protected set; }

        /// <summary>
        /// Commnad to view Backscatter data.
        /// </summary>
        public ReactiveCommand<object> BackscatterViewCommand { get; protected set; }

        /// <summary>
        /// Command to view Diagnostic data.
        /// </summary>
        public ReactiveCommand<object> DiagnosticViewCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize values.
        /// </summary>
        public ViewDataBaseViewModel()
        {
            // Project Manager
            _pm = IoC.Get<PulseManager>();
            _events = IoC.Get<IEventAggregator>();
            _events.Subscribe(this);
            _adcpConn = IoC.Get<AdcpConnection>();

            // Initialize for warning when not recording live data
            IsDisplayRecordingWarning = false;

            // Warning timer
            _warningRecordTimer = new System.Timers.Timer();
            _warningRecordTimer.Interval = 5000;                // 5 seconds.
            _warningRecordTimer.Elapsed += _warningRecordTimer_Elapsed;
            _warningRecordTimer.AutoReset = true;

            // Command to view the ViewData page
            HomeViewCommand = ReactiveCommand.Create();
            HomeViewCommand.Subscribe(_ => IoC.Get<IEventAggregator>().PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Command to view Graphical data
            GraphicalViewCommand = ReactiveCommand.Create();
            GraphicalViewCommand.Subscribe(_ => ActivateItem(IoC.Get<ViewDataBaseGraphicalViewModel>()));

            // Command to view Graphical data
            TextViewCommand = ReactiveCommand.Create();
            TextViewCommand.Subscribe(_ => ActivateItem(IoC.Get<ViewDataBaseTextViewModel>()));

            // Command to view DVL data
            DvlViewCommand = ReactiveCommand.Create();
            DvlViewCommand.Subscribe(_ => ActivateItem(IoC.Get<ViewDataBaseDvlViewModel>()));

            // Command to view DVL data
            BackscatterViewCommand = ReactiveCommand.Create();
            BackscatterViewCommand.Subscribe(_ => ActivateItem(IoC.Get<ViewDataBaseBackscatterViewModel>()));

            // Command to view Diagnotics data
            DiagnosticViewCommand = ReactiveCommand.Create();
            DiagnosticViewCommand.Subscribe(_ => ActivateItem(IoC.Get<DiagnosticsBaseViewModel>()));

            // Display Graphical view by default
            ActivateItem(IoC.Get<ViewDataBaseGraphicalViewModel>());
        }

        /// <summary>
        /// Display the warning that the user is not recording.  This is enabled based off the incoming ensemble data state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _warningRecordTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                IsDisplayRecordingWarning = true;
                System.Threading.Thread.Sleep(1000);
                IsDisplayRecordingWarning = false;
            });
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public void Dispose()
        {

        }

        #region EventHandlers

        /// <summary>
        /// Handle event when EnsembleEvent is received.
        /// This will create the displays for each config
        /// if it has not been created already.  It will also
        /// display the latest ensemble.
        /// </summary>
        /// <param name="ensEvent">Ensemble event.</param>
        public void Handle(EnsembleEvent ensEvent)
        {
            if (ensEvent.Ensemble != null)
            {
                // Update timer
                if (!_adcpConn.IsRecording &&                                           // Not recording
                    !_adcpConn.IsValidationTestRecording &&                             // Not Validation test recording
                    ensEvent.Source != EnsembleSource.Playback)                         // Not playing back data
                {
                    _warningRecordTimer.Enabled = true;
                }
                else 
                {
                    _warningRecordTimer.Enabled = false;
                }


            }
        }

        #endregion
    }
}
