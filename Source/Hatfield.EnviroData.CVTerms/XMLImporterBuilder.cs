using Hatfield.EnviroData.DataAcquisition;
using Hatfield.EnviroData.DataAcquisition.Criterias;
using Hatfield.EnviroData.DataAcquisition.ValueAssigners;
using Hatfield.EnviroData.DataAcquisition.XML;
using Hatfield.EnviroData.DataAcquisition.XML.Importers;
using Hatfield.EnviroData.DataAcquisition.XML.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.CVUpdater
{
    public class XMLImporterBuilder
    {
        public IDataImporter Build()
        {
            var parserFactory = new DefaultXMLParserFactory();
            var testImporter = new MultiLevelXMLDataImporter(ResultLevel.ERROR, "Description");

            var labNameFieldExtractConfiguration = new SimpleXMLExtractConfiguration("prefLabel", "", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "Term");

            var dateReportedFieldExtractConfiguration = new SimpleXMLExtractConfiguration("prefLabel", "", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "Name");

            var projectIDFieldExtractConfiguration = new SimpleXMLExtractConfiguration("definition", "", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "Definition");

            var sdgIDFieldExtractConfiguration = new SimpleXMLExtractConfiguration("category", "", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "Category");

            var labSignatoryFieldExtractConfiguration = new SimpleXMLExtractConfiguration("Description", "about", parserFactory.GetElementParser(typeof(string)), new SimpleValueAssigner(), typeof(string), "SourceVocabularyURI");

            testImporter.AddExtractConfiguration(labNameFieldExtractConfiguration);
            testImporter.AddExtractConfiguration(dateReportedFieldExtractConfiguration);
            testImporter.AddExtractConfiguration(projectIDFieldExtractConfiguration);
            testImporter.AddExtractConfiguration(sdgIDFieldExtractConfiguration);
            testImporter.AddExtractConfiguration(labSignatoryFieldExtractConfiguration);

            return testImporter;
        }
    }
}
