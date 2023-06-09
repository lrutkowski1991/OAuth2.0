using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            //ValidIssuer = "AuthorizationServer",
            //ValidAudience = "Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HereIsTheSecretKeyForJWTBearerForAuthenticationClient"))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Get token from header Authorization
                string authorizationHeader = context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Add authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
