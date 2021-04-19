using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board gameBoard;

    public GameObject[] pipePrefabs;

    private const int DEFAULTWIDTH = 4;
    private const int DEFAULTHEIGHT = 4;


    // Start is called before the first frame update
    void Start()
    {
        //// Set Fixed Board
        //Vector2 dimension = CheckDimension();

        //gameBoard.width = (int)dimension.x;
        //gameBoard.height = (int)dimension.y;

        //gameBoard.pipes = new Pipe[gameBoard.width, gameBoard.height];

        //foreach(var item in GameObject.FindGameObjectsWithTag("Pipe"))
        //{
        //    gameBoard.pipes[(int)item.transform.position.x, (int)item.transform.position.y] = item.GetComponent<Pipe>();
        //}

        //foreach(var item in gameBoard.pipes)
        //{
        //    if(item != null)
        //    {
        //        Debug.Log(item.gameObject.name);
        //    }   
        //}


        // Ramdom Generation Board
        if(gameBoard.width == 0 || gameBoard.height == 0)
        {
            gameBoard.width = DEFAULTWIDTH;
            gameBoard.height = DEFAULTHEIGHT;
        }

        GenerateBoard();

        gameBoard.winConnectors = WinConnectorsCount();

        Shuffle();

        gameBoard.curConnectors = CurrentConnectorsCount();
    }

    // Update is called once per frame
    void Update()
    {
        
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
