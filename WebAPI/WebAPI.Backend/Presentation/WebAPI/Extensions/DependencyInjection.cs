using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace WebAPI.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Managment & Monitoring - WebApi",
					Description = "This API will be responsible for receiving data (metrics, configurations, states, etc.) and distributing them, as well as for authorization.",
					
					Contact = new OpenApiContact
					{
						Name = "Daniel",
						Email = "daniil.nyashka@gmail.com",
						Url = new Uri("https://t.me/C9Morty"),
					}
				});
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer",
							},
							Scheme = "Bearer",
							Name = "Bearer",
							In = ParameterLocation.Header,
						}, new List<string>()
					},
				});
			});
			return services;
		}

		public static IServiceCollection AddControllersExtension(this IServiceCollection services)
		{
			services.AddControllers()
				.AddJsonOptions(options =>
				{
					options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.KebabCaseLower;
					options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower;
				});
			return services;
		}

		public static IServiceCollection AddCorsExtension(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
				builder =>
				{
					builder.AllowAnyOrigin()
						   .AllowAnyHeader()
						   .AllowAnyMethod();
				});
			});
			return services;
		}
		public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.Authority = configuration["Sts:ServerUrl"];
					options.Audience = configuration["Sts:Audience"];
				});
			return services;
		}
		public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
		{
			string superAdmin = configuration["ApiRoles:AdminRole"],
			admin = configuration["ApiRoles:ManagerRole"],
			user = configuration["ApiRoles:EmployeeRole"];

			services.AddAuthorization(options =>
			{
				options.AddPolicy(Authorization.SuperAdminPolicy, policy => policy.RequireRole(superAdmin));
				options.AddPolicy(Authorization.AdminPolicy, policy => policy.RequireRole(admin, superAdmin));
				options.AddPolicy(Authorization.UserPolicy, policy => policy.RequireRole(user, admin, superAdmin));
			});
			return services;
		}
	}
}