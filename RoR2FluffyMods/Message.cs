using RoR2;

namespace RoR2FluffyMods
{
    public static class Message
    {
        public static void SendToAll(string message, string colourHex)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = $"<color={colourHex}>{{0}}: {{1}}</color>",
                paramTokens = new string[] { MessageFrom, message } });
        }

        private static string MessageFrom { get => "TeleportVote"; }
    }    
}
