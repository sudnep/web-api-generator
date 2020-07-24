namespace CarbonKitchen.Recipes.Api.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Sieve.Attributes;

    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        [Required]
        [Column("RecipeId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int RecipeId { get; set; }

        [Column("RecipeIntField1")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int? RecipeIntField1 { get; set; }

        [Column("RecipeTextField1")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string RecipeTextField1 { get; set; }

        [Column("RecipeTextField2")]
        [Sieve(CanFilter = true, CanSort = true)]
        public string RecipeTextField2 { get; set; }

        [Column("RecipeDateField1")]
        public DateTime? RecipeDateField1 { get; set; }
    }
}
