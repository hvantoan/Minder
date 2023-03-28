using Coravel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minder.Database;
using Minder.Service.Hubs;
using Minder.Service.Implements;
using Minder.Service.Interfaces;
using Minder.Service.Jobs;
using Minder.Service.Models;
using Minder.Service.Models.Chat;
using Minder.Services.Common;
using Minder.Services.Helpers;
using Minder.Services.Implements;
using Minder.Services.Interfaces;
using Minder.Services.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TuanVu.Services.Common;

namespace Minder.Api {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddCors(options => {
                options.AddDefaultPolicy(builder => {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddMemoryCache();

            services.AddDbContext<MinderContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString(nameof(MinderContext))!)
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        RoleClaimType = "1",
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JwtSecret"]!))
                    };
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            var accessToken = context.Request.Query["accessToken"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/chat"))) {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options => options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
               .RequireAuthenticatedUser().Build());

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v0.1", new OpenApiInfo { Title = "Minder.Api", Version = "v0.1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Description = @"API KEY",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddScoped(provider => {
                var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                if (httpContext != null) {
                    string? url = UrlHelper.GetCurrentUrl(httpContext.Request, Configuration, "ImageUrl");
                    return new CurrentUser {
                        UserId = httpContext.User?.FindFirst(o => o.Type == Constant.TokenUserId)?.Value ?? string.Empty,
                        Url = url,
                    };
                }
                return new CurrentUser();
            });

            services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                  new[] { MediaTypeNames.Application.Octet }));

            services.AddStackExchangeRedisCache(options => {
                options.Configuration = Configuration.GetSection("RedisCacheSettings:ConnectionString").Value;
                options.InstanceName = Configuration.GetSection("RedisCacheSettings:InstanceName").Value;
            });

            services.AddHttpContextAccessor();
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            }).AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddSingleton<AdministrativeUnitResource>();
            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IAuthService, AuthService>()
                    .AddScoped<IEmailService, EmailService>()
                    .AddScoped<ICacheManager, CacheManager>()
                    .AddScoped<ITeamService, TeamService>()
                    .AddScoped<IFileService, FileService>()
                    .AddScoped<IAppInfoService, AppInfoService>()
                    .AddScoped<IStadiumService, StadiumService>()
                    .AddScoped<IConversationService, ConversationService>()
                    .AddScoped<IInviteSevice, InviteService>()
                    .AddSingleton<IDictionary<string, Connection>>(new Dictionary<string, Connection>());

            services.AddScheduler();
            services.AddScoped<ExpireInvitation>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseResponseCompression();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Minder.Api v0.1"));

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<ChatService>("/hubs/chat");
            });
            AutoMigrate(app);

            app.ApplicationServices.UseScheduler(scheduler => {
                scheduler.Schedule<ExpireInvitation>().DailyAt(0, 0).RunOnceAtStart();
            }).OnError(ex => Logger.System().Error("Scheduler ERROR", ex));
        }

        private static void AutoMigrate(IApplicationBuilder app) {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MinderContext>();
            dbContext.Database.Migrate();
        }
    }
}