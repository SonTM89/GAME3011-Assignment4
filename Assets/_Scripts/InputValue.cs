using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputValue : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject Chest;

    public static Difficulty gameDifficulty;

    [SerializeField] private TextMeshProUGUI difficultyText;

    public static PlayerSkill playerSkill;

    [SerializeField] private TextMeshProUGUI playerSkillText;

    [SerializeField] private GameObject message;

    [SerializeField] private GameObject announcement;

    private bool showAnnouncement;

    // Start is called before the first frame update
    void Start()
    {
        showAnnouncement = false;

        gameDifficulty = Difficulty.NONE;

        playerSkill = PlayerSkill.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        difficultyText.text = Enum.GetName(typeof(Difficulty), (int)gameDifficulty);

        playerSkillText.text = Enum.GetName(typeof(PlayerSkill), (int)playerSkill);

        // Show Anouncement message when GameUI is up
        if (gameUI.activeInHierarchy)
        {
            if (showAnnouncement == false)
            {
                announcement.gameObject.SetActive(true);
                StartCoroutine(HideMessage(3.0f, announcement));

                showAnnouncement = true;
            }
        }
        else
        {
            showAnnouncement = false;
        }
    }


    // Hide message after specific time period
    IEnumerator HideMessage(float delay, GameObject message)
    {
        yield return new WaitForSeconds(delay);
        message.gameObject.SetActive(false);
    }


    // Press Start Button
    public void StartGame()
    {
        gameUI.gameObject.SetActive(true);
        Chest.gameObject.SetActive(true);
    }


    // Press Quit Button
    public void QuitGame()
    {
        gameUI.gameObject.SetActive(false);
        Chest.gameObject.SetActive(false);
        Application.Quit();
    }


    // Press Skill Randomize Button
    public void SkillRandomize()
    {
        int count = Enum.GetValues(typeof(Difficulty)).Length;
        int level = UnityEngine.Random.Range(1, count);

        playerSkill = (PlayerSkill)level;
        Debug.Log(playerSkill.ToString());
    }


    // Press Easy Button
    public void EasySelected()
    {
        gameDifficulty = Difficulty.EASY;
    }


    // Press Medium Button
    public void MediumSelected()
    {
        gameDifficulty = Difficulty.MEDIUM;
    }


    // Press Hard Button
    public void HardSelected()
    {
        gameDifficulty = Difficulty.HARD;
    }


    // Click on the Lock
    public void OnChestClicked()
    {
        // Go to lock simulation inside if Difficulty and Skill is selected
        // Otherwise, show alert message
        if (gameDifficulty != Difficulty.NONE && playerSkill != PlayerSkill.NONE)
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            message.gameObject.SetActive(true);
            StartCoroutine(HideMessage(3.0f, message));
        }
    }
}
