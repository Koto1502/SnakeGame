using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControl : MonoBehaviour
{
    Vector2 SwipeStart;
    Vector2 SwipeEnd;
    float minimumDistance  = 0;

    public static event System.Action<SwipeDirection> OnSwipe = delegate { };
    public enum SwipeDirection
    {
        Up, Down, Left, Right
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                SwipeStart = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                SwipeEnd = touch.position;
                ProcessSwipe();
            }
        }

        //Mouse touch simulation
        if (Input.GetMouseButtonDown(0))
        {
            SwipeStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SwipeEnd = Input.mousePosition;
            ProcessSwipe();
        }
    }
    private void ProcessSwipe()
    {
        float distance = Vector2.Distance(SwipeStart, SwipeEnd); 
        if(distance > minimumDistance)
        {
            if (isVerticalSwipe()) 
            {
                if (SwipeEnd.y > SwipeStart.y)
                {
                    OnSwipe(SwipeDirection.Up);
                }
                else
                {
                    OnSwipe(SwipeDirection.Down);
                }
            }
            else
            {
                    if (SwipeEnd.x > SwipeStart.x)
                    {
                        OnSwipe(SwipeDirection.Right);
                    }
                    else
                    {
                        OnSwipe(SwipeDirection.Left);
                    }
            }
   
        }

    }

    bool isVerticalSwipe()
    {
        float vertical =Mathf.Abs(SwipeEnd.y - SwipeStart.y);
        float horizontal = Mathf.Abs(SwipeEnd.x - SwipeStart.x);
        if (vertical > horizontal)
            return true;
        return false;
    }
}
