using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);  
        SceneManager.LoadScene("UIManager", LoadSceneMode.Additive);
    }
}
