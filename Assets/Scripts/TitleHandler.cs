using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TitleHandler : MonoBehaviour
{
    public static TitleHandler Instance;

    [SerializeField] private Text highScores;
    [SerializeField] private Text highestScore;

    private HighScoresData highScoresData;

    private void Start()
    {
        highScoresData = new HighScoresData();
        LoadHighScores();
    }

    void Awake()
    {
        LoadHighScores();
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

//        LoadHighScores();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartGame();
        }
    }
    void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    [System.Serializable]
    class HighScoresData
    {
        public int[] highScores = new int[10];
        public string[] highScore_names = new string[10];
        public string highestScore_quote;
    }

    void DisplayHighScores()
    {
        Debug.Log("Disp");
        int[] scores = highScoresData.highScores;
        string[] names = highScoresData.highScore_names;
        string output = "";
        // display 2-10
        for ( int i = 1; i < scores.Length; i++)
        {
            int s = scores[i];
            string n = names[i];

            //Debug.Log(s);

            if ( n == null )
            {
                n = "AAA";

            }

            output += (i+1)+". "+ n + " - " + s + "\n";
        }

        highScores.text = output;

        int hs = scores[0];
        string hn = names[0];

        if ( hn == null)
        {
            hn = "AAA";
        }

        highestScore.text = "Highest Score\n1. "+hn + " - " + hs + "\n\"" + highScoresData.highestScore_quote +'"';
    }

    // put the given score in the top ten
    public void AddHighScore( int position, int score, string name )
    {
        Debug.Log("Add HS");
        for(int i=highScoresData.highScores.Length-1; i>-1; i--)
        {
            if ( i == position )
            {
                highScoresData.highScores[i] = score;
                highScoresData.highScore_names[i] = name;
                break;
            }else if ( i > position)
            {
                highScoresData.highScores[i] = highScoresData.highScores[i - 1];
                highScoresData.highScore_names[i] = highScoresData.highScore_names[i - 1];
            }
        }

        SaveHighScores();
    }

    public int GetBestScore()
    {
        return highScoresData.highScores[0];
    }

    public string GetBestName()
    {
        return highScoresData.highScore_names[0];
    }

    public void UpdateHighestScore( string quote )
    {
        highScoresData.highestScore_quote = quote;
    }

    // check if the score given will beat one of the top ten - returns the position
    public int CheckHighScore( int score )
    {
        for ( int i = 0; i < highScoresData.highScores.Length; i++)
        {
            int s = highScoresData.highScores[i];
            if ( score > s)
            {
                return i;
            }
        }
        return -1;
    }

    void LoadHighScores()
    {
        Debug.Log("Load High Scores");
        string path = Application.persistentDataPath + "/highScores.json";

        if ( File.Exists(path))
        {
            string json = File.ReadAllText(path);
            highScoresData = JsonUtility.FromJson<HighScoresData>(json);
        }
        DisplayHighScores();
    }

    void SaveHighScores()
    {
        string json = JsonUtility.ToJson(highScoresData);
        File.WriteAllText(Application.persistentDataPath + "/highScores.json", json);
    }
}
