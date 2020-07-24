namespace CarbonKitchen.Recipes.Api.Configuration
{
    using AutoMapper;
    using CarbonKitchen.Recipes.Api.Data.Entities;
    using CarbonKitchen.Recipes.Api.Models.Recipe;

    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            //createmap<to this, from this>
            CreateMap<Recipe, RecipeDto>()
                .ReverseMap();
            CreateMap<RecipeForCreationDto, Recipe>();
            CreateMap<RecipeForUpdateDto, Recipe>()
                .ReverseMap();
        }
    }
}