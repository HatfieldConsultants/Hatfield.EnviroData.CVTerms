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
        private static log4net.ILog log = log4net.LogManager.GetLogger("Hatfield.EnviroData.CVTerms");

        public static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
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
                biz.AddOrUpdateCVs(endpoint.Value, results.ExtractedEntities);
                biz.CheckForDeleted(endpoint.Value, results.ExtractedEntities);
                
            }
        }
    }
}

