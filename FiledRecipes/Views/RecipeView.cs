using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views
{
    /// <summary>
    /// 
    /// </summary>
    public class RecipeView : ViewBase, IRecipeView // klass som representerar presentationslagret
    {
        public virtual void Show(IRecipe recipe) {
         //visar enskilt recept, tar emot ett Irecipe-objekt 

            //instans av klassen recipeView används för att skriva ut recept i ett konsollfönster
            RecipeView receptvy = new RecipeView(); // instans av klassen recipeView
            
            receptvy.Header = recipe.Name.ToString(); // sätter panelnamnet på det recept man valt
            receptvy.ShowHeaderPanel();                // skriver ut namnpanelen
            Console.WriteLine("");
            Console.WriteLine("Ingredienser");
            Console.WriteLine("-------------");
            
            foreach (Ingredient ingredient in recipe.Ingredients) { // loopar igenom och skriver ut det valda receptets ingredienser
               
                Console.WriteLine(ingredient);
            }
            
            Console.WriteLine("");
            Console.WriteLine("Instruktioner");
            Console.WriteLine("-------------");
            
            foreach (String instruction in recipe.Instructions) {   // loopar igenom och skriver ut det valda receptetets instruktioner
                
                Console.WriteLine(instruction);
                Console.WriteLine("");
            }  
        }

        public virtual void Show(IEnumerable<IRecipe> recipes) {
        //lista på recept som visas var och en för sig, tar emot Irecipe-listobjekt
            
            RecipeView receptvyer = new RecipeView(); //instans som används för att skriva ut recepten i listan

            foreach (IRecipe recipe in recipes) { // en loop som loopar igenom och skriver alla recept i recipes-listan
           
                receptvyer.Header = recipe.Name.ToString(); //sätter panelnamnet på det aktuella receptet
                receptvyer.ShowHeaderPanel();               //skriver ut namnpanelen
                Console.WriteLine("");
                Console.WriteLine("Ingredienser");
                Console.WriteLine("-------------");

                foreach (Ingredient ingredient in recipe.Ingredients) {  //loopar igenom och skriver ut de aktuella ingredienser
                
                    Console.WriteLine(ingredient);
                }

                Console.WriteLine("");
                Console.WriteLine("Instruktioner");
                Console.WriteLine("-------------");

                foreach (String instruction in recipe.Instructions) {    //loopar igenom och skriver ut de aktuella instruktionerna
                
                    Console.WriteLine(instruction);
                    Console.WriteLine("");
                }

                receptvyer.ContinueOnKeyPressed(); //kall på onkeypressed i  klassen viewbase som ritar ut funktionen för att bläddra bland recepten
            }
        }
    }
}
