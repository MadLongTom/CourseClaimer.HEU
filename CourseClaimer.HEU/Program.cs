﻿using CourseClaimer.Wisedu.Components;
using CourseClaimer.Ocr;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Savorboard.CAP.InMemoryMessageQueue;
using System.Security.Cryptography;
using System.Text;
using CourseClaimer.Wisedu.Shared.Handlers;
using CourseClaimer.Wisedu.Shared.Services;
using DotNetCore.CAP;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
// Add services to the container.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddResponseCompression();
builder.Services.AddBootstrapBlazor();

// 增加 SignalR 服务数据传输大小限制配置
builder.Services.Configure<HubOptions>(option => option.MaximumReceiveMessageSize = null);

builder.Services.AddCap(x =>
{
    switch (builder.Configuration["DBProvider_CAP"])
    {
        case "InMemory":
            x.UseInMemoryStorage();
            break;
        case "PostgreSQL":
            x.UsePostgreSql(builder.Configuration["PGSQL_CAP"]);
            break;
        case "SQLite":
            x.UseSqlite(@"Data Source=CAPDB.db;");
            break;
    }

    x.UseInMemoryMessageQueue();
    x.UseDashboard(d =>
    {
        d.AllowAnonymousExplicit = true;
    });
    x.CollectorCleaningInterval = 5;
});

builder.Services.AddDbContext<ClaimDbContext>(optionsBuilder =>
{
    switch (builder.Configuration["DBProvider"])
    {
        case "SQLServer":
            optionsBuilder.UseSqlServer(@"Server=.;Database=ClaimerDb;Trusted_Connection=True;TrustServerCertificate=true", s => s.MigrationsAssembly("CourseClaimer.Wisedu.EntityFramework.SQLServer"));
            break;
        case "SQLite":
            optionsBuilder.UseSqlite(@"Data Source=ClaimerDB.db;", s => s.MigrationsAssembly("CourseClaimer.Wisedu.EntityFramework.SQLite"));
            break;
        case "PostgreSQL":
            optionsBuilder.UseNpgsql(builder.Configuration["PGSQL"],s => s.MigrationsAssembly("CourseClaimer.Wisedu.EntityFramework.PostgreSQL"));
            break;
        default:
            throw new("Unknown DBType");
    }
}, ServiceLifetime.Transient, ServiceLifetime.Transient);

builder.Services.AddHttpClient("JWXK", client =>
{
    client.BaseAddress = new(builder.Configuration["BasePath"]);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new("application/json"));
    client.DefaultRequestHeaders.Accept.Add(new("text/plain"));
    client.DefaultRequestHeaders.Accept.Add(new("*/*"));
    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0");
    client.DefaultRequestHeaders.Connection.Add("keep-alive");
}).AddHttpMessageHandler<MapForwarderHandler>()
  .AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30); // 总的超时时间
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5); //每次重试的超时时间
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30); //熔断时间
});

builder.Services.AddSingleton<Aes>(inst =>
{
    var util = Aes.Create();
    util.Key = "MWMqg2tPcDkxcm11"u8.ToArray();
    return util;
});

builder.Services.AddTransient<MapForwarderHandler>();
builder.Services.AddSingleton<OcrService>();
builder.Services.AddSingleton<AuthorizeService>();
builder.Services.AddSingleton<ClaimService>();
builder.Services.AddSingleton<EntityManagementService>();
builder.Services.AddSingleton<CapClaimService>();
builder.Services.AddHostedService<EntityManagementService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseResponseCompression();
}

app.Services.CreateScope().ServiceProvider.GetRequiredService<ClaimDbContext>().Database.Migrate();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
