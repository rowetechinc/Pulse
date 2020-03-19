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
 * 07/15/2013      RC          3.0.4      Initial coding
 * 12/06/2013      RC          3.2.0      Handle SelectedEnsembleEvent.
 * 01/28/2014      RC          3.2.3      Added VoltageRounded property.
 * 02/19/2014      RC          3.2.3      Changed the display of SystemSerialnumber and Subsystems if in DVL mode.
 * 07/24/2014      RC          3.4.0      Check if the Subsystem is given for a DVL.
 * 08/12/2014      RC          4.0.0      Removed the plots.
 * 11/11/2015      RC          4.3.1      Added timer to limit the update speed.
 * 05/19/2016      RC          4.4.7      Added GPS Heading.
 * 03/19/2020      RC          4.13.1     Added BurstIndex and BurstID.
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;
    using OxyPlot;
    using System.Windows;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ViewDataTextEnsembleViewModel : DisplayViewModel, IHandle<SelectedEnsembleEvent>
    {

        #region Variables

        /// <summary>
        /// Setup logger to report errors.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Pulse manager to manage the application.
        /// </summary>
        private PulseManager _pm;


        /// <summary>
        /// Subsystem Data Configuration for this view.
        /// </summary>
        private SubsystemDataConfig _Config;

        /// <summary>
        /// Timer to reduce the number of update calls the terminal window.
        /// </summary>
        private System.Timers.Timer _displayTimer;

        #region Defaults

        /// <summary>
        /// Color of the line for plots.
        /// </summary>
        private OxyColor LINE_COLOR = OxyColors.Orange;
        // private OxyColor LINE_COLOR = OxyColor.FromArgb(0xFF, 0x61, 0xA1, 0xC4); //FF61A1C4

        /// <summary>
        /// Maximum number of graph points.
        /// </summary>
        private const int MAX_GRAPH_POINTS = 10;

        #endregion

        #region TextPlots

        /// <summary>
        /// All the Plots on the page.
        /// </summary>
        public enum TextPlots
        {
            /// <summary>
            /// Speed of Sound plot.
            /// </summary>
            PLOT_SOS,

            /// <summary>
            /// Salinity plot.
            /// </summary>
            PLOT_SALINITY,

            /// <summary>
            /// Water Temp plot.
            /// </summary>
            PLOT_WATER_TEMP,

            /// <summary>
            /// System Temp plot.
            /// </summary>
            PLOT_SYS_TEMP,

            /// <summary>
            /// XDCR Depth plot.
            /// </summary>
            PLOT_XDCR_DEPTH,

            /// <summary>
            /// Pressure Plot.
            /// </summary>
            PLOT_PRESSURE,

            /// <summary>
            /// Heading Plot.
            /// </summary>
            PLOT_HEADING,

            /// <summary>
            /// Pitch plot.
            /// </summary>
            PLOT_PITCH,

            /// <summary>
            /// Roll Plot.
            /// </summary>
            PLOT_ROLL,

            /// <summary>
            /// All Plots.
            /// </summary>
            PLOT_ALL
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Current ensemble to display.
        /// </summary>
        private DataSet.Ensemble _DisplayEnsemble;

        /// <summary>
        /// Display the string representation of the ensemble.
        /// This will be updated when a new ensemble is received.
        /// </summary>
        public String DataSetString
        {
            get
            {
                if (_DisplayEnsemble == null)
                    return "";

                return _DisplayEnsemble.ToString();
            }
        }

        #region Ensemble
        
        /// <summary>
        /// Ensemble number.
        /// </summary>
        public string EnsembleNumber
        {
            get
            {
                if(_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.EnsembleNumber.ToString();
            }
        }

        /// <summary>
        /// Ensemble Date String.
        /// </summary>
        public string EnsDateString
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.EnsDateString;
            }
        }

        /// <summary>
        /// Ensemble Time String.
        /// </summary>
        public string EnsTimeString
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.EnsTimeString;
            }
        }

        /// <summary>
        /// Ensemble status.
        /// </summary>
        public string Status
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.Status.ToString();
            }
        }

        /// <summary>
        /// Ensemble Subsystem String Description.
        /// </summary>
        public string StringDesc
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.SubsystemConfig.StringDesc.ToString();
            }
        }

        /// <summary>
        /// Number of beams.
        /// </summary>
        public string NumBeams
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.NumBeams.ToString();
            }
        }

        /// <summary>
        /// Number of bins.
        /// </summary>
        public string NumBins
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.NumBins.ToString();
            }
        }

        /// <summary>
        /// Desired Ping Count.
        /// </summary>
        public string DesiredPingCount
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.DesiredPingCount.ToString();
            }
        }

        /// <summary>
        /// Actual Ping Count.
        /// </summary>
        public string ActualPingCount
        {
            get
            {
                if (_DisplayEnsemble == null)
                {
                    return "";
                }

                return _DisplayEnsemble.EnsembleData.ActualPingCount.ToString();
            }
        }

        /// <summary>
        /// A string of all the subsystems for the ADCP.
        /// This also check if the ADCP is in DVL mode.
        /// </summary>
        public string Subsystems
        {
            get
            {
                if (_DisplayEnsemble != null)
                {
                    // Check if the subsystem is only a DVL
                    if (_DisplayEnsemble.EnsembleData.SysSerialNumber.SubSystemsList.Contains(SerialNumber.DVL_Subsystem))
                    {
                        return "DVL";
                    }


                    return _DisplayEnsemble.EnsembleData.SysSerialNumber.SubSystems.ToString();
                }

                return "";
            }
        }

        /// <summary>
        /// System Serial Number.
        /// This will also check if its a DVL and change
        /// the serial number to DVL.
        /// </summary>
        public string SystemSerialNumber
        {
            get 
            {
                if (_DisplayEnsemble != null)
                {
                    if (_DisplayEnsemble.EnsembleData.SysSerialNumber == SerialNumber.DVL)
                    {
                        return "DVL";
                    }


                    return _DisplayEnsemble.EnsembleData.SysSerialNumber.SystemSerialNumber.ToString();
                }

                return "";
            }
        }

        /// <summary>
        /// System Serial Number.
        /// This will also check if its a DVL and change
        /// the serial number to DVL.
        /// </summary>
        public string Firmware
        {
            get
            {
                if (_DisplayEnsemble != null)
                {
                    if (_DisplayEnsemble.EnsembleData.SysSerialNumber == SerialNumber.DVL)
                    {
                        return "DVL";
                    }


                    return _DisplayEnsemble.EnsembleData.SysFirmware.ToString();
                }

                return "";
            }
        }

        /// <summary>
        /// Burst ID to label the data.
        /// This will also check if its a DVL and change
        /// the serial number to DVL.
        /// </summary>
        public string BurstID
        {
            get
            {
                if (_DisplayEnsemble != null)
                {
                    if (_DisplayEnsemble.EnsembleData.SysSerialNumber == SerialNumber.DVL)
                    {
                        return "DVL";
                    }


                    return _DisplayEnsemble.EnsembleData.BurstID.ToString();
                }

                return "";
            }
        }

        /// <summary>
        /// Burst Index to track the burst data.
        /// This will also check if its a DVL and change
        /// the serial number to DVL.
        /// </summary>
        public string BurstIndex
        {
            get
            {
                if (_DisplayEnsemble != null)
                {
                    if (_DisplayEnsemble.EnsembleData.SysSerialNumber == SerialNumber.DVL)
                    {
                        return "DVL";
                    }


                    return _DisplayEnsemble.EnsembleData.BurstIndex.ToString();
                }

                return "";
            }
        }

        #endregion

        #region Ancillary Rounded

        /// <summary>
        /// Get the Ancillary Bin Size in meters.
        /// Round to 3 decimal places.
        /// </summary>
        public string BinSizeRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.BinSize.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary First Bin Range in meters.
        /// Round to 3 decimal places.
        /// </summary>
        public string FirstBinRangeRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.FirstBinRange.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary First Profile ping time in seconds.
        /// Round to 2 decimal places.
        /// </summary>
        public string FirstPingTimeRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.FirstPingTime.ToString("0.00"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Last Profile ping time in seconds.
        /// Round to 2 decimal places.
        /// </summary>
        public string LastPingTimeRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.LastPingTime.ToString("0.00"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Heading in degrees.
        /// Round to 3 decimal places.
        /// </summary>
        public string HeadingRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.Heading.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Pitch in degrees.
        /// Round to 3 decimal places.
        /// </summary>
        public string PitchRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.Pitch.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Roll in degrees.
        /// Round to 3 decimal places.
        /// </summary>
        public string RollRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.Roll.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Water temperature in degrees.
        /// Round to 3 decimal places.
        /// </summary>
        public string WaterTempRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.WaterTemp.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Salinity in parts per thousand.
        /// Round to 3 decimal places.
        /// </summary>
        public string SalinityRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.Salinity.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Round version of the system temperature in degrees.
        /// If the temperature is bad, do not display anything.
        /// Round to 3 decimal places.
        /// </summary>
        public string SystemTempRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    if (_DisplayEnsemble.AncillaryData.SystemTemp == DataSet.AncillaryDataSet.BAD_SYS_TEMP)
                    {
                        return "";
                    }

                    return (_DisplayEnsemble.AncillaryData.SystemTemp.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Pressure in pascal.
        /// Round to 3 decimal places.
        /// </summary>
        public string PressureRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.Pressure.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Depth of the tranducer into the water in meters.
        /// Round to 3 decimal places.
        /// </summary>
        public string TransducerDepthRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.TransducerDepth.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Get the Ancillary Depth of the Speed of Sound in meters/sec.
        /// Round to 3 decimal places.
        /// </summary>
        public string SpeedOfSoundRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsAncillaryAvail)
                {
                    return (_DisplayEnsemble.AncillaryData.SpeedOfSound.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region System Setup

        /// <summary>
        /// Voltage of the ADCP.
        /// </summary>
        public string VoltageRounded
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsSystemSetupAvail)
                {
                    return (_DisplayEnsemble.SystemSetupData.Voltage.ToString("0.000"));
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Nmea Properties

        /// <summary>
        /// Determine if there is a GPS fix.
        /// </summary>
        private string _gpsFix;
        /// <summary>
        /// GPS fix property.
        /// </summary>
        public string GpsFix
        {
            get { return _gpsFix; }
            set
            {
                _gpsFix = value;
                this.NotifyOfPropertyChange(() => this.GpsFix);
            }
        }

        /// <summary>
        /// Latitude based off GPS data.
        /// </summary>
        private string _gpsLatitude;
        /// <summary>
        /// Latitude property.
        /// </summary>
        public string GpsLatitude
        {
            get { return _gpsLatitude; }
            set
            {
                _gpsLatitude = value;
                this.NotifyOfPropertyChange(() => this.GpsLatitude);
            }
        }

        /// <summary>
        /// GPS Longitude based off GPS data.
        /// </summary>
        private string _gpsLongitude;
        /// <summary>
        /// Longitude property.
        /// </summary>
        public string GpsLongitude
        {
            get { return _gpsLongitude; }
            set
            {
                _gpsLongitude = value;
                this.NotifyOfPropertyChange(() => this.GpsLongitude);
            }
        }

        /// <summary>
        /// Altitude based off GPS data.
        /// </summary>
        private string _gpsAltitude;
        /// <summary>
        /// Alitude property.
        /// </summary>
        public string GpsAltitude
        {
            get { return _gpsAltitude; }
            set
            {
                _gpsAltitude = value;
                this.NotifyOfPropertyChange(() => this.GpsAltitude);
            }
        }

        /// <summary>
        /// Speed based off GPS data.
        /// </summary>
        private string _gpsSpeed;
        /// <summary>
        /// Speed property.
        /// </summary>
        public string GpsSpeed
        {
            get { return _gpsSpeed; }
            set
            {
                _gpsSpeed = value;
                this.NotifyOfPropertyChange(() => this.GpsSpeed);
            }
        }

        /// <summary>
        /// Heading based off GPS data.
        /// </summary>
        private string _gpsHeading;
        /// <summary>
        /// Heading property.
        /// </summary>
        public string GpsHeading
        {
            get { return _gpsHeading; }
            set
            {
                _gpsHeading = value;
                this.NotifyOfPropertyChange(() => this.GpsHeading);
            }
        }

        #endregion

        #region DVL data

        /// <summary>
        /// The leak detection value.
        /// </summary>
        public string LeakDetection
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsDvlDataAvail)
                {
                    return (_DisplayEnsemble.DvlData.LeakDetectionToString());
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #region Ping Data

        /// <summary>
        /// Lag of the ADCP ping.
        /// </summary>
        public string Lag
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsSystemSetupAvail)
                {
                    return (_DisplayEnsemble.SystemSetupData.WpLagSamples.ToString());
                }
                else if (_DisplayEnsemble != null && _DisplayEnsemble.IsProfileEngineeringAvail)
                {
                    return (_DisplayEnsemble.ProfileEngineeringData.LagSamples.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// CPCE of the ADCP ping.
        /// </summary>
        public string CPCE
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsSystemSetupAvail)
                {
                    return (_DisplayEnsemble.SystemSetupData.WpCPCE.ToString());
                }
                else if (_DisplayEnsemble != null && _DisplayEnsemble.IsProfileEngineeringAvail)
                {
                    return (_DisplayEnsemble.ProfileEngineeringData.CPCE.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// NCE of the ADCP ping.
        /// </summary>
        public string NCE
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsSystemSetupAvail)
                {
                    return (_DisplayEnsemble.SystemSetupData.WpNCE.ToString());
                }
                else if (_DisplayEnsemble != null && _DisplayEnsemble.IsProfileEngineeringAvail)
                {
                    return (_DisplayEnsemble.ProfileEngineeringData.NCE.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// RepeatN of the ADCP ping.
        /// </summary>
        public string RepeatN
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsSystemSetupAvail)
                {
                    return (_DisplayEnsemble.SystemSetupData.WpRepeatN.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// Gap of the ADCP ping.
        /// </summary>
        public string Gap
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsProfileEngineeringAvail)
                {
                    return (_DisplayEnsemble.ProfileEngineeringData.PrePingGap.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        /// <summary>
        /// Gain of the ADCP ping.
        /// </summary>
        public string Gain
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsProfileEngineeringAvail)
                {
                    return (_DisplayEnsemble.ProfileEngineeringData.TRHighGain.ToString());
                }
                else
                {
                    return "0";
                }
            }
        }

        #endregion

        #region ADCP Orientation

        /// <summary>
        /// Display the orientation of the ADCP.
        /// </summary>
        public string AdcpOrientation
        {
            get
            {
                if (_DisplayEnsemble != null && _DisplayEnsemble.IsEnsembleAvail)
                {
                    if(_DisplayEnsemble.AncillaryData.IsUpwardFacing())
                    {
                        return "Upward Facing";
                    }

                    return "Downward Facing";
                }
                else
                {
                    return "";
                }
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Display the ensemble information.
        /// <param name="config">Configuration containing data source and SubsystemConfiguration.</param>
        /// </summary>
        public ViewDataTextEnsembleViewModel(SubsystemDataConfig config) 
            : base("ViewDataTextEnsembleViewModel")
        {
            // Set Subsystem 
            _Config = config;

            // Get PulseManager
            _pm = IoC.Get<PulseManager>();
            _pm.RegisterDisplayVM(this);

            // Set GPS data default
            SetGpsDataDefault();

            // Update the display
            _displayTimer = new System.Timers.Timer(500);
            _displayTimer.Elapsed += _displayTimer_Elapsed;
            _displayTimer.AutoReset = true;
            _displayTimer.Enabled = true;
        }

        /// <summary>
        /// Shutdown this object.
        /// </summary>
        public override void Dispose()
        {
            _displayTimer.Dispose();
        }

        #region Update Display

        /// <summary>
        /// Reduce the number of times the display is updated.
        /// This will update the display based off the timer and not
        /// based off when data is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _displayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Update the rounded values
            UpdateDisplay();
        }

        #endregion

        #region Recieve Ensemble

        /// <summary>
        /// Display the ensemble data.
        /// </summary>
        /// <param name="ensemble">Ensemble to display.</param>
        public void ReceiveEnsemble(DataSet.Ensemble ensemble)
        {
            // Set the display ensemble
            _DisplayEnsemble = ensemble;

            // Set GPS data
            SetGpsData(ensemble);
        }

        #endregion

        #region Update Display

        /// <summary>
        /// When a new dataset is received,
        /// notify that all these properties have
        /// changed.
        /// </summary>
        private void UpdateDisplay()
        {
            //this.NotifyOfPropertyChange(() => this.DataSetString);
            //this.NotifyOfPropertyChange(() => this.VoltageRounded);

            //this.NotifyOfPropertyChange(() => this.Subsystems);
            //this.NotifyOfPropertyChange(() => this.SystemSerialNumber);
            //this.NotifyOfPropertyChange(() => this.Firmware);

            //#region Ancillary Rounding
            //this.NotifyOfPropertyChange(() => this.BinSizeRounded);
            //this.NotifyOfPropertyChange(() => this.FirstBinRangeRounded);
            //this.NotifyOfPropertyChange(() => this.FirstPingTimeRounded);
            //this.NotifyOfPropertyChange(() => this.LastPingTimeRounded);
            //this.NotifyOfPropertyChange(() => this.HeadingRounded);
            //this.NotifyOfPropertyChange(() => this.PitchRounded);
            //this.NotifyOfPropertyChange(() => this.RollRounded);
            //this.NotifyOfPropertyChange(() => this.WaterTempRounded);
            //this.NotifyOfPropertyChange(() => this.SalinityRounded);
            //this.NotifyOfPropertyChange(() => this.SystemTempRounded);
            //this.NotifyOfPropertyChange(() => this.PressureRounded);
            //this.NotifyOfPropertyChange(() => this.TransducerDepthRounded);
            //this.NotifyOfPropertyChange(() => this.SpeedOfSoundRounded);

            //#endregion

            //this.NotifyOfPropertyChange(() => this.LeakDetection);
            this.NotifyOfPropertyChange(null);

        }

        /// <summary>
        /// Clear the display of all data.
        /// This is used when a new project is selected
        /// and the old data needs to be wiped from
        /// the screen.
        /// </summary>
        private void ClearDisplay()
        {
            // Clear the data
            _DisplayEnsemble = null;
            UpdateDisplay();

            // Clear the GPS data
            SetGpsDataDefault();
        }

        #endregion

        #region NMEA data

        /// <summary>
        /// Set the default value for all the GPS values.
        /// This will set all values to empty.
        /// </summary>
        private void SetGpsDataDefault()
        {
            GpsFix = DotSpatial.Positioning.FixQuality.NoFix.ToString();
            GpsLatitude = DotSpatial.Positioning.Latitude.Empty.ToString();
            GpsLongitude = DotSpatial.Positioning.Longitude.Empty.ToString();
            GpsAltitude = DotSpatial.Positioning.Distance.Empty.ToString();
            GpsSpeed = DotSpatial.Positioning.Speed.Empty.ToString();
        }

        /// <summary>
        /// Set the GPS data based off the NMEA data.
        /// </summary>
        /// <param name="adcpData">Ensemble containing NMEA data.</param>
        private void SetGpsData(DataSet.Ensemble adcpData)
        {
            // Check for NMEA data
            if (adcpData != null && adcpData.IsNmeaAvail)
            {
                // Check for GGA data
                if (adcpData.NmeaData.IsGpggaAvail())
                {
                    GpsFix = adcpData.NmeaData.GPGGA.FixQuality.ToString();

                    if (GpsFix == DotSpatial.Positioning.FixQuality.NoFix.ToString())
                    {
                        // No fix so set place holder, values would be NaN
                        GpsLatitude = "-";
                        GpsLongitude = "-";
                        GpsAltitude = "-";
                    }
                    else
                    {
                        // Set actual values
                        GpsLatitude = adcpData.NmeaData.GPGGA.Position.Latitude.ToString();
                        GpsLongitude = adcpData.NmeaData.GPGGA.Position.Longitude.ToString();
                        GpsAltitude = adcpData.NmeaData.GPGGA.Altitude.ToString();
                    }
                }
                else
                {
                    GpsFix = DotSpatial.Positioning.FixQuality.NoFix.ToString();
                    GpsLatitude = DotSpatial.Positioning.Latitude.Empty.ToString();
                    GpsLongitude = DotSpatial.Positioning.Longitude.Empty.ToString();
                    GpsAltitude = DotSpatial.Positioning.Distance.Empty.ToString();
                }

                // Check for VTG data
                if (adcpData.NmeaData.IsGpvtgAvail())
                {
                    //if (MeasurementStandard == Core.Commons.MeasurementStandards.METRIC)
                    //{
                        GpsSpeed = adcpData.NmeaData.GPVTG.Speed.ToMetersPerSecond().ToString();
                    //}
                    //else
                    //{
                    //    GpsSpeed = adcpData.NmeaData.GPVTG.Speed.ToFeetPerSecond().ToString();
                    //}

                    // Check if the value is good
                    if (GpsSpeed == Double.NaN.ToString())
                    {
                        GpsSpeed = "-";
                    }
                }
                else
                {
                    GpsSpeed = DotSpatial.Positioning.Speed.Empty.ToString();
                }

                // Heading
                if(adcpData.NmeaData.IsGphdtAvail())
                {
                    GpsHeading = adcpData.NmeaData.GPHDT.Heading.DecimalDegrees.ToString() + "°";
                }
                else
                {
                    GpsHeading = "-";
                }
            }
            else
            {
                SetGpsDataDefault();
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handle event when EnsembleEvent is received.
        /// This will create the displays for each config
        /// if it has not been created already.  It will also
        /// display the latest ensemble.
        /// </summary>
        /// <param name="ensEvent">Ensemble event.</param>
        public override void Handle(EnsembleEvent ensEvent)
        {
            // Check if source matches this display
            if (_Config.Source != ensEvent.Source || ensEvent.Ensemble == null)
            {
                return;
            }


            // If no subsystem is given, then a project is not selected
            // So receive all data and display
            // If the serial number is not set, this may be an old ensemble
            // Try to display it anyway
            if (!_Config.SubSystem.IsEmpty() && !ensEvent.Ensemble.EnsembleData.SysSerialNumber.IsEmpty())
            {
                // Verify the subsystem matches this viewmodel's subystem.
                if ((_Config.SubSystem == ensEvent.Ensemble.EnsembleData.GetSubSystem())        // Check if Subsystem matches 
                        && (_Config == ensEvent.Ensemble.EnsembleData.SubsystemConfig))         // Check if Subsystem Config matches
                {
                    // Receive the ensemble
                    ReceiveEnsemble(ensEvent.Ensemble);
                }
            }

        }

        /// <summary>
        /// Bulk Ensemble event.
        /// </summary>
        /// <param name="ensEvent"></param>
        public override void Handle(BulkEnsembleEvent ensEvent)
        {
            // DO NOTHING
        }

        /// <summary>
        /// Display the selected ensemble information.
        /// </summary>
        /// <param name="ensEvent"></param>
        public void Handle(SelectedEnsembleEvent ensEvent)
        {
            // Check if source matches this display
            if (_Config.Source != ensEvent.Source || ensEvent.Ensemble == null)
            {
                return;
            }

            // Receive the ensemble
            ReceiveEnsemble(ensEvent.Ensemble);
        }

        #endregion



    }
}
