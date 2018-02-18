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
//using System.Windows;
//using System.Windows.Media;

//namespace RTI
//{

//    /// <summary>
//    /// Test the DownloadFileOptions object.
//    /// This object is used to store the DownloadFile options.
//    /// </summary>
//    [TestFixture]
//    public class StatusEventTest
//    {
//        /// <summary>
//        /// Test only setting the message.
//        /// Default value for color and duration.
//        /// </summary>
//        [Test]
//        public void Test()
//        {
//            StatusEvent se = new StatusEvent("Message");

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_BLUE, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage None.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageNone()
//        {
//            StatusEvent se = new StatusEvent("Message",  MessageBoxImage.None);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_BLUE, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Asterisk.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageAsterisk()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Asterisk);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_BLUE, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Error.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageError()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Error);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_RED, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Exclamation.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageExclamation()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Exclamation);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_YELLOW, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Hand.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageHand()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Hand);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_RED, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Information.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageInformation()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Information);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_BLUE, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Question.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageQuestion()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Question);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_BLUE, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Stop.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageStop()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Stop);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_RED, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test MessageBoxImage Warning.
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageWarning()
//        {
//            StatusEvent se = new StatusEvent("Message", MessageBoxImage.Warning);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(StatusEvent.COLOR_YELLOW, se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test using a color value
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageColor()
//        {
//            StatusEvent se = new StatusEvent("Message", Colors.Black);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(Colors.Black.ToString(), se.Color, "Color Incorrect");
//            Assert.AreEqual("#FF000000", se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test using a color value
//        /// </summary>
//        [Test]
//        public void TestMessageBoxImageColor1()
//        {
//            StatusEvent se = new StatusEvent("Message", Colors.Orange);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(Colors.Orange.ToString(), se.Color, "Color Incorrect");
//            Assert.AreEqual("#FFFFA500", se.Color, "Color Incorrect");
//            Assert.AreEqual(StatusEvent.DEFAULT_DURATION, se.Duration, "Duration Incorrect");
//        }

//        /// <summary>
//        /// Test using a color value
//        /// </summary>
//        [Test]
//        public void TestDuration()
//        {
//            StatusEvent se = new StatusEvent("Message", Colors.Orange, 100);

//            Assert.AreEqual("Message", se.Message, "Message Incorrect");
//            Assert.AreEqual(Colors.Orange.ToString(), se.Color, "Color Incorrect");
//            Assert.AreEqual("#FFFFA500", se.Color, "Color Incorrect");
//            Assert.AreEqual(100, se.Duration, "Duration Incorrect");
//        }
//    }
//}
