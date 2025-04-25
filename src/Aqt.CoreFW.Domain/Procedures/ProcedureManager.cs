using System;
using System.Threading.Tasks;
using Aqt.CoreFW.Procedures; // Namespaces cần thiết
using Aqt.CoreFW.Domain.Procedures.Entities;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Aqt.CoreFW.Localization; // Add this for CoreFWResource
using Microsoft.Extensions.Localization; // Added for IStringLocalizer

// using Volo.Abp.Users; // Inject nếu cần

namespace Aqt.CoreFW.Domain.Procedures;

/// <summary>
/// Domain service responsible for managing Procedure entities.
/// </summary>
public class ProcedureManager : DomainService
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> _localizer;
    // private readonly ICurrentUser _currentUser;

    public ProcedureManager(
        IProcedureRepository procedureRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> localizer
        /*, ICurrentUser currentUser*/)
    {
        _procedureRepository = procedureRepository;
        _guidGenerator = guidGenerator;
        _localizer = localizer;
        // _currentUser = currentUser;
    }

    /// <summary>
    /// Creates a new Procedure entity after validating business rules.
    /// </summary>
    /// <param name="code">Unique code.</param>
    /// <param name="name">Name.</param>
    /// <param name="order">Display order.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">Status.</param>
    /// <returns>The newly created Procedure entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public async Task<Procedure> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        int order = 0,
        [CanBeNull] string? description = null,
        ProcedureStatus status = ProcedureStatus.Active)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), ProcedureConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(name, nameof(name), ProcedureConsts.MaxNameLength);
        Check.Length(description, nameof(description), ProcedureConsts.MaxDescriptionLength);

        // Validate Code uniqueness (globally)
        await ValidateCodeUniquenessAsync(code);

        var procedure = new Procedure(
            _guidGenerator.Create(),
            code,
            name,
            order,
            description,
            status
        );

        // Application Service will call Repository.InsertAsync
        return procedure;
    }

    /// <summary>
    /// Updates an existing Procedure entity after validating business rules.
    /// Note: Code cannot be changed.
    /// </summary>
    /// <param name="procedure">The entity to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="order">New display order.</param>
    /// <param name="description">New optional description.</param>
    /// <param name="status">New status.</param>
    /// <returns>The updated Procedure entity.</returns>
    /// <exception cref="UserFriendlyException">Thrown if business rules are violated.</exception>
    public Task<Procedure> UpdateAsync(
        [NotNull] Procedure procedure,
        [NotNull] string name,
        int order,
        [CanBeNull] string? description,
        ProcedureStatus status)
    {
        Check.NotNull(procedure, nameof(procedure));
        Check.NotNullOrWhiteSpace(name, nameof(name), ProcedureConsts.MaxNameLength);
        Check.Length(description, nameof(description), ProcedureConsts.MaxDescriptionLength);

        // Update properties using entity methods
        procedure.SetName(name);
        procedure.SetOrder(order);
        procedure.SetDescription(description);

        if (status == ProcedureStatus.Active) procedure.Activate();
        else procedure.Deactivate();

        // Application Service will call Repository.UpdateAsync
        return Task.FromResult(procedure); // No async validation needed here for standard updates
    }

    /// <summary>
    /// Updates synchronization information for a Procedure entity.
    /// </summary>
    /// <param name="procedure">The entity to update.</param>
    /// <param name="lastSyncedDate">New last synced date.</param>
    /// <param name="syncRecordId">New sync record ID.</param>
    /// <param name="syncRecordCode">New sync record code.</param>
    /// <returns>The updated Procedure entity.</returns>
    public Task<Procedure> UpdateSyncInfoAsync(
         [NotNull] Procedure procedure,
         DateTime? lastSyncedDate,
         Guid? syncRecordId,
         [CanBeNull] string? syncRecordCode)
    {
        Check.NotNull(procedure, nameof(procedure));
        Check.Length(syncRecordCode, nameof(syncRecordCode), ProcedureConsts.MaxSyncRecordCodeLength);

        procedure.UpdateSyncInfo(lastSyncedDate, syncRecordId, syncRecordCode);

        // Application Service will call Repository.UpdateAsync
         return Task.FromResult(procedure);
    }

    /// <summary>
    /// Helper method to validate code uniqueness (globally).
    /// </summary>
    private async Task ValidateCodeUniquenessAsync([NotNull] string code, Guid? excludeId = null)
    {
         if (await _procedureRepository.CodeExistsAsync(code, excludeId))
        {
            // Prefer UserFriendlyException with Localization key for better user experience
            throw new UserFriendlyException(_localizer["ProcedureCodeAlreadyExists", code]);
            // Alternative: BusinessException with Error Code
            // throw new BusinessException(CoreFWDomainErrorCodes.ProcedureCodeAlreadyExists).WithData("code", code);
        }
    }
} 