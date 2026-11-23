using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Serializable]
    public class SceneTimer
    {
        public string sceneName;
        public int timeLimit = 120;
    }

    [SerializeField] int playerLives = 3;
    [SerializeField] int playerScore = 0;
    [SerializeField] int timeLeft = 120;
    [SerializeField] SceneTimer[] sceneTimers;
    [SerializeField] AudioClip gameSessionMusic;

    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;

    AudioSource musicSource;
    float currentTimeLeft;
    bool isChangingScene = false;

    void Awake()
    {
        int gameSessionCount = FindObjectsByType<GameSession>(FindObjectsSortMode.None).Length;

        if (gameSessionCount > 1)
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
        livesText.text = "Lives: " + playerLives.ToString();
        scoreText.text = "Score: " + playerScore.ToString();

        musicSource = GetComponent<AudioSource>();
        musicSource.clip = gameSessionMusic;
        musicSource.Play();

        SetSceneTime(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (isChangingScene) { return; }

        currentTimeLeft -= Time.deltaTime;
        currentTimeLeft = Mathf.Max(currentTimeLeft, 0f);

        timeText.text = "Time: " + Mathf.CeilToInt(currentTimeLeft).ToString();

        if (currentTimeLeft <= 0f)
        {
            TimeRanOut();
        }
    }

    void TimeRanOut()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();

        if (player != null)
        {
            player.KillPlayer();
        }
        else
        {
            ProcessPlayerDeath();
        }
    }

    void SetSceneTime(string sceneName)
    {
        currentTimeLeft = timeLeft;

        foreach (SceneTimer timer in sceneTimers)
        {
            if (timer.sceneName == sceneName)
            {
                currentTimeLeft = timer.timeLimit;
                break;
            }
        }

        timeText.text = "Time: " + Mathf.CeilToInt(currentTimeLeft).ToString();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "End Screen")
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }

            Destroy(gameObject);
            return;
        }

        if (scene.name == "Game Over Screen") { return; }

        isChangingScene = false;
        SetSceneTime(scene.name);

        livesText.text = "Lives: " + playerLives.ToString();
        scoreText.text = "Score: " + playerScore.ToString();
    }

    public void ProcessPlayerDeath()
    {
        if (isChangingScene) { return; }

        isChangingScene = true;

        if (playerLives >= 1)
        {
            TakeLife();
        }
        else
        {
            musicSource.Stop();
            StartCoroutine(WaitAndLoadGameOver(2f));
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        playerScore += pointsToAdd;
        scoreText.text = "Score: " + playerScore.ToString();
    }

    private void TakeLife()
    {
        playerLives = playerLives - 1;
        musicSource.Stop();
        StartCoroutine(WaitAndReloadScene(2f));
        livesText.text = "Lives: " + playerLives.ToString();
    }

    IEnumerator WaitAndReloadScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        musicSource.Play();
    }

    IEnumerator WaitAndLoadGameOver(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        musicSource.Stop();
        SceneManager.LoadScene("Game Over Screen");
        Destroy(gameObject);
    }
}