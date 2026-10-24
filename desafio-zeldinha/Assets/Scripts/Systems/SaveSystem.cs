using UnityEngine;

namespace Zeldinha.Systems
{
    /// <summary>
    /// Sistema simples de save usando PlayerPrefs.
    /// </summary>
    public static class SaveSystem
    {
        const string KEY_FRAGMENTS = "LightFragments";
        const string KEY_HEALTH = "PlayerHealth";
        const string KEY_STAMINA = "PlayerStamina";
        const string KEY_SCENE = "CurrentScene";

        public static void SaveProgress()
        {
            if (GameManager.Instance == null) return;

            PlayerPrefs.SetInt(KEY_FRAGMENTS, GameManager.Instance.lightFragmentsCollected);

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var stats = player.GetComponent<Player.PlayerStats>();
                if (stats != null)
                {
                    PlayerPrefs.SetFloat(KEY_HEALTH, stats.currentHealth);
                    PlayerPrefs.SetFloat(KEY_STAMINA, stats.currentStamina);
                }
            }

            PlayerPrefs.SetString(KEY_SCENE, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            Debug.Log("Progresso salvo!");
        }

        public static void LoadProgress()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.lightFragmentsCollected = PlayerPrefs.GetInt(KEY_FRAGMENTS, 0);

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var stats = player.GetComponent<Player.PlayerStats>();
                if (stats != null)
                {
                    stats.currentHealth = PlayerPrefs.GetFloat(KEY_HEALTH, stats.maxHealth);
                    stats.currentStamina = PlayerPrefs.GetFloat(KEY_STAMINA, stats.maxStamina);
                }
            }

            Debug.Log("Progresso carregado!");
        }

        public static void ClearSave()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        public static bool HasSave() => PlayerPrefs.HasKey(KEY_FRAGMENTS);
    }
}
