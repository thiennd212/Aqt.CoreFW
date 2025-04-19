using System;
using System.Collections.Generic;
using System.IO; // Required for MemoryStream
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses;
using Aqt.CoreFW.Application.Contracts.WorkflowStatuses.Dtos;
using Aqt.CoreFW.Domain.Shared; // Keep for potential direct error code use, though L[] is preferred
using Aqt.CoreFW.Domain.WorkflowStatuses;
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities;
using Aqt.CoreFW.Localization;
using Aqt.CoreFW.Permissions;
using Microsoft.AspNetCore.Authorization;
using MiniExcelLibs; // Required for MiniExcel
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content; // Required for IRemoteStreamContent
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace Aqt.CoreFW.Application.WorkflowStatuses;

/// <summary>
/// Implements the application service for managing Workflow Statuses.
/// </summary>
[Authorize(CoreFWPermissions.WorkflowStatuses.Default)] // Default permission for the service
public class WorkflowStatusAppService :
    CrudAppService<
        WorkflowStatus,
        WorkflowStatusDto,
        Guid,
        GetWorkflowStatusesInput,
        CreateUpdateWorkflowStatusDto>,
    IWorkflowStatusAppService
{
    private readonly IWorkflowStatusRepository _workflowStatusRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IClock _clock; // Inject IClock

    public WorkflowStatusAppService(
        IRepository<WorkflowStatus, Guid> repository,
        IWorkflowStatusRepository workflowStatusRepository,
        IGuidGenerator guidGenerator,
        IClock clock)
        : base(repository)
    {
        _workflowStatusRepository = workflowStatusRepository;
        _guidGenerator = guidGenerator;
        _clock = clock; // Initialize IClock
        LocalizationResource = typeof(CoreFWResource);

        // Map permission names to base class policies
        GetPolicyName = CoreFWPermissions.WorkflowStatuses.Default;
        GetListPolicyName = CoreFWPermissions.WorkflowStatuses.Default;
        CreatePolicyName = CoreFWPermissions.WorkflowStatuses.Create;
        UpdatePolicyName = CoreFWPermissions.WorkflowStatuses.Edit;
        DeletePolicyName = CoreFWPermissions.WorkflowStatuses.Delete;
    }

    /// <summary>
    /// Creates a new Workflow Status.
    /// </summary>
    [Authorize(CoreFWPermissions.WorkflowStatuses.Create)]
    public override async Task<WorkflowStatusDto> CreateAsync(CreateUpdateWorkflowStatusDto input)
    {
        // Check for duplicates
        if (await _workflowStatusRepository.CodeExistsAsync(input.Code))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.WorkflowStatusCodeAlreadyExists, input.Code]);
        }
        if (await _workflowStatusRepository.NameExistsAsync(input.Name))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.WorkflowStatusNameAlreadyExists, input.Name]);
        }

        var entity = new WorkflowStatus(
            _guidGenerator.Create(),
            input.Code,
            input.Name,
            input.Order,
            input.Description,
            input.ColorCode,
            input.IsActive
        );

        await Repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<WorkflowStatus, WorkflowStatusDto>(entity);
    }

    /// <summary>
    /// Updates an existing Workflow Status.
    /// </summary>
    [Authorize(CoreFWPermissions.WorkflowStatuses.Edit)]
    public override async Task<WorkflowStatusDto> UpdateAsync(Guid id, CreateUpdateWorkflowStatusDto input)
    {
        var entity = await GetEntityByIdAsync(id);

        // Check for duplicates excluding self
        if (entity.Code != input.Code && await _workflowStatusRepository.CodeExistsAsync(input.Code, id))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.WorkflowStatusCodeAlreadyExists, input.Code]);
        }
        if (entity.Name != input.Name && await _workflowStatusRepository.NameExistsAsync(input.Name, id))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.WorkflowStatusNameAlreadyExists, input.Name]);
        }

        // Update using entity methods
        entity.SetCode(input.Code)
              .SetName(input.Name)
              .SetOrder(input.Order)
              .SetDescription(input.Description)
              .SetColorCode(input.ColorCode);

        if (input.IsActive) { entity.Activate(); } else { entity.Deactivate(); }

        await Repository.UpdateAsync(entity, autoSave: true);
        return ObjectMapper.Map<WorkflowStatus, WorkflowStatusDto>(entity);
    }

    /// <summary>
    /// Deletes a Workflow Status.
    /// </summary>
    [Authorize(CoreFWPermissions.WorkflowStatuses.Delete)]
    public override async Task DeleteAsync(Guid id)
    {
        // Check if in use before deleting
        if (await _workflowStatusRepository.IsInUseAsync(id))
        {
            var entity = await Repository.GetAsync(id); // Get details for error message
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CannotDeleteWorkflowStatusInUse, entity.Name ?? entity.Code]);
        }

        await base.DeleteAsync(id); // Base handles soft delete
    }

    /// <summary>
    /// Provides a lookup list of active Workflow Statuses.
    /// </summary>
    [AllowAnonymous] // Consider changing if auth needed for lookup
    public async Task<ListResultDto<WorkflowStatusLookupDto>> GetLookupAsync()
    {
        var statuses = await _workflowStatusRepository.GetListAsync(
            isActive: true,
            sorting: $"{nameof(WorkflowStatus.Order)} ASC, {nameof(WorkflowStatus.Name)} ASC"
        );
        var lookupDtos = ObjectMapper.Map<List<WorkflowStatus>, List<WorkflowStatusLookupDto>>(statuses);
        return new ListResultDto<WorkflowStatusLookupDto>(lookupDtos);
    }

    /// <summary>
    /// Gets a paged list of Workflow Statuses based on the provided input filters.
    /// Overrides the base GetListAsync to use the custom repository method for filtering.
    /// </summary>
    public override async Task<PagedResultDto<WorkflowStatusDto>> GetListAsync(GetWorkflowStatusesInput input)
    {
        var totalCount = await _workflowStatusRepository.GetCountAsync(
            filterText: input.Filter,
            isActive: input.IsActive
        );
        var statuses = await _workflowStatusRepository.GetListAsync(
            filterText: input.Filter,
            isActive: input.IsActive,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );
        var statusDtos = ObjectMapper.Map<List<WorkflowStatus>, List<WorkflowStatusDto>>(statuses);
        return new PagedResultDto<WorkflowStatusDto>(totalCount, statusDtos);
    }

    // Note: We override GetListAsync completely, so no need to override CreateFilteredQuery or ApplySorting.

    /// <summary>
    /// Exports the list of Workflow Statuses to an Excel file.
    /// </summary>
    /// <param name="input">Filtering and sorting parameters.</param>
    /// <returns>An IRemoteStreamContent containing the Excel file.</returns>
    [Authorize(CoreFWPermissions.WorkflowStatuses.ExportExcel)] // Ensure this permission is defined and granted
    public async Task<IRemoteStreamContent> GetListAsExcelAsync(GetWorkflowStatusesInput input)
    {
        // 1. Get the filtered and sorted list (fetch all matching records for export)
        var statuses = await _workflowStatusRepository.GetListAsync(
            filterText: input.Filter,
            isActive: input.IsActive,
            sorting: input.Sorting,
            maxResultCount: int.MaxValue, // Fetch all matching records
            skipCount: 0
        );

        // 2. Map the entity list to the Excel DTO list using ObjectMapper
        // The WorkflowStatusToExcelMappingAction will be automatically applied
        var excelDtos = ObjectMapper.Map<List<WorkflowStatus>, List<WorkflowStatusExcelDto>>(statuses);

        // 3. Tạo MemoryStream để lưu file Excel
        var stream = new MemoryStream();

        // 4. Sử dụng MiniExcel để lưu danh sách DTO vào stream
        await stream.SaveAsAsync(excelDtos, sheetName: "WorkflowStatuses");

        // 5. Reset vị trí stream về đầu để đọc được
        stream.Seek(0, SeekOrigin.Begin);

        // 6. Tạo tên file động
        var fileName = $"WorkflowStatuses_{_clock.Now:yyyyMMdd_HHmmss}.xlsx";

        // 7. Trả về RemoteStreamContent sử dụng MimeTypeNames
        return new RemoteStreamContent(
            stream,
            fileName,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );
    }
}