using Hatfield.EnviroData.DataAcquisition;
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
    public class CVTermAPILayer
    {
        public CVTermAPILayer() { }

        public List<string> GetAPIEndpoints(string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(url);

            List<string> Endpoints = new List<string>();

            foreach (HtmlNode div in doc.DocumentNode.SelectNodes("//div[contains(@class,'list-title')]"))
            {
                var name = div.ChildNodes.Select(x => x.Element("h3")).First().InnerText;
                name = name.Replace(" ", "").ToLower();
                char[] trimmed = name.ToCharArray();
                trimmed = Array.FindAll<char>(trimmed, (x => (char.IsLetterOrDigit(x))));
                name = new string(trimmed);
                var endpoint = name;
                Endpoints.Add(endpoint);
            }

            return Endpoints;
        }

        public string GetSingleCV(string apiRoot, string endpoint, string format)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                try
                {
                    HttpResponseMessage response = client.GetAsync(apiRoot + endpoint + "?format=" + format).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync();
                        return result.Result.ToString();
                    }
                    else
                    {
                        return "No Data Found";
                    }
                }
                catch (HttpRequestException e)
                {
                    return e.Message;
                }
            }
        }

        public IExtractedDataset<CVModel> ImportXMLData(XDocument doc)
        {
            var dataToImport = new XMLDataToImport("result", doc);
            var dataImporter = new XMLImporterBuilder().Build();

            var extractedDataSet = dataImporter.Extract<CVModel>(dataToImport);

            return extractedDataSet;
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
