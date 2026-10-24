using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Zeldinha.UI
{
    /// <summary>
    /// Controla a HUD (vida, stamina, fragmentos).
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Barras")]
        public Image healthFill;
        public Image staminaFill;

        [Header("Textos")]
        public TextMeshProUGUI fragmentText;
        public TextMeshProUGUI messageText;

        Player.PlayerStats stats;

        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                stats = player.GetComponent<Player.PlayerStats>();
                if (stats != null)
                {
                    stats.OnHealthChanged += UpdateHealth;
                    stats.OnStaminaChanged += UpdateStamina;
                    UpdateHealth(stats.currentHealth, stats.maxHealth);
                    UpdateStamina(stats.currentStamina, stats.maxStamina);
                }
            }

            if (Systems.GameManager.Instance != null)
            {
                Systems.GameManager.Instance.OnFragmentCollected += UpdateFragments;
                UpdateFragments(Systems.GameManager.Instance.lightFragmentsCollected);
            }

            if (messageText != null) messageText.text = "";
        }

        void UpdateHealth(float current, float max)
        {
            if (healthFill != null)
                healthFill.fillAmount = current / max;
        }

        void UpdateStamina(float current, float max)
        {
            if (staminaFill != null)
                staminaFill.fillAmount = current / max;
        }

        void UpdateFragments(int count)
        {
            if (fragmentText != null)
                fragmentText.text = $"Fragmentos de Luz: {count}/3";
        }

        public void ShowMessage(string msg, float duration = 2.5f)
        {
            if (messageText == null) return;
            StopAllCoroutines();
            StartCoroutine(MessageRoutine(msg, duration));
        }

        System.Collections.IEnumerator MessageRoutine(string msg, float duration)
        {
            messageText.text = msg;
            yield return new WaitForSeconds(duration);
            messageText.text = "";
        }

        void OnDestroy()
        {
            if (stats != null)
            {
                stats.OnHealthChanged -= UpdateHealth;
                stats.OnStaminaChanged -= UpdateStamina;
            }
        }
    }
}
