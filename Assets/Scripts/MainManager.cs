using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public Text BestScoreText;
    
    private bool m_Started = false;
    private int m_Points;
    private int m_rank;
    private string m_Name;

    private bool m_GameOver = false;
    private bool scoreProcessed = false;

    [SerializeField] private GameObject highScoreUI;
    [SerializeField] private GameObject bestScoreUI;

    // Start is called before the first frame update
    void Start()
    {
        scoreProcessed = false;
        SetBestScoreText();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) && scoreProcessed)
            {
                LoadTitle(); // SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;

        m_rank = TitleHandler.Instance.CheckHighScore(m_Points);
        if (m_rank > -1)
        {
            scoreProcessed = false;
            // show high score inputs;

            if (m_rank == 0) // if first, show quote box.
            {
                bestScoreUI.SetActive(true);
            }
            else
            {
                highScoreUI.SetActive(true);
            }
        }
        else
        {
            scoreProcessed = true;
            GameOverText.SetActive(true);
        }
    }
    
    public void SaveHighScore( string name )
    {
        Debug.Log("'" + name + "'");
        m_Name = name;
        TitleHandler.Instance.AddHighScore(m_rank, m_Points, m_Name );
        LoadTitle();
    }

    public void SetHighScoreName ( string name)
    {
        m_Name = name;
    }

    public void SaveBestScore( string quote )
    {
        Debug.Log(name + " " + m_rank);

        TitleHandler.Instance.UpdateHighestScore(quote);
        TitleHandler.Instance.AddHighScore(m_rank, m_Points, m_Name);
        LoadTitle();
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void SetBestScoreText()
    {
        int score = TitleHandler.Instance.GetBestScore();
        string name = TitleHandler.Instance.GetBestName();

        BestScoreText.text = "Best Score: " + name + " - " + score;
    }
}
