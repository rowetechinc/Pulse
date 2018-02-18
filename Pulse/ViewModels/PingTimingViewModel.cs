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
    /// Setup the ping timing for an ADCP configuration.
    /// </summary>
    public class PingTimingViewModel : PulseViewModel
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
        /// Adcp Subsystem Configuration key to use in the selected projects
        /// dictionary.
        /// </summary>
        public string ConfigKey { get; protected set; }

        /// <summary>
        /// Number of Pings within an ensemble.
        /// </summary>
        private ushort _CWPP;
        /// <summary>
        /// Number of Pings within an ensemble.
        /// </summary>
        public ushort CWPP
        {
            get { return _CWPP; }
            set
            {
                _CWPP = value;
                this.NotifyOfPropertyChange(() => this.CWPP);

                _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPP = value;
                _pm.SelectedProject.Save();
            }
        }

        /// <summary>
        /// Time between pings.
        /// </summary>
        private float _CWPTBP;
        /// <summary>
        /// Time between pings.
        /// </summary>
        public float CWPTBP
        {
            get { return _CWPTBP; }
            set
            {
                _CWPTBP = value;
                this.NotifyOfPropertyChange(() => this.CWPTBP);

                _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPTBP = value;
                _pm.SelectedProject.Save();
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
        public PingTimingViewModel()
            :base("Ping Timing ViewModel")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();


            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BurstModeView, ConfigKey)));

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
        /// <returns>TRUE = A good key was given.</returns>
        public bool SetConfigKey(object key)
        {
            string configKey = key as string;
            if (configKey != null)
            {
                ConfigKey = configKey;

                // Initialize the values
                return InitializeValues();
            }

            return false;
        }

        #region Initialize

        /// <summary>
        /// Initialize the values from the selected project.
        /// </summary>
        /// <returns>TRUE =  Key found in the dictionary. FALSE = key could not be found in the dictionary.</returns>
        private bool InitializeValues()
        {
            if (_pm.SelectedProject.Configuration.SubsystemConfigDict.ContainsKey(ConfigKey))
            {
                _CWPP = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPP;
                this.NotifyOfPropertyChange(() => this.CWPP);

                _CWPTBP = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CWPTBP;
                this.NotifyOfPropertyChange(() => this.CWPTBP);

                return true;
            }
            else
            {
                // If the key does not exist in the dictionary, go back to the configuration view
                return false;
            }

        }

        #endregion
 
    }
}
