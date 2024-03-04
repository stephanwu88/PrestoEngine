using Engine.Data;
using Engine.MVVM;
using System;

namespace Engine.WpfControl
{
   public abstract class WindowBaseNotifyPropertyChanged : NotifyPropertyChanged
    {
        public MyCommand<string> LoadedCommand => new Lazy<MyCommand<string>>(() => new MyCommand<string>(Loaded, CanLoaded)).Value;

        protected virtual void Loaded(string args)
        {

        }

        protected virtual bool CanLoaded(string args)
        {
            return true;
        }
    }
}
