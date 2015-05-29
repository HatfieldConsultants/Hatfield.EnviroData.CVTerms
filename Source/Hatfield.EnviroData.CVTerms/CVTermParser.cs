using Hatfield.EnviroData.DataAcquisition.XML;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace Hatfield.EnviroData.CVUpdater
{
    public class CVTermParser
    {
        public CVTermParser() { }

        public List<string> GetAPIEndpoints(string apiRoot, string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(url);

            List<string> Endpoints = new List<string>();

            foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[contains(@class,'list-title')]"))
            {
                var name = div.ChildNodes.Select(x => x.Element("h3")).First().InnerText;
                name = name.Replace(" ", "").ToLower();
                var endpoint = apiRoot + name;
                Endpoints.Add(endpoint);
            }

            return Endpoints;
        }

        public string GetCVSkos(string endpoint, string format)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endpoint + format);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                HttpResponseMessage response = client.GetAsync("/api/v1/actiontype/?format=skos").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync();
                        //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                        return result.Result.ToString();
                    }
                    else
                    {
                        return "No Data Found";
                    }
            }
        }

        public void ImportXMLData(XDocument doc)
        {
            var dataToImport = new XMLDataToImport("result", doc);

            var dataImporter = new XMLImporterBuilder().Build();

            var extractedDataSet = dataImporter.Extract<CVModel>(dataToImport);
        }

        //public IEnumerable<CVModel> GetAllCVs(List<string> endpoints)
        //{
        //    IEnumerable<CVModel> results;

        //    foreach (var endpoint in endpoints)
        //    { 

        //    }
        //}
    }
}
