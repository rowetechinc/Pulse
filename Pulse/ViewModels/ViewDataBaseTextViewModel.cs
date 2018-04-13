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
 * 07/15/2013      RC          3.0.4      Initial coding
 * 08/27/2013      RC          3.0.8      Filter ensembles on SubsystemDataConfig instead of SubsystemConfiguration to know where the data source is.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCollection to ReactiveList for Reactive 6.0.
 * 08/20/2014      RC          4.0.1      Changed the list of VM to match ViewDataGraphicalViewModel.
 * 08/21/2014      RC          4.0.1      Clear all the VM properly when changing projects.  On activate, make sure the VM will display.
 * 10/07/2015      RC          4.3.0     Changed dictionary to ConcurrentDicitionary.
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
    using System.Collections.Concurrent;

    /// <summary>
    /// Base view for all the Text data.
    /// </summary>
    public class ViewDataBaseTextViewModel : DisplayViewModel, IHandle<ProjectEvent>, IHandle<CloseVmEvent>
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

        #endregion

        #region Properties

        #region Configurations

        /// <summary>
        /// This dictonary is to used to quickly search if the VM
        /// has already been created based off the SubsystemDataConfig
        /// used.  This must be kept in-sync with the GraphicalVMList;
        /// </summary>
        private ConcurrentDictionary<SubsystemDataConfig, ViewDataTextViewModel> _textVMDict;

        /// <summary>
        /// The list of values from the VM dictionary.
        /// </summary>
        public List<ViewDataTextViewModel> TextVMList
        {
            get
            {
                return _textVMDict.Values.ToList();
            }
        }

        /// <summary>
        /// Selected Text ViewModel.
        /// </summary>
        private ViewDataTextViewModel _SelectedTextVM;
        /// <summary>
        /// Selected Text ViewModel.
        /// </summary>
        public ViewDataTextViewModel SelectedTextVM
        {
            get { return _SelectedTextVM; }
            set
            {
                _SelectedTextVM = value;
                this.NotifyOfPropertyChange(() => this.SelectedTextVM);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Create a base view for all the Graphical Views.
        /// </summary>
        public ViewDataBaseTextViewModel()
            : base("ViewDataBaseTextViewModel")
        {
            // Project Manager
            _pm = IoC.Get<PulseManager>();
            _pm.RegisterDisplayVM(this);
            _events = IoC.Get<IEventAggregator>();
            _events.Subscribe(this);

            // Initialize the dict
            _textVMDict = new ConcurrentDictionary<SubsystemDataConfig, ViewDataTextViewModel>();

            // Create the ViewModels based off the AdcpConfiguration
            AddConfigurations();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {
            // Shutdown all the VMs created
            foreach (var vm in TextVMList)
            {
                vm.Dispose();
            }
        }

        /// <summary>
        /// The list of configurations could have been cleared when
        /// loading a new project.  This will refresh the list if
        /// it needs to be.
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();

            // If no configurations are loaded, get them
            if (TextVMList.Count == 0)
            {
                AddConfigurations();
            }
        }

        #region Configuration

        /// <summary>
        /// Create all the ViewModels based off the Adcp Configuration in the selected project.
        /// </summary>
        private void AddConfigurations()
        {
            if (_pm.IsProjectSelected && _pm.SelectedProject.Configuration != null)
            {
                // Create a viewmodel for every configuration
                foreach (var config in _pm.SelectedProject.Configuration.SubsystemConfigDict.Values)
                {
                    AddConfig(new SubsystemDataConfig(config.SubsystemConfig, EnsembleSource.Playback));
                }
            }
        }

        /// <summary>
        /// Add a configuration.  This will create the ViewModel based
        /// off the configuration given.
        /// </summary>
        /// <param name="config">Configuration to use to create the ViewModel.</param>
        private void AddConfig(SubsystemDataConfig config)
        {
            if (!_textVMDict.ContainsKey(config))
            {
                if (_textVMDict.TryAdd(config, new ViewDataTextViewModel(config)))
                {
                    this.NotifyOfPropertyChange(() => this.TextVMList);

                    // Select a tab is nothing is selected
                    if (_SelectedTextVM == null)
                    {
                        if (TextVMList.Count > 0)
                        {
                            SelectedTextVM = TextVMList[0];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear all the configurations.
        /// This is done when a new project is selected.
        /// </summary>
        private void ClearConfig()
        {
            _SelectedTextVM = null;

            foreach (var vm in _textVMDict.Values)
            {
                vm.Dispose();
            }

            _textVMDict.Clear();
            this.NotifyOfPropertyChange(() => this.TextVMList);
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handle event when EnsembleEvent is received.
        /// This will create the displays for each config
        /// if it has not been created already.  It will also
        /// display the latest ensemble.
        /// </summary>
        /// <param name="ensEvent">Ensemble event.</param>
        public override void Handle(EnsembleEvent ensEvent)
        {
            if (ensEvent.Ensemble != null && ensEvent.Ensemble.IsEnsembleAvail)
            {
                // Create the config
                var ssDataConfig = new SubsystemDataConfig(ensEvent.Ensemble.EnsembleData.SubsystemConfig, ensEvent.Source);

                if (!_textVMDict.ContainsKey(ssDataConfig))
                {
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddConfig(ssDataConfig)));
                }
            }
        }

        /// <summary>
        /// Bulk Ensemble event.
        /// </summary>
        /// <param name="ensEvent"></param>
        public override void Handle(BulkEnsembleEvent ensEvent)
        {
            // DO NOTHING
        }

        /// <summary>
        /// New project is selected.
        /// </summary>
        /// <param name="prjEvent">Project event.</param>
        public void Handle(ProjectEvent prjEvent)
        {
            // Clear all the configs
            ClearConfig();

            // Add known configuration
            AddConfigurations();
        }

        /// <summary>
        /// Remove the display based off the SubsystemDataConfig
        /// given in the event.
        /// </summary>
        /// <param name="closeVmEvent">Contains the SubsystemDataConfig to remove the display.</param>
        public void Handle(CloseVmEvent closeVmEvent)
        {
            // Check if the display exist
            if (_textVMDict.ContainsKey(closeVmEvent.SubsysDataConfig))
            {
                // Dispose the display then remove the display
                _textVMDict[closeVmEvent.SubsysDataConfig].Dispose();
                ViewDataTextViewModel vm = null;
                if (_textVMDict.TryRemove(closeVmEvent.SubsysDataConfig, out vm))
                {
                    this.NotifyOfPropertyChange(() => this.TextVMList);

                    // Select a tab is nothing is selected
                    if (_SelectedTextVM == null)
                    {
                        if (TextVMList.Count > 0)
                        {
                            SelectedTextVM = TextVMList[0];
                        }
                    }
                }
            }
        }

        #endregion

    }
}
