using Hatfield.EnviroData.Core;
using Hatfield.EnviroData.DataAcquisition;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.CVUpdater
{
    public class CVTermBusinessLayer
    {
        DbSet _entityContext;
        DbContext _context;

        static Dictionary<string, dynamic> types = new Dictionary<string, dynamic> { { "actiontype", new CV_ActionType() }, { "aggregationstatistic", new CV_AggregationStatistic() }, { "annotationtype", new CV_AnnotationType() }, { "censorcode", new CV_CensorCode() }, { "dataqualitytype", new CV_DataQualityType() }, { "datasettype", new CV_DatasetTypeCV() }, { "directivetype", new CV_DirectiveType() }, { "elevationdatum", new CV_ElevationDatum() }, { "equipmenttype", new CV_EquipmentType() }, { "methodtype", new CV_MethodType() }, { "organizationtype", new CV_OrganizationType() }, { "propertydatatype", new CV_PropertyDataType() }, { "qualitycode", new CV_QualityCode() }, { "referencematerialmedium", new CV_ReferenceMaterialMedium() }, { "relationshiptype", new CV_RelationshipType() }, { "resulttype", new CV_ResultType() }, { "medium", new CV_SampledMedium() }, { "samplingfeaturegeotype", new CV_SamplingFeatureGeoType() }, { "samplingfeaturetype", new CV_SamplingFeatureType() }, { "sitetype", new CV_SiteType() }, { "spatialoffsettype", new CV_SpatialOffsetType() }, { "speciation", new CV_Speciation() }, { "specimenmedium", new CV_SpecimenMedium() }, { "specimentype", new CV_SpecimenType() }, { "status", new CV_Status() }, { "taxonomicclassifiertype", new CV_TaxonomicClassifierType() }, { "unitstype", new CV_UnitsType() }, { "variablename", new CV_VariableName() }, { "variabletype", new CV_VariableType() } };

        public CVTermBusinessLayer(DbContext context)
        {
            _context = context;
        }

        public IResult AddOrUpdateCVs(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            //Get the entity corresponding to the endpoint
            if (!types.ContainsKey(endpoint))
            {
                //do nothing if no matching type found for endpoint
                //need to add to the log in the future
                return new BaseResult(ResultLevel.INFO, "No matching type was found for the endpoint");
            }

            string resultMessages = "";

            var type = types[endpoint].GetType();
            _entityContext = _context.Set(type);

            for (var i=0; i<extractedEntities.Count(); i++)
            {
                var result = VerifyData(extractedEntities.ElementAt(i));

                if (result.Level == ResultLevel.INFO)
                {
                    string tableName = types[endpoint].ToString();

                    var results = _entityContext.Find(extractedEntities.ElementAt(i).Name);

                    if (results == null)
                    {
                        var cv = Activator.CreateInstance(type);
                        //Set properties on new object
                        cv.Term = extractedEntities.ElementAt(i).Term;
                        cv.Name = extractedEntities.ElementAt(i).Name;
                        //cv.Definition = entity.Definition;
                        cv.Category = extractedEntities.ElementAt(i).Category;
                        cv.SourceVocabularyURI = extractedEntities.ElementAt(i).SourceVocabularyURI;
                        AddSingleCV(cv);

                        resultMessages += "Term '" + extractedEntities.ElementAt(i).Term + "' was added";
                        resultMessages += "Fetching data for term'" + extractedEntities.ElementAt(i + 1).Term;

                    }
                    else
                    {
                        var cv = _context.Entry(results);
                        //cv.Property("Name").CurrentValue = entity.Name;
                        cv.Property("Term").CurrentValue = extractedEntities.ElementAt(i).Term;
                        //cv.Property("Definition").CurrentValue = entity.Definition;
                        cv.Property("Category").CurrentValue = extractedEntities.ElementAt(i).Category;
                        cv.Property("SourceVocabularyURI").CurrentValue = extractedEntities.ElementAt(i).SourceVocabularyURI;

                        _context.Entry(results).CurrentValues.SetValues(cv);

                        resultMessages += "Updating row '" + extractedEntities.ElementAt(i+1).Term + "' in type '" + type + "',";
                    }
                }
                else
                {
                    resultMessages += "Invalid Data";
                }

            }
            _context.SaveChanges();
            return new BaseResult(ResultLevel.INFO, " - Completed adding/updating CV for type'" + endpoint + "'" + resultMessages);

        }

        public IResult CheckForDeleted(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            string resultMessages = "";

            var result = new BaseResult(ResultLevel.INFO, "");
            if (!types.ContainsKey(endpoint))
            {
                //do nothing if no matching type found for endpoint
                //need to add to the log in the future
                return new BaseResult(ResultLevel.ERROR, "No matching CV found");
            }

            var type = types[endpoint].GetType();
            _entityContext = _context.Set(type);

            for (var i = 0; i< extractedEntities.Count(); i ++)
            {
                var cv = _context.Entry(extractedEntities.ElementAt(i));
                //var theEntity = _entityContext.Attach(entity);
                bool exists = extractedEntities.Where(x => x.Name == cv.Property("Name").CurrentValue.ToString()).Any();
                if (!exists)
                {
                    //_entityContext.Remove(entity);
                    resultMessages = "Deleted term " + cv.Property("Term").CurrentValue.ToString();
                }
            }
            //_context.SaveChanges();
            return new BaseResult(ResultLevel.INFO, " - CV of type '" + endpoint + "' has been updated," + resultMessages+", ");
        }

        public IResult VerifyData(CVModel entity)
        {
            if (entity.Term == null || entity.Name == null)
            {
                return new BaseResult(ResultLevel.ERROR, "Data does not contain sufficient information");
            }
            else
            { return new BaseResult(ResultLevel.INFO, "Data is complete"); }
        }

        public void AddSingleCV(object cv)
        {
            _entityContext.Add(cv);

        }
    }
}
