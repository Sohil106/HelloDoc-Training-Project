using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IChatclient
    {
        (string, string) GetUserInfo(string userType, int requestId);

        bool SaveMessage(string sender, string receiver, string message);

        string GetMessages(List<string> userIds);
    }
}
