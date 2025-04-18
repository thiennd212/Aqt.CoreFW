using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aqt.CoreFW.Application.Contracts.Workflows.Dtos
{
    public class UserRoleInfoDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string OrganizationUnitName { get; set; }
    }
}
