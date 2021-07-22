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
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			// Config options from appsettings.json
			_ = services
				.Configure<YtsOptions>(Configuration.GetSection(YtsOptions.SectionName))
				.Configure<FoldersOptions>(Configuration.GetSection(FoldersOptions.SectionName));

			_ = services.AddHttpClient();

			_ = services.AddDbContext<OliverContext>();

			_ = services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));

			// Allow requests from react app
			_ = services.AddCors(options =>
					options.AddDefaultPolicy(builder =>
						builder
							.AllowAnyOrigin()
							.AllowAnyHeader()
							.AllowAnyMethod()));

			services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Oliver", Version = "v1" }));

			_ = services
				.AddScoped<ICleanupService, CleanupService>()
				.AddScoped<IHashService, HashService>()
				.AddScoped<ITorrentService, TorrentService>()
				.AddScoped<IYtsService, YtsService>();

			// Background services
			services.AddHostedService<FileProcessingService>();
			services.AddHostedService<FileHashingService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				_ = app
					.UseDeveloperExceptionPage()
					.UseSwagger()
					.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oliver v1"));
			}

			_ = app
				.UseHttpsRedirection()
				.UseRouting()
				.UseCors()
				.UseAuthorization()
				.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}
