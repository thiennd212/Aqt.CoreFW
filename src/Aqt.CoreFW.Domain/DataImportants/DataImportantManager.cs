using System;
using System.Threading.Tasks;
using Aqt.CoreFW.DataImportants; // Namespaces cần thiết
using Aqt.CoreFW.Domain.DataImportants.Entities;
using Aqt.CoreFW.Domain.DataGroups; // Để inject IDataGroupRepository
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Users; // Để inject ICurrentUser nếu cần kiểm tra quyền phức tạp hơn
using Volo.Abp.Localization;
using Aqt.CoreFW.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Domain.Repositories;

namespace Aqt.CoreFW.Domain.DataImportants;

/// <summary>
/// Domain service responsible for managing DataImportant entities.
/// </summary>
public class DataImportantManager : DomainService
{
    private readonly IDataImportantRepository _dataImportantRepository;
    private readonly IDataGroupRepository _dataGroupRepository; // Inject repository của DataGroup
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    // private readonly ICurrentUser _currentUser; // Inject nếu cần

    public DataImportantManager(
        IDataImportantRepository dataImportantRepository,
        IDataGroupRepository dataGroupRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer/*,
        ICurrentUser currentUser*/)
    {
        _dataImportantRepository = dataImportantRepository;
        _dataGroupRepository = dataGroupRepository;
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        // _currentUser = currentUser;

        // Gán localizer cho lớp DomainService để có thể sử dụng L["..."]
        // LocalizationResource = typeof(CoreFWResource); // Dòng này không cần thiết và gây lỗi khi đã inject IStringLocalizer
    }

    /// <summary>
    /// Creates a new DataImportant entity after validating business rules.
    /// </summary>
    /// <param name="code">Unique code within the DataGroup.</param>
    /// <param name="name">Name.</param>
    /// <param name="dataGroupId">Parent DataGroup ID.</param>
    /// <param name="order">Display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">Status.</param>
    /// <returns>The newly created DataImportant entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<DataImportant> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        Guid dataGroupId,
        int order = 0,
        [CanBeNull] string? description = null,
        DataImportantStatus status = DataImportantStatus.Active)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), DataImportantConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(name, nameof(name), DataImportantConsts.MaxNameLength);
        Check.Length(description, nameof(description), DataImportantConsts.MaxDescriptionLength);

        // 1. Validate DataGroup existence
        await ValidateDataGroupExistsAsync(dataGroupId);

        // 2. Validate Code uniqueness within the specified DataGroup
        await ValidateCodeUniquenessAsync(code, dataGroupId);

        var dataImportant = new DataImportant(
            _guidGenerator.Create(),
            code,
            name,
            dataGroupId,
            order,
            description,
            status
        );

        // Application Service sẽ gọi Repository.InsertAsync
        return dataImportant;
    }

    /// <summary>
    /// Updates an existing DataImportant entity after validating business rules.
    /// </summary>
    /// <param name="dataImportant">The entity to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="dataGroupId">New parent DataGroup ID.</param>
    /// <param name="order">New display order.</param>
    /// <param name="description">New optional description.</param>
    /// <param name="status">New status.</param>
    /// <returns>The updated DataImportant entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<DataImportant> UpdateAsync(
        [NotNull] DataImportant dataImportant,
        [NotNull] string name,
        Guid dataGroupId, // Cho phép thay đổi DataGroup
        int order,
        [CanBeNull] string? description,
        DataImportantStatus status)
    {
        Check.NotNull(dataImportant, nameof(dataImportant));
        Check.NotNullOrWhiteSpace(name, nameof(name), DataImportantConsts.MaxNameLength);
        Check.Length(description, nameof(description), DataImportantConsts.MaxDescriptionLength);

        // 1. Validate DataGroup existence if it's changed AND check Code uniqueness in the NEW group
        if (dataImportant.DataGroupId != dataGroupId)
        {
            await ValidateDataGroupExistsAsync(dataGroupId);
            // Kiểm tra Code có bị trùng trong Group mới không (bỏ qua chính nó vì Code không đổi)
            await ValidateCodeUniquenessAsync(dataImportant.Code, dataGroupId, dataImportant.Id);
            dataImportant.SetDataGroupIdInternal(dataGroupId); // Change DataGroup ID via internal setter
        }

        // Update other properties
        dataImportant.SetName(name);
        dataImportant.SetOrder(order);
        dataImportant.SetDescription(description);

        if (status == DataImportantStatus.Active) dataImportant.Activate();
        else dataImportant.Deactivate();

        // Application Service sẽ gọi Repository.UpdateAsync
        return dataImportant;
    }

    /// <summary>
    /// Changes the DataGroup for an existing DataImportant.
    /// Ensures the code is unique in the new DataGroup.
    /// </summary>
    public async Task ChangeDataGroupAsync([NotNull] DataImportant dataImportant, Guid newDataGroupId)
    {
         Check.NotNull(dataImportant, nameof(dataImportant));

         if (dataImportant.DataGroupId == newDataGroupId)
         {
             return; // No change needed
         }

         await ValidateDataGroupExistsAsync(newDataGroupId);
         await ValidateCodeUniquenessAsync(dataImportant.Code, newDataGroupId, dataImportant.Id); // Check uniqueness in new group

         dataImportant.SetDataGroupIdInternal(newDataGroupId);
         // Repository.UpdateAsync will be called by the Application Service
    }


    /// <summary>
    /// Helper method to validate if a DataGroup exists.
    /// </summary>
    private async Task ValidateDataGroupExistsAsync(Guid dataGroupId)
    {
        // Sửa lỗi linter: Sử dụng FindAsync thay vì AnyAsync nếu AnyAsync không có sẵn
        var dataGroup = await _dataGroupRepository.FindAsync(dataGroupId);
        if (dataGroup == null)
        {
            // Sử dụng UserFriendlyException với localization key
            throw new UserFriendlyException(_localizer["DataGroupNotFoundForImportant", dataGroupId]);
            // Hoặc dùng BusinessException với mã lỗi (nếu cần xử lý đặc biệt hơn):
            // throw new BusinessException(CoreFWDomainErrorCodes.DataGroupNotFoundForImportant).WithData("id", dataGroupId);
        }
    }

    /// <summary>
    /// Helper method to validate code uniqueness within a DataGroup.
    /// </summary>
    private async Task ValidateCodeUniquenessAsync([NotNull] string code, Guid dataGroupId, Guid? excludeId = null)
    {
         if (await _dataImportantRepository.CodeExistsAsync(code, dataGroupId, excludeId))
        {
            // Sử dụng UserFriendlyException với localization key
            throw new UserFriendlyException(_localizer["DataImportantCodeAlreadyExists", code]);
            // Hoặc dùng BusinessException với mã lỗi:
            // throw new BusinessException(CoreFWDomainErrorCodes.DataImportantCodeAlreadyExists).WithData("code", code);
        }
    }
} 