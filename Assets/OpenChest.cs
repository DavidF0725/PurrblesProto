using UnityEngine;


public class OpenChest : MonoBehaviour
{
    public GameObject chestOpen, chestClose;

    private void Start()
    {
        chestClose.SetActive(true);
        chestOpen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        chestClose.SetActive(false);
        chestOpen.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        chestClose.SetActive(true);
        chestOpen.SetActive(false);
    }
}
