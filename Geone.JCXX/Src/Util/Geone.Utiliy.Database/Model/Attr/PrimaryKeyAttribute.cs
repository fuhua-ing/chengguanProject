using System;

namespace Geone.Utiliy.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
        //public IgnoreAttribute(bool check)
        //{
        //    Check = check;
        //}

        //public bool Check { get; }
    }
}