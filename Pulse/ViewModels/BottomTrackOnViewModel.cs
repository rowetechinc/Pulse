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
 * 10/31/2013      RC          3.2.0      Initial coding
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
    /// Turn on or off Bottom Track for the subsystem configuraiton.
    /// </summary>
    public class BottomTrackOnViewModel : PulseViewModel
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
        /// Broadband modes.
        /// </summary>
        public class BottomTrackOption
        {
            /// <summary>
            /// Title for the mode.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Description of the mode.
            /// </summary>
            public string Desc { get; set; }

            /// <summary>
            /// Value for the option.
            /// </summary>
            public bool Value { get; set; }

            /// <summary>
            /// Path to the image.
            /// </summary>
            public string Image { get; set; }

            /// <summary>
            /// Initialize the object.
            /// </summary>
            /// <param name="title">Bottom Track title.</param>
            /// <param name="desc">Descpition of the option.</param>
            /// <param name="value">Value for the option.</param>
            /// <param name="image">Image for the option.</param>
            public BottomTrackOption(string title, string desc, bool value, string image)
            {
                Title = title;
                Desc = desc;
                Value = value;
                Image = image;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Adcp Subsystem Configuration key to use in the selected projects
        /// dictionary.
        /// </summary>
        public string ConfigKey { get; protected set; }

        /// <summary>
        /// List of bottom track options.
        /// </summary>
        public ReactiveList<BottomTrackOption> OptionList { get; protected set; }

        /// <summary>
        /// Selected Option.
        /// </summary>
        private BottomTrackOption _SelectedOption;
        /// <summary>
        /// Selected Option.
        /// </summary>
        public BottomTrackOption SelectedOption
        {
            get { return _SelectedOption; }
            set
            {
                _SelectedOption = value;
                this.NotifyOfPropertyChange(() => this.SelectedOption);

                if (_SelectedOption != null)
                {
                    // Set the value to the config
                    _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON = SelectedOption.Value;
                    _pm.SelectedProject.Save();
                }
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
        /// Moving boat command.
        /// </summary>
        public ReactiveCommand<object> MovingCommand { get; protected set; }

        /// <summary>
        /// Monitoring ADCP command.
        /// </summary>
        public ReactiveCommand<object> MonitoringCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public BottomTrackOnViewModel()
            : base("Bottom Track On")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            ConfigKey = "";

            // Create the list
            CreateList();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BinsView, ConfigKey)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            MovingCommand = ReactiveCommand.Create();
            MovingCommand.Subscribe(_ => MovingBoat());

            MovingCommand = ReactiveCommand.Create();
            MovingCommand.Subscribe(_ => MonitoringAdcp());

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

                // Initialize
                InitializeSelectedOption();
            }
        }

        /// <summary>
        /// A moving boat and bottom track is needed.
        /// </summary>
        private void MovingBoat()
        {
            if (_pm.IsProjectSelected)
            {
                _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON = true;
            }
        }

        /// <summary>
        /// A stationary ADCP so bottom track is not needed.
        /// </summary>
        private void MonitoringAdcp()
        {
            if (_pm.IsProjectSelected)
            {
                _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON = false;
            }
        }

        #region Initialize

        /// <summary>
        /// Create the list and populate it.
        /// </summary>
        private void CreateList()
        {
            OptionList = new ReactiveList<BottomTrackOption>();

            OptionList.Add(new BottomTrackOption("Moving", "Using Bottom Track", true, "../Images/moving_boat.png"));
            OptionList.Add(new BottomTrackOption("Monitoring", "Not Using Bottom Track", false, "../Images/monitoring.png"));
        }

        /// <summary>
        /// Chose the selected option based off the CBTON command in the selected project.
        /// </summary>
        private void InitializeSelectedOption()
        {
            // Get the current value from the project
            bool option = _pm.SelectedProject.Configuration.SubsystemConfigDict[ConfigKey].Commands.CBTON;

            foreach (BottomTrackOption btOption in OptionList)
            {
                if (btOption.Value == option)
                {
                    SelectedOption = btOption;
                }
            }
        }

        #endregion

    }
}
