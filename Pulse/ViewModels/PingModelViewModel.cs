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
 * 01/03/2014      RC          3.2.3      Initial coding
 * 02/18/2014      RC          3.2.3      Fixed spacing for TotalPingTime in AddTotalConfigTime().
 * 
 * 
 * 
 */

namespace RTI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using System.Globalization;

    /// <summary>
    /// Display the Ping Model.
    /// </summary>
    public class PingModelViewModel : PulseViewModel
    {
        #region Variables

        #region Defaults

        /// <summary>
        /// Default Canvas Ping Timing height.
        /// </summary>
        private int DEFAULT_PING_TIMING_CANVAS_HEIGHT = 512;

        /// <summary>
        /// Default canvas width.
        /// </summary>
        private int DEFAULT_CANVAS_WIDTH = 5120;


        #endregion

        #endregion


        #region Properties

        /// <summary>
        /// Ping Model Canvas.  This is the main canvas.
        /// </summary>
        public Canvas PingModelCanvas { get; set; }

        /// <summary>
        /// Timing Canvas.  This will display all the
        /// timing diagrams.
        /// </summary>
        public Canvas PingTimingCanvas { get; set; }

        /// <summary>
        /// Timing Canvas.  This will display all the
        /// timing diagrams.
        /// </summary>
        public Canvas PingTimingTotalCanvas { get; set; }

        #endregion

        /// <summary>
        /// Ininitialize the Ping Model.
        /// </summary>
        public PingModelViewModel()
            : base("Ping Model")
        {
            // Main Canvas
            PingModelCanvas = new Canvas();
            PingModelCanvas.Width = DEFAULT_CANVAS_WIDTH;
            PingModelCanvas.Height = DEFAULT_PING_TIMING_CANVAS_HEIGHT;

            // Canvas for timing model
            PingTimingCanvas = new Canvas();                                                    // Create canvas
            PingTimingCanvas.Width = DEFAULT_CANVAS_WIDTH;                                      // Set canvas width
            PingTimingCanvas.Height = DEFAULT_PING_TIMING_CANVAS_HEIGHT;                        // Set canvas height
            Canvas.SetLeft(PingTimingCanvas, 0);                                                // Set canvas position width
            Canvas.SetTop(PingTimingCanvas, 0);                                                 // Set canvas position height
            PingModelCanvas.Children.Add(PingTimingCanvas);                                     // Add to main canvas

            // Canvas for timing model
            PingTimingTotalCanvas = new Canvas();                                                    // Create canvas
            PingTimingTotalCanvas.Width = DEFAULT_CANVAS_WIDTH;                                      // Set canvas width
            PingTimingTotalCanvas.Height = 60;                                                       // Set canvas height
            //Canvas.SetLeft(PingTimingTotalCanvas, 0);                                                // Set canvas position width
            //Canvas.SetTop(PingTimingTotalCanvas, 0);                           // Set canvas position height
            //PingModelCanvas.Children.Add(PingTimingTotalCanvas);                                     // Add to main canvas



            //// Red rectangle from the point P1(2, 4) that is 10px wide and 6px high
            ////PingModelImg.DrawRectangle(0, 0, 512, 512, Colors.Red);
            //PingModelImg.FillRectangle(0, 0, 5120, 512, Colors.Orange);
            //PingModelImg.FillRectangle(0, 0, 20, 200, Colors.Red);
            //PingModelImg.FillRectangle(30, 0, 50, 200, Colors.Red);
            //PingModelImg.FillRectangle(60, 0, 80, 200, Colors.Red);

            //int numConfigs = 2;

            //PingModelImg.FillRectangle(0, 0, (int)PingModelImg.Width / numConfigs, (int)PingModelImg.Height / numConfigs, Colors.Red);
            //PingModelImg.FillRectangle((int)PingModelImg.Width / numConfigs, ((int)PingModelImg.Height / numConfigs) - 20, ((int)PingModelImg.Width / numConfigs) + 100, (int)PingModelImg.Height / numConfigs, Colors.Black);

            //AddConfig(0, 2, 0, 0);
            //AddConfig(1, 2, 0, 0);

            ////PingModelImg.Invalidate();

            ////TextBlock lbl = new TextBlock();
            ////lbl.Text = "Hello";
            ////lbl.FontSize = 15;
            ////lbl.Foreground = new SolidColorBrush(Colors.Blue);
            ////WriteableBitmap tBmp = new WriteableBitmap(lbl);
            ////PingModelImg.Blit(new Point(0,0), tBmp, new Rect(0, 0, tBmp.PixelWidth, tBmp.PixelHeight), Colors.White, System.Windows.Media.Imaging.WriteableBitmapExtensions.BlendMode.Alpha);

            //TextBlock i = new TextBlock();
            //i.Text = "Hello";
            //i.Foreground = new SolidColorBrush(Colors.Blue);
            ////PingModelImg.Render(i, new TranslateTransform() { X = 0, Y = 0 });

            //string testString = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor";

            //// Create the initial formatted text string.
            //FormattedText formattedText = new FormattedText(
            //    testString,
            //    CultureInfo.GetCultureInfo("en-us"),
            //    FlowDirection.LeftToRight,
            //    new Typeface("Verdana"),
            //    32,
            //    Brushes.Black);

            //// The Visual to use as the source of the RenderTargetBitmap.
            //DrawingVisual drawingVisual = new DrawingVisual();
            //DrawingContext drawingContext = drawingVisual.RenderOpen();
            ////drawingContext.DrawImage(PingModelImg, new Rect(0, 0, PingModelImg.Width, PingModelImg.Height));
            //drawingContext.DrawText(formattedText, new Point(PingModelImg.Height / 2, 0));
            //drawingContext.Close();

            //// The BitmapSource that is rendered with a Visual.
            //RenderTargetBitmap rtb = new RenderTargetBitmap(PingModelImg.PixelWidth, PingModelImg.PixelHeight, 96, 96, PixelFormats.Pbgra32);
            //rtb.Render(drawingVisual);
           

        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        public override void Dispose()
        {

        }

        /// <summary>
        /// Add a ADCP configuration.
        /// </summary>
        /// <param name="config">ADCP Configuration.</param>
        public void AddConfiguration(AdcpConfiguration config)
        {
            // Pixels per second
            double totalPingTime = TotalPingTime(config);

            // Ensure the image is large enough
            if (totalPingTime > PingModelCanvas.Width)
            {
                PingTimingCanvas.Width = (int)totalPingTime;
                PingModelCanvas.Width = (int)totalPingTime;
            }

            // Determine how many pixels per second should be used
            //int pixelsPerSecond = (int)Math.Round(PingModelImg.Width / totalPingTime);
            double pixelsPerSecond = PingModelCanvas.Width / totalPingTime;

            // Add all the configurations to the image
            double startPixel = 0;
            foreach (var ssConfig in config.SubsystemConfigDict.Values)
            {
                //AddConfig(ssConfig, config.SubsystemConfigDict.Count, config.Commands.CEI.ToSeconds());
                //startPixel += AddConfigStaggered(ssConfig, config.SubsystemConfigDict.Count, config.Commands.CEI.ToSeconds(), pixelsPerSecond, startPixel * pixelsPerSecond);
                startPixel += AddConfigCanvas(ssConfig, config.SubsystemConfigDict.Count, config.Commands.CEI.ToSeconds(), pixelsPerSecond, startPixel * pixelsPerSecond);

            }

            // Add Total Config time label
            AddTotalConfigTime(totalPingTime);

        }

        /// <summary>
        /// Add a configuration to the canvas.  Give the subsystem configuration and 
        /// add it to the list of configurations.  This will add the ping time and the
        /// CEI timing.
        /// </summary>
        /// <param name="ssConfig">Subsystem configuration.</param>
        /// <param name="numConfigs">Number of configurations.</param>
        /// <param name="CEI">CEI time.</param>
        /// <param name="pixelsPerSecond">Number of pixels per second.</param>
        /// <param name="startPixel">Start pixel.</param>
        /// <returns>Total time included CEI.</returns>
        private double AddConfigCanvas(AdcpSubsystemConfig ssConfig, int numConfigs, int CEI, double pixelsPerSecond, double startPixel)
        {
            // Configuration number to know  the location to put the model
            // and to label the model
            int configNum = ssConfig.SubsystemConfig.CepoIndex;

            // Ensemble Length
            double pingTime = ssConfig.Commands.CWPTBP * ssConfig.Commands.CWPP;
            double ensembleLength = pingTime * pixelsPerSecond;

            // CEI length
            double ceiLength = CEI * pixelsPerSecond;

            // Width
            // Based off the number of configurations
            double startWidth = startPixel;

            // Height 
            // Start at top and go down half way
            double startHeight = (PingModelCanvas.Height / numConfigs) * configNum; ;
            double endHeight = (PingModelCanvas.Height / numConfigs);

            // Ensemble
            double widthEnsStart = startWidth;
            double heightEnsStart = startHeight;
            double widthEnsEnd = widthEnsStart + ensembleLength;
            double heightEnsEnd = endHeight;

            // CEI
            double widthCeiStart = widthEnsEnd;
            double heightCeiStart = startHeight;
            double widthCeiEnd = widthCeiStart + ceiLength;
            double heightCeiEnd = endHeight;


            // Gradient
            LinearGradientBrush linGrBrush = new LinearGradientBrush(Color.FromArgb(255, 29, 29, 29),
                                                                       Color.FromArgb(255,14, 14, 14),
                                                                       90);

            // Add Ensemble
            System.Windows.Shapes.Rectangle rectEns;
            rectEns = new System.Windows.Shapes.Rectangle();                           // Create a rectangle
            rectEns.Stroke = new SolidColorBrush(Colors.Black);                        // Border color
            rectEns.StrokeThickness = 10; 
            rectEns.Fill = linGrBrush;
            rectEns.Width = ensembleLength;                                            // Ensemble Length
            rectEns.Height = endHeight;                                                // Ensemble Height
            Canvas.SetLeft(rectEns, widthEnsStart);                                    // Location of the rectangle Width
            Canvas.SetTop(rectEns, heightEnsStart);                                    // Location of the rectangle Height
            PingTimingCanvas.Children.Add(rectEns);                                    // Add the rectangle to the canvas

            // Add CEI
            System.Windows.Shapes.Rectangle rectCEI;
            rectCEI = new System.Windows.Shapes.Rectangle();                        // Create a rectangle
            rectCEI.Stroke = new SolidColorBrush(Colors.Gray);                     // Border color
            rectCEI.StrokeThickness = 10;                                           // Thicker border
            rectCEI.Fill = new SolidColorBrush(Colors.Transparent);                 // No fill color
            rectCEI.Width = ceiLength;                                              // CEI Length
            rectCEI.Height = endHeight;                                             // CEI height
            Canvas.SetLeft(rectCEI, widthCeiStart);                                 // Location of the rectangle Width
            Canvas.SetTop(rectCEI, heightCeiStart);                                 // Location of the rectangle Height
            PingTimingCanvas.Children.Add(rectCEI);                                 // Add the rectangle to the canvas

            // Add the Ensemble Length labels
            TextBlock textBlockEns = new TextBlock();
            textBlockEns.Text = "[" + ssConfig.SubsystemConfig.CepoIndex + "] " + pingTime.ToString("0.000") + " sec";
            textBlockEns.FontSize = 40;                                             // Font size
            Canvas.SetLeft(textBlockEns, widthEnsStart + (ensembleLength / 2));     // Put in the middle of the box Width
            Canvas.SetTop(textBlockEns, heightEnsStart + (endHeight));              // Put in the middle of the box Height
            PingTimingCanvas.Children.Add(textBlockEns);                            // Add the text to the canvas

            // Add the CEI Length labels
            TextBlock textBlockCei = new TextBlock();
            textBlockCei.Text =  CEI.ToString("0.000") + " sec";
            textBlockCei.FontSize = 40;                                             // Font size
            Canvas.SetLeft(textBlockCei, widthCeiStart + (ceiLength / 2));          // Put in the middle of the box Width
            Canvas.SetTop(textBlockCei, heightCeiStart + (endHeight));              // Put in the middle of the box Height
            PingTimingCanvas.Children.Add(textBlockCei);                            // Add the text to the canvas

            // Return the last position
            return pingTime + CEI;
        }

        /// <summary>
        /// Get the total configuration time.  This will use accumulated
        /// ping time given to get the total configuation time.
        /// </summary>
        /// <param name="totalPingTime">Total ping time to display.</param>
        private void AddTotalConfigTime(double totalPingTime)
        {
            // Add the text labels
            TextBlock textBlock = new TextBlock();
            textBlock.Text = totalPingTime.ToString("0.000") + " seconds";
            textBlock.FontSize = 60;
            textBlock.Foreground = new SolidColorBrush(Color.FromArgb(0xFF,0xD9,0xE5,0x0F));
            textBlock.FontWeight = FontWeights.Bold;
            //Canvas.SetLeft(textBlock, PingTimingCanvas.Width / 2);
            //Canvas.SetTop(textBlock, PingTimingCanvas.Height);
            //PingTimingCanvas.Children.Add(textBlock);

            Canvas.SetLeft(textBlock, PingTimingTotalCanvas.Width / 2);
            Canvas.SetTop(textBlock, 0);
            PingTimingTotalCanvas.Children.Add(textBlock);
        }

        /// <summary>
        /// Get the total time it will take to ping for a complete cycle of all the
        /// subsystem configurations.
        /// </summary>
        /// <param name="config">Adcp Configuration.</param>
        /// <returns>Total time to ping.</returns>
        private double TotalPingTime(AdcpConfiguration config)
        {
            double pingTime = 0;

            // Accumulate all the ping time length and CEI for each config
            foreach (var ssConfig in config.SubsystemConfigDict.Values)
            {
                pingTime += ssConfig.Commands.CWPTBP * ssConfig.Commands.CWPP;
                pingTime += config.Commands.CEI.ToSecondsD();
            }

            return pingTime;
        }


        #region Bitmap

        //private int AddConfig(AdcpSubsystemConfig ssConfig, int numConfigs, int CEI, int pixelsPerSecond, int startPixel)
        //{
        //    int configNum = ssConfig.SubsystemConfig.CepoIndex;

        //    // Ensemble Length
        //    double pingTime = ssConfig.Commands.CWPTBP * ssConfig.Commands.CWPP;
        //    int ensembleLength = (int)(pingTime * pixelsPerSecond);

        //    // CEI length
        //    int ceiLength = CEI * pixelsPerSecond;
        //    //int CeiHeight = 20;


        //    // Width
        //    // Based off the number of configurations
        //    //int startWidth = (((int)PingModelImg.Width / numConfigs) * configNum);
        //    int startWidth = startPixel;

        //    // Height 
        //    // Start at top and go down half way
        //    int startHeight = 0;
        //    int endHeight = ((int)PingModelCanvas.Height / 2);

        //    // Ensemble
        //    int widthEnsStart = startWidth;
        //    int heightEnsStart = startHeight;
        //    int widthEnsEnd = widthEnsStart + ensembleLength;
        //    int heightEnsEnd = endHeight;

        //    // CEI
        //    int widthCeiStart = widthEnsEnd;
        //    //int heightCeiStart = endHeight - CeiHeight;
        //    int heightCeiStart = startHeight;
        //    int widthCeiEnd = widthCeiStart + ceiLength;
        //    int heightCeiEnd = endHeight;


        //    // Add Ensemble
        //    PingModelImg.FillRectangle(widthEnsStart, heightEnsStart, widthEnsEnd, heightEnsEnd, Colors.Red);
        //    PingModelImg.FillRectangle(widthCeiStart, heightCeiStart, widthCeiEnd, heightCeiEnd, Colors.Black);

        //    return (int)(pingTime + CEI);
        //}

        //private int AddConfigStaggered(AdcpSubsystemConfig ssConfig, int numConfigs, int CEI, int pixelsPerSecond, int startPixel)
        //{
        //    int configNum = ssConfig.SubsystemConfig.CepoIndex;

        //    // Ensemble Length
        //    double pingTime = ssConfig.Commands.CWPTBP * ssConfig.Commands.CWPP;
        //    int ensembleLength = (int)(pingTime * pixelsPerSecond);

        //    // CEI length
        //    int ceiLength = CEI * pixelsPerSecond;
        //    //int CeiHeight = 20;


        //    // Width
        //    // Based off the number of configurations
        //    //int startWidth = (((int)PingModelImg.Width / numConfigs) * configNum);
        //    int startWidth = startPixel;

        //    // Height 
        //    // Start at top and go down half way
        //    int startHeight = (int)((PingModelImg.Height / numConfigs) * configNum); ;
        //    int endHeight = (int)((PingModelImg.Height / numConfigs) * (configNum + 1));

        //    // Ensemble
        //    int widthEnsStart = startWidth;
        //    int heightEnsStart = startHeight;
        //    int widthEnsEnd = widthEnsStart + ensembleLength;
        //    int heightEnsEnd = endHeight;

        //    // CEI
        //    int widthCeiStart = widthEnsEnd;
        //    //int heightCeiStart = endHeight - CeiHeight;
        //    int heightCeiStart = startHeight;
        //    int widthCeiEnd = widthCeiStart + ceiLength;
        //    int heightCeiEnd = endHeight;


        //    // Add Ensemble
        //    PingModelImg.FillRectangle(widthEnsStart, heightEnsStart, widthEnsEnd, heightEnsEnd, Colors.Red);
        //    PingModelImg.FillRectangle(widthCeiStart, heightCeiStart, widthCeiEnd, heightCeiEnd, Colors.Black);

        //    return (int)(pingTime + CEI);
        //}

        //private void AddConfig(AdcpSubsystemConfig ssConfig, int numConfigs, int CEI)
        //{
        //    int configNum = ssConfig.SubsystemConfig.CepoIndex;

        //    int CeiWidth = 100;
        //    int CeiHeight = 20;
            

        //    // 
        //    int ceiOffsetWidth = CeiWidth;


        //    int totalWidth = ((int)PingModelImg.Width / numConfigs);
            
        //    double pingTime = ssConfig.Commands.CWPTBP * ssConfig.Commands.CWPP;
        //    double pingRatioToCei = pingTime / CEI;

        //    // Ensemble Length
        //    int ensembleLength = (int)(totalWidth * pingRatioToCei);

        //    // CEI length
        //    int ceiLength = totalWidth - ensembleLength;

        //    // Width
        //    // Based off the number of configurations
        //    int startWidth = (((int)PingModelImg.Width / numConfigs) * configNum);
        //    //int endWidth = (((int)PingModelImg.Width / numConfigs) * (configNum + 1)) - ceiOffsetWidth;

        //    // Height 
        //    // Start at top and go down half way
        //    int startHeight = 0;
        //    int endHeight = ((int)PingModelImg.Height / 2);

        //    // Ensemble
        //    int widthEnsStart = startWidth;
        //    int heightEnsStart = startHeight;
        //    //int widthEnsEnd = endWidth;
        //    int widthEnsEnd = widthEnsStart + ensembleLength;
        //    int heightEnsEnd = endHeight;

        //    // CEI
        //    int widthCeiStart = widthEnsEnd;
        //    int heightCeiStart = endHeight - CeiHeight;
        //    //int widthCeiEnd = endWidth + CeiWidth;
        //    int widthCeiEnd = widthCeiStart + ceiLength;
        //    int heightCeiEnd = endHeight;


        //    // Add Ensemble
        //    PingModelImg.FillRectangle(widthEnsStart, heightEnsStart, widthEnsEnd, heightEnsEnd, Colors.Red);
        //    //PingModelImg.FillRectangle(widthCeiStart, ((int)PingModelImg.Height / numConfigs) - 20, ((int)PingModelImg.Width / numConfigs) + 100, (int)PingModelImg.Height / numConfigs, Colors.Black);
        //    PingModelImg.FillRectangle(widthCeiStart, heightCeiStart, widthCeiEnd, heightCeiEnd, Colors.Black);
        //}
        #endregion

    }
}
