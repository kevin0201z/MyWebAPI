using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWebAPI.Model;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyWebAPI
{
    /// <summary>
    /// 服务扩展方法类
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Api版本信息
        /// </summary>
        public static IApiVersionDescriptionProvider provider;

        /// <summary>
        /// 跨域
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod()
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowCredentials();
                }));
        }

        /// <summary>
        /// Swagger支持
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // 多版本控制
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    // 添加文档信息
                    c.SwaggerDoc(item.GroupName, new OpenApiInfo
                    {
                        Title = "CoreWebApi",
                        Version = item.ApiVersion.ToString(),
                        Description = "ASP.NET CORE WebApi",
                        Contact = new OpenApiContact
                        {
                            Name = "Abc",
                            Email = "abc@gmail.com",
                            Url = new Uri("http://t.abc.com/")
                        }
                    });
                }
                // 在 Swagger 文档显示的 API 地址中将版本信息参数替换为实际的版本号
                c.DocInclusionPredicate((version, apiDescription) =>
                {
                    if (!version.Equals(apiDescription.GroupName))
                        return false;

                    var values = apiDescription.RelativePath
                        .Split('/')
                        .Select(v => v.Replace("v{version}", apiDescription.GroupName)); apiDescription.RelativePath = string.Join("/", values);
                    return true;
                });


                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                #region JWT认证Swagger授权
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头header中进行传输) 直接在下框中输入Bearer {token}（中间是空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                #endregion
            });
        }
        /// <summary>
        /// JWT验证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="token"></param>
        public static void ConfigureJWT(this IServiceCollection services, TokenManagement token)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = token.Audience,
                    ValidIssuer = token.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token.Secret))
                };
            });
        }

        public static void ConfigureApiVersion(this IServiceCollection services)
        {
            services.AddApiVersioning(option =>
            {
                // 可选，为true时API返回支持的版本信息
                option.ReportApiVersions = true;
                // 不提供版本时，默认为1.0
                option.AssumeDefaultVersionWhenUnspecified = true;
                // 请求中未指定版本时默认为1.0
                option.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddVersionedApiExplorer(option =>
            {　　　　　　　　　　// 版本名的格式：v+版本号
                option.GroupNameFormat = "'v'V";
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
        }

        public static void ConfigureUseSwaggerUI(this IApplicationBuilder app)
        {
            app.UseSwaggerUI(c =>
            {
                foreach (var item in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{item.GroupName}/swagger.json", "CoreAPI" + item.ApiVersion);
                }

                c.RoutePrefix = "";// 如果设为空，访问路径就是根域名/index.html，设置为空，表示直接在根域名访问；想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html
            });
        }
    }
}
