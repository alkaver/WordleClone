using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CalendarAPI;
using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,  
        Scheme = "Bearer"                  
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey 
            },
            new List<string>()
        }
    });
});


var keyVaultUrl = builder.Configuration["KeyVault:KeyVaultURL"];
var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential());

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration["ProdConnection"]));
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnection")));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();   // Zezwalaj na dowolne metody (GET, POST, PUT itd.)
        });
});

//login
builder.Services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTSetting:ValidIssuer"],
        ValidAudience = builder.Configuration["JWTSetting:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecurityKey"]))
    };
});



// Dodaj autoryzację
builder.Services.AddAuthorization();
builder.Services.AddHostedService<StreakResetService>();
builder.Services.AddHostedService<WordOfTheDayBackgroundService>();

var app = builder.Build();

//using IServiceScope scope = app.Services.CreateScope();
//using AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
app.UseSwagger();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if(db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await SeedAdminAsync(userManager, roleManager, configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowSpecificOrigins");


app.MapControllers();

app.Run();

static async Task SeedAdminAsync(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration)
{
    string adminEmail = configuration["AdminEmail"];
    string adminPassword = configuration["AdminPassword"];

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        throw new InvalidOperationException("Admin credentials are missing in configuration.");

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            throw new Exception($"Admin creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
