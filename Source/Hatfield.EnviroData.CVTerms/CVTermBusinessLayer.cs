using Hatfield.EnviroData.Core;
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

        public void AddOrUpdateCVs(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            //Get the entity corresponding to the endpoint
            if(!types.ContainsKey(endpoint))
            {
                //do nothing if no matching type found for endpoint
                //need to add to the log in the future
                return;
            }

            var type = types[endpoint].GetType();
            _entityContext = _context.Set(type);

            foreach (var entity in extractedEntities)
            {
                string result = VerifyData(entity);

                if (String.IsNullOrEmpty(result))
                {
                    string tableName = types[endpoint].ToString();

                    var results = _entityContext.Find(entity.Name);


                    if (results == null)
                    {
                        var cv = Activator.CreateInstance(type);
                        //Set properties on new object
                        cv.Term = entity.Term;
                        cv.Name = entity.Name;
                        //cv.Definition = entity.Definition;
                        cv.Category = entity.Category;
                        cv.SourceVocabularyURI = entity.SourceVocabularyURI;          
                        AddSingleCV(cv);
                    }
                    else
                    {
                        var cv = _context.Entry(results);
                        //cv.Property("Name").CurrentValue = entity.Name;
                        cv.Property("Term").CurrentValue = entity.Term;
                        //cv.Property("Definition").CurrentValue = entity.Definition;
                        cv.Property("Category").CurrentValue = entity.Category;
                        cv.Property("SourceVocabularyURI").CurrentValue = entity.SourceVocabularyURI;

                        _context.Entry(results).CurrentValues.SetValues(cv);
                        Console.WriteLine("Updated term '" + cv.Property("Name").CurrentValue.ToString() + "'");
                        _context.SaveChanges();
                    }
                }

            }
        }

        public void CheckForDeleted(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            if (!types.ContainsKey(endpoint))
            {
                //do nothing if no matching type found for endpoint
                //need to add to the log in the future
                return;
            }

            var type = types[endpoint].GetType();
            _entityContext = _context.Set(type);

            foreach (var entity in _entityContext)
            {
                var cv = _context.Entry(entity);
                //var theEntity = _entityContext.Attach(entity);
                bool exists = extractedEntities.Where(x => x.Name == cv.Property("Name").CurrentValue.ToString()).Any();
                if (!exists)
                {
                    //_entityContext.Remove(entity);
                    Console.WriteLine("Deleted term '" + cv.Property("Name").CurrentValue.ToString() + "' - not deleted because of FK conflict");
                }
            }
            //_context.SaveChanges();
        }

        public string VerifyData(CVModel entity)
        {
            string message = null;
            if (entity.Term == null || entity.Name == null)
            {
                message= "Bad Data, please contact site administrator";
            }
            return message;
        }

        public void AddSingleCV(object cv)
        {
            _entityContext.Add(cv);
            _context.SaveChanges();
        }
    }
}
