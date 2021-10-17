using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public bool isMatched = false;
    public int targetX;
    public int targetY;
    public int previousColumn;
    public int previousRow;
    public int column;
    public int row;

   
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    void Start() 
    {
        board = FindObjectOfType<Board>();
       
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;

        //row = targetY;
        //column = targetX;
        //previousColumn = column;
        //previousRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();
        ColorChange();
        targetX = column;
        targetY = row;
        ChangeThePositionOfSprites();


    }
    private void OnMouseDown()
    {
        if(board.currentState==GameState.move) // record only first touch position whether the game in move state or not
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
       
        
    }
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y-firstTouchPosition.y)>swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI; // distance of two point in terms of degree(angle)
            MoveSprites();
            board.currentState = GameState.wait; // waits for the calculation of angle
        }
        else
        {
            board.currentState = GameState.move;
        }
            
    }
    void MoveSprites()
    {
        if(swipeAngle>-45 && swipeAngle<=45 && column<board.width-1)
        {
            //Swiping Right
            otherDot = board.allDots[column + 1, row];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        } else if(swipeAngle > -45 && swipeAngle <= 135 &&  row<board.height-1  )
        {
            //Swiping Up
            otherDot = board.allDots[column, row+1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column>0)
        {
            //Swiping Left
            otherDot = board.allDots[column-1, row ];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1; 
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row>0)
        {
            //Swiping Down
            otherDot = board.allDots[column , row-1];
            previousColumn = column;
            previousRow = row;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
       StartCoroutine(CheckMoveCo()); //Check to see if the sprites are matched
    }
    private void ChangeThePositionOfSprites()
    {
        if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            // Move towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if(board.allDots[column,row]!=this.gameObject)
            {
                board.allDots[column, row] = this.gameObject; // avoids any collision between sprites while they are moving
            }
           
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
             
        }


        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            // Move towards the target
            tempPosition = new Vector2(transform.position.x , targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject; // avoids any collision between sprites while they are moving 
            } 
           
        }
        else
        { 
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
             
        }
    }


    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot!=null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentState = GameState.move;
            } 
            else
            {
                board.DestroyMatches();
              
            }
            otherDot = null;
        }
    }


     


    void FindMatches()
    {
        if(column>0 && column<board.width-1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if(leftDot1!=null && rightDot1!=null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
            
        }


        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row+1];
            GameObject downDot1 = board.allDots[column , row-1];
            if(upDot1!=null && downDot1!=null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }

            }
          
        }


    }

    private void ColorChange()
    {
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
            
        }
    }
}
 