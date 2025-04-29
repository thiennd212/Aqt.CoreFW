using Aqt.CoreFW.BDocuments; // Consts
using Aqt.CoreFW.Domain.Procedures.Entities; // Procedure
using Aqt.CoreFW.Domain.WorkflowStatuses.Entities; // WorkflowStatus
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Data; // For ExtraProperties
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.BDocuments.Entities;

/// <summary>
/// Represents an administrative document/case/profile submitted for a specific procedure.
/// </summary>
public class BDocument : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Foreign key to the Procedure this document belongs to.
    /// </summary>
    public virtual Guid ProcedureId { get; protected set; }

    /// <summary>
    /// Navigation property to the related Procedure. Load explicitly.
    /// </summary>
    [CanBeNull]
    public virtual Procedure? Procedure { get; protected set; }

    /// <summary>
    /// Unique code for the document.
    /// </summary>
    [NotNull]
    public virtual string MaHoSo { get; private set; }

    /// <summary>
    /// Name of the applicant.
    /// </summary>
    [NotNull]
    public virtual string TenChuHoSo { get; private set; }

    /// <summary>
    /// Applicant's identification number.
    /// </summary>
    [CanBeNull]
    public virtual string? SoDinhDanhChuHoSo { get; private set; }

    /// <summary>
    /// Applicant's address.
    /// </summary>
    [CanBeNull]
    public virtual string? DiaChiChuHoSo { get; private set; }

    /// <summary>
    /// Applicant's email address.
    /// </summary>
    [CanBeNull]
    public virtual string? EmailChuHoSo { get; private set; }

    /// <summary>
    /// Applicant's phone number.
    /// </summary>
    [CanBeNull]
    public virtual string? SoDienThoaiChuHoSo { get; private set; }

    // --- LOẠI BỎ CÁC TRƯỜNG TỜ KHAI ---

    /// <summary>
    /// Scope and content of activities.
    /// </summary>
    [CanBeNull]
    public virtual string? PhamViHoatDong { get; private set; } // MỚI

    /// <summary>
    /// Indicates if the applicant registered to receive results via postal service.
    /// </summary>
    public virtual bool DangKyNhanQuaBuuDien { get; private set; } // MỚI

    /// <summary>
    /// Foreign key to the current workflow status (nullable). Set by workflow process.
    /// </summary>
    public virtual Guid? TrangThaiHoSoId { get; private set; } // Nullable

    /// <summary>
    /// Navigation property to the related WorkflowStatus. Load explicitly.
    /// </summary>
    [CanBeNull]
    public virtual WorkflowStatus? TrangThaiHoSo { get; protected set; }

    /// <summary>
    /// Submission date.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? NgayNop { get; private set; }

    /// <summary>
    /// Reception date.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? NgayTiepNhan { get; private set; }

    /// <summary>
    /// Expected result date.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? NgayHenTra { get; private set; }

    /// <summary>
    /// Actual result date.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? NgayTraKetQua { get; private set; }

    /// <summary>
    /// Reason for rejection or additional info request.
    /// </summary>
    [CanBeNull]
    public virtual string? LyDoTuChoiHoacBoSung { get; private set; }

    /// <summary>
    /// Collection of component data (form JSON, file IDs).
    /// </summary>
    public virtual ICollection<BDocumentData> DocumentData { get; protected set; }

    /// <summary>
    /// Protected constructor for ORM.
    /// </summary>
    protected BDocument()
    {
        MaHoSo = string.Empty;
        TenChuHoSo = string.Empty;
        DocumentData = new Collection<BDocumentData>();
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// Creates a new BDocument instance. Called via BDocumentManager.
    /// </summary>
    internal BDocument(
        Guid id,
        Guid procedureId,
        [NotNull] string maHoSo,
        [NotNull] string tenChuHoSo,
        [CanBeNull] string? soDinhDanhChuHoSo = null,
        [CanBeNull] string? diaChiChuHoSo = null,
        [CanBeNull] string? emailChuHoSo = null,
        [CanBeNull] string? soDienThoaiChuHoSo = null,
        [CanBeNull] string? phamViHoatDong = null, // MỚI
        bool dangKyNhanQuaBuuDien = false, // MỚI
        [CanBeNull] DateTime? ngayNop = null)
        : base(id)
    {
        ProcedureId = procedureId;
        SetMaHoSoInternal(maHoSo);
        SetTenChuHoSoInternal(tenChuHoSo);
        SetSoDinhDanhChuHoSoInternal(soDinhDanhChuHoSo);
        SetDiaChiChuHoSoInternal(diaChiChuHoSo);
        SetEmailChuHoSoInternal(emailChuHoSo);
        SetSoDienThoaiChuHoSoInternal(soDienThoaiChuHoSo);
        SetPhamViHoatDongInternal(phamViHoatDong); // MỚI
        DangKyNhanQuaBuuDien = dangKyNhanQuaBuuDien; // MỚI

        TrangThaiHoSoId = null; // Initial status is null
        NgayNop = ngayNop ?? DateTime.Now;

        DocumentData = new Collection<BDocumentData>();
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    // --- Internal setters with validation ---

    private void SetMaHoSoInternal([NotNull] string maHoSo)
    {
        Check.NotNullOrWhiteSpace(maHoSo, nameof(maHoSo), BDocumentConsts.MaxMaHoSoLength);
        MaHoSo = maHoSo;
    }

    private void SetTenChuHoSoInternal([NotNull] string tenChuHoSo)
    {
        Check.NotNullOrWhiteSpace(tenChuHoSo, nameof(tenChuHoSo), BDocumentConsts.MaxTenChuHoSoLength);
        TenChuHoSo = tenChuHoSo;
    }

    private void SetSoDinhDanhChuHoSoInternal([CanBeNull] string? soDinhDanh)
    {
        Check.Length(soDinhDanh, nameof(soDinhDanh), BDocumentConsts.MaxSoDinhDanhChuHoSoLength);
        SoDinhDanhChuHoSo = soDinhDanh;
    }

    private void SetDiaChiChuHoSoInternal([CanBeNull] string? diaChi)
    {
        Check.Length(diaChi, nameof(diaChi), BDocumentConsts.MaxDiaChiChuHoSoLength);
        DiaChiChuHoSo = diaChi;
    }

     private void SetEmailChuHoSoInternal([CanBeNull] string? email)
    {
        Check.Length(email, nameof(email), BDocumentConsts.MaxEmailChuHoSoLength);
        // Optional: Basic email format validation
        EmailChuHoSo = email;
    }

     private void SetSoDienThoaiChuHoSoInternal([CanBeNull] string? phone)
    {
        Check.Length(phone, nameof(phone), BDocumentConsts.MaxSoDienThoaiChuHoSoLength);
        // Optional: Phone format validation
        SoDienThoaiChuHoSo = phone;
    }

     private void SetPhamViHoatDongInternal([CanBeNull] string? phamVi) // MỚI
    {
        // Check length if needed based on DB column type
        // Check.Length(phamVi, nameof(phamVi), BDocumentConsts.MaxPhamViHoatDongLength);
        PhamViHoatDong = phamVi;
    }


    private void SetLyDoTuChoiHoacBoSungInternal([CanBeNull] string? reason)
    {
         Check.Length(reason, nameof(reason), BDocumentConsts.MaxLyDoTuChoiHoacBoSungLength);
         LyDoTuChoiHoacBoSung = reason;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Updates main information. Called via BDocumentManager.
    /// </summary>
    internal BDocument UpdateInfo( // Đổi tên từ UpdateApplicantInfo
        [NotNull] string tenChuHoSo,
        [CanBeNull] string? soDinhDanhChuHoSo,
        [CanBeNull] string? diaChiChuHoSo,
        [CanBeNull] string? emailChuHoSo,
        [CanBeNull] string? soDienThoaiChuHoSo,
        [CanBeNull] string? phamViHoatDong, // MỚI
        bool dangKyNhanQuaBuuDien) // MỚI
    {
        SetTenChuHoSoInternal(tenChuHoSo);
        SetSoDinhDanhChuHoSoInternal(soDinhDanhChuHoSo);
        SetDiaChiChuHoSoInternal(diaChiChuHoSo);
        SetEmailChuHoSoInternal(emailChuHoSo);
        SetSoDienThoaiChuHoSoInternal(soDienThoaiChuHoSo);
        SetPhamViHoatDongInternal(phamViHoatDong); // MỚI
        DangKyNhanQuaBuuDien = dangKyNhanQuaBuuDien; // MỚI
        return this;
    }

    /// <summary>
    /// Changes the current workflow status. Validation happens in BDocumentManager.
    /// </summary>
    internal BDocument SetTrangThaiHoSoId(Guid? newTrangThaiHoSoId, // Nullable
        [CanBeNull] string? reason = null,
        [CanBeNull] DateTime? receptionDate = null,
        [CanBeNull] DateTime? resultDate = null)
    {
        TrangThaiHoSoId = newTrangThaiHoSoId;
        SetLyDoTuChoiHoacBoSungInternal(reason);

        if(receptionDate.HasValue) NgayTiepNhan = receptionDate;
        if(resultDate.HasValue) NgayTraKetQua = resultDate;

        // Consider adding Domain Events for status changes
        // AddDomainEvent(new BDocumentStatusChangedEvent(Id, newTrangThaiHoSoId));

        return this;
    }

    /// <summary>
    /// Updates appointment date.
    /// </summary>
    internal BDocument SetNgayHenTra([CanBeNull] DateTime? ngayHenTra)
    {
        NgayHenTra = ngayHenTra;
        return this;
    }


    // --- Methods to manage the DocumentData collection (Called by BDocumentManager) ---

    /// <summary>
    /// Adds new or updates existing data for a component.
    /// </summary>
    internal void AddOrUpdateData(Guid procedureComponentId, [CanBeNull] string? formData, [CanBeNull] Guid? fileId)
    {
        var existingData = DocumentData.FirstOrDefault(d => d.ProcedureComponentId == procedureComponentId);
        if (existingData != null)
        {
            existingData.SetData(formData, fileId);
        }
        else
        {
            var newData = new BDocumentData(this.Id, procedureComponentId, formData, fileId);
            DocumentData.Add(newData);
        }
    }

    /// <summary>
    /// Removes data entry for a component.
    /// </summary>
    internal void RemoveData(Guid procedureComponentId)
    {
        var dataToRemove = DocumentData.FirstOrDefault(d => d.ProcedureComponentId == procedureComponentId);
        if (dataToRemove != null)
        {
            DocumentData.Remove(dataToRemove);
        }
    }

    /// <summary>
    /// Clears all associated component data.
    /// </summary>
    internal void ClearData()
    {
         DocumentData.Clear();
    }
} 