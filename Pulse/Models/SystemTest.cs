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
 * 07/06/2012      RC          2.12       Initial coding
 * 07/17/2012      RC          2.12       Check the board revision and serial number.
 * 07/18/2012      RC          2.12       Test the status in the Water Profile and Bottom Track ensemble.
 * 07/23/2012      RC          2.12       Added a timeout check in the Water Profile and Bottom Track status to check if an ensemble was received.
 * 08/24/2012      RC          2.12       Added a test for EngHelp.txt and maint.txt to SysTest_AdcpFirmwareFiles().
 * 10/23/2012      RC          2.15       Added StopPinging to RunSystemTest().
 * 12/06/2012      RC          2.17       Added 5 second wait to SysTest_RtcTime() to ensure the time has been set to the ADCP.
 * 12/11/2012      RC          2.17       Removed 5 second wait for SysTest_RtcTime() and will put it with the STIME command.
 * 05/07/2013      RC          3.0.0      Removed test for compass right now until RecorderManager is replaced.
 * 
 */

using System.Collections.Generic;
using System.Threading;
using System;
using System.Windows;
using Caliburn.Micro;
using ReactiveUI;

//namespace RTI
//{

//    /// <summary>
//    /// Run a system test on the ADCP.  This will test all the
//    /// aspects of the ADCP.
//    /// </summary>
//    public class SystemTest : ReactiveObject
//    {

//        #region Structs and Enums

//        /// <summary>
//        /// Test status.
//        /// </summary>
//        public enum SysTestResultStatus
//        {
//            /// <summary>
//            /// Fail.
//            /// </summary>
//            FAIL = 0,

//            /// <summary>
//            /// Pass.
//            /// </summary>
//            PASS = 1,

//            /// <summary>
//            /// Test in progress.
//            /// </summary>
//            IN_PROGRESS,

//            /// <summary>
//            /// Test not started.
//            /// </summary>
//            NOT_STARTED,

//            /// <summary>
//            /// Not testing this.
//            /// </summary>
//            NOT_TESTING
//        }

//        #endregion

//        #region Variables

//        /// <summary>
//        /// Flag if we should test the serial and revision of the board.
//        /// Testing the serial number and revision of the board should be
//        /// done in house only because there is no way of knowing the latest
//        /// revision of a board outside.
//        /// </summary>
//        private bool TEST_BOARD_SERIAL_REV = false;

//        /// <summary>
//        /// Watchdog timer to watch for any tests that hang.
//        /// </summary>
//        private System.Timers.Timer _watchDog;

//        /// <summary>
//        /// Receive events with the event aggregator.
//        /// </summary>
//        private IEventAggregator _eventAggregator;

//        #endregion

//        #region Properties

//        #region Turn On Off All Test

//        /// <summary>
//        /// Turn on or off all the test.
//        /// </summary>
//        private bool _isAllTestOn;
//        /// <summary>
//        /// Turn on or off all the test.
//        /// </summary>
//        public bool IsAllTestOn
//        {
//            get { return _isAllTestOn; }
//            set
//            {
//                _isAllTestOn = value;
//                this.RaisePropertyChanged(_ => this.IsAllTestOn);

//                // Turn on or off all the test
//                SetAllTestOnOff(_isAllTestOn);
//            }
//        }

//        #endregion

//        #region WatchDog

//        /// <summary>
//        /// Watchdog timeout in milliseconds.
//        /// </summary>
//        private int _watchDogTimeout;
//        /// <summary>
//        /// Watchdog timeout in milliseconds.
//        /// </summary>
//        public int WatchDogTimeout
//        {
//            get { return _watchDogTimeout; }
//            set { this.RaiseAndSetIfChanged(x => x.WatchDogTimeout, value); }
//        }

//        #endregion

//        #region Error List

//        /// <summary>
//        /// List of of all the errors that occured
//        /// during the system test.
//        /// </summary>
//        private ObservableCollectionEx<string> _sysTestErrorList;
//        /// <summary>
//        /// List of of all the errors that occured
//        /// during the system test.
//        /// </summary>
//        public ObservableCollectionEx<string> SysTestErrorList
//        {
//            get { return _sysTestErrorList; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestErrorList, value); }
//        }

//        #endregion

//        #region ADCP COMM

//        /// <summary>
//        /// Flag if the Adcp Comm test should be run.
//        /// </summary>
//        private bool _isSysTest_AdcpComm;
//        /// <summary>
//        /// Flag if the Adcp Comm test should be run.
//        /// </summary>
//        public bool IsSysTest_AdcpComm
//        {
//            get { return _isSysTest_AdcpComm; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_AdcpComm, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP communication.
//        /// Did we get a break and were able to
//        /// set the serial number, firmware and hardware.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_AdcpComm;
//        /// <summary>
//        /// Test result for ADCP communication.
//        /// Did we get a break and were able to
//        /// set the serial number, firmware and hardware.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_AdcpComm
//        {
//            get { return _sysTestResult_AdcpComm; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_AdcpComm, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_AdcpCommImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_AdcpComm);
//            }
//        }

//        #endregion

//        #region Firmware Files

//        #region Help.txt

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the Help.txt file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileHelp;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the Help.txt file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileHelp
//        {
//            get { return _isSysTest_FirmwareFileHelp; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileHelp, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file Help.txt.
//        /// This will check that Help.txt is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileHelp;
//        /// <summary>
//        /// Test result for ADCP Firmware file Help.txt.
//        /// This will check that Help.txt is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileHelp
//        {
//            get { return _sysTestResult_FirmwareFileHelp; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileHelp, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileHelpImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileHelp);
//            }
//        }

//        #endregion

//        #region EngHelp.txt

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the EngHelp.txt file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileEngHelp;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the EngHelp.txt file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileEngHelp
//        {
//            get { return _isSysTest_FirmwareFileEngHelp; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileEngHelp, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file EngHelp.txt.
//        /// This will check that EngHelp.txt is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileEngHelp;
//        /// <summary>
//        /// Test result for ADCP Firmware file EngHelp.txt.
//        /// This will check that EngHelp.txt is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileEngHelp
//        {
//            get { return _sysTestResult_FirmwareFileEngHelp; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileEngHelp, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileEngHelpImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileEngHelp);
//            }
//        }

//        #endregion

//        #region maint.txt

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the maint.txt file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileMaint;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the maint.txt file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileMaint
//        {
//            get { return _isSysTest_FirmwareFileMaint; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileMaint, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file maint.txt.
//        /// This will check that maint.txt is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileMaint;
//        /// <summary>
//        /// Test result for ADCP Firmware file maint.txt.
//        /// This will check that maint.txt is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileMaint
//        {
//            get { return _sysTestResult_FirmwareFileMaint; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileMaint, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileMaintImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileMaint);
//            }
//        }

//        #endregion

//        #region RTISYS.bin

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the RTISYS.bin file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileRtisys;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the RTISYS.bin file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileRtisys
//        {
//            get { return _isSysTest_FirmwareFileRtisys; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileRtisys, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file RTISYS.bin.
//        /// This will check that RTISYS.bin is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileRtisys;
//        /// <summary>
//        /// Test result for ADCP Firmware file RTISYS.bin.
//        /// This will check that RTISYS.bin is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileRtisys
//        {
//            get { return _sysTestResult_FirmwareFileRtisys; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileRtisys, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileRtisysImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileRtisys);
//            }
//        }

//        #endregion

//        #region BOOT.bin

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the BOOT.bin file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileBoot;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the BOOT.bin file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileBoot
//        {
//            get { return _isSysTest_FirmwareFileBoot; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileBoot, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file BOOT.bin.
//        /// This will check that BOOT.bin is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileBoot;
//        /// <summary>
//        /// Test result for ADCP Firmware file BOOT.bin.
//        /// This will check that BOOT.bin is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileBoot
//        {
//            get { return _sysTestResult_FirmwareFileBoot; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileBoot, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileBootImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileBoot);
//            }
//        }

//        #endregion

