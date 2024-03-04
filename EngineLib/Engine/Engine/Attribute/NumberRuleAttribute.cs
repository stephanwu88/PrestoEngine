using System.Text;

namespace Engine.Common
{
    /// <summary>
    /// 验证对象值是否介于最大值和最小值之间，若不在该范围，则验证失败
    /// [NumberRule(0,150)]
    /// </summary>
    public class NumberRuleAttribute : AttributeBase
    {
        public NumberRuleAttribute(double minValue, double maxValue, string errorMessage = null)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            if (string.IsNullOrEmpty(errorMessage))
            {
                var sb = new StringBuilder(1024);
                sb.Append("The value should between ");
                sb.Append(minValue);
                sb.Append(" and ");
                sb.Append(maxValue);
                ErrorMessage = sb.ToString();
            }
            else
            {
                ErrorMessage = errorMessage;
            }

        }
        private double MinValue { get; }
        private double MaxValue { get; }

        public override bool IsValid(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // 若输入的是非数值
            if (!double.TryParse(obj.ToString(), out var value))
            {
                return false;
            }
            // 若不满足最大值和最小值限制
            if (value > MaxValue || value < MinValue)
                return false;
            return true;
        }
    }
}
