using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MYMVCAPP.Repository
{
    /// <summary>
    /// Tiện ích mở rộng cho ISession – cho phép lưu và đọc object dưới dạng JSON.
    /// Dùng Newtonsoft.Json để dễ debug và tránh lỗi serialize phức tạp.
    /// </summary>
    public static class SessionExtensions
    {
        // 🟢 Lưu object vào session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            if (session == null || string.IsNullOrEmpty(key)) return;
            var jsonData = JsonConvert.SerializeObject(value);
            session.SetString(key, jsonData);
        }

        // 🟢 Đọc object từ session
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            if (session == null || string.IsNullOrEmpty(key)) return default;
            var jsonData = session.GetString(key);
            return string.IsNullOrEmpty(jsonData)
                ? default
                : JsonConvert.DeserializeObject<T>(jsonData);
        }

        // 🟡 Tên ngắn gọn alias (nếu sau này muốn dùng cách khác)
        public static void SetJson(this ISession session, string key, object value)
            => SetObjectAsJson(session, key, value);

        public static T? GetJson<T>(this ISession session, string key)
            => GetObjectFromJson<T>(session, key);

        // 🗑️ Xóa session theo key
        public static void RemoveJson(this ISession session, string key)
        {
            if (session != null && !string.IsNullOrEmpty(key))
                session.Remove(key);
        }
    }
}