namespace ToDo_lists.Services
{
    class AiServiceConfig
    {
        public static string host = Environment.GetEnvironmentVariable("AI_MODULE_HOST");
        public static string port = Environment.GetEnvironmentVariable("AI_MODULE_PORT");
    }
}