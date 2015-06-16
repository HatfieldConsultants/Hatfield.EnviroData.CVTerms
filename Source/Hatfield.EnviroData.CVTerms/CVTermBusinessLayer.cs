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

        static Dictionary<string, dynamic> types = new Dictionary<string, dynamic> { { "actiontype", new CV_ActionType() }, { "aggregationstatistic", new CV_AggregationStatistic() }, { "annotationtype", new CV_AnnotationType() }, { "censorcode", new CV_CensorCode() }, { "dataqualitytype", new CV_DataQualityType() }, { "datasettype", new CV_DatasetTypeCV() }, { "directivetype", new CV_DirectiveType() }, { "elevationdatum", new CV_ElevationDatum() }, { "equipmenttype", new CV_EquipmentType() }, { "methodtype", new CV_MethodType() }, { "organizationtype", new CV_OrganizationType() }, { "propertydatatype", new CV_PropertyDataType() }, { "qualitycode", new CV_QualityCode() }, { "referencematerialmedium", new CV_ReferenceMaterialMedium() }, { "relationshiptype", new CV_RelationshipType() }, { "resulttype", new CV_ResultType() }, { "sampledmedium", new CV_SampledMedium() }, { "samplingfeaturegeotype", new CV_SamplingFeatureGeoType() }, { "samplingfeaturetype", new CV_SamplingFeatureType() }, { "sitetype", new CV_SiteType() }, { "spatialoffsettype", new CV_SpatialOffsetType() }, { "speciation", new CV_Speciation() }, { "specimenmedium", new CV_SpecimenMedium() }, { "specimentype", new CV_SpecimenType() }, { "status", new CV_Status() }, { "taxonomicclassifiertype", new CV_TaxonomicClassifierType() }, { "unitstype", new CV_UnitsType() }, { "variablename", new CV_VariableName() }, { "variabletype", new CV_VariableType() } };

        public CVTermBusinessLayer(DbContext context)
        {
            _context = context;
        }

        public void AddCVs(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            //Get the entity corresponding to the endpoint
            var type = types[endpoint].GetType();
            _entityContext = _context.Set(type);

            foreach (var entity in extractedEntities)
            {
                string result = VerifyData(entity);

                if (String.IsNullOrEmpty(result))
                {
                    
                    var duplicate = CheckForDuplicates(entity.Name);
                    if (!duplicate)
                    { 
                        //Instantiate a new object of the specific type
                        var cv = Activator.CreateInstance(type);
                        //Set properties on new object
                        cv.Term = entity.Term;
                        cv.Name = entity.Name;
                        cv.Definition = entity.Definition;
                        cv.Category = entity.Category;
                        cv.SourceVocabularyURI = entity.SourceVocabularyURI;
                    
                        AddSingleCV(cv);
                    }
                }
            }
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

        public bool CheckForDuplicates(string name)
        {
            bool isDuplicate = false;
            if (_entityContext.Find(name) != null)
            {
                isDuplicate = true;
            }
            return isDuplicate;
        }

        public void AddSingleCV(object cv)
        {
            _entityContext.Add(cv);
            _context.SaveChanges();
        }
    }
}
