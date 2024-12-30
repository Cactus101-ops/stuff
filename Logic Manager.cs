using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using System.Xml.Schema;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;




public class LogicManager : MonoBehaviour
{
    

    

    [Header("public variables")]
    public int pppoints;
    public GameObject Dog;
    public Transform spawnpoint;

    [SerializeField] public GameObject dogprefab;
    [SerializeField] public GameObject catprefab;
    [SerializeField] public Transform[] spawnPoints;
    [SerializeField] public Transform[] points;

    public string[] DogNames;
    public string dogname;
    public string[] CatNames;

    

    public Dictionary<Transform, ActionManager> mapSpawnpointtocollider = new Dictionary<Transform, ActionManager>();

    [Header("Lists")]
    public List<Dog> dogsawake = new List<Dog>();
    public List<Dog> dogssleeping = new List<Dog>();
    public List<Cat> activecats = new List<Cat>();
    private Transform targetpoint;

    public GameObject selectedCollider; 

    public List<Transform> occupiedspawnpoints = new List<Transform>();

    [Header("Manager References")]
    public UIManager UI;
    public GameData GD;
    public List<ActionManager> Colliders;
    public ActionManager AM;
    public DogManager DoM;
    public DogListUIManager DLUI;

    [Header("Misc References")]
    private PetFactory PF;
    private SaveLoadManager SLM;
    public Button dogbutton;  
    public Button catbutton;  

    
    private const string dogfilepath = "Assets/Files/Dogs.txt";
    private const string catfilepath = "Assets/Files/Cats.txt";
    private const string pointsfilepath = "Assets/Files/PawSativePoints.txt";

