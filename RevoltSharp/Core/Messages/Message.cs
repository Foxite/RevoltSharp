﻿namespace RevoltSharp
{
    /// <summary>
    /// Base chat messages that can be casted to <see cref="UserMessage"/> or <see cref="SystemMessage{Type}"/>
    /// </summary>
    public abstract class Message : Entity
    {
        public Message(RevoltClient client)
            : base(client)
        { }

        internal static Message Create(RevoltClient client, MessageJson model)
        {
            if (model.Author == "00000000000000000000000000")
            {
                if (model.System != null)
                {
                    switch (model.System.Type)
                    {
                        case "text":
                            return new SystemMessage<SystemText>(client, model, new SystemText());
                        case "user_added":
                            return new SystemMessage<SystemUserAdded>(client, model, new SystemUserAdded());
                        case "user_remove":
                            return new SystemMessage<SystemUserRemoved>(client, model, new SystemUserRemoved());
                        case "user_joined":
                            return new SystemMessage<SystemUserJoined>(client, model, new SystemUserJoined());
                        case "user_left":
                            return new SystemMessage<SystemUserLeft>(client, model, new SystemUserLeft());
                        case "user_kicked":
                            return new SystemMessage<SystemUserKicked>(client, model, new SystemUserKicked());
                        case "user_banned":
                            return new SystemMessage<SystemUserBanned>(client, model, new SystemUserBanned());
                        case "channel_renamed":
                            return new SystemMessage<SystemChannelRenamed>(client, model, new SystemChannelRenamed());
                        case "channel_description_changed":
                            return new SystemMessage<SystemChannelDescriptionChanged>(client, model, new SystemChannelDescriptionChanged());
                        case "channel_icon_changed":
                            return new SystemMessage<SystemChannelIconChanged>(client, model, new SystemChannelIconChanged());
                        case "channel_ownership_changed":
                            return new SystemMessage<SystemChannelOwnershipChanged>(client, model, new SystemChannelOwnershipChanged());
                    }
                }
                return new SystemMessage<SystemUnknown>(client, model, new SystemUnknown());
            }

            return new UserMessage(client, model);
        }

        /// <summary>
        /// Id of the message
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Parent channel id of the message
        /// </summary>
        public string ChannelId { get; internal set; }

        /// <summary>
        /// Parent channel of the message
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        /// Id of the user who posted the message
        /// </summary>
        public string AuthorId { get; internal set; }

        /// <summary>
        /// User who posted the message
        /// </summary>
        public User Author { get; internal set; }

        
    }
}
