using System;
using System.Collections;
using System.Collections.Generic;

// using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public enum RGB { 
    RED,
    GREEN,
    BLUE,
    YELLOW,
    CYAN,
    MAGENTA,
    WHITE
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject labyrinthTilePrefab;
    [SerializeField] AnimationCurve labyrinthMovementAnimation;
    [SerializeField] float labyrinthMovementTime = 0.4f;
    [SerializeField] LevelInfo levelInfo;

    [Header("LabyrinthParents")]
    [SerializeField] Transform[] labyrinthParents;
    [SerializeField] RawImage[] selectButtons;

    private  List<bool[,]> levelMatrices;
    private Vector2[] offsets = new Vector2[3];
    private bool labyrinthIsMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        levelMatrices = new List<bool[,]>();
        foreach (RawImage button in selectButtons)
        {
            button.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelInfo.labyrinths.Length; i++)
        {
            LabyrinthInfo labyrinthInfo = levelInfo.labyrinths[i];
            Color color = labyrinthInfo.color;
            bool[,] matrix = Labyrinth.CreateLabyrinth(color, labyrinthInfo.labyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, labyrinthParents[i]);
            levelMatrices.Add(matrix);

            selectButtons[i].color = color;
            selectButtons[i].gameObject.SetActive(true);
        }
        
    }
    
    public void MoveLabyrinth(int labyrinthIndex, Vector2 direction) {
        if (labyrinthIsMoving) { return; }
        Vector2 initialOffset = offsets[labyrinthIndex];
        Vector2 newOffset = initialOffset + direction;
        if (Mathf.Abs(newOffset.x) > levelInfo.maxOffset) { return; }
        if (Mathf.Abs(newOffset.y) > levelInfo.maxOffset) { return; }

        //update visuals
        IEnumerator coroutine = MoveLabyrinthOverTime(labyrinthIndex, direction);
        StartCoroutine(coroutine);
        offsets[labyrinthIndex] += direction;
        

        IEnumerator MoveLabyrinthOverTime(int labyrinthIndex,  Vector2 direction) {
            Vector2 prevOffset = offsets[labyrinthIndex];
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
                Vector2 offset = prevOffset + animRatio * direction;
                labyrinthParents[labyrinthIndex].position = new Vector3(offset.x, 0, offset.y);
            }
        }
    }

    public void CheckLabyrinthSolved() {
        // Debug.Log("Checking labyrinth");
        // BFSGrid bfsGrid = new BFSGrid(levelMatrix);
        // bfsGrid.FindShortestPath((levelInfo.mouseStartPos.x, levelInfo.mouseStartPos.y), (levelInfo.cheesePos.x, levelInfo.cheesePos.y));
    }
}
