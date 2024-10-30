using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.GatewaySolution.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var apiSettings = builder.Configuration.GetSection("ApiSettings");

            var secret = Environment.GetEnvironmentVariable("API_SECRET_KEY", EnvironmentVariableTarget.User) ?? string.Empty;
            var issuer = apiSettings.GetValue<string>("Issuer");
            var audience = apiSettings.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = issuer,
                    ValidateIssuer = true,
                    ValidAudience = audience,
                    ValidateAudience = true
                };
            });


            return builder;
        }
    }
}
