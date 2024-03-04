using System;
using System.Collections.Generic;
using System.Reflection;

namespace Engine.Common
{
    public class MyList<T> : List<T>, IEquatable<T>
    {
        private int ID { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (MethodBase.GetCurrentMethod().DeclaringType.FullName + "#"
                + ID).GetHashCode();
        }

        public bool Equals(T model)
        {
            return ID == 0;
        }
    }
}
