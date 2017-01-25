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
 * 06/17/2013      RC          3.0.1      Initial coding
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This object is used to describe all the available ADCP connections.
    /// </summary>
    public class AdcpConnectionOption : PulseViewModel
    {
        #region Properties

        /// <summary>
        /// Flag if this connection is connected.
        /// </summary>
        private bool _IsConnected;
        /// <summary>
        /// Flag if this connection is connected.
        /// </summary>
        public bool IsConnected
        {
            get { return _IsConnected; }
            set
            {
                _IsConnected = value;
                this.NotifyOfPropertyChange(() => this.IsConnected);
            }
        }

                /// <summary>
        /// Serial port options for this connection.
        /// </summary>
        private AdcpSerialPort.AdcpSerialOptions _Options;
        /// <summary>
        /// Serial port options for this connection.
        /// </summary>
        public AdcpSerialPort.AdcpSerialOptions Options
        {
            get { return _Options; }
            set
            {
                _Options = value;
                this.NotifyOfPropertyChange(() => this.Options);
            }
        }

        /// <summary>
        /// New project name for this connection.
        /// </summary>
        private string _NewProjectName;
        /// <summary>
        /// New project name for this connection.
        /// </summary>
        public string NewProjectName 
        {
            get { return _NewProjectName; } 
            set
            {
                _NewProjectName = value;
                this.NotifyOfPropertyChange(() => this.NewProjectName);
            }
        }

        #endregion

        /// <summary>
        /// Initialize the object.
        /// </summary>
        /// <param name="options">Serial port connection options.</param>
        /// <param name="prjName">New Project name.</param>
        /// <param name="isConnected">Flag if this connection is connected.</param>
        public AdcpConnectionOption(AdcpSerialPort.AdcpSerialOptions options, string prjName, bool isConnected = false)
            : base("AdcpConnectionOption")
        {
            Options = options;
            NewProjectName = prjName;
            IsConnected = isConnected;
        }

        /// <summary>
        /// Shutdown the object.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing
        }

        #region Override

        /// <summary>
        /// Display the object to a string.
        /// </summary>
        /// <returns>String of the object.</returns>
        public override string ToString()
        {
            return Options.ToString();
        }

        /// <summary>
        /// Hashcode for the object.
        /// This will return the hashcode for the
        /// this object's string.
        /// </summary>
        /// <returns>Hashcode for the object.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Determine if the given object is equal to this
        /// object.  This will check if the Status Value match.
        /// </summary>
        /// <param name="obj">Object to compare with this object.</param>
        /// <returns>TRUE = Status Value matched.</returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AdcpConnectionOption p = (AdcpConnectionOption)obj;

            return Options == p.Options;
        }

        /// <summary>
        /// Determine if the two AdcpConnectionListItemViewModel Value given are the equal.
        /// </summary>
        /// <param name="option1">First AdcpConnectionOption to check.</param>
        /// <param name="option2">AdcpConnectionOption to check against.</param>
        /// <returns>True if there options match.</returns>
        public static bool operator ==(AdcpConnectionOption option1, AdcpConnectionOption option2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(option1, option2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)option1 == null) || ((object)option2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return option1.Options == option2.Options;
        }

        /// <summary>
        /// Return the opposite of ==.
        /// </summary>
        /// <param name="option1">First AdcpConnectionOption to check.</param>
        /// <param name="option2">AdcpConnectionOption to check against.</param>
        /// <returns>Return the opposite of ==.</returns>
        public static bool operator !=(AdcpConnectionOption option1, AdcpConnectionOption option2)
        {
            return !(option1 == option2);
        }

        #endregion

    }
}
