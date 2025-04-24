using System;
using System.Threading.Tasks;
using Volo.Abp.Identity; // Standard OU Manager and Entity
using Aqt.CoreFW.OrganizationUnits; // Enums, Consts, Custom Repo
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectExtending; // Cần cho SetProperty
using Aqt.CoreFW.Domain.OrganizationUnits;
using Volo.Abp.Data; // Namespace chứa IOrganizationUnitRepository và OrganizationUnitExtensionProperties

namespace Aqt.CoreFW.Domain.OrganizationUnits; // Domain Service namespace

/// <summary>
/// Domain service to manage extended properties and related business rules for Organization Units.
/// Works in conjunction with the standard Volo.Abp.Identity.OrganizationUnitManager.
/// </summary>
public class ExtendedOrganizationUnitManager : DomainService
{
    private readonly IOrganizationUnitRepository _organizationUnitRepository;
    // Inject OrganizationUnitManager chuẩn nếu cần gọi trực tiếp các phương thức phức tạp của nó
    // private readonly OrganizationUnitManager _standardOrganizationUnitManager;

    public ExtendedOrganizationUnitManager(IOrganizationUnitRepository organizationUnitRepository)
                                            // OrganizationUnitManager standardOrganizationUnitManager)
    {
        _organizationUnitRepository = organizationUnitRepository;
        // _standardOrganizationUnitManager = standardOrganizationUnitManager;
    }

    /// <summary>
    /// Sets the extended properties for a newly created or existing OrganizationUnit.
    /// This should be called AFTER the standard OrganizationUnit is created or fetched.
    /// </summary>
    /// <param name="organizationUnit">The OrganizationUnit entity instance.</param>
    /// <param name="manualCode">Manual code (optional, checked for uniqueness if provided).</param>
    /// <param name="status">Status (required).</param>
    /// <param name="order">Order (required).</param>
    /// <param name="description">Description (optional).</param>
    /// <param name="lastSyncedTime">Sync info (optional).</param>
    /// <param name="syncRecordId">Sync info (optional).</param>
    /// <param name="syncRecordCode">Sync info (optional).</param>
    /// <returns>The updated OrganizationUnit entity.</returns>
    /// <exception cref="BusinessException">Thrown if manual code exists.</exception>
    public async Task<OrganizationUnit> SetExtendedPropertiesAsync(
        [NotNull] OrganizationUnit organizationUnit,
        [CanBeNull] string? manualCode,
        OrganizationUnitStatus status,
        int order,
        [CanBeNull] string? description = null,
        [CanBeNull] DateTime? lastSyncedTime = null,
        [CanBeNull] string? syncRecordId = null, // Sửa kiểu dữ liệu nếu cần
        [CanBeNull] string? syncRecordCode = null)
    {
        Check.NotNull(organizationUnit, nameof(organizationUnit));

        // 1. Validate Manual Code uniqueness if provided and changed
        var existingManualCode = organizationUnit.GetProperty<string>(OrganizationUnitExtensionProperties.ManualCode);
        if (!string.IsNullOrWhiteSpace(manualCode) && manualCode != existingManualCode)
        {
            await CheckManualCodeDuplicationAsync(manualCode, organizationUnit.Id);
            // Validate length
             Check.Length(manualCode, nameof(manualCode), Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxManualCodeLength);
            organizationUnit.SetProperty(OrganizationUnitExtensionProperties.ManualCode, manualCode, false); // SetProperty(name, value, validate: false) - validation đã làm thủ công
        }
         else if (string.IsNullOrWhiteSpace(manualCode) && !string.IsNullOrWhiteSpace(existingManualCode)) // Handle clearing the code
        {
            organizationUnit.SetProperty(OrganizationUnitExtensionProperties.ManualCode, null, false);
        }


        // 2. Validate other extended properties
        Check.Length(description, nameof(description), Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxDescriptionLength);
        Check.Length(syncRecordId, nameof(syncRecordId), Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxSyncRecordIdLength);
        Check.Length(syncRecordCode, nameof(syncRecordCode), Aqt.CoreFW.OrganizationUnits.OrganizationUnitConsts.MaxSyncRecordCodeLength);

        // 3. Set properties using Object Extension methods
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.Status, status, false);
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.Order, order, false);
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.Description, description, false);
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.LastSyncedTime, lastSyncedTime, false);
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.SyncRecordId, syncRecordId, false);
        organizationUnit.SetProperty(OrganizationUnitExtensionProperties.SyncRecordCode, syncRecordCode, false);

        // Repository.UpdateAsync sẽ được gọi bởi UoW nếu entity thay đổi
        return organizationUnit;
    }

    /// <summary>
    /// Helper method to check for Manual Code duplication.
    /// </summary>
    /// <exception cref="BusinessException">Thrown if the manual code already exists.</exception>
    public async Task CheckManualCodeDuplicationAsync([NotNull] string manualCode, Guid? excludedId = null)
    {
         Check.NotNullOrWhiteSpace(manualCode, nameof(manualCode)); // Mã thủ công không được rỗng nếu kiểm tra

        if (await _organizationUnitRepository.ManualCodeExistsAsync(manualCode, excludedId))
        {
            throw new BusinessException(CoreFWDomainErrorCodes.OrganizationUnitManualCodeAlreadyExists)
                .WithData("manualCode", manualCode);
        }
    }

    // --- Lưu ý ---
    // Việc tạo (Create), cập nhật tên (Update DisplayName), di chuyển (Move), xóa (Delete)
    // OrganizationUnit cơ bản nên được thực hiện thông qua Volo.Abp.Identity.OrganizationUnitManager
    // chuẩn, được gọi từ tầng Application Service.
    // Service này (ExtendedOrganizationUnitManager) chủ yếu tập trung vào việc quản lý
    // các thuộc tính mở rộng và các quy tắc liên quan đến chúng (như ManualCode uniqueness).
    // Ví dụ: Application Service khi tạo OU sẽ:
    // 1. Gọi CheckManualCodeDuplicationAsync (nếu manualCode được cung cấp).
    // 2. Gọi _standardOuManager.CreateAsync(...).
    // 3. Gọi SetExtendedPropertiesAsync(...) trên entity vừa tạo.
    // Tương tự cho Update.
} 