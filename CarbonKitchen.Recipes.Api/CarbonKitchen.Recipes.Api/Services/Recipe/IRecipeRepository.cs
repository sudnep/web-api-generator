namespace CarbonKitchen.Recipes.Api.Services.Recipe
{
    using CarbonKitchen.Recipes.Api.Data.Entities;
    using CarbonKitchen.Recipes.Api.Models.Pagination;
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using System.Threading.Tasks;

    public interface IRecipeRepository
    {
        PagedList<Recipe> GetRecipes(RecipeParametersDto recipeParameters);
        Task<Recipe> GetRecipeAsync(int recipeId);
        Recipe GetRecipe(int recipeId);
        void AddRecipe(Recipe recipe);
        void DeleteRecipe(Recipe recipe);
        void UpdateRecipe(Recipe recipe);
        bool Save();
    }
}
