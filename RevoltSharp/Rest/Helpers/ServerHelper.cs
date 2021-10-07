﻿using RevoltSharp.Rest;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public static class ServerHelper
    {
        public static Task<Server> GetServerAsync(this SelfUser user, string serverId)
            => GetServerAsync(user.Client.Rest, serverId);

        public static async Task<Server> GetServerAsync(this RevoltRestClient rest, string serverId)
        {
            ServerJson Server = await rest.SendRequestAsync<ServerJson>(RequestType.Get, $"/servers/{serverId}");
            return Server.ToEntity(rest.Client);
        }

        public static Task<ServerMember[]> GetMembersAsync(this Server server)
           => GetMembersAsync(server.Client.Rest, server.Id);

        public static async Task<ServerMember[]> GetMembersAsync(this RevoltRestClient rest, string serverId)
        {
            MembersListJson List = await rest.SendRequestAsync<MembersListJson>(RequestType.Get, $"servers/{serverId}/members");
            HashSet<ServerMember> Members = new HashSet<ServerMember>();
            for (int i = 0; i < List.Members.Length; i++)
            {
                Members.Add(ServerMember.Create(List.Members[i], List.Users[i]));
            }
            return Members.ToArray();
        }

        

    }
}