//        #region SYSCONF.bin

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the SYSCONF.bin file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileSysconf;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the SYSCONF.bin file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileSysconf
//        {
//            get { return _isSysTest_FirmwareFileSysconf; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileSysconf, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file SYSCONF.bin.
//        /// This will check that SYSCONF.bin is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileSysconf;
//        /// <summary>
//        /// Test result for ADCP Firmware file SYSCONF.bin.
//        /// This will check that SYSCONF.bin is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileSysconf
//        {
//            get { return _sysTestResult_FirmwareFileSysconf; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileSysconf, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileSysconfImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileSysconf);
//            }
//        }

//        #endregion

//        #region BBCODE.bin

//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the BBCODE.bin file should be run.
//        /// </summary>
//        private bool _isSysTest_FirmwareFileBbcode;
//        /// <summary>
//        /// Flag if the Adcp Firmware test to check for the BBCODE.bin file should be run.
//        /// </summary>
//        public bool IsSysTest_FirmwareFileBbcode
//        {
//            get { return _isSysTest_FirmwareFileBbcode; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_FirmwareFileBbcode, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware file BBCODE.bin.
//        /// This will check that BBCODE.bin is present.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_FirmwareFileBbcode;
//        /// <summary>
//        /// Test result for ADCP Firmware file BBCODE.bin.
//        /// This will check that BBCODE.bin is present.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_FirmwareFileBbcode
//        {
//            get { return _sysTestResult_FirmwareFileBbcode; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_FirmwareFileBbcode, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_FirmwareFileBbcodeImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_FirmwareFileBbcode);
//            }
//        }

//        #endregion

//        #endregion

//        #region Firmware Version

//        /// <summary>
//        /// Flag if the Adcp Firmware Version test should be run.
//        /// </summary>
//        private bool _isSysTest_AdcpFirmwareVersion;
//        /// <summary>
//        /// Flag if the Adcp Firmware Version test should be run.
//        /// </summary>
//        public bool IsSysTest_AdcpFirmwareVersion
//        {
//            get { return _isSysTest_AdcpFirmwareVersion; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_AdcpFirmwareVersion, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Firmware Version.
//        /// This will check that the correct firmware version.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_AdcpFirmwareVersion;
//        /// <summary>
//        /// Test result for ADCP Firmware.
//        /// This will check that the correct firmware version.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_AdcpFirmwareVersion
//        {
//            get { return _sysTestResult_AdcpFirmwareVersion; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_AdcpFirmwareVersion, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_AdcpFirmwareVersionImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_AdcpFirmwareVersion);
//            }
//        }

//        /// <summary>
//        /// Firmware version string.
//        /// </summary>
//        private string _firmwareVersionString;
//        /// <summary>
//        /// Firmware version string.
//        /// </summary>
//        public string FirmwareVersionString
//        {
//            get { return _firmwareVersionString; }
//            set { this.RaiseAndSetIfChanged(x => x.FirmwareVersionString, value); }
//        }

//        #endregion

//        #region I2C Memory Devices

//        #region I/O Board

//        /// <summary>
//        /// Flag if the ADCP Board I/O test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardIo;
//        /// <summary>
//        /// Flag if the ADCP Board I/O test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardIo
//        {
//            get { return _isSysTest_BoardIo; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardIo, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board I/O.
//        /// This will verify the serial number and revision are correct
//        /// for the I/O board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardIo;
//        /// <summary>
//        /// Test result for ADCP Board I/O.
//        /// This will verify the serial number and revision are correct
//        /// for the I/O board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardIo
//        {
//            get { return _sysTestResult_BoardIo; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardIo, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardIoImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardIo);
//            }
//        }

//        #endregion

//        #region Low Pwr Reg Board

//        /// <summary>
//        /// Flag if the ADCP Board Low Power Reg test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardLowPwrReg;
//        /// <summary>
//        /// Flag if the ADCP Board Low Power Reg test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardLowPwrReg
//        {
//            get { return _isSysTest_BoardLowPwrReg; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardLowPwrReg, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board Low Power Reg.
//        /// This will verify the serial number and revision are correct
//        /// for the Low Power Reg board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardLowPwrReg;
//        /// <summary>
//        /// Test result for ADCP Board Low Power Reg.
//        /// This will verify the serial number and revision are correct
//        /// for the Low Power Reg board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardLowPwrReg
//        {
//            get { return _sysTestResult_BoardLowPwrReg; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardLowPwrReg, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardLowPwrRegImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardLowPwrReg);
//            }
//        }

//        #endregion

//        #region Xmitter Board

//        /// <summary>
//        /// Flag if the ADCP Board Xmitter test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardXmitter;
//        /// <summary>
//        /// Flag if the ADCP Board Xmitter test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardXmitter
//        {
//            get { return _isSysTest_BoardXmitter; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardXmitter, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board Xmitter.
//        /// This will verify the serial number and revision are correct
//        /// for the Xmitter board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardXmitter;
//        /// <summary>
//        /// Test result for ADCP Board Xmitter.
//        /// This will verify the serial number and revision are correct
//        /// for the Xmitter board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardXmitter
//        {
//            get { return _sysTestResult_BoardXmitter; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardXmitter, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardXmitterImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardXmitter);
//            }
//        }

//        #endregion

//        #region Virtual Gnd Board

//        /// <summary>
//        /// Flag if the ADCP Board Virtual Gnd test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardVirtualGnd;
//        /// <summary>
//        /// Flag if the ADCP Board Virtual Gnd test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardVirtualGnd
//        {
//            get { return _isSysTest_BoardVirtualGnd; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardVirtualGnd, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board Virtual Gnd.
//        /// This will verify the serial number and revision are correct
//        /// for the Virtual Gnd board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardVirtualGnd;
//        /// <summary>
//        /// Test result for ADCP Board Virtual Gnd.
//        /// This will verify the serial number and revision are correct
//        /// for the Virtual Gnd board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardVirtualGnd
//        {
//            get { return _sysTestResult_BoardVirtualGnd; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardVirtualGnd, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardVirtualGndImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardVirtualGnd);
//            }
//        }

//        #endregion

//        #region Rcvr Board

//        /// <summary>
//        /// Flag if the ADCP Board Rcvr test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardRcvr;
//        /// <summary>
//        /// Flag if the ADCP Board Rcvr test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardRcvr
//        {
//            get { return _isSysTest_BoardRcvr; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardRcvr, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board Rcvr.
//        /// This will verify the serial number and revision are correct
//        /// for the Rcvr board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardRcvr;
//        /// <summary>
//        /// Test result for ADCP Board Rcvr.
//        /// This will verify the serial number and revision are correct
//        /// for the Rcvr board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardRcvr
//        {
//            get { return _sysTestResult_BoardRcvr; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardRcvr, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardRcvrImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardRcvr);
//            }
//        }

//        #endregion

//        #region Back Plane Board

//        /// <summary>
//        /// Flag if the ADCP Board Back Plane test should be run.
//        /// </summary>
//        private bool _isSysTest_BoardBackPlane;
//        /// <summary>
//        /// Flag if the ADCP Board Back Plane test should be run.
//        /// </summary>
//        public bool IsSysTest_BoardBackPlane
//        {
//            get { return _isSysTest_BoardBackPlane; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_BoardBackPlane, value); }
//        }

//        /// <summary>
//        /// Test result for ADCP Board Back Plane.
//        /// This will verify the serial number and revision are correct
//        /// for the Back Plane board for the ADCP.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_BoardBackPlane;
//        /// <summary>
//        /// Test result for ADCP Board Back Plane.
//        /// This will verify the serial number and revision are correct
//        /// for the Back Plane board for the ADCP.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_BoardBackPlane
//        {
//            get { return _sysTestResult_BoardBackPlane; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_BoardBackPlane, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_BoardBackPlaneImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_BoardBackPlane);
//            }
//        }

//        #endregion

//        #endregion

//        #region Serial Number

