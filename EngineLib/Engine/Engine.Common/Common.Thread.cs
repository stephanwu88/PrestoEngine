using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Engine.Common
{
    /// <summary>
    /// 线程操作
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 设置线程池线程数量
        /// </summary>
        /// <param name="minThreads">维持最小线程数</param>
        /// <param name="maxThreads">最大线程数，超过的都排队</param>
        public static void ThreadPoolSet(int minThreads = 200, int maxThreads = 1000)
        {
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            ThreadPool.SetMinThreads(minThreads, minThreads);
        }
    }

    /// <summary>
    /// 信号数据
    /// </summary>
    public class SignalData
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Message { get; set; } 
        public bool Error { get; set; } 
    }

    /// <summary>
    /// 信号量数据结构
    /// </summary>
    public class SignalStruct
    {
        public EventWaitHandle Handle;
        public SignalData Data;
    }

    /// <summary>
    /// 线程门控器
    /// 用于在线程中等待信号
    /// </summary>
    public class ThreadSignalGate
    {
        /// <summary>
        /// 信号量列表
        /// </summary>
        public Dictionary<string, SignalStruct> DicSignal;

        public static ThreadLocal<Dictionary<string, SignalStruct>> ThreadDicSignal = new ThreadLocal<Dictionary<string, SignalStruct>>(() =>
           {
               return new Dictionary<string, SignalStruct>();
           });

        /// <summary>
        /// 添加到对应线程字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddToDict(string key, SignalStruct value)
        {
            Dictionary<string, SignalStruct> dict = ThreadDicSignal.Value;
            dict.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void PrintDict()
        {
            Dictionary<string, SignalStruct> dict = ThreadDicSignal.Value;
            foreach (SignalStruct ss in dict.Values)
            {
                Console.WriteLine($"Token: {ss.Data?.Token}");
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreadSignalGate()
        {
            DicSignal = ThreadDicSignal.Value;
        }

        private SignalStruct NewSignal
        {
            get => new SignalStruct()
            {
                Handle = new AutoResetEvent(false),
                Data = default(SignalData)
            };
        }

        /// <summary>
        /// 发送信号
        /// </summary>
        public void SendSignal()
        {
            
        }

        /// <summary>
        /// 发送信号
        /// </summary>
        public void SendSignal(string sendToken)
        {
            DicSignal.DictFieldValue(sendToken)?.Handle?.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sendSignal"></param>

        public void SendSignal(SignalData sendSignal)
        {
            SignalStruct signal = DicSignal.DictFieldValue(sendSignal?.Token);
            if (signal?.Handle != default(EventWaitHandle))
            {
                signal?.Handle.Set();
                signal.Data = sendSignal;
            }
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool WaitSignal(int millisecondsTimeout)
        {
            return false;
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        /// <param name="waitToken"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public SignalData WaitSignal(string waitToken, int millisecondsTimeout)
        {
            DicSignal.AppandDict(waitToken, NewSignal);
            bool WaitSuceess = DicSignal.DictFieldValue(waitToken).Handle.WaitOne(millisecondsTimeout);
            SignalData data = new SignalData();
            if (WaitSuceess)
            {
                data = DicSignal.DictFieldValue(waitToken).Data;
            }
            else
            { 
                data = new SignalData() { Token = waitToken, Error = true, Message = "超时" };
            }
            DicSignal.Remove(waitToken);
            return data;
        }

        /// <summary>
        /// 等待信号
        /// </summary>
        /// <param name="waitSignal"></param>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public SignalData WaitSignal(SignalData waitSignal, int millisecondsTimeout)
        {
            DicSignal.AppandDict(waitSignal?.Token, NewSignal);
            bool ret =  DicSignal.DictFieldValue(waitSignal?.Token).Handle.WaitOne(millisecondsTimeout);
            if (!ret)
            {
                return new SignalData() { Token = waitSignal?.Token, Error = true, Message = "超时" };
            }
            return DicSignal.DictFieldValue(waitSignal.Token)?.Data;
        }

        #region 异步方法

        ///// <summary>
        ///// 异步发送信号
        ///// </summary>
        ///// <returns></returns>
        //public async Task AsyncSendSignal()
        //{
        //    await Task.Run(() =>
        //    {
        //        signal.Set();
        //    }).ConfigureAwait(true);
        //}

        ///// <summary>
        ///// 异步发送信号
        ///// </summary>
        ///// <param name="sendToken"></param>
        ///// <returns></returns>
        //public async Task AsyncSendSignal(string sendToken)
        //{
        //    await Task.Run(() =>
        //    {
        //        if (sendToken == EventToken)
        //            signal.Set();
        //    }).ConfigureAwait(true);
        //}

        ///// <summary>
        ///// 异步发送信号
        ///// </summary>
        ///// <param name="sendToken"></param>
        ///// <returns></returns>
        //public async Task AsyncSendSignal(SignalData sendSignal)
        //{
        //    await Task.Run(() =>
        //    {
        //        if (sendSignal?.Token == EventSignal?.Token)
        //            signal.Set();
        //    }).ConfigureAwait(true);
        //}

        ///// <summary>
        ///// 异步等待信号
        ///// </summary>
        ///// <param name="millisecondsTimeout"></param>
        ///// <returns></returns>
        //public async Task<bool> AsyncWaitSignal(int millisecondsTimeout)
        //{
        //    // 在后台线程等待信号
        //    return await Task.Run(() =>
        //    {
        //        // 等待信号，直到收到信号才继续进行
        //        bool isSignalReceived = signal.WaitOne(millisecondsTimeout);
        //        return isSignalReceived;
        //    }).ConfigureAwait(true);
        //}

        ///// <summary>
        ///// 异步等待信号
        ///// </summary>
        ///// <param name="waitToken"></param>
        ///// <param name="millisecondsTimeout"></param>
        ///// <returns></returns>
        //public async Task<bool> AsyncWaitSignal(string waitToken, int millisecondsTimeout)
        //{
        //    // 在后台线程等待信号
        //    return await Task.Run(() =>
        //    {
        //        lock (lockObject)
        //        {
        //            EventToken = waitToken;
        //            // 等待指定信号，直到收到信号才继续进行
        //            bool isSignalReceived = signal.WaitOne(millisecondsTimeout);
        //            EventToken = string.Empty;
        //            return isSignalReceived;
        //        }
        //    }).ConfigureAwait(true);
        //}

        ///// <summary>
        ///// 异步等待信号
        ///// </summary>
        ///// <param name="waitSignal"></param>
        ///// <param name="millisecondsTimeout"></param>
        ///// <returns></returns>
        //public async Task<bool> AsyncWaitSignal(SignalData waitSignal, int millisecondsTimeout)
        //{
        //    // 在后台线程等待信号
        //    return await Task.Run(() =>
        //    {
        //        lock (lockObject)
        //        {
        //            EventSignal = waitSignal;
        //            // 等待指定信号，直到收到信号才继续进行
        //            bool isSignalReceived = signal.WaitOne(millisecondsTimeout);
        //            EventSignal = null;
        //            return isSignalReceived;
        //        }
        //    }).ConfigureAwait(true);
        //}
        #endregion

    }

    /// <summary>
    /// 异步锁
    /// </summary>
    public class AsyncLock : IDisposable
    {
        private SemaphoreSlim semaphore;
        public AsyncLock()
        {
            semaphore = new SemaphoreSlim(1); 
        }

        public async Task<IDisposable> LockAsync()
        {
            return await LockAsync(CancellationToken.None);
        }

        public async Task<IDisposable> LockAsync(CancellationToken cancellation)
        {
            await semaphore.WaitAsync(cancellation);
            return new LockReleaser(semaphore);
        }

        /// <summary>
        /// 处于上下文锁定
        /// </summary>
        public bool IsLocked { get => semaphore?.CurrentCount == 0; }

        /// <summary>
        /// 锁释放状态
        /// </summary>
        public bool IsReleased { get => semaphore?.CurrentCount != 0; }

        /// <summary>
        /// 释放锁
        /// </summary>
        public void Release()
        {
            semaphore?.Release();
        }

        public void Dispose()
        {
            semaphore?.Release();
            semaphore?.Dispose();
            semaphore = null;
        }

        private class LockReleaser : IDisposable
        {
            private readonly SemaphoreSlim semaphore;
            public LockReleaser(SemaphoreSlim semaphore)
            {
                this.semaphore = semaphore;
            }

            public void Dispose()
            {
                semaphore?.Release();
            }
        }
    }
}
