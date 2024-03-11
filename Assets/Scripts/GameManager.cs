using System;
using System.Collections;
using System.Collections.Generic;

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

    // Start is called before the first frame update
    void Start()
    {
        // levelMatrices = new List<bool[,]>();
        foreach (RawImage button in selectButtons)
        {
            button.gameObject.SetActive(false);
        }
        playButton.material.SetColor("_PrimaryColor", Util.GetColorFromRGB(levelInfo.mouseColors[0]));

        if (levelInfo.mouseColors.Length == 1) {
            Color c = Color.Lerp(Util.GetColorFromRGB(levelInfo.mouseColors[0]), Color.black, 0.1f);
            playButton.material.SetColor("_SecondaryColor", c); 
        } else { playButton.material.SetColor("_SecondaryColor", Util.GetColorFromRGB(levelInfo.mouseColors[1])); }

        for (int i = 0; i < levelInfo.labyrinths.Length; i++)
        {
            LabyrinthInfo labyrinthInfo = levelInfo.labyrinths[i];
            Color color = Util.GetColorFromRGB(labyrinthInfo.rgbColor);
            bool[,] matrix = Labyrinth.CreateLabyrinth(color, labyrinthInfo.labyrinthTextFile, labyrinthTilePrefab, levelInfo.levelSize, labyrinthParents[i]);
            // levelMatrices.Add(matrix);
            labyrinthInfo.LevelMatrix = matrix;

            selectButtons[i].color = color;
            selectButtons[i].gameObject.SetActive(true);
        }

        Vector3 mousePos = new Vector3(levelInfo.mouseStartPos.x, 0.5f, levelInfo.mouseStartPos.y);
        Instantiate(mousePrefab).transform.position = mousePos;
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
        IEnumerator coroutine = MoveLabyrinthOverTime(labyrinthIndex, direction);
        StartCoroutine(coroutine);
        levelInfo.labyrinths[labyrinthIndex].Offset += direction;
        

        IEnumerator MoveLabyrinthOverTime(int labyrinthIndex,  Vector2 direction) {
            Vector2 prevOffset = levelInfo.labyrinths[labyrinthIndex].Offset;
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
