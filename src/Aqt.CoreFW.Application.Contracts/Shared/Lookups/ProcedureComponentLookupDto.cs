using Aqt.CoreFW.Components;
using System;

namespace Aqt.CoreFW.Application.Contracts.Shared.Lookups;

/// <summary>
/// DTO dùng cho việc lookup ProcedureComponent, bao gồm các thông tin cơ bản.
/// </summary>
public class ProcedureComponentLookupDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; }
    // Thêm các thuộc tính khác nếu cần cho việc lookup
} 