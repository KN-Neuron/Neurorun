using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
     
    }

    public static void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public static void LoadNextLevel()
    {
        string currentLevelName = SceneManager.GetActiveScene().name;
        try
        {
            int currentLevelIndex = int.Parse(currentLevelName.Substring(currentLevelName.Length - 1));
            if (currentLevelIndex < PlayerPrefs.GetInt("NumberOfLevels"))
            {
                SceneManager.LoadScene("Level" + (currentLevelIndex + 1));
            }
            else
            {
                SceneManager.LoadScene("Levels");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            SceneManager.LoadScene("Levels");
        }



     
   
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        
    }


    public static void AppQuit()
    {
        Application.Quit();
    }
}
