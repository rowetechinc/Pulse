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
// * 09/06/2012      RC          2.15       Added other Deployment senarios to test the settings.
// * 09/07/2012      RC          2.15       Update test to Adcp Manual Revision G.
// * 10/08/2012      RC          2.15       Update test to Predictor Rev E.
// * 12/27/2012      RC          2.17       Replaced Subsystem.Empty with Subsystem.IsEmpty().
// * 01/02/2013      RC          2.17       Changed DEFAULT_RANGE_600000 from 65 to 50 and DEFAULT_RANGE_300000 from 155 to 125 per Steve Maier.
// * 
// */

//namespace RTI
//{
//    using NUnit.Framework;

//    /// <summary>
//    /// Test the calculations for the AdcpPredictor object.
//    /// This object calculates the battery usage, ping characteristics
//    /// and memory usage based off the settings set in the ADCP.
//    /// 
//    /// Calculations based off spreadsheet "adcp predictor revE".
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class AdcpPredictorTest
//    {
//        ///// <summary>
//        ///// Test the predictor with the default input.
//        ///// </summary>
//        //[Test]
//        //public void DefaultInput()
//        //{
//        //    Subsystem ss = new Subsystem();
//        //    AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//        //    AdcpPredictor pred = new AdcpPredictor(input);

//        //    Assert.AreEqual(input.NbFudge, pred.NbFudge, "Fudge is incorrect.");

//        //    // Deployment
//        //    Assert.AreEqual(input.DeploymentDuration, pred.DeploymentDuration, "Deployment Duration is incorrect.");

//        //    // Commands
//        //    Assert.AreEqual(input.CEI, pred.CEI, "Ensemble Interval is incorrect.");

//        //    // WP Commands
//        //    Assert.AreEqual(input.CWPON, pred.CWPON, "Water Profile On is incorrect.");
//        //    Assert.AreEqual(input.CWPTBP, pred.CWPTBP, "Water Profile Time between ping is incorrect.");
//        //    Assert.AreEqual(input.CWPBN, pred.CWPBN, "Water Profile Number of bins is incorrect.");
//        //    Assert.AreEqual(input.CWPBS, pred.CWPBS, "Water Profile bin size is incorrect.");
//        //    Assert.AreEqual(input.CWPBB_LagLength, pred.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//        //    Assert.AreEqual(input.CWPBB_TransmitPulseType, pred.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//        //    Assert.AreEqual(input.CWPP, pred.CWPP, "Water Profile pings per ensemble is incorrect.");

//        //    // BT Commands
//        //    Assert.AreEqual(input.CBTON, pred.CBTON, "Bottom Track on is incorrect.");
//        //    Assert.AreEqual(input.CBTTBP, pred.CBTTBP, "Bottom Track time between pings is incorrect.");

//        //    // Batteries
//        //    Assert.AreEqual(input.BatteryType, pred.BatteryType, "Battery type is incorrect.");
//        //    Assert.AreEqual((int)input.BatteryType, pred.BatteryWattHr, "Battery Watt hr is incorrect.");
//        //    Assert.AreEqual(input.BatteryDerate, pred.BatteryDerate, "Battery derate is incorrect.");

//        //    // XDCR
//        //    Assert.AreEqual(input.SystemFrequency, pred.SystemFrequency, "System frequency is incorrect.");
//        //    Assert.AreEqual(input.SpeedOfSound, pred.SpeedOfSound, "Speed of sound is incorrect.");
//        //    Assert.AreEqual(input.BeamAngle, pred.BeamAngle, "Beam angle is incorrect.");
//        //    Assert.AreEqual(input.CyclesPerElement, pred.CyclesPerElement, "Cycles per element is incorrect.");
//        //    Assert.AreEqual(input.Beta, pred.Beta, "Beta is incorrect.");
//        //    Assert.AreEqual(input.SNR, pred.SNR, "SNR is incorrect.");
//        //    Assert.AreEqual(input.Beams, pred.Beams, "Beams is incorrect.");

//        //    // Power
//        //    Assert.AreEqual(input.SystemOnPower, pred.SystemOnPower, "System on power is incorrect.");
//        //    Assert.AreEqual(input.SystemSleepPower, pred.SystemSleepPower, "System sleep power is incorrect.");
//        //    Assert.AreEqual(input.SystemWakeup, pred.SystemWakeup, "System wakeup time is incorrect.");
//        //}

//        ///// <summary>
//        ///// Test the predictor with the default input.
//        ///// Check all the calculated results based off the
//        ///// default input.
//        ///// 
//        ///// Default frequency is a 300kHz.
//        ///// 
//        ///// Calculations based off spreadsheet "adcp predictor revE".
//        ///// </summary>
//        //[Test]
//        //public void DefaultCalculation()
//        //{
//        //    Subsystem ss = new Subsystem();
//        //    AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//        //    AdcpPredictor pred = new AdcpPredictor(input);

//        //    #region User Input

//        //    Assert.AreEqual(input.NbFudge, pred.NbFudge, "Fudge is incorrect.");

//        //    // Deployment
//        //    Assert.AreEqual(input.DeploymentDuration, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//        //    Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, pred.DeploymentDuration, "Deployment duration value is incorrect.");

//        //    // Commands
//        //    Assert.AreEqual(input.CEI, pred.CEI, "Ensemble Interval is incorrect.");
//        //    Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, pred.CEI, "Ensemble interval value is incorrect.");

//        //    // WP Commands
//        //    Assert.AreEqual(input.CWPON, pred.CWPON, "Water Profile On is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPON, pred.CWPON, "Water Profile On value is incorrect.");
//        //    Assert.AreEqual(input.CWPTBP, pred.CWPTBP, "Water Profile Time between ping is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPTBP, pred.CWPTBP, "Water Profile Time between pings value is incorrect.");
//        //    Assert.AreEqual(input.CWPBN, pred.CWPBN, "Water Profile Number of bins is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBN, pred.CWPBN, "Water Profile number of bins value is incorrect.");
//        //    Assert.AreEqual(input.CWPBS, pred.CWPBS, "Water Profile bin size is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBS, pred.CWPBS, "Water Profile Bin size value is incorrect.");
//        //    Assert.AreEqual(input.CWPBB_LagLength, pred.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//        //    Assert.AreEqual(input.CWPBB_TransmitPulseType, pred.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, pred.CWPBB_TransmitPulseType, "Water Profile Broadband value is incorrect.");
//        //    Assert.AreEqual(input.CWPP, pred.CWPP, "Water Profile pings per ensemble is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPP, pred.CWPP, "Water Profile pings per ensemble value is incorrect.");

//        //    // BT Commands
//        //    Assert.AreEqual(input.CBTON, pred.CBTON, "Bottom Track on is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CBTON, pred.CBTON, "Bottom Track on value is incorrect.");
//        //    Assert.AreEqual(input.CBTTBP, pred.CBTTBP, "Bottom Track time between pings is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTTBP, pred.CBTTBP, "Bottom Track Time between pings value is incorrect.");

//        //    // Batteries
//        //    Assert.AreEqual(input.BatteryType, pred.BatteryType, "Battery type is incorrect.");
//        //    Assert.AreEqual((int)input.BatteryType, pred.BatteryWattHr, "Battery Watt hr is incorrect.");
//        //    Assert.AreEqual(input.BatteryDerate, pred.BatteryDerate, "Battery derate is incorrect.");

//        //    // XDCR
//        //    Assert.AreEqual(input.SystemFrequency, pred.SystemFrequency, "System frequency is incorrect.");
//        //    Assert.AreEqual(input.SpeedOfSound, pred.SpeedOfSound, "Speed of sound is incorrect.");
//        //    Assert.AreEqual(input.BeamAngle, pred.BeamAngle, "Beam angle is incorrect.");
//        //    Assert.AreEqual(input.CyclesPerElement, pred.CyclesPerElement, "Cycles per element is incorrect.");
//        //    Assert.AreEqual(input.Beta, pred.Beta, "Beta is incorrect.");
//        //    Assert.AreEqual(input.SNR, pred.SNR, "SNR is incorrect.");
//        //    Assert.AreEqual(input.Beams, pred.Beams, "Beams is incorrect.");

//        //    // Power
//        //    Assert.AreEqual(input.SystemOnPower, pred.SystemOnPower, "System on power is incorrect.");
//        //    Assert.AreEqual(input.SystemSleepPower, pred.SystemSleepPower, "System sleep power is incorrect.");
//        //    Assert.AreEqual(input.SystemWakeup, pred.SystemWakeup, "System wakeup time is incorrect.");

//        //    #endregion

//        //    // Verify Input values
//        //    #region Verify Inputs
//        //    Assert.AreEqual(311281.25, input.SystemFrequency, 0.00001f, "Input System Frequency is incorrect.");
//        //    Assert.AreEqual(1, input.DeploymentDuration, "Input Deployment Duration is incorrect.");
//        //    Assert.AreEqual(1, input.CEI, 0.0001, "Input CEI is incorrect.");
//        //    Assert.AreEqual(true, input.CWPON, "Input CWPON is incorrect.");
//        //    Assert.AreEqual(true, input.CBTON, "Input CBTON is incorrect.");
//        //    Assert.AreEqual(0.25f, input.CBTTBP, 0.00001f, "Input CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.5f, input.CWPTBP, 0.00001f, "Input CWPTBP is incorrect.");
//        //    Assert.AreEqual(30, input.CWPBN, "Input CWPBN is incorrect.");
//        //    Assert.AreEqual(4.00f, input.CWPBS, 0.00001f, "Input CWPBS is incorrect.");
//        //    Assert.AreEqual(0.40f, input.CWPBL, 0.0001f, "Input CWPBL is incorrect.");
//        //    Assert.AreEqual(0.5f, input.CWPBB_LagLength, 0.00001f, "Input WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Input WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, input.CWPP, "Input CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, input.BatteryType, "Input Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, input.BatteryDerate, 0.000001f, "Input Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, input.SpeedOfSound, "Input Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, input.BeamAngle, "Input Beam Angle is incorrect.");

//        //    Assert.AreEqual(311281.25, pred.SystemFrequency, 0.00001f, "System Frequency is incorrect.");
//        //    Assert.AreEqual(1, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//        //    Assert.AreEqual(1, pred.CEI, 0.0001, "CEI is incorrect.");
//        //    Assert.AreEqual(true, pred.CWPON, "CWPON is incorrect.");
//        //    Assert.AreEqual(true, pred.CBTON, "CBTON is incorrect.");
//        //    Assert.AreEqual(0.25f, pred.CBTTBP, 0.00001f, "CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.5f, pred.CWPTBP, 0.00001f, "CWPTBP is incorrect.");
//        //    Assert.AreEqual(30, pred.CWPBN, "CWPBN is incorrect.");
//        //    Assert.AreEqual(4.00f, pred.CWPBS, 0.00001f, "CWPBS is incorrect.");
//        //    Assert.AreEqual(0.40f, pred.CWPBL, 0.0001f, "CWPBL is incorrect.");
//        //    Assert.AreEqual(0.5f, pred.CWPBB_LagLength, 0.00001f, "WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, pred.CWPP, "CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, pred.BatteryType, "Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, pred.BatteryDerate, 0.000001f, "Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, pred.SpeedOfSound, "Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, pred.BeamAngle, "Beam Angle is incorrect.");
//        //    #endregion

//        //    #region Verify Settings
//        //    Assert.AreEqual(12, pred.CyclesPerElement, "Cycles per element is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.Beta, 0.00001f, "Beta is incorrect.");
//        //    Assert.AreEqual(30.00f, pred.SNR, 0.00001f, "SNR is incorrect.");
//        //    Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//        //    Assert.AreEqual(4.50f, pred.SystemOnPower, 0.00001f, "System On Power is incorrect.");
//        //    Assert.AreEqual(0.00125, pred.SystemSleepPower, 0.0001f, "System Sleep Power is incorrect.");
//        //    Assert.AreEqual(1.0, pred.SystemWakeup, 0.00001f, "System Wakeup is incorrect.");
//        //    #endregion

