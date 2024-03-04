using System;
using System.Collections.Generic;

namespace Engine.Common
{

    //var personEqualityComparer = new EqualityComparer<Person>(
    //(x, y) => x.Name == y.Name && x.Age == y.Age,
    //obj => obj.Name.GetHashCode() ^ obj.Age.GetHashCode()
    //);

    // 使用比较器进行对象比较
    //Person person1 = new Person { Name = "Alice", Age = 25 };
    //Person person2 = new Person { Name = "Alice", Age = 25 };

    //bool areEqual = personEqualityComparer.Equals(person1, person2);  // 返回 true

    /// <summary>
    /// 通用比较类,主要用于比较两个对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomEqualComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> equalsFunc;
        private readonly Func<T, int> hashCodeFunc;

        public CustomEqualComparer(Func<T, T, bool> equalsFunc, Func<T, int> hashCodeFunc)
        {
            this.equalsFunc = equalsFunc ?? throw new ArgumentNullException(nameof(equalsFunc));
            this.hashCodeFunc = hashCodeFunc ?? throw new ArgumentNullException(nameof(hashCodeFunc));
        }

        public bool Equals(T x, T y)
        {
            return equalsFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return hashCodeFunc(obj);
        }
    }

    /// <summary>
    /// 主要用于自定义类型本身
    /// 比较相同类型的对象
    /// </summary>
    public class Entity : IEquatable<Entity>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Equals(Entity other)
        {
            if (other == null)
                return false;

            return Id == other.Id && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((Entity)obj);
        }

        public override int GetHashCode()
        {
            return (Id.GetHashCode() ^ Name.GetHashCode());
        }
    }
}
