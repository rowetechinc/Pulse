/*
 * Copyright © 2011 
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
 * 10/27/2015      RC          4.3.1      Initial coding
 * 
 */
using System;
using System.IO;
using System.Windows.Media;

namespace RTI
{
    namespace Pulse
    {
        /// <summary>
        /// Common values used in the application.
        /// This inclues the version number and company name.
        /// </summary>
        public class Version
        {
            #region Version Number

            /// <summary>
            /// Pulse version number.
            /// Version number is set in AssembleInfo.cs.
            /// </summary>
            public static string VERSION
            {
                get
                {
                    return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
            }

            /// <summary>
            /// Used to denote Beta or Alpha builds.  Or any
            /// special branches of the application.
            /// </summary>
            public const string VERSION_ADDITIONAL = "";

            #endregion
        }
    }
}
