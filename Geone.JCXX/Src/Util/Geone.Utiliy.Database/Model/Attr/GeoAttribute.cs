using System;

namespace Geone.Utiliy.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GeoAttribute : Attribute
    {
        //public GeoAttribute(int srid = 0)
        //{
        //    Srid = srid;
        //}

        //public int Srid { get; }
    }
}