using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



public class SaveLoadManager : MonoBehaviour
{
    
    private LogicManager LM;
    private GameObject dogprefab;
    private GameObject catprefab;
    public UIManager UI;
    public DogListUIManager DLUI;
   

    public void initialize(LogicManager logicManager, DogListUIManager doglistui)
    {
        this.LM = logicManager;
        this.DLUI = doglistui;
    }
    //=== Game Saving and Loading ===\\
    public void savegame()
    {
        //Serialize Dogs
        var dogmementos = new List<DogMemento>();

        

        foreach (var dog in LogicManager.Instance.dogsawake)
        {

            var memento = new DogMemento//using memento pattern here because the game needs to save the state of the dog and cat prefabs
            {
                savename = dog.dogname,
                savestats = dog.statsref.stats,

                isactive = true
            };

            memento.serialization(); // Convert the dictionary to a serializable list of key-value pairs because Unity can't serialize dictionaries with JSONUtility!!! THANKS FOR WASTING MY TIME!!!!
            dogmementos.Add(memento);

            Debug.Log($"Saving active dog: {dog.dogname}");
        }


        
        foreach (var dog in LogicManager.Instance.dogssleeping)
        {
            if (dog == null) continue;

            var memento = new DogMemento
            {
                savename = dog.dogname,
                savestats = dog.statsref.stats,
                isactive = false
            };
            memento.serialization();
            dogmementos.Add(memento);
        }

        string dogJson = JsonUtility.ToJson(new Wrapper<DogMemento> { Items = dogmementos }, true);
        File.WriteAllText("Assets/Files/Dogs.txt", dogJson);
        Debug.Log($"Saved {dogmementos.Count} dogs.");



        //Serialize Cats
        var catmementos = new List<CatMemento>();
        foreach (var cat in LogicManager.Instance.activecats)
        {
            var memento = new CatMemento 
            {
                Name = cat.name,
                Stats = cat.catstats,
                IsAngry = cat.isangry,
                AssociatedColliderID = cat.catlink?.colliderID ?? -1,

                PositionX = cat.transform.position.x,
                PositionY = cat.transform.position.y,
                PositionZ = cat.transform.position.z

            };

           
                Debug.Log($"Cat {cat.name} is linked to container {cat.catlink.associatedcollider.name}.");
            

            catmementos.Add(memento);
        }

        string catJson = JsonUtility.ToJson(new Wrapper<CatMemento> { Items = catmementos });
        File.WriteAllText("Assets/Files/Cats.txt", catJson);



        //Serialize Points
        PPPointsWrapper ppWrapper = new PPPointsWrapper
        {
            ppoints = LogicManager.Instance.pppoints
        };

        string json = JsonUtility.ToJson(ppWrapper, true);
        File.WriteAllText("Assets/Files/PawSatviePoints.txt", json);

        Debug.Log($"Paw-Sative Points saved: {LogicManager.Instance.points}");

        Debug.Log("Game saved successfully!");
    }

    public void LoadGame()
    {
        
        LogicManager.Instance.clearcatsanddogs();
        
        if (!File.Exists("Assets/Files/Dogs.txt") || !File.Exists("Assets/Files/Cats.txt") || !File.Exists("Assets/Files/PawSatviePoints.txt"))
        {
            Debug.LogError("Save files not found!");
            return;
        }

        if (File.Exists("Assets/Files/Dogs.txt"))
        {
            string dogJson = File.ReadAllText("Assets/Files/Dogs.txt");
            var wrapper = JsonUtility.FromJson<Wrapper<DogMemento>>(dogJson);
            if (wrapper?.Items != null)
            {
                LogicManager.Instance.dogssleeping.Clear();
                LogicManager.Instance.dogssleeping.Clear();

                foreach (var memento in wrapper.Items)
                {
                    memento.revertserialization(); // Convert serialized list back to a dictionary - it feels very counter intuative to do it this way but I don't want to mess with plugins
                    

                    GameObject dogInstance = Instantiate(LogicManager.Instance.dogprefab);
                    Dog dogComponent = dogInstance.GetComponent<Dog>();
                    if (memento.isactive)
                    {
                        LogicManager.Instance.dogsawake.Add(dogComponent);
                        LogicManager.Instance.Dog = dogInstance; // Update the reference here
                    }

                   

                    dogComponent.initialize(memento.savename, memento.savestats); // Initialize a dog with the stats that were saved
                    //if I give the new dog the exact same stats, is it still the same dog?


                    Debug.Log($"Loaded dog: {memento.savename}");
                }

                Debug.Log($"Loaded {LogicManager.Instance.dogsawake.Count} active dogs and {LogicManager.Instance.dogssleeping.Count} inactive dogs.");

            }
            else
            {
                Debug.LogWarning("No dog data found to load.");
            }
        }

        // Deserialize Cats
        string catsJson = File.ReadAllText("Assets/Files/Cats.txt");
        var catsData = JsonUtility.FromJson<Wrapper<CatMemento>>(catsJson).Items;
        foreach (var catData in catsData)
        {
            GameObject catObject = Instantiate(LogicManager.Instance.catprefab);
            Cat cat = catObject.GetComponent<Cat>();
            ColliderLink colliderLink = LogicManager.Instance.findcolliderbyID(catData.AssociatedColliderID);

            if (colliderLink == null)
            {
                Debug.LogWarning($"Container with ID '{catData.AssociatedColliderID}' not found for cat {catData.Name}.");
            }
            else
            {
                cat.catlink = colliderLink;
                colliderLink.associatedcollider.setcat(cat); // Link the cat to the collider after loading - like it never left :)
                Debug.Log($"Cat {catData.Name} linked to container {colliderLink.name} during load.");
            }

            
            cat.transform.position = new Vector3(catData.PositionX, catData.PositionY, catData.PositionZ);

            cat.initialize(catData.Name);//only needs a name to assign stats 

            
            if (catData.Stats != null)
            {
                foreach (var stat in catData.Stats)
                {
                    cat.catstats[stat.Key] = stat.Value;
                }
            }

            if (catData.IsAngry)
            {
                cat.becomeangry();
            }

            LogicManager.Instance.activecats.Add(cat);
        }

        // Deserialize Points
        string pawSativePointsJson = File.ReadAllText("Assets/Files/PawSatviePoints.txt");
        PPPointsWrapper loadedPPWrapper = JsonUtility.FromJson<PPPointsWrapper>(pawSativePointsJson);
        LogicManager.Instance.pppoints = loadedPPWrapper.ppoints;

        Debug.Log("Game loaded successfully!");

        UI.updatepppointstext();
        DLUI.filldoglist();
    }

   

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }//the gameshark of JSON Utility
}

