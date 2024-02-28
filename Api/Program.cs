using Application;
using Domain;
using Infrastructure;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    //Concatenando 2 autenticacao
    //options.AddPolicy("Admin", policy => policy.RequireRole("Admin").RequireClaim("id", "usuarioteste");


    //options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
    //    context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "usuarioteste") || context.User.IsInRole("Admin")));
});

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddDomain();
builder.Services.AddInfrastruture(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
    { 
       {

                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                    },
                new string[] {}
                } 
    }
);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseAuthorization();
app.Run();
