﻿using Engine.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Engine.ComDriver
{
    /// <summary>
    /// WebApi通讯服务
    /// </summary>
    public class WebReqCaller
    {
        /// <summary>
        /// HttpPost请求,发送实体 返回实体
        /// </summary>
        /// <typeparam name="TPost"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="Url"></param>
        /// <param name="PostMessage"></param>
        /// <param name="strSend"></param>
        /// <param name="strResponse"></param>
        /// <param name="PostSuccess"></param>
        /// <returns></returns>
        public static CallResult HttpPost<TPost, TResponse>(string Url, TPost PostMessage,out string strSend,out string strResponse,out bool PostSuccess)
        {
            return HttpPost<TPost, TResponse>(Url, PostMessage, "", "", out strSend, out strResponse, out PostSuccess);
        }

        /// <summary>
        /// HttpPost请求,发送实体 返回实体
        /// </summary>
        /// <typeparam name="TPost"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="Url"></param>
        /// <param name="PostMessage"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="strSend"></param>
        /// <param name="strResponse"></param>
        /// <param name="PostSuccess"></param>
        /// <returns></returns>
        public static CallResult HttpPost<TPost, TResponse>(string Url, TPost PostMessage, string UserName, string Password, out string strSend, out string strResponse, out bool PostSuccess)
        {
            PostSuccess = false;
            strSend = string.Empty;
            strResponse = string.Empty;
            try
            {
                CallResult res = ((object)PostMessage).MyJsonSerialize();
                if (res.Fail)
                {
                    res.Result = $"发送数据时序列字符串失败: {res.Result.ToMyString()}";
                    return res;
                }
                strSend = res.Result.ToMyString();
                strResponse = HttpPost(Url, strSend, UserName, Password, ref PostSuccess);
                res = strResponse.MyJsonDeserialize<TResponse>();
                if (res.Fail)
                {
                    res.Result = $"发送数据时返回字符串:{strResponse} \r\n返回报文序列字符串失败: {res.Result.ToMyString()}";
                    return res;
                }
                TResponse response = (TResponse)res.Result;
                return new CallResult() { Success = true, Result = response };
            }
            catch (Exception ex)
            {
                strSend = string.Empty;
                strResponse = string.Empty;
                return new CallResult() { Result = $"发送信息时遇到程序错误：{ex.Message}" };
            }
        }

        /// <summary>
        /// HttpPost请求,发送Json字符串
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, ref bool isSuccess)
        {
            return HttpPost(Url, postDataStr, "", "", ref isSuccess);
        }

        /// <summary>
        /// HttpPost请求,发送Json字符串
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="isSuccess"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, string UserName, string Password, ref bool isSuccess)
        {
            try
            {
                //创建一个HttpWeb请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json";
                //2024-01-26 在长钢和太原易思交互式 由于指定了request.ContentLength 返回报错 请求被中止: 请求已被取消。  
                //故而取消
                //request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                if (!string.IsNullOrEmpty(UserName))
                {
                    //CredentialCache credentialCache = new CredentialCache();
                    //credentialCache.Add(new Uri(baseUrl), "Basic", new NetworkCredential(Username, Password));
                    //request.Credentials = credentialCache;
                    string authorization = $"{UserName}:{Password}";
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization)));
                    //request.Headers["Authorization"] = " Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization));
                }
                //request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                //StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                //创建一个HttpWeb响应
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                isSuccess = true;
                return retString;
            }
            catch (Exception e)
            {
                isSuccess = false;
                Console.Write(e.Message);
                return e.Message;
            }
        }

        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        //public static HttpWebResponse CreateGetHttpResponse(string url, int timeout, string userAgent, CookieCollection cookies)
        public static HttpWebResponse CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout;
            //if (cookies != null)
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(cookies);
            //}
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        //public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout, string userAgent, CookieCollection cookies)
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = null;
            //若是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/json";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            //if (cookies != null)
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(cookies);
            //}
            //发送POST数据  
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
