# CourseClaimer.Wisedu

Auto course claiming for Wisedu sites.

## Configuration

In <code>appsettings.json</code>, edit your hostadresss, login port and database provider.

```json
{
  "BasePath": "https://jwxk.hrbeu.edu.cn/",
  "AuthPath": "https://jwxk.hrbeu.edu.cn/xsxk/auth/login",
  "DBProvider": "SQLite",
  "DBProvider_CAP": "InMemory",
}
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

speed limitation can be modified in <code>Shared.Extensions.EntityExtensions.cs</code>

```csharp
private const int LimitListMillSeconds = 400;
private const int LimitAddMillSeconds = 250;
```

## Usage

Add your information to the table tab, separating categories and courses with half width commas
