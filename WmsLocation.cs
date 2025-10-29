using ERP_WMS_TMS.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_WMS_TMS.Domain.WMS.Entities
{
    /// <summary>
    /// WMS货位表（管理仓库货位信息，适配方案1.1货位分配逻辑）
    /// </summary>
    public class WmsLocation : BaseBusinessEntity
    {
        /// <summary>
        /// 货位编码（唯一，如A01-01-01：A区01排01层）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string LocationCode { get; set; } = string.Empty;

        /// <summary>
        /// 仓库ID（关联WmsWarehouse表）
        /// </summary>
        [Required]
        public int WarehouseId { get; set; }

        /// <summary>
        /// 仓库导航属性
        /// </summary>
        [ForeignKey(nameof(WarehouseId))]
        public WmsWarehouse? Warehouse { get; set; }

        /// <summary>
        /// 货位区域（如A区、B区、冷链区）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string LocationArea { get; set; } = string.Empty;

        /// <summary>
        /// 货位类型（枚举：普通货位/冷链货位/贵重品货位）
        /// </summary>
        [Required]
        public LocationType LocationType { get; set; }

        /// <summary>
        /// 货位最大容量（可存放物料的总体积m³）
        /// </summary>
        [Column(TypeName = "decimal(10,4)")]
        public decimal MaxVolume { get; set; } = 0;

        /// <summary>
        /// 货位当前占用容量（m³）
        /// </summary>
        [Column(TypeName = "decimal(10,4)")]
        public decimal UsedVolume { get; set; } = 0;

        /// <summary>
        /// 货位最大承重（kg）
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal MaxWeight { get; set; } = 0;

        /// <summary>
        /// 货位当前承重（kg）
        /// </summary>
        [Column(TypeName = "decimal(10,3)")]
        public decimal UsedWeight { get; set; } = 0;

        /// <summary>
        /// 货位状态（枚举：空闲/占用/禁用/锁定）
        /// </summary>
        [Required]
        public LocationStatus LocationStatus { get; set; } = LocationStatus.Idle;

        /// <summary>
        /// 锁定截止时间（状态为锁定时有效，如货位分配后锁定30分钟）
        /// </summary>
        public DateTime? LockEndTime { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [MaxLength(500)]
        public string? Remark { get; set; }

        /// <summary>
        /// 货位库存导航属性（一对多：一个货位对应多个库存记录）
        /// </summary>
        public ICollection<WmsInventory> Inventories { get; set; } = new List<WmsInventory>();
    }

    /// <summary>
    /// 货位类型枚举
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// 普通货位
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 冷链货位
        /// </summary>
        ColdChain = 2,
        /// <summary>
        /// 贵重品货位
        /// </summary>
        Valuable = 3
    }

    /// <summary>
    /// 货位状态枚举
    /// </summary>
    public enum LocationStatus
    {
        /// <summary>
        /// 空闲（可分配）
        /// </summary>
        Idle = 1,
        /// <summary>
        /// 占用（已存放物料）
        /// </summary>
        Occupied = 2,
        /// <summary>
        /// 禁用（维护中，不可用）
        /// </summary>
        Disabled = 3,
        /// <summary>
        /// 锁定（临时锁定，如分配中）
        /// </summary>
        Locked = 4
    }
}
