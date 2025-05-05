using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing; // Hoặc Entity<Guid> nếu không cần auditing
using Volo.Abp.Guids; // GuidGenerator

namespace Aqt.CoreFW.Domain.BDocuments.Entities;

/// <summary>
/// Represents the data associated with a specific ProcedureComponent within a BDocument.
/// Stores either form input data (InputData) or a reference to an uploaded file (FileId).
/// Đại diện cho dữ liệu liên kết với một ProcedureComponent cụ thể trong một BDocument.
/// Lưu trữ dữ liệu nhập từ form (InputData) hoặc tham chiếu đến tệp đã tải lên (FileId).
/// </summary>
public class BDocumentData : AuditedEntity<Guid> // Hoặc Entity<Guid> nếu không cần auditing cho data này
{
    /// <summary>
    /// Foreign key to the parent BDocument.
    /// Khóa ngoại trỏ tới BDocument cha.
    /// </summary>
    public virtual Guid BDocumentId { get; protected set; } // Giữ nguyên tên

    /// <summary>
    /// Foreign key to the related ProcedureComponent definition.
    /// Khóa ngoại trỏ tới định nghĩa ProcedureComponent liên quan.
    /// </summary>
    public virtual Guid ProcedureComponentId { get; protected set; } // Giữ nguyên tên

    /// <summary>
    /// Stores input data (e.g., JSON for Form type).
    /// Lưu trữ dữ liệu nhập (ví dụ: JSON cho loại Form).
    /// </summary>
    [CanBeNull]
    public virtual string? InputData { get; protected set; } // Đã đổi tên từ DuLieuNhap

    /// <summary>
    /// Stores the ID of the uploaded file (File type or generated from Form).
    /// Lưu trữ ID của tệp đã tải lên (loại File hoặc được tạo từ Form).
    /// </summary>
    [CanBeNull]
    public virtual Guid? FileId { get; protected set; } // Giữ nguyên tên

    protected BDocumentData() { }

    internal BDocumentData(
        Guid bDocumentId, // Tham số giữ nguyên
        Guid procedureComponentId, // Tham số giữ nguyên
        [CanBeNull] string? inputData, // Tham số đã đổi tên
        [CanBeNull] Guid? fileId) // Tham số giữ nguyên
        : base() // Chỉ cần gọi base(), ABP sẽ tạo Guid
    {
        BDocumentId = bDocumentId;
        ProcedureComponentId = procedureComponentId;
        InputData = inputData; // Gán thuộc tính đã đổi tên
        FileId = fileId;
    }

    /// <summary>
    /// Sets or updates the data for this entry.
    /// Thiết lập hoặc cập nhật dữ liệu cho mục này.
    /// </summary>
    internal void SetData([CanBeNull] string? inputData, [CanBeNull] Guid? fileId) // Tham số đã đổi tên
    {
        InputData = inputData; // Gán thuộc tính đã đổi tên
        FileId = fileId;
    }
} 