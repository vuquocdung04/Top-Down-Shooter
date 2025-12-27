using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private Image healtBar;

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healtBar.fillAmount = currentHealth / maxHealth;
    }
}