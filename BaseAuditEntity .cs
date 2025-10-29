using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_WMS_TMS.Common.Entities
{
    /// <summary>
    /// 审计实体基类（所有业务实体继承此类，含公共审计字段）
    /// </summary>
    public abstract class BaseAuditEntity
    {
        /// <summary>
        /// 主键ID（自增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 创建人ID（关联系统用户表）
        /// </summary>
        [Required]
        public int CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public int? UpdateUserId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否删除（软删除标记）
        /// </summary>
        [Required]
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// 业务编号实体基类（含业务唯一编号字段）
    /// </summary>
    public abstract class BaseBusinessEntity : BaseAuditEntity
    {
        /// <summary>
        /// 业务唯一编号（如WMS入库单号、TMS运输单号）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string BusinessNo { get; set; } = string.Empty;
    }
}