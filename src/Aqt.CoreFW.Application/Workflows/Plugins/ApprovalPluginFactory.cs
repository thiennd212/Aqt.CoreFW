using Aqt.CoreFW.Application.Workflows.Interfaces;
using OptimaJet.Workflow.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Aqt.CoreFW.Application.Workflows.Plugins
{
    public class ApprovalPluginFactory : IApprovalPluginFactory
    {
        private readonly IApprovalPluginHandler _handler;
        public ApprovalPluginFactory(IApprovalPluginHandler handler)
        {
            _handler = handler;
        }
        public ApprovalPlugin Create()
        {
            var plugin = new ApprovalPlugin
            {
                AutoApprovalHistory = true,
                NameParameterForComment = "comment"
            };

            plugin.GetUserNamesByIds += _handler.GetUserNamesByIds;

            return plugin;
        }
    }
}
