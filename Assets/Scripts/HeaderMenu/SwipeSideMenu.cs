using UnityEngine;
using System.Collections;

public class SwipeSideMenu : MonoBehaviour {

    //void OnSwipeMenu(SwipeGesture gesture)
    //{
    //    GameObject selection = gesture.StartSelection;
    //        switch (gesture.Direction) {
    //            case FingerGestures.SwipeDirection.UpperRightDiagonal:
    //            case FingerGestures.SwipeDirection.Right:
    //            case FingerGestures.SwipeDirection.LowerRightDiagonal:
    //                    Vector3 screen = Utility.GetWidthHeightScreenFollowNGUI(HeaderMenu.Instance.btnBack.gameObject);
    //                    if (GameManager.CurrentScene == ESceneName.Gameplay)
    //                    {
    //                        if (HeaderMenu.Instance.IsHidden && HeaderMenu.Instance.IsHiddenListWaiting)
    //                        {
    //                            if (gesture.StartPosition.x > 50f)
    //                                return;
    //                            if (gesture.StartPosition.x <= 50f && gesture.Move.x > 50f)
    //                                if (HeaderMenu.Instance.IsHidden)
    //                                    HeaderMenu.Instance.IsHidden = false;
    //                        }
    //                        else if (HeaderMenu.Instance.IsHidden && !HeaderMenu.Instance.IsHiddenListWaiting)
    //                        {
                             
    //                            if (gesture.StartPosition.x * screen.z >= screen.x - 200f) return;
    //                            HeaderMenu.Instance.IsHiddenListWaiting = true;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (gesture.StartPosition.x > 50f)
    //                            return;
    //                        if (gesture.StartPosition.x <= 50f && gesture.Move.x > 50f)
    //                            if (HeaderMenu.Instance.IsHidden)
    //                                HeaderMenu.Instance.IsHidden = false;
    //                    }
    //                    break;
    //            case FingerGestures.SwipeDirection.LowerLeftDiagonal:
    //            case FingerGestures.SwipeDirection.Left:
    //            case FingerGestures.SwipeDirection.UpperLeftDiagonal:
    //                    if (GameManager.CurrentScene == ESceneName.Gameplay) {
    //                        if (!HeaderMenu.Instance.IsHidden && HeaderMenu.Instance.IsHiddenListWaiting)
    //                            HeaderMenu.Instance.IsHidden = true;
    //                        else if (HeaderMenu.Instance.IsHidden && HeaderMenu.Instance.IsHiddenListWaiting)
    //                        {
    //                            if (gesture.StartPosition.x < Screen.width - 50f)
    //                                return;
    //                            if (gesture.StartPosition.x > Screen.width - 50f && gesture.Move.x < -50f)
    //                            {
    //                                if (HeaderMenu.Instance.IsHiddenListWaiting)
    //                                    HeaderMenu.Instance.IsHiddenListWaiting = false;
    //                            }
    //                        }
    //                    }
    //                    else 
    //                    {
    //                            if (!HeaderMenu.Instance.IsHidden)
    //                                HeaderMenu.Instance.IsHidden = true;
    //                    }
    //                    break;
    //        }
       
    //}

    #region Utils

    // Convert from screen-space coordinates to world-space coordinates on the Z = 0 plane
    public static Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // we solve for intersection with z = 0 plane
        float t = -ray.origin.z / ray.direction.z;

        return ray.GetPoint(t);
    }
    
    #endregion
}
