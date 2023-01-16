using Microsoft.AspNetCore.Http.Features;
using System.Text;

namespace TodoList.UI.MVC.Extentions
{
    internal static class DebugPageExtension
    {
        public static void MapDebugPage(this WebApplication application, string pattern)
        {
            application.Map("/Debug", async (context) =>
            {
                var sb = new StringBuilder();
                var nl = System.Environment.NewLine;
                var rule = string.Concat(nl, new string('-', 40), nl);

                sb.Append($"Request{rule}");
                sb.Append($"{DateTimeOffset.Now}{nl}");
                sb.Append($"{context.Request.Method} {context.Request.Path}{nl}");
                sb.Append($"Scheme: {context.Request.Scheme}{nl}");
                sb.Append($"Host: {context.Request.Headers["Host"]}{nl}");
                sb.Append($"PathBase: {context.Request.PathBase.Value}{nl}");
                sb.Append($"Path: {context.Request.Path.Value}{nl}");
                sb.Append($"Query: {context.Request.QueryString.Value}{nl}{nl}");

                sb.Append($"Connection{rule}");
                sb.Append($"RemoteIp: {context.Connection.RemoteIpAddress}{nl}");
                sb.Append($"RemotePort: {context.Connection.RemotePort}{nl}");
                sb.Append($"LocalIp: {context.Connection.LocalIpAddress}{nl}");
                sb.Append($"LocalPort: {context.Connection.LocalPort}{nl}");
                sb.Append($"ClientCert: {context.Connection.ClientCertificate}{nl}{nl}");

                sb.Append($"Headers{rule}");
                foreach (var header in context.Request.Headers)
                {
                    sb.Append($"{header.Key}: {header.Value}{nl}");
                }
                sb.Append(nl);

                sb.Append($"Websockets{rule}");
                if (context.Features.Get<IHttpUpgradeFeature>() != null)
                {
                    sb.Append($"Status: Enabled{nl}{nl}");
                }
                else
                {
                    sb.Append($"Status: Disabled{nl}{nl}");
                }

                sb.Append($"Configuration{rule}");
                foreach (var pair in application.Configuration.AsEnumerable())
                {
                    sb.Append($"{pair.Key}: {pair.Value}{nl}");
                }
                sb.Append(nl);

                sb.Append($"Environment Variables{rule}");
                var vars = System.Environment.GetEnvironmentVariables();
                foreach (var key in vars.Keys.Cast<string>().OrderBy(key => key,
                    StringComparer.OrdinalIgnoreCase))
                {
                    var value = vars[key];
                    sb.Append($"{key}: {value}{nl}");
                }

                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(sb.ToString());
            });
        }
    }
}
