// <copyright file="RouteLeadActivity.cs" company="">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author></author>
// <date>10/26/2018 8:07:20 AM</date>
// <summary>Implements the RouteLeadActivity Workflow Activity.</summary>
namespace CrmLeadPackage.RouteLead
{
    using System;
    using System.Activities;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Workflow;

    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Client;  

    public sealed class RouteLeadActivity : CodeActivity
    {
        private QueryBase userQuery;

        /// <summary>
        /// Executes the workflow activity.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            if (tracingService == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve tracing service.");
            }

            tracingService.Trace("Entered RouteLeadActivity.Execute(), Activity Instance Id: {0}, Workflow Instance Id: {1}",
                executionContext.ActivityInstanceId,
                executionContext.WorkflowInstanceId);

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            if (context == null)
            {
                throw new InvalidPluginExecutionException("Failed to retrieve workflow context.");
            }

            tracingService.Trace("RouteLeadActivity.Execute(), Correlation Id: {0}, Initiating User: {1}",
                context.CorrelationId,
                context.InitiatingUserId);

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // TODO: Implement your custom Workflow business logic.
                
                // Set the Counters  
                int currentLeadCount = 0;
                int lowLeadCount=-1;
                Guid lowUserId = Guid.Empty;
                Guid leadId = context.PrimaryEntityId;

                // Gets the list of users  
                EntityCollection bec = service.RetrieveMultiple(userQuery);
                foreach (Entity e in bec.Entities)
                {
                    QueryExpression qe = new QueryExpression();
                    qe.EntityName = "lead";
                    qe.ColumnSet = new ColumnSet();
                    qe.ColumnSet.AllColumns = true;
                    qe.Criteria = new FilterExpression();
                    qe.Criteria.FilterOperator = LogicalOperator.And;
                    ConditionExpression ce = new ConditionExpression("ownerid", ConditionOperator.Equal, e.Attributes["systemuserid"]);
                    qe.Criteria.Conditions.Add(ce);
                    EntityCollection ecLead = service.RetrieveMultiple(qe);
                    currentLeadCount = ecLead.Entities.Count;
                    //if the first user, the user is marked the lowest      
                    if (lowLeadCount == -1)
                    {
                        lowLeadCount = currentLeadCount;
                        lowUserId = new Guid(e.Attributes["systemuserid"].ToString( ));
                    }
                    // if the number of leads is lowest, the current user is marked lowest      
                    if (currentLeadCount < lowLeadCount)
                    {
                        lowLeadCount = currentLeadCount;
                        lowUserId = new Guid(e.Attributes["systemuserid"].ToString( ));
                    } 
                }
                //Route the lead 
                if (lowUserId != Guid.Empty)
                {
                    AssignRequest assignRequest = new AssignRequest()
                    {
                        Assignee = new EntityReference("systemuser", lowUserId),
                        Target = new EntityReference("lead",leadId)
                    };
                    AssignResponse assignResponse = (AssignResponse) service.Execute(assignRequest);
                }  

            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());

                // Handle the exception.
                throw;
            }

            tracingService.Trace("Exiting RouteLeadActivity.Execute(), Correlation Id: {0}", context.CorrelationId);
        }
    }
}