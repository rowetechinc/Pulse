using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    using NUnit.Framework;

    /// <summary>
    /// Test the calculations for the AdcpPredictor object.
    /// This object calculates the battery usage, ping characteristics
    /// and memory usage based off the settings set in the ADCP.
    /// 
    /// Calculations based off spreadsheet "adcp predictor revE".
    /// 
    /// </summary>
    [TestFixture]
    public class PredictionModelTest
    {
        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void AllDataSets()
        {
          
            PredictionModel model = new PredictionModel();
            
            int cwpbn = 30;
            int beam = 4;
            int deploymentDuration = 30;
            float cei = 1.0f;
            bool IsE0000001 = true;
            bool IsE0000002 = true;
            bool IsE0000003 = true;
            bool IsE0000004 = true;
            bool IsE0000005 = true;
            bool IsE0000006 = true;
            bool IsE0000007 = true;
            bool IsE0000008 = true;
            bool IsE0000009 = true;
            bool IsE0000010 = true;
            bool IsE0000011 = true;
            bool IsE0000012 = true;
            bool IsE0000013 = true;
            bool IsE0000014 = true;
            bool IsE0000015 = true;

            double bytes = model.GetDataStorage(cwpbn, beam, deploymentDuration, cei, IsE0000001, IsE0000002, IsE0000003, IsE0000004, IsE0000005, IsE0000006, IsE0000007, IsE0000008, IsE0000009, IsE0000010, IsE0000011, IsE0000012, IsE0000013, IsE0000014, IsE0000015);
            double correctAnswer = 12172032000;
            Assert.AreEqual(correctAnswer, bytes);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void AllDataSetsInput()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();


            input.CWPBN = 30;
            input.Beams = 4;
            input.DeploymentDuration = 30;
            input.CEI = 1.0;
            input.CED_IsE0000001 = true;
            input.CED_IsE0000002 = true;
            input.CED_IsE0000003 = true;
            input.CED_IsE0000004 = true;
            input.CED_IsE0000005 = true;
            input.CED_IsE0000006 = true;
            input.CED_IsE0000007 = true;
            input.CED_IsE0000008 = true;
            input.CED_IsE0000009 = true;
            input.CED_IsE0000010 = true;
            input.CED_IsE0000011 = true;
            input.CED_IsE0000012 = true;
            input.CED_IsE0000013 = true;
            input.CED_IsE0000014 = true;
            input.CED_IsE0000015 = true;

            double bytes = model.GetDataStorage(input);
            double correctAnswer = 12172032000;
            Assert.AreEqual(correctAnswer, bytes);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void TurnOffSomeDataSets()
        {

            PredictionModel model = new PredictionModel();

            int cwpbn = 30;
            int beam = 4;
            int deploymentDuration = 30;
            float cei = 1.0f;
            bool IsE0000001 = true;
            bool IsE0000002 = false;
            bool IsE0000003 = true;
            bool IsE0000004 = false;
            bool IsE0000005 = true;
            bool IsE0000006 = true;
            bool IsE0000007 = false;
            bool IsE0000008 = true;
            bool IsE0000009 = false;
            bool IsE0000010 = true;
            bool IsE0000011 = false;
            bool IsE0000012 = true;
            bool IsE0000013 = false;
            bool IsE0000014 = true;
            bool IsE0000015 = false;

            double bytes = model.GetDataStorage(cwpbn, beam, deploymentDuration, cei, IsE0000001, IsE0000002, IsE0000003, IsE0000004, IsE0000005, IsE0000006, IsE0000007, IsE0000008, IsE0000009, IsE0000010, IsE0000011, IsE0000012, IsE0000013, IsE0000014, IsE0000015);
            double correctAnswer = 7153920000;
            Assert.AreEqual(correctAnswer, bytes);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void TurnOffSomeDataSetsInput()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();

            input.CWPBN = 30;
            input.Beams = 4;
            input.DeploymentDuration = 30;
            input.CEI = 1.0;
            input.CED_IsE0000001 = true;
            input.CED_IsE0000002 = false;
            input.CED_IsE0000003 = true;
            input.CED_IsE0000004 = false;
            input.CED_IsE0000005 = true;
            input.CED_IsE0000006 = true;
            input.CED_IsE0000007 = false;
            input.CED_IsE0000008 = true;
            input.CED_IsE0000009 = false;
            input.CED_IsE0000010 = true;
            input.CED_IsE0000011 = false;
            input.CED_IsE0000012 = true;
            input.CED_IsE0000013 = false;
            input.CED_IsE0000014 = true;
            input.CED_IsE0000015 = false;

            double bytes = model.GetDataStorage(input);
            double correctAnswer = 7153920000;
            Assert.AreEqual(correctAnswer, bytes);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void Std()
        {

            PredictionModel model = new PredictionModel();

            int CWPP = 9;
            double _CWPBS_ = 4;
            double _CWPBB_LagLength_ = 1.0;
            double _BeamAngle_ = 20.0;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _SystemFrequency_ = 288000.0;
            double _SpeedOfSound_ = 1490.0;
            double _CyclesPerElement_  = 12.0;
            double _SNR_ = 30.0;
            double _Beta_ =1.0;
            double _NbFudge_ = 1.4;

            double std = model.GetStandardDeviation(CWPP, _CWPBS_, _CWPBB_LagLength_, _BeamAngle_, _CWPBB_TransmitPulseType_, _SystemFrequency_, _SpeedOfSound_, _CyclesPerElement_, _SNR_, _Beta_, _NbFudge_);
            double correctAnswer = 0.010;
            Assert.AreEqual(correctAnswer, std, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void StdNb()
        {

            PredictionModel model = new PredictionModel();

            int CWPP = 9;
            double _CWPBS_ = 4;
            double _CWPBB_LagLength_ = 1.0;
            double _BeamAngle_ = 20.0;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            double _SystemFrequency_ = 288000.0;
            double _SpeedOfSound_ = 1490.0;
            double _CyclesPerElement_ = 12.0;
            double _SNR_ = 30.0;
            double _Beta_ = 1.0;
            double _NbFudge_ = 1.4;

            double std = model.GetStandardDeviation(CWPP, _CWPBS_, _CWPBB_LagLength_, _BeamAngle_, _CWPBB_TransmitPulseType_, _SystemFrequency_, _SpeedOfSound_, _CyclesPerElement_, _SNR_, _Beta_, _NbFudge_);
            double correctAnswer = 0.07;
            Assert.AreEqual(correctAnswer, std, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void StdInput()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();

            input.CWPP = 9;
            input.CWPBS = 4;
            input.CWPBB_LagLength = 1.0;
            input.BeamAngle = 20.0;
            input.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            input.SystemFrequency = 288000.0;
            input.SpeedOfSound = 1490.0;
            input.CyclesPerElement = 12;
            input.SNR = 30.0;
            input.Beta = 1.0;
            input.NbFudge = 1.4;

            double std = model.GetStandardDeviation(input);
            double correctAnswer = 0.010;
            Assert.AreEqual(correctAnswer, std, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void MaxVelocity()
        {

            PredictionModel model = new PredictionModel();

            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType type = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            double _CWPBB_LagLength_ = 1.0;
            double _BeamAngle_ = 20.0;
            double _SystemFrequency_ = 1152000.0;
            double _SpeedOfSound_ = 1490.0;
            double _CyclesPerElement_ = 12.0;
            double _CWPBS_ = 0.6;

            double vel = model.GetMaxVelocity(type, _CWPBB_LagLength_, _CWPBS_, _BeamAngle_, _SystemFrequency_, _SpeedOfSound_, _CyclesPerElement_);
            double correctAnswer = 1.867;
            Assert.AreEqual(correctAnswer, vel, 0.001);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void MaxVelocityInput()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();

            input.CWPBB_LagLength = 1.0;
            input.BeamAngle = 20.0;
            input.SystemFrequency = 288000.0;
            input.SpeedOfSound = 1490.0;
            input.CyclesPerElement = 12;

            double std = model.GetMaxVelocity(input);
            double correctAnswer = 2.669;
            Assert.AreEqual(correctAnswer, std, 0.001);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void MaxVelocityInputNb()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();

            input.CWPBB_LagLength = 1.0;
            input.BeamAngle = 20.0;
            input.SystemFrequency = 1152000.0;
            input.SpeedOfSound = 1467;
            input.CyclesPerElement = 12;
            input.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            input.CWPBS = 0.6f;

            double std = model.GetMaxVelocity(input);
            double correctAnswer = 1.867;
            Assert.AreEqual(correctAnswer, std, 0.001);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void Range()
        {

            PredictionModel model = new PredictionModel();

            bool _CWPON_ = true;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _CWPBS_ = 4.0;
            double _CWPBN_ = 30.0;
            double _CWPBL_ = 1.00;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            double _SystemFrequency_ = 288000.0;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12.0;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _CWPBB_LagLength_ = 1.0;
            bool _BroadbandPower_ = true;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            PredictionModel.PredictedRanges ranges = model.GetPredictedRange(_CWPON_, _CWPBB_TransmitPulseType_, _CWPBS_, _CWPBN_, _CWPBL_, _CBTON_, _CBTBB_TransmitPulseType_, _SystemFrequency_, _BeamDiameter_, _CyclesPerElement_, _BeamAngle_, _SpeedOfSound_, _CWPBB_LagLength_, _BroadbandPower_, salinity, temperature, xdcrDepth);
            double correctWpRange = 100.05;
            double correctBtRange = 199.14;
            double correctFirstBin = 5.484;
            double correctWpRangeUserSettings = _CWPBL_ + (_CWPBS_ * _CWPBN_);
            Assert.AreEqual(correctWpRange, ranges.WaterProfile, 0.01);
            Assert.AreEqual(correctBtRange, ranges.BottomTrack, 0.01);
            Assert.AreEqual(correctFirstBin, ranges.FirstBinPosition, 0.01);
            Assert.AreEqual(correctWpRangeUserSettings, ranges.ProfileRangeSettings);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void RangeInput()
        {

            PredictionModel model = new PredictionModel();
            PredictionModelInput input = new PredictionModelInput();

            input.CWPON = true;
            input.CWPBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            input.CWPBS = 4.0f;
            input.CWPBN = 30;
            input.CWPBL = 1.00f;
            input.CBTON = true;
            input.CBTBB_TransmitPulseType = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            input.SystemFrequency = 288000.0;
            input.BeamDiameter = 0.075;
            input.CyclesPerElement = 12;
            input.BeamAngle = 20;
            input.SpeedOfSound = 1490;
            input.CWPBB_LagLength = 1.0;
            input.BroadbandPower = true;

            PredictionModel.PredictedRanges ranges = model.GetPredictedRange(input);
            double correctWpRange = 100.05;
            double correctBtRange = 199.14;
            double correctFirstBin = 5.484;
            double correctWpRangeUserSettings = input.CWPBL + (input.CWPBS * input.CWPBN);
            Assert.AreEqual(correctWpRange, ranges.WaterProfile, 0.01);
            Assert.AreEqual(correctBtRange, ranges.BottomTrack, 0.01);
            Assert.AreEqual(correctFirstBin, ranges.FirstBinPosition, 0.01);
            Assert.AreEqual(correctWpRangeUserSettings, ranges.ProfileRangeSettings);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void RangeNB()
        {

            PredictionModel model = new PredictionModel();

            bool _CWPON_ = true;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            double _CWPBS_ = 4.0;
            double _CWPBN_ = 30.0;
            double _CWPBL_ = 1.00;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
            double _SystemFrequency_ = 288000.0;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12.0;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _CWPBB_LagLength_ = 1.0;
            bool _BroadbandPower_ = true;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            PredictionModel.PredictedRanges ranges = model.GetPredictedRange(_CWPON_, _CWPBB_TransmitPulseType_, _CWPBS_, _CWPBN_, _CWPBL_, _CBTON_, _CBTBB_TransmitPulseType_, _SystemFrequency_, _BeamDiameter_, _CyclesPerElement_, _BeamAngle_, _SpeedOfSound_, _CWPBB_LagLength_, _BroadbandPower_, salinity, temperature, xdcrDepth);
            double correctWpRange = 152.57;
            double correctBtRange = 319.14;
            double correctFirstBin = 5.025;
            double correctWpRangeUserSettings = _CWPBL_ + (_CWPBS_ * _CWPBN_);
            Assert.AreEqual(correctWpRange, ranges.WaterProfile, 0.01);
            Assert.AreEqual(correctBtRange, ranges.BottomTrack, 0.01);
            Assert.AreEqual(correctFirstBin, ranges.FirstBinPosition);
            Assert.AreEqual(correctWpRangeUserSettings, ranges.ProfileRangeSettings);
        }


        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void Power()
        {

            PredictionModel model = new PredictionModel();

            double _CEI_ = 1;
            double _DeploymentDuration_ = 30;
            int _Beams_ = 4;
            double _SystemFrequency_ = 288000;
            bool _CWPON_ = true;
            double _CWPBL_ = 1;
            double _CWPBS_ = 4;
            double _CWPBN_ = 30;
            double _CWPBB_LagLength_ = 1;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _CWPP_ = 9;
            double _CWPTBP_ = 0.5;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _SystemBootPower_ = 1.80;
            double _SystemWakeupTime_ = 0.40;
            double _SystemInitPower_ = 2.80;
            double _SystemInitTime_ = 0.25;
            bool _BroadbandPower_ = true;
            double _SystemSavePower_ = 1.80;
            double _SystemSaveTime_ = 0.15;
            double _SystemSleepPower_ = 0.024;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12;

            double _BatteryCapacity_ = 440.0;
            double _BatteryDerate_ = 0.85;
            double _BatterySelfDischarge_ = 0.05;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            double power = model.CalculatePower(_CEI_, _DeploymentDuration_, _Beams_, _SystemFrequency_, _CWPON_, _CWPBL_, _CWPBS_, _CWPBN_, _CWPBB_LagLength_, _CWPBB_TransmitPulseType_, _CWPP_, _CWPTBP_, _CBTON_, _CBTBB_TransmitPulseType_, _BeamAngle_, _SpeedOfSound_, _SystemBootPower_, _SystemWakeupTime_, _SystemInitPower_, _SystemInitTime_, _BroadbandPower_, _SystemSavePower_, _SystemSaveTime_, _SystemSleepPower_, _BeamDiameter_, _CyclesPerElement_, temperature, salinity, xdcrDepth);
            double correctAnswer = 30754.86;
            Assert.AreEqual(correctAnswer, power, 0.01);

            double batteryUsage = model.BatteryUsage(power, _DeploymentDuration_, _BatteryCapacity_, _BatteryDerate_, _BatterySelfDischarge_);
            double correctBattery = 82.23;
            Assert.AreEqual(correctBattery, batteryUsage, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void PowerNB()
        {

            PredictionModel model = new PredictionModel();

            double _CEI_ = 1;
            double _DeploymentDuration_ = 30;
            int _Beams_ = 4;
            double _SystemFrequency_ = 288000;
            bool _CWPON_ = true;
            double _CWPBL_ = 1;
            double _CWPBS_ = 4;
            double _CWPBN_ = 30;
            double _CWPBB_LagLength_ = 1;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.NARROWBAND;
            double _CWPP_ = 9;
            double _CWPTBP_ = 0.5;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.NARROWBAND_LONG_RANGE;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _SystemBootPower_ = 1.80;
            double _SystemWakeupTime_ = 0.40;
            double _SystemInitPower_ = 2.80;
            double _SystemInitTime_ = 0.25;
            bool _BroadbandPower_ = true;
            double _SystemSavePower_ = 1.80;
            double _SystemSaveTime_ = 0.15;
            double _SystemSleepPower_ = 0.024;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12;

            double _BatteryCapacity_ = 440.0;
            double _BatteryDerate_ = 0.85;
            double _BatterySelfDischarge_ = 0.05;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            double power = model.CalculatePower(_CEI_, _DeploymentDuration_, _Beams_, _SystemFrequency_, _CWPON_, _CWPBL_, _CWPBS_, _CWPBN_, _CWPBB_LagLength_, _CWPBB_TransmitPulseType_, _CWPP_, _CWPTBP_, _CBTON_, _CBTBB_TransmitPulseType_, _BeamAngle_, _SpeedOfSound_, _SystemBootPower_, _SystemWakeupTime_, _SystemInitPower_, _SystemInitTime_, _BroadbandPower_, _SystemSavePower_, _SystemSaveTime_, _SystemSleepPower_, _BeamDiameter_, _CyclesPerElement_, temperature, salinity, xdcrDepth);
            double correctAnswer = 34770.30;
            Assert.AreEqual(correctAnswer, power, 0.01);

            double batteryUsage = model.BatteryUsage(power, _DeploymentDuration_, _BatteryCapacity_, _BatteryDerate_, _BatterySelfDischarge_);
            double correctBattery = 92.97;
            Assert.AreEqual(correctBattery, batteryUsage, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void Power600()
        {

            PredictionModel model = new PredictionModel();

            double _CEI_ = 1;
            double _DeploymentDuration_ = 30;
            int _Beams_ = 4;
            double _SystemFrequency_ = 576000;
            bool _CWPON_ = true;
            double _CWPBL_ = 1;
            double _CWPBS_ = 4;
            double _CWPBN_ = 30;
            double _CWPBB_LagLength_ = 1;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _CWPP_ = 9;
            double _CWPTBP_ = 0.5;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _SystemBootPower_ = 1.80;
            double _SystemWakeupTime_ = 0.40;
            double _SystemInitPower_ = 2.80;
            double _SystemInitTime_ = 0.25;
            bool _BroadbandPower_ = true;
            double _SystemSavePower_ = 1.80;
            double _SystemSaveTime_ = 0.15;
            double _SystemSleepPower_ = 0.024;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12;

            double _BatteryCapacity_ = 440.0;
            double _BatteryDerate_ = 0.85;
            double _BatterySelfDischarge_ = 0.05;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            double power = model.CalculatePower(_CEI_, _DeploymentDuration_, _Beams_, _SystemFrequency_, _CWPON_, _CWPBL_, _CWPBS_, _CWPBN_, _CWPBB_LagLength_, _CWPBB_TransmitPulseType_, _CWPP_, _CWPTBP_, _CBTON_, _CBTBB_TransmitPulseType_, _BeamAngle_, _SpeedOfSound_, _SystemBootPower_, _SystemWakeupTime_, _SystemInitPower_, _SystemInitTime_, _BroadbandPower_, _SystemSavePower_, _SystemSaveTime_, _SystemSleepPower_, _BeamDiameter_, _CyclesPerElement_, temperature, salinity, xdcrDepth);
            double correctAnswer = 16852.22;
            Assert.AreEqual(correctAnswer, power, 0.01);

            double batteryUsage = model.BatteryUsage(power, _DeploymentDuration_, _BatteryCapacity_, _BatteryDerate_, _BatterySelfDischarge_);
            double correctBattery = 45.06;
            Assert.AreEqual(correctBattery, batteryUsage, 0.01);
        }

        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void PowerBurst()
        {

            PredictionModel model = new PredictionModel();

            double _CEI_ = 0.249;
            double _DeploymentDuration_ = 1;
            int _Beams_ = 4;
            double _SystemFrequency_ = 288000;
            bool _CWPON_ = true;
            double _CWPBL_ = 1;
            double _CWPBS_ = 4;
            double _CWPBN_ = 30;
            double _CWPBB_LagLength_ = 1;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _CWPP_ = 1;
            double _CWPTBP_ = 0.5;
            bool _CBTON_ = false;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _SystemBootPower_ = 1.80;
            double _SystemWakeupTime_ = 0.40;
            double _SystemInitPower_ = 2.80;
            double _SystemInitTime_ = 0.25;
            bool _BroadbandPower_ = true;
            double _SystemSavePower_ = 1.80;
            double _SystemSaveTime_ = 0.15;
            double _SystemSleepPower_ = 0.024;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12;

            int _CBI_EnsemblesPerBurst_ = 4096;
            double _CBI_BurstInterval_ = 3600;
            int _CBI_IsInterleaved_ = 0;

            double _BatteryCapacity_ = 440.0;
            double _BatteryDerate_ = 0.85;
            double _BatterySelfDischarge_ = 0.05;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            double power = model.CalculatePowerBurst(_CEI_, _DeploymentDuration_, _Beams_, _SystemFrequency_, _CWPON_, _CWPBL_, _CWPBS_, _CWPBN_, _CWPBB_LagLength_, _CWPBB_TransmitPulseType_, _CWPP_, _CWPTBP_, _CBTON_, _CBTBB_TransmitPulseType_, _BeamAngle_, _SpeedOfSound_, _SystemBootPower_, _SystemWakeupTime_, _SystemInitPower_, _SystemInitTime_, _BroadbandPower_, _SystemSavePower_, _SystemSaveTime_, _SystemSleepPower_, _BeamDiameter_, _CyclesPerElement_, temperature, salinity, xdcrDepth, _CBI_EnsemblesPerBurst_, _CBI_BurstInterval_, _CBI_IsInterleaved_);
            double correctAnswer = 65.10;
            Assert.AreEqual(correctAnswer, power, 0.01);

            double batteryUsage = model.BatteryUsage(power, _DeploymentDuration_, _BatteryCapacity_, _BatteryDerate_, _BatterySelfDischarge_);
            double correctBattery = 0.174;
            Assert.AreEqual(correctBattery, batteryUsage, 0.01);
        }


        /// <summary>
        /// Test the predictor with the default input.
        /// </summary>
        [Test]
        public void PowerBurstBt30()
        {

            PredictionModel model = new PredictionModel();

            double _CEI_ = 0.249;
            double _DeploymentDuration_ = 30;
            int _Beams_ = 4;
            double _SystemFrequency_ = 288000;
            bool _CWPON_ = true;
            double _CWPBL_ = 1;
            double _CWPBS_ = 4;
            double _CWPBN_ = 30;
            double _CWPBB_LagLength_ = 1;
            Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType _CWPBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCWPBB_TransmitPulseType.BROADBAND;
            double _CWPP_ = 1;
            double _CWPTBP_ = 0.5;
            bool _CBTON_ = true;
            Commands.AdcpSubsystemCommands.eCBTBB_Mode _CBTBB_TransmitPulseType_ = Commands.AdcpSubsystemCommands.eCBTBB_Mode.BROADBAND_CODED;
            double _BeamAngle_ = 20;
            double _SpeedOfSound_ = 1490;
            double _SystemBootPower_ = 1.80;
            double _SystemWakeupTime_ = 0.40;
            double _SystemInitPower_ = 2.80;
            double _SystemInitTime_ = 0.25;
            bool _BroadbandPower_ = true;
            double _SystemSavePower_ = 1.80;
            double _SystemSaveTime_ = 0.15;
            double _SystemSleepPower_ = 0.024;
            double _BeamDiameter_ = 0.075;
            double _CyclesPerElement_ = 12;

            int _CBI_EnsemblesPerBurst_ = 4096;
            double _CBI_BurstInterval_ = 3600;
            int _CBI_IsInterleaved_ = 0;

            double _BatteryCapacity_ = 440.0;
            double _BatteryDerate_ = 0.85;
            double _BatterySelfDischarge_ = 0.05;

            double temperature = 10.0;
            double salinity = 35.0;
            double xdcrDepth = 0.0;

            double power = model.CalculatePowerBurst(_CEI_, _DeploymentDuration_, _Beams_, _SystemFrequency_, _CWPON_, _CWPBL_, _CWPBS_, _CWPBN_, _CWPBB_LagLength_, _CWPBB_TransmitPulseType_, _CWPP_, _CWPTBP_, _CBTON_, _CBTBB_TransmitPulseType_, _BeamAngle_, _SpeedOfSound_, _SystemBootPower_, _SystemWakeupTime_, _SystemInitPower_, _SystemInitTime_, _BroadbandPower_, _SystemSavePower_, _SystemSaveTime_, _SystemSleepPower_, _BeamDiameter_, _CyclesPerElement_, temperature, salinity, xdcrDepth, _CBI_EnsemblesPerBurst_, _CBI_BurstInterval_, _CBI_IsInterleaved_);
            double correctAnswer = 12475.41;
            Assert.AreEqual(correctAnswer, power, 0.01);

            double batteryUsage = model.BatteryUsage(power, _DeploymentDuration_, _BatteryCapacity_, _BatteryDerate_, _BatterySelfDischarge_);
            double correctBattery = 33.36;
            Assert.AreEqual(correctBattery, batteryUsage, 0.01);
        }
    }
}
