using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogStats : MonoBehaviour 
{
    public Dictionary<string, int> stats { get; set; }

    private void Awake()
    {
        if (stats == null)
        {
            stats = new Dictionary<string, int>();
        }
    }

    //=== Sets a stat's value ===\\
    public void setstats(string statname, int value)
    {
        statname = statname.ToLower(); // make all the stat names lowercase just in...case..
        if (stats.ContainsKey(statname))
        {
            stats[statname] = value;
        }
        else
        {
            stats.Add(statname, value);
        }
    }
}




//stats now store but will need to be verified tomorrow to see if they actually store the values, then it's businesss as usual and back to making sure the two dictionaries compare themselevs which starts with
//repeating this process in the cat scripts.