namespace Aqt.CoreFW.Components;

public static class ComponentConsts
{
    // !! Quan trọng: Cập nhật các giá trị MaxLength dựa trên component-srs.md hoặc yêu cầu cụ thể !!
    // Giá trị dưới đây là giả định, cần kiểm tra lại với SRS.
    public const int MaxCodeLength = 50; // Giả định
    public const int MaxNameLength = 250; // Giả định
    public const int MaxDescriptionLength = 1000; // Giả định - Tăng độ dài cho mô tả
    public const int MaxTempPathLength = 500; // Giả định - Cho phép đường dẫn dài

    // Không cần MaxLength cho FormDefinition vì có thể là TEXT/NVARCHAR(MAX)
} 