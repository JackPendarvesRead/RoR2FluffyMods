using RoR2;

namespace RoR2FluffyMods
{
    public static class Message
    {
        public static void Send(string message)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = "{0}",
                paramTokens = new string[] { message }
            });
        }

        public static void Send(string message, string messageFrom)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = "{0}: {1}",
                paramTokens = new string[] { messageFrom, message }
            });
        }

        public static void SendColoured(string message, string colourHex)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = $"<color={colourHex}>{{0}}</color>",
                paramTokens = new string[] { message }
            });
        }

        public static void SendColoured(string message, string colourHex, string messageFrom)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = $"<color={colourHex}>{{0}}: {{1}}</color>",
                paramTokens = new string[] { messageFrom, message }
            });
        }
    }
}
