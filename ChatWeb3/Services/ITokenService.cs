using ChatWeb3Frontend.Models;

namespace ChatWeb3Frontend.Services
{
    public interface ITokenService
    {
        public Task<Response> GetVerificationMessage(string address);
        public Task<Response> VerifySignature(LoginDTO login);
    }
}
