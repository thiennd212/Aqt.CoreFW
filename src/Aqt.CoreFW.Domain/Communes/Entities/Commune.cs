using System;
using Aqt.CoreFW.Domain.Shared.Communes;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Communes.Entities;

public class Commune : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string Code { get; private set; }

    [NotNull]
    public virtual string Name { get; private set; }

    public virtual CommuneStatus Status { get; private set; }

    public virtual int Order { get; private set; }

    [CanBeNull]
    public virtual string? Description { get; private set; }

    public virtual Guid ProvinceId { get; private set; }

    public virtual Guid? DistrictId { get; private set; }

    [CanBeNull]
    public virtual DateTime? LastSyncedTime { get; private set; }

    [CanBeNull]
    public virtual string? SyncId { get; private set; }

    [CanBeNull]
    public virtual string? SyncCode { get; private set; }

    protected Commune()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    internal Commune(
        Guid id,
        [NotNull] string code,
        [NotNull] string name,
        Guid provinceId,
        Guid? districtId,
        int order = 0,
        [CanBeNull] string? description = null,
        CommuneStatus status = CommuneStatus.Active,
        [CanBeNull] DateTime? lastSyncedTime = null,
        [CanBeNull] string? syncId = null,
        [CanBeNull] string? syncCode = null)
        : base(id)
    {
        SetCodeInternal(code);
        ProvinceId = provinceId;
        DistrictId = districtId;

        SetNameInternal(name);
        SetOrderInternal(order);
        SetDescriptionInternal(description);
        Status = status;
        SetSyncInfoInternal(lastSyncedTime, syncId, syncCode);
    }

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), CommuneConsts.MaxCodeLength);
        Code = code;
    }

    private void SetNameInternal([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), CommuneConsts.MaxNameLength);
        Name = name;
    }

    private void SetOrderInternal(int order)
    {
        Order = order;
    }

    private void SetDescriptionInternal([CanBeNull] string? description)
    {
        Check.Length(description, nameof(description), CommuneConsts.MaxDescriptionLength);
        Description = description;
    }

    private void SetSyncInfoInternal([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode)
    {
        Check.Length(syncId, nameof(syncId), CommuneConsts.MaxSyncIdLength);
        Check.Length(syncCode, nameof(syncCode), CommuneConsts.MaxSyncCodeLength);

        LastSyncedTime = lastSyncedTime;
        SyncId = syncId;
        SyncCode = syncCode;
    }

    internal void SetDistrictInternal(Guid? districtId)
    {
         DistrictId = districtId;
    }

    internal Commune SetName([NotNull] string name)
    {
        SetNameInternal(name);
        return this;
    }

    public Commune SetOrder(int order)
    {
        SetOrderInternal(order);
        return this;
    }

    public Commune SetDescription([CanBeNull] string? description)
    {
        SetDescriptionInternal(description);
        return this;
    }

    public Commune Activate()
    {
        Status = CommuneStatus.Active;
        return this;
    }

    public Commune Deactivate()
    {
        Status = CommuneStatus.Inactive;
        return this;
    }

    public Commune SetSyncInfo([CanBeNull] DateTime? lastSyncedTime, [CanBeNull] string? syncId, [CanBeNull] string? syncCode)
    {
         SetSyncInfoInternal(lastSyncedTime, syncId, syncCode);
         return this;
    }
} 