
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    [SerializeField] string[] destroyOnScenes = { "Game Over Screen" };

    void Awake()
    {
        int numScenePersist = FindObjectsByType<ScenePersist>(FindObjectsSortMode.None).Length;

        if (numScenePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        CheckScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene(scene.name);
    }

    void CheckScene(string sceneName)
    {
        foreach (string name in destroyOnScenes)
        {
            if (sceneName == name)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    public void ResetScenePersist()
    {
        Destroy(gameObject);
    }
}