using System;
using Aqt.CoreFW.Domain.Shared.OrgStructure; // Sẽ tạo ở Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.OrgStructure.Entities;

public class JobTitle : FullAuditedAggregateRoot<Guid>
{
    public virtual string Code { get; private set; }
    public virtual string Name { get; private set; }
    public virtual string? Description { get; private set; }

    protected JobTitle() { /* For ORM */ }

    public JobTitle(Guid id, [NotNull] string code, [NotNull] string name, [CanBeNull] string? description = null) : base(id)
    {
        SetCode(code);
        SetName(name);
        Description = description; // Description không có validation đặc biệt ngoài nullability
    }

    public JobTitle SetCode([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), OrgStructureConsts.MaxJobTitleCodeLength);
        Code = code;
        return this;
    }

    public JobTitle SetName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), OrgStructureConsts.MaxJobTitleNameLength);
        Name = name;
        return this;
    }

    public JobTitle SetDescription([CanBeNull] string? description)
    {
        // Có thể thêm validation độ dài nếu cần
        Description = description;
        return this;
    }
}