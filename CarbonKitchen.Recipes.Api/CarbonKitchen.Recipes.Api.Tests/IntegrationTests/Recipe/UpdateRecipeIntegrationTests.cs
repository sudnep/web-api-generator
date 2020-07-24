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
    using Microsoft.Extensions.DependencyInjection;
    using CarbonKitchen.Recipes.Api.Data;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using AutoMapper;
    using CarbonKitchen.Recipes.Api.Configuration;
    using Bogus;

    [Collection("Sequential")]
    public class UpdateRecipeIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public UpdateRecipeIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PatchRecipeReturns204AndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RecipeProfile>();
            }).CreateMapper();

            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var expectedFinalObject = mapper.Map<RecipeDto>(fakeRecipeOne);
            expectedFinalObject.RecipeTextField1 = lookupVal;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
                context.Database.EnsureCreated();

                context.Recipes.RemoveRange(context.Recipes);
                context.Recipes.AddRange(fakeRecipeOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var patchDoc = new JsonPatchDocument<RecipeForUpdateDto>();
            patchDoc.Replace(r => r.RecipeTextField1, lookupVal);
            var serializedRecipeToUpdate = JsonConvert.SerializeObject(patchDoc);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/v1/recipes/?filters=RecipeTextField1=={fakeRecipeOne.RecipeTextField1}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<RecipeDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().RecipeId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/v1/recipes/{id}")
            {
                Content = new StringContent(serializedRecipeToUpdate,
                Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/v1/recipes/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<RecipeDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task BadPatchRecipeReturns400BadRequest()
        {
            //Arrange
            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var fakeRecipeOne = new FakeRecipe { }.Generate();

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
                context.Database.EnsureCreated();

                context.Recipes.RemoveRange(context.Recipes);
                context.Recipes.AddRange(fakeRecipeOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var manuallyCreatedInvalidPatchDoc = "[{\"value\":\"" + lookupVal + "\",\"path\":\"/RecipeIntField1\",\"op\":\"replace\"}]";

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/v1/recipes/?filters=RecipeTextField1=={fakeRecipeOne.RecipeTextField1}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<RecipeDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().RecipeId;

            // patch it
            var method = new HttpMethod("PATCH");
            var patchRequest = new HttpRequestMessage(method, $"api/v1/recipes/{id}")
            {
                Content = new StringContent(manuallyCreatedInvalidPatchDoc,
                Encoding.Unicode, "application/json")
            };
            var patchResult = await client.SendAsync(patchRequest)
                .ConfigureAwait(false);

            // Assert
            patchResult.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task PutRecipeReturnsBodyAndFieldsWereSuccessfullyUpdated()
        {
            //Arrange
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RecipeProfile>();
            }).CreateMapper();

            var lookupVal = "Easily Identified Value For Test"; // don't know the id at this scope, so need to have another value to lookup
            var newString = "New Val";
            var newInt = 12;
            var newDate = new Faker("en").Date.Past();
            var fakeRecipeOne = new FakeRecipe { }.Generate();
            var expectedFinalObject = mapper.Map<RecipeDto>(fakeRecipeOne);
            expectedFinalObject.RecipeTextField1 = lookupVal;
            expectedFinalObject.RecipeTextField2 = newString;
            expectedFinalObject.RecipeIntField1 = newInt;
            expectedFinalObject.RecipeDateField1 = newDate;

            var appFactory = _factory;
            using (var scope = appFactory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
                context.Database.EnsureCreated();

                context.Recipes.RemoveRange(context.Recipes);
                context.Recipes.AddRange(fakeRecipeOne);
                context.SaveChanges();
            }

            var client = appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var serializedRecipeToUpdate = JsonConvert.SerializeObject(expectedFinalObject);

            // Act
            // get the value i want to update. assumes I can use sieve for this field. if this is not an option, just use something else
            var getResult = await client.GetAsync($"api/v1/recipes/?filters=RecipeTextField1=={fakeRecipeOne.RecipeTextField1}")
                .ConfigureAwait(false);
            var getResponseContent = await getResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var getResponse = JsonConvert.DeserializeObject<IEnumerable<RecipeDto>>(getResponseContent);
            var id = getResponse.FirstOrDefault().RecipeId;

            // put it
            var patchResult = await client.PutAsJsonAsync($"api/v1/recipes/{id}", expectedFinalObject)
                .ConfigureAwait(false);

            // get it again to confirm updates
            var checkResult = await client.GetAsync($"api/v1/recipes/{id}")
                .ConfigureAwait(false);
            var checkResponseContent = await checkResult.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var checkResponse = JsonConvert.DeserializeObject<RecipeDto>(checkResponseContent);

            // Assert
            patchResult.StatusCode.Should().Be(204);
            checkResponse.Should().BeEquivalentTo(expectedFinalObject, options =>
                options.ExcludingMissingMembers());
        }
    }
}
