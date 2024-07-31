using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace HelloDoc.BAL.Repo
{
    public static class UserHandler
    {
        public static HashSet<(string, string)> ConnectedIds = new HashSet<(string, string)>();
    }
    public class Chathub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly IChatclient _IChatClient;

        public Chathub(IChatclient IChatClient)
        {
            _IChatClient = IChatClient;
        }

        public async Task SendMessage(string userId, string message)
        {
            _IChatClient.SaveMessage((Context.User.Claims.FirstOrDefault(_ => _.Type == "userId").Value), userId, message);
            await Clients.User(userId).SendAsync("ReceiveMessage", message, Context.UserIdentifier, DateTime.Now.ToString("hh:mm tt"));
        }

        public async Task GetUserInfo(string userType, int requestId)
        {
            string recieverId;
            string recievrName;

            var userInfo = _IChatClient.GetUserInfo(userType, requestId);

            var messages = _IChatClient.GetMessages(new List<string> { Context.UserIdentifier, userInfo.Item1 });

            await Clients.Caller.SendAsync("ReceiveUserInfo", userInfo.Item1, userInfo.Item2, messages);
        }   

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add((Context.UserIdentifier, Context.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email).Value));
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove((Context.UserIdentifier, Context.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email).Value));
            return base.OnDisconnectedAsync(exception);
        }
    }
}
