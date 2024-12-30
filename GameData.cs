using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




[Serializable]
public class DogMemento
{
    public string savename;
    public bool isactive;

    
    [NonSerialized]
    public Dictionary<string, int> savestats;

    // A serializable representation of the dictionary because Unity is fussy
    public List<SerializableKeyValuePair> SerializedStats;

    
    public void serialization() //cearal-izes the dictionary
    {
        if (savestats == null) return;

        SerializedStats = new List<SerializableKeyValuePair>();
        foreach (var kvp in savestats)
        {
            SerializedStats.Add(new SerializableKeyValuePair { Key = kvp.Key, Value = kvp.Value });
        }
    }

    // Convert serializable list back to dictionary because Unity is fussy
    public void revertserialization()
    {
        if (SerializedStats == null) return;

        savestats = new Dictionary<string, int>();
        foreach (var kvp in SerializedStats)
        {
            savestats[kvp.Key] = kvp.Value;
        }
    }
}

[Serializable]
public class SerializableKeyValuePair
{
    public string Key;
    public int Value;
}






[Serializable]
public class CatMemento
{
    public string Name;
    public Dictionary<string, int> Stats;
    public bool IsAngry;
    public int AssociatedColliderID; 
    public float PositionX; 
    public float PositionY; 
    public float PositionZ; 


}

[System.Serializable]
public class GameData
{
    public List<DogMemento> Dogs;
    public List<CatMemento> Cats;
    public int PlayerPPPoints;
    
    
    public string SelectedDogName;
    public string SelectedCatName;

}


[System.Serializable]
public class Wrapper<T>
{
    public T[] Items; // Uses "Items" to match the JSON structure
}

[System.Serializable]
public class PPPointsWrapper
{
    public int ppoints;
}

[System.Serializable]
public class StatsEntry
{
    public string Key;
    public int Value;

    public StatsEntry(string key, int value)
    {
        Key = key;
        Value = value;
    }
}
