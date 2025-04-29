using System.ComponentModel.DataAnnotations; // Optional, nếu cần validation

namespace Aqt.CoreFW.Web.Pages.Shared.ViewModels;

/// <summary>
/// ViewModel dùng chung để chứa dữ liệu cần thiết cho việc render một form động.
/// </summary>
public class DynamicFormViewModel
{
    /// <summary>
    /// Chuỗi JSON định nghĩa cấu trúc của form (mảng fields, label, type, etc.).
    /// Bắt buộc phải có.
    /// </summary>
    [Required] // Đánh dấu là bắt buộc nếu cần
    public string FormDefinition { get; set; } = string.Empty; // Khởi tạo để tránh null ref

    /// <summary>
    /// Chuỗi JSON chứa dữ liệu ban đầu cho form (dạng key-value).
    /// Có thể là null hoặc chuỗi JSON rỗng nếu là form mới.
    /// </summary>
    public string? FormData { get; set; }

    /// <summary>
    /// (Tùy chọn) Một định danh duy nhất cho instance của form này,
    /// hữu ích khi render nhiều form động trên cùng một trang để tạo ID HTML duy nhất.
    /// </summary>
    public string? InstanceId { get; set; }

    /// <summary>
    /// (Tùy chọn) Tên của form hoặc section, dùng để hiển thị tiêu đề hoặc mục đích khác.
    /// </summary>
    public string? FormName { get; set; }

    // Bạn có thể thêm các thuộc tính khác nếu cần cho mục đích render chung,
    // ví dụ: cờ bật/tắt chế độ chỉ đọc cho toàn form.
    // public bool IsReadonly { get; set; } = false;
}