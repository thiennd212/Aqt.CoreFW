using System;
using System.Collections.Generic;
using System.Linq; // Required for LINQ operations
using System.Threading.Tasks;
using Aqt.CoreFW.Components; // Namespaces cần thiết
using Aqt.CoreFW.Domain.Components.Entities;
using Aqt.CoreFW.Domain.Procedures; // For IProcedureRepository
using Aqt.CoreFW.Localization; // For IStringLocalizer<CoreFWResource>
using JetBrains.Annotations;
using Microsoft.Extensions.Localization; // For IStringLocalizer
using Volo.Abp;
using Volo.Abp.Domain.Repositories; // For IRepository<ProcedureComponentLink>
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Domain.Entities; // For EntityNotFoundException
// using Volo.Abp.Users; // Inject nếu cần CurrentUser

namespace Aqt.CoreFW.Domain.Components;

/// <summary>
/// Domain service responsible for managing ProcedureComponent entities and their relationship with Procedures.
/// </summary>
public class ProcedureComponentManager : DomainService
{
    private readonly IProcedureComponentRepository _componentRepository;
    // Inject the generic repository for the joining entity if specific methods aren't on IProcedureComponentRepository
    private readonly IRepository<ProcedureComponentLink> _procedureComponentLinkRepository;
    private readonly IProcedureRepository _procedureRepository; // To validate Procedure IDs exist
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStringLocalizer<CoreFWResource> L; // Inject Localizer
    // private readonly ICurrentUser _currentUser;

    public ProcedureComponentManager(
        IProcedureComponentRepository componentRepository,
        IRepository<ProcedureComponentLink> procedureComponentLinkRepository, // Inject repo với tên mới
        IProcedureRepository procedureRepository,
        IGuidGenerator guidGenerator,
        IStringLocalizer<CoreFWResource> stringLocalizer /* Add localizer */
        /*,ICurrentUser currentUser*/)
    {
        _componentRepository = componentRepository;
        _procedureComponentLinkRepository = procedureComponentLinkRepository; // Gán repo với tên mới
        _procedureRepository = procedureRepository;
        _guidGenerator = guidGenerator;
        L = stringLocalizer; // Assign localizer
        // _currentUser = currentUser;
    }

    /// <summary>
    /// Creates a new ProcedureComponent entity after validating business rules.
    /// The Application Service is responsible for inserting the returned entity.
    /// </summary>
    /// <param name="code">Unique code.</param>
    /// <param name="name">Name.</param>
    /// <param name="order">Display order.</param>
    /// <param name="type">Component type.</param>
    /// <param name="formDefinition">Form definition (required and validated if type is Form).</param>
    /// <param name="tempPath">Template path (required and validated if type is File).</param>
    /// <param name="description">Optional description.</param>
    /// <param name="status">Initial status.</param>
    /// <returns>The newly created, validated ProcedureComponent entity (not yet persisted).</returns>
    /// <exception cref="UserFriendlyException">Thrown if Code already exists.</exception>
    /// <exception cref="ArgumentException">Thrown if type/content mismatch or other invalid arguments.</exception>
    public async Task<ProcedureComponent> CreateAsync(
        [NotNull] string code,
        [NotNull] string name,
        int order,
        ComponentType type,
        [CanBeNull] string? formDefinition = null,
        [CanBeNull] string? tempPath = null,
        [CanBeNull] string? description = null,
        ComponentStatus status = ComponentStatus.Active)
    {
        // Basic argument checks using Volo.Abp.Check
        Check.NotNullOrWhiteSpace(code, nameof(code), ComponentConsts.MaxCodeLength);
        Check.NotNullOrWhiteSpace(name, nameof(name), ComponentConsts.MaxNameLength);
        Check.Length(description, nameof(description), ComponentConsts.MaxDescriptionLength);
        // Order validation if needed (e.g., >= 0)
        // Check.Range(order, nameof(order), 0);

        // Validate Code uniqueness (business rule)
        await ValidateCodeUniquenessAsync(code);

        // Validate Type and content consistency (business rule)
        //ValidateTypeAndContentConsistency(type, formDefinition, tempPath);
        // Optional: Validate FormDefinition JSON or TempPath format here if desired

        // Use internal constructor of the entity - this ensures the entity is created in a valid state
        var component = new ProcedureComponent(
            _guidGenerator.Create(),
            code,
            name,
            order,
            type,
            formDefinition, // Pass validated values
            tempPath,       // Pass validated values
            description,
            status
        );

        // Return the new entity; Application Service handles InsertAsync
        return component;
    }

