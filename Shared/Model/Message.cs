namespace Shared
{
    public class Message
    {
        public static readonly string Success = "Request processed successfully";

        public static readonly string Exists = "Already Exists";

        public static readonly string Error = "Something went wrong";
        public static string KeyNotFound(string key) => $"{key} not found";
    }
}
