using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptimaJet.Workflow.Core.Runtime;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Aqt.CoreFW.Application.Workflows.Providers
{
    public class AutoCompleteProvider: IDesignerAutocompleteProvider
    {
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly List<string> _cachedRoles;
        public AutoCompleteProvider(IRepository<IdentityRole> roleRepository)
        {
            _roleRepository = roleRepository;
            // Load 1 lần duy nhất khi khởi tạo (blocking)
            _cachedRoles = _roleRepository.GetListAsync().Result.Select(r => r.Name).ToList(); // ✅ Cho phép tại constructor
        }
        public List<string> GetAutocompleteSuggestions(SuggestionCategory category, string value, string schemeCode)
        {
            if (category == SuggestionCategory.RuleParameter && value == "CheckRole")
            {
                return _cachedRoles;
            }
            
            return new List<string>();
        }
    }
}
