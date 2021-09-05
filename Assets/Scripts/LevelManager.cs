using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    Vector2 playerInitialPosition;

    private void Start()
    {
        playerInitialPosition = FindObjectOfType<Fox>().transform.position;
    }

    public void Restart()
    {
        //1 Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        FindObjectOfType<Fox>().ResetPlayer();
        FindObjectOfType<Fox>().transform.position = playerInitialPosition;
    }
}
