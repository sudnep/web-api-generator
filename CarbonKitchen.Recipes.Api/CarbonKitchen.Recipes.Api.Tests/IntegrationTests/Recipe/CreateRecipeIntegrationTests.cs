namespace CarbonKitchen.Recipes.Api.Tests.IntegrationTests.Recipe
{
    using CarbonKitchen.Recipes.Api.Tests.Fakes.Recipe;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Newtonsoft.Json;
    using System.Net.Http;
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using FluentAssertions;
    using System.Dynamic;
    using FluentValidation.Results;

    [Collection("Sequential")]
    public class CreateRecipeIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public CreateRecipeIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PostRecipeReturnsSuccessCodeAndResourceWithAccurateFields()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var fakeRecipe = new FakeRecipeDto().Generate();

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/recipes", fakeRecipe)
                .ConfigureAwait(false);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var resultDto = JsonConvert.DeserializeObject<RecipeDto>(await httpResponse.Content.ReadAsStringAsync()
                .ConfigureAwait(false));

            httpResponse.StatusCode.Should().Be(201);
            resultDto.RecipeTextField1.Should().Be(fakeRecipe.RecipeTextField1);
            resultDto.RecipeTextField2.Should().Be(fakeRecipe.RecipeTextField2);
            resultDto.RecipeDateField1.Should().Be(fakeRecipe.RecipeDateField1);
        }

        [Fact]
        public async Task PostInvalidRecipeTextField1ReturnsBadRequestCode()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            
            var invalidRecipe = new FakeRecipeDto().Generate();
            invalidRecipe.RecipeTextField1 = null;

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/recipes", invalidRecipe)
                .ConfigureAwait(false);

            // add something like this to read the errors in the body?
            //var body = JsonConvert.DeserializeObject<ValidationResult>(await httpResponse.Content.ReadAsStringAsync()
            //    .ConfigureAwait(false));

            // Assert
            httpResponse.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PostInvalidRecipeDateField1ReturnsBadRequestCode()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // intentionally bad date field, can't use normal object because c# will yell about the date value not being valid
            var invalidRecipe = new
            {
                RecipeId = 1783336605,
                RecipeIntField1 = 0,
                RecipeTextField1 = "Investor",
                RecipeTextField2 = "focus group",
                RecipeDateField1 = "InvalidDateValue"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/recipes", invalidRecipe)
                .ConfigureAwait(false);

            // Assert
            httpResponse.StatusCode.Should().Be(400);
        }
    }
}
