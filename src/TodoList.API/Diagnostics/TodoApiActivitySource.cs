using System.Diagnostics;

namespace TodoList.API.Diagnostics
{
    public static class TodoApiActivitySource
    {
        public const string ActivitySourceName = "TodoList.API";

        private static readonly ActivitySource source = new ActivitySource(ActivitySourceName);

        public static ActivitySource Source => source;
    }
}
