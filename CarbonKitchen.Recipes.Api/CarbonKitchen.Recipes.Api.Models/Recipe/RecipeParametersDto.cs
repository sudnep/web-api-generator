namespace CarbonKitchen.Recipes.Api.Models.Recipe
{
    using CarbonKitchen.Recipes.Api.Models.Pagination;

    public class RecipeParametersDto : RecipePaginationParameters
    {
        public string Filters { get; set; }
        public string QueryString { get; set; }
        public string SortOrder { get; set; }
    }
}
