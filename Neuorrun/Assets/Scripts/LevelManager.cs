using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridPanel;
    public int numberOfRows;
    public int numberOfColumns;
    public int totalLevels;
    public Button menuButton;
    public Button tutorialButton;
    public Button firstButton;
    public Button lastButton;
    public EventSystem eventSystem;
    private int currentLevelIndex;
    private Button lastSelectedLevelButton;

    void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        CreateLevelButtons();
        eventSystem.SetSelectedGameObject(firstButton.gameObject);
        UpdateMenuButtonNavigation();
    }

    void CreateLevelButtons()
    {
        Button previousButton = null;
        for (int i = 0; i < totalLevels; i++)
        {
            GameObject levelButton = Instantiate(buttonPrefab, gridPanel);
            levelButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            int levelIndex = i + 1;
            levelButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelIndex));

            // Set up navigation for the buttons
            Button currentButton = levelButton.GetComponent<Button>();
            Navigation currentNav = currentButton.navigation;

            

            if (i == 0)
            {
                firstButton = currentButton;
            }
            else
            {
                currentNav.selectOnLeft = previousButton;
                Navigation prevNav = previousButton.navigation;
                prevNav.selectOnRight = currentButton;
                previousButton.navigation = prevNav;
            }

            // Set up bottom navigation to the menuButton for all level buttons
            currentNav.selectOnDown = menuButton;

            

            if (i == totalLevels - 1)
            {
                lastButton = currentButton;
            }

            currentButton.navigation = currentNav;
            previousButton = currentButton;
        }
        Navigation lastNav = lastButton.navigation;
        lastNav.selectOnRight = menuButton;
        lastButton.navigation = lastNav;

        // Set up navigation for the menuButton
        Navigation menuNav = menuButton.navigation;
        menuNav.selectOnUp = firstButton;
        menuNav.selectOnLeft = lastButton;
        menuButton.navigation = menuNav;

        Navigation tutorialNav = tutorialButton.navigation;
        tutorialNav.selectOnUp = firstButton;
        tutorialButton.navigation = tutorialNav;

        SaveNumberOfLevels();
    }

    void SaveNumberOfLevels()
    {
        PlayerPrefs.SetInt("NumberOfLevels", totalLevels);
    }

    void LoadLevel(int levelIndex)
    {
        lastSelectedLevelButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        SceneManager.LoadScene("Level" + levelIndex);
    }

    void UpdateMenuButtonNavigation()
    {
        if (lastSelectedLevelButton != null)
        {
            Navigation menuNav = menuButton.navigation;
            menuNav.selectOnUp = lastSelectedLevelButton;
            menuButton.navigation = menuNav;
        }
    }
}