using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Boilerplate.Api.ActionFilters;
using Boilerplate.Api.Middleware;
using Boilerplate.Api.Utils;
using Boilerplate.Api.Utils.Swagger;
using Boilerplate.DAL;
using Boilerplate.DAL.Entities;
using Boilerplate.Models;
using Boilerplate.Services.Abstractions;
using Boilerplate.Services.Implementations;
using Boilerplate.Services.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace Boilerplate.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;

            logger.LogInformation("Application started");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseInMemoryDatabase("db"); //for testing purposes only
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["App:Auth:Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    //these needed for allow signalr pass bearer token authentication
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hubs")))
                                context.Token = accessToken;

                            return Task.CompletedTask;
                        }
                    };

                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("JWT", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddMvc(options =>
                {            
                    options.Filters.Add(typeof(ModelValidatorActionFilter));
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(BaseResponse), StatusCodes.Status400BadRequest));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddVersionedApiExplorer(  
                options =>  
                {                    
                    options.GroupNameFormat = "'v'VVV";  
                    options.SubstituteApiVersionInUrl = true;  
                }); ;  
            
            services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1, 0); });

            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Info  
                    {  
                        Title = $"Project API {description.ApiVersion}",  
                        Version = description.ApiVersion.ToString(),  
                        Description = description.IsDeprecated ? $"Project API {description.ApiVersion} - DEPRECATED" : $"Project API {description.ApiVersion}"
                    });  
                }
                
                options.OperationFilter<SwaggerDefaultValues>();
                options.OperationFilter<SwaggerFileUploadFilter>();
                options.OperationFilter<SwaggerAuthorizedFilter>();
  
                options.IncludeXmlComments(Path.Combine(   
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{GetType().Assembly.GetName().Name}.xml"  
                ));

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "Header",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });

            services.AddAutoMapper(config =>
            {
                config.AddProfile<AutomapperProfile>();
            });

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            //application services
            services.AddTransient<IAuthService, DefaultJwtAuthService>();
            services.AddTransient<IUploadsService, DefaultUploadsService>();
            services.AddTransient<IEmailService, MailKitEmailService>();
            services.AddTransient<IRolesService, DefaultRolesService>();

            //chat services
            services.AddSignalR();
            services.AddTransient<IChatProvider, SignalrChatProvider>();
            services.AddTransient<IChatConnectionsStore, InMemoryChatConnectionStore>();
            services.AddTransient<IChatService, ChatService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IApiVersionDescriptionProvider provider)
        {
            //Empty response error hack-fix (seems fixed in new asp.net core versions)
            //app.Use(async (ctx, next) =>
            //{
            //    await next();
            //    if (ctx.Response.StatusCode == 204)
            //    {
            //        ctx.Response.ContentLength = 0;
            //    }
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //exception handling with middleware
            app.ConfigureExceptionHandler();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    //Build a swagger endpoint for each discovered API version  
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }

                    options.RoutePrefix = "";
                });

            // FOR ANGULAR
            app.UseCors(builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials()
            );

            app.UseSignalR(routes =>
            {
                routes.MapHub<MainHub>("/hubs/main");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataSeeder.Seed(context, userManager, roleManager);
        }
    }
}
