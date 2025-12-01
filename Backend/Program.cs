using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    _ = builder.Configuration.GetValue<bool>("UseInMemory") ?
        options.UseInMemoryDatabase("InMemoryDB") :
        options.UseSqlServer(builder.Configuration["DefaultConnection"]);
});

// IDENTITY
builder.Services.AddIdentity<IdentityUserEx, IdentityRole>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        //Disable Security Shit
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireLowercase = false;
        options.Lockout.MaxFailedAccessAttempts = int.MaxValue;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(0);
    }
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT AUTH
var jwtKey = builder.Configuration["JWT_KEY"] ?? throw new NullReferenceException("JWT Key is null!");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "UnkownIssuer";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "UnkownAudience";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.Zero,
    };
});

// SWAGGER WITH JWT
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(swagger =>
    {
        swagger.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Enter 'Bearer {token}'"
        });

        swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } },
            Array.Empty<string>()
        }
    });
    });
}

// CORS Allow All
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddControllers();

var app = builder.Build();

// SEED Dummy users
if(builder.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var identities = new List<IdentityUserEx>();
        for (int i = 0; i < 100; i++)
        {
            identities.Add(new IdentityUserEx
            {
                UserName = $"user-{i}",
            });
        }

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Users.AddRange(identities);
        await context.SaveChangesAsync(); // One SaveChanges for all 100 users
    }
}

// SEED ADMIN (TESTING)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserEx>>();

    const string adminRole = "ADMIN";
    const string adminUserName = "ADMIN";
    const string adminEmail = "admin@admin.com";
    const string adminPassword = "1";

    // Ensure ADMIN role exists
    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    // Ensure ADMIN user exists
    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if (adminUser == null)
    {
        adminUser = new IdentityUserEx
        {
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}

// MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();