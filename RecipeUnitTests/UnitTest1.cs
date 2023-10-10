using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;

namespace RecipeUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private BusinessLogicLayer _initializeBusinessLogic()
        {
            return new BusinessLogicLayer(new AppStorage());
        }
        [TestMethod]
        public void GetRecipesByIngredient_ValidId_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;
            int recipeCount = 2;

            // act
            HashSet<Recipe> recipes = bll.GetRecipesByIngredient(ingredientId, null);

            Assert.AreEqual(recipeCount, recipes.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ValidName_ReturnsRecipesWithIngredient()
        {
            // Arrange
            AppStorage appStorage = new AppStorage(); // Initialize your app storage
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage); // Initialize the BusinessLogicLayer

            // Create a test ingredient
            Ingredient ingredient = new Ingredient { Id = 1, Name = "Spaghetti" };
            appStorage.Ingredients.Add(ingredient);

            // Create test recipes with the ingredient
            Recipe recipe1 = new Recipe { Id = 1, Name = "Spaghetti Carbonara" };
            Recipe recipe2 = new Recipe { Id = 2, Name = "Spaghetti Bolognese" };
            Recipe recipe3 = new Recipe { Id = 3, Name = "Lasagna" };

            // Add the recipes to the app storage
            appStorage.Recipes.Add(recipe1);
            appStorage.Recipes.Add(recipe2);
            appStorage.Recipes.Add(recipe3);

            // Create RecipeIngredient objects to associate recipes with the ingredient
            RecipeIngredient recipeIngredient1 = new RecipeIngredient
            {
                IngredientId = ingredient.Id,
                RecipeId = recipe1.Id,
                Amount = 200,
                MeasurementUnit = MeasurementUnit.Grams
            };
            RecipeIngredient recipeIngredient2 = new RecipeIngredient
            {
                IngredientId = ingredient.Id,
                RecipeId = recipe2.Id,
                Amount = 300,
                MeasurementUnit = MeasurementUnit.Grams
            };
            appStorage.RecipeIngredients.Add(recipeIngredient1);
            appStorage.RecipeIngredients.Add(recipeIngredient2);

            // Act
            HashSet<Recipe> result = bll.GetRecipesByIngredient(null, "Spaghetti");

            // Assert
            Assert.IsTrue(result.Contains(recipe1)); // Expecting recipe1 to be in the result
            Assert.IsTrue(result.Contains(recipe2)); // Expecting recipe2 to be in the result
            Assert.IsFalse(result.Contains(recipe3)); // Expecting recipe3 not to be in the result
        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ReturnsNull()
        {

        }
    }
}