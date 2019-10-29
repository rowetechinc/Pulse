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
 * 10/28/2019      RC          4.13.0     Initial coding
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
    using System.Threading.Tasks;

    /// <summary>
    /// Base view for all the Backscatter views.
    /// </summary>
    public class ViewDataBaseProfile3DViewModel : DisplayViewModel, IHandle<ProjectEvent>, IHandle<CloseVmEvent>
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
        /// Buffer the incoming data.
        /// </summary>
        private ConcurrentQueue<BulkEnsembleEvent> _buffer;

        /// <summary>
        /// Flag to know if processing the buffer.
        /// </summary>
        private bool _isProcessingBuffer;

        #endregion

        #region Properties

        #region Configurations

        /// <summary>
        /// This dictonary is to used to quickly search if the VM
        /// has already been created based off the SubsystemDataConfig
        /// used.  This must be kept in-sync with the GraphicalVMList;
        /// </summary>
        private ConcurrentDictionary<SubsystemDataConfig, ViewDataProfile3DViewModel> _profile3DVMDict;

        /// <summary>
        /// The list of values from the VM dictionary.
        /// </summary>
        public List<ViewDataProfile3DViewModel> Profile3DVMList
        {
            get
            {
                return _profile3DVMDict.Values.ToList();
            }
        }

        /// <summary>
        /// Selected Profile 3D ViewModel.
        /// </summary>
        private ViewDataProfile3DViewModel _SelecteProfile3dVM;
        /// <summary>
        /// Selected Profile 3D ViewModel.
        /// </summary>
        public ViewDataProfile3DViewModel SelecteProfile3DVM
        {
            get { return _SelecteProfile3dVM; }
            set
            {
                _SelecteProfile3dVM = value;
                this.NotifyOfPropertyChange(() => this.SelecteProfile3DVM);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Create a base view for all the Graphical Views.
        /// </summary>
        public ViewDataBaseProfile3DViewModel()
            : base("ViewDataBaseProfile3DViewModel")
        {
            // Project Manager
            _pm = IoC.Get<PulseManager>();
            _pm.RegisterDisplayVM(this);
            _events = IoC.Get<IEventAggregator>();
            _events.Subscribe(this);
            _isProcessingBuffer = false;
            _buffer = new ConcurrentQueue<BulkEnsembleEvent>();

            // Initialize the dict
            _profile3DVMDict = new ConcurrentDictionary<SubsystemDataConfig, ViewDataProfile3DViewModel>();

            // Create the ViewModels based off the AdcpConfiguration
            AddConfigurations();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {
            // Shutdown all the VMs created
            foreach (var vm in Profile3DVMList)
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
            if(Profile3DVMList.Count == 0)
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
            if (!_profile3DVMDict.ContainsKey(config))
            {
                if (_profile3DVMDict.TryAdd(config, new ViewDataProfile3DViewModel(config)))
                {
                    this.NotifyOfPropertyChange(() => this.Profile3DVMList);

                    // Select a tab is nothing is selected
                    if (_SelecteProfile3dVM == null)
                    {
                        if (Profile3DVMList.Count > 0)
                        {
                            SelecteProfile3DVM = Profile3DVMList[0];
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
            _SelecteProfile3dVM = null;

            foreach(var vm in _profile3DVMDict.Values )
            {
                vm.Dispose();
            }

            _profile3DVMDict.Clear();
            this.NotifyOfPropertyChange(() => this.Profile3DVMList);
        }

        #endregion

        #region Bulk Ensemble Display

        /// <summary>
        /// Display the bulk ensemble event async.
        /// </summary>
        private void BulkEnsembleDisplayExecute()
        {
            while (_buffer.Count > 0)
            {
                _isProcessingBuffer = true;


                BulkEnsembleEvent ensEvent = null;
                if (_buffer.TryDequeue(out ensEvent))
                {
                    // Set the maximum display for each VM
                    //foreach (var vm in _backscatterVMDict.Values)
                    //{
                    //    vm.ClearPlots();
                    //    vm.MaxEnsembles = ensEvent.Ensembles.Count();
                    //}

                    // Look for all the configurations
                    // 12 is maximum configurations
                    if (ensEvent.Ensembles.Count() > 0)
                    {
                        for (int x = 0; x < ensEvent.Ensembles.Count(); x++)
                        {
                            // Check 12 ensembles, because there can be up to 12 different configurations
                            if(x > 12)
                            {
                                break;
                            }

                            var ensemble = ensEvent.Ensembles.IndexValue(x);

                            if (ensemble != null && ensemble.IsEnsembleAvail)
                            {
                                // Create the config
                                var ssDataConfig = new SubsystemDataConfig(ensemble.EnsembleData.SubsystemConfig, ensEvent.Source);

                                // Check if the config exist in the table
                                if (!_profile3DVMDict.ContainsKey(ssDataConfig))
                                {
                                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddConfig(ssDataConfig)));
                                }

                                //Wait for the dispatcher to add the config
                                // Monitor for any timeouts
                                int timeout = 0;
                                while (!_profile3DVMDict.ContainsKey(ssDataConfig))
                                {
                                    // Set a timeout and wait for the config
                                    timeout++;
                                    if (timeout > 10)
                                    {
                                        break;
                                    }
                                    System.Threading.Thread.Sleep(250);
                                }
                            }
                        }
                    }

                    // Get the number of ensembles and use it to set the max display
                    int maxEnsembles = 0;
                    if (_pm.IsProjectSelected)
                    {
                        maxEnsembles = _pm.SelectedProject.GetNumberOfEnsembles();
                    }

                    // Pass the ensembles to the displays
                    foreach (var vm in _profile3DVMDict.Values)
                    {
                        vm.DisplayBulkData(ensEvent.Ensembles, maxEnsembles);
                    }
                }
            }
            _isProcessingBuffer = false;

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

                // Check if the config exist in the table
                if (!_profile3DVMDict.ContainsKey(ssDataConfig))
                {
                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddConfig(ssDataConfig)));
                }
            }
        }

        /// <summary>
        /// Handle event when BulkEnsembleEvent is received.
        /// This will create the displays for each config
        /// if it has not been created already.  It will also
        /// display the latest ensemble.
        /// </summary>
        /// <param name="ensEvent">Ensemble event.</param>
        public override void Handle(BulkEnsembleEvent ensEvent)
        {
            if (ensEvent.Ensembles != null)
            {
                _buffer.Enqueue(ensEvent);

                // Execute async
                if (!_isProcessingBuffer)
                {
                    // Execute async
                    Task.Run(() => BulkEnsembleDisplayExecute());
                }
            }
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
            if(_profile3DVMDict.ContainsKey(closeVmEvent.SubsysDataConfig))
            {
                // Dispose the display then remove the display
                _profile3DVMDict[closeVmEvent.SubsysDataConfig].Dispose();
                ViewDataProfile3DViewModel vm = null;
                if (_profile3DVMDict.TryRemove(closeVmEvent.SubsysDataConfig, out vm))
                {
                    this.NotifyOfPropertyChange(() => this.Profile3DVMList);

                    // Select a tab is nothing is selected
                    if (_SelecteProfile3dVM == null)
                    {
                        if (Profile3DVMList.Count > 0)
                        {
                            SelecteProfile3DVM = Profile3DVMList[0];
                        }
                    }
                }
            }
        }

        #endregion

    }
}
