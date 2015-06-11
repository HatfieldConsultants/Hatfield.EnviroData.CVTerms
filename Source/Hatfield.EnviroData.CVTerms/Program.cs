using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.Entity;
using Hatfield.EnviroData.Core;

namespace Hatfield.EnviroData.CVUpdater
{
    class Program
    {
        public static void Main(string[] args)
        {
            string ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
            string VocabSiteUrl = ConfigurationManager.AppSettings["VocabTermsUrl"];

            CVTermAPILayer parser = new CVTermAPILayer();
            CVTermBusinessLayer biz = new CVTermBusinessLayer(new ODM2Entities());

            var endpoints = parser.GetAPIEndpoints(VocabSiteUrl);

            //Get data for each CV Type, extract and write to the DB
            foreach (var endpoint in endpoints)
            {
                var doc = new XDocument();
                var rawCV = parser.GetSingleCV(ApiUrl, endpoint.Value, "skos");
                var results = parser.ImportXMLData(XDocument.Parse(rawCV));
                biz.AddCVs(endpoint.Value, results.ExtractedEntities);
            }
        }
    }
}

