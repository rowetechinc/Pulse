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

    /// <summary>
    /// Allow the user to do a simple compass calibration.
    /// </summary>
    public class SimpleCompassCalWizardViewModel : PulseViewModel, IDeactivate
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
        /// Compass Calibration View Model.
        /// </summary>
        public SimpleCompassCalViewModel CompassCalVM { get; protected set; }

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
        public SimpleCompassCalWizardViewModel()
            : base("Simple Compass Cal Wizard")
        {
            // Initialize values
            _events = IoC.Get<IEventAggregator>();
            _pm = IoC.Get<PulseManager>();

            // Next command
            NextCommand = ReactiveCommand.Create();
            NextCommand.Subscribe(_ => NextPage());

            // Back coommand
            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.Back)));

            // Exit coommand
            ExitCommand = ReactiveCommand.Create();
            ExitCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.HomeView)));

            InitializeValue();
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {
            CompassCalVM.Dispose();
        }

        #region Initialize

        /// <summary>
        /// Initialize the value.
        /// </summary>
        private void InitializeValue()
        {
            // Get the compass cal view model.
            CompassCalVM = IoC.Get<SimpleCompassCalViewModel>();
        }

        #endregion

        #region Next Page

        /// <summary>
        /// If there is a presure sensor, go to
        /// the zero pressure sensor page, if not
        /// go to Deploy page.
        /// </summary>
        private void NextPage()
        {
            // If there is a pressure sensor
            // Go to zero pressure sensor page
            // If not go to Deploy page
            if (_pm.SelectedProject.Configuration.HardwareOptions.IsPressureSensor)
            {

                _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ZeroPressureSensorView));
            }
            else
            {
                _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DeployAdcpView));
            }
        }


        #endregion


        #region Deactivate

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        /// <param name="close"></param>
        void IDeactivate.Deactivate(bool close)
        {
            Dispose();
        }

        #endregion
    }
}
