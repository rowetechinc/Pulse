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
 * 09/13/2013      RC          3.1.1      Initial coding
 * 
 */

namespace RTI
{
    using System;

    /// <summary>
    /// This will be a dialog that gets an Import Project name.
    /// The user can press OK or Cancel.  To get the results, 
    /// check IsOk, IsCancel and ImportName.
    /// 
    /// ImportNameDialogViewModel vm = new ImportNameDialogViewModel();
    /// IoC.Get IWindowManager ().ShowDialog(vm);
    /// </summary>
    public class ImportNameDialogViewModel : PulseDialogViewModel
    {
        /// <summary>
        /// Import name for the project.
        /// </summary>
        private string _ImportName;
        /// <summary>
        /// Import name for the project.
        /// </summary>
        public string ImportName
        {
            get { return _ImportName; }
            set
            {
                _ImportName = value;
                this.NotifyOfPropertyChange(() => this.ImportName);
            }
        }

        /// <summary>
        /// Initialize values.
        /// </summary>
        public ImportNameDialogViewModel()
            : base("Import Project Name")
        {
            // Initialive values
            //base.DisplayName = "Import Project Name";
            IsOk = false;
            ImportName = DateTime.Today.ToString("yyyymmddhhmmss");
        }
    }
}
