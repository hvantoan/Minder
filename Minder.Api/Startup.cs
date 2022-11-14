using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minder.Database;
using Minder.Service.Hubs;
using Minder.Service.Models.Chat;
using Minder.Services.Implements;
using Minder.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace Minder.Api {
    public class Startup {

        const string CorsPolicy = nameof(CorsPolicy);
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services) {

            services.AddCors(options => {
                options.AddPolicy(
                    name: CorsPolicy,
                    builder => {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddDbContext<MinderContext>(options =>
               options.UseNpgsql(Configuration.GetConnectionString(nameof(MinderContext))), ServiceLifetime.Transient);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = true,
                        RoleClaimType = "1",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JwtSecret"]))
                    };
                });

            services.AddAuthorization(options => options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
               .RequireAuthenticatedUser().Build());

            services.AddControllers().AddNewtonsoftJson();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v0", new OpenApiInfo { Title = "Minder.Api", Version = "v0.1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Description = @"API KEY",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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

            services.AddResponseCompression(options => options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                  new[] { MediaTypeNames.Application.Octet }));

            services.AddHttpContextAccessor();
            services.AddSignalR(options => options.EnableDetailedErrors = true);

            services.AddScoped<IUserService, UserService>()
                    .AddScoped<IAuthService, AuthService>()
                    .AddSingleton<IDictionary<string, UserRoom>>(new Dictionary<string, UserRoom>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseResponseCompression();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v0/swagger.json", "Minder.Api v0.1"));

            app.UseRouting();
            app.UseCors(CorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<ChatService>("/chat");
            });
        }
    }
}
