using ChatWeb3.Controllers;
using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Models.OutputModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatWeb3.Services
{
    public class UserService: IUserService
    {
        Response response = new Response();         //response model
        private readonly ChatAppDbContext DbContext;    //orm 
        private readonly IConfiguration _configuration;     //config settings

        //-------------------------- Constructor --------------------------------------------//
        public UserService(IConfiguration configuration, ChatAppDbContext dbContext, ILogger<UserController> logger)
        {
            this._configuration = configuration;
            DbContext = dbContext;
        }
        //-------------------------- Service func to get user's own details for profie details --------------------------------------------//
        public Response GetYourself(string id)
        {
            Guid guid = new Guid(id);
            User? userLoggedIn = DbContext.Users.FindAsync(guid).Result;
            
            if(userLoggedIn == null || userLoggedIn.isDeleted==true)
            {
                response = new Response(404,"Can't get user details error","Something went wrong",false);
                return response;
            }

            ResponseUser res = new ResponseUser(userLoggedIn);
            response = new Response(200, "User details fetched",res,true);
            return response;
        }
        //-------------------------- get other users details with filteration and pagination--------------------------------------------//
        public Response GetUsers(string userId, string? searchString = null, string? accountAddress = null, string? id = null, string OrderBy = "username", int SortOrder = 1, int RecordsPerPage = 20, int PageNumber = 0)          // sort order   ===   e1 for ascending   -1 for descending
        {
            //get logged in user from database
            Guid guid = new Guid(userId);
            User? userLoggedIn = DbContext.Users.FindAsync(guid).Result;
            var userss = DbContext.Users.AsQueryable();
            userss = userss.Where(u => u != userLoggedIn);       //remove logged in user from list

            userss = userss.Where(t => t.isDeleted == false);     //remove deleted users from list

            //--------------------------filtering based on userId,searchString, Email, or Phone---------------------------------//
            
            if (id != null) { 
                Guid newId = new Guid(id);
                userss = userss.Where(s => (s.id == newId)); 
            }
            if (accountAddress != null)
            {
              userss = userss.Where(s => (s.accountAddress == accountAddress));
            }
            if (searchString != null) { userss = userss.Where(s => (EF.Functions.Like(s.username, "%" + searchString + "%"))); }

            var users = userss.ToList();

            // delegate used to create orderby depending on user input
            Func<User, System.Object> orderBy = s => s.username;
            if (OrderBy == "userid" || OrderBy == "ID" || OrderBy == "Id")
            {
                orderBy = x => x.id;
            }
            else if (OrderBy == "accountAddress" || OrderBy == "AccountAddress" || OrderBy == "address")
            {
                orderBy = x => x.accountAddress;
            }

            // sort according to input based on orderby
            if (SortOrder == 1)
            {
                users = users.OrderBy(orderBy).Select(c => (c)).ToList();
            }
            else
            {
                users = users.OrderByDescending(orderBy).Select(c => (c)).ToList();
            }

            //pagination
            users = users.Skip((PageNumber - 1) * RecordsPerPage)
                                  .Take(RecordsPerPage).ToList();

            List<ResponseUser> res = new List<ResponseUser>();

            foreach (var user in users)
            {
                ResponseUser r = new ResponseUser(user);              //need to use automapper or constructor here
                res.Add(r);
            }

            if (!res.Any())
            {
                response = new Response(200,"no user found","",true);
                return response;
            }

            response = new Response(200,"users list fetched",res,true);

            return response;
        }
        //-------------------------- update user details--------------------------------------------//
        public async Task<Response> UpdateUser(string id, UpdateUser update)
        {
            Guid guid = new Guid(id);
            User? userLoggedIn = DbContext.Users.FindAsync(guid).Result;

            if (userLoggedIn != null && userLoggedIn.isDeleted == false)
            {
                if (update.username != "string" && update.username != string.Empty)
                {
                    User? user = DbContext.Users.Where(s => (s.username == update.username && s.id != userLoggedIn.id)).FirstOrDefault();
                    if (user != null)
                    {
                        response = new Response(400, "Username already taken", "", false);
                        return response;
                    }
                    userLoggedIn.username = update.username;
                }
                if (update.pathToProfilePic != "string" && update.pathToProfilePic != string.Empty)
                {
                    userLoggedIn.pathToProfilePic = update.pathToProfilePic;
                }
                
                userLoggedIn.updatedAt= DateTime.Now;
                await DbContext.SaveChangesAsync();

                ResponseUser data = new ResponseUser(userLoggedIn);
                response = new Response(200,"User Updated Successfully",data,true);
                return response;
            }
            else
            {
                response = new Response(404, "User not found", "", false);
                return response;
            }
        }
        //-------------------------- func to help user choose an unique username--------------------------------------------//
        public Response ValidateUsername(string id, string username)
        {
            Guid guid = new Guid(id);
            User? userLoggedIn = DbContext.Users.FindAsync(guid).Result;

            if (userLoggedIn != null && userLoggedIn.isDeleted == false)
            {
                var users = DbContext.Users.Where(s => s.username == username).Select(s => s).ToList();
                ValidateUsernameModel result;
                if (users.Any())
                {
                    var rand = new Random();
                    string msg = rand.Next(100000,999999).ToString();
                    username = username.Trim();
                    result = new ValidateUsernameModel(username, false, $"{username}{msg}");
                    response = new Response(200, "Username already taken", result, true);
                    return response;
                }
                else
                {
                    result = new ValidateUsernameModel(username, true, username);
                    response = new Response(200, "Username available", result, true);
                    return response;
                }
            }
            else
            {
                response = new Response(404, "User not found", "", false);
                return response;
            }
        }
        //-------------------------- delete user account (soft delete) --------------------------------------------//
        public async Task<Response> DeleteUser(string id)
        {
            Guid guid = new Guid(id);
            User? user = DbContext.Users.FindAsync(guid).Result;
            //List<AccountMessageMapping> mappings = DbContext.AccountMessagemappings.Where(s => s.accountAddress == user.accountAddress).ToList();

            if (user != null && user.isDeleted == false)
            {
                user.isDeleted = true;
                await DbContext.SaveChangesAsync();

                response = new Response(200,"User Deleted Successfully","",true);
                return response;
            }
            else
            {
                response = new Response(404, "User not found", "", false);
                return response;
            }

        }
    }
}
