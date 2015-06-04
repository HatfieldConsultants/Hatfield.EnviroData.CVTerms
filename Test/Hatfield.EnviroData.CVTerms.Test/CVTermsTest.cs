using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hatfield.EnviroData.CVUpdater;
using System.Configuration;
using System.Xml.Linq;

namespace Hatfield.EnviroData.CVTermsUploader.Test
{
    [TestFixture]
    public class CVTermsTest
    {
        [Test]
        public void ScrapeCVEndpointsTest()
        {
            CVTermParser parser = new CVTermParser();
            var endpoints = parser.GetAPIEndpoints("http://vocabulary.odm2.org/api/v1/", "http://vocabulary.odm2.org");

            Assert.IsNotEmpty(endpoints);
            Assert.AreEqual(28, endpoints.Count());

            Assert.AreEqual("Action Type", endpoints.ElementAt(0).Key);
            Assert.AreEqual("http://vocabulary.odm2.org/api/v1/actiontype", endpoints.ElementAt(0).Value);
        }   

        [Test]
        public void GetJsonCVEndpointsTest()
        {

        }



        [Test]
        public void GetCVTest()
        {
            CVTermParser parser = new CVTermParser();
            var result = parser.GetCVSkos("http://vocabulary.odm2.org", "");

        }

        [Test]
        public void ImportXMLDataTest()
        {
            CVTermParser parser = new CVTermParser();
            var doc = XDocument.Parse(parser.GetCVSkos("http://vocabulary.odm2.org", ""));
            parser.ImportXMLData(doc);
        }
    }
}
