namespace CarbonKitchen.Recipes.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Recipes.Api.Data;
    using CarbonKitchen.Recipes.Api.Services;
    using CarbonKitchen.Recipes.Api.Services.Recipe;
    using CarbonKitchen.Recipes.Api.Tests.Fakes.Recipe;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    [Collection("Sequential")]
    public class DeleteRecipeRepositoryTests
    {
        [Fact]
        public void DeleteRecipe_ReturnsProperCount()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            var fakeRecipeThree = new FakeRecipe { }.Generate();

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));
                service.DeleteRecipe(fakeRecipeTwo);

                context.SaveChanges();

                //Assert
                var recipeList = context.Recipes.ToList();

                recipeList.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                recipeList.Should().ContainEquivalentOf(fakeRecipeOne);
                recipeList.Should().ContainEquivalentOf(fakeRecipeThree);
                Assert.DoesNotContain(recipeList, r => r == fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }
    }
}
