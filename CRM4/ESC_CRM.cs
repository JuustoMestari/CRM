using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk.Query;
using System.Collections;
using System.Net;
using System.Web.Security;
using System.Web.UI;
using System.Web.Services.Protocols;
using ESC_CRM_EX;
namespace ESC_CRM_
{
    public enum attribute_type
    {
        tString,tInteger,tPicklist,tLookup,tDateTime,tBoolean,tMoney,tDecimal,tFloat,tPartylist

    }
    public class ESC_CRM
    {
        protected CrmService m_crmService;
        protected string m_crmURL;
        protected string m_crmURL_MD;
        protected string m_crmName;
        protected string m_crmLogin;
        protected string m_crmPassword;
        protected string m_crmDomain;
        protected bool m_isconnected;

        protected ArrayList m_propertyList;
        protected string m_workingparam;
        protected DynamicEntity m_dynamicEntity_;
        protected FilterExpression m_filterExpression;
        public string debug;
        public ArrayList numeric_values_jobtitles;
        public ArrayList numeric_values_marketingsource;

        /// <summary>
        /// The class constructor.
        /// It initializes the connection parameters
        /// </summary>
        public ESC_CRM(string crmURL, string crmURLMD, string crmName, string crmLogin, string crmPassword, string crmDomain)
        {
            m_crmService = new CrmService();
            m_crmURL = crmURL; 				//ex : http://crm.domain.net/[OrganizationName]/XRMServices/2011/Organization.svc
            m_crmURL_MD = crmURLMD;			//ex : https://crm.domain.net/mscrmservices/2007/metadataservice.asmx
            m_crmName = crmName;			//OrganizationName
            m_crmLogin = crmLogin;
            m_crmPassword = crmPassword;
            m_crmDomain = crmDomain;
            m_isconnected = false;

            m_propertyList = new ArrayList();
            m_filterExpression = new FilterExpression();
        }

