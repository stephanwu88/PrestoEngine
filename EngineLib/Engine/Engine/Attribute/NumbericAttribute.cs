
namespace Engine.Common
{
    /// <summary>
    /// 验证对象值是否数值
    /// [Numberic()]
    /// </summary>
    public class NumbericAttribute : AttributeBase
    {
        public NumbericAttribute(string errorMessage = null)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                ErrorMessage = "无效的数值";
            }
            else
            {
                ErrorMessage = errorMessage;
            }

        }
        public override bool IsValid(object obj)
        {
            //空值不验证
            if (obj == null || string.IsNullOrEmpty(obj.ToMyString()))
            {
                return true;
            }
            // 若输入的是非数值
            return double.TryParse(obj.ToString(), out var value);
        }
    }
}
