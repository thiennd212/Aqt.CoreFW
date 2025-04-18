using Aqt.CoreFW.Application.Workflows.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Aqt.CoreFW.Application.Workflows.Plugins
{
    public class ApprovalPluginHandler : IApprovalPluginHandler
    {
        private readonly IRepository<IdentityUser, Guid> _userRepo;

        public ApprovalPluginHandler(IRepository<IdentityUser, Guid> userRepo)
        {
            _userRepo = userRepo;
        }
        public List<string> GetUserNamesByIds(List<string> ids)
        {
            var names = new List<string>();
            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out var guid))
                {
                    var user = _userRepo.FirstOrDefaultAsync(x => x.Id == guid).Result;
                    if (user != null)
                        names.Add($"{user.Name} {user.Surname}");
                }
            }
            return names;
        }
    }

}
