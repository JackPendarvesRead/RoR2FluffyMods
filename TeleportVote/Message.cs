using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleportVote
{
    internal static class Message
    {
        public static void SendToAll(string message, string colourHex)
        {
            Chat.SendBroadcastChat((Chat.ChatMessageBase)new Chat.SimpleChatMessage()
            {
                baseToken = $"<color={colourHex}>{{0}}: {{1}}</color>",
                paramTokens = new string[] { MessageFrom, message } });
        }

        private static string MessageFrom { get => "TeleportVote"; }

        /*
         Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = "<color=#e5eefc>{0}: {1}</color>",  paramTokens = new [] { "SOME_USERNAME_STRING", "SOME_TEXT_STRING" } })
         If you want to send something custom
         Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = "<color=#e5eefc>{0}</color>",  paramTokens = new [] { "SOME_TEXT_STRING" } })
         */
    }

    internal static class Colours
    {
        public static string Red => "#f01d1d";
        public static string Orange => "#ff7912";
        public static string Yellow => "#ffff26";
        public static string Green => "#0afa2a";
        public static string BluePurple => "#5409eb";
    }
}
