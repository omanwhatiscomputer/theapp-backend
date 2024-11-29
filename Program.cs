using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;


// setting Development ENV variables
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"){
    var cwd = Directory.GetCurrentDirectory();
    DotEnv.Load(Path.Combine(cwd, ".env"));
}


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://theapp-frontend.vercel.app")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// apply the default auth schemes and tell jwt what we want to authenticate
builder.Services.AddAuthentication(x => 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=> {
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
    
});
builder.Services.AddAuthorization();


builder.Services.AddControllers();



builder.Services.AddDbContext<UserDbContext>(opt =>
{
    // opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRESQL_CONN_STRING"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());





var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowSpecificOrigin");


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
