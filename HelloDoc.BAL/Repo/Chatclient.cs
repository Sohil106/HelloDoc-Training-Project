using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace HelloDoc.BAL.Repo
{
    public class Chatclient : IChatclient
    {
        private readonly ApplicationDbContext _context;
      

        public Chatclient(ApplicationDbContext db)
        {
            _context = db;
            
        }
        public (string, string) GetUserInfo(string userType, int requestId)
        {
            string receiverId;
            string receiverName;
            if (userType == "Admin")
            {
                Admin admin = _context.Admins.FirstOrDefault(_ => _.AdminId == 1);
                receiverId = admin.AspNetUserId;
                receiverName = admin.FirstName + " " + admin.LastName;
            }
            else if (userType == "Physician")
            {
                Request request = _context.Requests.Include(_ => _.Physician).FirstOrDefault(_ => _.RequestId == requestId);
                receiverId = request.Physician.AspNetUserId;
                receiverName = "Dr." + request.Physician.FirstName + " " + request.Physician.LastName;
            }
            else
            {
                Request request = _context.Requests.FirstOrDefault(_ => _.RequestId == requestId);
                User user = _context.Users.FirstOrDefault(_ => _.UserId == request.UserId);
                receiverId = user.AspNetUserId;
                receiverName = user.FirstName + " " + user.LastName;
            }

            return (receiverId, receiverName);
        }

        public bool SaveMessage(string sender, string receiver, string message)
        {
            try
            {
                MessagesTable messagestable = new MessagesTable
                {
                    Message = message,
                    Sender = sender,
                    Reciever = receiver,
                    SentTime = DateTime.Now
                };
                _context.MessagesTables.Add(messagestable);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetMessages(List<string> userIds)
        {
            var messages = _context.MessagesTables.Where(_ => userIds.Contains(_.Sender) && userIds.Contains(_.Reciever)).Select(_ => new
            {
                _.MessageId,
                _.Message,
                _.Sender,
                _.Reciever,
                _.SentTime,
                SentDate = _.SentTime.ToString("dd/MM/yyyy"),
                time = _.SentTime.ToString("hh:mm tt")
            }).OrderBy(_ => _.SentTime).ToList();
            return JsonSerializer.Serialize(messages);
        }


    }
}
