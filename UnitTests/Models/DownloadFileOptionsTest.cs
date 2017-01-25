///*
// * Copyright © 2011 
// * Rowe Technology Inc.
// * All rights reserved.
// * http://www.rowetechinc.com
// * 
// * Redistribution and use in source and binary forms, with or without
// * modification is NOT permitted.
// * 
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
// * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
// * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// * POSSIBILITY OF SUCH DAMAGE.
// * 
// * HISTORY
// * -----------------------------------------------------------------
// * Date            Initials    Vertion    Comments
// * -----------------------------------------------------------------
// * 04/10/2012      RC          2.08       Initial coding
// * 
// * 
// */

//using NUnit.Framework;

//namespace RTI
//{

//    /// <summary>
//    /// Test the DownloadFileOptions object.
//    /// This object is used to store the DownloadFile options.
//    /// </summary>
//    [TestFixture]
//    public class DownloadFileOptionsTest
//    {
//        /// <summary>
//        /// Test the empty constructor for the default values.
//        /// </summary>
//        [Test]
//        public void Constructor()
//        {
//            DownloadFileOptions dfo = new DownloadFileOptions();

//            Assert.AreEqual(Pulse.Commons.GetProjectDefaultFolderPath(), dfo.DownloadDirectory, "Download Directory Incorrect");
//            Assert.AreEqual(false, dfo.OverwriteDownloadFiles, "Overwrite files flag Incorrect");
//            Assert.AreEqual(true, dfo.SelectAllFiles, "Select All Files Incorrect");
//        }

//        /// <summary>
//        /// Set directory and Overwrite file and leave
//        /// Select all as default.
//        /// </summary>
//        [Test]
//        public void SetOverwrite()
//        {
//            DownloadFileOptions dfo = new DownloadFileOptions("anyDirectory", true);

//            Assert.AreEqual("anyDirectory", dfo.DownloadDirectory, "Download Directory Incorrect");
//            Assert.AreEqual(true, dfo.OverwriteDownloadFiles, "Overwrite files flag Incorrect");
//            Assert.AreEqual(true, dfo.SelectAllFiles, "Select All Files Incorrect");
//        }

//        /// <summary>
//        /// Set All the values.
//        /// </summary>
//        [Test]
//        public void SetAllValues()
//        {
//            DownloadFileOptions dfo = new DownloadFileOptions("anyDirectory", true, false);

//            Assert.AreEqual("anyDirectory", dfo.DownloadDirectory, "Download Directory Incorrect");
//            Assert.AreEqual(true, dfo.OverwriteDownloadFiles, "Overwrite files flag Incorrect");
//            Assert.AreEqual(false, dfo.SelectAllFiles, "Select All Files Incorrect");
//        }

//        /// <summary>
//        /// If the directory is empty, it will use the default value.
//        /// </summary>
//        [Test]
//        public void SetEmptyDirectory()
//        {
//            DownloadFileOptions dfo = new DownloadFileOptions("", true, false);

//            Assert.AreEqual(Pulse.Commons.GetProjectDefaultFolderPath(), dfo.DownloadDirectory, "Download Directory Incorrect");
//            Assert.AreEqual(true, dfo.OverwriteDownloadFiles, "Overwrite files flag Incorrect");
//            Assert.AreEqual(false, dfo.SelectAllFiles, "Select All Files Incorrect");
//        }

//        /// <summary>
//        /// If the directory is null, it will use the default value.
//        /// </summary>
//        [Test]
//        public void SetNullDirectory()
//        {
//            DownloadFileOptions dfo = new DownloadFileOptions(null, true, false);

//            Assert.AreEqual(Pulse.Commons.GetProjectDefaultFolderPath(), dfo.DownloadDirectory, "Download Directory Incorrect");
//            Assert.AreEqual(true, dfo.OverwriteDownloadFiles, "Overwrite files flag Incorrect");
//            Assert.AreEqual(false, dfo.SelectAllFiles, "Select All Files Incorrect");
//        }
//    }
//}
