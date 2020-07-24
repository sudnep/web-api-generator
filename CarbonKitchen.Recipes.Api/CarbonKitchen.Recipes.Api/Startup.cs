namespace CarbonKitchen.Recipes.Api
{
    using AutoBogus;
    using Autofac;
    using AutoMapper;
    using CarbonKitchen.Recipes.Api.Data;
    using CarbonKitchen.Recipes.Api.Data.Entities;
    using CarbonKitchen.Recipes.Api.Services.Recipe;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sieve.Services;
    using System;
    using System.Reflection;

    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "MyCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<SieveProcessor>();
            
            services.AddScoped<IRecipeRepository, RecipeRepository>();

            services.AddMvc();
            //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddDbContext<RecipeDbContext>(opt => 
                opt.UseInMemoryDatabase("RecipeDb"));

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html
        public void ConfigureContainer(ContainerBuilder builder)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var context = app.ApplicationServices.GetService<RecipeDbContext>())
            {
                context.Database.EnsureCreated();

                // auto generate some fake data. added rules to accomodate placeholder validation rules
                context.Recipes.Add(new AutoFaker<Recipe>()
                    .RuleFor(fake => fake.RecipeDateField1, fake => fake.Date.Past())
                    .RuleFor(fake => fake.RecipeIntField1, fake => fake.Random.Number()));
                context.Recipes.Add(new AutoFaker<Recipe>()
                    .RuleFor(fake => fake.RecipeDateField1, fake => fake.Date.Past())
                    .RuleFor(fake => fake.RecipeIntField1, fake => fake.Random.Number()));
                context.Recipes.Add(new AutoFaker<Recipe>()
                    .RuleFor(fake => fake.RecipeDateField1, fake => fake.Date.Past())
                    .RuleFor(fake => fake.RecipeIntField1, fake => fake.Random.Number()));

                context.SaveChanges();
            }
        }
    }
}
