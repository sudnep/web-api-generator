namespace CarbonKitchen.Recipes.Api.Models.Recipe
{
    using System;

    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public int? RecipeIntField1 { get; set; }
        public string RecipeTextField1 { get; set; } 
        public string RecipeTextField2 { get; set; }
        public DateTime? RecipeDateField1 { get; set; }
    }
}
