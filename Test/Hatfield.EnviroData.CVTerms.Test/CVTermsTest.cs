using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hatfield.EnviroData.CVUpdater;
using System.Configuration;

namespace Hatfield.EnviroData.CVTermsUploader.Test
{
    public class CVTermsTest
    {
       // [TestFixture]

        [Test]
        public void ScrapeCVEndpoints()
        {
            CVTermParser parser = new CVTermParser();
            var endpoints = parser.GetAPIEndpoints("http://vocabulary.odm2.org/api/v1/", "http://vocabulary.odm2.org");

            Assert.IsNotEmpty(endpoints);
            Assert.AreEqual(28, endpoints.Count());
        }   

        [Test]
        public void GetJsonCVEndpoints()
        {

        }

        [Test]
        public void GetXMLResponse()
        {
            CVTermParser parser = new CVTermParser();
            var result = parser.GetCV("http://vocabulary.odm2.org/api/v1/", "");
                
        }
    }
}
