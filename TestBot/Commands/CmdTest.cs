﻿using RevoltSharp;
using RevoltSharp.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestBot.Commands
{
    public class CmdTest : ModuleBase
    {
        [Command("test")]
        public async Task Test()
        {
            Console.WriteLine("MSG: test");
            await Context.Channel.SendMessageAsync("Test");
        }

        [Command("chan")]
        public async Task Channel()
        {
            await ReplyAsync(Context.Channel.Id);
        }

        [Command("roles")]
        public async Task Role()
        {
            await Context.Channel.SendMessageAsync("", null, new Embed[]
            {
                new EmbedBuilder
                {
                    Description = String.Join("\n", Context.Server.Roles.Select(x => $"{x.Name} - {x.Color} ({x.Id})"))
                }.Build()
            }); 
            foreach(var Role in Context.Server.Roles)
            {
                Console.WriteLine($"[ {Role.Name} ] {Role.Permissions.AssignRoles}:{Role.Permissions.BanMembers}:{Role.Permissions.ManageMessages}:{Role.Permissions.SendMessages}");
            }
            
        }

        [Command("embed")]
        public async Task Embed()
        {
            await Context.Channel.SendMessageAsync("Hello", null, new RevoltSharp.Embed[]
            {
                new EmbedBuilder
                {
                    Title = "Title",
                    Description = "Desc",
                    IconUrl = "https://img.fluxpoint.dev/2531914271424512.png",
                    Url = "https://fluxpoint.dev",
                    Color = new RevoltColor("")
                }.Build()
            });
        }
    }
}
