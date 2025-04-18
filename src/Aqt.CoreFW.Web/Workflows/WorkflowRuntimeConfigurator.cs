using Aqt.CoreFW.Application.Workflows.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Parser;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.Migrator;
using OptimaJet.Workflow.Oracle;
using System;
using System.Xml.Linq;

namespace Aqt.CoreFW.Web.Workflows
{
    public static class WorkflowRuntimeConfigurator
    {
        public static WorkflowRuntime Create(IServiceProvider sp)
        {
            try
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connStr = config.GetConnectionString("Default");

                var basicPlugin = sp.GetRequiredService<IBasicPluginFactory>().Create();
                var approvalPlugin = sp.GetRequiredService<IApprovalPluginFactory>().Create();

                var actionProvider = sp.GetRequiredService<IWorkflowActionProvider>();
                var ruleProvider = sp.GetRequiredService<IWorkflowRuleProvider>();

                var provider = new OracleProvider(connStr);

                var builder = new WorkflowBuilder<XElement>(
                    provider,
                    new XmlWorkflowParser(),
                    provider
                ).WithDefaultCache();

                var runtime = new WorkflowRuntime(Environment.MachineName)
                    .WithBuilder(builder)
                    .WithPersistenceProvider(provider)
                    .WithPlugins(null, basicPlugin, approvalPlugin)
                    .WithActionProvider(actionProvider)
                    .WithRuleProvider(ruleProvider)
                    //.RunMigrations()
                    .Start();

                return runtime;
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>> WorkflowRuntimeConfigurator FAILED: " + ex);
                throw;
            }
        }

        public static WorkflowRuntime CreateEmpty(IServiceProvider sp)
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("Default");

            var provider = new OracleProvider(connStr);

            var builder = new WorkflowBuilder<XElement>(
                provider,
                new XmlWorkflowParser(),
                provider
            ).WithDefaultCache();

            return new WorkflowRuntime(Environment.MachineName)
                .WithBuilder(builder)
                .WithPersistenceProvider(provider);
        }
    }

}
