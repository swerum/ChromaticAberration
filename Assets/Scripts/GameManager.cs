using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] AnimationCurve labyrinthMovementAnimation;
    [SerializeField] float labyrinthMovementTime = 0.4f;
    [SerializeField] LevelInfo levelInfo;

    [Header("References")]
    [SerializeField] private GameObject labyrinthTilePrefab;
    [SerializeField] private GameObject mousePrefab;
    [SerializeField] private GameObject cheesePrefab;

    [SerializeField] Transform[] labyrinthParents;
    [SerializeField] RawImage[] selectButtons;
    [SerializeField] RawImage playButton;

    // private Vector2[] offsets = new Vector2[3];
    private bool labyrinthIsMoving = false;
    private GameObject mouse;

    // Start is called before the first frame update
    void Start()
    {
        // levelMatrices = new List<bool[,]>();
        foreach (RawImage button in selectButtons)
        {
            button.gameObject.SetActive(false);
        }
        playButton.material.SetColor("_PrimaryColor", ColorUtil.GetColorFromRGB(levelInfo.mouseColors[0]));

        if (levelInfo.mouseColors.Length == 1) {
            Color c = Color.Lerp(ColorUtil.GetColorFromRGB(levelInfo.mouseColors[0]), Color.black, 0.1f);
            playButton.material.SetColor("_SecondaryColor", c); 
        } else { playButton.material.SetColor("_SecondaryColor", ColorUtil.GetColorFromRGB(levelInfo.mouseColors[1])); }

        for (int i = 0; i < levelInfo.labyrinths.Length; i++)
        {
            LabyrinthInfo labyrinthInfo = levelInfo.labyrinths[i];
            Debug.Log("Starting offset: "+labyrinthInfo.Offset);
            Color color = ColorUtil.GetColorFromRGB(labyrinthInfo.rgbColor);
            bool[,] matrix = Util.CreateLabyrinth(color, labyrinthInfo.labyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, labyrinthParents[i]);
            // levelMatrices.Add(matrix);
            labyrinthInfo.LevelMatrix = matrix;

            selectButtons[i].color = color;
            selectButtons[i].gameObject.SetActive(true);
        }

        Vector3 mousePos = new Vector3(levelInfo.mouseStartPos.x, 0.5f, levelInfo.mouseStartPos.y);
        mouse = Instantiate(mousePrefab);
        mouse.transform.position = mousePos;
        Vector3 cheesePos = new Vector3(levelInfo.cheesePos.x, 0.5f, levelInfo.cheesePos.y);
        Instantiate(cheesePrefab).transform.position = cheesePos;
    }
    
    public void MoveLabyrinth(int labyrinthIndex, Vector2 direction) {
        if (labyrinthIsMoving) { return; }
        Vector2 initialOffset = levelInfo.labyrinths[labyrinthIndex].Offset; //offsets[labyrinthIndex];
        Vector2 newOffset = initialOffset + direction;
        if (Mathf.Abs(newOffset.x) > levelInfo.maxOffset) { return; }
        if (Mathf.Abs(newOffset.y) > levelInfo.maxOffset) { return; }

        //update visuals
        LabyrinthInfo labyrinthInfo = levelInfo.labyrinths[labyrinthIndex];
        IEnumerator coroutine = MoveLabyrinth(labyrinthParents[labyrinthIndex], labyrinthInfo.Offset, direction, labyrinthMovementTime);
        StartCoroutine(coroutine);
        Debug.Log("adding "+direction+" to offset");
        levelInfo.labyrinths[labyrinthIndex].Offset += direction;
        
        IEnumerator MoveLabyrinth( Transform transform, Vector2 prevPosition, Vector2 offset, float movementTime) {
            // Vector2 prevOffset = levelInfo.labyrinths[labyrinthIndex].Offset;
            labyrinthIsMoving = true;
            IEnumerator coroutine = Util.MoveOverTime(transform,prevPosition, offset, movementTime);
            yield return StartCoroutine(coroutine);
            labyrinthIsMoving = false;
        }
    }
    [ExecuteInEditMode]
    public void CheckLabyrinthSolved() {
        Debug.Log("Checking labyrinth");
        DFSGrid dfs = new DFSGrid(levelInfo, mouse);
        bool reachedGoal = dfs.FindShortestPath();
        Debug.Log("Solved Puzzle: "+reachedGoal);
    }
}
