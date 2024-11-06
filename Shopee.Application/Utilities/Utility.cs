using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Utilities
{
    public static class Utility
    {
        public static Expression<Func<T, object>>? GetPropertyByString<T>(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            // Tạo parameter cho biểu thức
            var param = Expression.Parameter(typeof(T), "x");

            // Lấy thuộc tính từ tên thuộc tính
            var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null) return null;

            // Truy cập thuộc tính
            var propertyAccess = Expression.MakeMemberAccess(param, property);

            // Chuyển đổi về object
            var conversion = Expression.Convert(propertyAccess, typeof(object));
            return Expression.Lambda<Func<T, object>>(conversion, param);
        }
    }
}
