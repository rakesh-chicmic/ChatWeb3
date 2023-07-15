using ChatWeb3.Models;

namespace ChatWeb3Frontend.Services
{
    public interface IApiCalling
    {
        public List<OutputMessage> GetOutputMessages(Guid chatId);
    }
}
