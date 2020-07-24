namespace CarbonKitchen.Recipes.Api.Validators.Recipe
{
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RecipeForCreationDtoValidator : RecipeForManipulationDtoValidator<RecipeForCreationDto>
    {
        public RecipeForCreationDtoValidator()
        {
        }
    }
}