//        //    #region Verify Tables
//        //    Assert.AreEqual(1100000, pred.Freq_1200000, "1200000 Freq is incorrect.");
//        //    Assert.AreEqual(11000, pred.uF_1200000, "1200000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_1200000, "1200000 Xmit V is incorrect.");
//        //    Assert.AreEqual(1, pred.Bin_1200000, "1200000 Bin is incorrect.");
//        //    Assert.AreEqual(18, pred.Range_1200000, "1200000 Range is incorrect.");
//        //    Assert.AreEqual(6.020600f, pred.dB_1200000, 0.00001f, "1200000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_1200000, 0.00001f, "1200000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_1200000, "1200000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_1200000, 0.00001f, "1200000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_1200000, 0.00001f, "1200000 Leakage is incorrect.");

//        //    Assert.AreEqual(550000, pred.Freq_600000, "600000 Freq is incorrect.");
//        //    Assert.AreEqual(22000, pred.uF_600000, "600000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_600000, "600000 Xmit V is incorrect.");
//        //    Assert.AreEqual(2, pred.Bin_600000, "600000 Bin is incorrect.");
//        //    Assert.AreEqual(50, pred.Range_600000, "600000 Range is incorrect.");
//        //    Assert.AreEqual(3.010300f, pred.dB_600000, 0.00001f, "600000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_600000, 0.00001f, "600000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_600000, "600000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_600000, 0.00001f, "600000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_600000, 0.00001f, "600000 Leakage is incorrect.");

//        //    Assert.AreEqual(275000, pred.Freq_300000, "300000 Freq is incorrect.");
//        //    Assert.AreEqual(44000, pred.uF_300000, "300000 uF is incorrect.");
//        //    Assert.AreEqual(24, pred.XmtV_300000, "300000 Xmit V is incorrect.");
//        //    Assert.AreEqual(4, pred.Bin_300000, "300000 Bin is incorrect.");
//        //    Assert.AreEqual(100, pred.Range_300000, "300000 Range is incorrect.");
//        //    Assert.AreEqual(0, pred.dB_300000, 0.00001f, "300000 dB is incorrect.");
//        //    Assert.AreEqual(100, pred.WpRange_300000, 0.00001f, "300000 WP Range is incorrect.");
//        //    Assert.AreEqual(20, pred.XmtW_300000, "300000 Xmit W is incorrect.");
//        //    Assert.AreEqual(320, pred.BtRange_300000, 0.00001f, "300000 BT Range is incorrect.");
//        //    Assert.AreEqual(4.359817f, pred.LeakageuA_300000, 0.00001f, "300000 Leakage is incorrect.");

//        //    Assert.AreEqual(137500, pred.Freq_150000, "150000 Freq is incorrect.");
//        //    Assert.AreEqual(16000, pred.uF_150000, "150000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_150000, "150000 Xmit V is incorrect.");
//        //    Assert.AreEqual(8, pred.Bin_150000, "150000 Bin is incorrect.");
//        //    Assert.AreEqual(250, pred.Range_150000, "150000 Range is incorrect.");
//        //    Assert.AreEqual(-3.010300f, pred.dB_150000, 0.00001f, "150000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_150000, 0.00001f, "150000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_150000, "150000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_150000, 0.00001f, "150000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_150000, 0.00001f, "150000 Leakage is incorrect.");
//        //    #endregion

//        //    #region Verify Beam Xmt Power
//        //    Assert.AreEqual(20.00f, pred.BeamTransmitPower, "Beam Transmit Power is incorrect.");
//        //    #endregion

//        //    #region Verify Time
//        //    Assert.AreEqual(86400, pred.BottomTrackPings, "Bottom Track Pings is incorrect.");
//        //    Assert.AreEqual(0.375, pred.BottomTrackTime, 0.00001f, "Bottom Track Time is incorrect.");
//        //    Assert.AreEqual(0.500000f, pred.TimeBetweenPings, 0.0001f, "Time Between Pings is incorrect.");
//        //    Assert.AreEqual(0.500000f, pred.ProfileTime, 0.00001f, "Profile Time is incorrect.");
//        //    Assert.AreEqual(0.000694f, pred.LagTime, 0.00001f, "Lag Time is incorrect.");
//        //    Assert.AreEqual(0.005705f, pred.BinTime, 0.00001f, "Bin Time is incorrect.");
//        //    Assert.AreEqual(0.500000f, pred.ReceiveTime, 0.00001f, "Receive Time is incorrect.");
//        //    Assert.AreEqual(0.006245f, pred.TransmitCodeTime, 0.00001f, "Transmit Code Time is incorrect.");
//        //    #endregion

//        //    #region Verify Power
//        //    Assert.AreEqual(144.0, pred.BtTransmitPower, 0.00001f, "Bottom Track Transmit Power is incorrect.");
//        //    Assert.AreEqual(40.5, pred.BtReceivePower, 0.00001f, "Bottom Track Receive Power is incorrect.");
//        //    Assert.AreEqual(11.9906997, pred.TransmitPower, 0.00001f, "Transmit Power is incorrect.");
//        //    Assert.AreEqual(54.0f, pred.ReceivePower, 0.00001f, "Receive Power is incorrect.");
//        //    Assert.AreEqual(0.03f, pred.SleepPower, 0.00001f, "Sleep Power is incorrect.");
//        //    Assert.AreEqual(0.001250f, pred.WakeupPower, 0.00001f, "Wakeup Power is incorrect.");
//        //    Assert.AreEqual(46.8004745, pred.CapChargePower, 0.00001f, "Cap Charge Power is incorrect.");
//        //    Assert.AreEqual(297.322424, pred.TotalPower, 0.00001f, "Total Power is incorrect.");
//        //    Assert.AreEqual(374, pred.ActualBatteryPower, 0.001f, "Battery Power is incorrect.");
//        //    Assert.AreEqual(0.7949797, pred.NumberBatteryPacks, 0.00001f, "Number of Battery Packs is incorrect.");
//        //    #endregion

//        //    #region Verify BB
//        //    Assert.AreEqual(25940.104167f, pred.SampleRate, 0.01f, "Sample Rate is incorrect.");
//        //    Assert.AreEqual(0.026988f, pred.MetersPerSample, 0.0001f, "Meters Per Sample is incorrect.");
//        //    Assert.AreEqual(148, pred.BinSamples, "Bin Samples is incorrect.");
//        //    Assert.AreEqual(18, pred.LagSamples, "Lag Samples is incorrect.");
//        //    Assert.AreEqual(9, pred.CodeRepeats, "Code Repeats is incorrect.");
//        //    Assert.AreEqual(0.888001f, pred.rho, 0.0001f, "rho is incorrect.");
//        //    Assert.AreEqual(720.558449f, pred.UaHz, 0.0001f, "Ua Hz is incorrect.");
//        //    Assert.AreEqual(1.724537f, pred.UaRadial, 0.0001f, "Ua Radial is incorrect.");
//        //    Assert.AreEqual(0.021734f, pred.StdDevRadial, 0.0001f, "Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.044933f, pred.StdDevSystem, 0.0001f, "Std Dev System is incorrect.");
//        //    #endregion

//        //    #region Verify NB
//        //    Assert.AreEqual(1.4f, pred.NbFudge, 0.0001f, "NB Fudge is incorrect.");
//        //    Assert.AreEqual(0.094999f, pred.NbStdDevRadial, 0.0001f, "NB Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.196405f, pred.NbStdDevHSystem, 0.0001f, "NB Std Dev System is incorrect.");
//        //    Assert.AreEqual(0.349066f, pred.BeamAngleRadian, 0.0001f, "NB Angle is incorrect.");
//        //    Assert.AreEqual(0.005714f, pred.NbTa, 0.0001f, "NB Ta is incorrect.");
//        //    Assert.AreEqual(175.017751f, pred.NbBn, 0.0001f, "NB Bn is incorrect.");
//        //    Assert.AreEqual(0.004787f, pred.NbLamda, 0.0001f, "NB Lamda is incorrect.");
//        //    Assert.AreEqual(4.256711f, pred.NbL, 0.0001f, "NB L is incorrect.");
//        //    #endregion

//        //    #region Verify Bytes
//        //    Assert.AreEqual(1, pred.Wakeups, "Wakeups is incorrect.");
//        //    Assert.AreEqual(86400, pred.NumEnsembles, "Num Ensembles is incorrect.");
//        //    Assert.AreEqual(4396, pred.EnsembleSizeBytes, "Ensemble Size is incorrect.");
//        //    Assert.AreEqual(112, pred.ProfileOverhead, "Profile Overhead is incorrect.");
//        //    Assert.AreEqual(3360, pred.BytesPerBin, "Bytes Per Bin is incorrect.");
//        //    Assert.AreEqual(384, pred.BytesBottomTrack, "Bottom Track Bytes is incorrect.");
//        //    Assert.AreEqual(504, pred.BytesOverhead, "Overhead Bytes is incorrect.");
//        //    Assert.AreEqual(4, pred.BytesChecksum, "Checksum bytes is incorrect.");
//        //    Assert.AreEqual(32, pred.BytesWrapper, "Wrapper Bytes is incorrect.");
//        //    Assert.AreEqual(0, pred.BytesNoPing, "No Ping Bytes is incorrect.");
//        //    #endregion

//        //    // Results
//        //    Assert.AreEqual(250, pred.PredictedBottomRange, 0.00001f, "Bottom Track Range is incorrect.");
//        //    Assert.AreEqual(125.0, pred.PredictedProfileRange, 0.00001f, "Profile range is incorrect.");
//        //    Assert.AreEqual(5.042209, pred.MaximumVelocity, 0.00001f, "Max Velocity is incorrect.");
//        //    Assert.AreEqual(0.044933, pred.StandardDeviation, 0.00001f, "Profile Standard Deviation is incorrect.");
//        //    Assert.AreEqual(6.593134538, pred.ProfileFirstBinPosition, 0.00001f, "Profile First Bin Position is incorrect.");
//        //    Assert.AreEqual(379.8144, pred.DataSizeBytes / 1000000.0, 0.00001f, "Ensemble Data Size is incorrect.");
//        //    Assert.AreEqual(0.7949797, pred.NumberBatteryPacks, 0.00001f, "Battery Required is incorrect.");
//        //}

//        /// <summary>
//        /// Turn off Bottom Track.
//        /// 
//        /// Calculations based off spreadsheet "adcp predictor revE".
//        /// </summary>
//        [Test]
//        public void BottomTrackOff()
//        {
//            Subsystem ss = new Subsystem();
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.CBTON = false;                                                            // TURN OFF BT
//            AdcpPredictor pred = new AdcpPredictor(input);

//            #region User Input

//            Assert.AreEqual(input.NbFudge, pred.NbFudge, "Fudge is incorrect.");

//            // Deployment
//            Assert.AreEqual(input.DeploymentDuration, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_DEPLOY_DUR, pred.DeploymentDuration, "Deployment duration value is incorrect.");

//            // Commands
//            Assert.AreEqual(input.CEI, pred.CEI, "Ensemble Interval is incorrect.");
//            Assert.AreEqual(AdcpPredictorUserInput.DEFAULT_CEI, pred.CEI, "Ensemble interval value is incorrect.");

//            // WP Commands
//            Assert.AreEqual(input.CWPON, pred.CWPON, "Water Profile On is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPON, pred.CWPON, "Water Profile On value is incorrect.");
//            Assert.AreEqual(input.CWPTBP, pred.CWPTBP, "Water Profile Time between ping is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPTBP, pred.CWPTBP, "Water Profile Time between pings value is incorrect.");
//            Assert.AreEqual(input.CWPBN, pred.CWPBN, "Water Profile Number of bins is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBN, pred.CWPBN, "Water Profile number of bins value is incorrect.");
//            Assert.AreEqual(input.CWPBS, pred.CWPBS, "Water Profile bin size is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPBS, pred.CWPBS, "Water Profile Bin size value is incorrect.");
//            Assert.AreEqual(input.CWPBB_LagLength, pred.CWPBB_LagLength, "Water Profile lag length is incorrect.");
//            Assert.AreEqual(input.CWPBB_TransmitPulseType, pred.CWPBB_TransmitPulseType, "Water Profile Broadband is on is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_CWPBB_TRANSMITPULSETYPE, pred.CWPBB_TransmitPulseType, "Water Profile Broadband value is incorrect.");
//            Assert.AreEqual(input.CWPP, pred.CWPP, "Water Profile pings per ensemble is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CWPP, pred.CWPP, "Water Profile pings per ensemble value is incorrect.");

