﻿namespace RevoltSharp
{
    class SavedMessagesChannel : Channel
    {
        public string User { get; internal set; }

        internal override void Update(PartialChannelJson json)
        {
            
        }
    }
}