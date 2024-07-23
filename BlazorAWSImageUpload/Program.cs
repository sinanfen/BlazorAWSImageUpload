using BlazorAWSImageUpload.Data;
using Amazon.S3;
using BlazorAWSImageUpload.Services;
using Microsoft.AspNetCore.Http.Features;
using Amazon.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Configure AWS credentials
var awsOptions = builder.Configuration.GetAWSOptions();
awsOptions.Credentials = new BasicAWSCredentials(
    builder.Configuration["AWSCredentials:AccessKey"],
    builder.Configuration["AWSCredentials:SecretKey"]);

// Add AWS S3 service with explicit credentials
builder.Services.AddAWSService<IAmazonS3>(awsOptions);

// Add S3 service
builder.Services.AddScoped<IS3Service, S3Service>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();