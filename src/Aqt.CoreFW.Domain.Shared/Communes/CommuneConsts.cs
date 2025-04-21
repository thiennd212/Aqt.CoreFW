// src/Aqt.CoreFW.Domain.Shared/Communes/CommuneConsts.cs
namespace Aqt.CoreFW.Domain.Shared.Communes;

public static class CommuneConsts
{
    // !!! Quan trọng: Xem xét lại các giá trị MaxLength này dựa trên tài liệu SRS thực tế (commune-srs.md) !!!
    public const int MaxCodeLength = 50;
    public const int MaxNameLength = 255;
    public const int MaxDescriptionLength = 500;
    public const int MaxSyncIdLength = 50;
    public const int MaxSyncCodeLength = 50;
}