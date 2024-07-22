# CourseClaimer.Wisedu

Auto course claiming for Wisedu sites.

## Annotations

![image](https://github.com/user-attachments/assets/9c51eaeb-f426-4f00-aa3a-a23e7311cd33)

## OpenTelemetry

Use **<code>Prometheus</code>** to manage tracing and metrics, modify <code>prometheus.yml</code>

```yml
# my global config
global:
  scrape_interval: 15s # Set the scrape interval to every 15 seconds. Default is every 1 minute.
  evaluation_interval: 15s # Evaluate rules every 15 seconds. The default is every 1 minute.
  # scrape_timeout is set to the global default (10s).

# Alertmanager configuration
alerting:
  alertmanagers:
    - static_configs:
        - targets:
          # - alertmanager:9093

# Load rules once and periodically evaluate them according to the global 'evaluation_interval'.
rule_files:
  # - "first_rules.yml"
  # - "second_rules.yml"

# A scrape configuration containing exactly one endpoint to scrape:
# Here it's Prometheus itself.
scrape_configs:
  # The job name is added as a label `job=<job_name>` to any timeseries scraped from this config.
  - job_name: "prometheus"

    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.

    static_configs:
      - targets: ["IP:Port"]

```

## Configuration

In <code>appsettings.json</code>, edit your hostadresss, login port and database provider.

```json
{
  "BasePath": "https://jwxk.hrbeu.edu.cn/",
  "AuthPath": "https://jwxk.hrbeu.edu.cn/xsxk/auth/login",
  "DBProvider": "SQLite",
  "DBProvider_CAP": "InMemory",
  "ReLoginDelayMilliseconds": 300000,
  "CapTakeNum": 5,
  "QuartzDelayMilliseconds": 300000,
  "CronSchedule": "0 18/20 * * * ? ",
  "UseQuartz": false,
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

## Docker

A ubuntu docker img with dotnet 8 sdk and opencv

for running this interestring software in linux docker

using <code>docker build -t heujwxk .</code> in cli to build docker img

and then use <code>docker-compose up -d</code> to start the project

the data will be saved in the db as the compose goes.

the docker-compose.yml just give a example, though it could be run.

## Usage

Add your information to the table tab, separating categories and courses with half width commas
