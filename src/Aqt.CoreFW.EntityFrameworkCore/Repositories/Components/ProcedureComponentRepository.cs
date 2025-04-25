    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Threading;
    using System.Threading.Tasks;
    using Aqt.CoreFW.Components; // Enum namespace
    using Aqt.CoreFW.Domain.Components; // IProcedureComponentRepository
    using Aqt.CoreFW.Domain.Components.Entities; // Entities namespace
    using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext, EfCoreRepository
    using JetBrains.Annotations;
    using Microsoft.EntityFrameworkCore;
    using Volo.Abp; // For Check
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
    using Volo.Abp.EntityFrameworkCore;

    namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.Components;

    public class ProcedureComponentRepository :
        EfCoreRepository<CoreFWDbContext, ProcedureComponent, Guid>,
        IProcedureComponentRepository
    {
        public ProcedureComponentRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Helper to optionally include details (ProcedureLinks)
        // Note: Include paths are strings. Be careful with typos.
        protected virtual async Task<IQueryable<ProcedureComponent>> GetQueryableWithDetailsAsync(bool includeDetails = false)
        {
            var query = (await GetDbSetAsync()).AsQueryable();
            if (includeDetails)
            {
                // Include the navigation property specified in the entity
                query = query.Include(c => c.ProcedureLinks);
            }
            return query;
        }

        // Override GetQueryableAsync if you want default inclusion behavior for this repo
        // public override async Task<IQueryable<ProcedureComponent>> WithDetailsAsync()
        // {
        //     return await GetQueryableWithDetailsAsync(true);
        // }


        public async Task<ProcedureComponent?> FindByCodeAsync(
            [NotNull] string code,
            bool includeDetails = true, // Default to true for finding single item
            CancellationToken cancellationToken = default)
        {
            // Sử dụng GetQueryableWithDetailsAsync thay vì GetQueryableAsync để có thể include
            var query = await GetQueryableWithDetailsAsync(includeDetails);
            return await query.AsNoTracking() // Use NoTracking for read operation
                              .FirstOrDefaultAsync(p => p.Code == code, GetCancellationToken(cancellationToken));
        }

        public async Task<bool> CodeExistsAsync(
            [NotNull] string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.AsNoTracking().Where(p => p.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<ProcedureComponent>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            ComponentStatus? status = null,
            ComponentType? type = null,
            Guid? procedureId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            bool includeDetails = false, // Default to false for list
            CancellationToken cancellationToken = default)
        {
            var query = await GetListQueryInternalAsync(filterText, code, name, status, type, procedureId, includeDetails);

            query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
                $"{nameof(ProcedureComponent.Order)} asc, {nameof(ProcedureComponent.Name)} asc" // Default sort
                : sorting);

            return await query.PageBy(skipCount, maxResultCount)
                              .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            ComponentStatus? status = null,
            ComponentType? type = null,
            Guid? procedureId = null,
            CancellationToken cancellationToken = default)
        {
            // Don't include details for count
            var query = await GetListQueryInternalAsync(filterText, code, name, status, type, procedureId, includeDetails: false);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<Guid>> GetLinkedProcedureIdsAsync(
            Guid componentId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return await dbContext.ProcedureComponentLinks
                                  .AsNoTracking()
                                  .Where(l => l.ProcedureComponentId == componentId)
                                  .Select(l => l.ProcedureId)
                                  .Distinct() // Ensure distinct IDs
                                  .ToListAsync(GetCancellationToken(cancellationToken));
        }

         public async Task<List<ProcedureComponentLink>> GetComponentLinksAsync(
             Guid componentId,
             CancellationToken cancellationToken = default)
         {
            var dbContext = await GetDbContextAsync();
             // Return trackable entities as they might be used for deletion by the manager
             return await dbContext.ProcedureComponentLinks
                                   .Where(l => l.ProcedureComponentId == componentId)
                                   .ToListAsync(GetCancellationToken(cancellationToken));
         }

        // Implement efficient bulk delete for links
        public async Task DeleteManyComponentLinksAsync(
            [NotNull] List<ProcedureComponentLink> links,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
             Check.NotNull(links, nameof(links));
             if (!links.Any()) return; // Nothing to delete

             var dbContext = await GetDbContextAsync();
             // Ensure entities are tracked before removing
             // If they came from GetComponentLinksAsync, they should be tracked.
             // If created manually, they might need attaching first.
             dbContext.ProcedureComponentLinks.RemoveRange(links); // Use EF Core's RemoveRange

             if (autoSave)
             {
                 await dbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
             }
        }

        // Implement efficient bulk insert for links
        public async Task InsertManyComponentLinksAsync(
            [NotNull] List<ProcedureComponentLink> links,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
             Check.NotNull(links, nameof(links));
             if (!links.Any()) return; // Nothing to insert

            var dbContext = await GetDbContextAsync();
             // Use EF Core's AddRangeAsync for efficiency
             await dbContext.ProcedureComponentLinks.AddRangeAsync(links, GetCancellationToken(cancellationToken));

             if (autoSave)
             {
                 await dbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
             }
        }


        public async Task<List<ProcedureComponent>> GetLookupAsync(
            ComponentType? type = null,
            bool onlyActive = true,
            string? sorting = "Order ASC, Name ASC",
            CancellationToken cancellationToken = default)
        {
            var query = (await GetDbSetAsync()).AsNoTracking();

            if (onlyActive)
            {
                query = query.Where(p => p.Status == ComponentStatus.Active);
            }
            if(type.HasValue)
            {
                 query = query.Where(p => p.Type == type.Value);
            }

            query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
                $"{nameof(ProcedureComponent.Order)} asc, {nameof(ProcedureComponent.Name)} asc" // Default sort
                : sorting);

            // Select only necessary fields for lookup? Optional optimization.
            // return await query.Select(p => new ProcedureComponent { Id = p.Id, Name = p.Name, Code = p.Code, Type = p.Type }).ToListAsync(...)
            return await query.ToListAsync(GetCancellationToken(cancellationToken));
        }

        // --- Private Helper Method ---

        private async Task<IQueryable<ProcedureComponent>> GetListQueryInternalAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            ComponentStatus? status = null,
            ComponentType? type = null,
            Guid? procedureId = null,
            bool includeDetails = false)
        {
            var dbContext = await GetDbContextAsync();
            // Sử dụng GetQueryableWithDetailsAsync để có thể include nếu cần
            var query = await GetQueryableWithDetailsAsync(includeDetails);

             query = query.AsNoTracking() // Use AsNoTracking for read-only lists
                // Combined filter for Code or Name
                .WhereIf(!filterText.IsNullOrWhiteSpace(), p =>
                    (p.Code != null && p.Code.Contains(filterText!)) ||
                    (p.Name != null && p.Name.Contains(filterText!)))
                // Specific filters
                .WhereIf(!code.IsNullOrWhiteSpace(), p => p.Code == code)
                .WhereIf(!name.IsNullOrWhiteSpace(), p => p.Name != null && p.Name.Contains(name))
                .WhereIf(status.HasValue, p => p.Status == status!.Value)
                .WhereIf(type.HasValue, p => p.Type == type!.Value);

            // Filter by linked ProcedureId
            if (procedureId.HasValue)
            {
                // Join with the Link table to filter without needing Include for performance
                query = query.Where(c => dbContext.ProcedureComponentLinks
                                            .Any(l => l.ProcedureComponentId == c.Id && l.ProcedureId == procedureId.Value));

                // Alternative using Navigation Property (requires Include in GetQueryableWithDetailsAsync)
                // if (includeDetails) // Only works if details are included
                // {
                //     query = query.Where(c => c.ProcedureLinks.Any(l => l.ProcedureId == procedureId.Value));
                // }
                // else // Need to join if details not included
                // {
                //      query = query.Where(c => dbContext.ProcedureComponentLinks
                //                              .Any(l => l.ProcedureComponentId == c.Id && l.ProcedureId == procedureId.Value));
                // }
            }

            return query;
        }

        // Override GetAsync to ensure links are loaded if necessary
        public override async Task<ProcedureComponent> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
             var query = await GetQueryableWithDetailsAsync(includeDetails);
             var entity = await query.FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(ProcedureComponent), id);
            }

             return entity;
        }

         // Optional: Override FindAsync similarly if needed
         public override async Task<ProcedureComponent?> FindAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
         {
            var query = await GetQueryableWithDetailsAsync(includeDetails);
            return await query.FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));
         }
    }