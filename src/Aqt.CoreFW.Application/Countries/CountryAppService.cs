using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aqt.CoreFW.Application.Contracts.Countries;
using Aqt.CoreFW.Application.Contracts.Countries.Dtos;
using Aqt.CoreFW.Domain.Countries.Repositories;
using Aqt.CoreFW.Domain.Countries.Entities;
using Aqt.CoreFW.Domain.Shared; // For Error Codes
using Aqt.CoreFW.Localization; // For Localization
using Aqt.CoreFW.Permissions; // For Permissions
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core; // For LINQ extensions like WhereIf
using Volo.Abp.ObjectMapping; // Required for ObjectMapper

namespace Aqt.CoreFW.Application.Countries;

/// <summary>
/// Implements the application service for managing Countries.
/// </summary>
[Authorize(CoreFWPermissions.Countries.Default)] // Default authorization policy for the service
public class CountryAppService :
    CrudAppService<               // Inherits from the base CrudAppService
        Country,                  // The Entity
        CountryDto,               // The DTO to read Entity data
        Guid,                     // Primary key of the entity
        GetCountriesInput,        // Used for filtering and paging
        CreateUpdateCountryDto>,  // Used to create/update the entity
    ICountryAppService            // Implements the custom Country AppService interface
{
    private readonly ICountryRepository _countryRepository; // Custom repository for specific queries

    public CountryAppService(
        IRepository<Country, Guid> repository, // Standard repository from base CrudAppService
        ICountryRepository countryRepository)  // Injected custom repository
        : base(repository)
    {
        _countryRepository = countryRepository;
        LocalizationResource = typeof(CoreFWResource); // Set the localization resource
        // Set CRUD operation policy names based on defined permissions
        GetPolicyName = CoreFWPermissions.Countries.Default;
        GetListPolicyName = CoreFWPermissions.Countries.Default;
        CreatePolicyName = CoreFWPermissions.Countries.Create;
        UpdatePolicyName = CoreFWPermissions.Countries.Edit;
        DeletePolicyName = CoreFWPermissions.Countries.Delete;
    }

    /// <summary>
    /// Creates a new country after checking for duplicate codes.
    /// </summary>
    [Authorize(CoreFWPermissions.Countries.Create)] // Authorize based on the Create permission
    public override async Task<CountryDto> CreateAsync(CreateUpdateCountryDto input)
    {
        // Check if the code already exists
        if (await _countryRepository.CodeExistsAsync(input.Code))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
        }

        // Let the base CrudAppService handle the standard mapping and insertion
        return await base.CreateAsync(input);
    }

    /// <summary>
    /// Updates an existing country after checking for duplicate codes (excluding the current entity).
    /// </summary>
    [Authorize(CoreFWPermissions.Countries.Edit)] // Authorize based on the Edit permission
    public override async Task<CountryDto> UpdateAsync(Guid id, CreateUpdateCountryDto input)
    {
        var entity = await GetEntityByIdAsync(id); // Get the existing entity

        // Check if the code is being changed and if the new code already exists for another entity
        if (entity.Code != input.Code && await _countryRepository.CodeExistsAsync(input.Code, id))
        {
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryCodeAlreadyExists, input.Code]);
        }

        // Use the protected MapToEntity method from CrudAppService to map input DTO to the existing entity
        MapToEntity(input, entity);

        // Update the entity in the repository
        await Repository.UpdateAsync(entity, autoSave: true);

        // Map the updated entity back to the DTO using the protected MapToGetOutputDto method
        return MapToGetOutputDto(entity);
    }

    /// <summary>
    /// Deletes a country after checking for associated provinces/cities.
    /// </summary>
    [Authorize(CoreFWPermissions.Countries.Delete)] // Authorize based on the Delete permission
    public override async Task DeleteAsync(Guid id)
    {
        // TODO: Uncomment this block when Province entity and repository are available.
        /*
        // Check if the country has any linked provinces
        if (await _countryRepository.HasProvincesAsync(id))
        {
            // Get entity details for the error message
            var entity = await GetEntityByIdAsync(id); 
            throw new UserFriendlyException(L[CoreFWDomainErrorCodes.CountryHasProvincesCannotDelete, entity.Name ?? entity.Code]);
        }
        */

        // If no dependencies, proceed with the base delete (which performs soft delete)
        await base.DeleteAsync(id);
    }

    /// <summary>
    /// Gets a list of countries formatted for lookup controls (e.g., dropdowns).
    /// Allows anonymous access based on the attribute, adjust if authentication is needed.
    /// </summary>
    [AllowAnonymous] // Or apply a specific permission if lookup needs protection
    public async Task<ListResultDto<CountryLookupDto>> GetLookupAsync()
    {
        // Get the list of countries, sorted by name
        var countries = await _countryRepository.GetListAsync(sorting: nameof(Country.Name));

        // Map the entities to Lookup DTOs using the injected ObjectMapper
        var lookupDtos = ObjectMapper.Map<List<Country>, List<CountryLookupDto>>(countries);

        return new ListResultDto<CountryLookupDto>(lookupDtos);
    }

    /// <summary>
    /// Overrides the default GetListAsync to use the custom repository method for filtering.
    /// This provides more control over the query execution.
    /// </summary>
    public override async Task<PagedResultDto<CountryDto>> GetListAsync(GetCountriesInput input)
    {
        // Get the total count matching the filter (for pagination)
        var totalCount = await _countryRepository.GetCountAsync(input.Filter);

        // Get the paginated and sorted list matching the filter
        var countries = await _countryRepository.GetListAsync(
            filterText: input.Filter,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount
        );

        // Map the results to the DTO
        var countryDtos = ObjectMapper.Map<List<Country>, List<CountryDto>>(countries);

        return new PagedResultDto<CountryDto>(totalCount, countryDtos);
    }

    /*
    // This method is called by CrudAppService to apply default filtering.
    // Overriding it allows custom or more complex filtering logic.
    // However, if GetListAsync is fully overridden as above, this might not be necessary.
    protected override async Task<IQueryable<Country>> CreateFilteredQueryAsync(GetCountriesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();
        return queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                c => c.Code.Contains(input.Filter) || c.Name.Contains(input.Filter));
        // Note: Paging and Sorting are automatically applied by ABP CrudAppService after this step.
    }
    */
} 