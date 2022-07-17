using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("exit");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex + 1);
    }
}
