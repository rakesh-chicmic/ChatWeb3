using ChatWeb3.Models;

namespace ChatWeb3.Services
{
    public interface ITokenService
    {
        public Task<Response> GetVerificationMessage(string address);
        public Task<Response> VerifySignature(LoginDTO login);
    }
}
