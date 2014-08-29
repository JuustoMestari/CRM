using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Xrm.Sdk;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.ServiceModel.Security;
using System.ServiceModel;

/*/
 * REQUIRES those references to work 
 * Microsoft.xrm.client
 * Microsoft.Xrm.Sdk
 * Microsoft.Xrm.Sdk.Deployment
 * System.ServiceModel
 */

namespace ESC_CRM_
{
    public class ESC_CRM11 
    {
        protected IOrganizationService _service;
        public bool connect(string serviceURL, string domainName, string userName, string password)
        {
            try
            {

                Uri organizationUri = new Uri(serviceURL);
                Uri homeRealmUri = null;
                ClientCredentials credentials = new ClientCredentials();
                // set default credentials for OrganizationService
                credentials.Windows.ClientCredential = new NetworkCredential(userName, password, domainName);
                // credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
                OrganizationServiceProxy orgProxy = new OrganizationServiceProxy(organizationUri, homeRealmUri, credentials, null);
                _service = (IOrganizationService)orgProxy;

                //to check connection with CRM
                getAttributeMax("campaign", "exchangerate");
               
                return true;
            }
            catch (InvalidOperationException)
            {
                throw new connectionException("The URI provided cannot be resolved ( " + serviceURL + " )");
            }
            catch (SecurityNegotiationException)
            {
                throw new connectionException("The authentication failed ! Please check the credentials provided.");
            }
            catch (Exception ex)
            {
                throw new connectionException(ex.Message);
            }
        }

        public Guid addEntity(string entityName, Dictionary<string, object> parameters)
        {
            Entity myEntity = new Entity(entityName);
            Dictionary<string, long> errorList = new Dictionary<string, long>();
            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                long attributeLength = getAttributeMax(entityName, kvp.Key);
                if (kvp.Value.ToString().Length > attributeLength && attributeLength!=0)
                {
                    errorList.Add(kvp.Key, attributeLength);
                }
                else
                {
                    myEntity[kvp.Key] = kvp.Value;
                }

            }
            if (errorList.Count!=0) throw new attributelengthException(errorList);
            try
            {
                return _service.Create(myEntity);
            }
            catch (FaultException ex)
            {
                throw new mismatchException(ex.Message);
            }
        }
		
		//example of method to retrieve firstname and lastname of leads
		//using a condition, in this example, only display lead with donotsendmm to false
        public List<string> getLeadNames()
        {
            try
            {
                QueryExpression leadListExpression = new QueryExpression
                {
                    EntityName = "lead",
                    ColumnSet = new ColumnSet("firstname","lastname"),
                    Criteria = new FilterExpression
                    {
                       Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName="donotsendmm",
                            Operator=ConditionOperator.Equal,
                            Values={false}
                        },
                    }
                    }
                };

                DataCollection<Entity> leadListCollection = _service.RetrieveMultiple(leadListExpression).Entities;
                List<string> myList = new List<string>();
                foreach (Entity e in leadListCollection)
                {
                    //Use Attributes.Contains first or you'll get an exception otherwise
                    if (e.Attributes.Contains("firstname") && e.Attributes.Contains("lastname")) myList.Add((string)e.Attributes["firstname"] +" "+ (string)e.Attributes["lastname"]);
                }
                return myList;
            }
            catch (Exception ex)
            {
                throw new courseListException(ex.Message);
            }
        }
           
        public Dictionary<int, string> getOptionSet(string entityName, string attributeName)
        {
            // Create the request
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            // Execute the request
            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
            PicklistAttributeMetadata retrievedOptionSet = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;
            OptionMetadata[] optionSet = retrievedOptionSet.OptionSet.Options.ToArray();
            Dictionary<int, string> eventTypes = new Dictionary<int, string>();
            foreach (OptionMetadata om in optionSet)
            {
                eventTypes.Add((int)om.Value, om.Label.UserLocalizedLabel.Label);
            }
            return eventTypes;
        }
        
        public long getAttributeMax(string entityName, string attributeName)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            RetrieveAttributeResponse attributeResponse;
            // Execute the request
            try
            {
                attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
            }
            catch (FaultException ex)
            {
                throw new attributeMaxException(ex.Message);
            }
            var retrievedAttribute = attributeResponse.AttributeMetadata;
            //get the type of data
            string[] datatype = retrievedAttribute.GetType().ToString().Split('.');
            
            switch (datatype[datatype.Length-1])
            {
                case "StringAttributeMetadata": StringAttributeMetadata SAM = (StringAttributeMetadata)retrievedAttribute;
                    return (long)SAM.MaxLength;
                case "MemoAttributeMetadata": MemoAttributeMetadata MeAM = (MemoAttributeMetadata)retrievedAttribute;
                    return (long)MeAM.MaxLength;
                case "IntegerAttributeMetadata": IntegerAttributeMetadata IAM = (IntegerAttributeMetadata)retrievedAttribute;
                    return (long)IAM.MaxValue;
                case "MoneyAttributeMetadata": MoneyAttributeMetadata MAM = (MoneyAttributeMetadata)retrievedAttribute;
                    return (long)MAM.MaxValue;
                case "DecimalAttributeMetadata": DecimalAttributeMetadata DAM = (DecimalAttributeMetadata)retrievedAttribute;
                    return (long)DAM.MaxValue;           
                    
                default: return 0;
            }
            
        }
    }
}
