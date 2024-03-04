using Engine.Common;
using Engine.MVVM;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Engine.Automation.Sparker
{
    /// <summary>
    /// 配置分析程序
    /// </summary>
    public class ViewModelConfigPgm : ViewFrameBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Mode"></param>
        public ViewModelConfigPgm(EditMode Mode)
        {
            editMode = Mode;
        }

        /// <summary>
        /// 基体列表
        /// </summary>
        public List<string> MatrixList
        {
            get { return _MatrixList; }
            set { _MatrixList = value; RaisePropertyChanged(); }
        }
        private List<string> _MatrixList;

        /// <summary>
        /// 分析程序列表
        /// </summary>
        public List<string> PgmNameList
        {
            get { return _PgmNameList; }
            set { _PgmNameList = value; RaisePropertyChanged(); }
        }
        private List<string> _PgmNameList;

        /// <summary>
        /// 分析程序实体
        /// </summary>
        public ModelSpecPgm AnaPgm
        {
            get { return _AnaPgm; }
            set { _AnaPgm = value; RaisePropertyChanged(); }
        }
        private ModelSpecPgm _AnaPgm = new ModelSpecPgm();

        /// <summary>
        /// 确认
        /// </summary>
        public ICommand CommandSure
        {
            get => new MyCommand((parameter) =>
            {
                if (parameter == null) return;
                CallResult res = AnaPgm.Validate();
                if (res.Fail)
                {
                    sCommon.MyMsgBox(res.Result.ToMyString(), MsgType.Error);
                    return;
                }
                if (ActionCommit != null)
                {
                    if (editMode == EditMode.AddNew)
                        AnaPgm.Token = SystemDefault.UUID;
                    ActionCommit(parameter as Window, AnaPgm);
                }
                if(!ContAddMode)
                    (parameter as Window).Close();
            });
        }
    }
}