    /// <summary>
    /// Updates an existing ProcedureComponent entity after validating business rules.
    /// The Application Service is responsible for updating the entity in the repository.
    /// Note: The component's Code cannot be changed.
    /// </summary>
    /// <param name="component">The existing ProcedureComponent entity to update.</param>
    /// <param name="name">New name.</param>
    /// <param name="order">New display order.</param>
    /// <param name="type">New component type.</param>
    /// <param name="formDefinition">New form definition (validated based on type).</param>
    /// <param name="tempPath">New template path (validated based on type).</param>
    /// <param name="description">New optional description.</param>
    /// <param name="status">New status.</param>
    /// <returns>The updated ProcedureComponent entity (not yet persisted).</returns>
    /// <exception cref="ArgumentNullException">Thrown if component is null.</exception>
    /// <exception cref="ArgumentException">Thrown if type/content mismatch or other invalid arguments.</exception>
    public Task<ProcedureComponent> UpdateAsync(
        [NotNull] ProcedureComponent component,
        [NotNull] string name,
        int order,
        ComponentType type,
        [CanBeNull] string? formDefinition,
        [CanBeNull] string? tempPath,
        [CanBeNull] string? description,
        ComponentStatus status)
    {
        Check.NotNull(component, nameof(component)); // Ensure entity exists

        // Basic argument checks
        Check.NotNullOrWhiteSpace(name, nameof(name), ComponentConsts.MaxNameLength);
        Check.Length(description, nameof(description), ComponentConsts.MaxDescriptionLength);
        // Check.Range(order, nameof(order), 0); // If needed

        // Validate Type and content consistency before attempting to update
        //ValidateTypeAndContentConsistency(type, formDefinition, tempPath);
        // Optional: Validate FormDefinition JSON or TempPath format here

        // Update properties using the entity's public methods to ensure encapsulation and validation
        component.SetName(name);
        component.SetOrder(order);
        component.SetDescription(description);
        component.SetTypeAndContent(type, formDefinition, tempPath); // Entity handles internal consistency

        // Update status
        if (status == ComponentStatus.Active)
        {
            component.Activate();
        }
        else
        {
            component.Deactivate();
        }

        // Return the modified entity; Application Service handles UpdateAsync
        // No async operations within this standard update logic, so Task.FromResult is fine.
        return Task.FromResult(component);
    }

    /// <summary>
    /// Updates the links between a specific ProcedureComponent and Procedures.
    /// It synchronizes the links in the database with the provided list of Procedure IDs.
    /// </summary>
    /// <param name="componentId">The ID of the ProcedureComponent whose links are to be updated.</param>
    /// <param name="procedureIds">The complete list of Procedure IDs that should be linked to the component.</param>
    /// <exception cref="EntityNotFoundException">Thrown if the ProcedureComponent with componentId does not exist.</exception>
    /// <exception cref="UserFriendlyException">Thrown if any of the provided procedureIds do not correspond to existing Procedures.</exception>
    public async Task UpdateProcedureLinksAsync(Guid componentId, [NotNull] List<Guid> procedureIds)
    {
         // Ensure the input list is not null
         Check.NotNull(procedureIds, nameof(procedureIds));
         // Work with distinct IDs to avoid potential issues
         var distinctProcedureIds = procedureIds.Distinct().ToList();

        // 1. Verify the ProcedureComponent exists (essential)
        var componentExists = await _componentRepository.AnyAsync(c => c.Id == componentId);
        if (!componentExists)
        {
            throw new EntityNotFoundException(typeof(ProcedureComponent), componentId);
        }

        // 2. Validate all provided Procedure IDs correspond to existing Procedures (iterative check).
        if (distinctProcedureIds.Any())
        {
            foreach (var procId in distinctProcedureIds)
            {
                if (!await _procedureRepository.AnyAsync(p => p.Id == procId))
                {
                    // Provide a user-friendly error message
                    throw new UserFriendlyException(L["OneOrMoreProceduresNotFound"]); // Use injected L
                    // Alternative: Throw a BusinessException with more details if needed.
                }
            }
        }

        // 3. Get the current links from the database for this component.
        // Use the specific method from IProcedureComponentRepository for clarity and potential optimization.
        var currentLinks = await _componentRepository.GetComponentLinksAsync(componentId);
        var currentProcedureIds = currentLinks.Select(l => l.ProcedureId).ToList();

        // 4. Determine which links to add and which to remove.
        var procedureIdsToAdd = distinctProcedureIds.Except(currentProcedureIds).ToList();
        var linksToRemove = currentLinks.Where(l => !distinctProcedureIds.Contains(l.ProcedureId)).ToList();

        // 5. Remove the links that are no longer needed.
        if (linksToRemove.Any())
        {
             // Prefer specific repository method for potential batch operations.
             // Fallback to generic repository if the specific method is not implemented.
             try
             {
                await _componentRepository.DeleteManyComponentLinksAsync(linksToRemove, true);
             }
             catch (NotImplementedException) // Example fallback
             {
                await _procedureComponentLinkRepository.DeleteManyAsync(linksToRemove, true);
             }
        }

        // 6. Add the new links.
        if (procedureIdsToAdd.Any())
        {
             var newLinks = procedureIdsToAdd
                 .Select(procId => new ProcedureComponentLink(procId, componentId)) // Create new link entities
                 .ToList();

             // Prefer specific repository method for potential batch operations.
             // Fallback to generic repository.
              try
             {
                await _componentRepository.InsertManyComponentLinksAsync(newLinks, true);
             }
             catch (NotImplementedException) // Example fallback
             {
                await _procedureComponentLinkRepository.InsertManyAsync(newLinks, true);
             }
        }
        // Changes are saved via autoSave=true or by the UnitOfWork in the AppService.
    }


