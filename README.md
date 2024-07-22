# CourseClaimer.Wisedu

Auto course claiming for Wisedu sites.

## Annotations

![image](https://github.com/user-attachments/assets/9c51eaeb-f426-4f00-aa3a-a23e7311cd33)

## Configuration

In <code>appsettings.json</code>, edit your hostadresss, login port and database provider.

## OpenTelemetry

Use **Prometheus** to manage tracing and metrics

```json
{
  "BasePath": "https://jwxk.hrbeu.edu.cn/",
  "AuthPath": "https://jwxk.hrbeu.edu.cn/xsxk/auth/login",
  "DBProvider": "SQLite",
  "DBProvider_CAP": "InMemory",
  "ReLoginDelayMilliseconds": 300000,
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
