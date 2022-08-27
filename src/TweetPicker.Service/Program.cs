using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using TweetPicker.Library.DataAccess;
using TweetPicker.Service.Interfaces;
using TweetPicker.Service.Jobs;
using TweetPicker.Service.Services;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var host = Host.CreateDefaultBuilder(args)
               .ConfigureServices((_, services) =>
               {
                 // data access
                 services.AddScoped<GiveawayEntriesDataAccess>();
                 services.AddScoped<GiveawayTweetsDataAccess>();
                 services.AddScoped<RepliesDataAccess>();
                 services.AddScoped<RetweetsDataAccess>();
                 services.AddScoped<UsersDataAccess>();
                 
                 // services
                 services.AddScoped<ISyncRepliesService, SyncRepliesService>();
                 services.AddScoped<ISyncRetweetsService, SyncRetweetsService>();
                 services.AddScoped<ITwitterApiService, TwitterApiService>();
                 
                 services.AddQuartz(q =>
                 {
                   q.UseMicrosoftDependencyInjectionJobFactory();
                   q.UseSimpleTypeLoader();
                   q.UseInMemoryStore();
                   q.UseDefaultThreadPool(tp =>
                   {
                     tp.MaxConcurrency = 10;
                   });

                   var scheduling = config.GetSection("JobScheduling");

                   q.ScheduleJob<SyncRepliesJob>(trigger => trigger.WithIdentity("SyncRepliesJob trigger")
                                                                   .StartNow()
                                                                   .WithSimpleSchedule(builder => builder
                                                                                                  .WithIntervalInMinutes(
                                                                                                    scheduling.GetValue<int>("SyncReplies"))
                                                                                                  .RepeatForever()));
                   
                   q.ScheduleJob<SyncRetweetsJob>(trigger => trigger.WithIdentity("SyncRetweetsJob trigger")
                                                                   .StartNow()
                                                                   .WithSimpleSchedule(builder => builder
                                                                                                  .WithIntervalInMinutes(
                                                                                                    scheduling.GetValue<int>("SyncRetweets"))
                                                                                                  .RepeatForever()));
                 });

                 services.AddQuartzHostedService(options =>
                 {
                   options.WaitForJobsToComplete = true;
                 });
               })
               .Build();

await host.RunAsync();