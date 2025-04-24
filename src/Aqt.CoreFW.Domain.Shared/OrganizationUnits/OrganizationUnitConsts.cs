namespace Aqt.CoreFW.OrganizationUnits; // Namespace mới

public static class OrganizationUnitConsts
{
    // Độ dài tối đa cho các thuộc tính mở rộng của OrganizationUnit, dựa trên kế hoạch tổng thể
    public const int MaxManualCodeLength = 50; // Độ dài cho Mã thủ công
    public const int MaxDescriptionLength = 500; // Độ dài cho Mô tả
    public const int MaxSyncRecordIdLength = 100; // Độ dài cho ID đồng bộ (điều chỉnh nếu cần)
    public const int MaxSyncRecordCodeLength = 50; // Độ dài cho Mã đồng bộ

    // Lưu ý: Volo.Abp.Identity.OrganizationUnit định nghĩa MaxCodeLength và MaxDisplayNameLength
    // Không cần định nghĩa lại ở đây trừ khi muốn ghi đè (không khuyến khích)
} 