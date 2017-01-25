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
 * 09/26/2014      RC          4.1.0       Initial coding
 * 
 */

using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// DVL subsystem configuration.
    /// </summary>
    public class DvlSubsystemConfigurationViewModel : PulseViewModel
    {
        #region Variables

        /// <summary>
        /// DVL Setup VM to remove itself.
        /// </summary>
        private DvlSetupViewModel _dvlSetupVM;

        /// <summary>
        /// Subsystem configuration to set the commands.
        /// </summary>
        public AdcpSubsystemConfig AdcpSubConfig { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// String of the description of subsystem configuration.
        /// </summary>
        private string _Desc;
        /// <summary>
        /// String of the description of subsystem configuration.
        /// </summary>
        public string Desc
        {
            get { return _Desc; }
            set
            {
                _Desc = value;
                this.NotifyOfPropertyChange(() => this.Desc);
            }
        }

        #region Bottom Track

        /// <summary>
        /// CEOUTPUT list.
        /// </summary>
        public List<string> BtBeamMultiplexList { get; private set; }

        /// <summary>
        /// CTBMX description.
        /// </summary>
        public string CBTMX_Desc
        {
            get
            {
                return Commands.AdcpSubsystemCommands.GetCbtmxDesc();
            }
        }

        /// <summary>
        /// Bottom Track on off.
        /// </summary>
        public bool CBTON
        {
            get { return AdcpSubConfig.Commands.CBTON; }
            set
            {
                AdcpSubConfig.Commands.CBTON = value;
                this.NotifyOfPropertyChange(() => this.CBTON);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        /// <summary>
        /// Bottom Track maximum range.
        /// </summary>
        public float CBTMX
        {
            get { return AdcpSubConfig.Commands.CBTMX; }
            set
            {
                AdcpSubConfig.Commands.CBTMX = value;
                this.NotifyOfPropertyChange(() => this.CBTMX);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        /// <summary>
        /// CBTBB description.
        /// </summary>
        public string CBTBB_Desc
        {
            get
            {
                return Commands.AdcpSubsystemCommands.GetCbtbbDesc();
            }
        }

        /// <summary>
        /// Bottom Track long range depth.
        /// </summary>
        public float CBTBB_LongRangeDepth
        {
            get { return AdcpSubConfig.Commands.CBTBB_LongRangeDepth; }
            set
            {
                AdcpSubConfig.Commands.CBTBB_LongRangeDepth = value;
                this.NotifyOfPropertyChange(() => this.CBTBB_LongRangeDepth);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        /// <summary>
        /// Bottom Track beam multiplex.
        /// </summary>
        private string _CBTBB_BeamMultiplex;
        /// <summary>
        /// Bottom Track beam multiplex.
        /// </summary>
        public string CBTBB_BeamMultiplex
        {
            get { return _CBTBB_BeamMultiplex; }
            set
            {
                _CBTBB_BeamMultiplex = value;
                AdcpSubConfig.Commands.CBTBB_BeamMultiplex = (Commands.AdcpSubsystemCommands.BeamMultiplexOptions)System.Enum.Parse(typeof(Commands.AdcpSubsystemCommands.BeamMultiplexOptions), _CBTBB_BeamMultiplex);
                this.NotifyOfPropertyChange(() => this.CBTBB_BeamMultiplex);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        #endregion

        #region Water Track

        /// <summary>
        /// CWTBS description.
        /// </summary>
        public string CWTBS_Desc
        {
            get
            {
                return Commands.AdcpSubsystemCommands.GetCwtbsDesc();
            }
        }

        /// <summary>
        /// Water Track on off.
        /// </summary>
        public bool CWTON
        {
            get { return AdcpSubConfig.Commands.CWTON; }
            set
            {
                AdcpSubConfig.Commands.CWTON = value;
                this.NotifyOfPropertyChange(() => this.CWTON);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        /// <summary>
        /// Water Track Bin size.
        /// </summary>
        public float CWTBS
        {
            get { return AdcpSubConfig.Commands.CWTBS; }
            set
            {
                AdcpSubConfig.Commands.CWTBS = value;
                this.NotifyOfPropertyChange(() => this.CWTBS);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        /// <summary>
        /// CWTBL description.
        /// </summary>
        public string CWTBL_Desc
        {
            get
            {
                return Commands.AdcpSubsystemCommands.GetCwtblDesc();
            }
        }

        /// <summary>
        /// Water Track Blank.
        /// </summary>
        public float CWTBL
        {
            get { return AdcpSubConfig.Commands.CWTBL; }
            set
            {
                AdcpSubConfig.Commands.CWTBL = value;
                this.NotifyOfPropertyChange(() => this.CWTBL);

                // Update the display
                _dvlSetupVM.UpdateCommandSet();
            }
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to remove a subsystem.
        /// </summary>
        public ReactiveCommand<object> RemoveSubsystemCommand { get; protected set; }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <param name="config">Subsystem configuration.</param>
        /// <param name="dvlSetupVM">DVL Setup VM.</param>
        public DvlSubsystemConfigurationViewModel(ref AdcpSubsystemConfig config, DvlSetupViewModel dvlSetupVM)
            : base("DVL Subsystem Configuration")
        {
            // Initialize values
            _dvlSetupVM = dvlSetupVM;
            AdcpSubConfig = config;
            if (config != null)
                {
                    Desc = config.ToString();
                }

            Init();

            // Add Subsystem
            RemoveSubsystemCommand = ReactiveCommand.Create();
            RemoveSubsystemCommand.Subscribe(_ => RemoveSubsystem());
        }

        /// <summary>
        /// Dispose of the VM.
        /// </summary>
        public override void Dispose()
        {
            
        }

        /// <summary>
        /// Initialize the values.
        /// </summary>
        private void Init()
        {
            BtBeamMultiplexList = new List<string>();
            Array multiplexOptions = System.Enum.GetValues(typeof(Commands.AdcpSubsystemCommands.BeamMultiplexOptions));
            foreach (var output in multiplexOptions)
            {
                BtBeamMultiplexList.Add(Enum.GetName(typeof(Commands.AdcpSubsystemCommands.BeamMultiplexOptions), output));
            }
            _CBTBB_BeamMultiplex = Enum.GetName(typeof(Commands.AdcpSubsystemCommands.BeamMultiplexOptions), AdcpSubConfig.Commands.CBTBB_BeamMultiplex);
        }

        /// <summary>
        /// Remove the subsystem.
        /// </summary>
        private void RemoveSubsystem()
        {
            _dvlSetupVM.RemoveVM(this);
        }
    }
}
