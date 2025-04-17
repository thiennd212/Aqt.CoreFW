using Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;
using Aqt.CoreFW.Domain.JobTitles.Entities;
using Aqt.CoreFW.Localization;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Aqt.CoreFW.Application.JobTitles
{
    public class JobTitleToExcelMappingAction : IMappingAction<JobTitle, JobTitleExcelDto>, ITransientDependency
    {
        private readonly IStringLocalizer<CoreFWResource> _l;

        public JobTitleToExcelMappingAction(IStringLocalizer<CoreFWResource> l)
        {
            _l = l;
        }

        public void Process(JobTitle source, JobTitleExcelDto destination, ResolutionContext context)
        {
            destination.IsActiveText = source.IsActive ? _l["Active"] : _l["Inactive"];
        }
    }
}
