using ChatWeb3.Data;
using ChatWeb3.Models;
using ChatWeb3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NBitcoin.Secp256k1;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace ChatWeb3.Controllers
{
    [Route("api/v1/[controller]")]
    public class TokenController : Controller
    {
        ITokenService _tokenService;      //service dependency
        private readonly ILogger<TokenController> _logger;      //logger instance
        Response response = new Response();             //response model
        private IConfiguration _config;         // configuration settings

        //-------------------------- Constructor --------------------------------------------//
        public TokenController(IConfiguration config, ITokenService tokenService, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _config = config;
            _logger = logger;
        }

        //--------------- get verification message for generating and storing random message in db to authenticate via metamask -----------------------------//
        [AllowAnonymous]
        [HttpGet]
        [Route("/api/v1/getMessage")]
        public async Task<IActionResult> GetVerificationMessage(string address)
        {
            _logger.LogInformation("get verification message method started");
            try
            {
                response = await _tokenService.GetVerificationMessage(address);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Internal server error ", ex.Message);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //-------------------------- verify signature takes hash, signature , msg and account address to validate a user login/signin----------------------------------------//
        [AllowAnonymous]
        [HttpPost]
        [Route("/api/v1/verifySignature")]
        public async Task<IActionResult> VerifySignature([FromBody] LoginDTO login)
        {
            _logger.LogInformation("verification of signature method started");
            try
            {
                response = await _tokenService.VerifySignature(login);
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
