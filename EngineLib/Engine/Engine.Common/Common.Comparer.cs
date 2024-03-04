using System;
using System.Collections.Generic;

namespace Engine.Common
{
    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDirection
    {
        asc,
        desc,
    }

    /// <summary>
    /// 自定义排序器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomCompare<T> : IComparer<T>
    {
        private string OrderPropName;
        private SortDirection SortDirection;
        public CustomCompare(SortDirection _SortDirection = SortDirection.asc, string _OrderPropName = "OrderID")
        {
            OrderPropName = _OrderPropName;
            SortDirection = _SortDirection;
        }
        public int Compare(T x, T y)
        {
            try
            {
                if (SortDirection == SortDirection.asc)
                    return x.GetPropValue(OrderPropName).ToMyInt().CompareTo(y.GetPropValue(OrderPropName).ToMyInt());
                else
                    return y.GetPropValue(OrderPropName).ToMyInt().CompareTo(x.GetPropValue(OrderPropName).ToMyInt());
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// 自定义列表排序器 按照索引顺序排序
    /// </summary>
    public class CustomComparer : IComparer<string>
    {
        private List<string> keyOrder;

        public CustomComparer(List<string> order)
        {
            keyOrder = order;
        }

        public int Compare(string x, string y)
        {
            int index1 = keyOrder.IndexOf(x);
            int index2 = keyOrder.IndexOf(y);

            if (index1 == -1) index1 = int.MaxValue;
            if (index2 == -1) index2 = int.MaxValue;

            return index1.CompareTo(index2);
        }
    }
}
