using System;
using Aqt.CoreFW.Domain.Shared.JobTitles; // Dùng JobTitleConsts từ Domain.Shared
using Aqt.CoreFW.Ranks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.JobTitles.Entities;

/// <summary>
/// Represents a Job Title in the system.
/// </summary>
public class JobTitle : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique code for the Job Title.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the Job Title.
    /// </summary>
    [NotNull]
    public virtual string Name { get; private set; }

    /// <summary>
    /// Optional description for the Job Title.
    /// </summary>
    [CanBeNull]
    public virtual string? Description { get; private set; }

    /// <summary>
    /// Indicates if the Job Title is currently active and can be used.
    /// </summary>
    public virtual bool IsActive { get; private set; }

    /// <summary>
    //Timestamp of the last synchronization.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? LastSyncDate { get; private set; } // Đổi tên từ LastSyncedTime

    /// <summary>
    //ID of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual Guid? SyncRecordId { get; private set; } // Đổi tên từ SyncId và kiểu dữ liệu

    /// <summary>
    //Code of the record in the external system during synchronization.
    /// </summary>
    [CanBeNull]
    public virtual string? SyncRecordCode { get; private set; } // Đổi tên từ SyncCode

    /// <summary>
    /// Protected constructor for ORM.
    /// </summary>
    protected JobTitle()
    {
        /* For ORM */
        Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ cho non-nullable string
        Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ cho non-nullable string
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JobTitle"/> class.
    /// Ensures the entity is in a valid state upon creation.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The job title code.</param>
    /// <param name="name">The job title name.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="isActive">Initial active status (defaults to true).</param>
    public JobTitle(
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        [CanBeNull] string? description = null,
        bool isActive = true) : base(id)
    {
        // Sử dụng các phương thức Set để đảm bảo validation được áp dụng ngay khi tạo
        SetCode(code);
        SetName(name);
        SetDescription(description); // Cho phép null
        IsActive = isActive; // Trạng thái có thể gán trực tiếp hoặc qua phương thức nếu cần logic phức tạp hơn
    }

    /// <summary>
    /// Sets or updates the job title code.
    /// </summary>
    /// <param name="code">The new code.</param>
    /// <returns>The current JobTitle instance.</returns>
    /// <exception cref="ArgumentException">Thrown if code is null, empty, whitespace, or exceeds max length.</exception>
    public JobTitle SetCode([NotNull] string code)
    {
        // Kiểm tra null, rỗng hoặc khoảng trắng
        Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: JobTitleConsts.MaxCodeLength);
        // Check.Length đã được tích hợp trong NotNullOrWhiteSpace với maxLength
        Code = code;
        return this;
    }

    /// <summary>
    /// Sets or updates the job title name.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <returns>The current JobTitle instance.</returns>
    /// <exception cref="ArgumentException">Thrown if name is null, empty, whitespace, or exceeds max length.</exception>
    public JobTitle SetName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: JobTitleConsts.MaxNameLength);
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets or updates the job title description.
    /// </summary>
    /// <param name="description">The new description (can be null or empty).</param>
    /// <returns>The current JobTitle instance.</returns>
    /// <exception cref="ArgumentException">Thrown if description exceeds max length.</exception>
    public JobTitle SetDescription([CanBeNull] string? description)
    {
        // Cho phép null hoặc rỗng, chỉ kiểm tra độ dài nếu có giá trị
        if (!string.IsNullOrEmpty(description))
        {
            Check.Length(description, nameof(description), JobTitleConsts.MaxDescriptionLength);
        }
        Description = description;
        return this;
    }

    /// <summary>
    /// Activates the job title.
    /// </summary>
    /// <returns>The current JobTitle instance.</returns>
    public JobTitle Activate()
    {
        IsActive = true;
        return this;
    }

    /// <summary>
    /// Deactivates the job title.
    /// </summary>
    /// <returns>The current JobTitle instance.</returns>
    public JobTitle Deactivate()
    {
        IsActive = false;
        return this;
    }

    /// <summary>
    /// Updates the synchronization information.
    /// </summary>
    public JobTitle SetSyncInfo([CanBeNull] DateTime? lastSyncDate, [CanBeNull] Guid? syncRecordId, [CanBeNull] string? syncRecordCode) // Public
    {
        Check.Length(syncRecordCode, nameof(syncRecordCode), RankConsts.MaxSyncRecordCodeLength);

        LastSyncDate = lastSyncDate;
        SyncRecordId = syncRecordId;
        SyncRecordCode = syncRecordCode;

        return this;
    }
}