//        /// <summary>
//        /// Serial number of the system.
//        /// </summary>
//        private SerialNumber _sysSerialNumber;
//        /// <summary>
//        /// Serial number of the system.
//        /// </summary>
//        public SerialNumber SysSerialNumber
//        {
//            get { return _sysSerialNumber; }
//            set
//            {
//                _sysSerialNumber = value;
//                this.RaisePropertyChanged(_ => this.SysSerialNumber);
//                this.RaisePropertyChanged(_ => this.SysSerialNumberString);
//                this.RaisePropertyChanged(_ => this.BoardSerialIo);
//                this.RaisePropertyChanged(_ => this.BoardSerialLowPwrReg);
//                this.RaisePropertyChanged(_ => this.BoardSerialXmitter);
//                this.RaisePropertyChanged(_ => this.BoardSerialVirtualGnd);
//                this.RaisePropertyChanged(_ => this.BoardSerialRcvr);
//                this.RaisePropertyChanged(_ => this.BoardSerialBackplane);
//            }
//        }


//        /// <summary>
//        /// Serial number of the system displayed as a string.
//        /// </summary>
//        public string SysSerialNumberString
//        {
//            get { return _sysSerialNumber.ToString(); }
//            set
//            {
//                SysSerialNumber = new SerialNumber(value);
//                this.RaisePropertyChanged(_ => this.SysSerialNumberString);
//            }
//        }

//        #region Board Serial Numbers

//        /// <summary>
//        /// Serial number for the IO board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialIo;
//        /// <summary>
//        /// Serial number for the IO board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialIo
//        {
//            get { return _boardSerialIo; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialIo, value); }
//        }

//        /// <summary>
//        /// Serial number for the Low Power Reg board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialLowPwrReg;
//        /// <summary>
//        /// Serial number for the Low Power Reg board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialLowPwrReg
//        {
//            get { return _boardSerialLowPwrReg; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialLowPwrReg, value); }
//        }

//        /// <summary>
//        /// Serial number for the Xmitter board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialXmitter;
//        /// <summary>
//        /// Serial number for the Xmitter board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialXmitter
//        {
//            get { return _boardSerialXmitter; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialXmitter, value); }
//        }

//        /// <summary>
//        /// Serial number for the Virtual Ground board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialVirtualGnd;
//        /// <summary>
//        /// Serial number for the Virtual Ground board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialVirtualGnd
//        {
//            get { return _boardSerialVirtualGnd; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialVirtualGnd, value); }
//        }

//        /// <summary>
//        /// Serial number for the Receiver board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialRcvr;
//        /// <summary>
//        /// Serial number for the Receiver board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialRcvr
//        {
//            get { return _boardSerialRcvr; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialRcvr, value); }
//        }

//        /// <summary>
//        /// Serial number for the Backplane board.  They should match the system serial number.
//        /// </summary>
//        private string _boardSerialBackplane;
//        /// <summary>
//        /// Serial number for the Backplane board.  They should match the system serial number.
//        /// </summary>
//        public string BoardSerialBackplane
//        {
//            get { return _boardSerialBackplane; }
//            set { this.RaiseAndSetIfChanged(x => x.BoardSerialBackplane, value); }
//        }

//        #endregion

//        #endregion

//        #region Revision

//        #region Board Revision

//        /// <summary>
//        /// Revision for the IO board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevIo;
//        /// <summary>
//        /// Revision for the IO board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevIo
//        {
//            get { return _boardRevIo; }
//            set
//            {
//                _boardRevIo = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevIo);
//            }
//        }

//        /// <summary>
//        /// Revision for the Low Power Reg board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevLowPwrReg;
//        /// <summary>
//        /// Revision for the Low Power Reg board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevLowPwrReg
//        {
//            get { return _boardRevLowPwrReg; }
//            set
//            {
//                _boardRevLowPwrReg = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevLowPwrReg);
//            }
//        }

//        /// <summary>
//        /// Revision for the Xmitter board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevXmitter;
//        /// <summary>
//        /// Revision for the Xmitter board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevXmitter
//        {
//            get { return _boardRevXmitter; }
//            set
//            {
//                _boardRevXmitter = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevXmitter);
//            }
//        }

//        /// <summary>
//        /// Revision for the Virtual Ground board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevVirtualGnd;
//        /// <summary>
//        /// Revision for the Virtual Ground board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevVirtualGnd
//        {
//            get { return _boardRevVirtualGnd; }
//            set
//            {
//                _boardRevVirtualGnd = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevVirtualGnd);
//            }
//        }

//        /// <summary>
//        /// Revision for the Receiver board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevRcvr;
//        /// <summary>
//        /// Revision for the Receiver board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevRcvr
//        {
//            get { return _boardRevRcvr; }
//            set
//            {
//                _boardRevRcvr = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevRcvr);
//            }
//        }

//        /// <summary>
//        /// Revision for the Backplane board.  They should match the system serial number.
//        /// </summary>
//        private string _boardRevBackplane;
//        /// <summary>
//        /// Revision for the Backplane board.  They should match the system serial number.
//        /// </summary>
//        public string BoardRevBackplane
//        {
//            get { return _boardRevBackplane; }
//            set
//            {
//                _boardRevBackplane = value.ToUpper();
//                this.RaisePropertyChanged(_ => this.BoardRevBackplane);
//            }
//        }

//        #endregion

//        #endregion

//        #region Compass Test

//        /// <summary>
//        /// Flag if the Adcp compass output test should be run.
//        /// </summary>
//        private bool _isSysTest_AdcpCompass;
//        /// <summary>
//        /// Flag if the Adcp Compass output test should be run.
//        /// </summary>
//        public bool IsSysTest_AdcpCompass
//        {
//            get { return _isSysTest_AdcpCompass; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_AdcpCompass, value); }
//        }

//        /// <summary>
//        /// Test compass in the ADCP is giving valid values.
//        /// This will read the compass data and verify data is returned.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_AdcpCompass;
//        /// <summary>
//        /// Test compass in the ADCP is giving valid values.
//        /// This will read the compass data and verify data is returned.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_AdcpCompass
//        {
//            get { return _sysTestResult_AdcpCompass; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_AdcpCompass, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_AdcpCompassImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_AdcpCompass);
//            }
//        }

//        /// <summary>
//        /// Number of taps to expect to find.
//        /// </summary>
//        private int _compassTaps;
//        /// <summary>
//        /// Number of taps to expect to find.
//        /// </summary>
//        public int CompassTaps
//        {
//            get { return _compassTaps; }
//            set { this.RaiseAndSetIfChanged(x => x.CompassTaps, value); }
//        }

//        /// <summary>
//        /// Flag if the Compass Tap test should be run.
//        /// </summary>
//        private bool _isSysTest_CompassTap;
//        /// <summary>
//        /// Flag if the Compass Tap test should be run.
//        /// </summary>
//        public bool IsSysTest_CompassTap
//        {
//            get { return _isSysTest_CompassTap; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_CompassTap, value); }
//        }

//        /// <summary>
//        /// Test result for Compass Tap.
//        /// This will check that the compass tap is set correctly.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_CompassTap;
//        /// <summary>
//        /// Test result for Compass Tap.
//        /// This will check that the compass tap is set correctly.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_CompassTap
//        {
//            get { return _sysTestResult_CompassTap; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_CompassTap, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_CompassTapImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_CompassTap);
//            }
//        }

//        #endregion

//        #region RTC Time Test

//        /// <summary>
//        /// The system Date and Time.  Date and time
//        /// read from the ADCP.
//        /// </summary>
//        private DateTime _sysDateTime;
//        /// <summary>
//        /// The system Date and Time.  Date and time
//        /// read from the ADCP.
//        /// </summary>
//        public DateTime SysDateTime
//        {
//            get { return _sysDateTime; }
//            set
//            {
//                _sysDateTime = value;
//                this.RaisePropertyChanged(_ => this.SysDateTime);
//                this.RaisePropertyChanged(_ => this.SysDateTimeString);
//            }
//        }

//        /// <summary>
//        /// System Date and Time as a string.
//        /// </summary>
//        public string SysDateTimeString
//        {
//            get { return _sysDateTime.ToString(); }
//            set
//            {
//                this.RaisePropertyChanged(_ => this.SysDateTimeString);
//            }
//        }

