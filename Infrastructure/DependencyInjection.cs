

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Identity.Entity;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastruture(this IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration.GetConnectionString("Database");
            var secretKey = configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key!!");

            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //Challenge => caso não envie o token, ira ter que fazer o "desafio" no caso, login
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true; //Se deseja salvar o token, caso seja valido
                options.RequireHttpsMetadata = false; //Se é preciso https para transmitir o token, em produção deixa sim
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT: ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });


        }

    }
}
