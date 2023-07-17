using ChatWeb3.Models;

namespace ChatWeb3Frontend.Services
{
    public class ApiCalling : IApiCalling
    {
        List<OutputMessage> IApiCalling.GetOutputMessages(Guid chatId)
        {
            throw new NotImplementedException();
        }
    }
}
