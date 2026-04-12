using Microsoft.AspNetCore.SignalR;

namespace SimpleAuthSystem
{
    public class MyHub : Hub
    {
        public async Task SendRequest(string incomingString)
        {
            // 1. Notify the Server UI that a message arrived
            ServerEvents.RaiseMessageReceived(incomingString);

            // 2. Send back a string and a boolean as the response
            string responseText = $"Server received: {incomingString}";
            bool successStatus = true;

            await Clients.Caller.SendAsync("ReceiveResponse", responseText, successStatus);
        }
    }

    // Simple static class to bridge the Hub and the WPF Window
    public static class ServerEvents
    {
        public static event Action<string>? OnMessageReceived;
        public static void RaiseMessageReceived(string msg) => OnMessageReceived?.Invoke(msg);
    }
}
