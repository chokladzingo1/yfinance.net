using System.Collections.Generic;

namespace yfinance
{
    // Shared static resources for yfinance
    public static class Shared
    {
        // Equivalent to Python's module-level variables
        public static Dictionary<string, object> DFS = new Dictionary<string, object>();
        public static object ProgressBar = null;
        public static Dictionary<string, object> Errors = new Dictionary<string, object>();
        public static Dictionary<string, object> Tracebacks = new Dictionary<string, object>();
        public static Dictionary<string, object> ISINS = new Dictionary<string, object>();
    }
}