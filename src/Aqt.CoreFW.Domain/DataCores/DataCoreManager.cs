using System;
using System.Threading.Tasks;
using Aqt.CoreFW.DataCores;
using Aqt.CoreFW.Domain.DataCores.Entities;
using Aqt.CoreFW.Domain.DataGroups; // Để inject IDataGroupRepository
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Localization; // Needed for L
using Aqt.CoreFW.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Domain.Repositories; // Needed for CoreFWResource

namespace Aqt.CoreFW.Domain.DataCores;

/// <summary>
/// Domain service responsible for managing DataCore entities.
/// </summary>
public class DataCoreManager : DomainService
{
    private readonly IDataCoreRepository _dataCoreRepository;
    private readonly IDataGroupRepository _dataGroupRepository; // Inject repository của DataGroup
    private readonly IGuidGenerator _guidGenerator;

    // Inject IStringLocalizer<CoreFWResource> for localization
    private readonly IStringLocalizer<CoreFWResource> L;

    public DataCoreManager(
        IDataCoreRepository dataCoreRepository,
        IDataGroupRepository dataGroupRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> stringLocalizer) // Inject localizer
    {
        _dataCoreRepository = dataCoreRepository;
        _dataGroupRepository = dataGroupRepository;
        _guidGenerator = guidGenerator;
        L = stringLocalizer;
    }

    /// <summary>
    /// Creates a new DataCore entity after validating business rules.
    /// </summary>
    /// <param name="code">Unique code.</param>
    /// <param name="name">Name.</param>
    /// <param name="dataGroupId">Parent DataGroup ID.</param>
    /// <param name="order">Display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">Status.</param>
    /// <returns>The newly created DataCore entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<DataCore> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        Guid dataGroupId,
        int order = 0,
        [CanBeNull] string? description = null,
        DataCoreStatus status = DataCoreStatus.Active)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), DataCoreConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(name, nameof(name), DataCoreConsts.MaxNameLength);
        Check.Length(description, nameof(description), DataCoreConsts.MaxDescriptionLength);

        // 1. Validate DataGroup existence
        await ValidateDataGroupExistsAsync(dataGroupId);

        // 2. Validate Code uniqueness (assuming global uniqueness)
        if (await _dataCoreRepository.CodeExistsAsync(code))
        {
            throw new UserFriendlyException(L["DataCoreCodeAlreadyExists", code]);
            // Hoặc dùng mã lỗi: throw new BusinessException(CoreFWDomainErrorCodes.DataCoreCodeAlreadyExists).WithData("code", code);
        }

        var dataCore = new DataCore(
            _guidGenerator.Create(),
            code,
            name,
            dataGroupId,
            order,
            description,
            status
        );

        // Application Service will call Repository.InsertAsync
        return dataCore;
    }

    /// <summary>
    /// Updates an existing DataCore entity after validating business rules.
    /// </summary>
    /// <param name="dataCore">The entity to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="dataGroupId">New parent DataGroup ID.</param>
    /// <param name="order">New display order.</param>
    /// <param name="description">New optional description.</param>
    /// <param name="status">New status.</param>
    /// <returns>The updated DataCore entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<DataCore> UpdateAsync(
        [NotNull] DataCore dataCore,
        [NotNull] string name,
        Guid dataGroupId, // Allow changing DataGroup
        int order,
        [CanBeNull] string? description,
        DataCoreStatus status)
    {
        Check.NotNull(dataCore, nameof(dataCore));
        Check.NotNullOrWhiteSpace(name, nameof(name), DataCoreConsts.MaxNameLength);
        Check.Length(description, nameof(description), DataCoreConsts.MaxDescriptionLength);

        // 1. Validate DataGroup existence if it's changed
        if (dataCore.DataGroupId != dataGroupId)
        {
            await ValidateDataGroupExistsAsync(dataGroupId);
            dataCore.SetDataGroupIdInternal(dataGroupId); // Change DataGroup ID via internal setter
        }

        // Update other properties
        dataCore.SetName(name);
        dataCore.SetOrder(order);
        dataCore.SetDescription(description);

        if (status == DataCoreStatus.Active) dataCore.Activate();
        else dataCore.Deactivate();

        // Application Service will call Repository.UpdateAsync
        return dataCore;
    }

    /// <summary>
    /// Helper method to validate if a DataGroup exists.
    /// </summary>
    private async Task ValidateDataGroupExistsAsync(Guid dataGroupId)
    {
        var dataGroupExists = await _dataGroupRepository.AnyAsync(dg => dg.Id == dataGroupId);
        if (!dataGroupExists)
        {
             throw new UserFriendlyException(L["DataGroupNotFound", dataGroupId]);
            // Hoặc dùng mã lỗi: throw new BusinessException(CoreFWDomainErrorCodes.DataGroupNotFound).WithData("id", dataGroupId);
        }
    }

    // Other business methods can be added here if needed
} 