using UnityEngine;

public class ScenePersist : MonoBehaviour
{
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

    public void ResetScenePersist()
    {
        Destroy(gameObject);
    }
}
