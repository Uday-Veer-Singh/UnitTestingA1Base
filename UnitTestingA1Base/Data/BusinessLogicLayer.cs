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


        public HashSet<Recipe> GetRecipes(int id, string name)
        {
            // Initialize a collection to store found recipes.
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            // Search for recipes by ID.
            if (id != 0)
            {
                Recipe recipe = _appStorage.Recipes.FirstOrDefault(r => r.Id == id);

                if (recipe != null)
                {
                    recipes.Add(recipe);
                }
            }

            // If no recipe is found by ID, try searching by name.
            if (recipes.Count == 0 && !string.IsNullOrEmpty(name))
            {
                recipes = _appStorage.Recipes
                    .Where(r => r.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToHashSet();
            }

            return recipes;
        }

        public void RecipeWithIngridients(Recipe recipe, HashSet<Ingredient> ingredients)
        {
            // Check if a recipe with the same name already exists
            if (_appStorage.Recipes.Any(r => string.Equals(r.Name, recipe.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Recipe with the same name already exists");
            }

            // Generate a unique ID for the new recipe
            recipe.Id = _appStorage.GeneratePrimaryKey();

            // Add the recipe to the storage
            _appStorage.Recipes.Add(recipe);

            // Process each ingredient
            foreach (Ingredient ingredient in ingredients)
            {
                Ingredient existingIngredient = _appStorage.Ingredients
                    .FirstOrDefault(i => string.Equals(i.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase));

                if (existingIngredient == null)
                {
                    // Generate a unique ID for the new ingredient
                    ingredient.Id = _appStorage.GeneratePrimaryKey();

                    // Add the new ingredient to the storage
                    _appStorage.Ingredients.Add(ingredient);

                    // Create a RecipeIngredient entry for the new ingredient
                    _appStorage.RecipeIngredients.Add(new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredient.Id
                    });
                }
                else
                {
                    // Use the existing ingredient's ID
                    _appStorage.RecipeIngredients.Add(new RecipeIngredient
                    {
                        RecipeId = recipe.Id,
                        IngredientId = existingIngredient.Id
                    });
                }
            }
        }

        public void DeleteIngredient(int? id, string? name)
        {
            if (id == null && string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Either ID or Name must be provided.");
            }

            Ingredient ingredientToDelete = null;

            if (id != null)
            {
                ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Name.Contains(name));
            }

            if (ingredientToDelete == null)
            {
                throw new InvalidOperationException("Ingredient not found.");
            }

            // Check how many recipes use this ingredient
            List<RecipeIngredient> associatedRecipes = _appStorage.RecipeIngredients
                .Where(ri => ri.IngredientId == ingredientToDelete.Id)
                .ToList();

            if (associatedRecipes.Count > 1)
            {
                throw new InvalidOperationException("Multiple recipes use this ingredient. Cannot delete.");
            }

            if (associatedRecipes.Count == 1)
            {
                int recipeId = associatedRecipes[0].RecipeId;

                // Remove the associated Recipe and RecipeIngredient
                Recipe recipeToRemove = _appStorage.Recipes.FirstOrDefault(r => r.Id == recipeId);
                _appStorage.Recipes.Remove(recipeToRemove);
                _appStorage.RecipeIngredients.Remove(associatedRecipes[0]);
            }

            // Remove the ingredient
            _appStorage.Ingredients.Remove(ingredientToDelete);
        }

        public void DeleteRecipe(int? id, string name)
        {
            if (id == null && string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Either ID or Name must be provided.");
            }

            // Find the recipe to delete based on the provided ID or name
            Recipe recipeToDelete = id != null
                ? _appStorage.Recipes.FirstOrDefault(r => r.Id == id)
                : _appStorage.Recipes.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (recipeToDelete != null)
            {
                // Get all RecipeIngredient associations for the recipe
                var recipeIngredientsToDelete = _appStorage.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipeToDelete.Id)
                    .ToList();

                // Remove the RecipeIngredient associations
                foreach (var ri in recipeIngredientsToDelete)
                {
                    _appStorage.RecipeIngredients.Remove(ri);
                }

                // Remove the recipe itself
                _appStorage.Recipes.Remove(recipeToDelete);
            }
            else
            {
                throw new ArgumentException("Recipe not found.");
            }
        }

    }
}
