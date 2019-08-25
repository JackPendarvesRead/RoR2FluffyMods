using RoR2;

namespace RiskOfCatFacts
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

    public static class Colours
    {
        public static string LightBlue => "#03ffff";

        public static string Red => "#f01d1d";
        public static string Orange => "#ff7912";
        public static string Yellow => "#ffff26";
        public static string Green => "#0afa2a";
    }
}