    public static LogicManager Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        initialize();

       
    }


    private void Start()
    {
        initializespawnpointmap();
        PF = PetFactory.Instance;
        PetFactory.Instance.loadbutton(dogbutton);
        PetFactory.Instance.loadbutton(catbutton);


        DoM = new DogManager();
    }
   
    private void initialize()
    {
        if (UI == null)
        {
            UI = FindObjectOfType<UIManager>();
            
        }

        Colliders = new List<ActionManager>();
        
        activecats = new List<Cat>();
        
    }

    //=== spawn dogs and cats ===\\
    public void createdogbuttonclicked()
    {
        
        GameObject dog = PF.createdog();
        AudioManager.Instance.playhappybark();
    }

    public void createcatbuttonclicked()
    {
        
        GameObject cat = PF.createcat();
        
    }

    //=== maps cat spawans to colliders ===\\
    private void initializespawnpointmap()
    {
        foreach (Transform spawnpoint in spawnPoints)
        {
            var link = spawnpoint.GetComponent<ColliderLink>();
            

            Debug.Log($"Mapping spawn point {spawnpoint.name} to container {link.associatedcollider.name}");
            mapSpawnpointtocollider[spawnpoint] = link.associatedcollider;
        }
    }

    //=== gets random spawn point for cats ===\\

    public Transform getrandomspawn(List<Transform> occupiedSpawnPoints, Transform[] spawnPoints)
    {
        

        List<Transform> freespawnpoints = new List<Transform>();
        foreach (Transform sp in spawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(sp))
            {
                freespawnpoints.Add(sp);
            }
        }


        if (freespawnpoints.Count == 0)
        {
            Debug.LogError("All spawn points are occupied! Can't spawn more cats!");
            return null;
        }

        Transform selectedSpawnPoint = freespawnpoints[UnityEngine.Random.Range(0, freespawnpoints.Count)];
        occupiedSpawnPoints.Add(selectedSpawnPoint);
        return selectedSpawnPoint;
    }
    

    //=== Finds Collider by ID after loading the game ===\\   

    public ColliderLink findcolliderbyID(int id)
    {
        ColliderLink[] allcolliderlinks = FindObjectsOfType<ColliderLink>();//find objects of type is expensive so look into caching instead if possible
        foreach (var colliderLink in allcolliderlinks)
        {
            if (colliderLink.colliderID == id)
            {
                Debug.Log($"Found container with ID: {id} for {colliderLink.name}");
                return colliderLink;
            }
        }
        
        return null;
    }

    //=== For Setting the Collider the player clicks on ===\\
    public void setcollider(GameObject collider)
    {
        selectedCollider = collider;
        Debug.Log($"Selected collider set to: {collider.name}");
    }

    //=== Get Collider's Target Point's for moving the dog ===//
    public void settarget(Transform point)
    {
        targetpoint = point;
        Debug.Log($"set to: {point.position}");
    }

    public Transform gettarget()
    {
        return targetpoint;
    }



    //=== Stat Comparison System ===\\
    public static bool comparestats(Dictionary<string, int> dogstats, Dictionary<string, int> catstats, string stattocompare, LogicManager logicmanager, Dog dog, Cat cat)
    {

        if (dogstats.ContainsKey(stattocompare) && catstats.ContainsKey(stattocompare))
        {
            int catvalue = catstats[stattocompare];
            int dogvalue = dogstats[stattocompare];
            Debug.Log($"Comparing {stattocompare}: Dog = {dogvalue}, Cat = {catvalue}");

            if (dogvalue >= catvalue)
            {
                Debug.Log($"Good job, {dog.Name}! You earned bonus Paw-Sative Points!");
                logicmanager.worksuccess(dog.gameObject);
                cat.calmdown();
                
            }
            else
            {
                Debug.LogError($"{cat.name} scared off a dog. Better luck next time!");
                logicmanager.dogsawake.Remove(dog);
                UnityEngine.Object.Destroy(dog.gameObject);
                AudioManager.Instance.playworkfail();
                cat.becomeangry();
                LogicManager.Instance.DLUI.filldoglist();
                return false;
            }
        }return true;
    }

    public void worksuccess(GameObject Dog)
    {
        
        Dog dogComponent = Dog.GetComponent<Dog>();
        pppoints += 100;
        UI.updatepppointstext();
        Debug.Log("Paw-Sative Points added for good work!");
        AudioManager.Instance.playworksuccess();
        DoM.deactivatedog(dogComponent);
        LogicManager.Instance.DLUI.filldoglist();

    }
    
   
    public void subtractpppoints(int amount)
    {
        pppoints -= amount;
    }

    //=== Helper Methods For the SaveLoadManager ===\\
    public void clearcatsanddogs()
    {
        foreach (var dog in dogsawake.Concat(dogssleeping))
        {
            if (dog?.gameObject != null)
            {
                Destroy(dog.gameObject);
            }
        }
        dogsawake.Clear();
        dogssleeping.Clear();

        foreach (var cat in activecats)
        {
            if (cat?.gameObject != null)
            {
                Destroy(cat.gameObject);
            }
        }
        activecats.Clear();

        Dog = null;
        
        Debug.Log("Cleared all existing cats and dogs.");
    }

    
}



public class DogManager
{
    
    // Methods for Managing Dogs States
    public void activatedog(Dog dog)
    {
        if (!LogicManager.Instance.dogsawake.Contains(dog))
        {
            AudioManager.Instance.playalarmclock();
            LogicManager.Instance.dogsawake.Add(dog);
            LogicManager.Instance.dogssleeping.Remove(dog);
            LogicManager.Instance.DLUI.filldoglist();
        }
    }

    public void deactivatedog(Dog dog)
    {
        if (LogicManager.Instance.dogsawake.Contains(dog))
        {
            LogicManager.Instance.DLUI.filldoglist();
            LogicManager.Instance.dogsawake.Remove(dog);
            LogicManager.Instance.dogssleeping.Add(dog);
            LogicManager.Instance.StartCoroutine(reactivatedog(dog, 20f));
        }

        teleportdogtospawn(dog);
    }
    public IEnumerator reactivatedog(Dog dog, float delay)
    {
        yield return new WaitForSeconds(delay);//9 out of 10 cats love countdown
        LogicManager.Instance.DoM.activatedog(dog);
        Debug.Log($"{dog.Name} reactivated after {delay} seconds.");
    }
    private void teleportdogtospawn(Dog dog)
    {
        dog.transform.position = LogicManager.Instance.spawnpoint.position;
        
    }

   

}




   

