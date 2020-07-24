namespace CarbonKitchen.Recipes.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using CarbonKitchen.Recipes.Api.Data.Entities;
    using CarbonKitchen.Recipes.Api.Models.Pagination;
    using CarbonKitchen.Recipes.Api.Models.Recipe;
    using CarbonKitchen.Recipes.Api.Services;
    using CarbonKitchen.Recipes.Api.Services.Recipe;
    using CarbonKitchen.Recipes.Api.Validators.Recipe;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/recipes")]
    public class RecipesController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RecipesController(IRecipeRepository recipeRepository
            , IMapper mapper)
        {
            _recipeRepository = recipeRepository ??
                throw new ArgumentNullException(nameof(recipeRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetRecipes")]
        public ActionResult<IEnumerable<RecipeDto>> GetRecipes([FromQuery] RecipeParametersDto recipeParametersDto)
        {
            var recipesFromRepo = _recipeRepository.GetRecipes(recipeParametersDto);
            
            var previousPageLink = recipesFromRepo.HasPrevious
                    ? CreateRecipesResourceUri(recipeParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = recipesFromRepo.HasNext
                ? CreateRecipesResourceUri(recipeParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = recipesFromRepo.TotalCount,
                pageSize = recipesFromRepo.PageSize,
                pageNumber = recipesFromRepo.PageNumber,
                totalPages = recipesFromRepo.TotalPages,
                hasPrevious = recipesFromRepo.HasPrevious,
                hasNext = recipesFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var recipesDto = _mapper.Map<IEnumerable<RecipeDto>>(recipesFromRepo);
            return Ok(recipesDto);
        }


        [HttpGet("{recipeId}", Name = "GetRecipe")]
        public ActionResult<RecipeDto> GetRecipe(int recipeId)
        {
            var recipeFromRepo = _recipeRepository.GetRecipe(recipeId);

            if (recipeFromRepo == null)
            {
                return NotFound();
            }

            var recipeDto = _mapper.Map<RecipeDto>(recipeFromRepo);

            return Ok(recipeDto);
        }

        [HttpPost]
        public ActionResult<RecipeDto> AddRecipe(RecipeForCreationDto recipeForCreation)
        {
            var validationResults = new RecipeForCreationDtoValidator().Validate(recipeForCreation);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            var recipe = _mapper.Map<Recipe>(recipeForCreation);
            _recipeRepository.AddRecipe(recipe);
            _recipeRepository.Save();

            var recipeDto = _mapper.Map<RecipeDto>(recipe);
            return CreatedAtRoute("GetRecipe",
                new { recipeDto.RecipeId },
                recipeDto);
        }

        [HttpDelete("{recipeId}")]
        public ActionResult DeleteRecipe(int recipeId)
        {
            var recipeFromRepo = _recipeRepository.GetRecipe(recipeId);

            if (recipeFromRepo == null)
            {
                return NotFound();
            }

            _recipeRepository.DeleteRecipe(recipeFromRepo);
            _recipeRepository.Save();

            return NoContent();
        }

        [HttpPut("{recipeId}")]
        public IActionResult UpdateRecipe(int recipeId, RecipeForUpdateDto recipe)
        {
            var recipeFromRepo = _recipeRepository.GetRecipe(recipeId);

            if (recipeFromRepo == null)
            {
                return NotFound();
            }

            var validationResults = new RecipeForUpdateDtoValidator().Validate(recipe);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            _mapper.Map(recipe, recipeFromRepo);
            _recipeRepository.UpdateRecipe(recipeFromRepo);

            _recipeRepository.Save();

            return NoContent();
        }

        [HttpPatch("{recipeId}")]
        public IActionResult PartiallyUpdateRecipe(int recipeId, JsonPatchDocument<RecipeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingRecipe = _recipeRepository.GetRecipe(recipeId);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            var recipeToPatch = _mapper.Map<RecipeForUpdateDto>(existingRecipe); // map the recipe we got from the database to an updatable recipe model
            patchDoc.ApplyTo(recipeToPatch, ModelState); // apply patchdoc updates to the updatable recipe

            if (!TryValidateModel(recipeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(recipeToPatch, existingRecipe); // apply updates from the updatable recipe to the db entity so we can apply the updates to the database
            _recipeRepository.UpdateRecipe(existingRecipe); // apply business updates to data if needed

            _recipeRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateRecipesResourceUri(
            RecipeParametersDto recipeParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetRecipes",
                        new
                        {
                            filters = recipeParametersDto.Filters,
                            orderBy = recipeParametersDto.SortOrder,
                            pageNumber = recipeParametersDto.PageNumber - 1,
                            pageSize = recipeParametersDto.PageSize,
                            searchQuery = recipeParametersDto.QueryString
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetRecipes",
                        new
                        {
                            filters = recipeParametersDto.Filters,
                            orderBy = recipeParametersDto.SortOrder,
                            pageNumber = recipeParametersDto.PageNumber + 1,
                            pageSize = recipeParametersDto.PageSize,
                            searchQuery = recipeParametersDto.QueryString
                        });

                default:
                    return Url.Link("GetRecipes",
                        new
                        {
                            filters = recipeParametersDto.Filters,
                            orderBy = recipeParametersDto.SortOrder,
                            pageNumber = recipeParametersDto.PageNumber,
                            pageSize = recipeParametersDto.PageSize,
                            searchQuery = recipeParametersDto.QueryString
                        });
            }
        }
    }
}