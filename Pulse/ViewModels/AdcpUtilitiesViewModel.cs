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
 * 11/20/2013      RC          3.2.0      Initial coding
 * 12/09/2013      RC          3.2.0      Added ScreenCommand.
 * 12/20/2013      RC          3.2.1      Added PredicitionModelCommand.
 * 02/18/2014      RC          3.2.3      Added VmOptionsCommand.
 * 08/07/2014      RC          4.0.0      Updated ReactiveCommand to 6.0.
 * 10/02/2014      RC          4.1.0      Added RtiCompassCalCommand.
 * 05/06/2015      RC          4.1.4      Only load the advanced compass cal view.
 * 08/28/2017      RC          4.5.2      Added DataOutputView.
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
    /// Page to access all the ADCP Utility pages.
    /// </summary>
    public class AdcpUtilitiesViewModel : PulseViewModel
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

        #endregion

        #region Commands

        /// <summary>
        /// Command to go to the Compass Cal page.
        /// </summary>
        public ReactiveCommand<object> CompassCalCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Compass Utility page.
        /// </summary>
        public ReactiveCommand<object> CompassUtilityCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Terminal Page.
        /// </summary>
        public ReactiveCommand<object> TerminalCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Terminal Page.
        /// </summary>
        public ReactiveCommand<object> DownloadCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Terminal Page.
        /// </summary>
        public ReactiveCommand<object> UploadCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Terminal Page.
        /// </summary>
        public ReactiveCommand<object> ScreenCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Vessel Mount Options page.
        /// </summary>
        public ReactiveCommand<object> VmOptionsCommand { get; protected set; }

        /// <summary>
        /// Command to go to the ADCP Predicition Model page.
        /// </summary>
        public ReactiveCommand<object> PredicitionModelCommand { get; protected set; }

        /// <summary>
        /// Command to go to the RTI Compass Calibration page.
        /// </summary>
        public ReactiveCommand<object> RtiCompassCalCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Diagnostics page.
        /// </summary>
        public ReactiveCommand<object> DiagnosticsCommand { get; protected set; }

        /// <summary>
        /// Command to go to the Data Output page.
        /// </summary>
        public ReactiveCommand<object> DataOutputCommand { get; protected set; }        

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public AdcpUtilitiesViewModel()
            : base("ADCP Utilities")
        {
            // Set Event Aggregator
            _events = IoC.Get<IEventAggregator>();

            // Compass Cal command
            CompassCalCommand = ReactiveCommand.Create();
            CompassCalCommand.Subscribe(_ => CompassCal());

            // Compass Utility command
            CompassUtilityCommand = ReactiveCommand.Create();
            CompassUtilityCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.CompassUtilityView)));

            // Terminal command
            TerminalCommand = ReactiveCommand.Create();
            TerminalCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.TerminalView)));

            // Download command
            DownloadCommand = ReactiveCommand.Create();
            DownloadCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DownloadDataView)));

            // Upload Firmware command
            UploadCommand = ReactiveCommand.Create();
            UploadCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.UpdateFirmwareView)));

            // Screen data command
            ScreenCommand = ReactiveCommand.Create();
            ScreenCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.ScreenDataView)));

            VmOptionsCommand = ReactiveCommand.Create();
            VmOptionsCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.VesselMountOptionsView)));

            // Predicition Model command
            PredicitionModelCommand = ReactiveCommand.Create();
            PredicitionModelCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.AdcpPredictionModelView)));

            // RTI Compass Calibration Model command
            RtiCompassCalCommand = ReactiveCommand.Create();
            RtiCompassCalCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.RtiCompassCalView)));

            // Diagnostics View command
            DiagnosticsCommand = ReactiveCommand.Create();
            DiagnosticsCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DiagnosticView)));

            // Data Output View command
            DataOutputCommand = ReactiveCommand.Create();
            DataOutputCommand.Subscribe(_ => _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.DataOutputView)));
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }


        #region Compass Cal

        /// <summary>
        /// If it is an Admin user, go to the advanced compass cal page.
        /// </summary>
        private void CompassCal()
        {
            //if(Pulse.Commons.IsAdmin())
            //{
                // Advanced compass cal page
                _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.CompassCalView));
            //}
            //else
            //{
            //    _events.PublishOnUIThread(new ViewNavEvent(ViewNavEvent.ViewId.SimpleCompassCalView, true));
            //}
        }
        #endregion
    }
}
