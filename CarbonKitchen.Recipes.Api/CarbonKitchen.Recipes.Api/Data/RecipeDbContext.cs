namespace CarbonKitchen.Recipes.Api.Data
{
    using CarbonKitchen.Recipes.Api.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes { get; set; }
    }
}
