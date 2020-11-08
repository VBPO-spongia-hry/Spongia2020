using System;
using Environment;
using Missions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public GameObject deathUI;
    public GameObject victoryUI;
    public AudioClip loseClip;
    public AudioClip loseMusic;
    public AudioClip winClip;
    public AudioClip winMusic;
    public AudioMixer mixer;

    
    private void OnEnable()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        mixer.SetFloat("FightSound", 0);
        _menuTabs = menuUI.GetComponent<TabUI>();
        _menuTabs.Init();
        deathUI.SetActive(false);
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

    public void Death()
    {
        deathUI.SetActive(true);
        var source = GetComponent<AudioSource>();
        source.clip = loseClip;
        source.Play();
        var music = Map.Instance.GetComponent<AudioSource>();
        mixer.SetFloat("FightSound", -80);
        music.clip = loseMusic;
        music.Play();
    }
    public void Victory()
    {
        foreach (var mission in MissionManager.Instance.missions)
        {
            mission.ResetProgress();
        }
        victoryUI.SetActive(true);
        var source = GetComponent<AudioSource>();
        source.clip = winClip;
        source.Play();
        var music = Map.Instance.GetComponent<AudioSource>();
        mixer.SetFloat("FightSound", -80);
        music.clip = winMusic;
        music.Play();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        InputHandler.DisableInput = false;
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        InputHandler.DisableInput = false;
    }
    
}