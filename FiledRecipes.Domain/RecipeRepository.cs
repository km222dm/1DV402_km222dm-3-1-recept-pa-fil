using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FiledRecipes.Domain
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";

        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";

        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";

        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;

        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };

        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;

        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;

        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path); // ger sökvägen till filen som ska läsas in

            _recipes = new List<IRecipe>();
        }

        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }

        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }

        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;

            // Event will be null if there are no subscribers. 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        public virtual void Load() {
        
            // skapa lista som kan innehålla referenser till receptobjekt
            List <Recipe> recipeList = new List<Recipe>(); // -dynamisk array, ska hjälpa till att läsa igenom textfilen, varje gång den läses in sparas allt här så
                                                            // man kan hantera innehållet i filen på ett lättare sätt
            RecipeReadStatus status = RecipeReadStatus.Indefinite;
            
            //Öppna textfilen för läsning
            using (StreamReader fileReader = new StreamReader("C:\\Documents\\Recipes.txt", Encoding.Default)) {  
                // läser in filen, using - använd den här resursen och stäng sedan
                //så fort man inte är innanför existerar inte filereader och då stängs streamreader
            
                String line; // sträng som ska representera varje rad
                
                while ((line = fileReader.ReadLine()) != null) {

                    if (line == "") {

                        continue; //fortsätt läsa nästa rad
                    }

                    if (line == SectionRecipe) {

                        status = RecipeReadStatus.New;
                    }

                    else if(line == SectionIngredients) {

                        status = RecipeReadStatus.Ingredient;
                    }

                    else if (line == SectionInstructions) {

                        status = RecipeReadStatus.Instruction;
                    }

                    else {
                        
                        if (status == RecipeReadStatus.New) {
                            Recipe newRecipe = new Recipe(line);
                            recipeList.Add(newRecipe);
                            //Console.WriteLine("kolla hur många objekt som finns i listan: "+recipeList.Count);
                        }

                        else if (status == RecipeReadStatus.Ingredient) {
                            
                            //tolka line och splitta upp den i semikolon. .Split()
                            string[] ingredientValues = line.Split(';');
                          //  Console.WriteLine(" kolla så att line.Split stämmer: "+ingredientValues.Length);
                            
                            if (ingredientValues.Length != 3) {

                                throw new FileFormatException(); // kastas om arrayen inte innehåller 3 objekt, skapar en ny instans av fileFormatException-klassen
                            }

                            //Ingredient -objekt
                            Ingredient newIngredient = new Ingredient(); // inga parametrar behövs skickas med här eftersom ingr.-klassen inte tar några
                            
                            //Initiera ingrediens-objektet med mängd, mått och namn
                            newIngredient.Amount = ingredientValues[0];
                            newIngredient.Measure = ingredientValues[1];
                            newIngredient.Name = ingredientValues[2];
                            
                            //lägg till ingrediens-objektet i receptets lista med ingredienser
                            recipeList.Last().Add(newIngredient);
                        }

                        else if (status == RecipeReadStatus.Instruction) {
                            // recpieList.last/Latest.Add(line); // eftersom raden redan tolkas som en sträng och den behöver inte delas upp 
                            recipeList.Last().Add(line);
                        }

                        else {
                            Console.WriteLine("Något gick fel");
                            throw new FileFormatException();
                        }
                    } // else tar slut här
                } // while tar slut här
            } //slutar skanna filen här

            // sortera listan med recept avseende på receptens namn, använd Sort()
            recipeList.Sort();

            // tilldela avsett fält i klassen, _recipes, en referens till listan
           _recipes = recipeList.Cast<IRecipe>().ToList(); // detta gör att övriga menyval dyker upp

           // Tilldela avsedd egenskap i klassen, IsModified, ett värde som indikerar att listan med recept är oförändrad
           IsModified = false;

           // Utlös händelse om att recept har lästs in genom att anrop metoden OnRecipesChanged och skicka med parametern EventArgs.Empty
           OnRecipesChanged(EventArgs.Empty);

           Console.WriteLine("Recepten har lästs in");
        }

        public virtual void Save() { // finns redan textfilen ska den skrivas över
        
            using (FileStream fs = new FileStream("C:\\Documents\\Recipes.txt", FileMode.Create)) { //FileStream
           
                using (StreamWriter writer = new StreamWriter(fs, Encoding.Default)) { // skapa en streamWriter som skriver till textfilen
                
                    foreach (Recipe recipe in _recipes) {  // skriv recepten rad för rad till textfilen
                    
                        writer.WriteLine(SectionRecipe); // lägger till sectionRecipe
                        writer.WriteLine(recipe.Name);         //lägger till namner på receptet
                        writer.WriteLine(SectionIngredients); // lägger till sectionIngredients
                       
                        foreach (Ingredient ingredient in recipe.Ingredients) {   //loop som loopar igenom och skriver ut de aktuella ingredienser
                        
                            writer.WriteLine(ingredient.Amount+";"+ingredient.Measure+";"+ingredient.Name); //lägge till varje ingrediens med mängd, mått och namn
                        }
                        
                        writer.WriteLine(SectionInstructions); // lägger till sectionInstruction
                        
                        foreach (String instruction in recipe.Instructions) {    //loop som loopar igenom och skriver ut de aktuella instruktionerna
                        
                            writer.WriteLine(instruction); // lägger till varje instruktion
                        }

                    }
                }
            }

            Console.WriteLine("Filen har sparats");
        }
    }
}
