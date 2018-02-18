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
 * 11/01/2013      RC          3.2.0      Initial coding
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
    /// Set the bins values for each subsystem configuration.
    /// </summary>
    public class BinsViewModel : PulseViewModel
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

        #region Properties

        /// <summary>
        /// Adcp Subsystem Configuration key to use in the selected projects
        /// dictionary.
        /// </summary>
        public string ConfigKey { get; protected set; }

        /// <summary>
        /// Blank size in meters.
        /// </summary>
        private float _CWPBL;
        /// <summary>
        /// Blank size in meters.
        /// </summary>
        public float CWPBL
        {
            get { return _CWPBL; }
            set
            {
                _CWPBL = value;
                this.NotifyOfPropertyChange(() => this.CWPBL);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBL = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Bin Size in meters.
        /// </summary>
        private float _CWPBS;
        /// <summary>
        /// Bin Size in meters.
        /// </summary>
        public float CWPBS
        {
            get { return _CWPBS; }
            set
            {
                _CWPBS = value;
                this.NotifyOfPropertyChange(() => this.CWPBS);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBS = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Number of bins.
        /// </summary>
        private ushort _CWPBN;
        /// <summary>
        /// Number of bins.
        /// </summary>
        public ushort CWPBN
        {
            get { return _CWPBN; }
            set
            {
                _CWPBN = value;
                this.NotifyOfPropertyChange(() => this.CWPBN);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBN = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public BinsViewModel()
            :base("Bins ViewModel")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.PingTimingView, ConfigKey)));

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

        /// <summary>
        /// Set the configuration key.
        /// </summary>
        /// <param name="key">Key to convert to a string.</param>
        public void SetConfigKey(object key)
        {
            string configKey = key as string;
            if (configKey != null)
            {
                ConfigKey = configKey;

                // Initialize the values
                InitializeValues();
            }
        }

        #region Initialize

        /// <summary>
        /// Initialize the values from the selected project.
        /// </summary>
        private void InitializeValues()
        {
            _CWPBL = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBL;
            this.NotifyOfPropertyChange(() => this.CWPBL);

            _CWPBS = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBS;
            this.NotifyOfPropertyChange(() => this.CWPBS);

            _CWPBN = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPBN;
            this.NotifyOfPropertyChange(() => this.CWPBN);
        }

        #endregion

    }
}
