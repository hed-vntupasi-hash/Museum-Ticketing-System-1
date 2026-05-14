using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleAuthSystem.Pages
{
    public partial class ClientReceiverPage : Page
    {
        private bool serverStarted = false;

        public ClientReceiverPage()
        {
            InitializeComponent();

            // Listen for Hub events to update UI
            ServerEvents.OnMessageReceived += (msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    RequestList.Items.Add($"{DateTime.Now:HH:mm:ss}: {msg}");
                });
            };

            StartBackgroundServer();
        }

        private void StartBackgroundServer()
        {
            // Prevent duplicate server instances
            if (serverStarted)
                return;

            serverStarted = true;

            Task.Run(() =>
            {
                try
                {
                    var builder = WebApplication.CreateBuilder();

                    builder.Services.AddSignalR();

                    // Listen on Port 5000
                    builder.WebHost.UseUrls("http://*:5000");

                    var app = builder.Build();

                    app.MapHub<MyHub>("/requestHub");

                    Dispatcher.Invoke(() =>
                    {
                        StatusLabel.Text = "Status: Server Running on Port 5000";
                    });

                    app.Run();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusLabel.Text = $"Error: {ex.Message}";
                    });
                }
            });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null &&
                NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}