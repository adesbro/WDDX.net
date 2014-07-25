using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mueller.Wddx;
using Mueller.Wddx.Tests;
using NUnit.Framework;

namespace WDDX_Tests
{
    [TestFixture]
    public class TypedDeserializerTests
    {
        private const string BasicPacketFormatString = "<wddxPacket version=\"1.0\"><header /><data>{0}</data></wddxPacket>";

        private class TypedTestClass
        {
            public float NumberProp { get; set; }
            public bool BooleanProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public string StringProp { get; set; }
        }

        /// <summary>
        ///		Tests the deserialization of a basic WDDX struct into a typed instance.
        /// </summary>
        [Test]
        public void TestBasicGenericsDeserialization()
        {
            DateTime expectedDate = new DateTime(1998, 6, 12, 4, 32, 12, System.DateTimeKind.Local);

            string structPacket = String.Format(BasicPacketFormatString, "<struct><var name=\"NumberProp\"><number>-12.456</number></var><var name=\"BooleanProp\"><boolean value=\"true\" /></var><var name=\"DateTimeProp\"><dateTime>" + WddxTest.ISO8601DateFormatter(expectedDate) + "</dateTime></var><var name=\"aNull\"><null /></var><var name=\"StringProp\"><string>a string</string></var></struct>");

            WddxDeserializer deserializer = new WddxDeserializer();

            TypedTestClass typedTestClass = deserializer.Deserialize<TypedTestClass>(structPacket);
            Assert.AreEqual("a string", typedTestClass.StringProp);
            Assert.AreEqual(-12.456f, typedTestClass.NumberProp);
            Assert.AreEqual(true, typedTestClass.BooleanProp);
            Assert.AreEqual(expectedDate, typedTestClass.DateTimeProp);
        }

        [DataContract]
        private class TypedTestClass2
        {
            [DataMember(Name = "NumberProp")]
            public float NumberPropOveridden { get; set; }
            [DataMember(Name = "BooleanProp")]
            public bool BooleanPropOveridden { get; set; }
            public DateTime DateTimeProp { get; set; }
            public string StringProp { get; set; }
        }

        /// <summary>
        ///		Tests the deserialization of a basic WDDX struct into a typed instance with [DataMember] attributes used
        ///     to specify the names to deserialize to.
        /// </summary>
        [Test]
        public void TestBasicGenericsWithPartialDataMemberAttributeDeserialization()
        {
            DateTime expectedDate = new DateTime(1998, 6, 12, 4, 32, 12, System.DateTimeKind.Local);

            string structPacket = String.Format(BasicPacketFormatString, "<struct><var name=\"NumberProp\"><number>-12.456</number></var><var name=\"BooleanProp\"><boolean value=\"true\" /></var><var name=\"DateTimeProp\"><dateTime>" + WddxTest.ISO8601DateFormatter(expectedDate) + "</dateTime></var><var name=\"aNull\"><null /></var><var name=\"StringProp\"><string>a string</string></var></struct>");

            WddxDeserializer deserializer = new WddxDeserializer();

            TypedTestClass2 typedTestClass = deserializer.Deserialize<TypedTestClass2>(structPacket);
            Assert.AreEqual("a string", typedTestClass.StringProp);
            Assert.AreEqual(-12.456f, typedTestClass.NumberPropOveridden);
            Assert.AreEqual(true, typedTestClass.BooleanPropOveridden);
            Assert.AreEqual(expectedDate, typedTestClass.DateTimeProp);
        }

        private class TypedTestClass3
        {
            public int Int32Prop { get; set; }
            public string Int32AsStringProp { get; set; }
            public float NumberProp { get; set; }
            public string BooleanProp { get; set; }
            public string DateTimeProp { get; set; }
            public string StringProp { get; set; }
        }

        /// <summary>
        ///		Tests the deserialization of a basic WDDX struct into a typed instance with [DataMember] attributes used
        ///     to specify the names to deserialize to.
        /// </summary>
        [Test]
        public void TestBasicGenericsWithTypeConversionDeserialization()
        {
            DateTime expectedDate = new DateTime(1998, 6, 12, 4, 32, 12, System.DateTimeKind.Local);

            string structPacket = String.Format(BasicPacketFormatString, "<struct><var name=\"Int32Prop\"><number>1000</number></var><var name=\"Int32AsStringProp\"><number>1000</number></var><var name=\"NumberProp\"><number>-12.456</number></var><var name=\"BooleanProp\"><boolean value=\"true\" /></var><var name=\"DateTimeProp\"><dateTime>" + WddxTest.ISO8601DateFormatter(expectedDate) + "</dateTime></var><var name=\"aNull\"><null /></var><var name=\"StringProp\"><string>a string</string></var></struct>");

            WddxDeserializer deserializer = new WddxDeserializer();

            TypedTestClass3 typedTestClass = deserializer.Deserialize<TypedTestClass3>(structPacket);
            Assert.AreEqual(1000, typedTestClass.Int32Prop);
            Assert.AreEqual("1000", typedTestClass.Int32AsStringProp);
            Assert.AreEqual("a string", typedTestClass.StringProp);
            Assert.AreEqual(-12.456f, typedTestClass.NumberProp);
            Assert.AreEqual("True", typedTestClass.BooleanProp);
            Assert.AreEqual(expectedDate, DateTime.Parse(typedTestClass.DateTimeProp));
        }

