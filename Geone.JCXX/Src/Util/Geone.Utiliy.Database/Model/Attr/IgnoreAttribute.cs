using System;

namespace Geone.Utiliy.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        //public IgnoreAttribute(bool check)
        //{
        //    Check = check;
        //}

        //public bool Check { get; }
    }
}