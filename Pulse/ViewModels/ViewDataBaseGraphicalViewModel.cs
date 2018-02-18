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
 * 09/12/2013      RC          3.1.0      Added SelectedGraphicalVM to know which view to display.
 * 12/06/2013      RC          3.2.0      Handle BulkEnsembleEvent.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 08/20/2014      RC          4.0.1      Added CloseVmEvent event.  Changed the list of VM to match ViewDataGraphicalViewModel.
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
    using System.Threading.Tasks;

    /// <summary>
    /// Base view for all the Graphical views.
    /// </summary>
    public class ViewDataBaseGraphicalViewModel : PulseViewModel, IHandle<EnsembleEvent>, IHandle<BulkEnsembleEvent>, IHandle<ProjectEvent>, IHandle<CloseVmEvent>
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
        private ConcurrentDictionary<SubsystemDataConfig, ViewDataGraphicalViewModel> _graphicalVMDict;

        /// <summary>
        /// The list of values from the VM dictionary.
        /// </summary>
        public List<ViewDataGraphicalViewModel> GraphicalVMList
        {
            get
            {
                return _graphicalVMDict.Values.ToList();
            }
        }

        /// <summary>
        /// Selected Graphical ViewModel.
        /// </summary>
        private ViewDataGraphicalViewModel _SelectedGraphicalVM;
        /// <summary>
        /// Selected Graphical ViewModel.
        /// </summary>
        public ViewDataGraphicalViewModel SelectedGraphicalVM
        {
            get { return _SelectedGraphicalVM; }
            set
            {
                _SelectedGraphicalVM = value;
                this.NotifyOfPropertyChange(() => this.SelectedGraphicalVM);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Create a base view for all the Graphical Views.
        /// </summary>
        public ViewDataBaseGraphicalViewModel()
            : base("ViewDataBaseGraphicalViewModel")
        {
            // Project Manager
            _pm = IoC.Get<PulseManager>();
            _events = IoC.Get<IEventAggregator>();
            _events.Subscribe(this);
            _isProcessingBuffer = false;
            _buffer = new ConcurrentQueue<BulkEnsembleEvent>();

            // Initialize the dict
            _graphicalVMDict = new ConcurrentDictionary<SubsystemDataConfig, ViewDataGraphicalViewModel>();

            // Create the ViewModels based off the AdcpConfiguration
            AddConfigurations();
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {
            // Shutdown all the VMs created
            foreach (var vm in GraphicalVMList)
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
            if(GraphicalVMList.Count == 0)
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
            if (!_graphicalVMDict.ContainsKey(config))
            {
                if (_graphicalVMDict.TryAdd(config, new ViewDataGraphicalViewModel(config)))
                {
                    this.NotifyOfPropertyChange(() => this.GraphicalVMList);

                    // Select a tab is nothing is selected
                    if (_SelectedGraphicalVM == null)
                    {
                        if (GraphicalVMList.Count > 0)
                        {
                            SelectedGraphicalVM = GraphicalVMList[0];
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
            _SelectedGraphicalVM = null;

            foreach(var vm in _graphicalVMDict.Values )
            {
                vm.Dispose();
            }

            _graphicalVMDict.Clear();
            this.NotifyOfPropertyChange(() => this.GraphicalVMList);
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
                    foreach (var vm in GraphicalVMList)
                    {
                        vm.ClearPlots();
                        vm.DisplayMaxEnsembles = ensEvent.Ensembles.Count();
                    }

                    // Look for all the configurations
                    // 12 is maximum configurations
                    if (ensEvent.Ensembles.Count() > 0)
                    {
                        for (int x = 0; x < ensEvent.Ensembles.Count(); x++)
                        {
                            // Check 12 ensembles, because there can be up to 12 different configurations
                            if (x > 12)
                            {
                                break;
                            }

                            var ensemble = ensEvent.Ensembles.IndexValue(x);

                            // Verify the ensemble
                            if (ensemble != null && ensemble.IsEnsembleAvail)
                            {
                                // Create the config
                                var ssDataConfig = new SubsystemDataConfig(ensemble.EnsembleData.SubsystemConfig, ensEvent.Source);

                                // Check if the config exist in the table
                                if (!_graphicalVMDict.ContainsKey(ssDataConfig))
                                {
                                    Application.Current.Dispatcher.BeginInvoke(new System.Action(() => AddConfig(ssDataConfig)));
                                }

                                //Wait for the dispatcher to add the config
                                // Monitor for any timeouts
                                int timeout = 0;
                                while (!_graphicalVMDict.ContainsKey(ssDataConfig))
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

                        // Pass the ensembles to the displays
                        foreach (var vm in _graphicalVMDict.Values)
                        {
                            vm.DisplayBulkData(ensEvent.Ensembles);
                        }
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
        public void Handle(EnsembleEvent ensEvent)
        {
            if (ensEvent.Ensemble != null && ensEvent.Ensemble.IsEnsembleAvail)
            {
                // Create the config
                var ssDataConfig = new SubsystemDataConfig(ensEvent.Ensemble.EnsembleData.SubsystemConfig, ensEvent.Source);

                // Check if the config exist in the table
                if (!_graphicalVMDict.ContainsKey(ssDataConfig))
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
        public void Handle(BulkEnsembleEvent ensEvent)
        {
            _buffer.Enqueue(ensEvent);

            // Execute async
            if (!_isProcessingBuffer)
            {
                // Execute async
                Task.Run(() => BulkEnsembleDisplayExecute());
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
            if(_graphicalVMDict.ContainsKey(closeVmEvent.SubsysDataConfig))
            {
                // Dispose the display then remove the display
                _graphicalVMDict[closeVmEvent.SubsysDataConfig].Dispose();
                ViewDataGraphicalViewModel vm = null;
                if (_graphicalVMDict.TryRemove(closeVmEvent.SubsysDataConfig, out vm))
                {
                    this.NotifyOfPropertyChange(() => this.GraphicalVMList);

                    // Select a tab is nothing is selected
                    if (_SelectedGraphicalVM == null)
                    {
                        if (GraphicalVMList.Count > 0)
                        {
                            SelectedGraphicalVM = GraphicalVMList[0];
                        }
                    }
                }
            }
        }

        #endregion

    }
}
