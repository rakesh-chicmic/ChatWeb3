using Microsoft.AspNetCore.Mvc;
using ChatWeb3.Models;
using System.Text;
using System.Text.Json;
using ChatWeb3.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;

namespace ChatWeb3.Services
{
    public class TokenService:ITokenService
    {
        Response response;
        private readonly ChatAppDbContext DbContext;
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, ChatAppDbContext dbContext)
        {
            response = new Response();
            this._configuration = configuration;
            DbContext = dbContext;
        }

        public async Task<Response> GetVerificationMessage(string address)
        {
            var random = new Random();
            string message = "To authenticate you need to sign this message " + random.Next(100000,999999);
            AccountMessageMapping newMap = new AccountMessageMapping(address,message);
            var temp = DbContext.AccountMessagemappings.Where(a => a.accountAddress == address).Select(a=>a);
            if(temp.FirstOrDefault() == null)
            {
                DbContext.AccountMessagemappings.Add(newMap);
                await DbContext.SaveChangesAsync();
                response = new Response(200, "Message generated", newMap, true);
            }
            else
            {
                temp!.FirstOrDefault()!.message = message;
                await DbContext.SaveChangesAsync();
                response = new Response(200, "Message generated", temp!.FirstOrDefault()!, true);
            }
            return response;
        }

        public async Task<Response> VerifySignature(LoginDTO login)
        {
            var temp = DbContext.AccountMessagemappings.Where(a => a.accountAddress == login.signer).Select(a => a);
            if(temp.FirstOrDefault() == null || temp!.FirstOrDefault()!.message!=login.message)
            {
                return new Response(400, "Invalid credentials", "registration/login failed", false);
            }
            Response result = await Authenticate(login);
            Console.WriteLine(result.ToString());
            Console.WriteLine(result.data.ToString());
            UserRegisterLogin resultData = (UserRegisterLogin)result.data;
            if (result.success == true)
            {
                //build token and send in response
                string token = CreateToken(resultData.responseUser);
                resultData.token = token;
                result.data = resultData;
                return result;
            }
            return result;
        }

        private async Task<Response> Authenticate(LoginDTO login)
        {
            var signer = new Nethereum.Signer.MessageSigner();
            var account = signer.EcRecover(login.hash.HexToByteArray(), login.signature);

            if (account.ToLower().Equals(login.signer.ToLower()))
            {
                // read user from DB or create a new one
                List<User> users = DbContext.Users.Where(s => s.accountAddress == login.signer.ToLower()).Select(a => a).ToList();
                UserRegisterLogin data = new UserRegisterLogin();
                if(users.FirstOrDefault() == null)
                {
                    //user doesn't exist create new
                    User user = new User(login.signer, "username", "firstName", "lastName", "");
                    DbContext.Users.Add(user);
                    await DbContext.SaveChangesAsync();
                    ResponseUser responseUser = new ResponseUser(user);
                    data = new UserRegisterLogin(responseUser, false);
                }
                else
                {
                    //user already exists
                    User user = users!.FirstOrDefault()!;
                    ResponseUser responseUser = new ResponseUser(user);
                    data = new UserRegisterLogin(responseUser, true);
                }
                response = new Response(200, "Authentication successful", data, true);
                return response;
            }
            response = new Response(400,"Invalid credentials","registration/login failed",false);
            return response;
        }

        private string CreateToken(ResponseUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.firstName),
                new Claim(ClaimTypes.Sid, user.accountAddress),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}