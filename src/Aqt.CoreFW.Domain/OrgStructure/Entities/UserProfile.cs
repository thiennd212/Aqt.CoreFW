    using System;
    using Aqt.CoreFW.Domain.Shared.OrgStructure; // Sẽ tạo ở Domain.Shared sau
    using JetBrains.Annotations;
    using Volo.Abp;
    using Volo.Abp.Domain.Entities.Auditing;
    using Volo.Abp.Users; // Required for UserId reference

    namespace Aqt.CoreFW.Domain.OrgStructure.Entities;

    /// <summary>
    /// Represents extended profile information for an AbpIdentityUser.
    /// </summary>
    public class UserProfile : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Foreign key to the AbpIdentityUser.
        /// Should have a unique constraint in the database mapping.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// User's full name.
        /// </summary>
        public virtual string FullName { get; private set; }

        /// <summary>
        /// User's phone number (optional).
        /// </summary>
        public virtual string? PhoneNumber { get; private set; }

        /// <summary>
        /// User's address (optional).
        /// </summary>
        public virtual string? Address { get; private set; }

        /// <summary>
        /// URL of the user's avatar image (optional).
        /// </summary>
        public virtual string? AvatarUrl { get; private set; }

        /// <summary>
        /// Protected constructor for ORM.
        /// </summary>
        protected UserProfile() { /* For ORM */ }

        /// <summary>
        /// Creates a new instance of the <see cref="UserProfile"/> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="userId">The ID of the associated AbpIdentityUser.</param>
        /// <param name="fullName">The user's full name (required).</param>
        public UserProfile(Guid id, Guid userId, [NotNull] string fullName) : base(id)
        {
            // Consider checking if a profile for this userId already exists in a Domain Service or Application Service
            UserId = userId;
            SetFullName(fullName); // Use setter for validation
        }

        /// <summary>
        /// Sets the user's full name.
        /// </summary>
        /// <param name="fullName">The new full name.</param>
        /// <returns>The current UserProfile instance.</returns>
        public UserProfile SetFullName([NotNull] string fullName)
        {
            // Ensure OrgStructureConsts is defined in Domain.Shared later
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Check.Length(fullName, nameof(fullName), OrgStructureConsts.MaxUserProfileFullNameLength);
            FullName = fullName;
            return this;
        }

        /// <summary>
        /// Updates the user's profile information (excluding Avatar).
        /// </summary>
        /// <param name="fullName">The new full name.</param>
        /// <param name="phoneNumber">The new phone number (optional).</param>
        /// <param name="address">The new address (optional).</param>
        /// <returns>The current UserProfile instance.</returns>
        public UserProfile UpdateProfile([NotNull] string fullName, [CanBeNull] string? phoneNumber, [CanBeNull] string? address)
        {
            SetFullName(fullName); // Reuse validation

            // Add specific validations if needed (e.g., phone number format regex, address length)
            // Check.Length(phoneNumber, nameof(phoneNumber), MaxPhoneNumberLength, 0);
            // Check.Length(address, nameof(address), MaxAddressLength, 0);
            PhoneNumber = phoneNumber;
            Address = address;
            return this;
        }

        /// <summary>
        /// Changes the user's avatar URL.
        /// </summary>
        /// <param name="avatarUrl">The new avatar URL (optional).</param>
        /// <returns>The current UserProfile instance.</returns>
        public UserProfile ChangeAvatar([CanBeNull] string? avatarUrl)
        {
            // Add URL validation if needed
            // Consider checking if the URL is well-formed or reachable (might be better in Application layer)
            AvatarUrl = avatarUrl;
            return this;
        }
    }