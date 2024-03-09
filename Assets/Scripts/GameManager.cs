using System.Collections;
using UnityEngine;

public enum RGB { 
    RED,
    GREEN,
    BLUE
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject labyrinthTilePrefab;
    [SerializeField] private int maxOffset = 5;
    [SerializeField] private PostEffectsController postEffectsController;
    [SerializeField] AnimationCurve labyrinthMovementAnimation;
    [SerializeField] float labyrinthMovementTime = 0.4f;
    // [SerializeField] private float tileDistance = 0.05f;
    [Header("Level Info")]
    [SerializeField] private Vector2Int levelSize;
    [SerializeField] private TextAsset textFile;
    [SerializeField] private Vector2Int mouseStartPos;
    [SerializeField] private Vector2Int cheesePos;

    private bool[,] levelMatrix;
    private Vector2 redOffset = new(0,0);
    private Vector2 blueOffset = new(0,0);
    private bool labyrinthIsMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        levelMatrix = FillLevelMatrix();
        for (int i = 0; i < levelMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < levelMatrix.GetLength(1); j++)
            {
                if (levelMatrix[i,j]) {
                    InstantiateTile(levelMatrix, i, j);
                }
            }
        }
    }
    
    public void MoveLabyrinth(RGB colorChannel, Vector2 direction) {
        if (labyrinthIsMoving) { return; }
        Vector2 redDirection = Vector2.zero, blueDirection = Vector2.zero;
        //update offsets
        switch(colorChannel) {
            case RGB.RED:
                redDirection = VerifyDirection(redOffset, direction);
                break;
            case RGB.BLUE:
                blueDirection = VerifyDirection(blueOffset, direction);
                break;
            case RGB.GREEN:
                redDirection = VerifyDirection(redOffset,  -direction);
                blueDirection = VerifyDirection(blueOffset, -direction);
                break;
            default: Debug.LogError("There are only three colors"); break; 
        }
        //update visuals
        IEnumerator coroutine = MoveLabyrinthOverTime(redOffset, redDirection, blueOffset, blueDirection);
        StartCoroutine(coroutine);
        redOffset += redDirection;
        blueOffset += blueDirection;

        Vector2 VerifyDirection(Vector2 initialOffset, Vector2 attemptedChange) {
            Vector2 newOffset = initialOffset + attemptedChange;
            if (Mathf.Abs(newOffset.x) > maxOffset) return Vector2.zero;
            if (Mathf.Abs(newOffset.y) > maxOffset) return Vector2.zero;
            return attemptedChange;
        }

        IEnumerator MoveLabyrinthOverTime(Vector2 prevRedOffset, Vector2 redDirection, Vector2 prevBlueOffset, Vector2 blueDirection) {
            float time = 0f;
            labyrinthIsMoving = true;
            while (time < labyrinthMovementTime) {
                yield return new WaitForFixedUpdate();
                time += Time.deltaTime;
                float ratio = time / labyrinthMovementTime;
                float animRatio = labyrinthMovementAnimation.Evaluate(ratio);
                Vector2 redShaderOffset = prevRedOffset + animRatio * redDirection;
                Vector2 blueShaderOffset = prevBlueOffset + animRatio * blueDirection;
                postEffectsController.SetAberrationOffsets(redShaderOffset, blueShaderOffset);
            }
            Vector2 finalRedShaderOffset = prevRedOffset +  redDirection;
            Vector2 finalBlueShaderOffset = prevBlueOffset + blueDirection;
            postEffectsController.SetAberrationOffsets(finalRedShaderOffset, finalBlueShaderOffset);
            labyrinthIsMoving = false;
        }
    }

    private bool[,] FillLevelMatrix() {
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

    private void InstantiateTile(bool[,] levelMatrix, int row, int col) {
        // bool hasLeft    = hasNeighborAt(row+0, col-1);
        // bool hasRight   = hasNeighborAt(row+0, col+1);
        // bool hasTop     = hasNeighborAt(row+1, col+0);
        // bool hasBottom  = hasNeighborAt(row-1, col+0);
        // //get scale
        // float zScale = 1f + (BoolToInt(hasLeft) + BoolToInt(hasRight) - 2) * tileDistance;
        // float xScale = 1f + (BoolToInt(hasTop) + BoolToInt(hasBottom) - 2) * tileDistance;
        // Debug.Log("Left: "+hasLeft+", Right: "+hasRight+", at: ["+row+","+col+"], width: "+xScale);
        // Vector3 scale = new Vector3(xScale, 0.1f, zScale);
        // //get pos
        // float xPos = row + (BoolToInt(hasRight) - BoolToInt(hasRight)) * tileDistance;
        // float zPos = col + (BoolToInt(hasBottom) - BoolToInt(hasLeft)) * tileDistance;
        // Vector3 pos = new Vector3(row, 0, col); //new Vector3(xPos, 0, zPos);

        GameObject obj = Instantiate(labyrinthTilePrefab, new Vector3(row, 0, col), Quaternion.identity, gameObject.transform);
        // obj.transform.localScale = scale;
        // obj.transform.position = pos;

        // int BoolToInt(bool b) { return b ? 1 : 0; }
        // bool hasNeighborAt(int i, int j) {
        //     if (i >= levelMatrix.GetLength(0) || i < 0) return false;
        //     if (j >= levelMatrix.GetLength(1) || j < 0) return false;
        //     return levelMatrix[i,j];
        // }
    }


    public void CheckLabyrinthSolved() {
        Debug.Log("Checking labyrinth");
        BFSGrid bfsGrid = new BFSGrid(levelMatrix);
        bfsGrid.FindShortestPath((mouseStartPos.x, mouseStartPos.y), (cheesePos.x, cheesePos.y));
    }
}
