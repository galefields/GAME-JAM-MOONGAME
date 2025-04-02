using UnityEngine;
using UnityEngine.SceneManagement;
public class MainmenuControls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("Start Button Clicked!");
        SceneManager.LoadScene("StartScene");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Button Clicked!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
