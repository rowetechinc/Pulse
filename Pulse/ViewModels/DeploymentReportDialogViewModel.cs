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
 * 09/23/2013      RC          3.1.3      Initial coding
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;

    /// <summary>
    /// Display the Deployment configuration to the user before a deployment begins.
    /// </summary>
    public class DeploymentReportDialogViewModel : PulseDialogViewModel
    {
        #region Variables

        /// <summary>
        /// Pulse manager.
        /// </summary>
        private PulseManager _pm;

        #endregion

        #region Properties

        #region Configuration

        /// <summary>
        /// The current ADCP configuration.
        /// </summary>
        private AdcpConfiguration _Configuration;
        /// <summary>
        /// The current ADCP configuration.
        /// </summary>
        public AdcpConfiguration Configuration
        {
            get { return _Configuration; }
            set
            {
                _Configuration = value;
                this.NotifyOfPropertyChange(() => this.Configuration);
            }
        }

        #endregion

        #region Deployment Duration

        /// <summary>
        /// Set the string for the number of days for the deployment.
        /// </summary>
        public string DeploymentDuration
        {
            get
            {
                if (Configuration.DeploymentOptions.Duration == 1)
                {
                    return "1 day.";
                }
                else
                {
                    return string.Format("{0} days.", Configuration.DeploymentOptions.Duration);
                }
            }
        }

        #endregion

        #region Memory

        /// <summary>
        /// Total memory required.  The value is given in bytes.
        /// </summary>
        private long _MemoryCardRequired;
        /// <summary>
        /// Total memory required.  The value is given in bytes.
        /// </summary>
        public long MemoryCardRequired
        {
            get { return _MemoryCardRequired; }
            set
            {
                _MemoryCardRequired = value;
                this.NotifyOfPropertyChange(() => this.MemoryCardRequired);
            }
        }

        /// <summary>
        /// The memory card required scaled to a proper value.
        /// </summary>
        public string MemoryCardRequiredStr
        {
            get
            {
                return MathHelper.MemorySizeString(MemoryCardRequired);
            }
        }

        #endregion

        #region Batteries

        /// <summary>
        /// Total batteries required.  THis will be a double.  Must be rounded
        /// up for the total batteries.
        /// </summary>
        private double _BatteriesRequired;
        /// <summary>
        /// Total batteries required.  THis will be a double.  Must be rounded
        /// up for the total batteries.
        /// </summary>
        public double BatteriesRequired
        {
            get { return _BatteriesRequired; }
            set
            {
                _BatteriesRequired = value;
                this.NotifyOfPropertyChange(() => this.BatteriesRequired);
                this.NotifyOfPropertyChange(() => this.BatteriesRequiredStr);
            }
        }

        /// <summary>
        /// Convert the Batteries required to a string.
        /// This will round the value then set the string.
        /// </summary>
        public string BatteriesRequiredStr
        {
            get 
            {
                // Round the value
                int batt = (int)Math.Round(BatteriesRequired);

                // Determine the correct string
                if (batt <= 0)
                {
                    return "No Batteries";
                }
                else if (batt == 1)
                {
                    return "1 Battery";
                }
                else
                {
                    return string.Format("{0} batteries", batt);
                }
 
            }
        }

        #endregion

        #region CERECORD

        /// <summary>
        /// Set a string if there will be recording internally.
        /// </summary>
        public string CERECORDStr
        {
            get 
            {
                if (Configuration != null && (Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.Enable || Configuration.Commands.CERECORD_EnsemblePing == Commands.AdcpCommands.AdcpRecordOptions.BT_Eng))
                {
                    return "Data will be recorded internally.";
                }

                return "";
            }
        }

        #endregion

        #region ADCP Clock

        /// <summary>
        /// The current time of the ADCP.
        /// </summary>
        private string _AdcpClock;
        /// <summary>
        /// The current time of the ADCP.
        /// </summary>
        public string AdcpClock
        {
            get { return _AdcpClock; }
            set
            {
                _AdcpClock = value;
                this.NotifyOfPropertyChange(() => this.AdcpClock);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        public DeploymentReportDialogViewModel()
            : base("Deployment Report")
        {
            // Set Pulse Manager
            _pm = IoC.Get<PulseManager>();

            // Set Configuration
            Configuration = _pm.SelectedProject.Configuration;
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

    }
}