        /// <summary>
        /// This method connects to the CRM server (This is the first method you need to use)
        /// It creates a lead "test" and deletes it to check the connection with CRM.
        /// </summary>
        public bool connect()
        {
            try
            {
                Microsoft.Crm.Sdk.CrmAuthenticationToken token = new Microsoft.Crm.Sdk.CrmAuthenticationToken();
                token.AuthenticationType = 0; // Use Active Directory authentication.
                token.OrganizationName = m_crmName;
                m_crmService.Credentials = new NetworkCredential(m_crmLogin, m_crmPassword, m_crmDomain);
                m_crmService.Url = m_crmURL;
                m_crmService.CrmAuthenticationTokenValue = token;
                m_crmService.PreAuthenticate = true;
				
				
                DynamicEntity new_lead = new DynamicEntity("lead");
                StringProperty sp_lead_topic = new StringProperty("subject", "test");
                StringProperty sp_lead_lastname = new StringProperty("lastname", "test");
                new_lead.Properties.Add(sp_lead_lastname);
                new_lead.Properties.Add(sp_lead_topic);

                Guid created_lead = m_crmService.Create(new_lead);

                m_crmService.Delete("lead", created_lead);

                m_isconnected = true;
                return true;
            }
            catch (WebException)
            {
                throw new Exception("Failed to connect to the CRM Server !<br />" +
                    "Please check that the credentials(Login,Password and Domain) provided are correct.<br />" +
                    "<b><u>Addresses</u></b><br /><br />" +
                    "SERVER \t=><a href=\"" + m_crmURL + "\">" + m_crmURL + "</a><br />" +
                    "MD\t =><a href=\"" + m_crmURL_MD + "\">" + m_crmURL_MD + "</a><br />");
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException(ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// This method is used to add an entity to CRM (normal and dynamic entities). It returns true if
        /// the record has been added and false if there was a problem</summary>
        /// <param name="entityName">CRM name of the entity you want to add (user,lead,...)</param>
        /// <param name="stringParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value.
        /// This ArrayList contains the attributes of type String.
        /// </param>
        /// <param name="intParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value (integer value).
        /// This ArrayList contains the attributes of type Integer.
        /// </param>
        ///  <param name="pickParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value (integer value).
        /// This ArrayList contains the attributes of type Picklist.
        /// </param>
        /// <param name="lookupParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 3 cells : 1)The attribute name 2)The attribute type of the lookup (contact,lead,...)
        /// 3)The GUID of the element you want to use.
        /// This ArrayList contains the attributes of type Lookup.
        /// </param>
        /// <param name="datetimeParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 3 cells : 1)The attribute name 2)The datetimeformat (ex:{0:M/d/yyyy}) 3)The DateTime
        /// This ArrayList contains the attributes of type DateTime.
        /// </param>
        ///  <param name="boolParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (bool)
        /// This ArrayList contains the attributes of type Boolean.
        /// </param>
        /// <param name="decimalParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (decimal)
        /// This ArrayList contains the attributes of type Decimal.
        /// </param>
        /// <param name="floatParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (float)
        /// This ArrayList contains the attributes of type Float.
        /// </param>
        /// <param name="moneyParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (float)
        /// This ArrayList contains the attributes of type Money.
        /// </param>
        public Guid addEntity(string entityName, ArrayList Params)
        {
            Guid created_entity = new Guid();
            try
            {
                isconnected();

                m_workingparam = "";
                m_propertyList.Clear();

                for (int i = 0; i < Params.Count; i++)
                {
                    ArrayList instantParam = (ArrayList)Params[i];
                    switch ((attribute_type)instantParam[0])
                    {
                        case attribute_type.tString: createStringProperties(instantParam);
                            break;
                        case attribute_type.tInteger: createIntegerProperties(instantParam);
                            break;
                        case attribute_type.tPicklist: createPickProperties(instantParam);
                            break;
                        case attribute_type.tLookup: createLookupProperties(instantParam);
                            break;
                        case attribute_type.tDateTime: createDateTimeProperties(instantParam);
                            break;
                        case attribute_type.tBoolean: createBooleanProperties(instantParam);
                            break;
                        case attribute_type.tMoney: createMoneyProperties(instantParam);
                            break;
                        case attribute_type.tDecimal: createDecimalProperties(instantParam);
                            break;
                        case attribute_type.tFloat: createFloatProperties(instantParam);
                            break;
                        case attribute_type.tPartylist: createPartyListProperties(instantParam);
                            break;
                        default:
                            throw new ESC_CRM_EX.typenotfound("This type of data does not exist !", instantParam[0].ToString());

                    }
                }
                addProperties(entityName);
                created_entity = m_crmService.Create(m_dynamicEntity_);
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException("!SoapException!\n" + ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.addEntityException(ex.Message, entityName);
            }

            return created_entity;
        }

        /// <summary>
        /// This method is used to find a record and return the GUID of records matching the criteria
        /// </summary>
        /// <param name="entityName_">CRM name of the entity you want to look for</param>
        /// <param name="primarykeyName">The attribute name containing the GUID for the entity (contactid,campaignid,...) that will be returned in an array</param>
        ///<param name="stringParams">The ArrayList containing the attributes names and their values. The research only work with string type attributes. You can use % to replace characters. A logical AND is used to concatenate the parameters in the search</param>
        public ArrayList findEntity(string entityName_, string primarykeyName, ArrayList stringParams, string attribute_order)
        {
            ArrayList arrayResults = new ArrayList();
            try
            {
                isconnected();

                ColumnSet cols = new ColumnSet();
                createStringConditionExpression(stringParams);
                cols.Attributes.Add(primarykeyName);

                m_filterExpression.FilterOperator = LogicalOperator.And;
                OrderExpression oe = new OrderExpression();
                oe.AttributeName = attribute_order;
                oe.OrderType = OrderType.Descending;
                QueryExpression query = new QueryExpression();
                query.EntityName = entityName_;
                query.ColumnSet = cols;
                query.Criteria = m_filterExpression;
                query.Orders.Add(oe);

                RetrieveMultipleRequest retrieve = new RetrieveMultipleRequest();
                retrieve.Query = query;
                retrieve.ReturnDynamicEntities = true;

                RetrieveMultipleResponse retrieved = (RetrieveMultipleResponse)m_crmService.Execute(retrieve);
                int i = 0;
                foreach (DynamicEntity rec in retrieved.BusinessEntityCollection.BusinessEntities)
                {
                    Microsoft.Crm.Sdk.Key sp = (Microsoft.Crm.Sdk.Key)rec.Properties[primarykeyName];
                    arrayResults.Add(sp.Value);
                    i++;
                }
                if (i == 0) throw new entitynotfoundException();
                return arrayResults;
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException("!SoapException!\n" + ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (entitynotfoundException)
            {
                throw new entitynotfoundException();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// This method is used to get attributes from a specified entity. It returns Dictionary containing all the required attributes values.
        /// </summary>
        /// <param name="entityGuid">GUID of the entity</param>
        /// <param name="entityName_">The entity name type (contact,lead,...)</param>
        ///<param name="entityAttributes">The ArrayList containing the attributes names types you want to retrieve (firstname,lastname,...)</param>
        public Dictionary<string,string> getEntity(Guid entityGuid, string entityName_, ArrayList entityAttributes)
        {
            Dictionary<string, string> arrayData = new Dictionary<string, string>();
            try
            {
                isconnected();

                ArrayList arrayResults = new ArrayList();
                // Create the retrieve target.
                TargetRetrieveDynamic targetRetrieve = new TargetRetrieveDynamic();

                // Set the properties of the target.
                targetRetrieve.EntityName = entityName_;
                targetRetrieve.EntityId = entityGuid;

                // Create the request object.
                RetrieveRequest retrieve = new RetrieveRequest();
                ColumnSet col = new ColumnSet();

                // Set the properties of the request object.
                retrieve.Target = targetRetrieve;
                for (int i = 0; i < entityAttributes.Count; i++)
                {
                    col.AddColumn(entityAttributes[i].ToString());
                }

                retrieve.ColumnSet = col;

                // Indicate that the BusinessEntity should be retrieved as a
                // DynamicEntity.
                retrieve.ReturnDynamicEntities = true;

                // Execute the request.
                RetrieveResponse retrieved = (RetrieveResponse)m_crmService.Execute(retrieve);

                // Extract the DynamicEntity from the request.
                DynamicEntity entity = (DynamicEntity)retrieved.BusinessEntity;

                Microsoft.Crm.Sdk.CrmDateTime crmDateTimeVar = new Microsoft.Crm.Sdk.CrmDateTime();
                Microsoft.Crm.Sdk.CrmNumber crmNumberVar = new Microsoft.Crm.Sdk.CrmNumber();
                Picklist crmPickList = new Picklist();
                Guid crmGuid = new Guid();
                Microsoft.Crm.Sdk.Key keyVar = new Microsoft.Crm.Sdk.Key();
                Lookup lookupVar = new Lookup();
                Microsoft.Crm.Sdk.CrmBoolean boolVar = new Microsoft.Crm.Sdk.CrmBoolean();
                for (int i = 0; i < entityAttributes.Count; i++)
                {
                    if (entity.Properties.Contains(entityAttributes[i].ToString()))
                    {
                        if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(crmDateTimeVar.GetType()))
                        {
                            crmDateTimeVar = (Microsoft.Crm.Sdk.CrmDateTime)entity.Properties[entityAttributes[i].ToString()];
                            arrayData.Add(entityAttributes[i].ToString(),crmDateTimeVar.date.ToString());
                        }
                        else
                        {
                            if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(crmNumberVar.GetType()))
                            {
                                crmNumberVar = (Microsoft.Crm.Sdk.CrmNumber)entity.Properties[entityAttributes[i].ToString()];
                                arrayData.Add(entityAttributes[i].ToString(), crmNumberVar.Value.ToString());
                            }
                            else
                            {
                                if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(keyVar.GetType()))
                                {
                                    keyVar = (Microsoft.Crm.Sdk.Key)entity.Properties[entityAttributes[i].ToString()];
                                    arrayData.Add(entityAttributes[i].ToString(), keyVar.Value.ToString());
                                }
                                else
                                {
                                    if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(lookupVar.GetType()))
                                    {
                                        lookupVar = (Microsoft.Crm.Sdk.Lookup)entity.Properties[entityAttributes[i].ToString()];
                                        arrayData.Add(entityAttributes[i].ToString(), lookupVar.Value.ToString());
                                    }
                                    else
                                    {
                                        if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(boolVar.GetType()))
                                        {
                                            boolVar = (Microsoft.Crm.Sdk.CrmBoolean)entity.Properties[entityAttributes[i].ToString()];
                                            arrayData.Add(entityAttributes[i].ToString(), boolVar.Value.ToString());
                                        }
                                        else
                                        {
                                            if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(crmPickList.GetType()))
                                            {
                                                crmPickList = (Microsoft.Crm.Sdk.Picklist)entity.Properties[entityAttributes[i].ToString()];
                                                arrayData.Add(entityAttributes[i].ToString(), crmPickList.Value.ToString());
                                            }
                                            else
                                            {
                                                if (entity.Properties[entityAttributes[i].ToString()].GetType().Equals(crmGuid.GetType()))
                                                {
                                                    crmGuid = (Guid)entity.Properties[entityAttributes[i].ToString()];
                                                    arrayData.Add(entityAttributes[i].ToString(), crmGuid.ToString());
                                                }
                                                else
                                                {
                                                    arrayData.Add(entityAttributes[i].ToString(), entity.Properties[entityAttributes[i].ToString()].ToString());
                                                }

                                            }

                                        }
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        arrayData.Add(entityAttributes[i].ToString(), "");
                    }
                }
                return arrayData;
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException("!SoapException!\n" + ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.getEntityException(ex.Message, entityName_, entityGuid);
            }
        }

       
        /// <summary>
        /// This method is used to delete an entity
        /// </summary>
        /// <param name="entityGuid">GUID of the entity</param>
        /// <param name="entityName_">The entity name type (contact,lead,...)</param>
        public bool deleteEntity(Guid entityGuid, string entityName_)
        {
            try
            {
                isconnected();

                m_crmService.Delete(entityName_, entityGuid);
                return true;
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException("!SoapException!\n" + ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.deleteEntityException(ex.Message, entityName_, entityGuid);
            }

        }

        /// <summary>
        /// This method is used to update an entity to CRM (normal and dynamic entities). It returns true if
        /// the entity has been updated and false if there was a problem</summary>
        /// <param name="entityGuid">The guid of the entity you want to update</param>
        /// <param name="entityName">CRM name of the entity you want to add (user,lead,...)</param>
        /// <param name="primaryKeyName">The attribute name containing the guid for this entity(contactid,leadid,...)</param>
        /// <param name="stringParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value.
        /// This ArrayList contains the attributes of type String.
        /// </param>
        /// <param name="intParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value (integer value).
        /// This ArrayList contains the attributes of type Integer.
        /// </param>
        ///  <param name="pickParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The attribute value (integer value).
        /// This ArrayList contains the attributes of type Picklist.
        /// </param>
        /// <param name="lookupParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 3 cells : 1)The attribute name 2)The attribute type of the lookup (contact,lead,...)
        /// 3)The GUID of the element you want to use.
        /// This ArrayList contains the attributes of type Lookup.
        /// </param>
        /// <param name="datetimeParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 3 cells : 1)The attribute name 2)The datetimeformat (ex:{0:M/d/yyyy}) 3)The DateTime
        /// This ArrayList contains the attributes of type DateTime.
        /// </param>
        ///  <param name="boolParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (bool)
        /// This ArrayList contains the attributes of type Boolean.
        /// </param>
        /// <param name="decimalParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (decimal)
        /// This ArrayList contains the attributes of type Decimal.
        /// </param>
        /// <param name="floatParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (float)
        /// This ArrayList contains the attributes of type Float.
        /// </param>
        /// <param name="moneyParams">ArrayList containing other ArrayList. Each cell of the ArrayList
        /// contains another ArrayList made of 2 cells : 1)The attribute name 2)The Attribute value (float)
        /// This ArrayList contains the attributes of type Money.
        /// </param>
        public bool updateEntity(Guid entityGuid, string entityName, string primarykeyname, ArrayList stringParams, ArrayList intParams, ArrayList pickParams, ArrayList lookupParams, ArrayList datetimeParams, ArrayList boolParams, ArrayList decimalParams, ArrayList floatParams, ArrayList moneyParams)
        {
            try
            {
                if (!m_isconnected) return false;

                m_workingparam = "";
                m_propertyList.Clear();

                createKeyProperties(primarykeyname, entityGuid);
                createStringProperties(stringParams);
                createIntegerProperties(intParams);
                createLookupProperties(lookupParams);
                createPickProperties(pickParams);
                createBooleanProperties(boolParams);
                createDateTimeProperties(datetimeParams);
                createFloatProperties(floatParams);
                createDecimalProperties(decimalParams);
                createMoneyProperties(moneyParams);

                addProperties(entityName);

                m_crmService.Update(m_dynamicEntity_);
            }
            catch (SoapException ex)
            {
                throw new InvalidPluginExecutionException("!SoapException!\n" + ex.Detail.SelectSingleNode("//description").InnerText);
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.updateEntityException(ex.Message, entityName, entityGuid, primarykeyname);
            }

            return true;
        }


        protected bool createKeyProperties(string attributename, Guid guidattribute)
        {
            try
            {
                m_workingparam = attributename + "-" + guidattribute.ToString();
                m_propertyList.Add(new KeyProperty(attributename, new Microsoft.Crm.Sdk.Key(guidattribute)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "key");
            }
            return true;
        }

        protected bool createStringProperties(ArrayList stringParams)
        {

            try
            {
                //loop to create the string properties

                ArrayList paramtab = (ArrayList)stringParams;
                createm_workingparam(paramtab[1], paramtab[0], null);
                m_propertyList.Add(new StringProperty(paramtab[1].ToString(), paramtab[2].ToString()));

                debug = "*create String over";

            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "string " + m_workingparam);
            }
            return true;
        }

        protected bool createIntegerProperties(ArrayList intParams)
        {
            //loop to create the Integer properties

            try
            {

                ArrayList paramtab = (ArrayList)intParams;
                createm_workingparam(paramtab[1], paramtab[2], null);
                m_propertyList.Add(new CrmNumberProperty(paramtab[1].ToString(), new Microsoft.Crm.Sdk.CrmNumber(Convert.ToInt32(paramtab[2]))));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "integer property" + m_workingparam);
            }
            return true;
        }

        protected bool createPickProperties(ArrayList pickParams)
        {
          
                try
                {

                    createm_workingparam(pickParams[1], pickParams[2], null);
                    m_propertyList.Add(new PicklistProperty(pickParams[1].ToString(), new Picklist(Convert.ToInt32(pickParams[2]))));
                }
                catch (Exception ex)
                {
                    throw new createPropertyException(ex.Message, "picklist"+ m_workingparam);
                }
            

            return true;
        }

        protected bool createLookupProperties(ArrayList lookupParams)
        {
            //loop to create the Lookup properties

            try
            {
                ArrayList paramtab = (ArrayList)lookupParams;
                createm_workingparam(paramtab[1], paramtab[2], paramtab[3]);
                Guid paramvalue = new Guid(paramtab[3].ToString());
                m_propertyList.Add(new LookupProperty(paramtab[1].ToString(), new Lookup(paramtab[2].ToString(), paramvalue)));
                debug += "*create lookup over";
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "lookup" + m_workingparam);
            }

            return true;
        }

        protected bool createDateTimeProperties(ArrayList datetimeParams)
        {
            //loop to create the DateTime properties

            try
            {
                ArrayList paramtab = (ArrayList)datetimeParams;
                createm_workingparam(paramtab[1], paramtab[2], paramtab[3]);
                string paramvalue = String.Format(paramtab[2].ToString(), Convert.ToDateTime(paramtab[3]));
                m_propertyList.Add(new CrmDateTimeProperty(paramtab[1].ToString(), new Microsoft.Crm.Sdk.CrmDateTime(paramvalue)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "datetime" + m_workingparam);
            }


            return true;
        }

        protected bool createBooleanProperties(ArrayList boolParams)
        {
            //loop to create the Boolean properties

            try
            {
                ArrayList paramtab = (ArrayList)boolParams;
                createm_workingparam(paramtab[1], paramtab[2], null);
                bool paramvalue = Convert.ToBoolean(paramtab[2]);

                m_propertyList.Add(new CrmBooleanProperty(paramtab[1].ToString(), new Microsoft.Crm.Sdk.CrmBoolean(paramvalue)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, attribute_type.tBoolean + m_workingparam);
            }


            return true;
        }

        protected bool createMoneyProperties(ArrayList moneyParams)
        {
            //loop to create the Money properties

            try
            {
                ArrayList paramtab = (ArrayList)moneyParams;
                decimal paramvalue = Convert.ToDecimal(paramtab[2]);
                createm_workingparam(paramtab[1], paramtab[2], null);

                m_propertyList.Add(new CrmMoneyProperty(paramtab[1].ToString(), new CrmMoney(paramvalue)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "money" + m_workingparam);
            }


            return true;
        }

        protected bool createDecimalProperties(ArrayList decimalParams)
        {
            //loop to create the Boolean properties

            try
            {
                ArrayList paramtab = (ArrayList)decimalParams;
                createm_workingparam(paramtab[1], paramtab[2], null);
                decimal paramvalue = Convert.ToDecimal(paramtab[2].ToString());
                m_propertyList.Add(new CrmDecimalProperty(paramtab[1].ToString(), new CrmDecimal(paramvalue)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "decimal" + m_workingparam);
            }


            return true;
        }

        protected bool createFloatProperties(ArrayList floatParams)
        {
            //loop to create the Boolean properties

            try
            {
                ArrayList paramtab = (ArrayList)floatParams;
                createm_workingparam(paramtab[1], paramtab[2], null);
                double paramvalue = Convert.ToDouble(paramtab[2]);
                m_propertyList.Add(new CrmFloatProperty(paramtab[1].ToString(), new Microsoft.Crm.Sdk.CrmFloat(paramvalue)));
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, "float" + m_workingparam);
            }


            return true;
        }

        protected bool createPartyListProperties(ArrayList partyListParams)
        {
            //loop to create the Boolean properties

            try
            {
                ArrayList paramtab = (ArrayList)partyListParams;
                createm_workingparam(paramtab[1], paramtab[2], paramtab[3]);
                DynamicEntity activityParty = new DynamicEntity("activityparty");
                LookupProperty partyProperty = new LookupProperty("partyid", new Lookup(paramtab[2].ToString(), new Guid(paramtab[3].ToString())));
                activityParty.Properties.Add(partyProperty);
                DynamicEntityArrayProperty customersProperty = new DynamicEntityArrayProperty(paramtab[1].ToString(), new DynamicEntity[] { activityParty });
                m_propertyList.Add(customersProperty);
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createPropertyException(ex.Message, " partylist" + m_workingparam);
            }


            return true;
        }

        protected bool addProperties(string entityName)
        {

            try
            {
                m_dynamicEntity_ = new DynamicEntity(entityName);

                foreach (object param in m_propertyList)
                {

                    m_workingparam = param.ToString();
                    m_dynamicEntity_.Properties.Add((Property)param);


                }

            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.addPropertyException(ex.Message, m_workingparam);
            }
            return true;
        }

        protected bool createStringConditionExpression(ArrayList stringParams)
        {
            try
            {
                for (int i = 0; i < stringParams.Count; i++)
                {
                    ConditionExpression conditionEx = new ConditionExpression();
                    ArrayList paramtab = (ArrayList)stringParams[i];
                    //string[] paramtab = stringParams[i].ToString().Split('\0');
                    conditionEx.AttributeName = paramtab[0].ToString();
                    conditionEx.Operator = ConditionOperator.Like;
                    conditionEx.Values = new object[] { paramtab[1].ToString() }; //replace object by string
                    m_filterExpression.Conditions.Add(conditionEx);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ESC_CRM_EX.createStringConditionException(ex.Message);
            }
        }

        protected void isconnected()
        {
            try
            {
                if (!m_isconnected) throw new ESC_CRM_EX.notConnectedException();
            }
            catch (ESC_CRM_EX.notConnectedException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected void createm_workingparam(object s1, object s2, object s3)
        {
            if (s3 == null)
            {
                m_workingparam = " property\nAttribute \t:" + s1 + "\nValue \t\t:" + s2;
            }
            else
            {
                m_workingparam = " property\nParam 1 \t:" + s1 + "\nParam 2 \t:" + s2 + "\nParam 3 \t:" + s3;
            }
        }

      
        public bool campaignExists(string campaignID)
        {
            try
            {
                isconnected();

                ArrayList arrayResults = new ArrayList();
                // Create the retrieve target.
                TargetRetrieveDynamic targetRetrieve = new TargetRetrieveDynamic();

                // Set the properties of the target.
                targetRetrieve.EntityName = "campaign";
                targetRetrieve.EntityId = new Guid(campaignID);

                // Create the request object.
                RetrieveRequest retrieve = new RetrieveRequest();
                ColumnSet col = new ColumnSet();

                // Set the properties of the request object.
                retrieve.Target = targetRetrieve;
                col.AddColumn("name");

                retrieve.ColumnSet = col;

                // Indicate that the BusinessEntity should be retrieved as a
                // DynamicEntity.
                retrieve.ReturnDynamicEntities = true;

                // Execute the request.
                RetrieveResponse retrieved = (RetrieveResponse)m_crmService.Execute(retrieve);

                // Extract the DynamicEntity from the request.
                DynamicEntity entity = (DynamicEntity)retrieved.BusinessEntity;
                if (entity.Properties["name"].Equals(null)) return false;
                return true;
             }
            catch (Exception)
            {
                return false;
            }

        }
    }
}