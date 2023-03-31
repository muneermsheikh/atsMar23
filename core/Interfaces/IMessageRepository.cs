using core.Entities.Messages;
using core.Entities.EmailandSMS;
using core.Params;
using core.Dtos;

namespace core.Interfaces
{
     public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void AddMessage(EmailMessage message);
        void DeleteMessage(EmailMessage message);
        Task<EmailMessage> GetMessage(int id);
        Task<Pagination<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
    }
}