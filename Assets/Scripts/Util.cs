using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static bool[,] CreateLabyrinth(Color tileColor , TextAsset textFile, GameObject labyrinthTilePrefab, Vector2Int levelSize, Transform labyrinthParent) {
        bool[,] levelMatrix = FillLevelMatrix(levelSize, textFile);
        for (int i = 0; i < levelMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < levelMatrix.GetLength(1); j++)
            {
                if (levelMatrix[i,j]) {
                    GameObject tile = GameObject.Instantiate(labyrinthTilePrefab,new Vector3(i, 0, j), Quaternion.identity, labyrinthParent);
                    tile.GetComponent<MeshRenderer>().material.SetColor("_Color", tileColor);
                    // InstantiateTile(levelMatrix, i, j);
                }
            }
        }
        return levelMatrix;
    } 

    private static bool[,] FillLevelMatrix(Vector2Int levelSize, TextAsset textFile) {
        string fileContents = textFile.text;
        bool[,] levelMatrix = new bool[levelSize.x, levelSize.y];
        int row = 0; int col = 0;
        foreach (char c in fileContents)
        {
            switch (c) {
                case 'X': 
                    levelMatrix[row, col] = false;
                    col++;
                    break;
                case 'O':
                    levelMatrix[row, col] = true;
                    col++;
                    break;
                case '\n':
                    row++;
                    col = 0;
                    break;
                default: 
                    Debug.LogError("Unexpected character in level file: '"+c+"'");
                    break;
            }
        }
        return levelMatrix;
    }

    public static IEnumerator MoveOverTime( Transform transform, Vector2 prevPosition, Vector2 offset, float movementTime) {
        // Vector2 prevOffset = levelInfo.labyrinths[labyrinthIndex].Offset;
        float time = 0f;
        while (time < movementTime) {
            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
            float ratio = time / movementTime;
            float animRatio = AnimationCurve.EaseInOut(0,0,1,1).Evaluate(ratio);
            SetCurrentOffset(animRatio);
        }
        SetCurrentOffset(1f);

        void SetCurrentOffset(float animRatio) {
            Vector2 currentOffset = prevPosition + animRatio * offset;
            transform.position = new Vector3(currentOffset.x, 0, currentOffset.y);
        }
    }

    public static IEnumerator RotateOverTime( Transform transform, float endRotation, float duration) 
    {
        float startRotation = transform.eulerAngles.y;
        float t = 0.0f;
        while ( t  < duration )
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
    }
    public static IEnumerator ShrinkAway( Transform transform, float duration) 
    {
        float startScale = transform.localScale.x;
        float targetScale = 0;
        float t = 0.0f;
        while ( t  < duration )
        {
            t += Time.deltaTime;
            float ratio = AnimationCurve.EaseInOut(0,0,1,1).Evaluate(t/duration);
            float scale = Mathf.Lerp(startScale, targetScale, ratio);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        transform.gameObject.SetActive(false);
    }

    // private static void InstantiateTile(bool[,] levelMatrix, int row, int col, GameObject labyrinthTilePrefab) {
    //     // bool hasLeft    = hasNeighborAt(row+0, col-1);
    //     // bool hasRight   = hasNeighborAt(row+0, col+1);
    //     // bool hasTop     = hasNeighborAt(row+1, col+0);
    //     // bool hasBottom  = hasNeighborAt(row-1, col+0);
    //     // //get scale
    //     // float zScale = 1f + (BoolToInt(hasLeft) + BoolToInt(hasRight) - 2) * tileDistance;
    //     // float xScale = 1f + (BoolToInt(hasTop) + BoolToInt(hasBottom) - 2) * tileDistance;
    //     // Debug.Log("Left: "+hasLeft+", Right: "+hasRight+", at: ["+row+","+col+"], width: "+xScale);
    //     // Vector3 scale = new Vector3(xScale, 0.1f, zScale);
    //     // //get pos
    //     // float xPos = row + (BoolToInt(hasRight) - BoolToInt(hasRight)) * tileDistance;
    //     // float zPos = col + (BoolToInt(hasBottom) - BoolToInt(hasLeft)) * tileDistance;
    //     // Vector3 pos = new Vector3(row, 0, col); //new Vector3(xPos, 0, zPos);

    //     GameObject obj = Instantiate(labyrinthTilePrefab, new Vector3(row, 0, col), Quaternion.identity, gameObject.transform);
    //     // obj.transform.localScale = scale;
    //     // obj.transform.position = pos;

    //     // int BoolToInt(bool b) { return b ? 1 : 0; }
    //     // bool hasNeighborAt(int i, int j) {
    //     //     if (i >= levelMatrix.GetLength(0) || i < 0) return false;
    //     //     if (j >= levelMatrix.GetLength(1) || j < 0) return false;
    //     //     return levelMatrix[i,j];
    //     // }
    // }


}
