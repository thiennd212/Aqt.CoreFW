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
using Volo.Abp.Timing; // Clock

namespace Aqt.CoreFW.Domain.BDocuments.Entities;

/// <summary>
/// Represents an administrative document/case/profile submitted for a specific procedure.
/// Đại diện cho một hồ sơ hành chính được nộp cho một thủ tục cụ thể.
/// </summary>
public class BDocument : FullAuditedAggregateRoot<Guid>//, IHasExtraProperties
{
    /// <summary>
    /// Foreign key to the Procedure this document belongs to.
    /// Khóa ngoại trỏ tới Thủ tục mà hồ sơ này thuộc về.
    /// </summary>
    public virtual Guid ProcedureId { get; protected set; }

    /// <summary>
    /// Navigation property to the related Procedure. Load explicitly.
    /// Thuộc tính điều hướng đến Thủ tục liên quan. Cần được load tường minh.
    /// </summary>
    [CanBeNull]
    public virtual Procedure? Procedure { get; protected set; }

    /// <summary>
    /// Unique code for the document.
    /// Mã duy nhất của hồ sơ.
    /// </summary>
    [NotNull]
    public virtual string Code { get; private set; }

    /// <summary>
    /// Name of the applicant.
    /// Tên của người nộp hồ sơ.
    /// </summary>
    [NotNull]
    public virtual string ApplicantName { get; private set; }

    /// <summary>
    /// Applicant's identification number.
    /// Số định danh của người nộp hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual string? ApplicantIdentityNumber { get; private set; }

    /// <summary>
    /// Applicant's address.
    /// Địa chỉ của người nộp hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual string? ApplicantAddress { get; private set; }

    /// <summary>
    /// Applicant's email address.
    /// Địa chỉ email của người nộp hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual string? ApplicantEmail { get; private set; }

    /// <summary>
    /// Applicant's phone number.
    /// Số điện thoại của người nộp hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual string? ApplicantPhoneNumber { get; private set; }

    /// <summary>
    /// Scope and content of activities.
    /// Phạm vi và nội dung hoạt động.
    /// </summary>
    [CanBeNull]
    public virtual string? ScopeOfActivity { get; private set; }

    /// <summary>
    /// Indicates if the applicant registered to receive results via postal service.
    /// Cho biết người nộp có đăng ký nhận kết quả qua đường bưu điện không.
    /// </summary>
    public virtual bool ReceiveByPost { get; private set; }

    /// <summary>
    /// Foreign key to the current workflow status (nullable). Set by workflow process.
    /// Khóa ngoại trỏ tới trạng thái quy trình hiện tại (có thể null). Được thiết lập bởi quy trình công việc.
    /// </summary>
    public virtual Guid? WorkflowStatusId { get; private set; }

    /// <summary>
    /// Navigation property to the related WorkflowStatus. Load explicitly.
    /// Thuộc tính điều hướng đến Trạng thái quy trình liên quan. Cần được load tường minh.
    /// </summary>
    [CanBeNull]
    public virtual WorkflowStatus? WorkflowStatus { get; protected set; }

    /// <summary>
    /// Submission date.
    /// Ngày nộp hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? SubmissionDate { get; private set; }

    /// <summary>
    /// Reception date.
    /// Ngày tiếp nhận hồ sơ.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? ReceptionDate { get; private set; }

    /// <summary>
    /// Expected result date.
    /// Ngày hẹn trả kết quả.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? AppointmentDate { get; private set; }

    /// <summary>
    /// Actual result date.
    /// Ngày trả kết quả thực tế.
    /// </summary>
    [CanBeNull]
    public virtual DateTime? ResultDate { get; private set; }

    /// <summary>
    /// Reason for rejection or additional info request.
    /// Lý do từ chối hoặc yêu cầu bổ sung thông tin.
    /// </summary>
    [CanBeNull]
    public virtual string? RejectionOrAdditionReason { get; private set; }

    /// <summary>
    /// Collection of component data (form JSON, file IDs).
    /// Tập hợp dữ liệu thành phần (JSON của form, ID của file).
    /// </summary>
    public virtual ICollection<BDocumentData> DocumentData { get; protected set; }

    //public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }

    /// <summary>
    /// Protected constructor for ORM.
    /// Constructor được bảo vệ cho ORM.
    /// </summary>
    protected BDocument()
    {
        Code = string.Empty;
        ApplicantName = string.Empty;
        DocumentData = new Collection<BDocumentData>();
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// Creates a new BDocument instance. Called via BDocumentManager.
    /// Tạo một instance BDocument mới. Được gọi thông qua BDocumentManager.
    /// </summary>
    internal BDocument(
        Guid id,
        Guid procedureId,
        [NotNull] string code,
        [NotNull] string applicantName,
        IClock clock,
        [CanBeNull] string? applicantIdentityNumber = null,
        [CanBeNull] string? applicantAddress = null,
        [CanBeNull] string? applicantEmail = null,
        [CanBeNull] string? applicantPhoneNumber = null,
        [CanBeNull] string? scopeOfActivity = null,
        bool receiveByPost = false,
        [CanBeNull] DateTime? submissionDate = null)
        : base(id)
    {
        ProcedureId = procedureId;
        SetCodeInternal(code);
        SetApplicantNameInternal(applicantName);
        SetApplicantIdentityNumberInternal(applicantIdentityNumber);
        SetApplicantAddressInternal(applicantAddress);
        SetApplicantEmailInternal(applicantEmail);
        SetApplicantPhoneNumberInternal(applicantPhoneNumber);
        SetScopeOfActivityInternal(scopeOfActivity);
        ReceiveByPost = receiveByPost;

        WorkflowStatusId = null;
        SubmissionDate = submissionDate ?? DateTime.Now;

        DocumentData = new Collection<BDocumentData>();
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    // --- Internal setters with validation ---

    private void SetCodeInternal([NotNull] string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code), BDocumentConsts.MaxCodeLength);
        Code = code;
    }

