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

    /// <summary>
    /// Set the time of first ping and the ADCP time.
    /// </summary>
    public class TimeViewModel : PulseViewModel
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

        #region Class and Enum

        /// <summary>
        /// Options to select a time zone for the STIME command.
        /// </summary>
        public class AdcpTimeZoneOptions
        {
            /// <summary>
            /// Time Zone for this option.
            /// </summary>
            public RTI.Commands.AdcpTimeZone TimeZone { get; set; }

            /// <summary>
            /// Description for this option.
            /// </summary>
            public string Desc { get; set; }

            /// <summary>
            /// Set if this mode is selected.
            /// </summary>
            public bool IsSelected { get; set; }

            /// <summary>
            /// Option to select a time zone.
            /// </summary>
            /// <param name="tz">Time Zone.</param>
            /// <param name="desc">Description.</param>
            /// <param name="isSelected">Selected.</param>
            public AdcpTimeZoneOptions(RTI.Commands.AdcpTimeZone tz, string desc, bool isSelected)
            {
                TimeZone = tz;
                Desc = desc;
                IsSelected = isSelected;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Time of first ping.
        /// </summary>
        private DateTime _CETFP;
        /// <summary>
        /// Time of first ping.
        /// </summary>
        public DateTime CETFP
        {
            get { return _CETFP; }
            set
            {
                _CETFP = value;
                this.NotifyOfPropertyChange(() => this.CETFP);

                _pm.SelectedProject.Configuration.Commands.CETFP = value;
                _pm.SelectedProject.Save();
            }
        }

        /// <summary>
        /// System Time.  This is the date and time of the ADCP.
        /// </summary>
        private DateTime _STIME;
        /// <summary>
        /// System Time.  This is the date and time of the ADCP.
        /// </summary>
        public DateTime STIME
        {
            get { return _STIME; }
            set
            {
                _STIME = value;
                this.NotifyOfPropertyChange(() => this.STIME);
            }
        }

        /// <summary>
        /// List of time zone options.
        /// </summary>
        public ReactiveList<AdcpTimeZoneOptions> TimeZoneList { get; protected set; }

        /// <summary>
        /// Selected Time Zone option.
        /// </summary>
        private AdcpTimeZoneOptions _SelectedTimeZone;
        /// <summary>
        /// Selected Time Zone option.
        /// </summary>
        public AdcpTimeZoneOptions SelectedTimeZone
        {
            get { return _SelectedTimeZone; }
            set
            {
                if (_pm.IsProjectSelected)
                {
                    if (value == null)
                    {
                        value = new AdcpTimeZoneOptions(Commands.AdcpTimeZone.LOCAL, "Local", true);
                    }

                    _pm.SelectedProject.Configuration.Commands.TimeZone = value.TimeZone;
                    _pm.SelectedProject.Save();
                }

                _SelectedTimeZone = value;
                this.NotifyOfPropertyChange(() => this.SelectedTimeZone);
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
        /// Set the Time of First ping to current local date and time.
        /// </summary>
        public ReactiveCommand<object> SetLocalDateTimeCommand { get; protected set; }

        /// <summary>
        /// Set the Time of First ping to current GMT date and time.
        /// </summary>
        public ReactiveCommand<object> SetGmtDateTimeCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public TimeViewModel()
            : base("ADCP Time")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Initialize the list
            TimeZoneList = new ReactiveList<AdcpTimeZoneOptions>();
            TimeZoneList.Add(new AdcpTimeZoneOptions(RTI.Commands.AdcpTimeZone.LOCAL, "Local", true));
            TimeZoneList.Add(new AdcpTimeZoneOptions(RTI.Commands.AdcpTimeZone.GMT, "GMT", false));

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.EnsembleIntervalView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Local Date Time Command
            SetLocalDateTimeCommand = ReactiveCommand.Create();
            SetLocalDateTimeCommand.Subscribe(_ => SetLocalDateTime());

            // GMT Date Time Command
            SetGmtDateTimeCommand = ReactiveCommand.Create();
            SetGmtDateTimeCommand.Subscribe(_ => SetGmtDateTime());

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
            _CETFP = _pm.SelectedProject.Configuration.Commands.CETFP;
            this.NotifyOfPropertyChange(() => this.CETFP);

            _STIME = DateTime.Now;
            this.NotifyOfPropertyChange(() => this.STIME);

            if (_pm.IsProjectSelected)
            {
                if (_pm.SelectedProject.Configuration.Commands.TimeZone == Commands.AdcpTimeZone.LOCAL)
                {
                    SelectedTimeZone = TimeZoneList[0];
                    SelectedTimeZone.IsSelected = true;
                }
                else if (_pm.SelectedProject.Configuration.Commands.TimeZone == Commands.AdcpTimeZone.GMT)
                {
                    SelectedTimeZone = TimeZoneList[1];
                    SelectedTimeZone.IsSelected = true;
                }
                
            }
        }

        #endregion

        #region Time of First Ping

        /// <summary>
        /// Set the Time of First ping to the local time.
        /// </summary>
        private void SetLocalDateTime()
        {
            CETFP = DateTime.Now;
        }

        /// <summary>
        /// Set the Time of First ping to the GMT time.
        /// </summary>
        private void SetGmtDateTime()
        {
            CETFP = DateTime.Now.ToUniversalTime();
        }

        #endregion

        #region ADCP Time



        #endregion

    }
}
