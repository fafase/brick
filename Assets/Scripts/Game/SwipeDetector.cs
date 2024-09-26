using UnityEngine;
using UniRx;
using System;

public class SwipeDetector : MonoBehaviour
{
    // Minimum distance for the swipe to be recognized
    public float minSwipeDistance = 50f;

    // Observable for swipe events, emitting the swipe direction as a Vector2
    public IObservable<Vector2> SwipeAsObservable => swipeSubject;
    private Subject<Vector2> swipeSubject = new Subject<Vector2>();

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;

    // Threshold angle to detect vertical swipes (45 degrees from vertical)
    private float verticalThresholdCos; 

    private void Start()
    {
        verticalThresholdCos = Mathf.Cos(45f * Mathf.Deg2Rad);
        // Subscribe to updates based on platform
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                DetectMouseSwipe();
#else
                DetectTouchSwipe();
#endif
            })
            .AddTo(this);
    }

    private void DetectTouchSwipe()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                // Capture the start position of the touch
                startTouchPosition = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isSwiping)
                {
                    endTouchPosition = touch.position;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isSwiping)
                {
                    // Calculate swipe delta
                    Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                    if (swipeDelta.magnitude >= minSwipeDistance)
                    {
                        // Emit normalized swipe direction only if within vertical range
                        if (IsVerticalSwipe(swipeDelta.normalized))
                        {
                            swipeSubject.OnNext(swipeDelta.normalized);
                        }
                    }
                    ResetSwipe();
                }
                break;
        }
    }

    private void DetectMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Capture the start position of the mouse click
            startTouchPosition = Input.mousePosition;
            isSwiping = true;
        }

        if (isSwiping && Input.GetMouseButton(0))
        {
            // Update the end position while the mouse is being held down
            endTouchPosition = Input.mousePosition;
        }

        if (isSwiping && Input.GetMouseButtonUp(0))
        {
            // Calculate swipe delta
            Vector2 swipeDelta = endTouchPosition - startTouchPosition;

            if (swipeDelta.magnitude >= minSwipeDistance)
            {
                // Emit normalized swipe direction only if within vertical range
                if (IsVerticalSwipe(swipeDelta.normalized))
                {
                    swipeSubject.OnNext(swipeDelta.normalized);
                }
            }
            ResetSwipe();
        }
    }

    // Method to check if the swipe direction is within 90 degrees of the vertical axis
    private bool IsVerticalSwipe(Vector2 swipeDirection)
    {
        // Vertical axis is (0, 1) for up or (0, -1) for down
        Vector2 upDirection = Vector2.up;

        // Calculate dot product between swipe direction and vertical (up) direction
        float dotProduct = Vector2.Dot(swipeDirection, upDirection);

        // Compare the dot product against the cos(45 degrees), only emit if it's vertical
        return Mathf.Abs(dotProduct) >= verticalThresholdCos;
    }

    private void ResetSwipe()
    {
        isSwiping = false;
        startTouchPosition = Vector2.zero;
        endTouchPosition = Vector2.zero;
    }
}
