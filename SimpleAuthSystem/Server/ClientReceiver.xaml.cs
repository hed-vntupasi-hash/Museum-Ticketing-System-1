using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace SimpleAuthSystem;

public partial class ClientReceiver : Window
{
    public ClientReceiver()
    {
        InitializeComponent();

        // Listen for Hub events to update UI
        ServerEvents.OnMessageReceived += (msg) =>
        {
            Dispatcher.Invoke(() => RequestList.Items.Add($"{DateTime.Now:HH:mm:ss}: {msg}"));
        };

        StartBackgroundServer();
    }

    private void StartBackgroundServer()
    {
        Task.Run(() =>
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddSignalR();

            // Listen on Port 5000 on ALL local network adapters
            builder.WebHost.UseUrls("http://*:5000");

            var app = builder.Build();
            app.MapHub<MyHub>("/requestHub");

            Dispatcher.Invoke(() => StatusLabel.Text = "Status: Server Running on Port 5000");
            app.Run();
        });
    }
}