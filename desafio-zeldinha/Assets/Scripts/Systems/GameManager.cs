using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Zeldinha.Systems
{
    /// <summary>
    /// Gerenciador central do jogo. Controla estado global, fragmentos de luz e fluxo de cenas.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Progresso")]
        public int lightFragmentsCollected = 0;
        public const int TOTAL_FRAGMENTS = 3;

        [Header("Referências")]
        public GameObject player;
        public Transform playerSpawnPoint;

        public enum GameState { Playing, Paused, Dialogue, BossFight, Victory, GameOver }
        public GameState CurrentState { get; private set; } = GameState.Playing;

        public event System.Action<int> OnFragmentCollected;
        public event System.Action OnAllFragmentsCollected;
        public event System.Action OnPlayerDeath;
        public event System.Action OnVictory;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }

        public void CollectLightFragment()
        {
            lightFragmentsCollected = Mathf.Min(lightFragmentsCollected + 1, TOTAL_FRAGMENTS);
            OnFragmentCollected?.Invoke(lightFragmentsCollected);

            Debug.Log($"Fragmento de Luz coletado! Total: {lightFragmentsCollected}/{TOTAL_FRAGMENTS}");

            if (lightFragmentsCollected >= TOTAL_FRAGMENTS)
            {
                OnAllFragmentsCollected?.Invoke();
            }
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                case GameState.Dialogue:
                    Time.timeScale = 0f;
                    break;
                case GameState.BossFight:
                    Time.timeScale = 1f;
                    break;
                case GameState.Victory:
                    Time.timeScale = 0.3f;
                    OnVictory?.Invoke();
                    StartCoroutine(LoadVictorySceneAfterDelay(3f));
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    OnPlayerDeath?.Invoke();
                    break;
            }
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
                SetState(GameState.Paused);
            else if (CurrentState == GameState.Paused)
                SetState(GameState.Playing);
        }

        public void PlayerDied()
        {
            SetState(GameState.GameOver);
        }

        public void BossDefeated()
        {
            SetState(GameState.Victory);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        IEnumerator LoadVictorySceneAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1f;
            // SceneManager.LoadScene("Victory");
            Debug.Log("Vitória! Carregar cena de créditos/vitória.");
        }

        public bool HasAllFragments() => lightFragmentsCollected >= TOTAL_FRAGMENTS;
    }
}
