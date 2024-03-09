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
    [SerializeField] private PostEffectsController postEffectsController;
    [SerializeField] AnimationCurve labyrinthMovementAnimation;
    [SerializeField] float labyrinthMovementTime = 0.4f;
    [SerializeField] LevelInfo levelInfo;

    [Header("Materials")]
    [SerializeField] Material redTile;
    [SerializeField] Material blueTile;
    [SerializeField] Material greenTile;

    [Header("LabyrinthParents")]
    [SerializeField] Transform redParent;
    [SerializeField] Transform greenParent;
    [SerializeField] Transform blueParent;

    private bool[,] levelMatrix;
    private Vector2 redOffset = new(0,0);
    private Vector2 blueOffset = new(0,0);
    private bool labyrinthIsMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        Labyrinth.CreateLabyrinth(redTile, levelInfo.redLabyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, redParent);
        Labyrinth.CreateLabyrinth(greenTile, levelInfo.greenLabyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, greenParent);
        Labyrinth.CreateLabyrinth(blueTile, levelInfo.blueLabyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, blueParent);
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
            if (Mathf.Abs(newOffset.x) > levelInfo.maxOffset) return Vector2.zero;
            if (Mathf.Abs(newOffset.y) > levelInfo.maxOffset) return Vector2.zero;
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
                SetCurrentOffset(animRatio);
            }
            SetCurrentOffset(1f);
            labyrinthIsMoving = false;

            void SetCurrentOffset(float animRatio) {
                Vector2 red = prevRedOffset + animRatio * redDirection;
                Vector2 blue = prevBlueOffset + animRatio * blueDirection;
                redParent.position = new Vector3(red.x, 0, red.y);
                blueParent.position =  new Vector3(blue.x, 0, blue.y);
            }
        }
    }

    public void CheckLabyrinthSolved() {
        // Debug.Log("Checking labyrinth");
        // BFSGrid bfsGrid = new BFSGrid(levelMatrix);
        // bfsGrid.FindShortestPath((levelInfo.mouseStartPos.x, levelInfo.mouseStartPos.y), (levelInfo.cheesePos.x, levelInfo.cheesePos.y));
    }
}
