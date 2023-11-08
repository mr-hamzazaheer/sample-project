using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Shared
{
    public static class Extension
    {
        public static bool IsNullOrZero(this int value) => value == 0;
        public static string Serialize<T>(this T data)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }
        public static string Encode(this object text)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(text)))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(Convert.ToString(text));
                var ms = new MemoryStream();
                using (var stream = new GZipStream(ms, CompressionMode.Compress, true))
                    stream.Write(buffer, 0, buffer.Length);

                ms.Position = 0;

                var rawData = new byte[ms.Length];
                ms.Read(rawData, 0, rawData.Length);

                var compressedData = new byte[rawData.Length + 4];
                Buffer.BlockCopy(rawData, 0, compressedData, 4, rawData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, compressedData, 0, 4);
                string convertedData = Convert.ToBase64String(compressedData);
                convertedData = convertedData.Replace("+", "_@_");
                convertedData = convertedData.Replace("=", "_!_");
                convertedData = convertedData.Replace("/", "_~_");
                return convertedData;
            }
            else
                return string.Empty;
        }
        public static string Decode(this string compressedText)
        {
            if (!string.IsNullOrEmpty(compressedText))
            {
                compressedText = compressedText.Replace("_@_", "+");
                compressedText = compressedText.Replace("_!_", "=");
                compressedText = compressedText.Replace("_~_", "/");
                byte[] compressedData = Convert.FromBase64String(compressedText);
                using var ms = new MemoryStream();
                int dataLength = BitConverter.ToInt32(compressedData, 0);
                ms.Write(compressedData, 4, compressedData.Length - 4);

                var buffer = new byte[dataLength];

                ms.Position = 0;
                using (var stream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
            else
                return string.Empty;
        }
        public static string InvalidRequest()
        {
            return (new
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Invalid request params"
            }).Serialize();
        }
        public static DateTime DbDate(this string date, bool toUtc = false)
        {
            if (string.IsNullOrEmpty(date))
                return DateTime.MinValue;
            {
                DateTime dt = DateTime.Parse(date, CultureInfo.GetCultureInfo("en-US"));
                return (toUtc) ? DateTimeOffset.Parse(string.Format("{0:MM/dd/yyyy HH:mm}", dt)).DateTime : dt;
            }
        }
        public static string ViewDate(this DateTime date, bool viewTimewithDate = false)
        {
            string format = "MM/dd/yyyy";
            if (date == DateTime.MinValue)
                return string.Empty;

            return string.Format(viewTimewithDate ? "{0:" + format + " HH:mm}" : "{0:" + format + "}", date);
        }
        public static UserProfile UserProfile(this HttpContext httpContext) => httpContext.GetToken()?.GetClaim();
        public static bool ValidatePermission(this HttpContext httpContext)
        {
            string permissions = Convert.ToString(httpContext.Request.Headers["permissions"]).Decode();
            //// implement this functionality after adding permissions
            //if (string.IsNullOrEmpty(permissions))
            //    return false;
            //{
            //    List<ModuleAccessLevel> moduleAccessLevels = JsonConvert.DeserializeObject<List<ModuleAccessLevel>>(permissions);
            //    moduleAccessLevels = moduleAccessLevels.Where(z => z.Path != null).ToList();
            //    var url = "/" + httpContext.Request.RouteValues["controller"] + "/" + httpContext.Request.RouteValues["action"];
            //    return (!moduleAccessLevels.Count(x => x.Path.ToLower() == url.ToLower()).IsNullOrZero());
            //}
            return true;
        }
        public static bool ShouldBypassMiddleware(this string path)
        {
            var extensionsToSkip = new[] { ".png", ".jpg", ".jpeg", ".gif", ".js", ".svg", ".map", ".mp4", ".ogg", ".txt" };
            foreach (var extension in extensionsToSkip)
            {
                if (path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // Skip middleware processing
                }
            }
            return false; // Process the middleware
        }
    }
}
