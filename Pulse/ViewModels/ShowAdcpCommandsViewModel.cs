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

    /// <summary>
    /// Display all the commands of the ADCP.
    /// </summary>
    public class ShowAdcpCommandsViewModel : PulseDialogViewModel
    {
        #region Variables

        #endregion

        #region Properties

        /// <summary>
        /// All the ADCP commands.
        /// </summary>
        private string _AdcpCommands;
        /// <summary>
        /// All the ADCP commands.
        /// </summary>
        public string AdcpCommands
        {
            get { return _AdcpCommands; }
            set
            {
                _AdcpCommands = value;
                this.NotifyOfPropertyChange(() => this.AdcpCommands);
            }
        }

        #endregion

        /// <summary>
        /// Initialize the view model.
        /// </summary>
        /// <param name="commands">List of all the commands to display.</param>
        public ShowAdcpCommandsViewModel(List<string> commands)
            : base ("ADCP Commands")
        {
            // Set the commands
            SetCommands(commands);
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        #region Set commands

        /// <summary>
        /// Get all the commands from the list and create a list of
        /// commands.
        /// </summary>
        /// <param name="cmds">List of commands.</param>
        public void SetCommands(List<string> cmds)
        {
            StringBuilder sb = new StringBuilder();
            foreach(string cmd in cmds)
            {
                sb.AppendLine(cmd);
            }

            // Set all the commands
            AdcpCommands = sb.ToString();
        }

        #endregion

    }
}
