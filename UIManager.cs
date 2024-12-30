using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Dog References")]
    public GameObject dogbuttonprefab;
    public Transform doglistcontainer_D1;
    public Transform doglistcontainer_D2;

    [Header("UI")]
    public GameObject dogselectionUI_D1;
    public GameObject dogselectionUI_D2;
    public GameObject statscreen_D1;
    public GameObject statscreen_D2;
    public GameObject closebutton;

    [Header("Paw-Sative Points")]
    public TMP_Text PPpointsText;

    [Header("Manager References")]
    public List<ActionManager> AMS;
    public LogicManager LM;
    public DogMover DM;

    [Header("Selected dog/cat")]
    public Dog selecteddog_D1;//D1 means dog 1 for work
    public Dog selecteddog_D2;//D2 means dog 2 for leveling up shtuff!
    public Cat selectedcat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        initializemanagers();
        hideallUI();
        
    }

    private void Start()
    {
        updatepppointstext();
    }

    private void initializemanagers()
    {
        AMS = FindObjectsOfType<ActionManager>().ToList();
        LM = FindObjectOfType<LogicManager>();
    }
    public void Closebutton()
    {
        hideallUI();
    }   
    private void hideallUI()
    {
        dogselectionUI_D1?.SetActive(false);
        dogselectionUI_D2?.SetActive(false);
        statscreen_D1?.SetActive(false);
        statscreen_D2?.SetActive(false);
        closebutton?.SetActive(false);
    }

    public void restartgame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//need to delete or fix if I have time
    }

    //=== WORK SYSTEM (D1) ===//

    public void catselected(Cat cat)
    {
        selectedcat = cat;
        Debug.Log($"Cat selected: {selectedcat.name}");

       
        dogselectionUI_D1.SetActive(true);
        doglistcontainer_D1.gameObject.SetActive(true);
        filldoglist();
      }
    public void dogselected(Dog dog)
    {
        
        selecteddog_D1 = dog;
        Debug.Log($"Dog selected: {selecteddog_D1.dogname}");

        
        dogselectionUI_D1.SetActive(false);
        doglistcontainer_D1.gameObject.SetActive(false);
        
        statscreen_D1.SetActive(true);

        fillstatselectionUI(selectedcat);
        dog.movetopoint(LM.gettarget());
    }

    public void fillstatselectionUI(Cat cat)
    {
        Transform buttonContainer = statscreen_D1?.transform.Find("StatButtonsContainer");

        fillstatbuttons(buttonContainer, cat.catstats, statbuttonclicked);
    }

    public void statbuttonclicked(string statname)
    {

        bool success = LogicManager.comparestats(selecteddog_D1.GetComponent<DogStats>().stats, selectedcat.catstats, statname, LM, selecteddog_D1, selectedcat);
        if (success == true)
        {
            Debug.Log("Stat comparison succeeded.");
        }
        else
        {
            Debug.Log("Stat comparison failed.");
        }

        statscreen_D1.SetActive(false);
        
    }

    //=== STAT LEVELING (D2) ===//

    public void openlevelupscreen()
    {
        
        dogselectionUI_D2?.SetActive(true);
        doglistcontainer_D2?.gameObject.SetActive(true);
        filldoglistforlevelup();
    }

    public void dogselectedforlevelUp(Dog dog)
    {


        selecteddog_D2 = dog;
        Debug.Log($"Dog selected for leveling up: {selecteddog_D2.dogname}");
        
        dogselectionUI_D2.SetActive(false);
        doglistcontainer_D2.gameObject.SetActive(false);
        statscreen_D2.SetActive(true);
        fillstatlevelUpUI(selecteddog_D2);
    }

    public void fillstatlevelUpUI(Dog dog)
    {
        DogStats dogStats = dog?.GetComponent<DogStats>();
        Transform buttonContainer = statscreen_D2?.transform.Find("StatButtonsContainer");



        fillstatbuttons(buttonContainer, dogStats.stats, statlevelupclicked);
    }

    public void statlevelupclicked(string statName)
    {
        if (LM.pppoints < 50)
        {
            Debug.LogError("Insufficient Paw-Sawtive Points.");
            return;
        }

        LM.subtractpppoints(50);
        updatepppointstext();
        AudioManager.Instance.playlevelup();

        if (!updatestat(selecteddog_D2, statName))//calls the updatestat bool method
        {
            return;
        }
        Debug.Log($"Leveled up {statName} for {selecteddog_D2.dogname}.");

        statscreen_D2?.SetActive(false);
    }

    //=== SHARED FUNCTIONS ===//
    
    public void filldoglist()
    {
        filldogbuttons(doglistcontainer_D1, LogicManager.Instance.dogsawake, dogselected);
    }

    private void filldoglistforlevelup()
    {
        filldogbuttons(doglistcontainer_D2, LogicManager.Instance.dogsawake, dogselectedforlevelUp);
    }

    private void filldogbuttons(Transform container, List<Dog> dogs, UnityEngine.Events.UnityAction<Dog> callback)
    {
        clearcontainer(container);

        foreach (Dog dog in dogs)
        {
            createdogbutton(container, dog, callback);
        }
    }

    private void fillstatbuttons(Transform container, Dictionary<string, int> stats, UnityEngine.Events.UnityAction<string> callback)
    {
        foreach (Transform child in container)
        {
            Button button = child.GetComponent<Button>();
            if (button == null)
                continue;

            string statName = child.name.Replace("Button", "").ToLower();
            button.gameObject.SetActive(stats.ContainsKey(statName));
            button.onClick.RemoveAllListeners();

            if (stats.ContainsKey(statName))
            {
                button.onClick.AddListener(() => callback(statName));
            }
        }
    }

    private void createdogbutton(Transform container, Dog dog, UnityEngine.Events.UnityAction<Dog> callback)//this is not the button that spawns a dog!
    {
        GameObject buttonObj = Instantiate(dogbuttonprefab, container);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
        {
            text.text = dog.dogname;
            text.color = Color.black;
            text.fontSize = 20;
        }

        button.onClick.AddListener(() => callback(dog));
    }

    private bool updatestat(Dog dog, string statName)
    {
        DogStats dogStats = dog?.GetComponent<DogStats>();
        
        dogStats.stats[statName] += 1;
        return true;
    }

   
    private void clearcontainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    

    public void updatepppointstext()
    {
       
        PPpointsText.text = $"{LM.pppoints}";
    }

}







