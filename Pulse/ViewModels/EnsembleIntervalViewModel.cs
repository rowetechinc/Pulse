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
* 11/07/2013      RC          3.2.0      Initial coding
* 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
* 10/31/2016      RC          4.4.17     Fixed the CEI command only being an integer.
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
    public class EnsembleIntervalViewModel : PulseViewModel
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

        #endregion

        #region Properties

        /// <summary>
        /// Ensemble Interval in seconds.
        /// </summary>
        private double _CEI;
        /// <summary>
        /// Ensemble Interval in seconds.
        /// </summary>
        public double CEI
        {
            get { return _CEI; }
            set
            {
                _CEI = value;
                this.NotifyOfPropertyChange(() => this.CEI);

                // Display the time span string
                DisplayTimeSpan(value);

                // Update the project
                _pm.SelectedProject.Configuration.Commands.CEI = new RTI.Commands.TimeValue((float)value);
                _pm.SelectedProject.Save();
            }
        }

        /// <summary>
        /// Time span as a string
        /// </summary>
        private string _CEI_Timespan;
        /// <summary>
        /// Time span as a string
        /// </summary>
        public string CEI_Timespan
        {
            get { return _CEI_Timespan; }
            set
            {
                _CEI_Timespan = value;
                this.NotifyOfPropertyChange(() => this.CEI_Timespan);
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

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public EnsembleIntervalViewModel()
            : base("Ensemble Interval")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpConfigurationView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            InitializeValue();
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Initialize

        /// <summary>
        /// Initialize the value.
        /// </summary>
        private void InitializeValue()
        {
            _CEI = _pm.SelectedProject.Configuration.Commands.CEI.ToSeconds();
            this.NotifyOfPropertyChange(() => this.CEI);

            // Display the timespan
            DisplayTimeSpan(_CEI);
        }

        #endregion

        #region Date Time Display

        /// <summary>
        /// Create a pretty timespan for the given seconds.
        /// </summary>
        /// <param name="seconds">Number of seconds.</param>
        private void DisplayTimeSpan(double seconds)
        {
            int milliSec = (int)(Math.Round(seconds * 1000));
            TimeSpan ts = new TimeSpan(0,0,0,0, milliSec);
            CEI_Timespan = MathHelper.TimeSpanPrettyFormat(ts);

        }

        #endregion

    }
}
