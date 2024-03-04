using System;
using PlcTypes = Engine.ComDriver.Types;

namespace Engine.ComDriver.Siemens
{
    /// <summary>
    /// Siemens numeric format to C# 
    /// </summary>
    public static partial class Conversion
    {
        /// <summary>
        /// Converts from double to DWord (DBD)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt32 ConvertToUInt(this double input)
        {
            uint output;
            output = PlcTypes.DWord.FromByteArray(PlcTypes.Real.ToByteArray(input, SystemDefault.SiemensByteOrder32), SystemDefault.SiemensByteOrder32);
            return output;
        }

        /// <summary>
        /// Converts from DWord (DBD) to double
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double ConvertToDouble(this uint input)
        {
            double output;
            output = PlcTypes.Real.FromByteArray(PlcTypes.DWord.ToByteArray(input, SystemDefault.SiemensByteOrder32), SystemDefault.SiemensByteOrder32);
            return output;
        }
    }
}
