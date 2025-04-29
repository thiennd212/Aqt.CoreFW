using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing; // Hoặc Entity<Guid>

namespace Aqt.CoreFW.Domain.BDocuments.Entities;

/// <summary>
/// Represents the data associated with a specific ProcedureComponent within a BDocument.
/// Stores either form input data (DuLieuNhap) or a reference to an uploaded file (FileId).
/// </summary>
public class BDocumentData : AuditedEntity<Guid> // Hoặc Entity<Guid> nếu không cần audit
{
    public virtual Guid BDocumentId { get; protected set; }
    public virtual Guid ProcedureComponentId { get; protected set; }

    /// <summary>
    /// Stores input data (e.g., JSON for Form type).
    /// </summary>
    [CanBeNull]
    public virtual string? DuLieuNhap { get; protected set; }

    /// <summary>
    /// Stores the ID of the uploaded file (File type or generated from Form).
    /// </summary>
    [CanBeNull]
    public virtual Guid? FileId { get; protected set; }

    protected BDocumentData() { }

    internal BDocumentData(
        Guid bDocumentId,
        Guid procedureComponentId,
        [CanBeNull] string? formData,
        [CanBeNull] Guid? fileId)
        // Gọi constructor base() để ABP tự tạo Guid nếu Id chưa có
        : base()
    {
        BDocumentId = bDocumentId;
        ProcedureComponentId = procedureComponentId;
        // Validation should happen before calling this.
        DuLieuNhap = formData;
        FileId = fileId;
    }

    internal void SetData([CanBeNull] string? formData, [CanBeNull] Guid? fileId)
    {
        // Validation should happen before calling this.
        DuLieuNhap = formData;
        FileId = fileId;
    }
} 