# CourseClaimer.Wisedu

Auto course claiming for Wisedu sites.

## Configuration

In <code>appsettings.json</code>, edit your hostadresss, login port and database provider.

```json
{
  "BasePath": "https://jwxk.hrbeu.edu.cn/",
  "AuthPath": "https://jwxk.hrbeu.edu.cn/xsxk/auth/login",
  "DBProvider": "SQLite",
}
```
**Notice: Once you changed the DBProvider, you should delete the <code>Migrations</code> folder and run the migrate command**

```shell
dotnet ef migrations add Init
```

You can also change the database provider for EventBus(CAP) in <code>Program.cs</code>

```csharp
builder.Services.AddCap(x =>
{ 
    x.UseInMemoryStorage();
    //x.UseSqlite(cfg => cfg.ConnectionString = "Data Source=CAPDB.db");
    x.UseInMemoryMessageQueue(); 
    x.UseDashboard(d =>
    {
        d.AllowAnonymousExplicit = true;
    });
    x.CollectorCleaningInterval = 5;
});
```

And configure the AesKey

```csharp
builder.Services.AddSingleton<Aes>(inst =>
{
    var util = Aes.Create();
    util.Key = "MWMqg2tPcDkxcm11"u8.ToArray();
    return util;
});
```

speed limitation can be modified in <code>Shard.Extensions.EntityExtensions.cs</code>

```csharp
private const int LimitListMillSeconds = 400;
private const int LimitAddMillSeconds = 250;
```

## Usage

Add your information to the table tab, separating categories and courses with half width commas
