using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Aqt.CoreFW.Domain.BDocuments; // IBDocumentRepository
using Aqt.CoreFW.Domain.BDocuments.Entities; // Entities namespace
using Aqt.CoreFW.EntityFrameworkCore; // CoreFWDbContext
using JetBrains.Annotations; // For [NotNull] attribute
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Aqt.CoreFW.EntityFrameworkCore.Repositories.BDocuments;

public class BDocumentRepository :
    EfCoreRepository<CoreFWDbContext, BDocument, Guid>,
    IBDocumentRepository
{
    public BDocumentRepository(IDbContextProvider<CoreFWDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    // Overriding GetQueryableAsync to apply default filters or includes if necessary
    // public override async Task<IQueryable<BDocument>> GetQueryableAsync()
    // {
    //     return (await base.GetQueryableAsync()).Where(/* Default filters if needed */);
    // }

    // Helper to optionally include details
    protected virtual async Task<IQueryable<BDocument>> GetQueryableWithDetailsAsync(bool includeDetails = false)
    {
        var query = (await GetDbSetAsync()).AsQueryable();
        if (includeDetails)
        {
            query = query.Include(d => d.DocumentData);
            // Optionally include Procedure and Status if needed for specific operations,
            // but be mindful of performance impacts. Consider separate methods if frequently needed.
            query = query.Include(d => d.Procedure);
            // query = query.Include(d => d.WorkflowStatus);
        }
        return query;
    }

    public async Task<BDocument?> FindByCodeAsync(
        [NotNull] string code,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));

        var query = await GetQueryableWithDetailsAsync(includeDetails);
        return await query.AsNoTracking() // Use AsNoTracking for read-only operations
                          .FirstOrDefaultAsync(p => p.Code == code, GetCancellationToken(cancellationToken));
    }

    public async Task<bool> CodeExistsAsync(
        [NotNull] string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));

        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking().Where(p => p.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<BDocument>> GetListAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? workflowStatusId = null,
        DateTime? submissionDateFrom = null,
        DateTime? submissionDateTo = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryInternalAsync(filterText, procedureId, workflowStatusId, submissionDateFrom, submissionDateTo, includeDetails);

        // Apply sorting. Default sort if not specified.
        query = query.OrderBy(sorting.IsNullOrWhiteSpace() ?
            $"{nameof(BDocument.CreationTime)} desc" // Default sort by CreationTime descending
            : sorting);

        return await query.PageBy(skipCount, maxResultCount)
                          .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filterText = null,
        Guid? procedureId = null,
        Guid? workflowStatusId = null,
        DateTime? submissionDateFrom = null,
        DateTime? submissionDateTo = null,
        CancellationToken cancellationToken = default)
    {
        // Details are not needed for count
        var query = await GetListQueryInternalAsync(filterText, procedureId, workflowStatusId, submissionDateFrom, submissionDateTo, includeDetails: false);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<BDocument?> GetWithDataAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        // Explicitly include DocumentData
        var query = await GetQueryableWithDetailsAsync(includeDetails: true);
        // Use tracking here as the entity might be updated after retrieval
        return await query.FirstOrDefaultAsync(d => d.Id == id, GetCancellationToken(cancellationToken));
    }


    // --- Private Helper Method for Query Construction ---

    private async Task<IQueryable<BDocument>> GetListQueryInternalAsync(
         string? filterText = null,
         Guid? procedureId = null,
         Guid? workflowStatusId = null,
         DateTime? submissionDateFrom = null,
         DateTime? submissionDateTo = null,
         bool includeDetails = false)
    {
        var query = await GetQueryableWithDetailsAsync(includeDetails);

        // Apply filters using WhereIf for conditional filtering
        query = query.AsNoTracking() // Use AsNoTracking for list queries
             .WhereIf(!filterText.IsNullOrWhiteSpace(), p =>
                 (p.Code != null && p.Code.Contains(filterText!)) || // Search Code
                 (p.ApplicantName != null && p.ApplicantName.Contains(filterText!)) || // Search ApplicantName
                 (p.ApplicantIdentityNumber != null && p.ApplicantIdentityNumber.Contains(filterText!)) || // Search ApplicantIdentityNumber
                 (p.ApplicantEmail != null && p.ApplicantEmail.Contains(filterText!)) || // Search ApplicantEmail
                 (p.ApplicantPhoneNumber != null && p.ApplicantPhoneNumber.Contains(filterText!))) // Search ApplicantPhoneNumber
             .WhereIf(procedureId.HasValue, p => p.ProcedureId == procedureId!.Value) // Filter by ProcedureId
             .WhereIf(workflowStatusId.HasValue, p => p.WorkflowStatusId == workflowStatusId!.Value) // Filter by StatusId
             .WhereIf(submissionDateFrom.HasValue, p => p.SubmissionDate.HasValue && p.SubmissionDate.Value.Date >= submissionDateFrom.Value.Date) // Filter by SubmissionDate Start Date
             .WhereIf(submissionDateTo.HasValue, p => p.SubmissionDate.HasValue && p.SubmissionDate.Value.Date <= submissionDateTo.Value.Date); // Filter by SubmissionDate End Date

        return query;
    }

    // Optional: Implement GetListWithNavigationPropertiesAsync if complex includes are needed
    // public async Task<List<BDocument>> GetListWithNavigationPropertiesAsync(...) { ... }

    // Optional: Implement GetWithNavigationPropertiesAsync if complex includes are needed
    // public async Task<BDocument> GetWithNavigationPropertiesAsync(...) { ... }
}