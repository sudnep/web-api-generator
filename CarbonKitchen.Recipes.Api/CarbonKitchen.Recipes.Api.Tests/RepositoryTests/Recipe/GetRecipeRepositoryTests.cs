namespace CarbonKitchen.Recipes.Api.Tests.RepositoryTests
{
    using FluentAssertions;
    using CarbonKitchen.Recipes.Api.Data;
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using CarbonKitchen.Recipes.Api.Services;
    using CarbonKitchen.Recipes.Api.Services.Recipe;
    using CarbonKitchen.Recipes.Api.Tests.Fakes.Recipe;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Linq;
    using Xunit;

    [Collection("Sequential")]
    public class GetRecipeRepositoryTests
    {
        [Fact]
        public void GetRecipe_ParametersMatchExpectedValues()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipe = new FakeRecipe { }.Generate();

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipe);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                //Assert
                var recipeById = context.Recipes.FirstOrDefault(r => r.RecipeId == fakeRecipe.RecipeId);

                recipeById.Should().BeEquivalentTo(fakeRecipe);
                recipeById.RecipeId.Should().Be(fakeRecipe.RecipeId);
                recipeById.RecipeTextField1.Should().Be(fakeRecipe.RecipeTextField1);
                recipeById.RecipeTextField2.Should().Be(fakeRecipe.RecipeTextField2);
                recipeById.RecipeDateField1.Should().Be(fakeRecipe.RecipeDateField1);
            }
        }

        [Fact]
        public void GetRecipes_CountMatchesAndContainsEvuivalentObjects()
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
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto());

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(3);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeOne);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ReturnExpectedPageSize()
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
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { PageSize = 2 });

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(2);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeOne);
                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ReturnExpectedPageNumberAndSize()
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
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { PageSize = 1, PageNumber = 2 });

                //Assert
                recipeRepo.Should()
                    .NotBeEmpty()
                    .And.HaveCount(1);

                recipeRepo.Should().ContainEquivalentOf(fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ListSortedInAscOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.RecipeTextField1 = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.RecipeTextField1 = "Alpha";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.RecipeTextField1 = "Charlie";

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { SortOrder = "RecipeTextField1" });

                //Assert
                recipeRepo.Should()
                    .ContainInOrder(fakeRecipeTwo, fakeRecipeOne, fakeRecipeThree);

                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetRecipes_ListSortedInDescOrder()
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.RecipeTextField1 = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.RecipeTextField1 = "Alpha";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.RecipeTextField1 = "Charlie";

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { SortOrder = "-RecipeTextField1" });

                //Assert
                recipeRepo.Should()
                    .ContainInOrder(fakeRecipeThree, fakeRecipeOne, fakeRecipeTwo);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("RecipeTextField1 == Alpha")]
        [InlineData("RecipeTextField2 == Bravo")]
        [InlineData("RecipeIntField1 == 5")]
        [InlineData("RecipeTextField1 == Charlie")]
        [InlineData("RecipeTextField2 == Delta")]
        [InlineData("RecipeIntField1 == 6")]
        [InlineData("RecipeTextField1 == Echo")]
        [InlineData("RecipeTextField2 == Foxtrot")]
        [InlineData("RecipeIntField1 == 7")]
        public void GetRecipes_FilterListWithExact(string filters)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.RecipeTextField1 = "Alpha";
            fakeRecipeOne.RecipeTextField2 = "Bravo";
            fakeRecipeOne.RecipeIntField1 = 5;

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.RecipeTextField1 = "Charlie";
            fakeRecipeTwo.RecipeTextField2 = "Delta";
            fakeRecipeTwo.RecipeIntField1 = 6;

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.RecipeTextField1 = "Echo";
            fakeRecipeThree.RecipeTextField2 = "Foxtrot";
            fakeRecipeThree.RecipeIntField1 = 7;

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { Filters = filters });

                //Assert
                recipeRepo.Should()
                    .HaveCount(1);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("RecipeTextField1@=Hart", 1)]
        [InlineData("RecipeTextField2@=Fav", 1)]
        [InlineData("RecipeTextField1@=*hart", 2)]
        [InlineData("RecipeTextField2@=*fav", 2)]
        public void GetRecipes_FilterListWithContains(string filters, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.RecipeTextField1 = "Alpha";
            fakeRecipeOne.RecipeTextField2 = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.RecipeTextField1 = "Hartsfield";
            fakeRecipeTwo.RecipeTextField2 = "Favaro";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.RecipeTextField1 = "Bravehart";
            fakeRecipeThree.RecipeTextField2 = "Jonfav";

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { Filters = filters });

                //Assert
                recipeRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }

        [Theory]
        [InlineData("hart", 1)]
        [InlineData("fav", 1)]
        [InlineData("Fav", 0)]
        public void GetRecipes_SearchQueryReturnsExpectedRecordCount(string queryString, int expectedCount)
        {
            //Arrange
            var dbOptions = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseInMemoryDatabase(databaseName: $"RecipeDb{Guid.NewGuid()}")
                .Options;
            var sieveOptions = Options.Create(new SieveOptions());

            var fakeRecipeOne = new FakeRecipe { }.Generate();
            fakeRecipeOne.RecipeTextField1 = "Alpha";
            fakeRecipeOne.RecipeTextField2 = "Bravo";

            var fakeRecipeTwo = new FakeRecipe { }.Generate();
            fakeRecipeTwo.RecipeTextField1 = "Hartsfield";
            fakeRecipeTwo.RecipeTextField2 = "White";

            var fakeRecipeThree = new FakeRecipe { }.Generate();
            fakeRecipeThree.RecipeTextField1 = "Bravehart";
            fakeRecipeThree.RecipeTextField2 = "Jonfav";

            //Act
            using (var context = new RecipeDbContext(dbOptions))
            {
                context.Recipes.AddRange(fakeRecipeOne, fakeRecipeTwo, fakeRecipeThree);
                context.SaveChanges();

                var service = new RecipeRepository(context, new SieveProcessor(sieveOptions));

                var recipeRepo = service.GetRecipes(new RecipeParametersDto { QueryString = queryString });

                //Assert
                recipeRepo.Should()
                    .HaveCount(expectedCount);

                context.Database.EnsureDeleted();
            }
        }
    }
}
