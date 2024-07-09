
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace LearningHangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHangfire(configuration => configuration
              .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
              .UseSimpleAssemblyNameTypeSerializer()
              .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                })
            );

            builder.Services.AddHangfireServer();


            builder.Services.AddControllers();
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

            app.UseAuthorization();

            //app.MapControllers();

            app.UseHangfireDashboard();

            BackgroundJob.Enqueue(() => Debug.WriteLine("Bom dia!"));

            BackgroundJob.Schedule(
                () => Debug.WriteLine("Hello, world"),
                TimeSpan.FromSeconds(5)
            );

            RecurringJob.AddOrUpdate("cronzin basico", () => Debug.WriteLine("que isso po!"), Cron.Minutely);

            RecurringJob.AddOrUpdate("cronzin personalizadin", () => Debug.WriteLine("ai sim hein 10s"), "*/10 * * * * *"); // 10s?


            app.Run();
        }

        
    }
}
