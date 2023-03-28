using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSplitDirection
{
    NONE= 0,
    HORIZONTAL = 1,
    VERTICAL = 2,
}

public class BinarySpacePartitioner
{
    private RoomNode _rootNode;
    private int _maxIterations;

    private float minDivideRatio = 0.45f;
    private float maxDivideRatio = 0.65f;

    public BinarySpacePartitioner(RectInt size)
    {
        _rootNode = new RoomNode(size);
    }

    public RoomNode MakeRoomTree(int maxIterations)
    {
        _maxIterations = maxIterations;
        DivideSpace(_rootNode, 0);

        return _rootNode;
    }

    private void DivideSpace(RoomNode node, int iterations = 0)
    {
        if (iterations == _maxIterations)
            return;

        eSplitDirection splitDireciton = (node.Width > node.Height) ? eSplitDirection.VERTICAL : eSplitDirection.HORIZONTAL;
        node.SplittedDirection = splitDireciton;


        if (eSplitDirection.HORIZONTAL == splitDireciton)
        {
            //가로
            int dividedHeight = Mathf.RoundToInt(UnityEngine.Random.Range(node.Height * minDivideRatio, node.Height * maxDivideRatio));

            RectInt topRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y + dividedHeight,
                node.Width, node.Height - dividedHeight
            );

            RectInt bottomRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y,
                node.Width, dividedHeight
            );

            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftPosition.x, 0, node.BottomLeftPosition.y + dividedHeight));
            line.SetPosition(1, new Vector3(node.TopRightPosition.x, 0, node.BottomLeftPosition.y + dividedHeight));

            RoomNode leftNode = new RoomNode(topRoomSize);
            RoomNode rightNode = new RoomNode(bottomRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }
        else
        {
            //세로
            int dividedWidth = Mathf.RoundToInt(UnityEngine.Random.Range(node.Width * minDivideRatio, node.Width * maxDivideRatio));

            RectInt leftRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y,
                dividedWidth, node.Height
            );

            RectInt rightRoomSize = new RectInt(
                node.BottomLeftPosition.x + dividedWidth, node.BottomLeftPosition.y,
                node.Width - dividedWidth, node.Height
            );

            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftPosition.x + dividedWidth, 0, node.BottomLeftPosition.y));
            line.SetPosition(1, new Vector3(node.BottomLeftPosition.x + dividedWidth, 0, node.TopRightPosition.y));

            RoomNode leftNode = new RoomNode(leftRoomSize);
            RoomNode rightNode = new RoomNode(rightRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }

        DivideSpace(node.Left, iterations + 1);
        DivideSpace(node.Right, iterations + 1);
    }
}