//        /// <summary>
//        /// Flag if the RTC Time test should be run.
//        /// </summary>
//        private bool _isSysTest_RtcTime;
//        /// <summary>
//        /// Flag if the RTC Time test should be run.
//        /// </summary>
//        public bool IsSysTest_RtcTime
//        {
//            get { return _isSysTest_RtcTime; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_RtcTime, value); }
//        }

//        /// <summary>
//        /// Test that the RTC can be set and is giving the correct time.
//        /// This will set the RTC time.  Then verify the correct time is given
//        /// when checking the time.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_RtcTime;
//        /// <summary>
//        /// Test that the RTC can be set and is giving the correct time.
//        /// This will set the RTC time.  Then verify the correct time is given
//        /// when checking the time.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_RtcTime
//        {
//            get { return _sysTestResult_RtcTime; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_RtcTime, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_RtcTimeImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_RtcTime);
//            }
//        }

//        #endregion

//        #region ADCP Status Test

//        #region Water Profile

//        /// <summary>
//        /// Flag if the Water Profile Status test should be run.
//        /// </summary>
//        private bool _isSysTest_AdcpStatusWp;
//        /// <summary>
//        /// Flag if the Water Profile Status test should be run.
//        /// </summary>
//        public bool IsSysTest_AdcpStatusWp
//        {
//            get { return _isSysTest_AdcpStatusWp; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_AdcpStatusWp, value); }
//        }

//        /// <summary>
//        /// Check the Water Profile status for any errors.  The status will detect
//        /// hardware or ping issues.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_AdcpStatusWp;
//        /// <summary>
//        /// Check the Water Profile status for any errors.  The status will detect
//        /// hardware or ping issues.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_AdcpStatusWp
//        {
//            get { return _sysTestResult_AdcpStatusWp; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_AdcpStatusWp, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_AdcpStatusWpImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_AdcpStatusWp);
//            }
//        }

//        /// <summary>
//        /// Status string for the Water Profile data.
//        /// </summary>
//        private string _adcpStatusWpString;
//        /// <summary>
//        /// Status string for the Water Profile data.
//        /// </summary>
//        public string AdcpStatusWpString
//        {
//            get { return _adcpStatusWpString; }
//            set { this.RaiseAndSetIfChanged(x => x.AdcpStatusWpString, value); }
//        }

//        #endregion

//        #region Bottom Track

//        /// <summary>
//        /// Flag if the Bottom Track Status test should be run.
//        /// </summary>
//        private bool _isSysTest_AdcpStatusBt;
//        /// <summary>
//        /// Flag if the Bottom Track Status test should be run.
//        /// </summary>
//        public bool IsSysTest_AdcpStatusBt
//        {
//            get { return _isSysTest_AdcpStatusBt; }
//            set { this.RaiseAndSetIfChanged(x => x.IsSysTest_AdcpStatusBt, value); }
//        }

//        /// <summary>
//        /// Check the Bottom Track status for any errors.  The status will detect
//        /// hardware or ping issues.
//        /// </summary>
//        private SysTestResultStatus _sysTestResult_AdcpStatusBt;
//        /// <summary>
//        /// Check the Bottom Track status for any errors.  The status will detect
//        /// hardware or ping issues.
//        /// </summary>
//        public SysTestResultStatus SysTestResult_AdcpStatusBt
//        {
//            get { return _sysTestResult_AdcpStatusBt; }
//            set { this.RaiseAndSetIfChanged(x => x.SysTestResult_AdcpStatusBt, value); }
//        }
//        /// <summary>
//        /// Get the image to display based off
//        /// the test has passed or failed.
//        /// </summary>
//        public string SysTestResult_AdcpStatusBtImage
//        {
//            get
//            {
//                return SysTestResultImage(_sysTestResult_AdcpStatusBt);
//            }
//        }

//        /// <summary>
//        /// Status string for the Bottom Track data.
//        /// </summary>
//        private string _adcpStatusBtString;
//        /// <summary>
//        /// Status string for the Bottom Track data.
//        /// </summary>
//        public string AdcpStatusBtString
//        {
//            get { return _adcpStatusBtString; }
//            set { this.RaiseAndSetIfChanged(x => x.AdcpStatusBtString, value); }
//        }

//        #endregion

//        #endregion

//        #endregion

//        /// <summary>
//        /// Initialize the values.
//        /// </summary>
//        public SystemTest(IEventAggregator eventAggregator)
//        {
//            // Init values
//            _eventAggregator = eventAggregator;
            
//            // Compass
//            CompassTaps = 4;
            
//            // Serial Number
//            SysSerialNumber = new SerialNumber();

//            // Firmware Version
//            FirmwareVersionString = "";

//            // Status
//            AdcpStatusWpString = "";
//            AdcpStatusBtString = "";

//            // Watchdog
//            //_watchDogFail = false;
//            WatchDogTimeout = 5000;
//            _watchDog = new System.Timers.Timer();
//            _watchDog.Elapsed += new System.Timers.ElapsedEventHandler(_watchDog_Elapsed);
//            _watchDog.Interval = WatchDogTimeout;

//            // Init test
//            InitTestResults();
//            IsAllTestOn = true;
//            SysTestErrorList = new ObservableCollectionEx<string>();
//        }

//        /// <summary>
//        /// Shutdown the object.
//        /// </summary>
//        public void Shutdown()
//        {
//            _watchDog.Enabled = false;
//            _watchDog.Dispose();
//        }


//        #region Init

//        /// <summary>
//        /// Initialize the test results.
//        /// </summary>
//        private void InitTestResults()
//        {
//            SysTestResult_AdcpComm = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_AdcpFirmwareVersion = SysTestResultStatus.NOT_STARTED;

//            SysTestResult_FirmwareFileHelp = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileEngHelp = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileMaint = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileRtisys = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileBoot = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileSysconf = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_FirmwareFileBbcode = SysTestResultStatus.NOT_STARTED;

//            SysTestResult_BoardIo = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_BoardLowPwrReg = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_BoardXmitter = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_BoardVirtualGnd = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_BoardRcvr = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_BoardBackPlane = SysTestResultStatus.NOT_STARTED;

//            SysTestResult_AdcpCompass = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_CompassTap = SysTestResultStatus.NOT_STARTED;

//            SysTestResult_AdcpStatusWp = SysTestResultStatus.NOT_STARTED;
//            SysTestResult_AdcpStatusBt = SysTestResultStatus.NOT_STARTED;

//            SysTestResult_RtcTime = SysTestResultStatus.NOT_STARTED;

//            //SysSerialNumberString = "";
//            SysSerialNumber = new SerialNumber();

//            FirmwareVersionString = "";

//            AdcpStatusWpString = "";
//            AdcpStatusBtString = "";

//            BoardSerialBackplane = "";
//            BoardSerialIo = "";
//            BoardSerialLowPwrReg = "";
//            BoardSerialRcvr = "";
//            BoardSerialVirtualGnd = "";
//            BoardSerialXmitter = "";
//            BoardRevBackplane = "";
//            BoardRevIo = "";
//            BoardRevLowPwrReg = "";
//            BoardRevRcvr = "";
//            BoardRevVirtualGnd = "";
//            BoardRevXmitter = "";
//        }

//        /// <summary>
//        /// Turn on or off all the test.
//        /// </summary>
//        /// <param name="turnOnTest">TRUE = Turn ON all test.</param>
//        private void SetAllTestOnOff(bool turnOnTest)
//        {
//            IsSysTest_AdcpComm = turnOnTest;
//            IsSysTest_AdcpFirmwareVersion = turnOnTest;

//            IsSysTest_FirmwareFileHelp = turnOnTest;
//            IsSysTest_FirmwareFileEngHelp = turnOnTest;
//            IsSysTest_FirmwareFileMaint = turnOnTest;
//            IsSysTest_FirmwareFileRtisys = turnOnTest;
//            IsSysTest_FirmwareFileBoot = turnOnTest;
//            IsSysTest_FirmwareFileSysconf = turnOnTest;
//            IsSysTest_FirmwareFileBbcode = turnOnTest;

