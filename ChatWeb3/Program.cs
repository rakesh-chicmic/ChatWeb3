using ChatWeb3.Data;
using ChatWeb3.Hubs;
using ChatWeb3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandlerService>();
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//add dbcontext
builder.Services.AddDbContext<ChatAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

//add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            if (string.IsNullOrEmpty(accessToken) == false)
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

//add signalr for sockets
builder.Services.AddSignalR();

//create cors policy to allow origins
builder.Services.AddCors(options => options.AddPolicy(name: "CorsPolicy",
policy =>
{
    policy.WithOrigins().AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    policy.WithOrigins("http://127.0.0.1:5500").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
}
));

//adding services for dependecy injection
builder.Services.AddScoped<IUploadPicService, UploadPicService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//use https redirection
app.UseHttpsRedirection();

//for uploading and giving access to static files
try
{
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "Assets")),
        RequestPath = "/Assets"
    });
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseRouting();
// Use authentication and authorization
app.UseAuthorization();

/*app.UseEndpoints(endpoints =>
{
    app.MapControllers();
    endpoints.MapHub<ChatAppHub>("/hubs/chat");
});*/
app.MapHub<ChatHub>("/chatHubs");

// Map the controllers
app.MapControllers();
// Run the application
app.Run();
