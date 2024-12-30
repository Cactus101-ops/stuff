using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BaseDog : MonoBehaviour
{
    public string dogname { get; set; }
    public LogicManager LM;
    public ActionManager AM;
    
    [SerializeField] protected int Laziness;
    [SerializeField] protected int Playfulness;
    [SerializeField] protected int Boisterous;
    [SerializeField] protected int Smarts;
    [SerializeField] public string Name = "";
    


   


   
}
