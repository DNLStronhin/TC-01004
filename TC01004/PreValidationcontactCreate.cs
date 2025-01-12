
// <copyright file="PreValidationcontactCreate.cs" company="">
// Copyright (c) 2024 All Rights Reserved
// </copyright>
// <author></author>
// <date>6/24/2024 4:32:09 PM</date>
// <summary>Implements the PreValidationcontactCreate Plugin.</summary>
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TC_01004.TC01004
{

    /// <summary>
    /// PreValidationcontactCreate Plugin.
    /// </summary>    
    public class PreValidationcontactCreate: PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreValidationcontactCreate"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information.</param>
        public PreValidationcontactCreate(string unsecure, string secure)
            : base(typeof(PreValidationcontactCreate))
        {
            
           // TODO: Implement your custom configuration handling.
        }


        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// </remarks>
        protected override void ExecuteCdsPlugin(ILocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException(nameof(localContext));
            }           
            // Obtain the tracing service
            ITracingService tracingService = localContext.TracingService;

            try
            { 
                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)localContext.PluginExecutionContext;

                // Obtain the organization service reference for web service calls.  
                IOrganizationService currentUserService = localContext.CurrentUserService;

                if (context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is EntityReference)
                {
                    Relationship data = context.InputParameters["Relationship"] as Relationship;
                    var schemaName = data.SchemaName;
                    if (schemaName != "new_Account_Contact_Contact")
                    {
                        return;
                    }
                    var contactRef = context.InputParameters["Target"] as EntityReference;
                    var contact = currentUserService.Retrieve("contact", contactRef.Id, new ColumnSet(new string[] { "fullname", "emailaddress1" }));
                    var accountRefs = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                    var account = currentUserService.Retrieve("account", accountRefs[0].Id, new ColumnSet(new string[] { "name" })); ;

                    Entity email = new Entity("email");
                    email["subject"] = $"Account has been associated {account.GetAttributeValue<string>("name")} with contact {contact.GetAttributeValue<string>("fullname")}";
                    email["description"] = $"Account has been added - https://orgd517a8d5.crm11.dynamics.com/main.aspx?appid=30cf0aa1-1d32-ef11-8e4e-6045bd0d3e34&pagetype=entityrecord&etn=account&id={accountRefs[0].Id}";

                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference("systemuser", context.UserId);
                    EntityCollection from = new EntityCollection(new List<Entity>() { fromParty });
                    email["from"] = from;
                    Entity toParty = new Entity("activityparty");
                    toParty["partyid"] = new EntityReference("contact", contactRef.Id);
                    EntityCollection to = new EntityCollection(new List<Entity>() { toParty });
                    email["to"] = to;
                    currentUserService.Create(email);
                }


            }	
            // Only throw an InvalidPluginExecutionException. Please Refer https://go.microsoft.com/fwlink/?linkid=2153829.
            catch (Exception ex)
            {
                tracingService?.Trace("An error occurred executing Plugin TC_01004.TC01004.PreValidationcontactCreate : {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred executing Plugin TC_01004.TC01004.PreValidationcontactCreate .", ex);
            }	
        }
    }
}
