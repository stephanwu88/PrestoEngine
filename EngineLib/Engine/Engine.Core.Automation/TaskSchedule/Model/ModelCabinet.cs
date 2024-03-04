using Engine.Data.DBFAC;
using System.Windows.Media;
using DefPosKey = Engine.PosKey;

namespace Engine.Core.TaskSchedule
{
    [Table(Name = "location_cabinet", Comments = "子工位")]
    public class ModelCabinet : ModelSubPos
    {

    }

    [Table(Name = "location_cabinet", Comments = "子工位")]
    public class ViewCabinet : ModelCabinet
    {
        private string _PosKey;

        [Column(Name = "PosKey", Comments = "工位状态")]
        public override string PosKey
        {
            get => _PosKey;
            set
            {
                if (_PosKey != value)
                {
                    _PosKey = value;
                    //RaisePropertyChanged("PosKey");
                    if(_PosKey== DefPosKey.Empty)
                        _PosColorMark = new SolidColorBrush(Colors.Yellow);
                    else if(_PosKey==DefPosKey.WaitSample)
                        _PosColorMark = new SolidColorBrush(Colors.White);
                    else if(_PosKey == DefPosKey.Sampled)
                        _PosColorMark = new SolidColorBrush(Colors.Green);
                    else if(_PosKey==DefPosKey.Timed)
                        _PosColorMark = new SolidColorBrush(Colors.Red);
                    else if(_PosKey==DefPosKey.Disabled)
                        _PosColorMark = new SolidColorBrush(Colors.Gray);
                    else
                        _PosColorMark = new SolidColorBrush(Colors.Gray);
                   // RaisePropertyChanged("PosColorMark");
                }
            }
        }

        private SolidColorBrush _PosColorMark;
        public SolidColorBrush PosColorMark
        {
            get => _PosColorMark;
            set
            {
                if (_PosColorMark != value)
                {
                    _PosColorMark = value;
                    //RaisePropertyChanged("PosColorMark");
                }
            }
        }
    }
}
