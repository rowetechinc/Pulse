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
 * 04/04/2012      RC          2.07       Initial coding
 * 07/26/2012      RC          2.13       Updated the test with FileInfo and DownloadProgress and DownloadFileSize. 
 * 
 * 
 */

using NUnit.Framework;
using System;
using RTI.Commands;

namespace RTI
{
    /// <summary>
    /// Test the DownloadFile object.
    /// </summary>
    [TestFixture]
    public class DownloadFileTest
    {
        ///// <summary>
        ///// Test the empty contructor.
        ///// </summary>
        //[Test]
        //public void TestEmptyConstructor()
        //{
        //    DownloadFile file = new DownloadFile();

        //    Assert.AreEqual(string.Empty, file.FileInfo.FileName, "File Name is incorrect");
        //    Assert.AreEqual(0, file.FileInfo.FileSize, "File Size String is incorrect");
        //    Assert.AreEqual(true, file.IsSelected, "IsSelected is incorrect");
        //    Assert.AreEqual(DateTime.Now.Year, file.FileInfo.ModificationDateTime.Year, "Mod Date Year is incorrect");
        //    Assert.AreEqual(DateTime.Now.Month, file.FileInfo.ModificationDateTime.Month, "Mod Date Month is incorrect");
        //    Assert.AreEqual(DateTime.Now.Day, file.FileInfo.ModificationDateTime.Day, "Mod Date Day is incorrect");
        //    Assert.AreEqual(true, file.IsSelected, "IsSelected is incorrect.");
        //    Assert.AreEqual(0, file.DownloadFileSize, "Download File Size is incorrect.");
        //    Assert.AreEqual(0, file.DownloadProgress, "Download progress is incorrect.");
        //}

        ///// <summary>
        ///// Test the constructor that takes 1 string.
        ///// </summary>
        //[Test]
        //public void Test1Constructor()
        //{
        //    string fileInfo = "A0000001.ENS 2012/04/02 16:53:11      1.004";
        //    RTI.Commands.AdcpEnsFileInfo ensFile = new RTI.Commands.AdcpEnsFileInfo(fileInfo);

        //    DownloadFile file = new DownloadFile(ensFile);

        //    Assert.AreEqual("A0000001.ENS", file.FileInfo.FileName, "File Name is incorrect");
        //    Assert.AreEqual(1.004, file.FileInfo.FileSize, "File Size String is incorrect");
        //    Assert.AreEqual(true, file.IsSelected, "IsSelected is incorrect");
        //    Assert.AreEqual(2012, file.FileInfo.ModificationDateTime.Year, "Mod Date Year is incorrect");
        //    Assert.AreEqual(04, file.FileInfo.ModificationDateTime.Month, "Mod Date Month is incorrect");
        //    Assert.AreEqual(02, file.FileInfo.ModificationDateTime.Day, "Mod Date Day is incorrect");
        //    Assert.AreEqual(16, file.FileInfo.ModificationDateTime.Hour, "Mod Date Hour is incorrect");
        //    Assert.AreEqual(53, file.FileInfo.ModificationDateTime.Minute, "Mod Date Minute is incorrect");
        //    Assert.AreEqual(11, file.FileInfo.ModificationDateTime.Second, "Mod Date Second is incorrect");
        //    Assert.AreEqual(true, file.IsSelected, "IsSelected is incorrect.");
        //    Assert.AreEqual(1052770, file.DownloadFileSize, "Download File Size is incorrect.");
        //    Assert.AreEqual(0, file.DownloadProgress, "Download progress is incorrect.");
        //}
    }
}