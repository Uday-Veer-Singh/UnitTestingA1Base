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
            AppStorage appStorage = new AppStorage(); 
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage); 

            // Create a test ingredient
            Ingredient ingredient = new Ingredient { Id = 1, Name = "Spaghetti" };
            appStorage.Ingredients.Add(ingredient);

            // Create test recipes with the ingredient
            Recipe recipe1 = new Recipe { Id = 1, Name = "Spaghetti Carbonara" };
            Recipe recipe2 = new Recipe { Id = 2, Name = "Lasagna" };

            // Add the recipes to the app storage
            appStorage.Recipes.Add(recipe1);
            appStorage.Recipes.Add(recipe2);

            // Create RecipeIngredient objects to associate recipes with the ingredient
            RecipeIngredient recipeIngredient1 = new RecipeIngredient
            {
                IngredientId = ingredient.Id,
                RecipeId = recipe1.Id,
                Amount = 200,
                MeasurementUnit = MeasurementUnit.Grams
            };

            appStorage.RecipeIngredients.Add(recipeIngredient1);

            // Act
            HashSet<Recipe> result = bll.GetRecipesByIngredient(null, "Spaghetti");

            // Assert
            // Expecting recipe1 to be in the result
            Assert.IsTrue(result.Contains(recipe1)); 
            Assert.IsFalse(result.Contains(recipe2)); 
            // Expecting recipe3 not to be in the result
        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ReturnsNull()
        {
            // Arrange
            // Initialize your app storage
            AppStorage appStorage = new AppStorage();

            // Initialize the BusinessLogicLayer
            BusinessLogicLayer bll = new BusinessLogicLayer(appStorage); 

            // Create an ingredient with an ID that doesn't exist in the storage
            Ingredient ingredient = new Ingredient { Id = 999, Name = "Invalid Ingredient" };
            appStorage.Ingredients.Add(ingredient);

            // Act
            // Provide an invalid ID that doesn't exist
            HashSet<Recipe> result = bll.GetRecipesByIngredient(123, null);

            // Assert
            // Expecting the result to be null
            Assert.IsTrue(result.Count == 0); // Expecting an empty HashSet
        }

    }
}