using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hatfield.EnviroData.Core;

namespace Hatfield.EnviroData.CVUpdater
{
    public class CVTermRepository
    {
        ODM2Entities context = new ODM2Entities();

        static Dictionary<string, dynamic> types = new Dictionary<string, dynamic> { { "actiontype", new CV_ActionType() }, { "aggregationstatistic", new CV_AggregationStatistic() }, { "annotationtype", new CV_AnnotationType() }, { "censorcode", new CV_CensorCode() }, { "dataqualitytype", new CV_DataQualityType() }, { "datasettype", new CV_DatasetTypeCV() }, { "directivetype", new CV_DirectiveType() }, { "elevationdatum", new CV_ElevationDatum() }, { "equipmenttype", new CV_EquipmentType() }, { "methodtype", new CV_MethodType() }, { "organizationtype", new CV_OrganizationType() }, { "propertydatatype", new CV_PropertyDataType() }, { "qualitycode", new CV_QualityCode() }, { "referencematerialmedium", new CV_ReferenceMaterialMedium() }, { "relationshiptype", new CV_RelationshipType() }, { "resulttype", new CV_ResultType() }, { "sampledmedium", new CV_SampledMedium() }, { "samplingfeaturegeotype", new CV_SamplingFeatureGeoType() }, { "samplingfeaturetype", new CV_SamplingFeatureType() }, { "sitetype", new CV_SiteType() }, { "spatialoffsettype", new CV_SpatialOffsetType() }, { "speciation", new CV_Speciation() }, { "specimenmedium", new CV_SpecimenMedium() }, { "specimentype", new CV_SpecimenType() }, { "status", new CV_Status() }, { "taxonomicclassifiertype", new CV_TaxonomicClassifierType() }, { "unitstype", new CV_UnitsType() }, { "variablename", new CV_VariableName() }, { "variabletype", new CV_VariableType() } };

        public void WriteToDB(string endpoint, IEnumerable<CVModel> extractedEntities)
        {
            //Get the entity corresponding to the endpoint
            var type = types[endpoint].GetType();
            var currentContext = context.Set(type);

            foreach (var entity in extractedEntities)
            {
                if (currentContext.Find(entity.Name) == null )
                {
                    if (entity.Term == null || entity.Name == null )
                    {
                        continue;
                    }
                    else
                    {
                        //Instantiate a new object of the specific type
                        var theObject = Activator.CreateInstance(type);
                        //Set properties on new object
                        theObject.Term = entity.Term;
                        theObject.Name = entity.Name;
                        theObject.Definition = entity.Definition;
                        theObject.Category = entity.Category;
                        theObject.SourceVocabularyURI = entity.SourceVocabularyURI;

                        currentContext.Add(theObject);
                        context.SaveChanges();
                    }
                    }
                else
                {
                    continue;
                }
            }
        }
    }
}
