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
// * 11/29/2012      RC          2.17       Initial coding
// * 
// * 
// */

//namespace RTI
//{
//    using NUnit.Framework;
//    using System.Collections.ObjectModel;

//    /// <summary>
//    /// Test the SeriesType object.
//    /// This object describes all the different
//    /// series types and there connections together.
//    /// </summary>
//    [TestFixture]
//    public class SeriesTypeTest
//    {

//        #region Lists

//        /// <summary>
//        /// Test the List contains the correct items for Water Profile Series.
//        /// </summary>
//        [Test]
//        public void WaterProfileListTest()
//        {
//            var source = new DataSource(DataSource.eSource.WaterProfile);
//            var list = BaseSeriesType.GetBaseSeriesList(source.Source);

//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam)), "Beam Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_XYZ)), "XYZ Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_ENU)), "ENU Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude)), "Amplitude is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation)), "Correlation is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_SNR)), "SNR is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range)), "Range is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Heading)), "Heading is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pitch)), "Pitch is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Roll)), "Roll is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Sys)), "Sys Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Water)), "Water Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pressure)), "Pressure is missing.");
//            Assert.AreEqual(5, list.Count, "Count is incorrect.");
//        }

//        /// <summary>
//        /// Test the List contains the correct items for Water Profile Ancillary Series.
//        /// </summary>
//        [Test]
//        public void WaterProfileAncillaryList()
//        {
//            var source = new DataSource(DataSource.eSource.AncillaryWaterProfile);
//            var list = BaseSeriesType.GetBaseSeriesList(source.Source);

//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam)), "Beam Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_XYZ)), "XYZ Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_ENU)), "ENU Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude)), "Amplitude is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation)), "Correlation is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_SNR)), "SNR is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range)), "Range is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Heading)), "Heading is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pitch)), "Pitch is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Roll)), "Roll is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Sys)), "Sys Temperature is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Water)), "Water Temperature is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pressure)), "Pressure is missing.");
//            Assert.AreEqual(6, list.Count, "Count is incorrect.");
//        }

//        /// <summary>
//        /// Test the List contains the correct items for Bottom Track Series.
//        /// </summary>
//        [Test]
//        public void BottomTrackList()
//        {
//            var source = new DataSource(DataSource.eSource.BottomTrack);
//            var list = BaseSeriesType.GetBaseSeriesList(source.Source);

//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam)), "Beam Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_XYZ)), "XYZ Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_ENU)), "ENU Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude)), "Amplitude is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation)), "Correlation is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_SNR)), "SNR is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range)), "Range is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Heading)), "Heading is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pitch)), "Pitch is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Roll)), "Roll is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Sys)), "Sys Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Water)), "Water Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pressure)), "Pressure is missing.");
//            Assert.AreEqual(7, list.Count, "Count is incorrect.");
//        }

//        /// <summary>
//        /// Test the List contains the correct items for Bottom Track Ancillary Series.
//        /// </summary>
//        [Test]
//        public void BottomTrackAncillaryList()
//        {
//            var source = new DataSource(DataSource.eSource.AncillaryBottomTrack);
//            var list = BaseSeriesType.GetBaseSeriesList(source.Source);

//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam)), "Beam Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_XYZ)), "XYZ Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_ENU)), "ENU Velocity is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude)), "Amplitude is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation)), "Correlation is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_SNR)), "SNR is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range)), "Range is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Heading)), "Heading is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pitch)), "Pitch is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Roll)), "Roll is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Sys)), "Sys Temperature is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Water)), "Water Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pressure)), "Pressure is missing.");
//            Assert.AreEqual(5, list.Count, "Count is incorrect.");
//        }


//        /// <summary>
//        /// Test the List contains the correct items for Water Track Series.
//        /// </summary>
//        [Test]
//        public void WaterTrackList()
//        {
//            var source = new DataSource(DataSource.eSource.WaterTrack);
//            var list = BaseSeriesType.GetBaseSeriesList(source.Source);

//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_Beam)), "Beam Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_XYZ)), "XYZ Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Velocity_ENU)), "ENU Velocity is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Amplitude)), "Amplitude is missing.");
//            Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Correlation)), "Correlation is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_SNR)), "SNR is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Range)), "Range is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Heading)), "Heading is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pitch)), "Pitch is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Roll)), "Roll is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Sys)), "Sys Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Temperature_Water)), "Water Temperature is missing.");
//            //Assert.IsTrue(list.Contains(new BaseSeriesType(BaseSeriesType.eBaseSeriesType.Base_Pressure)), "Pressure is missing.");
//            Assert.AreEqual(5, list.Count, "Count is incorrect.");
//        }

        

//        #endregion

       
//    }
//}
