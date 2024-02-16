using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite bgImage;
    public List<Button> btns = new List<Button>();
    public Sprite[] puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    private bool firstGuess, secondGuess;
    private int countGuess;
    private int countCorrectGuess;
    private int gameGuess;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;
    public TextMeshProUGUI totalAttemptsText;
    public TextMeshProUGUI correctGuessesText;
    public TextMeshProUGUI timerText;
    public float gameDuration = 10.0f; 
    private float remainingTime;


    private void Awake()
    {
        puzzles = Resources.LoadAll<Sprite>("Sprite/Food");
    }

    void Start()
    {
        GetButton();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuess = gamePuzzles.Count / 2;
        remainingTime = gameDuration;
        UpdateTimerUI();
        StartCoroutine(StartGameTimer());

    }

    void GetButton()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickAPuzzle());
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;
        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    public void PickAPuzzle()
    {
        string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
            StartCoroutine(CheckIfThePuzzlesMatch());
            countGuess++;
            totalAttemptsText.text = "Total Attempts: " + countGuess;
        }
    }

    IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(.5f);
        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.5f);
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;
            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);
            AudioSource correctMatchSound = new GameObject("CorrectMatchSound").AddComponent<AudioSource>();
            correctMatchSound.clip = Resources.Load<AudioClip>("Sprite/Correct");
            correctMatchSound.Play();
            CheckIfTheGameisFinished();
        }
        else
        {
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
            AudioSource incorrectMatchSound = new GameObject("IncorrectMatchSound").AddComponent<AudioSource>();
            incorrectMatchSound.clip = Resources.Load<AudioClip>("Sprite/Incorrect");
            incorrectMatchSound.Play();
        }

        yield return new WaitForSeconds(.5f);
        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameisFinished()
    {
        countCorrectGuess++;
        correctGuessesText.text = "Correct Guesses: " + countCorrectGuess;

        if (countCorrectGuess >= gameGuess)
        {
            int currentSceneindex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneindex + 1);
        }
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    IEnumerator StartGameTimer()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            remainingTime -= 1.0f;
            UpdateTimerUI();

            if (remainingTime <= 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.Ceil(remainingTime).ToString();
    }
}
