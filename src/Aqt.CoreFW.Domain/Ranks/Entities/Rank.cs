using System;
using Aqt.CoreFW.Ranks; // Namespace chứa Consts và Enum từ Domain.Shared
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Ranks.Entities // Namespace cụ thể cho Entity
{
    /// <summary>
    /// Represents a rank entity.
    /// Inherits from FullAuditedAggregateRoot for audit logging and soft delete.
    /// </summary>
    public class Rank : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Unique code for the rank. Cannot be changed after creation.
        /// Enforced globally unique by Domain/Application Service.
        /// </summary>
        [NotNull]
        public virtual string Code { get; private set; }

        /// <summary>
        /// Name of the rank.
        /// </summary>
        [NotNull]
        public virtual string Name { get; private set; }

        /// <summary>
        /// Status of the rank (Active/Inactive).
        /// </summary>
        public virtual RankStatus Status { get; private set; }

        /// <summary>
        /// Display or processing order.
        /// </summary>
        public virtual int Order { get; private set; }

        /// <summary>
        /// Optional description.
        /// </summary>
        [CanBeNull]
        public virtual string? Description { get; private set; }

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
        /// Protected constructor for ORM frameworks.
        /// </summary>
        protected Rank()
        {
            /* For ORM */
            Code = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
            Name = string.Empty; // Khởi tạo giá trị mặc định hợp lệ
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Rank"/> class.
        /// Ensures required fields are provided and validates initial state.
        /// Use RankManager to create instances.
        /// </summary>
        /// <param name="id">The unique identifier for the rank.</param>
        /// <param name="code">The unique code for the rank.</param>
        /// <param name="name">The name of the rank.</param>
        /// <param name="order">The display order.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="status">The status of the rank.</param>
        /// <param name="lastSyncDate">Optional last synchronization date.</param>
        /// <param name="syncRecordId">Optional synchronization record ID.</param>
        /// <param name="syncRecordCode">Optional synchronization record code.</param>
        internal Rank( // Constructor internal để buộc tạo qua RankManager
            Guid id,
            [NotNull] string code,
            [NotNull] string name,
            int order = 0,
            [CanBeNull] string? description = null,
            RankStatus status = RankStatus.Active,
            [CanBeNull] DateTime? lastSyncDate = null,
            [CanBeNull] Guid? syncRecordId = null,
            [CanBeNull] string? syncRecordCode = null)
            : base(id)
        {
            // Set Code trực tiếp, chỉ một lần và đã được validate bởi RankManager
            SetCodeInternal(code);

            // Set các thuộc tính khác qua internal setters để validate
            SetNameInternal(name);
            SetOrderInternal(order);
            SetDescriptionInternal(description);
            Status = status; // Gán trực tiếp enum
            SetSyncInfoInternal(lastSyncDate, syncRecordId, syncRecordCode);
        }

        // --- Internal setters with validation ---

        // Code chỉ được set 1 lần trong constructor
        private void SetCodeInternal([NotNull] string code)
        {
            // RankManager đã kiểm tra tính duy nhất
            Check.NotNullOrWhiteSpace(code, nameof(code), RankConsts.MaxCodeLength);
            Code = code;
        }

        private void SetNameInternal([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), RankConsts.MaxNameLength);
            Name = name;
        }

        private void SetOrderInternal(int order)
        {
            // Có thể thêm validation nếu Order có ràng buộc (ví dụ: >= 0)
            Order = order;
        }

        private void SetDescriptionInternal([CanBeNull] string? description)
        {
            Check.Length(description, nameof(description), RankConsts.MaxDescriptionLength);
            Description = description;
        }

        private void SetSyncInfoInternal([CanBeNull] DateTime? lastSyncDate, [CanBeNull] Guid? syncRecordId, [CanBeNull] string? syncRecordCode)
        {
            // Check độ dài SyncRecordCode
            Check.Length(syncRecordCode, nameof(syncRecordCode), RankConsts.MaxSyncRecordCodeLength);

            LastSyncDate = lastSyncDate;
            SyncRecordId = syncRecordId;
            SyncRecordCode = syncRecordCode;
        }


        // --- Public methods to change state (có thể gọi từ Application Service hoặc RankManager) ---

        /// <summary>
        /// Changes the name of the rank.
        /// </summary>
        public Rank SetName([NotNull] string name) // Public vì thay đổi tên không phức tạp
        {
            SetNameInternal(name);
            return this;
        }

        /// <summary>
        /// Changes the display/processing order.
        /// </summary>
        public Rank SetOrder(int order) // Public
        {
            SetOrderInternal(order);
            return this;
        }

        /// <summary>
        /// Changes the description.
        /// </summary>
        public Rank SetDescription([CanBeNull] string? description) // Public
        {
            SetDescriptionInternal(description);
            return this;
        }

        /// <summary>
        /// Sets the rank status to Active.
        /// </summary>
        public Rank Activate() // Public
        {
            Status = RankStatus.Active;
            return this;
        }

        /// <summary>
        /// Sets the rank status to Inactive.
        /// </summary>
        public Rank Deactivate() // Public
        {
            Status = RankStatus.Inactive;
            return this;
        }

        /// <summary>
        /// Updates the synchronization information.
        /// </summary>
        public Rank SetSyncInfo([CanBeNull] DateTime? lastSyncDate, [CanBeNull] Guid? syncRecordId, [CanBeNull] string? syncRecordCode) // Public
        {
            SetSyncInfoInternal(lastSyncDate, syncRecordId, syncRecordCode);
            return this;
        }

        // Lưu ý: Không có public SetCode method.
    }
}