using Microsoft.EntityFrameworkCore;
using PdmoonblogApi.Models;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using PdmoonblogApi.Interfaces;
using PdmoonblogApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Load App Secrets
builder.Configuration.AddJsonFile("appsecrets.json", optional: true, reloadOnChange: true);

/* ADD SERVICES */
// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<BlogDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// AWS S3
var awsSettings = builder.Configuration.GetSection("AWS").Get<AwsSettings>() ?? throw new Exception("AWS settings not found");
var awsCredentials = new BasicAWSCredentials(awsSettings.AccessKeyId, awsSettings.SecretAccessKey);
var awsRegion = RegionEndpoint.GetBySystemName(awsSettings.Region);
builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(awsCredentials, awsRegion));
builder.Services.AddScoped<IAwsS3Service, AwsS3Service>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapGroup("/api/auth").MapIdentityApi<IdentityUser>();
app.MapPost("/api/auth/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
})
.RequireAuthorization();
app.MapControllers();

app.Run();
