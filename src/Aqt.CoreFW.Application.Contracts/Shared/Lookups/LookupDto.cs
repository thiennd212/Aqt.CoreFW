using System;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

/// <summary>
/// DTO đơn giản để dùng trong các dropdown lookup.
/// </summary>
/// <typeparam name="TKey">Kiểu dữ liệu của khóa chính.</typeparam>
public class LookupDto<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
    public string? DisplayName { get; set; }
}

/// <summary>
/// LookupDto với khóa chính là Guid.
/// </summary>
public class LookupDto : LookupDto<Guid>
{
} 