//            // BT Commands
//            Assert.AreEqual(input.CBTON, pred.CBTON, "Bottom Track on is incorrect.");
//            Assert.AreEqual(false, pred.CBTON, "Bottom Track on value is incorrect.");
//            Assert.AreEqual(input.CBTTBP, pred.CBTTBP, "Bottom Track time between pings is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.DEFAULT_300_CBTTBP, pred.CBTTBP, "Bottom Track Time between pings value is incorrect.");

//            // Batteries
//            Assert.AreEqual(input.BatteryType, pred.BatteryType, "Battery type is incorrect.");
//            Assert.AreEqual((int)input.BatteryType, pred.BatteryWattHr, "Battery Watt hr is incorrect.");
//            Assert.AreEqual(input.BatteryDerate, pred.BatteryDerate, "Battery derate is incorrect.");

//            // XDCR
//            Assert.AreEqual(input.SystemFrequency, pred.SystemFrequency, "System frequency is incorrect.");
//            Assert.AreEqual(input.SpeedOfSound, pred.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(input.BeamAngle, pred.BeamAngle, "Beam angle is incorrect.");
//            Assert.AreEqual(input.CyclesPerElement, pred.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(input.Beta, pred.Beta, "Beta is incorrect.");
//            Assert.AreEqual(input.SNR, pred.SNR, "SNR is incorrect.");
//            Assert.AreEqual(input.Beams, pred.Beams, "Beams is incorrect.");

//            // Power
//            Assert.AreEqual(input.SystemOnPower, pred.SystemOnPower, "System on power is incorrect.");
//            Assert.AreEqual(input.SystemSleepPower, pred.SystemSleepPower, "System sleep power is incorrect.");
//            Assert.AreEqual(input.SystemWakeup, pred.SystemWakeup, "System wakeup time is incorrect.");

//            #endregion

//            #region Calculations
//            #region Verify Inputs
//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_300, input.SystemFrequency, 0.00001f, "Input System Frequency is incorrect.");
//            Assert.AreEqual(1, input.DeploymentDuration, "Input Deployment Duration is incorrect.");
//            Assert.AreEqual(1, input.CEI, 0.0001, "Input CEI is incorrect.");
//            Assert.AreEqual(true, input.CWPON, "Input CWPON is incorrect.");
//            Assert.AreEqual(false, input.CBTON, "Input CBTON is incorrect.");
//            Assert.AreEqual(0.25f, input.CBTTBP, 0.00001f, "Input CBTTBP is incorrect.");
//            Assert.AreEqual(0.5f, input.CWPTBP, 0.00001f, "Input CWPTBP is incorrect.");
//            Assert.AreEqual(30, input.CWPBN, "Input CWPBN is incorrect.");
//            Assert.AreEqual(4.00f, input.CWPBS, 0.00001f, "Input CWPBS is incorrect.");
//            Assert.AreEqual(0.40f, input.CWPBL, 0.0001f, "Input CWPBL is incorrect.");
//            Assert.AreEqual(0.5f, input.CWPBB_LagLength, 0.00001f, "Input WP Lag Length is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Input WP BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(1, input.CWPP, "Input CWPP is incorrect.");
//            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, input.BatteryType, "Input Battery Type is incorrect.");
//            Assert.AreEqual(0.85f, input.BatteryDerate, 0.000001f, "Input Battery Derate is incorrect.");
//            Assert.AreEqual(1490, input.SpeedOfSound, "Input Speed of sound is incorrect.");
//            Assert.AreEqual(20, input.BeamAngle, "Input Beam Angle is incorrect.");

//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_300, pred.SystemFrequency, 0.00001f, "System Frequency is incorrect.");
//            Assert.AreEqual(1, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//            Assert.AreEqual(1, pred.CEI, 0.0001, "CEI is incorrect.");
//            Assert.AreEqual(true, pred.CWPON, "CWPON is incorrect.");
//            Assert.AreEqual(false, pred.CBTON, "CBTON is incorrect.");
//            Assert.AreEqual(0.25f, pred.CBTTBP, 0.00001f, "CBTTBP is incorrect.");
//            Assert.AreEqual(0.5f, pred.CWPTBP, 0.00001f, "CWPTBP is incorrect.");
//            Assert.AreEqual(30, pred.CWPBN, "CWPBN is incorrect.");
//            Assert.AreEqual(4.00f, pred.CWPBS, 0.00001f, "CWPBS is incorrect.");
//            Assert.AreEqual(0.40f, pred.CWPBL, 0.0001f, "CWPBL is incorrect.");
//            Assert.AreEqual(0.5f, pred.CWPBB_LagLength, 0.00001f, "WP Lag Length is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "WP BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(1, pred.CWPP, "CWPP is incorrect.");
//            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, pred.BatteryType, "Battery Type is incorrect.");
//            Assert.AreEqual(0.85f, pred.BatteryDerate, 0.000001f, "Battery Derate is incorrect.");
//            Assert.AreEqual(1490, pred.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(20, pred.BeamAngle, "Beam Angle is incorrect.");
//            #endregion

//            #region Verify Settings
//            Assert.AreEqual(12, pred.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(1.00f, pred.Beta, 0.00001f, "Beta is incorrect.");
//            Assert.AreEqual(30.00f, pred.SNR, 0.00001f, "SNR is incorrect.");
//            Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//            Assert.AreEqual(4.50f, pred.SystemOnPower, 0.00001f, "System On Power is incorrect.");
//            Assert.AreEqual(0.00125, pred.SystemSleepPower, 0.0001f, "System Sleep Power is incorrect.");
//            Assert.AreEqual(1.0, pred.SystemWakeup, 0.00001f, "System Wakeup is incorrect.");
//            #endregion

//            #region Verify Tables
//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_1200, pred.Freq_1200000, "1200000 Freq is incorrect.");
//            Assert.AreEqual(11000, pred.uF_1200000, "1200000 uF is incorrect.");
//            Assert.AreEqual(0, pred.XmtV_1200000, "1200000 Xmit V is incorrect.");
//            Assert.AreEqual(1, pred.Bin_1200000, "1200000 Bin is incorrect.");
//            Assert.AreEqual(18, pred.Range_1200000, "1200000 Range is incorrect.");
//            Assert.AreEqual(9.542425f, pred.dB_1200000, 0.00001f, "1200000 dB is incorrect.");
//            Assert.AreEqual(0, pred.WpRange_1200000, 0.00001f, "1200000 WP Range is incorrect.");
//            Assert.AreEqual(0, pred.XmtW_1200000, "1200000 Xmit W is incorrect.");
//            Assert.AreEqual(0, pred.BtRange_1200000, 0.00001f, "1200000 BT Range is incorrect.");
//            Assert.AreEqual(0, pred.LeakageuA_1200000, 0.00001f, "1200000 Leakage is incorrect.");

//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_600, pred.Freq_600000, "600000 Freq is incorrect.");
//            Assert.AreEqual(22000, pred.uF_600000, "600000 uF is incorrect.");
//            Assert.AreEqual(0, pred.XmtV_600000, "600000 Xmit V is incorrect.");
//            Assert.AreEqual(2, pred.Bin_600000, "600000 Bin is incorrect.");
//            Assert.AreEqual(50, pred.Range_600000, "600000 Range is incorrect.");
//            Assert.AreEqual(3.010300f, pred.dB_600000, 0.00001f, "600000 dB is incorrect.");
//            Assert.AreEqual(0, pred.WpRange_600000, 0.00001f, "600000 WP Range is incorrect.");
//            Assert.AreEqual(0, pred.XmtW_600000, "600000 Xmit W is incorrect.");
//            Assert.AreEqual(0, pred.BtRange_600000, 0.00001f, "600000 BT Range is incorrect.");
//            Assert.AreEqual(0, pred.LeakageuA_600000, 0.00001f, "600000 Leakage is incorrect.");

//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_300, pred.Freq_300000, "300000 Freq is incorrect.");
//            Assert.AreEqual(44000, pred.uF_300000, "300000 uF is incorrect.");
//            Assert.AreEqual(24, pred.XmtV_300000, "300000 Xmit V is incorrect.");
//            Assert.AreEqual(4, pred.Bin_300000, "300000 Bin is incorrect.");
//            Assert.AreEqual(100, pred.Range_300000, "300000 Range is incorrect.");
//            Assert.AreEqual(0, pred.dB_300000, 0.00001f, "300000 dB is incorrect.");
//            Assert.AreEqual(100.0000, pred.WpRange_300000, 0.00001f, "300000 WP Range is incorrect.");
//            Assert.AreEqual(50, pred.XmtW_300000, "300000 Xmit W is incorrect.");
//            Assert.AreEqual(0, pred.BtRange_300000, 0.00001f, "300000 BT Range is incorrect.");
//            Assert.AreEqual(4.359817f, pred.LeakageuA_300000, 0.00001f, "300000 Leakage is incorrect.");

//            Assert.AreEqual(RTI.Core.Commons.FREQ_BASE / RTI.Core.Commons.FREQ_DIV_150, pred.Freq_150000, "150000 Freq is incorrect.");
//            Assert.AreEqual(16000, pred.uF_150000, "150000 uF is incorrect.");
//            Assert.AreEqual(0, pred.XmtV_150000, "150000 Xmit V is incorrect.");
//            Assert.AreEqual(8, pred.Bin_150000, "150000 Bin is incorrect.");
//            Assert.AreEqual(250, pred.Range_150000, "150000 Range is incorrect.");
//            Assert.AreEqual(-11.249387f, pred.dB_150000, 0.00001f, "150000 dB is incorrect.");
//            Assert.AreEqual(0, pred.WpRange_150000, 0.00001f, "150000 WP Range is incorrect.");
//            Assert.AreEqual(0, pred.XmtW_150000, "150000 Xmit W is incorrect.");
//            Assert.AreEqual(0, pred.BtRange_150000, 0.00001f, "150000 BT Range is incorrect.");
//            Assert.AreEqual(0, pred.LeakageuA_150000, 0.00001f, "150000 Leakage is incorrect.");
//            #endregion

//            #region Verify Beam Xmt Power
//            Assert.AreEqual(47.22222f, pred.BeamTransmitPower, 0.0001f, "Beam Transmit Power is incorrect.");
//            #endregion

//            #region Verify Time
//            Assert.AreEqual(0, pred.BottomTrackPings, "Bottom Track Pings is incorrect.");
//            Assert.AreEqual(0, pred.BottomTrackTime, 0.00001f, "Bottom Track Time is incorrect.");
//            Assert.AreEqual(0.500000f, pred.TimeBetweenPings, 0.0001f, "Time Between Pings is incorrect.");
//            Assert.AreEqual(0.500000f, pred.ProfileTime, 0.00001f, "Profile Time is incorrect.");
//            Assert.AreEqual(0.000694f, pred.LagTime, 0.00001f, "Lag Time is incorrect.");
//            Assert.AreEqual(0.005705f, pred.BinTime, 0.00001f, "Bin Time is incorrect.");
//            Assert.AreEqual(0.500000f, pred.ReceiveTime, 0.00001f, "Receive Time is incorrect.");
//            Assert.AreEqual(0.006245f, pred.TransmitCodeTime, 0.00001f, "Transmit Code Time is incorrect.");
//            #endregion

//            #region Verify Power
//            Assert.AreEqual(0, pred.BtTransmitPower, 0.00001f, "Bottom Track Transmit Power is incorrect.");
//            Assert.AreEqual(0, pred.BtReceivePower, 0.00001f, "Bottom Track Receive Power is incorrect.");
//            Assert.AreEqual(28.31137f, pred.TransmitPower, 0.00001f, "Transmit Power is incorrect.");
//            Assert.AreEqual(54.0f, pred.ReceivePower, 0.00001f, "Receive Power is incorrect.");
//            Assert.AreEqual(0.03f, pred.SleepPower, 0.00001f, "Sleep Power is incorrect.");
//            Assert.AreEqual(0.001250f, pred.WakeupPower, 0.00001f, "Wakeup Power is incorrect.");
//            Assert.AreEqual(8.4966769f, pred.CapChargePower, 0.00001f, "Cap Charge Power is incorrect.");
//            Assert.AreEqual(90.83930f, pred.TotalPower, 0.00001f, "Total Power is incorrect.");
//            Assert.AreEqual(374, pred.ActualBatteryPower, 0.001f, "Battery Power is incorrect.");
//            Assert.AreEqual(0.2428859f, pred.NumberBatteryPacks, 0.00001f, "Number of Battery Packs is incorrect.");
//            #endregion

