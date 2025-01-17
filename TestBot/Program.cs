﻿using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace TestBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Start().GetAwaiter().GetResult();
        }

        public static RevoltClient Client;
        public static async Task Start()
        {
            // Yes ik i can use json file blah blah :p
            string Token = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RevoltBots/Config.txt");
            Client = new RevoltClient(Token, ClientMode.WebSocket, new ClientConfig
            {
                Debug = new ClientDebugConfig { LogRestRequestJson = false, LogRestRequest = true, LogWebSocketFull = false, LogWebSocketReady = false, LogWebSocketError = true, LogWebSocketUnknownEvent = true },
                Owners = new string[] { "01FE57SEGM0CBQD6Y7X10VZQ49" }
            });
            Client.OnReady += Client_OnReady;
            Client.OnWebSocketError += Client_OnWebSocketError;
            await Client.StartAsync();
            CommandHandler Commands = new CommandHandler(Client);
            await Commands.Service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await Task.Delay(-1);
        }

        private static void Client_OnReady(SelfUser value)
        {
            Console.WriteLine("Ready: " + value.Username);
        }

        private static void Client_OnWebSocketError(SocketError value)
        {
            Console.WriteLine("Error: " + value.Messaage);
        }
    }

    public class CommandHandler
    {
        public CommandHandler(RevoltClient client)
        {
            Client = client;
            client.OnMessageRecieved += message => Client_OnMessageRecieved(message).GetAwaiter().GetResult();
            client.OnReactionAdded += Client_OnReactionAdded;
            client.OnReactionRemoved += Client_OnReactionRemoved;
        }

        private void Client_OnReactionAdded(Emoji emoji, ServerChannel channel, Downloadable<string, ServerMember> memberDownload, Downloadable<string, Message> messageDownload) {
            if (memberDownload.Id != Client.CurrentUser.Id) {
                Client.Rest.AddMessageReactionAsync(channel.Id, messageDownload.Id, emoji.Id).GetAwaiter().GetResult();
            }
        }

        private void Client_OnReactionRemoved(Emoji emoji, ServerChannel channel, Downloadable<string, ServerMember> memberDownload, Downloadable<string, Message> messageDownload) {
            if (memberDownload.Id != Client.CurrentUser.Id) {
                Client.Rest.RemoveMessageReactionAsync(channel.Id, messageDownload.Id, emoji.Id, Client.CurrentUser.Id).GetAwaiter().GetResult();
            }
        }

        public RevoltClient Client;
        public CommandService Service = new CommandService();
        private async Task Client_OnMessageRecieved(Message msg)
        {
            UserMessage Message = msg as UserMessage;
            if (Message == null || Message.Author.IsBot)
                return;
            int argPos = 0;
            if (!(Message.HasCharPrefix('!', ref argPos) || Message.HasMentionPrefix(Client.CurrentUser, ref argPos)))
                return;
            CommandContext context = new CommandContext(Client, Message);
            var result = await Service.ExecuteAsync(context, argPos, null);
            Console.WriteLine(result);
            if (result is ExecuteResult er) {
                Console.WriteLine(er.Exception);
            }
        }
    }
}
