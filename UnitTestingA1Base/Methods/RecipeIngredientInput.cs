using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Methods
{
    public class RecipeIngredientInput
    {
        public Recipe Recipe { get; set; }
        public HashSet<Ingredient> Ingredients { get; set; }
    }
}
