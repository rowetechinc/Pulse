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
    public class FrequencyViewModel : PulseViewModel
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

        #region Frequency Strings

        /// <summary>
        /// 38 kHz Frequency.
        /// </summary>
        public const string FREQ_38 = "38";

        /// <summary>
        /// 75 kHz Frequency.
        /// </summary>
        public const string FREQ_75 = "75";

        /// <summary>
        /// 150 kHz Frequency.
        /// </summary>
        public const string FREQ_150 = "150";

        /// <summary>
        /// 300 kHz Frequency.
        /// </summary>
        public const string FREQ_300 = "300";

        /// <summary>
        /// 600 kHz Frequency.
        /// </summary>
        public const string FREQ_600 = "600";

        /// <summary>
        /// 1200 kHz Frequency.
        /// </summary>
        public const string FREQ_1200 = "1200";

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// List of subsystems.
        /// </summary>
        public ReactiveList<Subsystem> SubsystemList { get; set; }

        /// <summary>
        /// Selected Subsystem
        /// </summary>
        private Subsystem _SelectedSubsystem;
        /// <summary>
        /// Selected Subsystem
        /// </summary>
        public Subsystem SelectedSubsystem
        {
            get { return _SelectedSubsystem; }
            set
            {
                _SelectedSubsystem = value;
                this.NotifyOfPropertyChange(() => this.SelectedSubsystem);
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
        /// Frequency command.
        /// </summary>
        public ReactiveCommand<object> FreqCommand { get; protected set; }

        #endregion


        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public FrequencyViewModel()
            :base("Frequency ViewModel")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();


            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedSubsystem, x => x.Value != null));
            NextCommand.Subscribe(_ => OnNextCommand());

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            FreqCommand = ReactiveCommand.Create();
            FreqCommand.Subscribe(param => AddConfig(param));

            // Set the possible subsystems
            SetSubsystems();

        }


        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Initialize

        private void SetSubsystems()
        {
            SubsystemList = new ReactiveList<Subsystem>();

            if (_pm.IsProjectSelected)
            {
                foreach (Subsystem ss in _pm.SelectedProject.Configuration.SerialNumber.SubSystemsList)
                {
                    SubsystemList.Add(ss);
                }
            }
        }

        #endregion

        #region Next Command

        /// <summary>
        /// Add the configuration and then move to the next display.
        /// </summary>
        private void OnNextCommand()
        {
            if (_SelectedSubsystem != null)
            {
                AdcpSubsystemConfig ssConfig = null;
                _pm.SelectedProject.Configuration.AddConfiguration(_SelectedSubsystem, out ssConfig);
                _pm.SelectedProject.Save();

                _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.BroadbandModeView, ssConfig.ToString()));
            }
        }

        #endregion

        #region Add Config

        /// <summary>
        /// Add the configuration based off the parameter given.
        /// </summary>
        /// <param name="param">Frequency string.</param>
        private void AddConfig(object param)
        {
            string freq = param as string;
            if (freq != null)
            {
                switch(freq)
                {
                    case FREQ_38:
                        Add38Freq();
                        break;
                    case FREQ_75:
                        Add75Freq();
                        break;
                    case FREQ_150:
                        Add150Freq();
                        break;
                    case FREQ_300:
                        Add300Freq();
                        break;
                    case FREQ_600:
                        Add600Freq();
                        break;
                    case FREQ_1200:
                        Add1200Freq();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Add38Freq()
        {
            if (_pm.IsProjectSelected)
            {
                //_pm.SelectedProject.Configuration.AddConfiguration(new Subsystem());
            }
        }

        private void Add75Freq()
        {

        }

        private void Add150Freq()
        {

        }

        private void Add300Freq()
        {

        }

        private void Add600Freq()
        {

        }

        private void Add1200Freq()
        {

        }
        #endregion
    }
}
