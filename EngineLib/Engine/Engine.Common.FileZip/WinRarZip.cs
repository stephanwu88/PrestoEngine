using Microsoft.Win32;
using System;
using System.IO;

namespace Engine.Common
{
    /// <summary>
    /// WinRAR文件压缩和解压
    /// 支持rar和zip文件操作
    /// 使用前将WinRAR.exe程序置于执行程序目录下以供调用
    /// </summary>
    public sealed class WinRarZip
    {
        #region 内部变量
        private static bool _IsSetUp;
        private static string _RarExe;
        #endregion

        /// <summary>
        /// 压缩文件/文件夹
        /// </summary>
        /// <param name="filePath">需要压缩的文件/文件夹路径</param>
        /// <param name="zipFile">压缩文件</param>
        /// <param name="password">密码</param>
        /// <param name="filterExtenList">需要过滤的文件后缀名</param>
        public static bool Compression(string filePath, string zipFile, string password = "")
        {
            #region Rar命令行指令
            //RAR 5.50 x64 版权所有(C) 1993 - 2017 Alexander Roshal    11 八月 2017
            //试用版 输入 'rar -?' 获得帮助
            //用法:     rar < command > -<switch 1 > -<switch N > < archive > < files...>
            //      < @listfiles...> < path_to_extract\>

            //  < Commands >
            //  a             添加文件到压缩文档
            //  c             添加添加压缩文档注释
            //  ch           更改压缩文档参数
            //  cw            写入压缩文档注释到文件
            //  d             从压缩文档删除文件
            //  e             提取文件不带压缩路径
            //  f             刷新压缩文档中的文件
            //  i[par] =< str > 在压缩文档里查找字符串
            //  k 锁定压缩文档
            //  l[t[a], b]     列出压缩文档内容[technical[all], bare]
            //  m[f]         移动到压缩文档[仅文件]
            //  p 打印文件到 stdout
            //  r             修复压缩文档
            //  rc            重新构建丢失的卷
            //  rn            重命名归档的文件
            //  rr[N]         添加数据恢复记录
            //  rv[N]         创建恢复卷
            //  s[name | -]     转换压缩文档到或从 SFX
            //  t 测试压缩文档的文件
            //  u 更新压缩文档中的文件
            //  v[t[a], b]     详细列出压缩文档的内容[technical[all], bare]
            //  x 解压文件带完整路径

            //  <Switches>
            //  -停止参数扫描
            //  @[+]
            // 禁用[enable] 文件列表
            //  ac 压缩或解压后清除压缩文档属性
            //  ad 扩展压缩文档名称到目标路径
            //  ag[format] 使用当前日期生成压缩文档名称
            //  ai 忽略文件属性
            //  ao 添加文件带有压缩文档属性集
            //  ap<path> 设置压缩文档内部的路径
            //  as            同步压缩文档内容
            //  c-            禁用内容显示
            //  cfg-          禁用读取配置
            //  cl            转换名称为小写
            //  cu            转换名称为大写
            //  df            压缩后删除文件
            //  dh            打开共享的文件
            //  dr           删除文件到回收站
            //  ds            为固实压缩禁用名称排序
            //  dw            压缩后删除文件
            //  e[+]<attr>    设置文件排除和包含属性
            //  ed            不要添加空目录
            //  en            不要放置 'end of archive' 块
            //  ep            从名称里排除路径
            //  ep1           从名称里排除根目录
            //  ep2           扩展路径为完整路径
            //  ep3          扩展路径为完整路径包括驱动器盘符
            //  f             刷新文件
            //  hp[password]  加密文件数据及文件头
            //  ht[b | c]       设置哈希类型[BLAKE2, CRC32] 用于文件校验和
            //  id[c, d, p, q] 禁用消息
            //  ieml[addr] 通过电邮发送压缩文档
            //  ierr 发送所有压缩文档到 stderr
            //  ilog[name]    记录错误到日志文件(仅适用于已注册版本)
            //  inul 禁用所有消息
            //  ioff 完成操作后关闭 PC
            //  isnd          禁用声音
            //  iver          仅显示版本号
            //  k             锁定压缩文档
            //  kb            保留损坏的已解压文件
            //  log[f][=name]
            //        将名称写入日志文件
            //  m<0..5>       设置压缩等级 (0-store...3-default...5-maximal)
            //  ma[4 | 5] 指定压缩格式的版本
            //  mc<par> 设置高级压缩参数
            //  md<n>[k, m, g] 词典大小单位为 KB, MB 或 GB
            //  ms[ext; ext]   指定要存储的文件类型
            //  mt<threads>   设置线程数
            //  n<file>       额外管理器包含文件
            //  n@            从 stdin 读取额外的过滤器掩码
            //  n@<list>      从列表文件读取额外的过滤器掩码
            //  o[+|-]        设置覆盖模式
            //  oc            设置 NTFS 压缩属性
            //  oh            保存硬链接为链接而不是文件
            //  oi[0-4][:min] 将相同的文件保存为参考
            //  ol[a]         将符号链接处理为链接 [absolute paths]
            //  oni           允许潜在的不兼容名称
            //  or            自动重命名文件
            //  os            保存 NTFS 流
            //  ow            保存或恢复文件拥有者和组
            //  p[password]   设置密码
            //  p-            不要查询密码
            //  qo[-|+]       添加快速打开信息 [none|force]
            //  r             递归子目录
            //  r-            禁用递归
            //  r0            递归子目录仅用于通配符名称
            //  ri<P>[:<S>]   设置优先级 (0-默认,1-最小.15-最大) 和休眠时间单位为 ms
            //  rr[N] 添加数据恢复记录
            //  rv[N] 创建恢复卷
            //  s[< N >, v[-], e] 创建固实压缩文档
            //  s-            禁用固实压缩文档
            //  sc<chr>[obj]  指定字符集
            //  sfx[name]     创建 SFX 压缩文档
            //  si[name]      从标准输入读取数据 (stdin)
            //  sl<size>      处理小于指定大小的文件
            //  sm<size>      处理大于指定大小的文件
            //  t             压缩后测试
            //  ta<date>     处理在<date> 之后修改的文件日期格式为 YYYYMMDDHHMMSS
            //  tb<date>      处理在<date> 之前修改的文件日期格式为 YYYYMMDDHHMMSS
            //  tk            保留原来的压缩时间
            //  tl            设置压缩时间为最近的文件
            //  tn<time>      处理比<time> 更新的文件
            //  to<time> 处理比 <time> 更老的文件
            //  ts[m|c|a]     复制或恢复文件时间（修改，创建，访问）
            //  u 更新文件
            //  v<size>[k, b] 创建卷大小为 =< size > *1000[*1024, *1]
            //  vd 创建卷之前删除磁盘内容
            //  ver[n] 文件版本控制
            //  vn 使用旧式的卷命名规则
            //  vp 每个卷之前暂停
            //  w<path> 指定工作目录
            //  x<file> 排除特定文件
            //  x@           读取文件名以便从 stdin 排除
            //  x@<list>      排除在特定列表文件里列出的文件
            //  y             对所有问题回答是
            //  z[file]       从文件读取压缩文档注释

            #endregion
            if (!IsSetUp)
                throw new Exception("未安装WinRAR压缩工具");
            string args = string.Empty;
            string strPasswd = password.Length > 0 ? string.Format("-p{0}", password) : "";
            string strExtension = Path.GetExtension(zipFile).ToLower(); //.zip .rar
            switch (strExtension)
            {
                case ".zip":
                    args = string.Format("a {0} -as -r -afzip -ed -ibck -inul -m3 -mt5 -ep1 {1} {2}", strPasswd, zipFile, filePath);
                    break;
                case ".rar":
                default:
                    args = string.Format("a {0} -as -r -afrar -ed -ibck -inul -m3 -mt5 -ep1 {1} {2}", strPasswd, zipFile, filePath);
                    break;
            }
            return StartProcess(_RarExe, args);
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="CompressFileName">压缩文件</param>
        /// <param name="DeCompressFileName">解压缩文件</param>
        /// <returns></returns>
        public static bool DeCompression(string CompressFileName, string DeCompressFileName, string Password = "")
        {
            if (!IsSetUp)
                throw new Exception("未安装WinRAR压缩工具");
            if (!Directory.Exists(DeCompressFileName))
            {
                Directory.CreateDirectory(DeCompressFileName);
            }
            string strPasswd = Password.Length > 0 ? string.Format("-p{0}", Password) : "";
            string args = string.Format("x {0} -ibck -inul -y -mt5 {1} {2}", strPasswd, CompressFileName, DeCompressFileName);
            return StartProcess(_RarExe, args);
        }

        /// <summary>
        /// WinRAR压缩安装与否
        /// </summary>
        public static bool IsSetUp
        {
            get
            {
                CheckWinRarReg();
                return _IsSetUp;
            }
        }

        /// <summary>
        /// WinRAR压缩运行文件
        /// </summary>
        public static string RarExe
        {
            get
            {
                CheckWinRarReg();
                return _RarExe;
            }
        }

        #region 内部方法
        /// <summary>
        /// 检查注册表
        /// </summary>
        private static void CheckWinRarReg()
        {
            if (string.IsNullOrEmpty(_RarExe) || !File.Exists(_RarExe))
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                string strFilePath = key.GetValue(string.Empty).ToString();
                if (string.IsNullOrEmpty(strFilePath))
                {
                    string strTryPath = Environment.CurrentDirectory + @"\WinRAR.exe";
                    if (File.Exists(strTryPath))
                        strFilePath = strTryPath;
                }
                _RarExe = strFilePath;
                _IsSetUp = !string.IsNullOrEmpty(strFilePath);
            }
        }

        /// <summary>
        /// 启动进程执行exe
        /// </summary>
        /// <param name="ExePath">进程执行文件</param>
        /// <param name="Parameters">Shell指令命令行参数</param>
        /// <param name="IsHiddeen">执行过程是否窗口化显示</param>
        /// <param name="TimeOutSec">进程监管超时时间S</param>
        /// <returns></returns>
        private static bool StartProcess(string ExePath, string Parameters, bool IsHiddeen = false, int TimeOutSec = 0)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            if (IsHiddeen)
            {
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
            }
            else
            {
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                psi.CreateNoWindow = false;
            }
            psi.FileName = ExePath;
            psi.Arguments = Parameters;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);

            //System.IO.StreamReader outputStreamReader = process.StandardOutput;            
            //System.IO.StreamReader errStreamReader = process.StandardError;             
            int i = 0;
            while (!process.HasExited)
            {
                process.WaitForExit(1000);
                if (i >= TimeOutSec && TimeOutSec > 0)
                {
                    process.Kill();
                    return false;
                }
                i++;
            }
            process.Close();
            process.Dispose();
            return true;
        }
        #endregion
    }
}
