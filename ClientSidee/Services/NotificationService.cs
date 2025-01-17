using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

public class NotificationService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    private readonly AuthService _authService;
    public event Action<string> OnFriendRequestReceived;
    public event Action<string> OnFriendRequestAccepted;

    public NotificationService(AuthService authService, NavigationManager navigationManager)
    {
        _authService = authService;

        var hubUrl = navigationManager.ToAbsoluteUri("/Application/Hubs/ChatHub");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = async () => await _authService.GetTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On("ReceiveFriendRequest", (object data) =>
        {
            OnFriendRequestReceived?.Invoke("You have a new friend request.");
        });

        _hubConnection.On("FriendRequestAccepted", (object data) =>
        {
            OnFriendRequestAccepted?.Invoke("Your friend request was accepted.");
        });
    }

    public async Task StartAsync()
    {
        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}