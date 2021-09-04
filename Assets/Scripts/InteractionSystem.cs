using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    [Header("Detection Parameters")]
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;
    //Cached Trigger Object
    public GameObject detectObject;
    [Header("Examine Fields")]
    //Examine Window
    public GameObject examineWindow;
    public Image examineImage;
    public Text examineText;
    public bool isExamining;

    [Header("Others")]
    //List of Picked items
    public List<GameObject> pickedItems = new List<GameObject>();


    void Update()
    {
        if(DetectObject())
        {
            if(InteractInput())
            {
                detectObject.GetComponent<Item>().Interact();
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    bool DetectObject()
    {

        Collider2D obj = 
            Physics2D.OverlapCircle(detectionPoint.position,detectionRadius,detectionLayer);
        if(obj==null)
        {
            detectObject = null;
            return false;
        }
        else
        {
            detectObject = obj.gameObject;
            return true;
        }
    }

    public void PickUpItem(GameObject item)
    {
        pickedItems.Add(item);
    }

    public void ExamineItem(Item item)
    {
        if(isExamining)
        {
            //Hide Examine window
            examineWindow.SetActive(false);
            //disable the boolean
            isExamining = false;
        }
        else
        {
            //Show the item's image in the middle of screen
            examineImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
            //Write description text underneath the image
            examineText.text = item.descriptionText;
            //Display an Examine window
            examineWindow.SetActive(true);
            //enable the boolean
            isExamining = true;
        }
    }
}
