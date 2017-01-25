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
* 11/15/2013      RC          3.2.0      Initial coding
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
    /// Allow the user to zero the pressure sensor.
    /// </summary>
    public class ZeroPressureSensorViewModel : PulseViewModel
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
        /// Command to zero the pressure sensor.
        /// </summary>
        public ReactiveCommand<object> ZeroPressureSensorCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public ZeroPressureSensorViewModel()
            : base("Zero Pressure Sensor")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();
            _adcpConnection = IoC.Get<AdcpConnection>();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DeployAdcpView)));

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            // Zero Pressure sensor command
            ZeroPressureSensorCommand = ReactiveCommand.Create();
            ZeroPressureSensorCommand.Subscribe(_ => ZeroPressureSensor());

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

        }

        #endregion

        #region Zero Pressure Sensor

        /// <summary>
        /// Send the command to zero the pressure sensor
        /// </summary>
        private void ZeroPressureSensor()
        {
            if (_adcpConnection.IsAdcpSerialConnected())
            {
                // Send command to zero the pressure sensor
                _adcpConnection.AdcpSerialPort.SendData(RTI.Commands.AdcpCommands.CMD_CPZ);
            }
        }

        #endregion

    }
}
