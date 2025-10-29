using ERP_WMS_TMS.Common.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP_WMS_TMS.Infrastructure.Data.Entities
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    public class SysUser : BaseAuditEntity
    {
        /// <summary>
        /// 用户名（登录用）
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码（MD5加密）
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string RealName { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(11)]
        public string? Phone { get; set; }

        /// <summary>
        /// 角色ID（关联SysRole表）
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色导航属性
        /// </summary>
        [ForeignKey(nameof(RoleId))]
        public SysRole? Role { get; set; }

        /// <summary>
        /// 状态（1：启用，0：禁用）
        /// </summary>
        [Required]
        public int Status { get; set; } = 1;
    }

    /// <summary>
    /// 系统角色表
    /// </summary>
    public class SysRole : BaseAuditEntity
    {
        /// <summary>
        /// 角色名称（如仓库操作员、运输调度员）
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// 权限JSON（存储模块权限配置）
        /// </summary>
        public string? Permissions { get; set; } = string.Empty;
    }
}