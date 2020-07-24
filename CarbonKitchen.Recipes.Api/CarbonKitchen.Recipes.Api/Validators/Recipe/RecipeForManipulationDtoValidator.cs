namespace CarbonKitchen.Recipes.Api.Validators.Recipe
{
    using FluentValidation;
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using System;

    public class RecipeForManipulationDtoValidator<T> : AbstractValidator<T> where T : RecipeForManipulationDto
    {
        public RecipeForManipulationDtoValidator()
        {
            RuleFor(r => r.RecipeTextField1)
                .NotEmpty();
            RuleFor(r => r.RecipeIntField1)
                .GreaterThanOrEqualTo(0);
            RuleFor(r => r.RecipeDateField1)
                .LessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
