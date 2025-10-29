using ERP_WMS_TMS.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_WMS_TMS.Domain.WMS.Entities
{
    /// <summary>
    /// WMS物料表（管理所有库存物料基础信息）
    /// </summary>
    public class WmsMaterial : BaseBusinessEntity
    {
        /// <summary>
        /// 物料编码（唯一，如MAT-2024001）
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string MaterialCode { get; set; } = string.Empty;

        /// <summary>
        /// 物料名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// 物料规格（如10kg/袋、200mm*300mm）
        /// </summary>
        [MaxLength(50)]
        public string? Specification { get; set; }

        /// <summary>
        /// 物料单位（如个、kg、箱）
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// 物料类型（枚举：原材料/半成品/成品）
        /// </summary>
        [Required]
        public MaterialType MaterialType { get; set; }

        /// <summary>
        /// 物料重量（kg，精确到3位小数）
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal Weight { get; set; } = 0;

        /// <summary>
        /// 物料体积（m³，精确到4位小数）
        /// </summary>
        [Column(TypeName = "decimal(10,4)")]
        public decimal Volume { get; set; } = 0;

        /// <summary>
        /// 安全库存数量（低于此值触发预警）
        /// </summary>
        public int SafetyStockQty { get; set; } = 0;

        /// <summary>
        /// 供应商ID（关联ERP供应商表，后续ERP模块扩展）
        /// </summary>
        public int SupplierId { get; set; } = 0;

        /// <summary>
        /// 物料状态（1：启用，0：禁用）
        /// </summary>
        [Required]
        public int Status { get; set; } = 1;

        /// <summary>
        /// 备注信息
        /// </summary>
        [MaxLength(500)]
        public string? Remark { get; set; }

        /// <summary>
        /// 批次导航属性（一对多：一个物料对应多个批次）
        /// </summary>
        public ICollection<WmsMaterialBatch> MaterialBatches { get; set; } = new List<WmsMaterialBatch>();
    }

    /// <summary>
    /// 物料类型枚举
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// 原材料
        /// </summary>
        RawMaterial = 1,
        /// <summary>
        /// 半成品
        /// </summary>
        SemiFinished = 2,
        /// <summary>
        /// 成品
        /// </summary>
        FinishedProduct = 3
    }
}
