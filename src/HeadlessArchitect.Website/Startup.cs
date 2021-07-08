namespace HeadlessArchitect.Website
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Diagnostics;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Microsoft.Extensions.Logging;

    using Deliverystack.Core.Configuration;
    using Deliverystack.DeliveryApi.Models;
    using Deliverystack.Interfaces;
    using Deliverystack.StackContent;

    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            IsDevelopment = env.IsDevelopment();
        }

        private IConfiguration Configuration { get; }

        private DeliverystackConfiguration DsConfig { get; set; }

        private bool IsDevelopment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // cms client
            services.AddSingleton<IConfigureJsonSerializer, ContentstackJsonSerializerConfigurator>();
            services.AddSingleton(new ContentstackDeliveryClientOptions(
                    Configuration.GetSection("ContentstackOptions")));
            services.AddHttpClient<IDeliveryClient, ContentstackDeliveryClient>();
            DsConfig = new DeliverystackConfiguration(Configuration.GetSection("Deliverystack"));

            if (DsConfig.HostWebApi)
            {
                // server cache
                services.AddSingleton(serviceProvider =>
                {
                    return new PathApiCache(
                        serviceProvider.GetRequiredService<IDeliveryClient>());
                });

                services.AddSingleton(serviceProvider =>
                {
                    return new RedirectApiCache(
                        serviceProvider.GetRequiredService<IDeliveryClient>());
                });
            }

            // web API client configuration 
            services.AddSingleton(new PathApiConfig(Configuration.GetSection("PathApi")));
            services.AddTransient<PathApiClient, PathApiClient>();
            services.AddSingleton(
                new RedirectApiConfig(Configuration.GetSection("RedirectApi")));
            services.AddTransient<RedirectApiClient, RedirectApiClient>();

            foreach (string name in Directory.GetFiles(
                AppDomain.CurrentDomain.BaseDirectory, "*.dll").Where(r =>
                    !AppDomain.CurrentDomain.GetAssemblies().ToList().Select(a =>
                        a.Location).ToArray().Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList())
            {
                try
                {
                    Assembly.Load(Path.GetFileNameWithoutExtension(new FileInfo(name).Name));
                }
                catch(Exception)
                {
                    Console.WriteLine("Unable to load assembly " + name);
                }
            }

            if (DsConfig.HostWebApi)
            {
                // web API server configuration
                services.AddMvc();
//TODO:? EnableEndpointRouting
//TODO: was false when believed to be working last
                services.AddControllers(options => options.EnableEndpointRouting = false);
//                services.AddControllers(options => options.EnableEndpointRouting = true);

                if (IsDevelopment)
                {
                    //TODO: /swagger/index.html does not seem to work
                    services.AddSwaggerGen(swagger =>
                    {
                        swagger.SwaggerDoc("v1",
                            new OpenApiInfo
                            {
                                Title = "HeadlessArchitect.DeliveryApi",
                                Version = "v1"
                            });
                        swagger.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    });
                }
            }

            services.AddRazorPages();
            Console.WriteLine("ConfigureServices() complete: " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
        }

        public void Configure(IApplicationBuilder app, 
            RedirectApiClient redirectApiClient, 
            ILogger<Startup> logger)
        {
            logger.LogInformation("Configure()");

            if (!IsDevelopment)
            {
                app.UseResponseCaching();
                //TODO: app.UseResponseCompression();
            }

            if (IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
//                app.UseExceptionHandler("/Error");

            }
            else
            {
                app.UseExceptionHandler("/Error");
                // TODO: https://aka.ms/aspnetcore-hsts
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            if (IsDevelopment)
            {
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "swagger";
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                });
            }

            if (DsConfig.StaticRoutes != null && DsConfig.StaticRoutes.Count > 0)
            {
                app.UseRouter(BuildRouter(app, DsConfig.StaticRoutes.ToArray()));
            }

if (DsConfig.HostWebApi)
{
    app.UseMvcWithDefaultRoute();
}
else
{
    app.UseMvc();
}

app.UseRouting();

if (!IsDevelopment)
{
    app.UseAuthorization();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

            logger.LogDebug("Configure() complete: " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
        }

        //TODO:                    app.UseExceptionHandler();
        //TODO: notfound?

private IRouter BuildRouter(IApplicationBuilder applicationBuilder, string[] staticRoutes)
{
    RouteBuilder routeBuilder = new RouteBuilder(applicationBuilder);

    foreach (string route in staticRoutes)
    {
        routeBuilder.MapMiddlewareGet(route, appBuilder => { appBuilder.UseStaticFiles(); });
    }



//        routeBuilder.MapMiddlewareGet("/lib/{*path}", appBuilder => { appBuilder.UseStaticFiles(); });
//    routeBuilder.MapMiddlewareGet("/css/{*path}", appBuilder => { appBuilder.UseStaticFiles(); });
//    routeBuilder.MapMiddlewareGet("/js/{*path}", appBuilder => { appBuilder.UseStaticFiles(); });
//    routeBuilder.MapMiddlewareGet("/img/{*path}", appBuilder => { appBuilder.UseStaticFiles(); });
//    routeBuilder.MapMiddlewareGet("/favicon.ico", appBuilder => { appBuilder.UseStaticFiles(); });
    return routeBuilder.Build();
}
    }
}
