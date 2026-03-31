using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public string scenesellected;

    public void StartGame()
    {
        if (scenesellected != null)
        {
            for (int i = 0; true; i++)
            {
                string scene = SceneUtility.GetScenePathByBuildIndex(i);
                Debug.Log(scene);
                Debug.Log(scenesellected);
                if (scene == "")
                {
                    break;
                }
                if (scene == "Assets/Scenes/"+scenesellected+".unity") 
                { 
                    SceneManager.LoadScene(i);
                    break;
                }
            }
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