//            IsSysTest_BoardIo = turnOnTest;
//            IsSysTest_BoardLowPwrReg = turnOnTest;
//            IsSysTest_BoardXmitter = turnOnTest;
//            IsSysTest_BoardVirtualGnd = turnOnTest;
//            IsSysTest_BoardRcvr = turnOnTest;
//            IsSysTest_BoardBackPlane = turnOnTest;

//            IsSysTest_AdcpCompass = turnOnTest;
//            IsSysTest_CompassTap = turnOnTest;

//            IsSysTest_RtcTime = turnOnTest;

//            IsSysTest_AdcpStatusWp = turnOnTest;
//            IsSysTest_AdcpStatusBt = turnOnTest;
//        }

//        #endregion

//        #region Run Test

//        /// <summary>
//        /// Run all the test selected.  For each test 
//        /// selected, it will run the test.
//        /// </summary>
//        /// <param name="adcpSerialPort">ADCP serial port to pass commands to the ADCP for the test.</param>
//        public void RunSystemTest(AdcpSerialPort adcpSerialPort)
//        {
//            // Stop the ADCP from pinging
//            adcpSerialPort.StopPinging();

//            // Clear the error list and reinitialize the test results
//            SysTestErrorList.Clear();
//            InitTestResults();

//            // ADCP Communication
//            if (_isSysTest_AdcpComm)
//            {
//                SysTest_AdcpComm(adcpSerialPort);
//            }

//            // ADCP Firmware Version
//            if (_isSysTest_AdcpFirmwareVersion)
//            {
//                SysTest_AdcpFirmwareVersion(adcpSerialPort);
//            }

//            // RTC Time
//            // THIS TEST NEEDS TO BEFORE THE STATUS TESTS
//            if (_isSysTest_RtcTime)
//            {
//                SysTest_RtcTime(adcpSerialPort);
//            }

//            // Water Profile Status
//            if (_isSysTest_AdcpStatusWp)
//            {
//                SysTest_AdcpStatusWp(adcpSerialPort);
//            }

//            // Bottom Track Status
//            if (_isSysTest_AdcpStatusBt)
//            {
//                SysTest_AdcpStatusBt(adcpSerialPort);
//            }

//            // ADCP I2C Memory Devices
//            if (_isSysTest_BoardIo || _isSysTest_BoardLowPwrReg || _isSysTest_BoardXmitter || _isSysTest_BoardVirtualGnd || _isSysTest_BoardRcvr || _isSysTest_BoardBackPlane)
//            {
//                SysTest_BoardSerialRev(adcpSerialPort);
//            }

//            // ADCP Firmware Files
//            if (_isSysTest_FirmwareFileHelp || _isSysTest_FirmwareFileEngHelp || _isSysTest_FirmwareFileMaint || _isSysTest_FirmwareFileRtisys || _isSysTest_FirmwareFileBoot || _isSysTest_FirmwareFileSysconf || _isSysTest_FirmwareFileBbcode)
//            {
//                SysTest_AdcpFirmwareFiles(adcpSerialPort);
//            }

//            //// Compass 
//            //if (_isSysTest_AdcpCompass)
//            //{
//            //    SysTest_AdcpCompass(adcpSerialPort);
//            //}

//            //// Compass Taps
//            //if (_isSysTest_CompassTap)
//            //{
//            //    SysTest_CompassTap(adcpSerialPort);
//            //}
//        }

//        #endregion

//        #region ADCP Communication Test

//        /// <summary>
//        /// Run the ADCP communication test.
//        /// </summary>
//        private void SysTest_AdcpComm(AdcpSerialPort adcpSerialPort)
//        {
//            if (_isSysTest_AdcpComm)
//            {
//                SysTestResult_AdcpComm = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Run Test
//            SystemTestResult result = adcpSerialPort.SysTestAdcpCommunication();

//            // Get the serial number from the result
//            if (result.Results is RTI.Commands.BreakStmt)
//            {
//                RTI.Commands.BreakStmt breakStmt = (RTI.Commands.BreakStmt)result.Results;
//                SysSerialNumber = breakStmt.SerialNum;
//            }

//            // Check results
//            if (result.TestResult)
//            {
//                SysTestResult_AdcpComm = SysTestResultStatus.PASS;
//            }
//            else
//            {
//                SysTestResult_AdcpComm = SysTestResultStatus.FAIL;

//                // Add all the errors
//                foreach (string error in result.ErrorListStrings)
//                {
//                    SysTestErrorList.Add(error);
//                }
//            }
//        }

//        #endregion

//        #region ADCP Firmware Version Test

//        /// <summary>
//        /// Run the ADCP firmware Version test.
//        /// </summary>
//        private void SysTest_AdcpFirmwareVersion(AdcpSerialPort adcpSerialPort)
//        {
//            // Start test
//            SysTestResult_AdcpFirmwareVersion = SysTestResultStatus.IN_PROGRESS;

//            // Run test
//            SystemTestResult result = adcpSerialPort.SysTestFirmwareVersion(false);

//            // Set the version found

//            if (result.Results is RTI.Commands.BreakStmt)
//            {
//                RTI.Commands.BreakStmt brkStmt = (RTI.Commands.BreakStmt)result.Results;

//                FirmwareVersionString = brkStmt.FirmwareVersion.ToString();
//            }

//            // Check results
//            if (result.TestResult)
//            {
//                SysTestResult_AdcpFirmwareVersion = SysTestResultStatus.PASS;
//            }
//            else
//            {
//                SysTestResult_AdcpFirmwareVersion = SysTestResultStatus.FAIL;

//                // Add all the errors
//                foreach (string error in result.ErrorListStrings)
//                {
//                    SysTestErrorList.Add(error);
//                }
//            }
//        }

//        #endregion

//        #region ADCP Board Serial and Rev Test

//        /// <summary>
//        /// Run the ADCP I2C Memory Devices test.
//        /// This will check the boards serial number and revision.
//        /// </summary>
//        private void SysTest_BoardSerialRev(AdcpSerialPort adcpSerialPort)
//        {
//            // Start test
//            if (_isSysTest_BoardIo)
//            {
//                SysTestResult_BoardIo = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_BoardLowPwrReg)
//            {
//                SysTestResult_BoardLowPwrReg = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_BoardXmitter)
//            {
//                SysTestResult_BoardXmitter = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_BoardVirtualGnd)
//            {
//                SysTestResult_BoardVirtualGnd = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_BoardRcvr)
//            {
//                SysTestResult_BoardRcvr = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_BoardBackPlane)
//            {
//                SysTestResult_BoardBackPlane = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Create the I2c Mem Dev object that will contain the serial number and revisions
//            // for all the boards
//            RTI.Commands.I2cMemDevs memDevs = CreateBoardSerialRev();

//            // Run test
//            SystemTestResult result = adcpSerialPort.SysTestI2cMemoryDevices(TEST_BOARD_SERIAL_REV, memDevs);

//            // Check results
//            SetResultsIo(result);                       // I/O Board
//            SetResultsLowPwrReg(result);                // Low Pwr Reg Board
//            SetResultsXmitter(result);                  // Xmitter Board
//            SetResultsVirtualGnd(result);               // Virtual Gnd Board
//            SetResultsRcvr(result);                     // Rcvr Board
//            SetResultsBackPlane(result);                // Back Plane Board

//            // Add all the error strings to the list
//            foreach (string error in result.ErrorListStrings)
//            {
//                SysTestErrorList.Add(error);
//            }
//        }

//        /// <summary>
//        /// Populate the object with the current board serial number
//        /// and revision.  This will be used to test against the boards
//        /// found in the ADCP.
//        /// </summary>
//        /// <returns>Object containing the serial number and revision for all the boards.</returns>
//        private RTI.Commands.I2cMemDevs CreateBoardSerialRev()
//        {
//            RTI.Commands.I2cMemDevs memDevs = new Commands.I2cMemDevs();

//            // Back Plane
//            memDevs.BackPlaneBoard.SetSerial(BoardSerialBackplane);
//            memDevs.BackPlaneBoard.Revision = BoardRevBackplane;

