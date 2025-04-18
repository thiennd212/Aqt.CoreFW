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
    public class BasicPluginFactory : IBasicPluginFactory
    {
        private readonly IBasicPluginHandler _handler;
        public BasicPluginFactory(IBasicPluginHandler handler)
        {
            _handler = handler;
        }

        public BasicPlugin Create()
        {
            var plugin = new BasicPlugin();

            plugin.WithActors(new List<string> { "Author" });

            plugin.UsersInRoleAsync += async (roleName, _) =>
            {
                return await _handler.GetUserIdsInRoleAsync(roleName);
            };

            plugin.CheckPredefinedActorAsync += async (pi, rt, actorName, identityId) =>
            {
                return await _handler.CheckActorIsAuthorAsync(identityId, actorName, pi.IdentityId);
            };

            return plugin;
        }
    }
}
