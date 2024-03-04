

namespace Engine.ComDriver.PANalytical
{
    public class sComSuperQ5 : sComRuleAsc
    {
        public sComSuperQ5()
        {
            EDX = "\0";
        }

        public sComSuperQ5(DriverItem<NetworkCommParam> DrvItem) : base(DrvItem)
        {
            EDX = "\0";
        }
    }
}
