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
 * 12/05/2013      RC          3.2.0      Initial coding
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

    /// <summary>
    /// Set the burst mode commands.
    /// </summary>
    public class BurstModeViewModel : PulseViewModel
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
        /// Burst Interval hour.
        /// </summary>
        private UInt16 _CBI_BurstInterval_Hour;
        /// <summary>
        /// Burst Interval hour.
        /// </summary>
        public UInt16 CBI_BurstInterval_Hour
        {
            get { return _CBI_BurstInterval_Hour; }
            set
            {
                _CBI_BurstInterval_Hour = value;
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Hour);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval = new RTI.Commands.TimeValue(_CBI_BurstInterval_Hour, _CBI_BurstInterval_Minute, _CBI_BurstInterval_Seconds, 0);
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Burst Interval Minute.
        /// </summary>
        private UInt16 _CBI_BurstInterval_Minute;
        /// <summary>
        /// Burst Interval Minute.
        /// </summary>
        public UInt16 CBI_BurstInterval_Minute
        {
            get { return _CBI_BurstInterval_Minute; }
            set
            {
                _CBI_BurstInterval_Minute = value;
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Minute);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval = new RTI.Commands.TimeValue(_CBI_BurstInterval_Hour, _CBI_BurstInterval_Minute, _CBI_BurstInterval_Seconds, 0);
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Burst Interval Seconds.
        /// </summary>
        private UInt16 _CBI_BurstInterval_Seconds;
        /// <summary>
        /// Burst Interval Seconds.
        /// </summary>
        public UInt16 CBI_BurstInterval_Seconds
        {
            get { return _CBI_BurstInterval_Seconds; }
            set
            {
                _CBI_BurstInterval_Seconds = value;
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Seconds);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval = new RTI.Commands.TimeValue(_CBI_BurstInterval_Hour, _CBI_BurstInterval_Minute, _CBI_BurstInterval_Seconds, 0);
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Number of ensembles within a burst.
        /// </summary>
        private ushort _CBI_NumEnsembles;
        /// <summary>
        /// Number of ensembles within a burst.
        /// </summary>
        public ushort CBI_NumEnsembles
        {
            get { return _CBI_NumEnsembles; }
            set
            {
                _CBI_NumEnsembles = value;
                this.NotifyOfPropertyChange(() => this.CBI_NumEnsembles);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_NumEnsembles = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        /// <summary>
        /// Interleave the burst interval subsystem configurations flag.
        /// </summary>
        private bool _CBI_BurstPairFlag;
        /// <summary>
        /// Interleave the burst interval subsystem configurations flag.
        /// </summary>
        public bool CBI_BurstPairFlag
        {
            get { return _CBI_BurstPairFlag; }
            set
            {
                _CBI_BurstPairFlag = value;
                this.NotifyOfPropertyChange(() => this.CBI_BurstPairFlag);

                if (_pm.IsProjectSelected)
                {
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstPairFlag = value;
                    _pm.SelectedProject.Save();
                }
            }
        }

        #endregion

                /// <summary>
        /// Initialize the view model.
        /// </summary>
        public BurstModeViewModel()
            :base("Bins ViewModel")
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

                if (!_pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
                {
                    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpConfigurationView));
                    return;
                }

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
            if (_pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
            {
                _CBI_BurstInterval_Hour = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval.Hour;
                _CBI_BurstInterval_Minute = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval.Minute;
                _CBI_BurstInterval_Seconds = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstInterval.Second;
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Hour);
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Minute);
                this.NotifyOfPropertyChange(() => this.CBI_BurstInterval_Seconds);


                _CBI_NumEnsembles = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_NumEnsembles;
                this.NotifyOfPropertyChange(() => this.CBI_NumEnsembles);

                _CBI_BurstPairFlag = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBI_BurstPairFlag;
                this.NotifyOfPropertyChange(() => this.CBI_BurstPairFlag);
            }
        }

        #endregion


    }
}
