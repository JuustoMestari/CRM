using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESC_CRM_;
using Microsoft.Xrm.Sdk;
/*
 * 
 *  Might require installation of Windows Identity Foundation
 *  http://www.microsoft.com/en-us/download/details.aspx?id=17331
 *  If you have troubles installing it on Windows 8 
 *  http://www.stratospher.es/blog/post/installing-windows-identity-foundation-on-windows-8-the-certificate-for-the-signer-of-the-message-is-invalid-or-not-found
 */
namespace CRM11
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ESC_CRM11 mycrm = new ESC_CRM11();
                if (mycrm.connect(@"http://address/organization/XRMServices/2011/Organization.svc", "domain", "user", "password"))
                {
                    Console.WriteLine("Connected to CRM !");
                    
                    //Example : creating a campaign response with a lead associated
                    //You will need a valid campaign GUID for this example
                    Guid mycampaign = new Guid("CA1B3C80-AD58-E111-AAAA-0050569732E5");

                    Dictionary<string, object> leadParams = new Dictionary<string, object>();
                    leadParams.Add("subject", "Your subject here");
                    leadParams.Add("firstname", "David");
                    leadParams.Add("lastname", "Davenne");
                    leadParams.Add("mobilephone", "+358 12 1212123");
                    leadParams.Add("emailaddress1", "david@davenne.be");
                    leadParams.Add("address1_line1", "My address, 42");

                    //Two Options type attribute (true/false)
                    leadParams.Add("donotsendmm", false);

                    leadParams.Add("campaignid", "");

					//Creating the lead and saving its GUID to associate with the campaign response later
                    Guid created_lead = mycrm.addEntity("lead", leadParams);
                    Console.WriteLine("Lead created !");

                    /*PARAMETERS FOR CAMPAIGN RESPONSE*/
                    Dictionary<string, object> CRParams = new Dictionary<string, object>();
                    CRParams.Add("subject", "Campaign response Subject");
                    CRParams.Add("firstname", "David");
                    CRParams.Add("lastname", "Davenne");

                    CRParams.Add("description", "Description1\nDescription2");
                    //Date and Time type attribute
                    CRParams.Add("receivedon", DateTime.Now);

                    //Decimal Number type
                    CRParams.Add("exchangerate", Convert.ToDecimal(13.2));

                    //Lookup type
                    EntityReference regardingObj = new EntityReference("campaign", mycampaign);
                    CRParams.Add("regardingobjectid", regardingObj);

                    //OptionSet type
                    CRParams.Add("prioritycode", new OptionSetValue(1));

                    //Party List type attribute
                    Entity customer = new Entity("activityparty");
                    customer["partyid"] = new EntityReference("lead", created_lead);
                    CRParams.Add("customer", new EntityCollection(new List<Entity>() { customer }));


                    mycrm.addEntity("campaignresponse", CRParams);
                    Console.WriteLine("Campaign response created !");
					
					//get Maximum value (set in entities/attributes settings) for a specifit attribute
                    Console.WriteLine("Description max chars : "+mycrm.getAttributeMax("campaignresponse", "description"));
                    Console.WriteLine("Exchange rate max value : " + mycrm.getAttributeMax("campaignresponse", "exchangerate"));

                    Console.WriteLine("LEADs");
                    List<string> leadList = new List<string>();
                    leadList = mycrm.getLeadNames();
                    foreach(string x in leadList)
                        {
                            Console.WriteLine("-" + x);
                        }

                    Console.WriteLine("Option set :");
                    Dictionary<int, string> myOptionSet = new Dictionary<int, string>();
                    myOptionSet = mycrm.getOptionSet("lead", "leadsourcecode");
                    foreach (KeyValuePair<int, string> kvp in myOptionSet)
                    {
                        Console.WriteLine("Key :" + kvp.Key + "-Value :" + kvp.Value.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Can't connect to CRM.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : "+ex.Message);
                
            }
            Console.WriteLine("Done !");
            Console.ReadLine();
        }
    }
}