//            // I/O
//            memDevs.IoBoard.SetSerial(BoardSerialIo);
//            memDevs.IoBoard.Revision = BoardRevIo;

//            // Low Power Reg
//            memDevs.LowPwrRegBoard.SetSerial(BoardSerialLowPwrReg);
//            memDevs.LowPwrRegBoard.Revision = BoardRevLowPwrReg;

//            // RCVR
//            memDevs.RcvrBoard.SetSerial(BoardSerialRcvr);
//            memDevs.RcvrBoard.Revision = BoardRevRcvr;

//            // Virtual Gnd
//            memDevs.VirtualGndBoard.SetSerial(BoardSerialVirtualGnd);
//            memDevs.VirtualGndBoard.Revision = BoardRevVirtualGnd;

//            // Xmitter
//            memDevs.XmitterBoard.SetSerial(BoardSerialXmitter);
//            memDevs.XmitterBoard.Revision = BoardRevXmitter;

//            return memDevs;
//        }

//        #region I/O Board Result

//        /// <summary>
//        /// Check the results for the I/O board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsIo(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardIo)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardIo = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_IO_BRD))
//                    {
//                        SysTestResult_BoardIo = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardIo != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardIo = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_IO_BRD))
//                    {
//                        SysTestResult_BoardIo = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardIo != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardIo = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;    // Cast object to get serial and rev

//                    BoardSerialIo = memDevsResult.IoBoard.SerialNum.ToString();                         // Serial number
//                    BoardRevIo = memDevsResult.IoBoard.Revision;                                        // Revision
//                }
//            }
//        }

//        #endregion

//        #region Low Power Reg Board Result

//        /// <summary>
//        /// Check the results for the Low Power Reg board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsLowPwrReg(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardLowPwrReg)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardLowPwrReg = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_LOW_PWR_REG_BRD))
//                    {
//                        SysTestResult_BoardLowPwrReg = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardLowPwrReg != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardLowPwrReg = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_LOW_PWR_REG_BRD))
//                    {
//                        SysTestResult_BoardLowPwrReg = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardLowPwrReg != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardLowPwrReg = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;            // Cast object to get serial and rev

//                    BoardSerialLowPwrReg = memDevsResult.LowPwrRegBoard.SerialNum.ToString();                   // Serial number
//                    BoardRevLowPwrReg = memDevsResult.LowPwrRegBoard.Revision;                                  // Revision
//                }
//            }
//        }

//        #endregion

//        #region Xmitter Board Result

//        /// <summary>
//        /// Check the results for the Xmitter board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsXmitter(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardXmitter)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardXmitter = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_XMITTER_BRD))
//                    {
//                        SysTestResult_BoardXmitter = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardXmitter != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardXmitter = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_XMITTER_BRD))
//                    {
//                        SysTestResult_BoardXmitter = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardXmitter != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardXmitter = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;            // Cast object to get serial and rev

//                    BoardSerialXmitter = memDevsResult.XmitterBoard.SerialNum.ToString();                       // Serial number
//                    BoardRevXmitter = memDevsResult.XmitterBoard.Revision;                                      // Revision
//                }
//            }
//        }

//        #endregion

//        #region Virtual Gnd Board Result

//        /// <summary>
//        /// Check the results for the Virtual Gnd board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsVirtualGnd(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardVirtualGnd)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardVirtualGnd = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_VIRTUAL_GND_BRD))
//                    {
//                        SysTestResult_BoardVirtualGnd = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardVirtualGnd != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardVirtualGnd = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_VIRTUAL_GND_BRD))
//                    {
//                        SysTestResult_BoardVirtualGnd = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardVirtualGnd != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardVirtualGnd = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;                // Cast object to get serial and rev

//                    BoardSerialVirtualGnd = memDevsResult.VirtualGndBoard.SerialNum.ToString();                     // Serial number
//                    BoardRevVirtualGnd = memDevsResult.VirtualGndBoard.Revision;                                    // Revision
//                }
//            }
//        }

//        #endregion

//        #region Rcvr Board Result

//        /// <summary>
//        /// Check the results for the Rcvr board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsRcvr(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardRcvr)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardRcvr = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_RCVR_BRD))
//                    {
//                        SysTestResult_BoardRcvr = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardRcvr != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardRcvr = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_RCVR_BRD))
//                    {
//                        SysTestResult_BoardRcvr = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardRcvr != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardRcvr = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;            // Cast object to get serial and rev

//                    BoardSerialRcvr = memDevsResult.RcvrBoard.SerialNum.ToString();                             // Serial number
//                    BoardRevRcvr = memDevsResult.RcvrBoard.Revision;                                            // Revision
//                }
//            }
//        }

//        #endregion

//        #region Back Plane Board Result

//        /// <summary>
//        /// Check the results for the Back Plane board result.  This will
//        /// check if the serial number and revision passed or failed.
//        /// If either failed, then the board failed.
//        /// </summary>
//        /// <param name="result">Results of the test for the board revision and serial number.</param>
//        private void SetResultsBackPlane(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_BoardBackPlane)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_BoardBackPlane = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set for serial number
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_SERIAL_BACKPLANE_BRD))
//                    {
//                        SysTestResult_BoardBackPlane = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Revision error code might set this to fail
//                        if (SysTestResult_BoardBackPlane != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardBackPlane = SysTestResultStatus.PASS;
//                        }
//                    }

//                    // Check if the error code is set for revision
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.INCORRECT_REV_BACKPLANE_BRD))
//                    {
//                        SysTestResult_BoardBackPlane = SysTestResultStatus.FAIL;
//                    }
//                    else
//                    {
//                        // If it is not currently set to fail, then set to pass
//                        // Serial error code might set this to fail
//                        if (SysTestResult_BoardBackPlane != SysTestResultStatus.FAIL)
//                        {
//                            SysTestResult_BoardBackPlane = SysTestResultStatus.PASS;
//                        }
//                    }
//                }

//                // Set the serial number and revision of the board
//                // Verify we can cast the object
//                if (result.Results is RTI.Commands.I2cMemDevs)
//                {
//                    RTI.Commands.I2cMemDevs memDevsResult = (RTI.Commands.I2cMemDevs)result.Results;            // Cast object to get serial and rev

//                    BoardSerialBackplane = memDevsResult.BackPlaneBoard.SerialNum.ToString();                   // Serial number
//                    BoardRevBackplane = memDevsResult.BackPlaneBoard.Revision;                                  // Revision
//                }

//            }
//        }

//        #endregion

//        #endregion

//        #region Firmware Files Test

//        /// <summary>
//        /// Run the ADCP firmware files test.
//        /// </summary>
//        private void SysTest_AdcpFirmwareFiles(AdcpSerialPort adcpSerialPort)
//        {
//            // Start test
//            if (_isSysTest_FirmwareFileHelp)
//            {
//                SysTestResult_FirmwareFileHelp = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_FirmwareFileEngHelp)
//            {
//                SysTestResult_FirmwareFileEngHelp = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_FirmwareFileMaint)
//            {
//                SysTestResult_FirmwareFileMaint = SysTestResultStatus.IN_PROGRESS;
//            }

//            if(_isSysTest_FirmwareFileRtisys)
//            {
//                SysTestResult_FirmwareFileRtisys = SysTestResultStatus.IN_PROGRESS;
//            }
            
//            if(_isSysTest_FirmwareFileBoot)
//            {
//                SysTestResult_FirmwareFileBoot = SysTestResultStatus.IN_PROGRESS;
//            }
            
//            if(_isSysTest_FirmwareFileSysconf)
//            {
//                SysTestResult_FirmwareFileSysconf = SysTestResultStatus.IN_PROGRESS;
//            }

//            if (_isSysTest_FirmwareFileBbcode)
//            {
//                SysTestResult_FirmwareFileBbcode = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Run test
//            SystemTestResult result = adcpSerialPort.SysTestFirmwareFiles();

