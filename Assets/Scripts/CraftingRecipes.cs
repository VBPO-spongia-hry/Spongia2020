using System;
using System.Collections.Generic;
using Items;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Recipes", order = 3)]
public class CraftingRecipes : ScriptableObject
{
    public Recipe[] recipes;
}

[Serializable]
public class Recipe
{
    [Serializable]
    public class Ingredient
    {
        public string item;
        public int count;
    }
    public List<Ingredient> ingredients;
    public Item result;
}