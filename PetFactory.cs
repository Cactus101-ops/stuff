using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetFactory : MonoBehaviour
{
    public static PetFactory Instance { get; private set;}

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        Instance.initialize(LogicManager.Instance, UIManager.instance);
    }

    private LogicManager LM;
    public UIManager UI;

    private void initialize(LogicManager logicmanager, UIManager uimanager)
    {
        this.LM = logicmanager;
    }

    public void loadbutton(Button spawnButton)
    {
        spawnButton.onClick.RemoveAllListeners();
        
    }

    private void spawnbuttonclicked()
    {
        createdog();
        
    }

    public GameObject createdog()
    {
        if (LM.pppoints > 100)
        {
            
            LM.subtractpppoints(100);

            
            if (UI == null)
            {
                UI = FindObjectOfType<UIManager>();
            }
            UI.updatepppointstext(); //not too sure why but it needs a null check here to work, just keeps throwing null ref errors without it

            GameObject doginstance = Instantiate(LM.dogprefab, LM.spawnpoint.position, Quaternion.identity);
            Dog dogcomponent = doginstance.GetComponent<Dog>();

            string dogname = LM.DogNames[UnityEngine.Random.Range(0, LM.DogNames.Length)];
            LM.dogname = dogname;
            dogcomponent.initialize(dogname);

            LM.Dog = doginstance;
            Debug.Log($"Dog {dogname} created and added to awake dogs.");

            LM.DoM.activatedog(dogcomponent);
            return doginstance;
        }
        else
        {
            Debug.Log("Not enough Paw-Sative Points to spawn a dog.");
            return null;
        }
    }

    public GameObject createcat()
    {
        string catname = LM.CatNames[UnityEngine.Random.Range(0, LM.CatNames.Length)];
        Debug.Log($"Spawning cat with name: {catname}");

        Transform spawnpoint = LM.getrandomspawn(LM.occupiedspawnpoints, LM.spawnPoints);

        GameObject catinstance = Instantiate(LM.catprefab, spawnpoint.position, Quaternion.identity);
        Cat catcomponent = catinstance.GetComponent<Cat>();

        catcomponent.initialize(catname);

        
        if (LM.mapSpawnpointtocollider.TryGetValue(spawnpoint, out ActionManager actionManager))
        {
            actionManager.setcat(catcomponent);

            ColliderLink colliderLink = actionManager.GetComponentInChildren<ColliderLink>();
            if (colliderLink != null)
            {
                catcomponent.catlink = colliderLink;
                Debug.Log($"Cat {catname} linked to container: {colliderLink.name}");
            }
            
        }
       
        activecatadd(catcomponent);

        return catinstance;
    }

    public void activecatadd(Cat cat)
    {
        if (!LogicManager.Instance.activecats.Contains(cat))
        {
            LogicManager.Instance.activecats.Add(cat);
        }
    }
}
