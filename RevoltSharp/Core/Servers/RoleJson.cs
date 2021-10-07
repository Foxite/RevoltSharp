﻿using Newtonsoft.Json;

namespace RevoltSharp
{
    public class RoleJson
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("permissions")]
        public int[] Permissions;

        [JsonProperty("hoist")]
        public bool Hoist;

        [JsonProperty("rank")]
        public int Rank;

        [JsonProperty("colour")]
        public string Colour;
    }
}
