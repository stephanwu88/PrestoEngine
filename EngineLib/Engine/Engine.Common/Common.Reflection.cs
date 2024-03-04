using Engine.Data.DBFAC;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Engine.Common
{
    /// <summary>
    /// 反射 - 操作类/实例  动态实例化类
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 从本程序集创建实例
        /// </summary>
        /// <param name="TypeName">命名空间.类名</param>
        /// <param name="args">构造参数</param>
        /// <returns></returns>
        public static object CreateInstance(string TypeName, object[] args = null)
        {
            try
            {
                return CreateInstance("", TypeName, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 指定程序集创建实例
        /// </summary>
        /// <param name="AssemblyName">程序集名</param>
        /// <param name="TypeName">命名空间.类名</param>
        /// <param name="args">构造参数</param>
        /// <returns></returns>
        public static object CreateInstance(string AssemblyName, string TypeName, object[] args = null)
        {
            try
            {
                Assembly assem = default(Assembly);
                if (string.IsNullOrEmpty(AssemblyName))
                {
                    assem = MyAssembly.Default.AssemblyInfo;
                }
                else
                {
                    assem = Assembly.Load(AssemblyName);
                }
                return CreateInstance(assem, TypeName, args);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 指定程序集创建实例 - 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AssemblyName"></param>
        /// <param name="TypeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(string AssemblyName, string TypeName, object[] args = null)
        {
            object objInstance = CreateInstance(AssemblyName, TypeName, args);
            if (objInstance == null)
                return default(T);
            return (T)objInstance;
        }

        /// <summary>
        /// 指定程序集创建实例 - 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AssemblyName"></param>
        /// <param name="TypeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(this MyAssembly MyAssembly, string TypeName, object[] args = null)
        {
            object objInstance = CreateInstance(MyAssembly.AssemblyInfo, TypeName, args);
            if (objInstance == null)
                return default(T);
            return (T)objInstance;
        }

        /// <summary>
        /// 指定程序集创建实例 - 泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AssemblyName"></param>
        /// <param name="TypeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(this Assembly assembly, string TypeName, object[] args = null)
        {
            object objInstance = CreateInstance(assembly, TypeName, args);
            if (objInstance == null)
                return default(T);
            return (T)objInstance;
        }

        /// <summary>
        /// 指定程序集创建实例
        /// </summary>
        /// <param name="AssemblyName">程序集名</param>
        /// <param name="TypeName">命名空间.类名</param>
        /// <param name="args">构造参数</param>
        /// <returns></returns>
        public static object CreateInstance(this Assembly assembly, string TypeName, object[] args = null)
        {
            try
            {
                //获取Type对象
                Type type = assembly.GetType(TypeName);
                //根据对象创建实例
                object objInstance = Activator.CreateInstance(type, args);
                return objInstance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public static Type CreateType(this string TypeName)
        {
            try
            {
                Assembly assem = MyAssembly.Default.AssemblyInfo;
                Type type = assem.GetType(TypeName);
                return type;
            }
            catch (Exception ex)
            {
                return null;                
            }
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <param name="assemblyeName"></param>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public static Type CreateType(this string assemblyeName, string TypeName)
        {
            try
            {
                Assembly assem = Assembly.Load(assemblyeName);
                Type type = assem.GetType(TypeName);
                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据类型创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="TypeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateInstance(this Type type, object[] args = null)
        {
            try
            {
                //根据类型创建实例
                object objInstance = Activator.CreateInstance(type, args);
                return objInstance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 反射 - 操作属性
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取实体属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static List<string> GetPropNames<T>(this T data)
        {
            List<string> PropNames = new List<string>();
            Type ObjType = typeof(T);
            foreach (PropertyInfo pi in ObjType.GetProperties())
                PropNames.Add(pi.Name);
            return PropNames;
        }

        /// <summary>
        /// 获取实体属性名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool ContainsPropName<T>(this T data, string PropName)
        {
            List<string> PropNames = new List<string>();
            Type ObjType = typeof(T);
            foreach (PropertyInfo pi in ObjType.GetProperties())
                PropNames.Add(pi.Name);
            return PropNames.Contains(PropName);
        }

        /// <summary>
        /// 获取实体属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static List<string> GetPropValue<T>(this T data)
        {
            List<string> PropValues = new List<string>();
            Type ObjType = typeof(T);
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                if (pi.Name == "Item" || pi.Name == "Error")
                    continue;
                if (pi.PropertyType == typeof(double))
                {
                    double dVal = pi.GetValue(data, null).ToMyDouble();
                    if (dVal != SystemDefault.InValidDouble)
                        PropValues.Add(dVal.ToMyString());
                }
                else if (pi.PropertyType == typeof(string))
                    PropValues.Add(pi.GetValue(data, null).ToMyString());
                else
                    PropValues.Add(pi.GetValue(data, null).ToMyString());
            }
            return PropValues;
        }

        /// <summary>
        /// 获取实体属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pi"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object GetPropValue<T>(this PropertyInfo pi, T data)
        {
            try
            {
                //遇到Item属性时，GetValue原生方法会异常
                object obj = pi.GetValue(data, null);
                return obj;
            }
            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// 获取实体属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropValue<T>(this T data, string propName)
        {
            object ret = default(object);
            Type ObjType = typeof(T);
            foreach (PropertyInfo pi in ObjType.GetProperties())
            {
                //object objValue = pi.GetValue(data, null);
                //if (objValue == null)
                //    continue;
                //if (objValue.GetType() == typeof(ExpandoObject))
                //{
                //    var dic = (IDictionary<string, object>)objValue;
                //    foreach (string key in dic.Keys)
                //    {
                //        if (key == propName)
                //            return dic[key];
                //    }
                //}
                //else
                {
                    if (pi.Name != propName)
                        continue;
                    if (pi.PropertyType.FullName == typeof(double).FullName)
                    {
                        double dVal = pi.GetValue(data, null).ToMyDouble();
                        if (dVal != SystemDefault.InValidDouble)
                            ret = dVal;
                    }
                    else if (pi.PropertyType.FullName == typeof(string).FullName)
                        ret = pi.GetValue(data, null).ToMyString();
                    else
                        ret = pi.GetValue(data, null).ToMyString();
                }
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExpandoObject"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetExpandoObjectValue(object ExpandoObject, string propName)
        {
            Type t = ExpandoObject.GetType();
            if (t == typeof(ExpandoObject))
            {
                var dic = (IDictionary<string, object>)ExpandoObject;
                foreach (string key in dic.Keys)
                {
                    if (key == propName)
                        return dic[key];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取实体属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static object GetPropValue<T>(this T data, PropertyInfo pi)
        {
            object ret = default(object);
            if (pi.PropertyType == typeof(double))
            {
                double dVal = pi.GetValue(data, null).ToMyDouble();
                if (dVal != SystemDefault.InValidDouble)
                    ret = dVal;
            }
            if (pi.PropertyType == typeof(int))
            {
                int iVal = pi.GetValue(data, null).ToMyInt();
                if (iVal != SystemDefault.InValidInt)
                    ret = iVal;
            }
            else if (pi.PropertyType == typeof(string))
                ret = pi.GetValue(data, null).ToMyString();
            else
                ret = pi.GetValue(data, null).ToMyString();
            return ret;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="value"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool SetPropValue<T>(this PropertyInfo pi, ref T model, object value)
        {
            try
            {
                bool Success = true;
                object objPropValue = default(object);
                if (pi.PropertyType.FullName == typeof(DateTime).FullName)
                    //objPropValue = (DateTime)value;
                    objPropValue = value.ToMyDateTime();
                else if (pi.PropertyType.FullName == typeof(Int32).FullName)
                    objPropValue = value.ToMyInt();
                else if (pi.PropertyType.FullName == typeof(Boolean).FullName)
                    objPropValue = value.ToMyBool();
                else if (pi.PropertyType.FullName == typeof(String).FullName)
                    objPropValue = value.ToMyString().Trim();
                else if (pi.PropertyType.FullName == typeof(double).FullName)
                    objPropValue = value.ToMyDouble(SystemDefault.InValidDouble);
                else
                    Success = false;
                if (Success)
                    pi.SetValue(model, objPropValue, null);
                return Success;
            }
            catch (Exception )
            {
                return false;
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Model"></param>
        /// <param name="PropName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropValue<T>(this T Model, string PropName, object value)
        {
            try
            {
                if (Model == null)
                    return false;
                var Type = typeof(T);
                foreach (PropertyInfo pi in Type.GetProperties())
                {
                    if (pi.Name == PropName)
                    {
                        pi.SetPropValue<T>(ref Model, value);
                    }
                }
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        /// <summary> 
        /// 获取类的属性值的自定义特性
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static object GetPropAttribute<TClass, TAttr>(this string PropertyName)
        {
            try
            {
                PropertyInfo pi = typeof(TClass).GetProperty(PropertyName);
                object[] attributes = pi.GetCustomAttributes(typeof(TAttr), true);
                if (attributes != null)
                {
                    foreach (var attr in attributes)
                    {
                        //if (attr.GetType() == typeof(ColumnAttribute))
                        //if (attr is ColumnAttribute)
                        return attr;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// 获取类的属性值的自定义表特性
        /// ex: Model.GetTableAttr(x => x.Material).Name
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="d"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TableDef GetTableAttr<TClass>(this TClass d)
        {
            return TableAttribute.Table<TClass>();
        }

        /// <summary>
        /// 获取类的属性值的自定义列特性
        /// ex: Model.GetColumnAttr(x => x.Material).Name
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="d"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ColumnAttribute GetColumnAttr<TClass, TRes>(this TClass d, Expression<Func<TClass, TRes>> selector)
        {
            return d.GetPropAttribute<TClass, ColumnAttribute, TRes>(selector);
        }

        /// <summary> 
        /// 获取类的属性值的自定义特性
        /// ex: "GetPropAttribute<TClass,TAttr,string>(x=>x.Name)"
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static TAttr GetPropAttribute<TClass, TAttr, TRes>(this TClass d, Expression<Func<TClass, TRes>> selector)
            where TAttr : Attribute
        {
            try
            {
                // 使用表达式树获取 stu1.Name 属性上的特性
                Expression<Func<TClass, TRes>> expression = selector;
                MemberExpression memberExpression = expression.Body as MemberExpression;
                var attribute = memberExpression.Member.GetCustomAttribute<TAttr>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// 指定验证对象列表
        /// </summary>
        /// <param name="ValidatePropList">验证参与属性列表</param>
        /// <param name="IfContain">指定列表的属性参与验证</param>
        /// <returns></returns>
        public static CallResult Validate<T>(this T data, List<string> ValidatePropList = null, bool IfContain = true)
        {
            CallResult _result = new CallResult() { Success = true };
            var Type = typeof(T);
            PropertyInfo[] properties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
                if (pi.Name == "Error" || pi.Name == "Item")
                    continue;
                if (ValidatePropList != null)
                {
                    if (IfContain && !ValidatePropList.Contains(pi.Name) ||
                        (!IfContain && ValidatePropList.Contains(pi.Name)))
                        continue;
                }
                _result = data.Validate(pi.Name);
                if (_result.Fail)
                    return _result;
            }
            return _result;
        }

        /// <summary>
        /// 验证指定属性的关联规则
        /// </summary>
        /// <returns></returns>
        public static CallResult Validate<T>(this T data, string PropName)
        {
            var Type = typeof(T);
            CallResult _result = new CallResult() { Success = true };
            PropertyInfo pi = Type.GetProperty(PropName);
            if (pi == null)
                return _result;
            // 属性Error和Item来自接口IDataErrorInfo，无需进行验证
            if (pi.Name == "Error" || pi.Name == "Item")
                return _result;
            object value = pi.GetValue(data, null);
            object[] attributes = pi.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is NumberRuleAttribute numberRule)
                {
                    if (!numberRule.IsValid(value))
                    {
                        _result.Result = numberRule.ErrorMessage;
                        return _result;
                    }
                }
                else if (attribute is StringRuleAttribute stringRule)
                {
                    _result = StringRuleAttribute.Validate<T>(PropName, value);
                    if (_result.Fail)
                    {
                        return _result;
                    }
                }
                else if (attribute is NumbericAttribute numRule)
                {
                    if (!numRule.IsValid(value))
                    {
                        _result.Result = numRule.ErrorMessage;
                        return _result;
                    }
                }
            }
            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult SafeInvoke<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
            where TSource : class
        {
            return source != null ? func(source) : default(TResult);
        }
    }

    /// <summary>
    /// 反射 - 操作方法、函数
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 解析方法(同步 or 异步)并调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="FuncExpress"></param>
        /// <param name="FuncParamValue">分隔符,</param>
        /// <returns></returns>
        public static CallResult ParseMethodInvoke<T>(this T Source, string FuncExpress, string FuncParamValue)
        {
            CallResult result = new CallResult();
            string FuncName = FuncExpress.MidString("", "(").Trim();
            if (Source.IsSyncMethod(FuncName))
            {
                return Source.ParseSyncMethodInvoke(FuncExpress, FuncParamValue);
            }
            else if (Source.IsAsyncMethod(FuncName))
            {
                var task = Source.ParseAsyncMethodInvoke(FuncExpress, FuncParamValue);
                task.ContinueWith((TaskResult) =>
                {
                    result = TaskResult.Result;
                });
            }
            else
            {
                result.Fail = true;
                result.Result = $"解析指令[ {FuncName} ]无效";
            }
            return result;
        }

        /// <summary>
        /// 解析同步方法并调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="FuncExpress"></param>
        /// <param name="FuncParamValue"></param>
        /// <returns></returns>
        public static CallResult ParseSyncMethodInvoke<T>(this T Source, string FuncExpress, string FuncParamValue)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            try
            {
                if (string.IsNullOrEmpty(FuncExpress))
                {
                    ret.Result = "未关联到任务指令";
                    return ret;
                }
                //解析函数名称
                string FuncName = FuncExpress.MidString("", "(").Trim();
                //解析函数参数
                string FuncParamField = FuncExpress.MidString("(", ")", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
                if (!string.IsNullOrEmpty(FuncParamValue))
                {
                    //参数元
                    string[] ArrayDataValue = FuncParamValue.ToMyString().MySplit(',', false, true).ToArray();
                    //参数换元
                    FuncParamField = string.Format(FuncParamField, ArrayDataValue);
                }
                //参数字符串按，分割给参
                List<string> FuncParams = FuncParamField.MySplit(',', false, true);
                ret.Result = Source.SyncInvokeMethod(FuncName, FuncParams.ToArray());
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.Result = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 解析异步方法并调用
        /// </summary>
        /// <param name="FuncExpress"></param>
        /// <param name="FuncParamValue"></param>
        /// <returns></returns>
        public static async Task<CallResult> ParseAsyncMethodInvoke<T>(this T Source, string FuncExpress, string FuncParamValue)
        {
            CallResult ret = new CallResult() { Success = false, Result = "" };
            try
            {
                if (string.IsNullOrEmpty(FuncExpress))
                {
                    ret.Result = "未关联到任务指令";
                    return ret;
                }
                //解析函数名称
                string FuncName = FuncExpress.MidString("", "(").Trim();
                //解析函数参数
                string FuncParamField = FuncExpress.MidString("(", ")", EndStringSearchMode.FromTailAndToEndWhenUnMatch);
                if (!string.IsNullOrEmpty(FuncParamValue))
                {
                    //参数元
                    string[] ArrayDataValue = FuncParamValue.ToMyString().MySplit(',', false, true).ToArray();
                    //参数换元
                    FuncParamField = string.Format(FuncParamField, ArrayDataValue);
                }
                //参数字符串按，分割给参
                List<string> FuncParams = FuncParamField.MySplit(',', false, true);
                ret.Result = await Source.AsyncInvokeMethod(FuncName, FuncParams.ToArray());
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.Result = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 同步or异步方法动态调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceObject"></param>
        /// <param name="MethodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="methodParamTypes"></param>
        /// <returns></returns>
        public static CallResult InvokeMethod<T>(this T SourceObject, string MethodName,
            object[] methodParams = null, Type[] methodParamTypes = null)
        {
            CallResult result = new CallResult() { Success = false };
            try
            {
                MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
                if (dynMethod == null) throw new Exception($"方法[{MethodName} ]不存在");
                //var method = obj.GetType().GetMethod("Method", new Type[] { typeof(T) });
                int MethodParamCnt = dynMethod.GetParameters().Length;
                int SetParamCnt = 0;
                if (methodParams != null) SetParamCnt = methodParams.Length;
                if (MethodParamCnt >= SetParamCnt)
                {
                    if (SourceObject.IsSyncMethod(MethodName))
                    {
                        result.Result = dynMethod.Invoke(SourceObject, methodParams);
                        result.Success = true;
                    }
                    else if (SourceObject.IsSyncMethod(MethodName))
                    {
                        //获取返回类型
                        Type returnType = dynMethod.ReturnType;
                        //构造 Task<> 类型
                        Type taskType = typeof(Task<>).MakeGenericType(returnType);
                        //创建 Task 对象
                        //var task = Activator.CreateInstance(taskType);
                        Task invokeTask = (Task)dynMethod.Invoke(SourceObject, methodParams);
                        invokeTask.ContinueWith((TaskResult) =>
                        {
                            // 获取异步任务返回值
                            //object objValue = invokeTask.GetPropValue("Result");
                            PropertyInfo taskResultProp = invokeTask.GetType().GetProperty("Result");
                            object objValue = taskResultProp.GetValue(invokeTask);
                            result.Result = objValue;
                            result.Success = true;
                        });
                    }
                    else
                    {
                        result.Result = $"解析指令[ {MethodName} ]无效";
                        result.Fail = true;
                    }
                }
                else
                {
                    result.Result = "指令参数数量不匹配";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Result = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 同步方法动态调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceObject"></param>
        /// <param name="MethodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="methodParamTypes"></param>
        /// <returns></returns>
        public static CallResult SyncInvokeMethod<T>(this T SourceObject, string MethodName,
            object[] methodParams = null, Type[] methodParamTypes = null)
        {
            CallResult result = new CallResult() { Success = false };
            try
            {
                MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
                if (dynMethod == null) throw new Exception($"方法[{MethodName} ]不存在");
                //var method = obj.GetType().GetMethod("Method", new Type[] { typeof(T) });
                int MethodParamCnt = dynMethod.GetParameters().Length;
                int SetParamCnt = 0;
                if (methodParams != null) SetParamCnt = methodParams.Length;
                if (MethodParamCnt >= SetParamCnt)
                {
                    result.Result = dynMethod.Invoke(SourceObject, methodParams);
                    result.Success = true;
                }
                else
                {
                    result.Result = "指令参数数量不匹配";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Result = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 异步方法动态调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceObject"></param>
        /// <param name="MethodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="methodParamTypes"></param>
        /// <returns></returns>
        public static async Task<CallResult> AsyncInvokeMethod<T>(this T SourceObject, string MethodName,
            object[] methodParams = null, Type[] methodParamTypes = null)
        {
            CallResult result = new CallResult() { Success = false };
            try
            {
                MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
                if (dynMethod == null) throw new Exception($"方法[{MethodName} ]不存在");
                int MethodParamCnt = dynMethod.GetParameters().Length;
                int SetParamCnt = 0;
                if (methodParams != null) SetParamCnt = methodParams.Length;
                if (MethodParamCnt >= SetParamCnt)
                {
                    //获取返回类型
                    Type returnType = dynMethod.ReturnType;
                    //构造 Task<> 类型
                    Type taskType = typeof(Task<>).MakeGenericType(returnType); 
                    //创建 Task 对象
                    //var task = Activator.CreateInstance(taskType);
                    Task invokeTask = (Task)dynMethod.Invoke(SourceObject, methodParams);
                    await invokeTask.ConfigureAwait(false);
                    // 获取异步任务返回值
                    //object objValue = invokeTask.GetPropValue("Result");
                    PropertyInfo taskResultProp = invokeTask.GetType().GetProperty("Result");
                    object objValue = taskResultProp.GetValue(invokeTask);
                    result.Result = objValue;
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Result = "指令参数数量不匹配";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Result = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 判定方法是否为异步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod<T>(this T SourceObject, string MethodName, Type[] methodParamTypes = null)
        {
            MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
            return dynMethod != null && dynMethod.IsAsyncMethod();
        }

        /// <summary>
        /// 判定方法是否为同步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsSyncMethod<T>(this T SourceObject, string MethodName, Type[] methodParamTypes = null)
        {
            MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
            return dynMethod != null && !dynMethod.IsAsyncMethod();
        }

        /// <summary>
        /// 判定方法是否为异步方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(this MethodInfo method)
        {
            if (method == null)
                return false;
            try
            {
                Type returnType = method.ReturnType;
                bool IsTaskGenericType = false;
                if (returnType.IsGenericType)
                    IsTaskGenericType = returnType.GetGenericTypeDefinition() == typeof(Task<>);
                bool IsTaskType = returnType == typeof(Task);
                return (returnType.IsGenericType && IsTaskGenericType) || IsTaskType;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取函数信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceObject"></param>
        /// <param name="MethodName"></param>
        /// <param name="methodParamTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T>(this T SourceObject, string MethodName, Type[] methodParamTypes = null)
        {
            if (SourceObject == null)
                return null;
            MethodInfo dynMethod = GetMethodInfo<T>(MethodName, methodParamTypes);
            return dynMethod;
        }

        /// <summary>
        /// 获取函数信息
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo<T>(string MethodName, Type[] methodParamTypes = null)
        {
            try
            {
                Type ObjType = typeof(T);
                MethodInfo dynMethod = default(MethodInfo);
                if (methodParamTypes == null)
                    dynMethod = ObjType.GetMethod(MethodName);
                else
                    dynMethod = ObjType.GetMethod(MethodName, methodParamTypes);
                return dynMethod;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
