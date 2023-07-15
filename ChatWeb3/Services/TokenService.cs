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
        Response response;      //resonse model
        private readonly ChatAppDbContext DbContext;    //ORM db context injected
        private readonly IConfiguration _configuration;     //configuration settings injected

        //-------------------------- Constructor --------------------------------------------//
        public TokenService(IConfiguration configuration, ChatAppDbContext dbContext)
        {
            response = new Response();
            this._configuration = configuration;
            DbContext = dbContext;
        }

        //-------------------------- generate verification message for auth --------------------------------------------//
        public async Task<Response> GetVerificationMessage(string address)
        {
            if(address == null || address == string.Empty)
            {
                response = new Response(400, "Value cannot be null","", true);
                return response;
            }
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

        //-------------------------- verify auth details --------------------------------------------//
        public async Task<Response> VerifySignature(LoginDTO login)
        {
            var temp = DbContext.AccountMessagemappings.Where(a => a.accountAddress == login.signer).Select(a => a);
            if(temp.FirstOrDefault() == null || temp!.FirstOrDefault()!.message!=login.message)
            {
                return new Response(400, "Invalid credentials", "registration/login failed", false);
            }
            List<User> users = DbContext.Users.Where(s=>s.accountAddress == login.signer).ToList();
            User? user1 = users.FirstOrDefault();
            if(user1 !=null && user1.isDeleted == true)
            {
                response = new Response(400, "Account deleted", "registration/login failed", false);
                return response;
            }
            Response result = await Authenticate(login);

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

        //-------------------------- helper function --------------------------------------------//
        private async Task<Response> Authenticate(LoginDTO login)
        {
            var signer = new Nethereum.Signer.MessageSigner();
            var account = signer.EcRecover(login.hash.HexToByteArray(), login.signature);

            if (account.ToLower().Equals(login.signer.ToLower()))
            {
                // read user from DB or create a new one
                List<User> users = DbContext.Users.Where(s => s.accountAddress == login.signer.ToLower()).Select(a => a).ToList();
                User? user1 = users.FirstOrDefault();
                //if(user1 !=null && user1.isDeleted == true)
                //{
                    //this code can be used if you want to recover acc on sign in but then it will be completely illogical to delete acc
                    //user1.isDeleted = false;
                    //await DbContext.SaveChangesAsync();
                //}

                users = users.Where(s=>s.isDeleted == false).ToList();

                UserRegisterLogin data = new UserRegisterLogin();
                if(user1 == null)
                {
                    //user doesn't exist create new
                    var random = new Random();
                    string msg = random.Next(100000, 999999).ToString();
                    User user = new User(login.signer, $"username{msg}", "firstName", "lastName", "");
                    DbContext.Users.Add(user);
                    await DbContext.SaveChangesAsync();
                    ResponseUser responseUser = new ResponseUser(user);
                    data = new UserRegisterLogin(responseUser, false);
                }
                else
                {
                    //user already exists
                    ResponseUser responseUser = new ResponseUser(user1);
                    data = new UserRegisterLogin(responseUser, true);
                }
                response = new Response(200, "Authentication successful", data, true);
                return response;
            }
            response = new Response(400,"Invalid credentials","registration/login failed",false);
            return response;
        }

        //-------------------------- helper function to create token --------------------------------------------//
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