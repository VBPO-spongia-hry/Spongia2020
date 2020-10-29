using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject menuUI;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Slider hungerSlider;
    public TextMeshProUGUI hungerText;
    public Slider infectionSlider;
    public TextMeshProUGUI infectionText;
    private TabUI _menuTabs;
    public PlayerVitals vitals;
    public Button interactionButton;
    public static UIController Instance;

    private void OnEnable()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        _menuTabs = menuUI.GetComponent<TabUI>();
        hungerSlider.maxValue = vitals.hunger;
        infectionSlider.maxValue = vitals.infection;
        interactionButton.onClick.AddListener(() => PlayerMovement._interactable?.Interact());
    }

    private void Update()
    {
        healthSlider.value = vitals.health;
        healthSlider.maxValue = vitals.maxHealth;
        healthText.SetText($"{healthSlider.value}/{healthSlider.maxValue}");
        hungerSlider.value = vitals.hunger;
        hungerText.SetText($"{hungerSlider.value}/{hungerSlider.maxValue}");
        infectionSlider.value = vitals.infection;
        infectionText.SetText($"{infectionSlider.value}/{infectionSlider.maxValue}");
        interactionButton.interactable = PlayerMovement._interactable != null;
    }

    public void ShowMap()
    {
        menuUI.SetActive(true);
        _menuTabs.Switch(0);
    }

    public void ShowMissions()
    {
        menuUI.SetActive(true);
        _menuTabs.Switch(1);
    }

    public void ShowInventory()
    {
        menuUI.SetActive(true);
        _menuTabs.Switch(2);
    }

    public void HideUI()
    {
        menuUI.SetActive(false);
    }
}