//            #region Verify BB
//            Assert.AreEqual(25940.104167f, pred.SampleRate, 0.01f, "Sample Rate is incorrect.");
//            Assert.AreEqual(0.026988f, pred.MetersPerSample, 0.0001f, "Meters Per Sample is incorrect.");
//            Assert.AreEqual(148, pred.BinSamples, "Bin Samples is incorrect.");
//            Assert.AreEqual(18, pred.LagSamples, "Lag Samples is incorrect.");
//            Assert.AreEqual(9, pred.CodeRepeats, "Code Repeats is incorrect.");
//            Assert.AreEqual(0.888888f, pred.rho, 0.0001f, "rho is incorrect.");
//            Assert.AreEqual(720.558449f, pred.UaHz, 0.0001f, "Ua Hz is incorrect.");
//            Assert.AreEqual(1.724537f, pred.UaRadial, 0.0001f, "Ua Radial is incorrect.");
//            Assert.AreEqual(0.021734f, pred.StdDevRadial, 0.0001f, "Std Dev Radial is incorrect.");
//            Assert.AreEqual(0.044933f, pred.StdDevSystem, 0.0001f, "Std Dev System is incorrect.");
//            #endregion

//            #region Verify NB
//            Assert.AreEqual(1.4f, pred.NbFudge, 0.0001f, "NB Fudge is incorrect.");
//            Assert.AreEqual(0.094999f, pred.NbStdDevRadial, 0.0001f, "NB Std Dev Radial is incorrect.");
//            Assert.AreEqual(0.196405f, pred.NbStdDevHSystem, 0.0001f, "NB Std Dev System is incorrect.");
//            Assert.AreEqual(0.349066f, pred.BeamAngleRadian, 0.0001f, "NB Angle is incorrect.");
//            Assert.AreEqual(0.005714f, pred.NbTa, 0.0001f, "NB Ta is incorrect.");
//            Assert.AreEqual(175.017751f, pred.NbBn, 0.0001f, "NB Bn is incorrect.");
//            Assert.AreEqual(0.004787f, pred.NbLamda, 0.0001f, "NB Lamda is incorrect.");
//            Assert.AreEqual(4.256711f, pred.NbL, 0.0001f, "NB L is incorrect.");
//            #endregion

//            #region Verify Bytes
//            Assert.AreEqual(1, pred.Wakeups, "Wakeups is incorrect.");
//            Assert.AreEqual(86400, pred.NumEnsembles, "Num Ensembles is incorrect.");
//            Assert.AreEqual(4012, pred.EnsembleSizeBytes, "Ensemble Size is incorrect.");
//            Assert.AreEqual(112, pred.ProfileOverhead, "Profile Overhead is incorrect.");
//            Assert.AreEqual(3360, pred.BytesPerBin, "Bytes Per Bin is incorrect.");
//            Assert.AreEqual(0, pred.BytesBottomTrack, "Bottom Track Bytes is incorrect.");
//            Assert.AreEqual(504, pred.BytesOverhead, "Overhead Bytes is incorrect.");
//            Assert.AreEqual(4, pred.BytesChecksum, "Checksum bytes is incorrect.");
//            Assert.AreEqual(32, pred.BytesWrapper, "Wrapper Bytes is incorrect.");
//            Assert.AreEqual(0, pred.BytesNoPing, "No Ping Bytes is incorrect.");
//            #endregion

//            // Results
//            Assert.AreEqual(0, pred.PredictedBottomRange, 0.00001f, "Bottom Track Range is incorrect.");
//            Assert.AreEqual(100.0000, pred.PredictedProfileRange, 0.00001f, "Profile range is incorrect.");
//            Assert.AreEqual(5.042209, pred.MaximumVelocity, 0.00001f, "Max Velocity is incorrect.");
//            Assert.AreEqual(0.044843546, pred.StandardDeviation, 0.00001f, "Profile Standard Deviation is incorrect.");
//            Assert.AreEqual(6.5931345, pred.ProfileFirstBinPosition, 0.00001f, "Profile First Bin Position is incorrect.");
//            Assert.AreEqual(346.636800, pred.DataSizeBytes / 1000000.0, 0.00001f, "Ensemble Data Size is incorrect.");
//            Assert.AreEqual(0.2428859, pred.NumberBatteryPacks, 0.00001f, "Battery Required is incorrect.");

//            #endregion
//        }

//        /// <summary>
//        /// 1200 KHz, 4Beam 30 Degree system.
//        /// 
//        /// Calculations based off spreadsheet "adcp predictor revE".
//        /// </summary>
//        [Test]
//        public void Sys12004Beam30Deg()
//        {
//            // Create subsystem with 1200kHz, 4 beams 20 degree
//            Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 0);
//            AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//            input.DeploymentDuration = 365;
//            input.CEI = 3600;
//            input.CWPON = true;
//            input.CBTON = true;
//            input.CBTTBP = 1;
//            input.CBTBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
//            input.CWPTBP = 1;
//            input.CWPBN = 30;
//            input.CWPBS = 4;
//            input.CWPBL = 0.50f;
//            input.CWPBB_LagLength = 1;
//            input.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
//            input.CWPP = 30;
//            input.CyclesPerElement = 12;
//            input.BroadbandPower = true;
//            input.BatteryType = DeploymentOptions.AdcpBatteryType.Alkaline_38C;
//            input.BeamAngle = 30;
//            input.BeamDiameter = RTI.Core.Commons.CERAMIC_DIA_1200_2;
//            input.Beams = 4;

//            AdcpPredictor pred = new AdcpPredictor(input);

//            #region Verify Inputs
//            Assert.AreEqual(1245125, input.SystemFrequency, 0.00001f, "Input System Frequency is incorrect.");
//            Assert.AreEqual(365, input.DeploymentDuration, "Input Deployment Duration is incorrect.");
//            Assert.AreEqual(3600, input.CEI, 0.0001, "Input CEI is incorrect.");
//            Assert.AreEqual(true, input.CWPON, "Input CWPON is incorrect.");
//            Assert.AreEqual(true, input.CBTON, "Input CBTON is incorrect.");
//            Assert.AreEqual(1f, input.CBTTBP, 0.00001f, "Input CBTTBP is incorrect.");
//            Assert.AreEqual(1f, input.CWPTBP, 0.00001f, "Input CWPTBP is incorrect.");
//            Assert.AreEqual(30, input.CWPBN, "Input CWPBN is incorrect.");
//            Assert.AreEqual(4.00f, input.CWPBS, 0.00001f, "Input CWPBS is incorrect.");
//            Assert.AreEqual(0.5f, input.CWPBL, 0.0001f, "Input CWPBL is incorrect.");
//            Assert.AreEqual(1f, input.CWPBB_LagLength, 0.00001f, "Input WP Lag Length is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Input WP BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, input.CBTBB_TransmitPulseType, "Input BT BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(30, input.CWPP, "Input CWPP is incorrect.");
//            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, input.BatteryType, "Input Battery Type is incorrect.");
//            Assert.AreEqual(0.85f, input.BatteryDerate, 0.000001f, "Input Battery Derate is incorrect.");
//            Assert.AreEqual(1490, input.SpeedOfSound, "Input Speed of sound is incorrect.");
//            Assert.AreEqual(30, input.BeamAngle, "Input Beam Angle is incorrect.");
//            Assert.AreEqual(4, input.Beams, "Input Number of beams is incorrect.");

//            Assert.AreEqual(1245125, pred.SystemFrequency, 0.00001f, "System Frequency is incorrect.");
//            Assert.AreEqual(365, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//            Assert.AreEqual(3600, pred.CEI, 0.0001, "CEI is incorrect.");
//            Assert.AreEqual(true, pred.CWPON, "CWPON is incorrect.");
//            Assert.AreEqual(true, pred.CBTON, "CBTON is incorrect.");
//            Assert.AreEqual(1f, pred.CBTTBP, 0.00001f, "CBTTBP is incorrect.");
//            Assert.AreEqual(1f, pred.CWPTBP, 0.00001f, "CWPTBP is incorrect.");
//            Assert.AreEqual(30, pred.CWPBN, "CWPBN is incorrect.");
//            Assert.AreEqual(4.00f, pred.CWPBS, 0.00001f, "CWPBS is incorrect.");
//            Assert.AreEqual(0.5f, pred.CWPBL, 0.0001f, "CWPBL is incorrect.");
//            Assert.AreEqual(1f, pred.CWPBB_LagLength, 0.00001f, "WP Lag Length is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "WP BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE, input.CBTBB_TransmitPulseType, "BT BB Transmit Pulse Type is incorrect.");
//            Assert.AreEqual(30, pred.CWPP, "CWPP is incorrect.");
//            Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, pred.BatteryType, "Battery Type is incorrect.");
//            Assert.AreEqual(0.85f, pred.BatteryDerate, 0.000001f, "Battery Derate is incorrect.");
//            Assert.AreEqual(1490, pred.SpeedOfSound, "Speed of sound is incorrect.");
//            Assert.AreEqual(30, pred.BeamAngle, "Beam Angle is incorrect.");
//            Assert.AreEqual(4, input.Beams, "Number of beams is incorrect.");
//            #endregion

//            #region Verify Settings
//            Assert.AreEqual(12, pred.CyclesPerElement, "Cycles per element is incorrect.");
//            Assert.AreEqual(1.00f, pred.Beta, 0.00001f, "Beta is incorrect.");
//            Assert.AreEqual(30.00f, pred.SNR, 0.00001f, "SNR is incorrect.");
//            Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//            Assert.AreEqual(4.50f, pred.SystemOnPower, 0.00001f, "System On Power is incorrect.");
//            Assert.AreEqual(0.00125, pred.SystemSleepPower, 0.0001f, "System Sleep Power is incorrect.");
//            Assert.AreEqual(1.0, pred.SystemWakeup, 0.00001f, "System Wakeup is incorrect.");
//            #endregion

//            #region Verify Tables
//            Assert.AreEqual(1100000, pred.Freq_1200000, "1200000 Freq is incorrect.");
//            Assert.AreEqual(11000, pred.uF_1200000, "1200000 uF is incorrect.");
//            Assert.AreEqual(24, pred.XmtV_1200000, "1200000 Xmit V is incorrect.");
//            Assert.AreEqual(1, pred.Bin_1200000, "1200000 Bin is incorrect.");
//            Assert.AreEqual(18, pred.Range_1200000, "1200000 Range is incorrect.");
//            Assert.AreEqual(5, pred.XmtW_1200000, "1200000 Xmit W is incorrect.");
//            Assert.AreEqual(0.051, pred.BeamDiameter, 0.001f, "1200000 Beam Diameter is incorrect.");
//            Assert.AreEqual(0.08, pred.Sampling_1200000, 0.01f, "1200000 Sampling is incorrect.");
//            Assert.AreEqual(12, pred.CPE_1200000, 0.001f, "1200000, CPE is incorrect.");
//            Assert.AreEqual(0.00119667, pred.WaveLength, 0.0001f, "WaveLength is incorrect.");
//            Assert.AreEqual(42.5, pred.DI_1200000, 0.01f, "12000000 DI is incorrect.");
//            Assert.AreEqual(6.0, pred.dB_1200000, 0.1f, "1200000 dB is incorrect.");
//            Assert.AreEqual(30, pred.BeamAngle, 0.1f, "Beam Angle is incorrect.");
//            Assert.AreEqual(0.5236f, pred.BeamAngleRadian, 0.001f, "Beam Angle Radians is incorrect.");
//            Assert.AreEqual(0.92f, pred.rScale_1200000, 0.1f, "1200000 rScale is incorrect.");
//            Assert.AreEqual(22.1, pred.WpRange_1200000, 0.1f, "1200000 WP Range is incorrect.");
//            Assert.AreEqual(71.9, pred.BtRange_1200000, 0.1f, "1200000 BT Range is incorrect.");
//            Assert.AreEqual(24, pred.XmtV_1200000, "1200000, XmtV is incorrect.");
//            Assert.AreEqual(2.179908, pred.LeakageuA_1200000, 0.01f, "1200000 Leakage is incorrect.");

