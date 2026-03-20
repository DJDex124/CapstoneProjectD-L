using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);  
        SceneManager.LoadScene("UIManager", LoadSceneMode.Additive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene("MazeGeneration", LoadSceneMode.Additive);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene("InventorySystem", LoadSceneMode.Additive);
        }
    }
}
