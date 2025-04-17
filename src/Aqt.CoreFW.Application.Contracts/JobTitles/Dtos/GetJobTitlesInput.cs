    using Volo.Abp.Application.Dtos;

    namespace Aqt.CoreFW.Application.Contracts.JobTitles.Dtos;

    /// <summary>
    /// Input DTO for retrieving a paginated and sorted list of Job Titles.
    /// Includes filtering options.
    /// </summary>
    public class GetJobTitlesInput : PagedAndSortedResultRequestDto // Kế thừa để có sẵn SkipCount, MaxResultCount, Sorting
    {
        /// <summary>
        /// Text to filter job titles by Code or Name.
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Filter job titles by their active status.
        /// Null means return all (both active and inactive).
        /// True means return only active.
        /// False means return only inactive.
        /// </summary>
        public bool? IsActive { get; set; }
    }