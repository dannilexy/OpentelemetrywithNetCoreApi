using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetryDemo
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Logging.ClearProviders();
            builder.Logging.AddOpenTelemetry(x =>
            {
                x.SetResourceBuilder(ResourceBuilder.CreateEmpty()
                    .AddService("OpenTelemetryDemo").AddAttributes(new Dictionary<string, object>()
                    {
                        ["deployment.environment"] = builder.Environment.EnvironmentName
                    }));
                x.IncludeScopes = true;
                x.IncludeFormattedMessage = true;

                x.AddOtlpExporter(a =>
                {
                    a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                    a.Protocol = OtlpExportProtocol.HttpProtobuf;
                    a.Headers = "X-Seq-ApiKey=SPGlVfkUSaC2TjvXXKMI";
                });
            });

            builder.Services.AddOpenTelemetry().WithTracing(b =>
                 b.AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation()
                 .AddOtlpExporter(a =>
                 {
                     a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                     a.Protocol = OtlpExportProtocol.HttpProtobuf;
                     a.Headers = "X-Seq-ApiKey=SPGlVfkUSaC2TjvXXKMI";
                 }));

            //builder.Services.AddOpenTelemetry().WithMetrics(m =>
            //{
            //    m.AddMeter("OpenTelemetryDemo").AddOtlpExporter();
            //});
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
