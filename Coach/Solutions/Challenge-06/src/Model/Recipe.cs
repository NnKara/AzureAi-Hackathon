using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDevHackathon.ConsoleApp.VectorDB.Recipes
{

    ///<summary>
    /// Represents a recipe with various properties such as name, description, cuisine, difficulty, and more.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Gets or sets the unique identifier for the recipe.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the name of the recipe.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the description of the recipe.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the vector representation of the recipe.
        /// </summary>
        public List<float> vectors { get; set; }

        /// <summary>
        /// Gets or sets the cuisine type of the recipe.
        /// </summary>
        public string cuisine { get; set; }

        /// <summary>
        /// Gets or sets the difficulty level of the recipe.
        /// </summary>
        public string difficulty { get; set; }

        /// <summary>
        /// Gets or sets the preparation time for the recipe.
        /// </summary>
        public string prepTime { get; set; }

        /// <summary>
        /// Gets or sets the cooking time for the recipe.
        /// </summary>
        public string cookTime { get; set; }

        /// <summary>
        /// Gets or sets the total time required for the recipe.
        /// </summary>
        public string totalTime { get; set; }

        /// <summary>
        /// Gets or sets the number of servings the recipe yields.
        /// </summary>
        public int servings { get; set; }

        /// <summary>
        /// Gets or sets the list of ingredients required for the recipe.
        /// </summary>
        public List<string> ingredients { get; set; }

        /// <summary>
        /// Gets or sets the list of instructions to prepare the recipe.
        /// </summary>
        public List<string> instructions { get; set; }
    }


}