//            //Assert.AreEqual(600000, pred.Freq_600000, "600000 Freq is incorrect.");
//            //Assert.AreEqual(22000, pred.uF_600000, "600000 uF is incorrect.");
//            //Assert.AreEqual(0, pred.XmtV_600000, "600000 Xmit V is incorrect.");
//            //Assert.AreEqual(2, pred.Bin_600000, "600000 Bin is incorrect.");
//            //Assert.AreEqual(50, pred.Range_600000, "600000 Range is incorrect.");
//            //Assert.AreEqual(-3.010300f, pred.dB_600000, 0.00001f, "600000 dB is incorrect.");
//            //Assert.AreEqual(0, pred.WpRange_600000, 0.00001f, "600000 WP Range is incorrect.");
//            //Assert.AreEqual(0, pred.XmtW_600000, "600000 Xmit W is incorrect.");
//            //Assert.AreEqual(0, pred.BtRange_600000, 0.00001f, "600000 BT Range is incorrect.");
//            //Assert.AreEqual(0, pred.LeakageuA_600000, 0.00001f, "600000 Leakage is incorrect.");

//            //Assert.AreEqual(300000, pred.Freq_300000, "300000 Freq is incorrect.");
//            //Assert.AreEqual(44000, pred.uF_300000, "300000 uF is incorrect.");
//            //Assert.AreEqual(0, pred.XmtV_300000, "300000 Xmit V is incorrect.");
//            //Assert.AreEqual(4, pred.Bin_300000, "300000 Bin is incorrect.");
//            //Assert.AreEqual(125, pred.Range_300000, "300000 Range is incorrect.");
//            //Assert.AreEqual(-6.020600f, pred.dB_300000, 0.00001f, "300000 dB is incorrect.");
//            //Assert.AreEqual(0, pred.WpRange_300000, 0.00001f, "300000 WP Range is incorrect.");
//            //Assert.AreEqual(0, pred.XmtW_300000, "300000 Xmit W is incorrect.");
//            //Assert.AreEqual(0, pred.BtRange_300000, 0.00001f, "300000 BT Range is incorrect.");
//            //Assert.AreEqual(0, pred.LeakageuA_300000, 0.00001f, "300000 Leakage is incorrect.");

//            //Assert.AreEqual(150000, pred.Freq_150000, "150000 Freq is incorrect.");
//            //Assert.AreEqual(16000, pred.uF_150000, "150000 uF is incorrect.");
//            //Assert.AreEqual(0, pred.XmtV_150000, "150000 Xmit V is incorrect.");
//            //Assert.AreEqual(8, pred.Bin_150000, "150000 Bin is incorrect.");
//            //Assert.AreEqual(265, pred.Range_150000, "150000 Range is incorrect.");
//            //Assert.AreEqual(-9.030900f, pred.dB_150000, 0.00001f, "150000 dB is incorrect.");
//            //Assert.AreEqual(0, pred.WpRange_150000, 0.00001f, "150000 WP Range is incorrect.");
//            //Assert.AreEqual(0, pred.XmtW_150000, "150000 Xmit W is incorrect.");
//            //Assert.AreEqual(0, pred.BtRange_150000, 0.00001f, "150000 BT Range is incorrect.");
//            //Assert.AreEqual(0, pred.LeakageuA_150000, 0.00001f, "150000 Leakage is incorrect.");
//            #endregion

//            #region Verify Beam Xmt Power
//            Assert.AreEqual(4.97f, pred.BeamTransmitPower, 0.01f, "Beam Transmit Power is incorrect.");
//            #endregion

//            #region Verify Time
//            Assert.AreEqual(26280, pred.BottomTrackPings, "Bottom Track Pings is incorrect.");
//            Assert.AreEqual(0.1079f, pred.BottomTrackTime, 0.01f, "Bottom Track Time is incorrect.");
//            Assert.AreEqual(1.000f, pred.TimeBetweenPings, 0.01f, "Time Between Pings is incorrect.");
//            Assert.AreEqual(1.000f, pred.ProfileTime, 0.01f, "Profile Time is incorrect.");
//            Assert.AreEqual(0.0015f, pred.LagTime, 0.001f, "Lag Time is incorrect.");
//            Assert.AreEqual(0.0062f, pred.BinTime, 0.001f, "Bin Time is incorrect.");
//            Assert.AreEqual(1.0000f, pred.ReceiveTime, 0.001f, "Receive Time is incorrect.");
//            Assert.AreEqual(0.0077f, pred.TransmitCodeTime, 0.001f, "Transmit Code Time is incorrect.");
//            #endregion

//            #region Verify Power
//            Assert.AreEqual(31.30f, pred.BtTransmitPower, 0.1f, "Bottom Track Transmit Power is incorrect.");
//            Assert.AreEqual(70.88f, pred.BtReceivePower, 0.1f, "Bottom Track Receive Power is incorrect.");
//            Assert.AreEqual(11.18, pred.TransmitPower, 0.1f, "Transmit Power is incorrect.");
//            Assert.AreEqual(657.0f, pred.ReceivePower, 0.1f, "Receive Power is incorrect.");
//            Assert.AreEqual(10.95f, pred.SleepPower, 0.1f, "Sleep Power is incorrect.");
//            Assert.AreEqual(10.95f, pred.WakeupPower, 0.1f, "Wakeup Power is incorrect.");
//            Assert.AreEqual(13.34f, pred.CapChargePower, 0.1f, "Cap Charge Power is incorrect.");
//            Assert.AreEqual(805.58, pred.TotalPower, 0.1f, "Total Power is incorrect.");
//            //Assert.AreEqual(374, pred.ActualBatteryPower, 0.001f, "Battery Power is incorrect.");
//            Assert.AreEqual(2.1543, pred.NumberBatteryPacks, 0.1f, "Number of Battery Packs is incorrect.");
//            #endregion

//            #region Verify BB
//            Assert.AreEqual(103760.41, pred.SampleRate, 0.01f, "Sample Rate is incorrect.");
//            Assert.AreEqual(0.0062, pred.MetersPerSample, 0.1f, "Meters Per Sample is incorrect.");
//            Assert.AreEqual(643, pred.BinSamples, "Bin Samples is incorrect.");
//            Assert.AreEqual(160, pred.LagSamples, "Lag Samples is incorrect.");
//            Assert.AreEqual(5, pred.CodeRepeats, "Code Repeats is incorrect.");
//            Assert.AreEqual(0.80f, pred.rho, 0.01f, "rho is incorrect.");
//            Assert.AreEqual(324.25f, pred.UaHz, 0.1f, "Ua Hz is incorrect.");
//            Assert.AreEqual(0.194f, pred.UaRadial, 0.1f, "Ua Radial is incorrect.");
//            Assert.AreEqual(0.0014f, pred.StdDevRadial, 0.001f, "Std Dev Radial is incorrect.");
//            Assert.AreEqual(0.0004f, pred.StdDevSystem, 0.001f, "Std Dev System is incorrect.");
//            #endregion

//            #region Verify NB
//            Assert.AreEqual(1.4f, pred.NbFudge, 0.001f, "NB Fudge is incorrect.");
//            Assert.AreEqual(0.022f, pred.NbStdDevRadial, 0.01f, "NB Std Dev Radial is incorrect.");
//            Assert.AreEqual(0.006f, pred.NbStdDevHSystem, 0.01f, "NB Std Dev System is incorrect.");
//            //Assert.AreEqual(0.349066f, pred.BeamAngleRadian, 0.0001f, "NB Angle is incorrect.");
//            Assert.AreEqual(0.0062f, pred.NbTa, 0.01f, "NB Ta is incorrect.");
//            Assert.AreEqual(161.3f, pred.NbBn, 0.1f, "NB Bn is incorrect.");
//            Assert.AreEqual(0.0012f, pred.NbLamda, 0.01f, "NB Lamda is incorrect.");
//            Assert.AreEqual(4.6188f, pred.NbL, 0.01f, "NB L is incorrect.");
//            #endregion

//            #region Verify Bytes
//            Assert.AreEqual(8760, pred.Wakeups, "Wakeups is incorrect.");
//            Assert.AreEqual(8760, pred.NumEnsembles, "Num Ensembles is incorrect.");
//            Assert.AreEqual(4396, pred.EnsembleSizeBytes, "Ensemble Size is incorrect.");
//            Assert.AreEqual(112, pred.ProfileOverhead, "Profile Overhead is incorrect.");
//            Assert.AreEqual(3360, pred.BytesPerBin, "Bytes Per Bin is incorrect.");
//            Assert.AreEqual(384, pred.BytesBottomTrack, "Bottom Track Bytes is incorrect.");
//            Assert.AreEqual(504, pred.BytesOverhead, "Overhead Bytes is incorrect.");
//            Assert.AreEqual(4, pred.BytesChecksum, "Checksum bytes is incorrect.");
//            Assert.AreEqual(32, pred.BytesWrapper, "Wrapper Bytes is incorrect.");
//            Assert.AreEqual(0, pred.BytesNoPing, "No Ping Bytes is incorrect.");
//            #endregion

//            // Results
//            Assert.AreEqual(71.92, pred.PredictedBottomRange, 0.1f, "Bottom Track Range is incorrect.");
//            Assert.AreEqual(22.14, pred.PredictedProfileRange, 0.1f, "Profile range is incorrect.");
//            Assert.AreEqual(0.388, pred.MaximumVelocity, 0.1f, "Max Velocity is incorrect.");
//            Assert.AreEqual(0.0004, pred.StandardDeviation, 0.001f, "Profile Standard Deviation is incorrect.");
//            Assert.AreEqual(6.99, pred.ProfileFirstBinPosition, 0.001f, "Profile First Bin Position is incorrect.");
//            Assert.AreEqual(38.50896, pred.DataSizeBytes / 1000000.0, 0.001f, "Ensemble Data Size is incorrect.");
//            Assert.AreEqual(2.15, pred.NumberBatteryPacks, 0.1f, "Battery Required is incorrect.");
//        }


//        ///// <summary>
//        ///// 1200 KHz, 4Beam 20 Degree system.
//        ///// 
//        ///// Calculations based off spreadsheet "adcp predictor revE".
//        ///// </summary>
//        //[Test]
//        //public void Sys12004Beam20Deg()
//        //{
//        //    // Create subsystem with 1200kHz, 4 beams 20 degree
//        //    Subsystem ss = new Subsystem(Subsystem.SUB_1_2MHZ_4BEAM_20DEG_PISTON_2, 0);
//        //    AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);

//        //    AdcpPredictor pred = new AdcpPredictor(input);

//        //    #region Verify Inputs
//        //    Assert.AreEqual(1245125, input.SystemFrequency, 0.00001f, "Input System Frequency is incorrect.");
//        //    Assert.AreEqual(1, input.DeploymentDuration, "Input Deployment Duration is incorrect.");
//        //    Assert.AreEqual(1, input.CEI, 0.0001, "Input CEI is incorrect.");
//        //    Assert.AreEqual(true, input.CWPON, "Input CWPON is incorrect.");
//        //    Assert.AreEqual(true, input.CBTON, "Input CBTON is incorrect.");
//        //    Assert.AreEqual(0.05f, input.CBTTBP, 0.00001f, "Input CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.1f, input.CWPTBP, 0.00001f, "Input CWPTBP is incorrect.");
//        //    Assert.AreEqual(20, input.CWPBN, "Input CWPBN is incorrect.");
//        //    Assert.AreEqual(1.00f, input.CWPBS, 0.00001f, "Input CWPBS is incorrect.");
//        //    Assert.AreEqual(1.0f, input.CWPBL, 0.0001f, "Input CWPBL is incorrect.");
//        //    Assert.AreEqual(0.5f, input.CWPBB_LagLength, 0.00001f, "Input WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Input WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, input.CWPP, "Input CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, input.BatteryType, "Input Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, input.BatteryDerate, 0.000001f, "Input Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, input.SpeedOfSound, "Input Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, input.BeamAngle, "Input Beam Angle is incorrect.");

