using ERP_WMS_TMS.Common.Entities;
using ERP_WMS_TMS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ERP_WMS_TMS.Common.Helpers
{
    /// <summary>
    /// 日志工具类（统一日志写入，适配方案6.1统一日志中心）
    /// </summary>
    public static class LogHelper
    {
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// 初始化服务提供器（程序启动时调用）
        /// </summary>
        /// <param name="serviceProvider">DI服务提供器</param>
        public static void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="moduleName">模块名称（WMS/TMS/ERP/Integration）</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="businessNo">业务编号（如入库单号、运输单号）</param>
        /// <param name="content">日志内容</param>
        /// <param name="operatorId">操作人ID（默认0：系统操作）</param>
        /// <param name="requestId">请求ID（跨模块关联用，默认自动生成）</param>
        public static async Task WriteLogAsync(
            string moduleName,
            LogLevel logLevel,
            string businessNo,
            string content,
            int operatorId = 0,
            string? requestId = null)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("LogHelper未初始化，请先调用Init方法");

            // 自动生成RequestId（32位Guid）
            requestId ??= Guid.NewGuid().ToString("N");

            // 从DI获取日志仓储
            using var scope = _serviceProvider.CreateScope();
            var logRepo = scope.ServiceProvider.GetRequiredService<IRepository<SystemLog>>();

            // 构造日志实体
            var log = new SystemLog
            {
                RequestId = requestId,
                ModuleName = moduleName,
                LogLevel = logLevel,
                BusinessNo = businessNo,
                Content = content,
                OperatorId = operatorId,
                CreateTime = DateTime.Now
            };

            // 写入数据库（批量写入逻辑后续在Infrastructure层扩展）
            await logRepo.AddAsync(log);
        }
    }

    /// <summary>
    /// 日志级别枚举
    /// </summary>
    public enum LogLevel
    {
        Info = 1,    // 信息
        Warn = 2,    // 警告
        Error = 3,   // 错误
        Fatal = 4    // 致命错误
    }

    /// <summary>
    /// 系统日志实体（对应数据库SystemLog表）
    /// </summary>
    public class SystemLog : BaseAuditEntity
    {
        /// <summary>
        /// 请求ID（跨模块关联用）
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// 模块名称
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// 日志级别（对应LogLevel枚举）
        /// </summary>
        [Required]
        public int LogLevel { get; set; }

        /// <summary>
        /// 业务编号
        /// </summary>
        [MaxLength(50)]
        public string BusinessNo { get; set; } = string.Empty;

        /// <summary>
        /// 日志内容
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}