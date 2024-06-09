using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class WordScrambler : MonoBehaviour
{
   // public List<string> wordsList; // List of words to choose from
    public TMP_Text scrambledWordText; // Reference to TMP Text component to display scrambled word
    public float characterSpacing = 5f; // Adjustable character spacing

    private string selectedWord; // The word selected from the list
    private string scrambledWord; // The scrambled version of the selected word
    private string scrambledWordforColor; // The scrambled version of the selected word
    private float missingCharacterProbability = 0.5f;
    private int maxMissingCharacters = 3;

    public GameObject[] Inputfields;
    public InputFieldManager InputfieldManager;
    public HardModeInputFieldManager HardInputfieldManager;
    public List<string> wordsList;

    void Start()
    {
        LoadWordsFromJSON();
        SelectWord();
        ScrambleWord();
        DisplayScrambledWord();
        InputfieldManager?.Initialise();
        HardInputfieldManager?.Initialise();
    }
    void LoadWordsFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "words.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WordList wordList = JsonUtility.FromJson<WordList>(json);

            // Assign the words from JSON to the wordsList
            wordsList = wordList.words;
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
    void SelectWord()
    {
        // Randomly select a word from the list
        selectedWord = wordsList[Random.Range(0, wordsList.Count)];
    }

    public string CorrectWord()
    {
        return selectedWord;
    }

    void ScrambleWord()
    {
        // Convert the selected word to a char array
        char[] chars = selectedWord.ToCharArray();

        // Determine the number of characters to be missing based on the word length
        int missingCount = Mathf.Min(Mathf.FloorToInt(chars.Length / 3f), maxMissingCharacters);

        // Keep track of which characters should not be scrambled
        HashSet<int> nonScrambledIndices = new HashSet<int>();

        // Add the first one or two indices to the non-scrambled set
        for (int i = 0; i < Mathf.Min(2, chars.Length); i++)
        {
            nonScrambledIndices.Add(i);
        }

        // Randomly shuffle characters except for the non-scrambled ones
        for (int i = 0; i < chars.Length; i++)
        {
            if (!nonScrambledIndices.Contains(i))
            {
                int randomIndex = Random.Range(i, chars.Length);
                char temp = chars[randomIndex];
                chars[randomIndex] = chars[i];
                chars[i] = temp;
            }
        }

        // Add input fields for all characters
        for (int i = 0; i < chars.Length; i++)
        {
            Inputfields[i].SetActive(true);
            if (InputfieldManager)
                InputfieldManager.activeInputFields++;
            if (HardInputfieldManager)
                HardInputfieldManager.activeInputFields++;
        }

        // Convert the char array back to a string
        scrambledWord = new string(chars);
       // Debug.Log(scrambledWord.Length);
    }


    void DisplayScrambledWord()
    {
        // Clear previous display
        scrambledWordText.text = "";

        // Define color mapping based on distance from correct position
        Dictionary<int, Color> colorMap = new Dictionary<int, Color>()
        {
            {-5, new Color(213f/255f, 0f, 3f/255f)},   // Dark red
            {-4, new Color(255f/255f, 104f/255f, 48f/255f)},   // Dark orange
            {-3, new Color(255f/255f, 163f/255f, 25f/255f)},   // Light Orange
            //{-3, new Color(255f/255f, 165f/255f, 45f/255f)},   // Light Orange
            {-2, new Color(255f/255f, 247f/255f, 89f/255f)},   // Yellow
            //{-2, new Color(255f/255f, 247f/255f, 89f/255f)},   // Yellow
            {-1, new Color(165f/255f, 252f/255f, 3f/255f)},  // Light Green
           // {-1, new Color(192f/255f, 255f/255f, 114f/255f)},  // Light Green
            {1, new Color(82f/255f, 252f/255f, 3f/255f)},    // Green
            //{1, new Color(126f/255f, 217f/255f, 87f/255f)},    // Green
            {2, new Color(93f/255f, 225f/255f, 230f/255f)},    // Turquoise
            {3, new Color(55f/255f, 182f/255f, 255f/255f)},    // Light blue
            {4, new Color(6f/255f, 128f/255f, 255f/255f)},     // Mid blue
            {5, new Color(0f, 79f/255f, 255f/255f)},           // Dark blue
            {-6, new Color(213f/255f,0f, 3f/255f)},          // Default color for distances less than -5
            {6, new Color(0f, 79f/255f, 255f/255f)}            // Default color for distances greater than 5
        };

        // Iterate through the original word
        for (int i = 0; i < selectedWord.Length; i++)
        {
            char correctChar = selectedWord[i];

            // Check if the character is missing
            if (scrambledWord.Length > i && scrambledWord[i] != ' ')
            {
                char scrambledChar = scrambledWord[i];

                // Calculate distance between scrambled character and correct character
                int distance = scrambledChar - correctChar;

                // Get color based on distance from correct position
                Color color;
                if (colorMap.ContainsKey(distance))
                {
                    color = colorMap[distance];
                }
                else if (distance > 5 || distance < -5)
                {
                    // Use default color for distances greater than 5 or less than -5
                    color = colorMap[distance > 5 ? 6 : -6];
                }
                else
                {
                    // Use default color (white) if distance is not in the color map
                    color = Color.white;
                }

                // Append character to the text with the appropriate color and underline
                scrambledWordText.text += "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + scrambledChar + "</color>";
            }
            else
            {
                // If character is missing, display a placeholder (e.g., '_') with underline
                scrambledWordText.text += "<u>_</u>";
            }

            // Add adjustable space between characters
            scrambledWordText.characterSpacing = characterSpacing;
        }
    }


    [System.Serializable]
    public class WordList
    {
        public List<string> words;
    }
}