//        //    Assert.AreEqual(1245125, pred.SystemFrequency, 0.00001f, "System Frequency is incorrect.");
//        //    Assert.AreEqual(1, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//        //    Assert.AreEqual(1, pred.CEI, 0.0001, "CEI is incorrect.");
//        //    Assert.AreEqual(true, pred.CWPON, "CWPON is incorrect.");
//        //    Assert.AreEqual(true, pred.CBTON, "CBTON is incorrect.");
//        //    Assert.AreEqual(0.05f, pred.CBTTBP, 0.00001f, "CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.1f, pred.CWPTBP, 0.00001f, "CWPTBP is incorrect.");
//        //    Assert.AreEqual(20, pred.CWPBN, "CWPBN is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.CWPBS, 0.00001f, "CWPBS is incorrect.");
//        //    Assert.AreEqual(1.0f, pred.CWPBL, 0.0001f, "CWPBL is incorrect.");
//        //    Assert.AreEqual(0.5f, pred.CWPBB_LagLength, 0.00001f, "WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, pred.CWPP, "CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, pred.BatteryType, "Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, pred.BatteryDerate, 0.000001f, "Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, pred.SpeedOfSound, "Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, pred.BeamAngle, "Beam Angle is incorrect.");
//        //    #endregion

//        //    #region Verify Settings
//        //    Assert.AreEqual(12, pred.CyclesPerElement, "Cycles per element is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.Beta, 0.00001f, "Beta is incorrect.");
//        //    Assert.AreEqual(30.00f, pred.SNR, 0.00001f, "SNR is incorrect.");
//        //    Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//        //    Assert.AreEqual(4.50f, pred.SystemOnPower, 0.00001f, "System On Power is incorrect.");
//        //    Assert.AreEqual(0.00125, pred.SystemSleepPower, 0.0001f, "System Sleep Power is incorrect.");
//        //    Assert.AreEqual(1.0, pred.SystemWakeup, 0.00001f, "System Wakeup is incorrect.");
//        //    #endregion

//        //    #region Verify Tables
//        //    Assert.AreEqual(1200000, pred.Freq_1200000, "1200000 Freq is incorrect.");
//        //    Assert.AreEqual(11000, pred.uF_1200000, "1200000 uF is incorrect.");
//        //    Assert.AreEqual(24, pred.XmtV_1200000, "1200000 Xmit V is incorrect.");
//        //    Assert.AreEqual(1, pred.Bin_1200000, "1200000 Bin is incorrect.");
//        //    Assert.AreEqual(20, pred.Range_1200000, "1200000 Range is incorrect.");
//        //    Assert.AreEqual(0.0, pred.dB_1200000, 0.00001f, "1200000 dB is incorrect.");
//        //    Assert.AreEqual(20, pred.WpRange_1200000, 0.00001f, "1200000 WP Range is incorrect.");
//        //    Assert.AreEqual(5, pred.XmtW_1200000, "1200000 Xmit W is incorrect.");
//        //    Assert.AreEqual(40, pred.BtRange_1200000, 0.00001f, "1200000 BT Range is incorrect.");
//        //    Assert.AreEqual(2.179908, pred.LeakageuA_1200000, 0.00001f, "1200000 Leakage is incorrect.");

//        //    Assert.AreEqual(600000, pred.Freq_600000, "600000 Freq is incorrect.");
//        //    Assert.AreEqual(22000, pred.uF_600000, "600000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_600000, "600000 Xmit V is incorrect.");
//        //    Assert.AreEqual(2, pred.Bin_600000, "600000 Bin is incorrect.");
//        //    Assert.AreEqual(50, pred.Range_600000, "600000 Range is incorrect.");
//        //    Assert.AreEqual(-3.010300f, pred.dB_600000, 0.00001f, "600000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_600000, 0.00001f, "600000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_600000, "600000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_600000, 0.00001f, "600000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_600000, 0.00001f, "600000 Leakage is incorrect.");

//        //    Assert.AreEqual(300000, pred.Freq_300000, "300000 Freq is incorrect.");
//        //    Assert.AreEqual(44000, pred.uF_300000, "300000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_300000, "300000 Xmit V is incorrect.");
//        //    Assert.AreEqual(4, pred.Bin_300000, "300000 Bin is incorrect.");
//        //    Assert.AreEqual(125, pred.Range_300000, "300000 Range is incorrect.");
//        //    Assert.AreEqual(-6.020600f, pred.dB_300000, 0.00001f, "300000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_300000, 0.00001f, "300000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_300000, "300000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_300000, 0.00001f, "300000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_300000, 0.00001f, "300000 Leakage is incorrect.");

//        //    Assert.AreEqual(150000, pred.Freq_150000, "150000 Freq is incorrect.");
//        //    Assert.AreEqual(16000, pred.uF_150000, "150000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_150000, "150000 Xmit V is incorrect.");
//        //    Assert.AreEqual(8, pred.Bin_150000, "150000 Bin is incorrect.");
//        //    Assert.AreEqual(265, pred.Range_150000, "150000 Range is incorrect.");
//        //    Assert.AreEqual(-9.030900f, pred.dB_150000, 0.00001f, "150000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_150000, 0.00001f, "150000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_150000, "150000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_150000, 0.00001f, "150000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_150000, 0.00001f, "150000 Leakage is incorrect.");
//        //    #endregion

//        //    #region Verify Beam Xmt Power
//        //    Assert.AreEqual(5.00f, pred.BeamTransmitPower, "Beam Transmit Power is incorrect.");
//        //    #endregion

//        //    #region Verify Time
//        //    Assert.AreEqual(86400, pred.BottomTrackPings, "Bottom Track Pings is incorrect.");
//        //    Assert.AreEqual(0.06000f, pred.BottomTrackTime, 0.00001f, "Bottom Track Time is incorrect.");
//        //    Assert.AreEqual(0.100000f, pred.TimeBetweenPings, 0.0001f, "Time Between Pings is incorrect.");
//        //    Assert.AreEqual(0.100000f, pred.ProfileTime, 0.00001f, "Profile Time is incorrect.");
//        //    Assert.AreEqual(0.000713f, pred.LagTime, 0.00001f, "Lag Time is incorrect.");
//        //    Assert.AreEqual(0.001426f, pred.BinTime, 0.00001f, "Bin Time is incorrect.");
//        //    Assert.AreEqual(0.100000f, pred.ReceiveTime, 0.00001f, "Receive Time is incorrect.");
//        //    Assert.AreEqual(0.002140f, pred.TransmitCodeTime, 0.00001f, "Transmit Code Time is incorrect.");
//        //    #endregion

//        //    #region Verify Power
//        //    Assert.AreEqual(5.7600000f, pred.BtTransmitPower, 0.00001f, "Bottom Track Transmit Power is incorrect.");
//        //    Assert.AreEqual(12.9600000f, pred.BtReceivePower, 0.00001f, "Bottom Track Receive Power is incorrect.");
//        //    Assert.AreEqual(1.0269810f, pred.TransmitPower, 0.00001f, "Transmit Power is incorrect.");
//        //    Assert.AreEqual(21.6f, pred.ReceivePower, 0.00001f, "Receive Power is incorrect.");
//        //    Assert.AreEqual(0.03f, pred.SleepPower, 0.00001f, "Sleep Power is incorrect.");
//        //    Assert.AreEqual(0.001250f, pred.WakeupPower, 0.00001f, "Wakeup Power is incorrect.");
//        //    Assert.AreEqual(2.03772668, pred.CapChargePower, 0.00001f, "Cap Charge Power is incorrect.");
//        //    Assert.AreEqual(43.4159582, pred.TotalPower, 0.00001f, "Total Power is incorrect.");
//        //    Assert.AreEqual(374, pred.ActualBatteryPower, 0.001f, "Battery Power is incorrect.");
//        //    Assert.AreEqual(0.1160854498, pred.NumberBatteryPacks, 0.00001f, "Number of Battery Packs is incorrect.");
//        //    #endregion

//        //    #region Verify BB
//        //    Assert.AreEqual(103760.416667f, pred.SampleRate, 0.01f, "Sample Rate is incorrect.");
//        //    Assert.AreEqual(0.006747f, pred.MetersPerSample, 0.0001f, "Meters Per Sample is incorrect.");
//        //    Assert.AreEqual(148, pred.BinSamples, "Bin Samples is incorrect.");
//        //    Assert.AreEqual(74, pred.LagSamples, "Lag Samples is incorrect.");
//        //    Assert.AreEqual(3, pred.CodeRepeats, "Code Repeats is incorrect.");
//        //    Assert.AreEqual(0.666001f, pred.rho, 0.0001f, "rho is incorrect.");
//        //    Assert.AreEqual(701.083896f, pred.UaHz, 0.0001f, "Ua Hz is incorrect.");
//        //    Assert.AreEqual(0.419482f, pred.UaRadial, 0.0001f, "Ua Radial is incorrect.");
//        //    Assert.AreEqual(0.009398f, pred.StdDevRadial, 0.0001f, "Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.019431f, pred.StdDevSystem, 0.0001f, "Std Dev System is incorrect.");
//        //    #endregion

//        //    #region Verify NB
//        //    Assert.AreEqual(1.4f, pred.NbFudge, 0.0001f, "NB Fudge is incorrect.");
//        //    Assert.AreEqual(0.094999f, pred.NbStdDevRadial, 0.0001f, "NB Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.196405f, pred.NbStdDevHSystem, 0.0001f, "NB Std Dev System is incorrect.");
//        //    Assert.AreEqual(0.349066f, pred.BeamAngleRadian, 0.0001f, "NB Angle is incorrect.");
//        //    Assert.AreEqual(0.001428f, pred.NbTa, 0.0001f, "NB Ta is incorrect.");
//        //    Assert.AreEqual(700.071002f, pred.NbBn, 0.0001f, "NB Bn is incorrect.");
//        //    Assert.AreEqual(0.001197f, pred.NbLamda, 0.0001f, "NB Lamda is incorrect.");
//        //    Assert.AreEqual(1.064178f, pred.NbL, 0.0001f, "NB L is incorrect.");
//        //    #endregion

//        //    #region Verify Bytes
//        //    Assert.AreEqual(1, pred.Wakeups, "Wakeups is incorrect.");
//        //    Assert.AreEqual(86400, pred.NumEnsembles, "Num Ensembles is incorrect.");
//        //    Assert.AreEqual(3276, pred.EnsembleSizeBytes, "Ensemble Size is incorrect.");
//        //    Assert.AreEqual(112, pred.ProfileOverhead, "Profile Overhead is incorrect.");
//        //    Assert.AreEqual(2240, pred.BytesPerBin, "Bytes Per Bin is incorrect.");
//        //    Assert.AreEqual(384, pred.BytesBottomTrack, "Bottom Track Bytes is incorrect.");
//        //    Assert.AreEqual(504, pred.BytesOverhead, "Overhead Bytes is incorrect.");
//        //    Assert.AreEqual(4, pred.BytesChecksum, "Checksum bytes is incorrect.");
//        //    Assert.AreEqual(32, pred.BytesWrapper, "Wrapper Bytes is incorrect.");
//        //    Assert.AreEqual(0, pred.BytesNoPing, "No Ping Bytes is incorrect.");
//        //    #endregion

//        //    // Results
//        //    Assert.AreEqual(40, pred.PredictedBottomRange, 0.00001f, "Bottom Track Range is incorrect.");
//        //    Assert.AreEqual(20, pred.PredictedProfileRange, 0.00001f, "Profile range is incorrect.");
//        //    Assert.AreEqual(1.226483, pred.MaximumVelocity, 0.00001f, "Max Velocity is incorrect.");
//        //    Assert.AreEqual(0.019431, pred.StandardDeviation, 0.00001f, "Profile Standard Deviation is incorrect.");
//        //    Assert.AreEqual(2.74927762, pred.ProfileFirstBinPosition, 0.00001f, "Profile First Bin Position is incorrect.");
//        //    Assert.AreEqual(283.046400, pred.DataSizeBytes / 1000000.0, 0.00001f, "Ensemble Data Size is incorrect.");
//        //    Assert.AreEqual(0.1160854498, pred.NumberBatteryPacks, 0.00001f, "Battery Required is incorrect.");
//        //}

