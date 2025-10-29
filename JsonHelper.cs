using Newtonsoft.Json;
using System;

namespace ERP_WMS_TMS.Common.Helpers
{
    /// <summary>
    /// JSON序列化/反序列化工具（封装Newtonsoft.Json，适配方案8.2组件封装）
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 序列化（默认配置：日期格式yyyy-MM-dd HH:mm:ss，忽略空值）
        /// </summary>
        /// <param name="obj">待序列化对象</param>
        /// <returns>JSON字符串</returns>
        public static string Serialize(object obj)
        {
            if (obj == null)
                return string.Empty;

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>目标类型对象</returns>
        public static T? Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                // 写入错误日志（无需await，后台执行）
                _ = LogHelper.WriteLogAsync(
                    moduleName: "Common",
                    logLevel: LogLevel.Error,
                    businessNo: "JSON_DESERIALIZE",
                    content: $"JSON反序列化失败，类型：{typeof(T).Name}，错误：{ex.Message}",
                    operatorId: 0);

                return default;
            }
        }
    }
}