//            // Check the results
//            SetResultsHelp(result);             // Help.txt
//            SetResultsEngHelp(result);          // EngHelp.txt
//            SetResultsMaint(result);            // maint.txt
//            SetResultsRtisys(result);           // RTISYS.bin
//            SetResultsBoot(result);             // BOOT.bin
//            SetResultsSysconf(result);          // SYSCONF.bin
//            SetResultsBbcode(result);           // BBCODE.bin
//        }

//        /// <summary>
//        /// Check the results if the Help.txt set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsHelp(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileHelp)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileHelp = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_HELP_MISSING))
//                    {
//                        SysTestResult_FirmwareFileHelp = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("HELP.TXT missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileHelp = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the EngHelp.txt set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsEngHelp(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileEngHelp)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileEngHelp = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_ENGHELP_MISSING))
//                    {
//                        SysTestResult_FirmwareFileEngHelp = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("ENGHELP.TXT missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileEngHelp = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the maint.txt set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsMaint(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileMaint)
//            {
//                // If all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileMaint = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_MAINT_MISSING))
//                    {
//                        SysTestResult_FirmwareFileMaint = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("MAINT.TXT missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileMaint = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the RTISYS.bin set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsRtisys(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileRtisys)
//            {
//                // IF all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileRtisys = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_RTISYS_MISSING))
//                    {
//                        SysTestResult_FirmwareFileRtisys = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("RTISYS.BIN missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileRtisys = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the BOOT.bin set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsBoot(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileBoot)
//            {
//                // IF all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileBoot = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_BOOT_MISSING))
//                    {
//                        SysTestResult_FirmwareFileBoot = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("BOOT.BIN missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileBoot = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the SYSCONF.bin set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsSysconf(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileSysconf)
//            {
//                // IF all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileSysconf = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_SYSCONF_MISSING))
//                    {
//                        SysTestResult_FirmwareFileSysconf = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("SYSCONF.BIN missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileSysconf = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Check the results if the BBCODE.bin set any errors.
//        /// </summary>
//        /// <param name="result">Result from the AdcpSerialPort::SysTestFirmwareFile().</param>
//        private void SetResultsBbcode(SystemTestResult result)
//        {
//            // If this test is going to be tested
//            // Set the results
//            if (_isSysTest_FirmwareFileBbcode)
//            {
//                // IF all pass, then this one passed
//                if (result.TestResult)
//                {
//                    SysTestResult_FirmwareFileBbcode = SysTestResultStatus.PASS;
//                }
//                else
//                {
//                    // Check if the error code is set
//                    if (result.ErrorCodes.Contains(SystemTestErrorCodes.FIRMWARE_BBCODE_MISSING))
//                    {
//                        SysTestResult_FirmwareFileBbcode = SysTestResultStatus.FAIL;
//                        SysTestErrorList.Add("BBCODE.BIN missing.");
//                    }
//                    else
//                    {
//                        SysTestResult_FirmwareFileBbcode = SysTestResultStatus.PASS;
//                    }
//                }
//            }
//        }

//        #endregion

//        #region Compass Test

//        ///// <summary>
//        ///// Test that the compass is outputing data.  If it only displaying
//        ///// 0 for the ENGPNI command, then the compass is not working.
//        ///// </summary>
//        ///// <param name="adcpSerialPort">ADCP to test.</param>
//        //private void SysTest_AdcpCompass(AdcpSerialPort adcpSerialPort)
//        //{
//        //    // Run Test
//        //    SystemTestResult result = adcpSerialPort.SysTestCompass();

//        //    // Check results
//        //    if (result.TestResult)
//        //    {
//        //        SysTestResult_AdcpCompass = SysTestResultStatus.PASS;
//        //    }
//        //    else
//        //    {
//        //        SysTestResult_AdcpCompass = SysTestResultStatus.FAIL;

//        //        // Add all the errors
//        //        foreach (string error in result.ErrorListStrings)
//        //        {
//        //            SysTestErrorList.Add(error);
//        //        }
//        //    }
//        //}

//        ///// <summary>
//        ///// Run a test to check the compass taps.
//        ///// This will connect the serial port to the compass.  It will then
//        ///// send a command to get the parameters from the compass.  It then disconnects 
//        ///// and waits for an event from the RecorderManager.  Subscribe to the RecorderManager
//        ///// for the compass data.  When it is received it will unsubscribe.
//        ///// </summary>
//        ///// <param name="adcpSerialPort">ADCP serial port.</param>
//        //private void SysTest_CompassTap(AdcpSerialPort adcpSerialPort)
//        //{
//        //    SysTestResult_CompassTap = SysTestResultStatus.IN_PROGRESS;

//        //    // Subscribe to receive compass responses
//        //    RecorderManager.Instance.CompassCodec.CompassEvent += new PniPrimeCompassBinaryCodec.CompassEventHandler(CompassCodecEventHandler);

//        //    adcpSerialPort.StartCompassMode();                                                          // Start compass mode
//        //    adcpSerialPort.SendCompassCommand(PniPrimeCompassBinaryCodec.GetParamCommand());            // Send command
//        //    adcpSerialPort.StopCompassMode();                                                           // Stop compass mode

//        //    // Wait for event to be received by CompassCodecEventHandler()
//        //    // Start timer for timeout
//        //    //_watchDog.Enabled = true;

//        //    // Let the process run and determine if the watchdog was set off
//        //    System.Threading.Thread.Sleep(WatchDogTimeout + 100);

//        //    //// Check the watch dog status
//        //    //if (_watchDogFail)
//        //    //{
//        //    //    // Reset the value
//        //    //    _watchDogFail = false;

//        //        // If we are still waiting for a result
//        //        // then the test did not complete and set as fail
//        //        if (SysTestResult_CompassTap == SysTestResultStatus.IN_PROGRESS)
//        //        {
//        //            SysTestResult_CompassTap = SysTestResultStatus.FAIL;
//        //            SysTestErrorList.Add(string.Format("Compass not communicating to check taps value."));
//        //        }
//        //    //}
//        //}

//        ///// <summary>
//        ///// Event handler for the compass codec.  This is called when compass data is received that was decoded.
//        ///// When data is received, unsubscribe from the event.
//        ///// </summary>
//        ///// <param name="data">Decoded compass data.</param>
//        //private void CompassCodecEventHandler(PniPrimeCompassBinaryCodec.CompassEventArgs data)
//        //{
//        //    //MessageBox.Show(string.Format("Compass data received: {0}", data.EventType));

//        //    // Compass Cal Parameters (Taps Count)
//        //    if (data.EventType == PniPrimeCompassBinaryCodec.ID.kParamResp)
//        //    {
//        //        //MessageBox.Show("Compass data received kParamResp");

//        //        // Disable the watchdog
//        //        _watchDog.Enabled = false;

//        //        PniPrimeCompassBinaryCodec.Parameters param = (PniPrimeCompassBinaryCodec.Parameters)data.Value;
//        //        if (CompassTaps != param.Count)
//        //        {
//        //            SysTestResult_CompassTap = SysTestResultStatus.FAIL;

//        //            SysTestErrorList.Add(string.Format("Compass Taps does not match.  Expected: {0}  Found: {1}", CompassTaps, param.Count));
//        //        }
//        //        else
//        //        {
//        //            SysTestResult_CompassTap = SysTestResultStatus.PASS;
//        //        }

//        //        // Unsubscribe
//        //        RecorderManager.Instance.CompassCodec.CompassEvent -= CompassCodecEventHandler;
//        //    }
//        //}

//        #endregion

//        #region RTC Time Test

//        /// <summary>
//        /// Test the RTC time.  Set the time, then verify the time is correctly returned from the
//        /// ADCP.  The time is test agaisnt the system time.
//        /// </summary>
//        /// <param name="adcpSerialPort">Serial port to run the test.</param>
//        private void SysTest_RtcTime(AdcpSerialPort adcpSerialPort)
//        {
//            if (_isSysTest_RtcTime)
//            {
//                SysTestResult_RtcTime = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Run Test
//            SystemTestResult result = adcpSerialPort.SysTestRtcTime();

//            // Get the serial number from the result
//            if (result.Results is DateTime)
//            {
//                DateTime sysDt = (DateTime)result.Results;
//                SysDateTime = sysDt;
//            }

