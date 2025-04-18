using Aqt.CoreFW.Application.Workflows.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Aqt.CoreFW.Application.Workflows.Plugins
{
    public class BasicPluginHandler : IBasicPluginHandler
    {
        private readonly IRepository<IdentityUser, Guid> _userRepo;
        private readonly IIdentityRoleRepository _roleRepo;

        public BasicPluginHandler(IRepository<IdentityUser, Guid> userRepo,
                                  IIdentityRoleRepository roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        public async Task<List<string>> GetUserIdsInRoleAsync(string roleName)
        {
            var role = await _roleRepo.FindByNormalizedNameAsync(roleName.ToUpper());
            if (role == null) return new();

            var users = await _userRepo.GetListAsync();
            return users.Where(u => u.Roles.Any(r => r.RoleId == role.Id))
                        .Select(x => x.Id.ToString())
                        .ToList();
        }

        public Task<bool> CheckActorIsAuthorAsync(string identityId, string actorName, string initiator)
        {
            if (actorName == "Author")
                return Task.FromResult(identityId == initiator);
            return Task.FromResult(false);
        }
    }

}
