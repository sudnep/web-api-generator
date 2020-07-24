namespace CarbonKitchen.Recipes.Api.Tests.Fakes.Recipe
{
    using AutoBogus;
    using CarbonKitchen.Recipes.Api.Models.Recipe;

    // or replace 'AutoFaker' with 'Faker' if you don't want all fields to be auto faked
    public class FakeRecipeDto : AutoFaker<RecipeDto>
    {
        public FakeRecipeDto()
        {
            // leaving the first 49 for potential special use cases in startup builds that need explicit values
            RuleFor(r => r.RecipeId, r => r.Random.Number(50, 100000));
            RuleFor(r => r.RecipeIntField1, r => r.Random.Number(0, 1000000)); // example validation rul says that it needs be above 0
            RuleFor(r => r.RecipeDateField1, r => r.Date.Past()); // example validation rule says it has to be in the past
        }
    }
}
