

using Engine.Data;

namespace Engine.Automation.Sparker
{
    public class ElemCell : NotifyObject
    {
        public string Value { get; set; }
        public string State { get => _State; set { _State = value; RaisePropertyChanged(); } }
        private string _State { get; set; }
    }

    public class AnaElemData: NotifyObject
    {
        public string ElementName { get; set; }
        public string PotState { get; set; }
        public ElemCell N1 { get => N1; set { N1 = value; RaisePropertyChanged(); } } 
        private ElemCell _N1 { get; set; } = new ElemCell();
        public ElemCell N2 { get => N2; set { N2 = value; RaisePropertyChanged(); } }
        private ElemCell _N2 { get; set; } = new ElemCell();
        public ElemCell N3 { get => N3; set { N3 = value; RaisePropertyChanged(); } }
        private ElemCell _N3 { get; set; } = new ElemCell();
        public ElemCell N4 { get => N4; set { N4 = value; RaisePropertyChanged(); } }
        private ElemCell _N4 { get; set; } = new ElemCell();
        public ElemCell N5 { get => N5; set { N5 = value; RaisePropertyChanged(); } }
        private ElemCell _N5 { get; set; } = new ElemCell();
        public ElemCell N6 { get => N6; set { N6 = value; RaisePropertyChanged(); } }
        private ElemCell _N6 { get; set; } = new ElemCell();
        public ElemCell N7 { get => N7; set { N7 = value; RaisePropertyChanged(); } }
        private ElemCell _N7 { get; set; } = new ElemCell();
        public ElemCell N8 { get => N8; set { N8 = value; RaisePropertyChanged(); } }
        private ElemCell _N8 { get; set; } = new ElemCell();
        public string Mean { get; set; }
        public string RSD { get; set; }
        public string AVG { get; set; }
    }

    public class SampleAnaStruct
    {
       
    }
}