    private void SetApplicantNameInternal([NotNull] string applicantName)
    {
        Check.NotNullOrWhiteSpace(applicantName, nameof(applicantName), BDocumentConsts.MaxApplicantNameLength);
        ApplicantName = applicantName;
    }

    private void SetApplicantIdentityNumberInternal([CanBeNull] string? identityNumber)
    {
        Check.Length(identityNumber, nameof(identityNumber), BDocumentConsts.MaxApplicantIdentityNumberLength);
        ApplicantIdentityNumber = identityNumber;
    }

    private void SetApplicantAddressInternal([CanBeNull] string? address)
    {
        Check.Length(address, nameof(address), BDocumentConsts.MaxApplicantAddressLength);
        ApplicantAddress = address;
    }

    private void SetApplicantEmailInternal([CanBeNull] string? email)
    {
        Check.Length(email, nameof(email), BDocumentConsts.MaxApplicantEmailLength);
        ApplicantEmail = email;
    }

    private void SetApplicantPhoneNumberInternal([CanBeNull] string? phone)
    {
        Check.Length(phone, nameof(phone), BDocumentConsts.MaxApplicantPhoneNumberLength);
        ApplicantPhoneNumber = phone;
    }

    private void SetScopeOfActivityInternal([CanBeNull] string? scope)
    {
        ScopeOfActivity = scope;
    }

    private void SetRejectionOrAdditionReasonInternal([CanBeNull] string? reason)
    {
        Check.Length(reason, nameof(reason), BDocumentConsts.MaxRejectionOrAdditionReasonLength);
        RejectionOrAdditionReason = reason;
    }

    // --- Public methods to change state ---

    /// <summary>
    /// Updates main information. Called via BDocumentManager.
    /// Cập nhật thông tin chính. Được gọi thông qua BDocumentManager.
    /// </summary>
    internal BDocument UpdateInfo(
        [NotNull] string applicantName,
        [CanBeNull] string? applicantIdentityNumber,
        [CanBeNull] string? applicantAddress,
        [CanBeNull] string? applicantEmail,
        [CanBeNull] string? applicantPhoneNumber,
        [CanBeNull] string? scopeOfActivity,
        bool receiveByPost)
    {
        SetApplicantNameInternal(applicantName);
        SetApplicantIdentityNumberInternal(applicantIdentityNumber);
        SetApplicantAddressInternal(applicantAddress);
        SetApplicantEmailInternal(applicantEmail);
        SetApplicantPhoneNumberInternal(applicantPhoneNumber);
        SetScopeOfActivityInternal(scopeOfActivity);
        ReceiveByPost = receiveByPost;
        return this;
    }

    /// <summary>
    /// Changes the current workflow status. Validation happens in BDocumentManager.
    /// Thay đổi trạng thái quy trình hiện tại. Validation xảy ra trong BDocumentManager.
    /// </summary>
    internal BDocument SetWorkflowStatusId(Guid? newWorkflowStatusId,
        [CanBeNull] string? reason = null,
        [CanBeNull] DateTime? receptionDate = null,
        [CanBeNull] DateTime? resultDate = null)
    {
        WorkflowStatusId = newWorkflowStatusId;
        SetRejectionOrAdditionReasonInternal(reason);

        if(receptionDate.HasValue) ReceptionDate = receptionDate;
        if(resultDate.HasValue) ResultDate = resultDate;

        return this;
    }

    /// <summary>
    /// Updates appointment date.
    /// Cập nhật ngày hẹn trả.
    /// </summary>
    internal BDocument SetAppointmentDate([CanBeNull] DateTime? appointmentDate)
    {
        AppointmentDate = appointmentDate;
        return this;
    }

    // --- Methods to manage the DocumentData collection (Called by BDocumentManager) ---

    /// <summary>
    /// Adds new or updates existing data for a component.
    /// Thêm mới hoặc cập nhật dữ liệu hiện có cho một thành phần.
    /// </summary>
    internal void AddOrUpdateData(Guid procedureComponentId, [CanBeNull] string? inputData, [CanBeNull] Guid? fileId)
    {
        var existingData = DocumentData.FirstOrDefault(d => d.ProcedureComponentId == procedureComponentId);
        if (existingData != null)
        {
            existingData.SetData(inputData, fileId);
        }
        else
        {
            var newData = new BDocumentData(this.Id, procedureComponentId, inputData, fileId);
            DocumentData.Add(newData);
        }
    }

    /// <summary>
    /// Removes data entry for a component.
    /// Xóa mục dữ liệu cho một thành phần.
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
    /// Xóa tất cả dữ liệu thành phần liên quan.
    /// </summary>
    internal void ClearData()
    {
        DocumentData.Clear();
    }
} 