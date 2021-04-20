using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Pipe Elements")]
    public GameObject[] pipePrefabs;

    private const int DEFAULTWIDTH = 4;
    private const int DEFAULTHEIGHT = 4;

    [Header("Game Attributes")]
    public Board gameBoard;
    public Difficulty gameDifficulty;
    public PlayerSkill playerSkill;

    [Header("Timer")]
    public float timeRemaining;
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI secondText;

    [Header("Text References")]
    public TextMeshProUGUI curConnectorsText;
    public TextMeshProUGUI winConnectorsText;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject gameOverText;

    private bool win;
    private bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        // Set up Input Value and Generate Board Randomly

        win = false;
        gameOver = false;

        gameDifficulty = InputValue.gameDifficulty;

        playerSkill = InputValue.playerSkill;

        SettingDifficulty();

        CheckingPlayerSkill();

        if (gameBoard.width == 0 || gameBoard.height == 0)
        {
            gameBoard.width = DEFAULTWIDTH;
            gameBoard.height = DEFAULTHEIGHT;

            timeRemaining = 30.0f;
        }

        CameraAdjustment(gameBoard.width / 2, gameBoard.height / 2 - 1);

        GenerateBoard();

        gameBoard.winConnectors = WinConnectorsCount();

        winConnectorsText.text = gameBoard.winConnectors.ToString();

        Shuffle();

        gameBoard.curConnectors = CurrentConnectorsCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver == false)
        {
            TimeCounter();

            curConnectorsText.text = gameBoard.curConnectors.ToString();

            if (gameBoard.curConnectors == gameBoard.winConnectors)
            {
                gameOver = true;
                win = true;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (win)
            {
                winText.gameObject.SetActive(true);
                StartCoroutine(ShowMessage(2.0f));
            }
            else
            {
                gameOverText.gameObject.SetActive(true);
                StartCoroutine(ShowMessage(2.0f));
            }
        }
    }


    // Setting Difficulty up to Input value
    private void SettingDifficulty()
    {
        if (gameDifficulty == Difficulty.EASY)
        {
            gameBoard.width = 4;
            gameBoard.height = 4;

            timeRemaining = 30.0f;
        }
        else if (gameDifficulty == Difficulty.MEDIUM)
        {
            gameBoard.width = 6;
            gameBoard.height = 6;

            timeRemaining = 90.0f;
        }
        else if (gameDifficulty == Difficulty.HARD)
        {
            gameBoard.width = 8;
            gameBoard.height = 8;

            timeRemaining = 180.0f;
        }
    }


    // Checking Hacking Skill to add extra time
    private void CheckingPlayerSkill()
    {
        if(playerSkill == PlayerSkill.LEVEL1)
        {
            timeRemaining += 5.0f;
        }
        else if (playerSkill == PlayerSkill.LEVEL2)
        {
            timeRemaining += 10.0f;
        }
        else if (playerSkill == PlayerSkill.LEVEL3)
        {
            timeRemaining += 20.0f;
        }
    }


    // Changing Camera Position
    private void CameraAdjustment(float x, float y)
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + x, Camera.main.transform.position.y + y, -10.0f);
    }


    // Show Message after finishing Game and change to Start scene
    IEnumerator ShowMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cursor.lockState = CursorLockMode.Confined;
        SceneManager.LoadScene("StartScene");
    }


    // Count down the time to set the game over state
    private void TimeCounter()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int minute = (int)(timeRemaining) / 60;
            int second = (int)timeRemaining - (60 * minute);

            minuteText.text = (minute > 9) ? minute.ToString() : "0" + minute.ToString();
            secondText.text = (second > 9) ? second.ToString() : "0" + second.ToString();
        }
        else
        {
            gameOver = true;
        }
    }


    void GenerateBoard()
    {
        gameBoard.pipes = new Pipe[gameBoard.width, gameBoard.height];

        int[] tempConnectorValues = { 0, 0, 0, 0 }; // Up, Right, Down, Left

        for (int h = 0; h < gameBoard.height; h++)
        {
            for(int w = 0; w < gameBoard.width; w++)
            {
                // Condition for LEFT and RIGHT connectors
                if (w == 0)
                    tempConnectorValues[(int)Position.LEFT] = 0;
                else
                    tempConnectorValues[(int)Position.LEFT] = gameBoard.pipes[w - 1, h].connectorValues[(int)Position.RIGHT];

                if (w == gameBoard.width - 1)
                    tempConnectorValues[(int)Position.RIGHT] = 0;
                else
                    tempConnectorValues[(int)Position.RIGHT] = Random.Range(0, 2);


                // Condition for UP and DOWN connectors
                if (h == 0)
                    tempConnectorValues[(int)Position.DOWN] = 0;
                else
                    tempConnectorValues[(int)Position.DOWN] = gameBoard.pipes[w, h - 1].connectorValues[(int)Position.UP];

                if (h == gameBoard.height - 1)
                    tempConnectorValues[(int)Position.UP] = 0;
                else
                    tempConnectorValues[(int)Position.UP] = Random.Range(0, 2);


                // Calculate number of connector of Pipe to Instantiate the right Prefab
                int pipeType = tempConnectorValues[(int)Position.UP] + tempConnectorValues[(int)Position.RIGHT] + tempConnectorValues[(int)Position.DOWN] + tempConnectorValues[(int)Position.LEFT];

                if(pipeType == (int)PipeType.LINE && tempConnectorValues[(int)Position.UP] != tempConnectorValues[(int)Position.DOWN])
                {
                    pipeType = (int)PipeType.CORNER;
                }

                GameObject gO = (GameObject) Instantiate(pipePrefabs[pipeType], new Vector3(w, h, 0), Quaternion.identity);

                while(gO.GetComponent<Pipe>().connectorValues[(int)Position.UP] != tempConnectorValues[(int)Position.UP]
                    || gO.GetComponent<Pipe>().connectorValues[(int)Position.RIGHT] != tempConnectorValues[(int)Position.RIGHT]
                    || gO.GetComponent<Pipe>().connectorValues[(int)Position.DOWN] != tempConnectorValues[(int)Position.DOWN]
                    || gO.GetComponent<Pipe>().connectorValues[(int)Position.LEFT] != tempConnectorValues[(int)Position.LEFT])
                {
                    gO.GetComponent<Pipe>().RotatePipe();
                }

                gameBoard.pipes[w, h] = gO.GetComponent<Pipe>();
            }
        }
    }


    // Checking width and height's values of FIxed Board
    Vector2 CheckDimension()
    {
        Vector2 dimension = Vector2.zero;

        GameObject[] pipes = GameObject.FindGameObjectsWithTag("Pipe");

        foreach(var pipe in pipes)
        {
            if(pipe.transform.position.x > dimension.x)
            {
                dimension.x = pipe.transform.position.x;
            }

            if (pipe.transform.position.y > dimension.y)
            {
                dimension.y = pipe.transform.position.y;
            }
        }

        dimension.x++;
        dimension.y++;

        return dimension;
    }


    // Shuffle every pipe in the board by rotating randomly
    void Shuffle()
    {
        foreach(var pipe in gameBoard.pipes)
        {
            if(pipe != null)
            {
                int randVal = Random.Range(0, 4);

                for (int i = 0; i < randVal; i++)
                {
                    pipe.RotatePipe();
                }
            }
        }
    }


    public int CurrentConnectorsCount()
    {
        int currentCount = 0;

        for(int h = 0; h < gameBoard.height; h++)
        {
            for(int w = 0; w < gameBoard.width; w++)
            {
                if(gameBoard.pipes[w, h] != null)
                {
                    if(h < gameBoard.height - 1)
                    {
                        if (gameBoard.pipes[w, h + 1] != null)
                        {
                            if (gameBoard.pipes[w, h].connectorValues[(int)Position.UP] == 1 && gameBoard.pipes[w, h + 1].connectorValues[(int)Position.DOWN] == 1)
                            {
                                currentCount++;
                            }
                        }
                    }

                    if(w < gameBoard.width - 1)
                    {
                        if (gameBoard.pipes[w + 1, h] != null)
                        {
                            if (gameBoard.pipes[w, h].connectorValues[(int)Position.RIGHT] == 1 && gameBoard.pipes[w + 1, h].connectorValues[(int)Position.LEFT] == 1)
                            {
                                currentCount++;
                            }
                        }
                    }       
                }
                
            }
        }

        return currentCount;
    }


    public int CalculateConnectors(int w, int h)
    {
        int pipeConnectors = 0;

        if (gameBoard.pipes[w, h] != null)
        {
            if (h < gameBoard.height - 1)
            {
                if (gameBoard.pipes[w, h + 1] != null)
                {
                    if (gameBoard.pipes[w, h].connectorValues[(int)Position.UP] == 1 && gameBoard.pipes[w, h + 1].connectorValues[(int)Position.DOWN] == 1)
                    {
                        pipeConnectors++;
                    }
                }
            }

            if (w < gameBoard.width - 1)
            {
                if (gameBoard.pipes[w + 1, h] != null)
                {
                    if (gameBoard.pipes[w, h].connectorValues[(int)Position.RIGHT] == 1 && gameBoard.pipes[w + 1, h].connectorValues[(int)Position.LEFT] == 1)
                    {
                        pipeConnectors++;
                    }
                }
            }

            if(w > 0)
            {
                if(gameBoard.pipes[w - 1, h] != null)
                {
                    if(gameBoard.pipes[w, h].connectorValues[(int)Position.LEFT] == 1 && gameBoard.pipes[w - 1, h].connectorValues[(int)Position.RIGHT] == 1)
                    {
                        pipeConnectors++;
                    }
                }
            }


            if(h > 0)
            {
                if (gameBoard.pipes[w, h - 1] != null)
                {
                    if (gameBoard.pipes[w, h].connectorValues[(int)Position.DOWN] == 1 && gameBoard.pipes[w, h - 1].connectorValues[(int)Position.UP] == 1)
                    {
                        pipeConnectors++;
                    }
                }
            }

        }

        return pipeConnectors;
    }


    // Count number of connectors to win
    int WinConnectorsCount()
    {
        int winConnectors = 0;

        foreach(var pipe in gameBoard.pipes)
        {
            if(pipe != null)
            {
                foreach (var j in pipe.connectorValues)
                {
                    winConnectors += j;
                }
            }
        }

        winConnectors /= 2;

        return winConnectors;
    }
}


// Game Board Structure
[System.Serializable]
public struct Board
{
    public int winConnectors;
    public int curConnectors;

    public int width;
    public int height;
    public Pipe[,] pipes;
}
