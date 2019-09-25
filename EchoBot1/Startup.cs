// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using EchoBot1.Bots;
using EchoBot1.Services;
using Microsoft.Bot.Builder.Azure;
using EchoBot1.Dialogs;

namespace EchoBot1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Configure Services
            services.AddSingleton<BotServices>();

            // Configure state
            ConfigureState(services);

            ConfigureDialogs(services);


            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //services.AddTransient<IBot, GreetingBot>();
            services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes)
            //services.AddSingleton<IStorage, MemoryStorage>();

            var storageAccount = "DefaultEndpointsProtocol=https;AccountName=tutorialbot;AccountKey=89kTzYT9SQACtLx4emPCMwv5UJqlYc7Df0RsSWGtiHTXLJzBJfW1Nl+KUmf+RirFFd57gYZbtWgmIUX7z4s3Sg==;EndpointSuffix=core.windows.net";
            var storageContainer = "tutorialbot";

            services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));

            // Create the User State
            services.AddSingleton<UserState>();

            // Create the Conversation State
            services.AddSingleton<ConversationState>();

            // Create an instance of the state service
            services.AddSingleton<BotStateService>();
        }
        public void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
