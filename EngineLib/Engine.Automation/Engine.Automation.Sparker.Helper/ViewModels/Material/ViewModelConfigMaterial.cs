using Engine.Common;
using Engine.Data.DBFAC;
using Engine.MVVM;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    public class ViewModelConfigMaterial : ViewFrameBase
    {
        private IDBFactory<ServerNode> DbMyCon = DbFactory.Data;

        /// <summary>
        /// 分析程序
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get { return _AnaPgm; }
            set
            {
                _AnaPgm = value;
                MaterialMain.PgmToken = AnaPgm.Token;
                MaterialMain.Token = SystemDefault.UUID;
                RaisePropertyChanged();
            }
        }
        private ModelSpecPgm _AnaPgm;

        
        /// <summary>
        /// 仪器名称
        /// </summary>
        public string InsName
        {
            get { return _InsName; }
            set { _InsName = value; RaisePropertyChanged(); }
        }
        private string _InsName;

        /// <summary>
        /// 
        /// </summary>
        private void GetMaterialList()
        {
            //string strSql  = "select distinct Material from material_main " 
            //DbMyCon.ExcuteQuery();
        }

        public ModelMaterialMain MaterialMain
        {
            get { return _MaterialMain; }
            set { _MaterialMain = value; RaisePropertyChanged(); }
        }
        private ModelMaterialMain _MaterialMain = new ModelMaterialMain();

        /// <summary>
        /// 提交数据
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((parameter) =>
            {
                CallResult res = MaterialMain.Validate();
                if (res.Fail)
                {
                    sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Warning);
                    return;
                }
                ActionCommit?.Invoke(parameter as Window, MaterialMain);
                if (!ContAddMode)
                    (parameter as Window).Close();
            });
        }
    }
}
