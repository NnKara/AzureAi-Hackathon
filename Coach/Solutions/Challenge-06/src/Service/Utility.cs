using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace AIDevHackathon.ConsoleApp.VectorDB.Recipes.Service
{
    internal static class Utility
    {
        /// <summary>
        /// Parses the documents in the specified folder path and returns a list of recipes.
        /// </summary>
        /// <param name="Folderpath">The path to the folder containing the recipe documents.</param>
        /// <returns>A list of <see cref="Recipe"/> objects parsed from the documents.</returns>
        public static List<Recipe> ParseDocuments(string Folderpath)
        {
            List<Recipe> ret = new List<Recipe>();

            // Get all the files in the folder and parse them
            Directory.GetFiles(Folderpath).ToList().ForEach(f =>
                {
                    var jsonString = File.ReadAllText(f);

                    // Deserialize the JSON string to a Recipe object
                    Recipe recipe = JsonConvert.DeserializeObject<Recipe>(jsonString);
                    recipe.id = recipe.name.ToLower().Replace(" ", "");
                    ret.Add(recipe);

                }
            );

            return ret;
        }
    }
}
