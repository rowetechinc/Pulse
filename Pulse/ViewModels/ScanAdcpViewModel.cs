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
* 11/18/2013      RC          3.2.0      Initial coding
* 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
* 09/13/2017      RC          4.5.4      Added SerialNumberGenerator to allow either scan or select. 
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
    using ReactiveUI;
    using Caliburn.Micro;
    using System.Windows;
    using System.Threading.Tasks;

    /// <summary>
    /// Scan the ADCP for its configuration.
    /// </summary>
    public class ScanAdcpViewModel : PulseViewModel
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

        /// <summary>
        /// Connection to the ADCP.
        /// </summary>
        private AdcpConnection _adcpConnection;

        #endregion

        #region Properties

        /// <summary>
        /// Serial number generator.
        /// </summary>
        public SerialNumberGeneratorViewModel SerialNumberGeneratorVM { get; protected set; }

        /// <summary>
        /// ADCP Serial Number.
        /// </summary>
        private string _AdcpSerialNumber;
        /// <summary>
        /// ADCP Serial Number.
        /// </summary>
        public string AdcpSerialNumber
        {
            get { return _AdcpSerialNumber; }
            set
            {
                _AdcpSerialNumber = value;
                this.NotifyOfPropertyChange(() => this.AdcpSerialNumber);
            }
        }

        /// <summary>
        /// Flag to know if the scan is still loading.
        /// </summary>
        private bool _IsScanning;
        /// <summary>
        /// Flag to know if the scan is still loading.
        /// </summary>
        public bool IsScanning
        {
            get { return _IsScanning; }
            set
            {
                _IsScanning = value;
                this.NotifyOfPropertyChange(() => this.IsScanning);
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
        /// Command to scan the ADCP.
        /// </summary>
        public ReactiveCommand<System.Reactive.Unit> ScanAdcpCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public ScanAdcpViewModel()
            : base("Scan ADCP")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Serial Number Generator view model
            SerialNumberGeneratorVM = IoC.Get<SerialNumberGeneratorViewModel>();
            SerialNumberGeneratorVM.UpdateEvent += SerialNumberGeneratorVM_UpdateEvent;

            // Next command
            NextCommand = ReactiveCommand.Create(this.WhenAny(x => x.IsScanning, x => !x.Value));
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ModeView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Scan ADCP command
            ScanAdcpCommand = ReactiveCommand.CreateAsyncTask(_ => ScanAdcp());

            InitializeValue();
        }

        /// <summary>
        /// Update the serial number of the serial number generater updates the number.
        /// </summary>
        private void SerialNumberGeneratorVM_UpdateEvent()
        {
            AdcpSerialNumber = SerialNumberGeneratorVM.SerialNumber.ToString();
            _pm.SelectedProject.SerialNumber = SerialNumberGeneratorVM.SerialNumber;
            AdcpSerialNumber += "\n";
            AdcpSerialNumber += _pm.SelectedProject.SerialNumber.GetSerialNumberDescString(true);

            // Add a default configuration for the new subsystem added
            if (SerialNumberGeneratorVM.SerialNumber.SubSystemsList.Count > 0)
            {
                AdcpSubsystemConfig config = null;
                _pm.SelectedProject.Configuration.AddConfiguration(SerialNumberGeneratorVM.SerialNumber.SubSystemsList.Last(), out config);
            }
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
            IsScanning = false;
            if(_pm.IsProjectSelected)
            {
                SerialNumberGeneratorVM.SerialNumber = _pm.SelectedProject.SerialNumber;
            }
        }

        #endregion

        #region Scan ADCP

        /// <summary>
        /// Start the scanning.
        /// </summary>
        private async Task ScanAdcp()
        {
            // Set the flag that we are scanning
            IsScanning = true;

            // Execute scan
            await Task.Run(() => ExecuteScanAdcp());
        }

        /// <summary>
        /// Scan the ADCP for its configuration.
        /// </summary>
        private void ExecuteScanAdcp()
        {
            if (_pm.IsProjectSelected)
            {
                // Get the ADCP configuration
                _pm.SelectedProject = _adcpConnection.SetAdcpConfiguration(_pm.SelectedProject);

                // Set the serial number
                AdcpSerialNumber = _pm.SelectedProject.SerialNumber.ToString();
                AdcpSerialNumber += "\n";
                AdcpSerialNumber += _pm.SelectedProject.SerialNumber.GetSerialNumberDescString(true);

                // Set the serial number to the generator
                SerialNumberGeneratorVM.UpdateSerialNumber(_pm.SelectedProject.SerialNumber);

                // Turn off the flag
                IsScanning = false;
            }
        }

        #endregion


    }
}
