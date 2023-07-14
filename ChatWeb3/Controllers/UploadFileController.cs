using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace ChatWeb3.Controllers
{
    //controller to handle file upload 
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        IUploadPicService uploadPicServiceInstance;      //service dependency
        private readonly ILogger<UploadFileController> _logger;     //logger instance
        Response response = new Response();             //response model

        //-------------------------- Constructor --------------------------------------------//
        public UploadFileController(ILogger<UploadFileController> logger, IConfiguration configuration, ChatAppDbContext dbContext)
        {
            uploadPicServiceInstance = new UploadPicService(configuration, dbContext);
            _logger = logger;
        }

        //-------------------------- upload file with type to send in b/w chats --------------------------------------------//
        [HttpPost, DisableRequestSizeLimit, Authorize]
        [Route("/api/v1/uploadFile")]
        public async Task<IActionResult> FileUploadAsync(int type, IFormFile file)
        {
            //type 2 is for image and save in images folder and type 2 is for file to save in files folder
            _logger.LogInformation("File/Image Upload method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;                                //extracting email from header token
                //string? token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();       //getting token from authorization header
                response = await uploadPicServiceInstance.FileUploadAsync(file,id,type);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //-------------------------- upload profile pic and change in user db --------------------------------------------//
        [HttpPost, DisableRequestSizeLimit, Authorize]
        [Route("/api/v1/uploadProfilePic")]
        public async Task<IActionResult> ProfilePicUploadAsync(IFormFile file)                //[FromForm] FileUpload File
        {
            _logger.LogInformation("Pic Upload method started");
            try
            {
                string id = User.FindFirstValue(ClaimTypes.PrimarySid)!;
                response = await uploadPicServiceInstance.ProfilePicUploadAsync(file, id);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
