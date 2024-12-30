using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Cat : BaseCat
{
    
    public bool isangry { get; set; } 
    public enum catstate { Neutral, Angry }
    public catstate currentstate = catstate.Neutral;
    
    public Dictionary<string, int> catstats { get; set; }
   

    private static readonly Dictionary<string, Dictionary<string, int>> cattemplates = new()
    {
        { "Fat Cat", new Dictionary<string, int> { { "laziness", 5 }, { "playfulness", 1 }, { "boisterous", 1 }, { "smarts", 1 } } },
        { "Zorp Glorp Cat", new Dictionary<string, int> { { "laziness", 2 }, { "playfulness", 5 }, { "boisterous", 4 }, { "smarts", 2 } } },
        { "Professor Cats", new Dictionary<string, int> { { "laziness", 4 }, { "playfulness", 2 }, { "boisterous", 5 }, { "smarts", 4 } } },
        { "Neuromancer Cat", new Dictionary<string, int> { { "laziness", 2 }, { "playfulness", 3 }, { "boisterous", 4 }, { "smarts", 5 } } },
        { "Caffine Cat", new Dictionary<string, int> { { "laziness", 1 }, { "playfulness", 2 }, { "boisterous", 2 }, { "smarts", 5 } } },
    };//all about figuring a way to work with the cats, using Laziness on the Fat Cat early on would be a bad idea etc 

    
    private void Awake()
    {
        if (catstats == null)
        {
            catstats = new Dictionary<string, int>(); 
           
            isangry = false; 
        }
    }
    public void becomeangry()
    {
        isangry = true;
        if (currentstate == catstate.Angry)
        {
            return;
        }
        Debug.Log($"{name} is now Angry!");
        currentstate = catstate.Angry;

        


        foreach (var key in catstats.Keys.ToList())
        {
            catstats[key]++;
        }
    }

    public void calmdown()
    {
        isangry = false;
        if (currentstate != catstate.Angry)
        {
            return;
        }

        currentstate = catstate.Neutral;
        Debug.Log($"{name} has calmed down.");
        

        foreach (var key in catstats.Keys.ToList())
         {
            catstats[key]--;
         }
    }
    
    
    //=== Method to initialize the cat with its name and stats ===\\
    public void initialize(string catName)
    {
       
        this.name = catName;

        this.catstats = new Dictionary<string, int>();
        whencatspawn();
        
    }
    public void whencatspawn()
    {

        if (cattemplates.TryGetValue(name, out var stats))
        {
            catstats = new Dictionary<string, int>(stats);
            Debug.Log($"{name} initialized with stats: {string.Join(", ", catstats.Select(s => $"{s.Key}={s.Value}"))}");
        }
       

    }
    //=== get a Collider's ID and assign it to the cat for the purposes of serializing which container it belongs to ===\\
    private ColliderLink colliderlink;
    public ColliderLink catlink
    {
        get => colliderlink;
        set
        {
            colliderlink = value;
            Debug.Log($"{name} assigned to container: {value?.name}");
        }
    }
    

    
}