    /// <summary>
    /// Private helper method to validate the uniqueness of the ProcedureComponent code.
    /// Throws a UserFriendlyException if the code already exists (excluding the given ID).
    /// </summary>
    private async Task ValidateCodeUniquenessAsync([NotNull] string code, Guid? excludeId = null)
    {
         // Use the dedicated repository method
         if (await _componentRepository.CodeExistsAsync(code, excludeId))
        {
            // Use a localized error message defined in Domain.Shared
            throw new UserFriendlyException(L["ComponentCodeAlreadyExists", code]); // Use injected L
            // Alternative using BusinessException with Error Code:
            // throw new BusinessException(CoreFWDomainErrorCodes.ComponentCodeAlreadyExists).WithData("code", code);
        }
    }

    /// <summary>
    /// Private helper method to validate the consistency between Type, FormDefinition, and TempPath.
    /// Throws ArgumentException if rules are violated.
    /// </summary>
    private void ValidateTypeAndContentConsistency(ComponentType type, [CanBeNull] string? formDefinition, [CanBeNull] string? tempPath)
    {
        // This logic is crucial and mirrors the entity's internal checks.
        // It prevents invalid combinations before attempting to set them on the entity.
         if (type == ComponentType.Form)
        {
            // If Type is Form, FormDefinition must have a value.
            if (string.IsNullOrWhiteSpace(formDefinition))
            {
                throw new ArgumentException("FormDefinition cannot be empty when component Type is Form.", nameof(formDefinition));
                // Consider BusinessException with specific error code if needed
            }
            // If Type is Form, TempPath must be null or empty.
             if (!string.IsNullOrWhiteSpace(tempPath))
            {
                 throw new ArgumentException("TempPath must be empty when component Type is Form.", nameof(tempPath));
             }
             // Optional: Add JSON format validation for FormDefinition
             // try
             // {
             //    if(formDefinition != null) System.Text.Json.JsonDocument.Parse(formDefinition);
             // }
             // catch (System.Text.Json.JsonException ex)
             // {
             //    throw new ArgumentException($"FormDefinition is not valid JSON: {ex.Message}", nameof(formDefinition));
             // }
        }
        else // Type == ComponentType.File
        {
            // If Type is File, TempPath must have a value.
            if (string.IsNullOrWhiteSpace(tempPath))
            {
               throw new ArgumentException("TempPath cannot be empty when component Type is File.", nameof(tempPath));
            }
            // If Type is File, FormDefinition must be null or empty.
             if (!string.IsNullOrWhiteSpace(formDefinition))
            {
                throw new ArgumentException("FormDefinition must be empty when component Type is File.", nameof(formDefinition));
             }
             // Optional: Add path format validation for TempPath
             // Example: Check for invalid characters, URI format, etc.
        }
    }
} 