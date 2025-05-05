using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Aqt.CoreFW.BDocuments.Dtos
{
    /// <summary>
    /// DTO để thêm hoặc cập nhật dữ liệu chi tiết của một hồ sơ (BDocumentData)
    /// liên kết với một thành phần thủ tục cụ thể (ProcedureComponent).
    /// </summary>
    public class AddOrUpdateBDocumentDataInputDto : EntityDto<Guid?> // Id là của BDocumentData nếu cập nhật
    {
        /// <summary>
        /// ID của hồ sơ cha (BDocument). Bắt buộc.
        /// </summary>
        [Required]
        public Guid BDocumentId { get; set; }

        /// <summary>
        /// ID của thành phần thủ tục (ProcedureComponent) mà dữ liệu này thuộc về. Bắt buộc.
        /// </summary>
        [Required]
        public Guid ProcedureComponentId { get; set; }

        /// <summary>
        /// Dữ liệu đầu vào (ví dụ: JSON cho loại Form).
        /// </summary>
        public string? InputData { get; set; } // Thay thế JsonData/XmlData

        /// <summary>
        /// ID của tệp đính kèm (nếu component là loại File).
        /// </summary>
        public Guid? FileId { get; set; } // Thêm FileId

        // Thuộc tính Note đã bị loại bỏ vì không có trong entity BDocumentData
    }
} 