//        ///// <summary>
//        ///// Test a deployment for 300Khz for 450 days.
//        ///// 
//        ///// Calculations based off spreadsheet "adcp predictor revE".
//        ///// 
//        ///// Depth Cell Size: 8m
//        ///// Number of cells: 12 cells
//        ///// Ping Interval: 2 seconds (Not needed because only 1 ping per ensemble)
//        ///// Measure Interval: 1 Hour
//        ///// Days: 15 months (450 days)
//        ///// Ping Per Ensemble: 1 ping
//        ///// </summary>
//        //[Test]
//        //public void Deployment450Days_300Khz()
//        //{
//        //    Subsystem ss = new Subsystem(Subsystem.SUB_300KHZ_4BEAM_20DEG_PISTON_4, 0);
//        //    AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);
//        //    input.DeploymentDuration = 450;     // Number of days for deployment
//        //    input.CEI = 3600;                   // Times between Ensmbles (Number of seconds in a hour = 3600) 
//        //    input.CWPBN = 12;                   // Number of bins
//        //    input.CWPTBP = 2;                   // Time between pings
//        //    input.CWPBS = 8;                    // Bin Size
//        //    input.CBTON = false;                // Bottom Track Off
//        //    input.CWPP = 1;                     // Pings Per Ensemble
//        //    input.CWPBB_LagLength = 1;          // Lag Length
//        //    input.CWPBL = 0.40f;                // Blank
//        //    input.BatteryType = DeploymentOptions.AdcpBatteryType.Alkaline_21D;

//        //    AdcpPredictor pred = new AdcpPredictor(input);

//        //    Assert.AreEqual(311281.25, pred.SystemFrequency, 0.0001, "System Frequency is incorrect.");

//        //    #region Calculations

//        //    Assert.AreEqual(1490, pred.SpeedOfSound, "Speed Of Sound is incorrect.");
//        //    Assert.AreEqual(20, pred.BeamAngle, "Beam Angle is incorrect.");
//        //    Assert.AreEqual(25940.104167, pred.SampleRate, 0.0001, "Sample Rate is incorrect.");
//        //    Assert.AreEqual(0.026988, pred.MetersPerSample, 0.0001, "Meters Per Samples is incorrect.");
//        //    Assert.AreEqual(8, pred.CWPBS, "Bin Lenght is incorrect.");
//        //    Assert.AreEqual(296, pred.BinSamples, "Bin Samples is incorrect.");

//        //    Assert.AreEqual(1, pred.CWPBB_LagLength, "Lag Length is incorrect.");

//        //    Assert.AreEqual(38, pred.LagSamples, 0.0001, "Lag Samples is incorrect.");

//        //    Assert.AreEqual(RTI.Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "Broadband On/Off is incorrect.");
//        //    Assert.AreEqual(0.001465, pred.LagTime, 0.0001, "Lag Time is incorrect.");
//        //    Assert.AreEqual(0.011411, pred.BinTime, 0.0001, "Bin Time is incorrect.");
//        //    Assert.AreEqual(8, pred.CodeRepeats, "Code Repeats is incorrect.");

//        //    Assert.AreEqual(1, pred.CWPP, "Pings Per Ensemble is incorrect.");
//        //    Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//        //    Assert.AreEqual(20, pred.BeamTransmitPower, "Beams Transmit Power is incorrect.");
//        //    Assert.AreEqual(0.011719, pred.TransmitCodeTime, 0.0001, "Transmit Code time is incorrect.");

//        //    Assert.AreEqual(459, pred.ActualBatteryPower, "Battery Watt Hr is incorrect.");
//        //    Assert.AreEqual(0.0, pred.BtTransmitPower, "Bottom Track Transmit power is incorrect.");
//        //    Assert.AreEqual(0.0, pred.BtReceivePower, "Bottom Track Receive Power is incorrect.");
//        //    Assert.AreEqual(2.812633, pred.TransmitPower, 0.0001, "Transmit power is incorrect.");
//        //    Assert.AreEqual(27, pred.ReceivePower, 0.0001, "Receive Power is incorrect.");
//        //    Assert.AreEqual(13.5, pred.SleepPower, 0.0001, "Sleep Power is incorrect.");
//        //    Assert.AreEqual(13.5, pred.WakeupPower, 0.0001, "Wakeup Power is incorrect.");
//        //    Assert.AreEqual(2.3128737, pred.CapChargePower, 0.0001, "Cap Charge Power is incorrect.");
//        //    Assert.AreEqual(59.125507, pred.TotalPower, 0.0001, "Total Power is incorrect.");

//        //    Assert.AreEqual(0.1288137, pred.NumberBatteryPacks, 0.0001, "Number of battery packs is incorrect.");

//        //    #endregion

//        //    #region Results

//        //    Assert.AreEqual(0.0, pred.PredictedBottomRange, "Predicted Bottom Track Range is incorrect.");
//        //    Assert.AreEqual(137.04120, pred.PredictedProfileRange, 0.0001, "Predicted Profile Range is incorrect.");
//        //    Assert.AreEqual(2.388415, pred.MaximumVelocity, 0.0001, "Maximum velocity is incorrect.");
//        //    Assert.AreEqual(12.48940129, pred.ProfileFirstBinPosition, 0.0001, "First Bin Position is incorrect.");
//        //    Assert.AreEqual(0.015532, pred.StandardDeviation, 0.0001, "Standard Deviation is incorrect.");
//        //    Assert.AreEqual(21.556800, pred.DataSizeBytes / 1000000.0, "Data Size in bytes is incorrect.");

//        //    #endregion
//        //}

//        ///// <summary>
//        ///// Test a 600kHz system with only Water Profile BroardBand turned on.
//        ///// </summary>
//        //[Test]
//        //public void Deployment2Days_600Khz_WP_BB()
//        //{
//        //    Subsystem ss = new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3, 0);
//        //    AdcpPredictorUserInput input = new AdcpPredictorUserInput(ss);

//        //    // Set values
//        //    Assert.AreEqual(622562.5, input.SystemFrequency, "System Frequency is incorrect.");         // Frequency set with subsystem
//        //    input.DeploymentDuration = 2;
//        //    input.CEI = 0.25;
//        //    input.CWPON = true;
//        //    input.CBTON = false;
//        //    input.CBTTBP = 0;
//        //    input.CWPTBP = 0.25f;
//        //    input.CWPBN = 30;
//        //    input.CWPBS = 1.00f;
//        //    input.CWPBL = 0.40f;
//        //    input.CWPBB_LagLength = 1.00f;
//        //    input.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
//        //    input.CWPP = 1;
//        //    input.BatteryType = DeploymentOptions.AdcpBatteryType.Alkaline_38C;
//        //    input.BatteryDerate = 0.85f;
//        //    input.SpeedOfSound = 1490;
//        //    input.BeamAngle = 20;


//        //    AdcpPredictor pred = new AdcpPredictor(input);

//        //    // Verify Input values
//        //    #region Verify Inputs
//        //    Assert.AreEqual(622562.5, input.SystemFrequency, 0.00001f, "Input System Frequency is incorrect.");
//        //    Assert.AreEqual(2, input.DeploymentDuration, "Input Deployment Duration is incorrect.");
//        //    Assert.AreEqual(0.25, input.CEI, 0.0001, "Input CEI is incorrect.");
//        //    Assert.AreEqual(true, input.CWPON, "Input CWPON is incorrect.");
//        //    Assert.AreEqual(false, input.CBTON, "Input CBTON is incorrect.");
//        //    Assert.AreEqual(0, input.CBTTBP, 0.00001f, "Input CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.25f, input.CWPTBP, 0.00001f, "Input CWPTBP is incorrect.");
//        //    Assert.AreEqual(30, input.CWPBN, "Input CWPBN is incorrect.");
//        //    Assert.AreEqual(1.00f, input.CWPBS, 0.00001f, "Input CWPBS is incorrect.");
//        //    Assert.AreEqual(0.40f, input.CWPBL, 0.00001f, "Input CWPBL is incorrect.");
//        //    Assert.AreEqual(1.00f, input.CWPBB_LagLength, 0.00001f, "Input WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, input.CWPBB_TransmitPulseType, "Input WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, input.CWPP, "Input CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, input.BatteryType, "Input Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, input.BatteryDerate, 0.000001f, "Input Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, input.SpeedOfSound, "Input Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, input.BeamAngle, "Input Beam Angle is incorrect.");

//        //    Assert.AreEqual(622562.5, pred.SystemFrequency, 0.00001f, "System Frequency is incorrect.");
//        //    Assert.AreEqual(2, pred.DeploymentDuration, "Deployment Duration is incorrect.");
//        //    Assert.AreEqual(0.25, pred.CEI, 0.0001, "CEI is incorrect.");
//        //    Assert.AreEqual(true, pred.CWPON, "CWPON is incorrect.");
//        //    Assert.AreEqual(false, pred.CBTON, "CBTON is incorrect.");
//        //    Assert.AreEqual(0, pred.CBTTBP, 0.00001f, "CBTTBP is incorrect.");
//        //    Assert.AreEqual(0.25f, pred.CWPTBP, 0.00001f, "CWPTBP is incorrect.");
//        //    Assert.AreEqual(30, pred.CWPBN, "CWPBN is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.CWPBS, 0.00001f, "CWPBS is incorrect.");
//        //    Assert.AreEqual(0.40f, pred.CWPBL, 0.00001f, "CWPBL is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.CWPBB_LagLength, 0.00001f, "WP Lag Length is incorrect.");
//        //    Assert.AreEqual(Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND, pred.CWPBB_TransmitPulseType, "WP BB Transmit Pulse Type is incorrect.");
//        //    Assert.AreEqual(1, pred.CWPP, "CWPP is incorrect.");
//        //    Assert.AreEqual(DeploymentOptions.AdcpBatteryType.Alkaline_38C, pred.BatteryType, "Battery Type is incorrect.");
//        //    Assert.AreEqual(0.85f, pred.BatteryDerate, 0.000001f, "Battery Derate is incorrect.");
//        //    Assert.AreEqual(1490, pred.SpeedOfSound, "Speed of sound is incorrect.");
//        //    Assert.AreEqual(20, pred.BeamAngle, "Beam Angle is incorrect.");
//        //    #endregion

//        //    #region Verify Settings
//        //    Assert.AreEqual(12, pred.CyclesPerElement, "Cycles per element is incorrect.");
//        //    Assert.AreEqual(1.00f, pred.Beta, 0.00001f, "Beta is incorrect.");
//        //    Assert.AreEqual(30.00f, pred.SNR, 0.00001f, "SNR is incorrect.");
//        //    Assert.AreEqual(4, pred.Beams, "Beams is incorrect.");
//        //    Assert.AreEqual(4.50f, pred.SystemOnPower, 0.00001f, "System On Power is incorrect.");
//        //    Assert.AreEqual(0.00125, pred.SystemSleepPower, 0.0001f, "System Sleep Power is incorrect.");
//        //    Assert.AreEqual(1.0, pred.SystemWakeup, 0.00001f, "System Wakeup is incorrect.");
//        //    #endregion

//        //    #region Verify Tables
//        //    Assert.AreEqual(1200000, pred.Freq_1200000, "1200000 Freq is incorrect.");
//        //    Assert.AreEqual(11000, pred.uF_1200000, "1200000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_1200000, "1200000 Xmit V is incorrect.");
//        //    Assert.AreEqual(1, pred.Bin_1200000, "1200000 Bin is incorrect.");
//        //    Assert.AreEqual(20, pred.Range_1200000, "1200000 Range is incorrect.");
//        //    Assert.AreEqual(0, pred.dB_1200000, 0.00001f, "1200000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_1200000, 0.00001f, "1200000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_1200000, "1200000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_1200000, 0.00001f, "1200000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_1200000, 0.00001f, "1200000 Leakage is incorrect.");

