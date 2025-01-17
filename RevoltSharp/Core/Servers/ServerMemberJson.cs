﻿using Newtonsoft.Json;
using Optionals;
using System;

namespace RevoltSharp
{
    internal class ServerMemberJson
    {
        [JsonProperty("_id")]
        public ServerMemberIdsJson Id;

        [JsonProperty("nickname")]
        public string Nickname;

        [JsonProperty("avatar")]
        public AttachmentJson Avatar;

        [JsonProperty("roles")]
        public string[] Roles;

        [JsonProperty("joined_at")]
        public DateTime JoinedAt;

        [JsonProperty("timeout")]
        public Optional<DateTime> Timeout;
    }
    internal class ServerMemberIdsJson
    {
        [JsonProperty("server")]
        public string Server;

        [JsonProperty("user")]
        public string User;
    }
}
