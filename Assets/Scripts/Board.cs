using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    private FindMatches findMatches;
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
       allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        setUp();
    }

   private void setUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j +offSet);
             GameObject backGroundTile=Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backGroundTile.transform.parent = this.transform; 
                backGroundTile.name="( " +i + "," +j + " )";
                int dotToUse = Random.Range(0, dots.Length);
                // As long as matches are true that is sprites are come together as 3 or 4 , sprites will be located at different position on the tile
                // then when they locate at different position while loop return false 
                int maxIterations = 0;
                while (MatchesAt(i,j,dots[dotToUse]) && maxIterations<100) 
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;  



                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + "," + j + " )";
                allDots[i, j] = dot;
            } 
        }
    }

    private bool MatchesAt(int column, int row,GameObject sprite) // Checking the matches beginning of the tile screen
    {
        if(column>1 && row>1)
        {
            if(allDots[column-1,row].tag==sprite.tag && allDots[column-2,row].tag==sprite.tag)
            {
                return true;
            }
            if (allDots[column , row-1].tag == sprite.tag && allDots[column, row-2].tag == sprite.tag)
            {
                return true;
            }

        }else if( column<=1 ||  row<=1)
        {
            if(row>1)
            {
                if(allDots[column,row-1].tag==sprite.tag && allDots[column,row-2].tag==sprite.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column-1, row].tag == sprite.tag && allDots[column-2, row].tag == sprite.tag)
                {
                    return true;
                }
            } 
        }



        return false;
    }

    private void DestroyMatchesAt(int column,int row)
    {
        if(allDots[column,row].GetComponent<Dot>().isMatched)
        {
            findMatches.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }

    }
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j]!=null)
                {
                    DestroyMatchesAt(i, j);
                }

            }
        }
        StartCoroutine(DecreaseRowCo());
    }


    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j]==null)
                {
                    nullCount++;
                }else if(nullCount>0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j]==null)
                {
                    Vector2 tempPosition = new Vector2(i, j+offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                     GameObject sprite = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = sprite;
                    sprite.GetComponent<Dot>().row = j;
                    sprite.GetComponent<Dot>().column = i;

                }
            }

        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j]!=null)
                {
                    if(allDots[i,j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }


    private IEnumerator FillBoardCo()
    { 
        RefillBoard();   
        yield return new WaitForSeconds(.5f);
        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }

}
