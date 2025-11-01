using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Files;
using Newtonsoft.Json;

namespace QuestNav.Config
{
    /// <summary>
    /// Pure C# class (NOT MonoBehaviour) that runs EmbedIO server on background thread.
    /// Uses async/await which is acceptable per Unity rules for non-MonoBehaviour classes.
    /// Does not call Unity APIs directly - uses ILogger interface instead.
    /// </summary>
    public class ConfigServer
    {
        private WebServer m_server;
        private CancellationTokenSource m_cancellationTokenSource;
        private readonly ReflectionBinding m_binding;
        private readonly ConfigStore m_store;
        private readonly string m_authToken;
        private readonly int m_port;
        private readonly bool m_enableCORSDevMode;
        private readonly string m_staticPath;
        private readonly ILogger m_logger;

        public bool IsRunning => m_server != null && m_server.State == WebServerState.Listening;
        public string BaseUrl => $"http://localhost:{m_port}/";

        public ConfigServer(ReflectionBinding binding, ConfigStore store, string authToken, int port, bool enableCORSDevMode, string staticPath, ILogger logger)
        {
            m_binding = binding;
            m_store = store;
            m_authToken = authToken;
            m_port = port;
            m_enableCORSDevMode = enableCORSDevMode;
            m_staticPath = staticPath;
            m_logger = logger;
        }

        public void Start()
        {
            if (IsRunning)
            {
                m_logger?.LogWarning("[ConfigServer] Server already running");
                return;
            }

            m_cancellationTokenSource = new CancellationTokenSource();

            m_logger?.Log($"[ConfigServer] Starting server on port {m_port}");
            m_logger?.Log($"[ConfigServer] Static files path: {m_staticPath}");
            m_logger?.Log($"[ConfigServer] Auth token: {m_authToken}");

            m_server = new WebServer(o => o
                .WithUrlPrefix($"http://*:{m_port}/")
                .WithMode(HttpListenerMode.EmbedIO))
                .WithModule(new ActionModule("/api", HttpVerbs.Any, HandleApiRequest))
                .WithStaticFolder("/", m_staticPath, true);

            Task.Run(async () =>
            {
                try
                {
                    await m_server.RunAsync(m_cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    m_logger?.LogError($"[ConfigServer] Server error: {ex.Message}");
                }
            });

            m_logger?.Log($"[ConfigServer] Server started at {BaseUrl}");
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            m_logger?.Log("[ConfigServer] Stopping server...");
            
            m_cancellationTokenSource?.Cancel();
            m_server?.Dispose();
            m_server = null;
            
            m_logger?.Log("[ConfigServer] Server stopped");
        }

        private async Task HandleApiRequest(IHttpContext context)
        {
            try
            {
                // CORS headers
                if (m_enableCORSDevMode)
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

                // OPTIONS preflight
                if (context.Request.HttpVerb == HttpVerbs.Options)
                {
                    context.Response.StatusCode = 200;
                    return;
                }

                // Auth check
                string authHeader = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    context.Response.StatusCode = 401;
                    await context.SendDataAsync(new { success = false, message = "Unauthorized" });
                    return;
                }

                string token = authHeader.Substring("Bearer ".Length).Trim();
                if (token != m_authToken)
                {
                    context.Response.StatusCode = 401;
                    await context.SendDataAsync(new { success = false, message = "Invalid token" });
                    return;
                }

                // Route requests
                string path = context.Request.Url.AbsolutePath;

                if (path == "/api/schema" && context.Request.HttpVerb == HttpVerbs.Get)
                {
                    await HandleGetSchema(context);
                }
                else if (path == "/api/config" && context.Request.HttpVerb == HttpVerbs.Get)
                {
                    await HandleGetConfig(context);
                }
                else if (path == "/api/config" && context.Request.HttpVerb == HttpVerbs.Post)
                {
                    await HandlePostConfig(context);
                }
                else if (path == "/api/info" && context.Request.HttpVerb == HttpVerbs.Get)
                {
                    await HandleGetInfo(context);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.SendDataAsync(new { success = false, message = "Not found" });
                }
            }
            catch (Exception ex)
            {
                m_logger?.LogError($"[ConfigServer] Request error: {ex.Message}");
                context.Response.StatusCode = 500;
                await context.SendDataAsync(new { success = false, message = ex.Message });
            }
        }

        private async Task HandleGetSchema(IHttpContext context)
        {
            var schema = m_binding.GenerateSchema();
            await context.SendDataAsync(schema);
        }

        private async Task HandleGetConfig(IHttpContext context)
        {
            var values = m_binding.GetAllValues();
            var result = new
            {
                success = true,
                values = values,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            await context.SendDataAsync(result);
        }

        private async Task HandlePostConfig(IHttpContext context)
        {
            string body = await context.GetRequestBodyAsStringAsync();
            var updateRequest = JsonConvert.DeserializeObject<ConfigUpdateRequest>(body);

            if (updateRequest == null || string.IsNullOrEmpty(updateRequest.path))
            {
                context.Response.StatusCode = 400;
                await context.SendDataAsync(new { success = false, message = "Invalid request" });
                return;
            }

            var oldValue = m_binding.GetValue(updateRequest.path);
            bool success = m_binding.SetValue(updateRequest.path, updateRequest.value);
            var newValue = m_binding.GetValue(updateRequest.path);

            if (success)
            {
                var configData = new ConfigData { values = m_binding.GetAllValues() };
                m_store.SaveConfig(configData);

                var response = new ConfigUpdateResponse
                {
                    success = true,
                    message = "Configuration updated",
                    oldValue = oldValue,
                    newValue = newValue
                };

                await context.SendDataAsync(response);
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.SendDataAsync(new ConfigUpdateResponse
                {
                    success = false,
                    message = "Failed to update configuration"
                });
            }
        }

        private async Task HandleGetInfo(IHttpContext context)
        {
            var info = new
            {
                version = "1.0.0",
                platform = "Android",
                deviceModel = "Quest",
                connectedClients = 0,
                configPath = m_store.GetConfigPath(),
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            await context.SendDataAsync(info);
        }
    }
}
