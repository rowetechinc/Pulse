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
// * 07/26/2012      RC          2.13       Initial coding
// * 07/30/2012      RC          2.13       Removed BatteryWattHr from User input and replaced with battery type.
// * 09/07/2012      RC          2.15       Update test to Adcp Manual Revision G.
// * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
// * 
// * 
// */

//namespace RTI
//{
//    using NUnit.Framework;

//    /// <summary>
//    /// Test the user input for the Adcp Predictor.
//    /// </summary>
//    [TestFixture]
//    public class AdcpPredictorUserInputTest
//    {

//        #region Defaults

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// Default values.
//        /// </summary>
//        [Test]
//        public void TestDefaults()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region Fudge

//        /// <summary>
//        /// Modify Fudge value.
//        /// </summary>
//        [Test]
//        public void TestFudge()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.NbFudge = 2.0;

//            Assert.AreEqual(2.0, input.NbFudge, "Fudge is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Intervale is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region Deployment Duration

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// Deployment duration
//        /// </summary>
//        [Test]
//        public void TestDeploymentDuration()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.DeploymentDuration = 250;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(250, input.DeploymentDuration, "Deployment Duration is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Intervale is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CEI

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CEI.
//        /// </summary>
//        [Test]
//        public void TestCEI()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CEI = 17;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(17, input.CEI, "Ensemble Interval is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "CEI is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPON

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPON
//        /// </summary>
//        [Test]
//        public void TestCWPON()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPON = false;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(false, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPTBP

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPTBP
//        /// </summary>
//        [Test]
//        public void TestCWPTBP()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPTBP = 2.456f;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
            
//            Assert.AreEqual(2.456f, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPBN

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPBN
//        /// </summary>
//        [Test]
//        public void TestCWPBN()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPBN = 45;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");

//            Assert.AreEqual(45, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPBS

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPBS
//        /// </summary>
//        [Test]
//        public void TestCWPBS()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPBS = 3.456f;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");

//            Assert.AreEqual(3.456f, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
            
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region WPLagLength

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// WPLagLength
//        /// </summary>
//        [Test]
//        public void TestWPLagLength()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPBB_LagLength = 4.4567;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");

//            Assert.AreEqual(4.4567, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPBB

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPBB Transmit Pulse Type NarrowBand
//        /// </summary>
//        [Test]
//        public void TestCWPBB_TransmitPulseTypeNarrowband()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPBB_TransmitPulseType = RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");

//            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPBB Transmit Pulse Type NarrowBand
//        /// </summary>
//        [Test]
//        public void TestCWPBB_TransmitPulseTypeBroadband()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPBB_TransmitPulseType = RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");

//            Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreNotEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreNotEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND_PULSE_TO_PULSE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreNotEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NONCODED_PULSE_TO_PULSE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CWPP

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CWPP
//        /// </summary>
//        [Test]
//        public void TestCWPP()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CWPP = 76;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");

//            Assert.AreEqual(76, input.CWPP, "Water Profile pings per ensemble is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CBTON

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CBTON
//        /// </summary>
//        [Test]
//        public void TestCBTON()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CBTON = false;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(false, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CBTTBP

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CBTTBP
//        /// </summary>
//        [Test]
//        public void TestCBTTBP()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CBTTBP = 4.4567f;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");

//            Assert.AreEqual(4.4567f, input.CBTTBP, "Bottom Track time between pings is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region BatteryWattHr

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// BatteryWattHr
//        /// </summary>
//        [Test]
//        public void TestBatteryWattHr()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.BatteryType = DeploymentOptions.AdcpBatteryType.Lithium_7DD;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Lithium_7DD, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreNotEqual(DeploymentOptions.AdcpBatteryType.Alkaline_21D, input.BatteryType, "Battery Watt hr is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region BatteryDerate

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// BatteryDerate
//        /// </summary>
//        [Test]
//        public void TestBatteryDerate()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.BatteryDerate = 7.4563;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");

//            Assert.AreEqual(7.4563, input.BatteryDerate, "Battery derate is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SystemFrequency

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SystemFrequency
//        /// </summary>
//        [Test]
//        public void TestSystemFrequency()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SystemFrequency = 61111.123;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(61111.123, input.SystemFrequency, 0.01, "System frequency is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SpeedOfSound

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SpeedOfSound
//        /// </summary>
//        [Test]
//        public void TestSpeedOfSound()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SpeedOfSound = 123.456;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");

//            Assert.AreEqual(123.456, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
            
            
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region BeamAngle

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// BeamAngle
//        /// </summary>
//        [Test]
//        public void TestBeamAngle()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.BeamAngle = 42.123;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");

//            Assert.AreEqual(42.123, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");


//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region CyclesPerElement

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// CyclesPerElement
//        /// </summary>
//        [Test]
//        public void TestCyclesPerElement()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CyclesPerElement = 777;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");

//            Assert.AreEqual(777, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region Beta

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// Beta
//        /// </summary>
//        [Test]
//        public void TestBeta()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.Beta = 123.4567;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");

//            Assert.AreEqual(123.4567, input.Beta, "Beta is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SNR

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SNR
//        /// </summary>
//        [Test]
//        public void TestSNR()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SNR = 789.456;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");

//            Assert.AreEqual(789.456, input.SNR, "SNR is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region Beams

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// Beams
//        /// </summary>
//        [Test]
//        public void TestBeams()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.Beams = 8;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");

//            Assert.AreEqual(8, input.Beams, "Beams is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SystemOnPower

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SystemOnPower
//        /// </summary>
//        [Test]
//        public void TestSystemOnPower()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SystemOnPower = 456.123;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(456.123, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SystemSleepPower

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SystemSleepPower
//        /// </summary>
//        [Test]
//        public void TestSystemSleepPower()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SystemSleepPower = 123.456;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");

//            Assert.AreEqual(123.456, input.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion

//        #region SystemWakeup

//        /// <summary>
//        /// Test the constructor the Adcp Predictor input object.
//        /// SystemWakeup
//        /// </summary>
//        [Test]
//        public void TestSystemWakeup()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.SystemWakeup = 56.56;

//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_FUDGE, input.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, input.DeploymentDuration, "Deployment Duration is incorrect.");

//            // Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, input.CEI, "Ensemble Interval is incorrect.");

//            // WP Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPON, input.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPTBP, input.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBN, input.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBS, input.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_WP_LAG_LENGTH, input.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPBB_TRANSMIT_PULSE_TYPE, input.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CWPP, input.CWPP, "Water Profile pings per ensemble is incorrect.");

//            // BT Commands
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTON, input.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CBTTBP, input.CBTTBP, "Bottom Track time between pings is incorrect.");

//            // Batteries
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_TYPE, input.BatteryType, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BATTERY_DERATE, input.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_FREQ, input.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SPEED_OF_SOUND, input.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAM_ANGLE, input.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CYCLES_PER_ELEMENT, input.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BETA, input.Beta, "Beta is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SNR, input.SNR, "SNR is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_BEAMS, input.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_ON_PWR, input.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_SYS_SLEEP_PWR, input.SystemSleepPower, "System sleep power is incorrect.");

//            Assert.AreEqual(56.56, input.SystemWakeup, "System wakeup time is incorrect.");
//            Assert.AreNotEqual(AdcpPredictorUserInput.DEFAULT_SYS_WAKEUP, input.SystemWakeup, "System wakeup time is incorrect.");
//        }

//        #endregion
//    }
//}
