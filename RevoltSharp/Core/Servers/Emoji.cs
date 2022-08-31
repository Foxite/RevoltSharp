﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RevoltSharp
{
    public class Emoji : Entity
    {
        internal Emoji(RevoltClient client, EmojiJson model) : base(client)
        {
            Id = model.Id;
            Name = model.Name;
            CreatorId = model.CreatorId;
            ServerId = model.Parent.ServerId;
        }

        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public string ServerId { get; internal set; }

        public string CreatorId { get; internal set; }

        public string ImageUrl
            => Client.Config.Debug.UploadUrl + "/emojis/" + Id;

    }
}
