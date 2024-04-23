
using Aspose.Cells.Charts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using stage_api.configuration;
using stage_api.Models;
using System.Configuration;
using System.Text;

namespace stage_api
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

            var startup = new Startup(builder.Configuration);

            startup.ConfigureServices(builder.Services);

            //         builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

            //builder.Services.AddDefaultIdentity<ApplicationUser>()
            //             .AddRoles<IdentityRole>()
            //             .AddEntityFrameworkStores<dbContext>();
            //         builder.Services.AddCors();

            //Jwt Authentication

            //var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"]?.ToString());
            //builder.Services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = false;
            //    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ClockSkew = TimeSpan.Zero

            //    };
            //});

            // Add services to the container.
            builder.Services.AddControllers();
			//builder.Services.AddScoped<AuthenticationService>();
			builder.Services.AddScoped<DataProcessingService>();
			builder.Services.AddScoped<FileUploadService>();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			//builder.Services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowSpecificOrigin",
			//		builder =>
			//		{
			//			builder.WithOrigins("http://localhost:4200")
			//				   .AllowAnyMethod()
			//				   .AllowAnyHeader();
			//		});
			//});

			// Set ExcelPackage license context
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

			

   //         builder.Services.AddDbContext<dbContext>(options =>
			//{
			//	options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
			//	Console.WriteLine("Database connection established successfully.");
			//});

			var app = builder.Build();

            startup.Configure(app, app.Lifetime);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

            app.UseCors(builder =>
            builder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();
			app.UseCors("AllowSpecificOrigin");
			app.MapControllers();
			// Additional controller to test database connection

			app.MapGet("/DataBaseConnection", () => "Database connection established successfully.");



			app.Run();
		}
	}
}
