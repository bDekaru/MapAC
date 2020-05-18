using System;

namespace MapAC.DatLoader
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DatDatabaseTypeAttribute : Attribute
    {
        public DatDatabaseType Type { get; set; }

        public DatDatabaseTypeAttribute(DatDatabaseType type)
        {
            Type = type;
        }
    }
}
