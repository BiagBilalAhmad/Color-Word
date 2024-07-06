using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening; // Import DOTween namespace
using UnityEngine.EventSystems;

public class InputFieldManager : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public Button[] keyboardButtons;
    public string correctWord = "unity"; // Change this to your correct word
    private int correctCount = 0;
    private bool[] correctGuesses; // Array to track correct guesses
    public TMP_Text WordCountTxt, GuessWordCount;
    public WordScrambler wordScrambler;
    public ParticleSystem Confetti, Confetti1;
    public GameObject RestartButon;
    public Timer timr;
    private TMP_InputField activeInputField; // Track the active input field
    private int currentActiveInputFieldIndex; // Track the index of the active input field
    public int activeInputFields;

    public GameObject Keys;

    public GameObject pauseMenu;

    void Start()
    {
        ResumeGame();

        // Ensure EventSystem exists
        if (!EventSystem.current)
        {
            Debug.LogError("No EventSystem found in the scene.");
            return;
        }
        // Set up event listeners for keyboard buttons
        for (int i = 0; i < keyboardButtons.Length; i++)
        {
            int index = i; // Capture current index
            keyboardButtons[i].onClick.AddListener(delegate { OnKeyboardButtonClick(index); });
        }
    }

    void Update()
    {
        // Check for mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject != null)
            {
                TMP_InputField clickedInputField = selectedObject.GetComponent<TMP_InputField>();
                if (clickedInputField != null)
                {
                    // Find the index of the clicked input field
                    for (int i = 0; i < inputFields.Length; i++)
                    {
                        if (inputFields[i] == clickedInputField)
                        {
                            currentActiveInputFieldIndex = i;
                            activeInputField = inputFields[i];
                            Debug.Log("Clicked input field: " + i);
                            break;
                        }
                    }
                }
                else
                {
                    SetInputFieldFocus(currentActiveInputFieldIndex);
                }
            }
            else
            {
                SetInputFieldFocus(currentActiveInputFieldIndex);
            }
        }

        // Check for arrow key input
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveFocusLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveFocusRight();
        }
    }

    public void Initialise()
    {
        correctWord = wordScrambler.CorrectWord();
        WordCountTxt.text = correctWord.Length.ToString();
        GuessWordCount.text = correctCount + "/" + WordCountTxt.text;

        // Initialize correctGuesses array
        correctGuesses = new bool[correctWord.Length];

        // Set up event listeners for input fields
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i; // Capture current index
            inputFields[i].onValueChanged.AddListener(delegate { OnInputValueChanged(index); });
            inputFields[i].characterLimit = 1; // Set character limit to 1
        }

        // Set focus to the first input field
        SetInputFieldFocus(0);
    }

    void OnInputValueChanged(int index)
    {
        if (index < activeInputFields)
        {
            // Check if the entered character matches the correct character
            if (inputFields[index].text == correctWord[index].ToString())
            {
                SoundManager.instance?.PlayCorrectClickSound();
                // Check if this character has already been marked as correct
                if (!correctGuesses[index])
                {
                    // Increment correct count only if it's a new correct guess
                    correctCount++;
                    correctGuesses[index] = true;
                    Debug.Log("Correct guesses: " + correctCount);
                    GuessWordCount.text = correctCount + "/" + WordCountTxt.text;
                }
            }
            else
            {
                SoundManager.instance?.PlayWrongClickSound();
                // If the character was previously marked as correct, decrement the correct count
                if (correctGuesses[index])
                {
                    correctCount--;
                    correctGuesses[index] = false;
                    Debug.Log("Correct guesses: " + correctCount);
                    GuessWordCount.text = correctCount + "/" + WordCountTxt.text;
                }

                // Shake the input field
                ShakeInputField(inputFields[index]);
            }

            // Move to the next input field when a character is entered
            if(!Input.GetKeyDown(KeyCode.Backspace))
                SetInputFieldFocus(index + 1);
            else
            {
                if(index > 0)
                    SetInputFieldFocus(index - 1);
            }

            // If all input fields have been filled
            if (correctCount == correctWord.Length)
            {
                Debug.Log("Congratulations! You guessed the correct word.");
                Confetti.Play();
                Confetti1.Play();
                RestartButon.SetActive(true);
                timr.enabled = false;
                Keys.SetActive(false);
                // Add your logic for what happens when the word is guessed correctly
            }
            else
            {
                Debug.Log("Try again! You have " + correctCount + " correct guesses.");
                // Add your logic for what happens when the word is not guessed correctly
            }
        }
    }

    void OnKeyboardButtonClick(int index)
    {
        if (activeInputField != null)
        {
            //if (index == 26) // Backspace button
            //{
            //    if (activeInputField.text.Length > 0)
            //    {
            //        activeInputField.text = activeInputField.text.Substring(0, activeInputField.text.Length - 1);
            //    }
            //}
            //else
            //{
            // Clear the existing text before appending the new character
            activeInputField.text = keyboardButtons[index].GetComponentInChildren<TMP_Text>().text;
            //}

            activeInputField.ActivateInputField();
        }
    }

    void SetInputFieldFocus(int index)
    {
        if (index < activeInputFields)
        {
            inputFields[index].Select();
            inputFields[index].ActivateInputField();
            activeInputField = inputFields[index]; // Set the active input field
            currentActiveInputFieldIndex = index; // Update the active input field index
        }
    }

    void MoveFocusLeft()
    {
        if (currentActiveInputFieldIndex > 0)
        {
            SetInputFieldFocus(currentActiveInputFieldIndex - 1);
        }
    }

    void MoveFocusRight()
    {
        if (currentActiveInputFieldIndex < activeInputFields - 1)
        {
            SetInputFieldFocus(currentActiveInputFieldIndex + 1);
        }
    }

    void ShakeInputField(TMP_InputField inputField)
    {
        // Shake the input field using DOTween
        inputField.transform.DOShakePosition(0.5f, new Vector3(10, 0, 0), 10, 90, false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void GotoHomePage()
    {
        SceneManager.LoadScene(0);
    }
}
