// src/Aqt.CoreFW.Domain.Shared/WorkflowStatuses/WorkflowStatusConsts.cs
namespace Aqt.CoreFW.Domain.Shared.WorkflowStatuses; // Đảm bảo namespace đúng theo kế hoạch

public static class WorkflowStatusConsts
{
    // Độ dài tối đa theo kế hoạch
    public const int MaxCodeLength = 20;
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxColorCodeLength = 7; // Ví dụ: #RRGGBB
}