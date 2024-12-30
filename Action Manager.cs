using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ActionManager : MonoBehaviour
{
    [Header("class references")]
    public UIManager UI;  
    public Cat linkedcat; 
    public DogMover DM; 
    public LogicManager LM;
    public ColliderLink colliderlink; 

    public Transform correspondingPoint;


    //=== player clicking on a cat ===\\

    public void setcat(Cat cat)
    {

        linkedcat = cat;
        Debug.Log($"Linked cat {cat.name} to container {gameObject.name}.");
    }


    private void OnMouseDown()
    {
        if (linkedcat == null)
        {
            Debug.LogError("No cat is linked to this container!");
            return;
        }
        LM.settarget(correspondingPoint);
        UIManager.instance.catselected(linkedcat);
        LogicManager.Instance.setcollider(this.gameObject);
       
    }
    //=== method to link a cat to it's container ===\\
   


    
}

