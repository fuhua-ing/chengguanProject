using System;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtend
    {
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return (source.IndexOf(value, comparisonType) >= 0);
        }
    }
}