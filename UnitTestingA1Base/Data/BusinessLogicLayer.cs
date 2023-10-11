using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class BusinessLogicLayer
    {
        private AppStorage _appStorage;

        public BusinessLogicLayer(AppStorage appStorage) 
        {
            _appStorage = appStorage;
        }

        public HashSet<Recipe> GetRecipesByIngredient(int? id, string name)
        {
            Ingredient ingredient;
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);

                // Check if the ingredient is not null
                if (ingredient != null)
                {
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients
                        .Where(rI => rI.IngredientId == ingredient.Id)
                        .ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id))
                        .ToHashSet();
                }
            }
            else if (name != null)
            {
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.Contains(name));

                // Check if the ingredient is not null
                if (ingredient != null)
                {
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients
                        .Where(rI => rI.IngredientId == ingredient.Id)
                        .ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id))
                        .ToHashSet();
                }
            }

            return recipes;
        }


        public HashSet<Recipe> GetRecipesByDietaryRestriction(int? id, string? name)
        {
            Ingredient ingredient;
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id != null)
            {
                // Retrieve the ingredient by ID
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);

                if (ingredient != null)
                {
                    // Get all RecipeIngredient entries associated with the ingredient
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients
                        .Where(ri => ri.IngredientId == ingredient.Id)
                        .ToHashSet();

                    // Find recipes that have a relationship with the ingredient
                    recipes = _appStorage.Recipes
                        .Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id))
                        .ToHashSet();
                }
            }
            else if (!string.IsNullOrEmpty(name))
            {
                // Retrieve the ingredient by name (case-insensitive search)
                ingredient = _appStorage.Ingredients.FirstOrDefault(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

                if (ingredient != null)
                {
                    // Get all RecipeIngredient entries associated with the ingredient
                    HashSet<RecipeIngredient> recipeIngredients = _appStorage.RecipeIngredients
                        .Where(ri => ri.IngredientId == ingredient.Id)
                        .ToHashSet();

                    // Find recipes that have a relationship with the ingredient
                    recipes = _appStorage.Recipes
                        .Where(r => recipeIngredients.Any(ri => ri.RecipeId == r.Id))
                        .ToHashSet();
                }
            }

            return recipes;
        }

    }
}
