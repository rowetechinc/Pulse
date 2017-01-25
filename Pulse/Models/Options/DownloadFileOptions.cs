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
 * Date            Initials    Vertion    Comments
 * -----------------------------------------------------------------
 * 04/10/2012      RC          2.07       Initial coding
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Options for downloading files.
    /// This includeds the download directory and the
    /// whether to overwrite files.
    /// </summary>
    public class DownloadFileOptions
    {

        #region Defaults

        /// <summary>
        /// Default storage path for downloaded data.
        /// </summary>
        private string DEFAULT_DOWNLOAD_DIR = Pulse.Commons.GetProjectDefaultFolderPath();

        /// <summary>
        /// Default value to overwrite existing files.
        /// </summary>
        private const bool DEFAULT_OVERWRITE = false;

        /// <summary>
        /// Default value to select all files when populating the list.
        /// </summary>
        private const bool DEFAULT_SELECT_ALL = true;

        #endregion

        #region Properties

        /// <summary>
        /// Directory to download the files.
        /// </summary>
        public string DownloadDirectory { get; set; }

        /// <summary>
        /// Set flag whether to overwrite files or not.
        /// </summary>
        public bool OverwriteDownloadFiles { get; set; }

        /// <summary>
        /// Set flag whether to select all the files.
        /// When populating the list of files, use 
        /// this flag to select all the files are keep
        /// them all unselected.
        /// </summary>
        public bool SelectAllFiles { get; set; }

        #endregion

        /// <summary>
        /// Set the options for downloading the data.
        /// Use default values.
        /// </summary>
        public DownloadFileOptions()
        {
            DownloadDirectory = DEFAULT_DOWNLOAD_DIR;
            OverwriteDownloadFiles = DEFAULT_OVERWRITE;
            SelectAllFiles = DEFAULT_SELECT_ALL;
        }

        /// <summary>
        /// Set the options for downloading the data.
        /// If the directory is empty, use the default directory.
        /// </summary>
        /// <param name="dir">Directory to download data.</param>
        /// <param name="overwrite">Overwrite files.  Default = false.</param>
        /// <param name="selectAll">Select all files.  Default = true.</param>
        public DownloadFileOptions(string dir, bool overwrite = DEFAULT_OVERWRITE, bool selectAll = DEFAULT_SELECT_ALL)
        {
            if (string.IsNullOrEmpty(dir))
            {
                DownloadDirectory = DEFAULT_DOWNLOAD_DIR;
            }
            else
            {
                DownloadDirectory = dir;
            }
            OverwriteDownloadFiles = overwrite;
            SelectAllFiles = selectAll;
        }


    }
}
