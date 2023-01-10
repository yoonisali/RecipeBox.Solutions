namespace RecipeBox.Models  
{
  public class Recipe
  {
    public int RecipeId { get; set; }
    public string Ingredients { get; set; }
    public string Instructions { get; set; }
    public string Name { get; set; }
    public List<RecipeTag> JoinEntities { get;}
    public ApplicationUser User { get; set; }
  }

}