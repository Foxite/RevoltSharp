﻿using Newtonsoft.Json;
using RevoltSharp.Rest;
using RevoltSharp.Rest.Requests;
using RevoltSharp.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RevoltSharp
{
    /// <summary>
    /// Revolt bot client used to connect to the Revolt chat API and WebSocket with a bot.
    /// </summary>
    public class RevoltClient : ClientEvents
    {
        /// <summary>
        /// Create a Revolt bot client.
        /// </summary>
        /// <param name="token">Bot token to connect with.</param>
        /// <param name="mode">Use http for http requests only with no websocket.</param>
        /// <param name="config">Optional config stuff for the bot and lib.</param>
        public RevoltClient(string token, ClientMode mode, ClientConfig? config = null)
        {
            try
            {
                DisableConsoleQuickEdit.Go();
            }
            catch { }
            if (string.IsNullOrEmpty(token))
                throw new RevoltArgumentException("Client token is missing!");

            Token = token;
            Config = config ?? new ClientConfig();
            UserBot = Config.UserBot;
            if (Config.Debug == null)
                Config.Debug = new ClientDebugConfig();
            Serializer = new JsonSerializer();
            Serializer.Converters.Add(new OptionConverter());
            Rest = new RevoltRestClient(this);
            if (mode == ClientMode.WebSocket)
                WebSocket = new RevoltSocketClient(this);
        }

        /// <summary>
        /// Revolt bot token used for http requests and websocket.
        /// </summary>
        public string Token { get; internal set; }

        public string Version { get; } = "4.0.7";

        internal bool UserBot { get; set; }

        internal JsonSerializer Serializer { get; set; }

        /// <summary>
        /// Client config options for user-agent and debug options including self-host support.
        /// </summary>
        public ClientConfig Config { get; internal set; }

        /// <summary>
        /// Internal rest/http client used to connect to the Revolt API.
        /// </summary>
        /// <remarks>
        /// You can also make custom requests with <see cref="RevoltRestClient.SendRequestAsync(RequestType, string, RevoltRequest)"/> and json class based on <see cref="RevoltRequest"/>
        /// </remarks>
        public RevoltRestClient Rest { get; internal set; }

        internal RevoltSocketClient WebSocket;

        internal bool FirstConnection = true;

        /// <summary>
        /// Start the WebSocket connection to Revolt.
        /// </summary>
        /// <remarks>
        /// Will throw a <see cref="RevoltException"/> if <see cref="ClientMode.Http"/>
        /// </remarks>
        /// <exception cref="RevoltException"></exception>
        public async Task StartAsync()
        {
            if (FirstConnection)
            {
                QueryRequest Query = await Rest.SendRequestAsync<QueryRequest>(RequestType.Get, "/");
                if (Query == null)
                {
                    Console.WriteLine("[RevoltSharp] Client failed to connect to the revolt api at " + Config.ApiUrl);
                    throw new RevoltException("Client failed to connect to the revolt api at " + Config.ApiUrl);
                }

                if (!Uri.IsWellFormedUriString(Query.serverFeatures.imageServer.url, UriKind.Absolute))
                    throw new RevoltException("[RevoltSharp] Server Image server url is an invalid format.");

                Config.Debug.WebsocketUrl = Query.websocketUrl;
                Config.Debug.UploadUrl = Query.serverFeatures.imageServer.url + "/";

                FirstConnection = false;
            }

            if (Config.UserBot)
            {
                UserJson SelfUser = await Rest.SendRequestAsync<UserJson>(RequestType.Get, "/users/@me");
                if (SelfUser == null)
                    throw new RevoltException("Failed to login to user account.");

                if (WebSocket != null)
                    WebSocket.CurrentUser = new SelfUser(this, SelfUser);

                Console.WriteLine($"[RevoltSharp] User login: {SelfUser.Username} ({SelfUser.Id})");
            }



            if (WebSocket != null)
            {
                var tcs = new TaskCompletionSource();

                void HandleConnected() => tcs.SetResult();
                void HandleError(SocketError error) => tcs.SetException(new RevoltException(error.Messaage));

                this.OnConnected += HandleConnected;
                this.OnWebSocketError += HandleError;
                
                _ = WebSocket.SetupWebsocket();
                
                await tcs.Task;
                this.OnConnected -= HandleConnected;
                this.OnWebSocketError -= HandleError;
            }
        }

        /// <summary>
        /// Stop the WebSocket connection to Revolt.
        /// </summary>
        /// <remarks>
        /// Will throw a <see cref="RevoltException"/> if <see cref="ClientMode.Http"/>
        /// </remarks>
        /// <exception cref="RevoltException"></exception>
        public async Task StopAsync()
        {
            if (WebSocket == null)
                throw new RevoltException("Client is in http-only mode.");

            if (WebSocket.WebSocket != null)
            {
                WebSocket.StopWebSocket = true;
                await WebSocket.WebSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "", WebSocket.CancellationToken);
            }
        }

        /// <summary>
        /// Get the current bot <see cref="User"/> after ready event.
        /// </summary>
        /// <remarks>
        /// Using <see cref="ClientMode.Http"/> means this is always <see langword="null"/> or use <see cref="Rest"/> to get user.
        /// </remarks>
        public SelfUser CurrentUser
            => WebSocket != null ? WebSocket.CurrentUser : null;

        public Server[] Servers
            => WebSocket != null ? WebSocket.ServerCache.Values.ToArray() : new Server[0];

        public User[] Users
           => WebSocket != null ? WebSocket.UserCache.Values.ToArray() : new User[0];



        public User GetUser(string id)
        {
            if (WebSocket != null && WebSocket.UserCache.TryGetValue(id, out User User))
                return User;
            return null;
        }

        public Channel GetChannel(string id)
        {
            if (WebSocket != null && WebSocket.ChannelCache.TryGetValue(id, out Channel Chan))
                return Chan;
            return null;
        }

        public Server GetServer(string id)
        {
            if (WebSocket != null && WebSocket.ServerCache.TryGetValue(id, out Server Server))
                return Server;
            return null;
        }

        public Emoji GetEmoji(string id)
        {
            if (WebSocket != null)
            {
                if (WebSocket.EmojiCache.TryGetValue(id, out Emoji emoji))
                    return emoji;
                return new Emoji(this, id);
            }
            return null;
        }
    }

    /// <summary>
    /// Run the client with Http requests only or use websocket to get cached data such as servers, channels and users instead of just ids.
    /// </summary>
    /// <remarks>
    /// Using <see cref="ClientMode.Http"/> means that some data will be <see langword="null"/> such as <see cref="Message.Author"/> and will only contain ids <see cref="Message.AuthorId"/>
    /// </remarks>
    public enum ClientMode
    {
        /// <summary>
        /// Client will only use the http/rest client of Revolt and removes any usage/memory of websocket stuff. 
        /// </summary>
        Http, 
        /// <summary>
        /// Will use both WebSocket and http/rest client so you can get cached data for <see cref="User"/>, <see cref="Server"/> and <see cref="Channel"/>
        /// </summary>
        WebSocket
    }
}
