using HealthSync.MiddleWare;
using Hospital_API.Domain.Model;
using Hospital_API.Infrastructure.ApplicationDbContext;
using Hospital_API.Infrastructure.PresceptionService;
using Hospital_API.Infrastructure.Services;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//. add json converter to make the string of the values for the enum appearing in the swagger --> very important 
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

//. the settings for the LockOut 
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    //. limited access failed to login 
    options.Lockout.MaxFailedAccessAttempts = 5;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Connections")));

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRegisterationProcessesRepository, RegisterationProcessesRepository>();
builder.Services.AddScoped<ITokenIPService, TokenIPService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISeedingSuperAdminRoleService, SeedingSuperAdminRoleService>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorAuthService, DoctorAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAppoinmentRepository, AppoinmentRepository>();
builder.Services.AddScoped<INurseRepository, NurseRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmergencyContactsRepository, EmergencyContactsRepository>();
builder.Services.AddScoped<IPresceptionRepository, prescriptionRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Logging.AddConsole();

//.for the sample dependency not for the DB 
builder.Services.AddTransient<ISideService, SideService>();

//. Email Settings 
builder.Services.AddFluentEmail(builder.Configuration["SMTP:From"], "HealthSync").AddRazorRenderer()
    .AddSmtpSender(new System.Net.Mail.SmtpClient
    {
        Host = builder.Configuration["SMTP:Host"],
        Port = int.Parse(builder.Configuration["SMTP:Port"]),
        EnableSsl = true,
        Credentials = new NetworkCredential(builder.Configuration["SMTP:From"], builder.Configuration["SMTP:Password"])
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true; ///. Unique email
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

//. Cors settings 
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    //. taking the project NewRoleName and adding .xml --> (ITI_Tutorial_Task_Web_API.xml) 
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    //. the Whole Path that existing on my device and /ProjectName.xml --> (C:\Users\user\source\repos\ITI_Tutorial_Task_Web_API\bin\Debug\net8.0\ITI_Tutorial_Task_Web_API.xml)
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    Console.WriteLine(xmlPath);

    //. telling swagger to use the XML File and reading the comments of type XML Comments 
    options.IncludeXmlComments(xmlPath);

    //.making the swagger open the button of the Authorization that making me can test the APIs from the Swagger not from Postman 
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Token: Bearer {your token}" //. there is a space between the token and Bearer word 
    });


    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{} //. putting the token in the atray of string or all
        }
    });
    options.UseInlineDefinitionsForEnums();
});

var app = builder.Build();

//. registeratoin for the seeding super Admin 
using (var scope = app.Services.CreateScope())
{
    var SeedingService = scope.ServiceProvider.GetRequiredService<ISeedingSuperAdminRoleService>();
    await SeedingService.SeedingSuperAdmin();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCors("MyPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalExceptionMiddleWare>();

app.Run();
