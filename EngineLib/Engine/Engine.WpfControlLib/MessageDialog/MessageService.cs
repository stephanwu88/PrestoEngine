﻿using Engine.WpfTheme;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Engine.WpfControl
{
    public static class MessageService
    {
        #region - 提示消息 -

        public static void ShowSnackMessage(object message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    window.AddSnackMessage(message);
                }
            });
        }

        public static void ShowSnackMessageWithNotice(object message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    string info = $"{LanguageService.Instance.GetConfigByKey("Notice")}：[{DateTime.Now.ToString("HH:mm:ss")}] {message}";

                    window.AddSnackMessage(info);
                }
            });
        }

        /// <summary> 输出消息和操作按钮 </summary>
        public static void ShowSnackMessage(object message, object actionContent, Action actionHandler)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    window.AddSnackMessage(message, actionContent, actionHandler);
                }
            });

        }

        /// <summary> 输出消息、按钮和参数 </summary>
        public static void ShowSnackMessage<TArgument>(string message, object actionContent, Action<TArgument> actionHandler,
            TArgument actionArgument)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    window.AddSnackMessage(message, actionContent, actionHandler, actionArgument);
                }
            });

        }


        #endregion

        #region - 窗口消息 -

        public static async Task ShowWaittingMessge(Action action, Action closeAction = null)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
             {
                 if (CheckOpen()) return;

                 var view = new WaittingMessageDialog();

                 //show the dialog
                 await DialogHost.ShowWithOpen(view, "windowDialog", (l, e) =>
                 {
                     Task.Run(action).ContinueWith(m =>
                     {
                         Application.Current.Dispatcher.Invoke(() =>
                         {
                             e.Session.Close(false);

                             closeAction?.Invoke();
                         });
                     });
                 });

             });
        }

        /// <summary> 带有返回结果的等待消息窗口 </summary>
        public static async Task<T> ShowWaittingResultMessge<T>(Func<T> action)
        {
            if (CheckOpen() || action == null) return default(T);

            T result = default(T);

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var view = new WaittingMessageDialog();

                //show the dialog
                await DialogHost.ShowWithOpen(view, "windowDialog", (l, e) =>
                 {
                     Task.Run(() => result = action.Invoke()).ContinueWith(m =>
                          {
                              Application.Current.Dispatcher.Invoke(() =>
                              {
                                  e.Session.Close(false);
                              });
                          });
                 });
            });

            return result;
        }

        public static async Task ShowPercentProgress(Action<IPercentProgress> action, Action closeAction = null)
        {
            Func<PercentProgressDialog, object> func = l =>
            {
                action(l);
                return null;
            };

            await ShowProgressMessge(func, closeAction);
        }

        public static async Task<T> ShowPercentProgress<T>(Func<IPercentProgress, T> action, Action closeAction = null)
        {
            Func<PercentProgressDialog, T> func = l =>
            {
                return action(l);
            };

            return await ShowProgressMessge(func, closeAction);
        }

        /// <summary> 带有结果的进度消息 </summary>
        public static async Task<T> ShowStringProgress<T>(Func<IStringProgress, T> action, Action closeAction = null)
        {
            Func<StringProgressDialog, T> func = l =>
            {
                return action(l);
            };

            return await ShowProgressMessge(func, closeAction);
        }

        /// <summary> 进度消息 </summary>
        public static async Task ShowStringProgress(Action<IStringProgress> action, Action closeAction = null)
        {
            Func<StringProgressDialog, object> func = l =>
               {
                   action(l);
                   return null;
               };

            await ShowProgressMessge(func, closeAction);
        }

        public static async Task<R> ShowProgressMessge<T, R>(Func<T, R> action, Action closeAction = null) where T : new()
        {
            if (CheckOpen()) return default(R);

            R result = default(R);

            await Application.Current.Dispatcher.Invoke(async () =>
              {
                  var view = new T();

                  //show the dialog
                  await DialogHost.ShowWithOpen(view, "windowDialog", (l, e) =>
                    {
                        Task.Run(() => result = action.Invoke(view)).ContinueWith(m =>
                         {
                             Application.Current.Dispatcher.Invoke(() =>
                             {
                                 e.Session.Close(false);

                                 closeAction?.Invoke();
                             });
                         });
                    });

              });

            return result;
        }

        public static async Task ShowSumitMessge(string message, bool isLog = true)
        {
            if (CheckOpen()) return;

            await Application.Current.Dispatcher.Invoke(async () =>
             {
                 var view = new SampleMessageDialog();

                 view.MessageStr = message;

                 await DialogHost.Show(view, "windowDialog");
             });
        }

        /// <summary> 显示自定义窗口 </summary>
        public static async Task<T> ShowCustomDialog<T>(UIElement element, Action<object, DialogClosingEventArgs> action = null)
        {
            if (CheckOpen()) return default(T);

            T result = default(T);

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                //show the dialog
                result = (T)await DialogHost.ShowWithClose(element, "windowDialog", (l, e) =>
                  {
                      action?.Invoke(l, e);
                  });
            });

            return result;
        }

        static bool CheckOpen()
        {
            if (IsOpened())
            {
                string message = LanguageService.Instance.GetMessageByCode("M00002");

                ShowSnackMessageWithNotice(message);
                return true;
            }

            return false;
        }

        public static bool IsOpened()
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return DialogHost.IsOpened();
            });

        }

        public static async Task ShowResultMessge(string message, Action<object, DialogClosingEventArgs> action)
        {
            if (CheckOpen()) return;

            await Application.Current.Dispatcher.Invoke(async () =>
             {
                 var view = new ResultMessageDialog();

                 view.MessageStr = message;

                 //show the dialog
                 await DialogHost.ShowWithClose(view, "windowDialog", (l, e) =>
                 {
                     action?.Invoke(l, e);
                 });
             });

        }

        /// <summary> 检查显示主窗口模糊效果 </summary>
        public static void BeginDefaultBlurEffect(bool isuse)
        {
            if (isuse)
            {
                if (Application.Current.MainWindow is WindowBase window)
                {
                    window.AdornerDecoratorEffect = window.DefaultBlurEffect;
                }
            }
            else
            {
                if (Application.Current.MainWindow is WindowBase window)
                {
                    window.AdornerDecoratorEffect = null;
                }
            }
        }

        public static async Task<bool> ShowResultMessge(string message)
        {
            if (CheckOpen()) return false;

            bool result = false;

            Action<object, DialogClosingEventArgs> action = (l, k) =>
             {
                 result = (bool)k.Parameter;
             };

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var view = new ResultMessageDialog();

                view.MessageStr = message;

                //show the dialog
                await DialogHost.ShowWithClose(view, "windowDialog", (l, e) =>
                {
                    action?.Invoke(l, e);
                });
            });

            return result;
        }

        #endregion

        #region - 蒙版消息 -

        public static MessageCloseLayerCommand CloseLayerCommand { get; } = new MessageCloseLayerCommand();

        /// <summary> 显示蒙版 </summary>
        public static void ShowLayer(FrameworkElement element)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() =>
            {
                if (Application.Current.MainWindow is IWindowBase window)
                {
                    window.ShowLayer(element);
                }
            }));
        }

        /// <summary> 关闭蒙版 </summary>
        public static void CloseLayer()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new Action(() =>
            {
                if (Application.Current.MainWindow is IWindowBase window)
                {
                    window.CloseLayer();
                }
            }));
        }

        static ManualResetEvent _asyncShowWaitHandle = new ManualResetEvent(false);

        ///// <summary> 显示蒙版 </summary>
        //public static async Task<bool> ShowObjectWithPropertyForm<T>(T value, Predicate<T> match = null, string title = null, int layerIndex = 0)
        //{
        //    bool result = false;

        //    await Application.Current.Dispatcher.Invoke(async () =>
        //     {
        //         if (Application.Current.MainWindow is IWindowBase window)
        //         {
        //             PropertyGrid form = new PropertyGrid();

        //             form.Title = title;

        //             form.Style = Application.Current.FindResource("S.PropertyGrid.Default.WithSumit") as Style;

        //             form.SelectObject = value;

        //             form.Close += (l, k) =>
        //             {
        //                 CloseLayer();
        //                 _asyncShowWaitHandle.Set();
        //                 result = false;
        //             };

        //             form.Sumit += (l, k) =>
        //             {
        //                 var check = form.ModelState(out List<string> errors);

        //                 if (!check)
        //                 {
        //                     MessageService.ShowSnackMessageWithNotice(errors.FirstOrDefault());
        //                     return;
        //                 }

        //                 if (match != null && !match(value))
        //                 {
        //                     return;
        //                 }

        //                 CloseLayer();
        //                 _asyncShowWaitHandle.Set();
        //                 result = true;

        //             };

        //             window.ShowLayer(form);

        //             _asyncShowWaitHandle.Reset();

        //             var task = new Task(() =>
        //             {
        //                 _asyncShowWaitHandle.WaitOne();
        //             });

        //             task.Start();

        //             await task;
        //         }
        //     });

        //    return result;

        //}

        /// <summary>
        /// 显示蒙版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="match"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static async Task<bool> ShowObjectWithContent<T>(T value, Predicate<T> match = null, string title = null)

        {
            bool result = false;

            await Application.Current.Dispatcher.Invoke(async () =>
            {
                if (Application.Current.MainWindow is IWindowBase window)
                {
                    ObjectContentDialog content = new ObjectContentDialog();

                    content.Content = value;

                    content.Title = title;

                    //content.Style = Application.Current.FindResource("S.PropertyGrid.Default.WithSumit") as Style;

                    //content.SelectObject = value;

                    //content.Close += (l, k) =>
                    //{
                    //    CloseWithLayer();
                    //    _asyncShowWaitHandle.Set();
                    //    result = false;
                    //};

                    content.Sumited += (l, k) =>
                    {
                        if (match != null && !match(value)) return;

                        CloseLayer();
                        _asyncShowWaitHandle.Set();
                        result = true;
                    };

                    content.Closed += (l, k) =>
                   {
                       CloseLayer();
                       _asyncShowWaitHandle.Set();
                       result = false;
                   };

                    window.ShowLayer(content);

                    _asyncShowWaitHandle.Reset();

                    var task = new Task(() =>
                    {
                        _asyncShowWaitHandle.WaitOne();
                    });

                    task.Start();

                    await task;
                }
            });

            return result;

        }

        #endregion

        #region - 气泡消息 -

        /// <summary> 显示气泡消息 </summary>
        public static void ShowNotifyMessage(string message, string title = null, NotifyBalloonIcon tipIcon = NotifyBalloonIcon.Info, int timeout = 1000)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    window.ShowNotifyMessage(title, message, tipIcon, timeout);
                }
            });

        }

        /// <summary> 显示自定义气泡消息 </summary>
        public static void ShowNotifyDialogMessage(string message, string title = null, int closeTime = -1)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NotifyDialogWindow.ShowMessage(message, title, closeTime);
            });
        }

        #endregion

        #region - 列表消息 -

        static NotifyMessageWindow _notifyMessage;

        /// <summary> 显示自定义气泡消息 </summary>
        public static void ShowSysMessage(MessageBase message)
        {
            if (_notifyMessage == null)
            {
                _notifyMessage = new NotifyMessageWindow();

                Application.Current.MainWindow.Closed += (l, k) =>
              {
                  _notifyMessage.Close();
              };

                _notifyMessage.Show();

            }

            _notifyMessage.Add(message);
        }

        /// <summary> 输出消息、按钮和参数 </summary>
        public static void ShowWinMessage(MessageBase message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IWindowBase window = Application.Current.MainWindow as IWindowBase;

                if (window != null)
                {
                    window.ShowWindowNotifyMessage(message);
                }
            });
        }

        public static void ShowSysSuccessMessage(string message)
        {
            ShowSysMessage(new SuccessMessage
            {
                Message = message
            });
        }

        public static void ShowSysInfoMessage(string message)
        {
            ShowSysMessage(new InfoMessage
            {
                Message = message
            });
        }

        public static void ShowSysErrorMessage(string message)
        {
            ShowSysMessage(new ErrorMessage
            {
                Message = message
            });
        }

        public static void ShowSysWarnMessage(string message)
        {
            ShowSysMessage(new WarnMessage
            {
                Message = message
            });
        }

        public static void ShowSysFatalMessage(string message)
        {
            ShowSysMessage(new FatalMessage
            {
                Message = message
            });
        }

        /// <summary> 显示等待效果的 </summary>
        public static async Task<T> ShowSysWaittingMessage<T>(Func<WaittingMessage, T> action)
        {
            var control = new WaittingMessage();

            ShowSysMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        /// <summary> 显示带进度的消息 </summary>
        public static async Task<T> ShowSysProgressBarMessage<T>(Func<PercentProgressMessage, T> action)
        {
            var control = new PercentProgressMessage();

            ShowSysMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        /// <summary> 显示带进度的消息 </summary>
        public static async Task<T> ShowSysProgressStrMessage<T>(Func<StringProgressMessage, T> action)
        {
            var control = new StringProgressMessage();

            ShowSysMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        public static void ShowWinSuccessMessage(string message)
        {
            ShowWinMessage(new SuccessMessage
            {
                Message = message
            });
        }

        public static void ShowWinInfoMessage(string message)
        {
            ShowWinMessage(new InfoMessage
            {
                Message = message
            });
        }

        /// <summary> 显示带进度的消息 </summary>
        public static async Task<T> ShowWinProgressStrMessage<T>(Func<StringProgressMessage, T> action)
        {
            var control = new StringProgressMessage();

            ShowWinMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        /// <summary> 显示等待效果的 </summary>
        public static async Task<T> ShowWinWaittingMessage<T>(Func<WaittingMessage, T> action)
        {
            var control = new WaittingMessage();

            ShowWinMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        /// <summary> 显示带进度的消息 </summary>
        public static async Task<T> ShowWinProgressBarMessage<T>(Func<PercentProgressMessage, T> action)
        {
            var control = new PercentProgressMessage();

            ShowWinMessage(control);

            var result = await Task<T>.Run(() => action.Invoke(control));

            control.Close();

            return result;
        }

        public static void ShowWinErrorMessage(string message)
        {
            ShowWinMessage(new ErrorMessage
            {
                Message = message
            });
        }

        public static void ShowWinWarnMessage(string message)
        {
            ShowWinMessage(new WarnMessage
            {
                Message = message
            });
        }

        public static void ShowWinFatalMessage(string message)
        {
            ShowWinMessage(new FatalMessage
            {
                Message = message
            });
        }

        #endregion

        #region - 任务图标消息 -

        /// <summary> 调用主窗口任务栏缩略图 </summary>
        public static void ShowTaskbar(Action<TaskbarItemInfo> action)
        {
            var bar = Application.Current.MainWindow.TaskbarItemInfo;

            if (bar == null) return;

            bar.ProgressValue = 1;

            action?.Invoke(bar);
        }

        /// <summary> 调用主窗口任务栏缩略图 显示任务完成百分比</summary>
        public static void ShowTaskbarNormal(Action<TaskbarItemInfo> action)
        {
            var bar = Application.Current.MainWindow.TaskbarItemInfo;

            if (bar == null) return;

            bar.ProgressState = TaskbarItemProgressState.Normal;

            action?.Invoke(bar);
        }

        /// <summary> 调用主窗口任务栏缩略图 显示任务完成百分比</summary>
        public static async Task ShowTaskbarPercent(Action<TaskbarItemInfo> action)
        {
            var bar = Application.Current.MainWindow.TaskbarItemInfo;

            if (bar == null) return;

            bar.ProgressState = TaskbarItemProgressState.Normal;

            await Task.Run(() =>
            {
                var t = Application.Current.Dispatcher.Invoke(() =>
                {
                    return Application.Current.MainWindow.TaskbarItemInfo;
                });

                action?.Invoke(t);
            });

            //bar.ProgressValue = 0;

            bar.ProgressState = TaskbarItemProgressState.None;
        }

        /// <summary> 调用主窗口任务栏缩略图 显示等待效果 </summary>
        public static async Task<bool> ShowTaskbarWaitting(Func<bool> action)
        {
            var bar = Application.Current.MainWindow.TaskbarItemInfo;

            if (bar == null) return false;
            if (action == null) return false;

            bar.ProgressState = TaskbarItemProgressState.Indeterminate;

            var result = await Task.Run(() => action.Invoke());

            bar.ProgressValue = 1;

            if (result)
            {
                bar.ProgressState = TaskbarItemProgressState.Normal;
            }
            else
            {
                bar.ProgressState = TaskbarItemProgressState.Error;
            }

            Thread.Sleep(1000);

            bar.ProgressState = TaskbarItemProgressState.None;

            return result;
        }


        /// <summary> 调用主窗口任务栏缩略图 </summary>
        public static void ShowTaskbarImage(ImageSource image)
        {
            var bar = Application.Current.MainWindow.TaskbarItemInfo;

            if (bar == null) return;

            bar.Overlay = image;
        }

        #endregion

    }

    public class MessageCloseLayerCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MessageService.CloseLayer();
        }

        public event EventHandler CanExecuteChanged;
    }
}
