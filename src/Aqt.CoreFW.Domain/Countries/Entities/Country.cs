using System;
using Aqt.CoreFW.Domain.Shared.Countries; // Sẽ tạo ở Bước 2 (Domain.Shared)
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Aqt.CoreFW.Domain.Countries.Entities;

/// <summary>
/// Represents a Country entity.
/// </summary>
public class Country : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// The unique code of the country (e.g., VN, US).
    /// </summary>
    public virtual string Code { get; private set; }

    /// <summary>
    /// The name of the country.
    /// </summary>
    public virtual string Name { get; private set; } // Changed to private set

    /// <summary>
    /// Constructor for ORM.
    /// </summary>
    protected Country() { /* Required for ORM */ }

    /// <summary>
    /// Creates a new instance of the <see cref="Country"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the country.</param>
    /// <param name="code">The country code.</param>
    /// <param name="name">The country name.</param>
    public Country(Guid id, [NotNull] string code, [NotNull] string name) : base(id)
    {
        SetCode(code);
        SetName(name);
    }

    /// <summary>
    /// Sets the country code after validation.
    /// </summary>
    /// <param name="code">The country code.</param>
    /// <returns>The Country entity.</returns>
    public Country SetCode([NotNull] string code) // Changed to public
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        // Ensure CountryConsts is accessible (will be created in Domain.Shared step)
        Check.Length(code, nameof(code), CountryConsts.MaxCodeLength);
        Code = code;
        return this;
    }

    /// <summary>
    /// Sets the country name after validation.
    /// </summary>
    /// <param name="name">The country name.</param>
    /// <returns>The Country entity.</returns>
    public Country SetName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        // Ensure CountryConsts is accessible (will be created in Domain.Shared step)
        Check.Length(name, nameof(name), CountryConsts.MaxNameLength);
        Name = name;
        return this;
    }
} 