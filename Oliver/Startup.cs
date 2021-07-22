using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Oliver.BackgroundServices;
using Oliver.Data;
using Oliver.Domain.Config;
using Oliver.Exceptions;
using Oliver.Services;
using Oliver.Services.Interfaces;

namespace Oliver {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Config options from appsettings.json
			services.Configure<YtsOptions>(Configuration.GetSection(YtsOptions.SectionName));
			services.Configure<FoldersOptions>(Configuration.GetSection(FoldersOptions.SectionName));

			services.AddHttpClient();

			services.AddDbContext<OliverContext>();

			services.AddControllers(options => {
				options.Filters.Add(typeof(ExceptionFilter));
			});

			// Allow requests from react app
			services.AddCors(options => {
				options.AddDefaultPolicy(builder => {
					builder
						.AllowAnyOrigin()
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			services.AddSwaggerGen(c => {
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Oliver", Version = "v1" });
			});

			services.AddScoped<ICleanupService, CleanupService>();
			services.AddScoped<IHashService, HashService>();
			services.AddScoped<ITorrentService, TorrentService>();
			services.AddScoped<IYtsService, YtsService>();

			// Background services
			services.AddHostedService<FileProcessingService>();
			services.AddHostedService<FileHashingService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oliver v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}
	}
}