        private class TypedTestClass4
        {
            public class PackageFile
            {
                public int FileSize { get; set; }
                [DataMember(Name = "Global_Flag")]
                public string GlobalFlag { get; set; }
                public string Url { get; set; }
                public string Fingerprint { get; set; }
            }

            public class Package
            {
                public string FileCount { get; set; }
                public string SentTime { get; set; }
                public PackageFile[] PackageFiles { get; set; }
                public string ExpiryTime { get; set; }
                public string SenderName { get; set; }
            }

            public string Id { get; set; }
            public string Result { get; set; }
            [DataMember(Name = "package_list")]
            public Dictionary<string, Package> Packages { get; set; }
        }

        [Test]
        public void TestComplexGenericsWithArrayDeserialization()
        {
            const string structPacket = @"
<wddxPacket version='1.0'>
   <header/>
   <data>
      <struct>
         <var name='Id'>
            <string>8888</string>
         </var>
         <var name='Result'>
            <string>Success</string>
         </var>
         <var name='package_list'>
            <struct>
               <var name='1037-z140558641359'>
                  <struct>
                     <var name='FileCount'>
                        <string>1</string>
                     </var>
                     <var name='SentTime'>
                        <string>1405586413</string>
                     </var>
                     <var name='PackageFiles'>
                        <array length='1'>
                           <struct>
                              <var name='FileSize'>
                                 <number>15525</number>
                              </var>
                              <var name='Global_Flag'>
                                 <string>4</string>
                              </var>
                              <var name='Url'>
                                 <string>https://testurl/monkey.kpg</string>
                              </var>
                              <var name='Fingerprint'>
                                 <string>c85c136975fe355c75cea70474c0f180-15525</string>
                              </var>
                           </struct>
                        </array>
                     </var>
                     <var name='ExpiryTime'>
                        <string>1406203199</string>
                     </var>
                     <var name='SenderName'>
                        <string>Bob Smith</string>
                     </var>
                  </struct>
               </var>
               <var name='1037-w20140717084438533'>
                  <struct>
                     <var name='FileCount'>
                        <string>1</string>
                     </var>
                     <var name='SentTime'>
                        <string>1405586737</string>
                     </var>
                     <var name='PackageFiles'>
                        <array length='1'>
                           <struct>
                              <var name='FileSize'>
                                 <number>53372</number>
                              </var>
                              <var name='Global_Flag'>
                                 <string>1060</string>
                              </var>
                              <var name='Url'>
                                 <string>https://testurl/fish.jpg</string>
                              </var>
                              <var name='Fingerprint'>
                                 <string>0460a2681b2547af346948a7ca4e5b9d</string>
                              </var>
                           </struct>
                        </array>
                     </var>
                     <var name='ExpiryTime'>
                        <string>1406203199</string>
                     </var>
                     <var name='SenderName'>
                        <string>Jane Dole</string>
                     </var>
                  </struct>
               </var>
            </struct>
         </var>
      </struct>
   </data>
</wddxPacket>
";
            var deserializer = new WddxDeserializer();
            var typedTestClass = deserializer.Deserialize<TypedTestClass4>(structPacket);
            Assert.AreEqual("8888", typedTestClass.Id);
            Assert.IsTrue(typedTestClass.Packages.Count == 2);
            Assert.IsTrue(typedTestClass.Packages.ContainsKey("1037-z140558641359"));
            Assert.IsTrue(typedTestClass.Packages.ContainsKey("1037-w20140717084438533"));
            
            var package1 = typedTestClass.Packages["1037-z140558641359"];
            Assert.AreEqual("1", package1.FileCount);
            Assert.AreEqual("1405586413", package1.SentTime);
            Assert.IsNotNull(package1.PackageFiles);
            Assert.IsTrue(package1.PackageFiles.Length == 1);
            Assert.IsTrue(package1.PackageFiles[0].FileSize == 15525);
            Assert.IsTrue(package1.PackageFiles[0].GlobalFlag == "4");
            Assert.IsTrue(package1.PackageFiles[0].Url == "https://testurl/monkey.kpg");
            Assert.IsTrue(package1.PackageFiles[0].Fingerprint == "c85c136975fe355c75cea70474c0f180-15525");
        }
    }
}
