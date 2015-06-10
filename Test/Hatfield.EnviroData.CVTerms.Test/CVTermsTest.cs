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
            CVTermAPILayer parser = new CVTermAPILayer();
            var endpoints = parser.GetAPIEndpoints( "http://vocabulary.odm2.org");

            Assert.IsNotEmpty(endpoints);
            Assert.AreEqual(28, endpoints.Count());
        }   

        [Test]
        public void GetJsonCVEndpointsTest()
        {

        }

        [Test]
        public void GetCVTest()
        {
            CVTermAPILayer parser = new CVTermAPILayer();
            var result = parser.GetSingleCV("http://vocabulary.odm2.org/api/v1/","http://vocabulary.odm2.org/api/v1/actiontype/", "skos");
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetCVFailTest()
        {
            CVTermAPILayer parser = new CVTermAPILayer();
            var result = parser.GetSingleCV("http://vocabulary.odm2.org/api/v1/" , "http://vocabulary.odm2.org/api/v1/actiontyumtype/", "skos");
            Assert.IsNotNull(result);
            Assert.AreSame("No Data Found", result);
        }

        [Test]
        public void ExtractSKOSIntoCVModelsTest()
        {
            CVTermAPILayer parser = new CVTermAPILayer();
            var doc = XDocument.Parse(parser.GetSingleCV("http://vocabulary.odm2.org/api/v1/" ,"http://vocabulary.odm2.org/api/v1/actiontype/", ""));
            var extractedDataSet = parser.ImportXMLData(doc);
            Assert.AreEqual(true, extractedDataSet.IsExtractedSuccess);
            Assert.AreEqual(24, extractedDataSet.ExtractedEntities.Count());
        }
    }
}
