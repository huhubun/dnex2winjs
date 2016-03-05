using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dnex2winjs
{
    public static class DNEX
    {
        static readonly Type TYPE_OF_EXCEPTION = typeof(Exception);
        static readonly Assembly assembly = Assembly.Load("mscorlib");

        /// <summary>
        /// 获取 .NET Framework 定义的所有异常的名称和 HResult
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetAllExceptions()
        {
            var result = new Dictionary<string, int>();

            // 获取 .NET Framework 定义的所有 Exception 类
            var allExceptions = assembly.GetTypes().Where(t => t.IsClass && t.IsPublic && (t.IsSubclassOf(TYPE_OF_EXCEPTION) || t.Equals(TYPE_OF_EXCEPTION)));

            foreach (var exceptionType in allExceptions)
            {
                Exception ex = null;

                try
                {
                    // 使用无参构造函数初始化
                    ex = assembly.CreateInstance(exceptionType.Namespace + "." + exceptionType.Name, false, BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null, null) as Exception;
                }
                catch
                {
                    // 对于没有无参构造函数的异常类（例如 RuntimeWrappedException）
                    // 使用一个参数，类型为 Object 的构造函数初始化
                    ex = assembly.CreateInstance(exceptionType.Namespace + "." + exceptionType.Name, false, BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { new object() }, null, null) as Exception;
                }

                result.Add(exceptionType.Name, ex.HResult);
            }

            return result;
        }

        public static Dictionary<string, int> GetAllHResults()
        {
            var result = new Dictionary<string, int>();

            var HResultsClass = assembly.GetTypes().First(t => t.Name.Equals("__HResults"));

            foreach (var item in HResultsClass.GetRuntimeFields())
            {
                result.Add(item.Name, (int)item.GetValue(null));
            }

            return result;
        }
    }
}
