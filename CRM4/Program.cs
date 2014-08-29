public ESC_CRM crm;
crm = new ESC_CRM(connecParams["crm.URL"], connecParams["crm.metadataservice"], connecParams["crm.Name"], connecParams["crm.User"], connecParams["crm.Password"], connecParams["crm.Domain"]);
crm.connect();
if (!crm.campaignExists(connecParams["campaignID"]));


 /*PARAMETERS FOR LEAD*/
ArrayList paramsLead = new ArrayList() { 
	new ArrayList() { attribute_type.tString,"subject", lastname.Value.ToString() + ", " + firstname.Value.ToString() + " : "+campaignname},
	new ArrayList() { attribute_type.tString,"firstname", firstname.Value.ToString() }, 
	new ArrayList() { attribute_type.tString,"middlename", middlename.Value.ToString() },
	new ArrayList() { attribute_type.tString,"lastname", lastname.Value.ToString() },
	new ArrayList() { attribute_type.tString,"mobilephone", mobilephone_prefix.Value.ToString()+" "+ mobilephone.Value.ToString() },
	new ArrayList() { attribute_type.tString,"telephone1", telephone_prefix.Value.ToString()+" "+telephone.Value.ToString() },
	new ArrayList() { attribute_type.tString,"telephone2", telephone2_prefix.Value.ToString()+" "+ telephone2.Value.ToString() },
	new ArrayList() { attribute_type.tString,"emailaddress1", emailaddress.Value.ToString()},
	new ArrayList() { attribute_type.tString,"address1_line1", streetaddress1.Value.ToString()},
	new ArrayList() { attribute_type.tString,"address1_line2", streetaddress2.Value.ToString()},
	new ArrayList() { attribute_type.tString,"address1_postalcode", postalcode.Value.ToString()},
	new ArrayList() { attribute_type.tString,"address1_city", city.Value.ToString()},
	new ArrayList() { attribute_type.tString,"ssn", ssn.Value.ToString() },
	new ArrayList() { attribute_type.tBoolean,"donotsendmm", false },   
	new ArrayList() { attribute_type.tLookup,"campaignid", "campaign", connecParams["campaignID"]}
};

Guid created_lead = crm.addEntity("lead", paramsLead);

/*PARAMETERS FOR CAMPAIGN RESPONSE*/

ArrayList paramsCampaignResponse = new ArrayList() { 
	new ArrayList() { attribute_type.tString,"subject", lastname.Value.ToString() + ", " + firstname.Value.ToString()+ " : "+campaignname},
	new ArrayList() { attribute_type.tString,"firstname", firstname.Value.ToString() }, 
	new ArrayList() { attribute_type.tString,"lastname", lastname.Value.ToString() },
	new ArrayList() { attribute_type.tString,"msa_middlenameinitial", middlename.Value.ToString() },

	new ArrayList() { attribute_type.tString,"mobilephone",mobilephone_prefix.Value.ToString()+" "+ mobilephone.Value.ToString() },
	new ArrayList() { attribute_type.tString,"telephone", telephone_prefix.Value.ToString()+" "+telephone.Value.ToString() },
	new ArrayList() { attribute_type.tString,"telephone2",telephone2_prefix.Value.ToString()+" "+ telephone2.Value.ToString() },

	new ArrayList() { attribute_type.tString,"emailaddress", emailaddress.Value.ToString()},

	new ArrayList() { attribute_type.tString,"msa_streetaddress1", streetaddress1.Value.ToString()},
	new ArrayList() { attribute_type.tString,"msa_streetaddress2", streetaddress2.Value.ToString()},
	new ArrayList() { attribute_type.tString,"msa_postalcode",postalcode.Value.ToString()},
	new ArrayList() { attribute_type.tString,"msa_city", city.Value.ToString()},
	new ArrayList() { attribute_type.tString,"homecounty", homecounty.Value.ToString()},


	new ArrayList() { attribute_type.tBoolean,"donotusemydata", do_not_use_data },  

	new ArrayList() { attribute_type.tString,"description", extra_info.Text },  

	new ArrayList() { attribute_type.tDateTime,"receivedon", "{0:M.d.yyyy}",DateTime.Now.ToString() },
	new ArrayList() { attribute_type.tLookup,"regardingobjectid", "campaign",connecParams["campaignID"]},
	new ArrayList() { attribute_type.tPartylist,"customer", "lead", created_lead.ToString()}
					};

crm.addEntity("campaignresponse", paramsCampaignResponse);