using System.Reflection;

namespace dnex2winjs
{
    public static class Helper
    {
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
