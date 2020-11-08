using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabUI : MonoBehaviour
{
    public Button[] tabButtons;
    public GameObject[] tabs;
    public Color selectedColor;
    public Color deselectedColor;

    // Start is called before the first frame update
     public void Init()
    {
        Switch(0);
       /* for (int i = 0; i < TabButtons.Length; i++)
        {
            TabButtons[i].onClick.AddListener(() => Switch(i));
        }*/
    }
    public void Switch(int tabIndex){
        foreach (var tab in tabs)
        {
            tab.SetActive(false);
        }
        foreach (var button in tabButtons)
        {
            button.GetComponent<Image>().color = deselectedColor;
//            button.GetComponentInChildren<Text>().color = Color.white;
        }
        tabs[tabIndex].SetActive(true);
        tabButtons[tabIndex].GetComponent<Image>().color = selectedColor;
//        tabButtons[tabIndex].GetComponentInChildren<Text>().color = Color.black;
    }
}
