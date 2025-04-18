using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.Workflows.Interfaces
{
    public interface IBasicPluginHandler : ITransientDependency
    {
        Task<List<string>> GetUserIdsInRoleAsync(string roleName);
        Task<bool> CheckActorIsAuthorAsync(string identityId, string actorName, string processInitiator);
    }

}
