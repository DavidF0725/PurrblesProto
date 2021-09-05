using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{

    //Interaction Type
    public enum InteractionType { NONE, Pickup, Examine }
    public InteractionType type;
    [Header("Examine")]
    public string descriptionText;

    //Script that only gets called in the inspector
    //Collider Trigger
    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 10;
    }

    public void Interact()
    {
        switch(type)
        {
            case InteractionType.Pickup:
                //Add the object to the PickedUpItems list
                FindObjectOfType<InteractionSystem>().PickUpItem(gameObject);
                //Disable
                gameObject.SetActive(false);
                break;
            case InteractionType.Examine:
                //Call the Examine item in the interactionSystem
                FindObjectOfType<InteractionSystem>().ExamineItem(this);
                break;
            default:
                Debug.Log("NULL ITEM");
                break;
        }
    }

}
