using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eRelativeRectDirection
{
    NONE = 0,
    LEFT = 1,
    RIGHT,
    DOWN,
    UP,
}

public static class StructureHelper
{
    /// <summary>
    /// target이 center기준으로 어디에 있는지 확인
    /// </summary>
    public static eRelativeRectDirection DistinguishRectPosition(this RectInt center, RectInt compareTarget)
    {
        if (center.xMin > compareTarget.xMax)
        {
            //왼쪽
            return eRelativeRectDirection.LEFT;
        }
        else if (center.xMax < compareTarget.xMin)
        {
            //오른쪽
            return eRelativeRectDirection.RIGHT;
        }
        else if (center.yMin > compareTarget.yMax)
        {
            //아래
            return eRelativeRectDirection.DOWN;
        }
        else if (center.yMax < compareTarget.yMin)
        {
            //위
            return eRelativeRectDirection.UP;
        }

        return eRelativeRectDirection.NONE;
    }

    public static float GetShortestDistance(this RectInt rect, RectInt target)
    {
        eRelativeRectDirection relative = rect.DistinguishRectPosition(target);
        switch (relative)
        {
            case eRelativeRectDirection.LEFT:
                return (rect.xMin - target.xMax);
            case eRelativeRectDirection.RIGHT:
                return (target.xMin - rect.xMax);
            case eRelativeRectDirection.DOWN:
                return (rect.yMin - target.yMax);
            case eRelativeRectDirection.UP:
                return (target.yMin - rect.yMax);
            default:
                {
                    Debug.LogError("GetShortestDistance:: direction Error");
                    return -1.0f;
                }
        }
    }

    public static bool CheckOverlapRange(this RectInt from, RectInt to, int doorLength)
    {
        //연결가능 유효성 체크. 너비, 높이 둘 중 하나만 참이면 됨
        int xMin = Mathf.Min(from.xMin, to.xMin);
        int xMax = Mathf.Max(from.xMax, to.xMax);
        int yMin = Mathf.Min(from.yMin, to.yMin);
        int yMax = Mathf.Max(from.yMax, to.yMax);

        bool isOverlapped = (xMax - xMin < from.width + to.width) || (yMax - yMin < from.height + to.height);

        //문 생성이 불가능하다면 겹치는 걸로 취급하지 않는다.
        int doorXMin = Mathf.Max(from.xMin, to.xMin);
        int doorXMax = Mathf.Min(from.xMax, to.xMax);
        int doorYMin = Mathf.Max(from.yMin, to.yMin);
        int doorYMax = Mathf.Min(from.yMax, to.yMax);

        bool canMakeDoor = false;

        eRelativeRectDirection relative = from.DistinguishRectPosition(to);
        if(eRelativeRectDirection.LEFT == relative || eRelativeRectDirection.RIGHT == relative)
        {
            canMakeDoor = Mathf.Abs(doorYMax - doorYMin) >= doorLength;
        }
        else if(eRelativeRectDirection.UP == relative || eRelativeRectDirection.DOWN == relative)
        {
            canMakeDoor = Mathf.Abs(doorXMax - doorXMin) >= doorLength;
        }

        return (isOverlapped && canMakeDoor);
    }
}
