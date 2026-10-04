using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public static bool isLoadButtoned;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneFirst()
    {
        SceneLoad("MainScene");
    }
    public void LoadNamingScene()
    {
        SceneLoad("ChooseName");
    }

    public void LoadMainScene()
    {
        isLoadButtoned = true;

        SceneManager.LoadScene("MainScene");
        
    }

    public void titleSceneLoad()
    {
        SceneLoad("TitleScene");

    }

    void SceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void MainSceneLoad()
    {
        SceneManager.LoadScene("MainScene");
    }
}
