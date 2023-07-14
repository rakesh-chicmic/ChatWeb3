using Microsoft.AspNetCore.Mvc;
using ChatWeb3.Models;
using System.Text;
using System.Text.Json;
using ChatWeb3.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace ChatWeb3.Services
{
    public class UploadPicService:IUploadPicService
    {
        Response response;          //response model
        private readonly ChatAppDbContext DbContext;        //dbcontext injected
        private readonly IConfiguration _configuration;     //config settings

        //-------------------------- Constructor --------------------------------------------//
        public UploadPicService(IConfiguration configuration, ChatAppDbContext dbContext)
        {
            response = new Response();
            this._configuration = configuration;
            DbContext = dbContext;
        }
        //-------------------------- service func to upload a file b/w chats --------------------------------------------//
        //type of file = type 1 -> files  type 2 -> images
        public async Task<Response> FileUploadAsync(IFormFile file,string id,int type)
        {
            if (file == null)
            {
                response = new Response(400, "Please provide a file for successful upload", string.Empty, false);
                return response;
            }
            if (file.Length > 0)
            {
                string folderName;
                if (type == 2)  //type 2 is for images
                {
                    folderName = Path.Combine("Assets", "Images"); 
                }
                else // type 1 or any other int corres. to files type
                {
                    folderName = Path.Combine("Assets", "Files");
                }
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = string.Concat(
                                    Path.GetFileNameWithoutExtension(file.FileName),
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                    Path.GetExtension(file.FileName)
                                    );

                var fullPath = Path.Combine(pathToSave, fileName);
                //save file
                using (var stream = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(stream);
                }
                FileUploadResponse res = new FileUploadResponse(fileName, Path.Combine(folderName, fileName));
                response = new Response(200, "File Uploaded Successfully", res, true);
                return response;
            }
            response = new Response(400, "Please provide a file for successful upload", string.Empty, false);
            return response;
        }

        //-------------------------- service func to upload user profile pic and update db --------------------------------------------//
        public async Task<Response> ProfilePicUploadAsync(IFormFile file, string id)
        {
            Guid guid = new Guid(id);
            User? user = await DbContext.Users.FindAsync(guid);
            var folderName = Path.Combine("Assets", "ProfilePics");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (file == null)
            {
                response = new Response(400, "Please provide a file for successful upload", string.Empty, false);
                return response;
            }
            if (file.Length > 0)
            {
                var fileName = string.Concat(
                                    id,
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff"), 
                                    Path.GetFileNameWithoutExtension(file.FileName),
                                    Path.GetExtension(file.FileName)
                                    );

                var fullPath = Path.Combine(pathToSave, fileName);

                if (user!=null)
                {
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);
                    }
                    user.pathToProfilePic = Path.Combine(folderName, fileName);
                    await DbContext.SaveChangesAsync(); 
                    ResponseUser responseUser = new ResponseUser(user);
                    FileResponseData data = new FileResponseData(responseUser, fileName, Path.Combine(folderName, fileName));
                    response = new Response(200, "File Uploaded Successfully", data, true);
                    return response;
                }
                response = new Response(500, "Something went wrong", "", false);
            }
            response = new Response(400, "Please provide a file for successful upload", string.Empty, false);
            return response;
        }


    }
}