//        //    Assert.AreEqual(600000, pred.Freq_600000, "600000 Freq is incorrect.");
//        //    Assert.AreEqual(22000, pred.uF_600000, "600000 uF is incorrect.");
//        //    Assert.AreEqual(18, pred.XmtV_600000, "600000 Xmit V is incorrect.");
//        //    Assert.AreEqual(2, pred.Bin_600000, "600000 Bin is incorrect.");
//        //    Assert.AreEqual(50, pred.Range_600000, "600000 Range is incorrect.");
//        //    Assert.AreEqual(-3.010300f, pred.dB_600000, 0.00001f, "600000 dB is incorrect.");
//        //    Assert.AreEqual(43.979400f, pred.WpRange_600000, 0.00001f, "600000 WP Range is incorrect.");
//        //    Assert.AreEqual(15, pred.XmtW_600000, "600000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_600000, 0.00001f, "600000 BT Range is incorrect.");
//        //    Assert.AreEqual(2.66983f, pred.LeakageuA_600000, 0.00001f, "600000 Leakage is incorrect.");

//        //    Assert.AreEqual(300000, pred.Freq_300000, "300000 Freq is incorrect.");
//        //    Assert.AreEqual(44000, pred.uF_300000, "300000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_300000, "300000 Xmit V is incorrect.");
//        //    Assert.AreEqual(4, pred.Bin_300000, "300000 Bin is incorrect.");
//        //    Assert.AreEqual(125, pred.Range_300000, "300000 Range is incorrect.");
//        //    Assert.AreEqual(-6.020600f, pred.dB_300000, 0.00001f, "300000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_300000, 0.00001f, "300000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_300000, "300000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_300000, 0.00001f, "300000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_300000, 0.00001f, "300000 Leakage is incorrect.");

//        //    Assert.AreEqual(150000, pred.Freq_150000, "150000 Freq is incorrect.");
//        //    Assert.AreEqual(16000, pred.uF_150000, "150000 uF is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtV_150000, "150000 Xmit V is incorrect.");
//        //    Assert.AreEqual(8, pred.Bin_150000, "150000 Bin is incorrect.");
//        //    Assert.AreEqual(265, pred.Range_150000, "150000 Range is incorrect.");
//        //    Assert.AreEqual(-9.030900, pred.dB_150000, 0.00001f, "150000 dB is incorrect.");
//        //    Assert.AreEqual(0, pred.WpRange_150000, 0.00001f, "150000 WP Range is incorrect.");
//        //    Assert.AreEqual(0, pred.XmtW_150000, "150000 Xmit W is incorrect.");
//        //    Assert.AreEqual(0, pred.BtRange_150000, 0.00001f, "150000 BT Range is incorrect.");
//        //    Assert.AreEqual(0, pred.LeakageuA_150000, 0.00001f, "150000 Leakage is incorrect.");
//        //    #endregion

//        //    #region Verify Beam Xmt Power
//        //    Assert.AreEqual(15.00, pred.BeamTransmitPower, "Beam Transmit Power is incorrect.");
//        //    #endregion

//        //    #region Verify Time
//        //    Assert.AreEqual(0, pred.BottomTrackPings, "Bottom Track Pings is incorrect.");
//        //    Assert.AreEqual(0.000000f, pred.BottomTrackTime, 0.00001f, "Bottom Track Time is incorrect.");
//        //    Assert.AreEqual(0.250000f, pred.TimeBetweenPings, 0.0001f, "Time Between Pings is incorrect.");
//        //    Assert.AreEqual(0.250000f, pred.ProfileTime, 0.00001f, "Profile Time is incorrect.");
//        //    Assert.AreEqual(0.001426f, pred.LagTime, 0.00001f, "Lag Time is incorrect.");
//        //    Assert.AreEqual(0.001426f, pred.BinTime, 0.00001f, "Bin Time is incorrect.");
//        //    Assert.AreEqual(0.250000f, pred.ReceiveTime, 0.00001f, "Receive Time is incorrect.");
//        //    Assert.AreEqual(0.002853f, pred.TransmitCodeTime, 0.00001f, "Transmit Code Time is incorrect.");
//        //    #endregion

//        //    #region Verify Power
//        //    Assert.AreEqual(0.000000f, pred.BtTransmitPower, 0.00001f, "Bottom Track Transmit Power is incorrect.");
//        //    Assert.AreEqual(0.000000f, pred.BtReceivePower, 0.00001f, "Bottom Track Receive Power is incorrect.");
//        //    Assert.AreEqual(32.863399, pred.TransmitPower, 0.00001f, "Transmit Power is incorrect.");
//        //    Assert.AreEqual(216.0, pred.ReceivePower, 0.00001f, "Receive Power is incorrect.");
//        //    Assert.AreEqual(0.0600000f, pred.SleepPower, 0.00001f, "Sleep Power is incorrect.");
//        //    Assert.AreEqual(0.0012500f, pred.WakeupPower, 0.00001f, "Wakeup Power is incorrect.");
//        //    Assert.AreEqual(9.86201853, pred.CapChargePower, 0.00001f, "Cap Charge Power is incorrect.");
//        //    Assert.AreEqual(258.7866677889, pred.TotalPower, 0.00001f, "Total Power is incorrect.");
//        //    Assert.AreEqual(374, pred.ActualBatteryPower, 0.001f, "Battery Power is incorrect.");
//        //    Assert.AreEqual(0.69194294, pred.NumberBatteryPacks, 0.00001f, "Number of Battery Packs is incorrect.");
//        //    #endregion

//        //    #region Verify BB
//        //    Assert.AreEqual(51880.208f, pred.SampleRate, 0.01f, "Sample Rate is incorrect.");
//        //    Assert.AreEqual(0.013494f, pred.MetersPerSample, 0.0001f, "Meters Per Sample is incorrect.");
//        //    Assert.AreEqual(74, pred.BinSamples, "Bin Samples is incorrect.");
//        //    Assert.AreEqual(74, pred.LagSamples, "Lag Samples is incorrect.");
//        //    Assert.AreEqual(2, pred.CodeRepeats, "Code Repeats is incorrect.");
//        //    Assert.AreEqual(0.4995f, pred.rho, 0.0001f, "rho is incorrect.");
//        //    Assert.AreEqual(350.541948f, pred.UaHz, 0.0001f, "Ua Hz is incorrect.");
//        //    Assert.AreEqual(0.419482f, pred.UaRadial, 0.0001f, "Ua Radial is incorrect.");
//        //    Assert.AreEqual(0.023629f, pred.StdDevRadial, 0.0001f, "Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.048852f, pred.StdDevSystem, 0.0001f, "Std Dev System is incorrect.");
//        //    #endregion

//        //    #region Verify NB
//        //    Assert.AreEqual(1.4f, pred.NbFudge, 0.0001f, "NB Fudge is incorrect.");
//        //    Assert.AreEqual(0.189998f, pred.NbStdDevRadial, 0.0001f, "NB Std Dev Radial is incorrect.");
//        //    Assert.AreEqual(0.392810f, pred.NbStdDevHSystem, 0.0001f, "NB Std Dev System is incorrect.");
//        //    Assert.AreEqual(0.349066f, pred.BeamAngleRadian, 0.0001f, "NB Angle is incorrect.");
//        //    Assert.AreEqual(0.001428f, pred.NbTa, 0.0001f, "NB Ta is incorrect.");
//        //    Assert.AreEqual(700.071002f, pred.NbBn, 0.0001f, "NB Bn is incorrect.");
//        //    Assert.AreEqual(0.002393f, pred.NbLamda, 0.0001f, "NB Lamda is incorrect.");
//        //    Assert.AreEqual(1.064178f, pred.NbL, 0.0001f, "NB L is incorrect.");
//        //    #endregion

//        //    #region Verify Bytes
//        //    Assert.AreEqual(1, pred.Wakeups, "Wakeups is incorrect.");
//        //    Assert.AreEqual(691200, pred.NumEnsembles, "Num Ensembles is incorrect.");
//        //    Assert.AreEqual(4012, pred.EnsembleSizeBytes, "Ensemble Size is incorrect.");
//        //    Assert.AreEqual(112, pred.ProfileOverhead, "Profile Overhead is incorrect.");
//        //    Assert.AreEqual(3360, pred.BytesPerBin, "Bytes Per Bin is incorrect.");
//        //    Assert.AreEqual(0, pred.BytesBottomTrack, "Bottom Track Bytes is incorrect.");
//        //    Assert.AreEqual(504, pred.BytesOverhead, "Overhead Bytes is incorrect.");
//        //    Assert.AreEqual(4, pred.BytesChecksum, "Checksum bytes is incorrect.");
//        //    Assert.AreEqual(32, pred.BytesWrapper, "Wrapper Bytes is incorrect.");
//        //    Assert.AreEqual(0, pred.BytesNoPing, "No Ping Bytes is incorrect.");
//        //    #endregion

//        //    // Results
//        //    Assert.AreEqual(0, pred.PredictedBottomRange, 0.00001f, "Bottom Track Range is incorrect.");
//        //    Assert.AreEqual(43.979400, pred.PredictedProfileRange, 0.00001f, "Profile range is incorrect.");
//        //    Assert.AreEqual(1.226483, pred.MaximumVelocity, 0.00001f, "Max Velocity is incorrect.");
//        //    Assert.AreEqual(0.048852, pred.StandardDeviation, 0.00001f, "Profile Standard Deviation is incorrect.");
//        //    Assert.AreEqual(2.3992776, pred.ProfileFirstBinPosition, 0.00001f, "Profile First Bin Position is incorrect.");
//        //    Assert.AreEqual(2773.0944, pred.DataSizeBytes / 1000000.0, 0.00001f, "Ensemble Data Size is incorrect.");
//        //    Assert.AreEqual(0.69194294, pred.NumberBatteryPacks, 0.00001f, "Battery Required is incorrect.");


//        //}

//        ///// <summary>
//        ///// Test 600 Waves PUV values.
//        ///// </summary>
//        //[Test]
//        //public void WavesPUV()
//        //{
//        //    double binSize = 0.5;
//        //    double blank = 0.5;
//        //    double lag = 0.20;
//        //    double range = 0.0;
//        //    double sd = 0.0;
//        //    double maxVel = 0.0;
//        //    double firstBin = 0.0;

//        //    AdcpPredictor.WavesModelPUV(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), binSize, blank, lag, out range, out sd, out maxVel, out firstBin);

//        //    Assert.AreEqual(37.9588, range, 0.000001, "Range is incorrect.");
//        //    Assert.AreEqual(0.0991559, sd, 0.000001, "StandardDev is incorrect.");
//        //    Assert.AreEqual(2.2172619, maxVel, 0.000001, "Maximum Velocity is incorrect.");
//        //    Assert.AreEqual(1.30, firstBin, 0.000001, "First Bin is incorrect.");
//        //}

//        ///// <summary>
//        ///// Test 600kHz Waves Watt-Hrs based off samples per burst and sample rate.
//        ///// example per day
//        ///// xmt W-hr    = 24 * (2048 * 0.00155) * xWatts / 3600
//        ///// awake W-hr  = 24 * (2048 * 0.5 + 2) * rWatts / 3600
//        ///// asleep W-hr = 24 * 0.00125
//        ///// for 600 kHz the 440 w-hr 2x19 pack lasts 10 days burst once per hour
//        ///// </summary>
//        //[Test]
//        //public void WavesWattHours()
//        //{
//        //    // 2048 samples per burst
//        //    // 0.5 sec sample rate
//        //    double result = AdcpPredictor.WavesWattHoursPerBurst(new Subsystem(Subsystem.SUB_600KHZ_4BEAM_20DEG_PISTON_3), 2048, 0.5);

//        //    Assert.AreEqual(1.363462, result, 0.00001, "Waves Watt-Hrs is incorrect.");
//        //}

//        ///// <summary>
//        ///// Number of bytes will be used for the given number of samples and bins
//        ///// for waves burst.
//        ///// </summary>
//        //[Test]
//        //public void WavesBytesPerBurst()
//        //{
//        //    // 2048 samples per burst
//        //    // 30 bins
//        //    double result = AdcpPredictor.WavesRecordBytesPerBurst(2048, 30);

//        //    Assert.AreEqual(8216576, result, "Waves Recorded bytes is incorrect.");
//        //}
//    }
//}
