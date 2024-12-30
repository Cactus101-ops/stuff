using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DogMover : MonoBehaviour
{
    public float movespeed = 1f; 
    public Transform targetlocation; 
    private bool moving = false; 
    public Dog associatedDog; 

    void Update()
    {
        if (moving && targetlocation != null)
        {
            Vector2 currentPos = transform.position;
            Vector2 targetPos = targetlocation.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPos, targetPos, movespeed * Time.deltaTime);

            Debug.Log($"Current Position: {currentPos} | Target Position: {targetPos} | Moving To: {newPosition}");

            transform.position = newPosition;

            // If the dog is close enough to the target, stop the movement
            if (Vector2.Distance(currentPos, targetPos) < 0.1f)
            {
                Debug.Log($"Dog {associatedDog.Name} reached the target point!");
                moving = false;
                reachedtarget(associatedDog);
            }
        }


    }

    private void reachedtarget(Dog dog)
    {
        Debug.Log($"{dog.name} reached the target point!");
    }
}