//            // Check results
//            if (result.TestResult)
//            {
//                SysTestResult_RtcTime = SysTestResultStatus.PASS;
//            }
//            else
//            {
//                SysTestResult_RtcTime = SysTestResultStatus.FAIL;

//                // Add all the errors
//                foreach (string error in result.ErrorListStrings)
//                {
//                    SysTestErrorList.Add(error);
//                }
//            }
//        }

//        #endregion

//        #region ADCP Status Test

//        #region Water Profile

//        /// <summary>
//        /// Get the ADCP status from an ensemble.  This will ping the system and check the status.
//        /// To get the ensemble, it will subscribe to receive and ensemble event.  When the ensemble
//        /// is received it will be checked.
//        /// </summary>
//        /// <param name="adcpSerialPort">Serial port to run the test.</param>
//        private void SysTest_AdcpStatusWp(AdcpSerialPort adcpSerialPort)
//        {
//            if (_isSysTest_AdcpStatusWp)
//            {
//                SysTestResult_AdcpStatusWp = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Subscribe to receive response
//            _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Subscribe(On_ReceiveEnsembleWaterProfile, ThreadOption.PublisherThread, true, ReceiveEnsembleFilter);

//            // Setup the ADCP to ping, then ping it and wait for a response
//            adcpSerialPort.SysTestSingleWaterProfilePing();


//            // Let the process run, if after a period of time, the test 
//            // is still in progress, then fail the test
//            System.Threading.Thread.Sleep(WatchDogTimeout + 100);

//            if (SysTestResult_AdcpStatusWp == SysTestResultStatus.IN_PROGRESS)
//            {
//                // Unsubscribe
//                _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Unsubscribe(On_ReceiveEnsembleWaterProfile);

//                SysTestResult_AdcpStatusWp = SysTestResultStatus.FAIL;
//                SysTestErrorList.Add("Failed to receive a Water Profile Ensemble from the ADCP.");
//            }
//        }

//        /// <summary>
//        /// Receive the event when an ensemble is received.
//        /// When the ensemble is received, unsubscribe then check the
//        /// status in the Ensemble DataSet.
//        /// </summary>
//        /// <param name="ensemble">Ensemble received.</param>
//        private void On_ReceiveEnsembleWaterProfile(DataSet.Ensemble ensemble)
//        {
//            // Unsubscribe
//            _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Unsubscribe(On_ReceiveEnsembleWaterProfile);

//            // Check the data for status
//            // Check for Ensemble data
//            if (ensemble.IsEnsembleAvail)
//            {
//                // Get the status for the ensemble
//                Status status = ensemble.EnsembleData.Status;
//                AdcpStatusWpString = status.ToString();

//                // If it is good, value should be 0.
//                if (status.Value != 0)
//                {
//                    // Could not send a command to the ADCP
//                    SysTestResult_AdcpStatusWp = SysTestResultStatus.FAIL;
//                    SysTestErrorList.Add(status.ToString());
//                }
//                else
//                {
//                    SysTestResult_AdcpStatusWp = SysTestResultStatus.PASS;
//                }
//            }
//            else
//            {
//                // Could not send a command to the ADCP
//                SysTestResult_AdcpStatusWp = SysTestResultStatus.FAIL;
//                SysTestErrorList.Add("Failed to get a complete ensemble for Water Profile Status.");
//            }
//        }

//        #endregion

//        #region Bottom Track

//        /// <summary>
//        /// Get the ADCP status from an ensemble.  This will ping the system and check the status.
//        /// To get the ensemble, it will subscribe to receive and ensemble event.  When the ensemble
//        /// is received it will be checked.
//        /// </summary>
//        /// <param name="adcpSerialPort">Serial port to run the test.</param>
//        private void SysTest_AdcpStatusBt(AdcpSerialPort adcpSerialPort)
//        {
//            if (_isSysTest_AdcpStatusBt)
//            {
//                SysTestResult_AdcpStatusBt = SysTestResultStatus.IN_PROGRESS;
//            }

//            // Subscribe to receive response
//            _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Subscribe(On_ReceiveEnsembleBottomTrack, ThreadOption.PublisherThread, true, ReceiveEnsembleFilter);

//            // Setup the ADCP to ping, then ping it and wait for a response
//            adcpSerialPort.SysTestSingleBottomTrackPing();

//            // Let the process run, if after a period of time, the test 
//            // is still in progress, then fail the test
//            System.Threading.Thread.Sleep(WatchDogTimeout + 100);

//            if (SysTestResult_AdcpStatusBt == SysTestResultStatus.IN_PROGRESS)
//            {
//                // Unsubscribe
//                _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Unsubscribe(On_ReceiveEnsembleWaterProfile);

//                SysTestResult_AdcpStatusBt = SysTestResultStatus.FAIL;
//                SysTestErrorList.Add("Failed to receive a Bottom Track Ensemble from the ADCP.");
//            }
//        }

//        /// <summary>
//        /// Receive the event when an ensemble is received.
//        /// When the ensemble is received, unsubscribe then check the
//        /// status in the Ensemble DataSet.
//        /// </summary>
//        /// <param name="ensemble">Ensemble received.</param>
//        private void On_ReceiveEnsembleBottomTrack(DataSet.Ensemble ensemble)
//        {
//            // Unsubscribe
//            _eventAggregator.GetEvent<DisplayRawEnsembleEvent>().Unsubscribe(On_ReceiveEnsembleBottomTrack);

//            // Check the data for status
//            // Check for Ensemble data
//            if (ensemble.IsBottomTrackAvail)
//            {
//                // Get the status for the ensemble
//                Status status = ensemble.BottomTrackData.Status;
//                AdcpStatusBtString = status.ToString();

//                // Look for non pinging errors
//                // The test could be run on the bench and not in water where bottom track will work
//                if (status.IsReceiverDataError() || status.IsReceiverTimeout() || status.IsRealTimeClockError() || status.IsTemperatureError())
//                {
//                    // Could not send a command to the ADCP
//                    SysTestResult_AdcpStatusBt = SysTestResultStatus.FAIL;
//                    SysTestErrorList.Add(status.ToString());
//                }
//                else
//                {
//                    SysTestResult_AdcpStatusBt = SysTestResultStatus.PASS;
//                }
//            }
//            else
//            {
//                // Could not send a command to the ADCP
//                SysTestResult_AdcpStatusBt = SysTestResultStatus.FAIL;
//                SysTestErrorList.Add("Failed to get a complete ensemble for Bottom Track Status.");
//            }
//        }

//        #endregion

//        /// <summary>
//        /// Always return true;
//        /// </summary>
//        /// <param name="ensemble">Ensemble received.</param>
//        /// <returns>TRUE</returns>
//        private bool ReceiveEnsembleFilter(DataSet.Ensemble ensemble)
//        {
//            return true;
//        }

//        #endregion

//        #region WatchDog

//        /// <summary>
//        /// Watchdog timer has gone off.  Set the fail flag.
//        /// </summary>
//        /// <param name="sender">Not used.</param>
//        /// <param name="e">Not used.</param>
//        private void _watchDog_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
//        {
//            //_watchDogFail = true;           // Set flag of a failed test
//            _watchDog.Enabled = false;      // Turn off watchdog
//        }

//        #endregion

//        #region Utils

//        /// <summary>
//        /// If the system test result is true, give the
//        /// pass image.  If not give the fail image.
//        /// </summary>
//        /// <param name="status">Test status.</param>
//        /// <returns>Image based off status.</returns>
//        private string SysTestResultImage(SysTestResultStatus status)
//        {
//            switch (status)
//            {
//                case SysTestResultStatus.PASS:
//                    return "../Images/test_pass.png";
//                case SysTestResultStatus.FAIL:
//                    return "../Images/test_fail.png";
//                case SysTestResultStatus.IN_PROGRESS:
//                    return "../Images/test_progress.png";
//                case SysTestResultStatus.NOT_TESTING:
//                    return "../Images/testnottesting.png";
//                case SysTestResultStatus.NOT_STARTED:
//                    return "../Images/test_notstarted.png";
//                default:
//                    return "";  // no image
//            }
//        }

//        #endregion

//    }
//}
