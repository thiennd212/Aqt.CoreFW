// src/Aqt.CoreFW.Domain.Shared/CoreFWDomainErrorCodes.WorkflowStatuses.cs
namespace Aqt.CoreFW;

// Phần mở rộng của CoreFWDomainErrorCodes dành riêng cho WorkflowStatus
public static partial class CoreFWDomainErrorCodes
{
    // WorkflowStatus Error Codes (Format: CoreFW:[Module]:#####)
    // !!! Quan trọng: Hãy kiểm tra số thứ tự cuối cùng (ví dụ: 00030)
    // và chọn số duy nhất tiếp theo. Ví dụ dưới đây bắt đầu từ 00031.
    public const string WorkflowStatusCodeAlreadyExists = "CoreFW:WorkflowStatuses:00031";
    public const string WorkflowStatusNameAlreadyExists = "CoreFW:WorkflowStatuses:00032";
    public const string CannotDeleteWorkflowStatusInUse = "CoreFW:WorkflowStatuses:00033";
}