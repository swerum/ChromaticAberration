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
    [SerializeField] float mouseMovementTime = 0.4f;
    [SerializeField] AudioClip moveSound;
    [SerializeField] AudioClip failSound;

    [Header("References")]
    [SerializeField] LevelInfo[] levels;
    [SerializeField] Transform[] labyrinthParents;
    [SerializeField] Camera labyrinthCamera;
    [SerializeField] AudioSource audioSource;
    [Header("Prefabs")]
    [SerializeField] private GameObject labyrinthTilePrefab;
    [SerializeField] private GameObject mousePrefab;
    [SerializeField] private GameObject cheesePrefab;


    [Header("Images")]
    [SerializeField] RawImage[] selectButtons;
    [SerializeField] RawImage playButton;
    [SerializeField] RawImage labyrinthDisplayImage;
    [SerializeField] RawImage hintImage;
    [SerializeField] RawImage gameCompleteImage;

    // private Vector2[] offsets = new Vector2[3];
    private bool disallowMovement = false;
    private GameObject mouse;
    private GameObject cheese;
    private Vector3 baseCameraPosition;
    // private LevelInfo currentLevel;
    private int currentLevelIndex;

    // Start is called before the first frame update
    private void Start() {
        // audioSource.PlayOneShot(ambientSound,1);
        baseCameraPosition = labyrinthCamera.transform.position;
        InitializeLevel(0);
    }

    private void InitializeLevel(int index)
    {
        LevelInfo level = levels[index];
        currentLevelIndex = index;
        //set camera settings
        float cameraOffset = Mathf.Max(level.LevelSize.x, level.LevelSize.y) / 2f;
        labyrinthCamera.transform.position = baseCameraPosition + new Vector3(cameraOffset, 0, cameraOffset);
        labyrinthCamera.orthographicSize = level.cameraSize;

        StartCoroutine(ShowHint(level.hint));
        foreach (RawImage button in selectButtons)
        {
            button.gameObject.SetActive(false);
        }
        playButton.material.SetColor("_PrimaryColor", ColorUtil.GetColorFromRGB(level.mouseColors[0]));

        if (level.mouseColors.Length == 1) {
            Color c = Color.Lerp(ColorUtil.GetColorFromRGB(level.mouseColors[0]), Color.black, 0.1f);
            playButton.material.SetColor("_SecondaryColor", c); 
        } else { playButton.material.SetColor("_SecondaryColor", ColorUtil.GetColorFromRGB(level.mouseColors[1])); }

        for (int i = 0; i < level.labyrinths.Length; i++)
        {
            LabyrinthInfo labyrinthInfo = level.labyrinths[i];
            Color color = ColorUtil.GetColorFromRGB(labyrinthInfo.rgbColor);
            bool[,] matrix = Util.CreateLabyrinth(color, labyrinthInfo.labyrinthTextFile, labyrinthTilePrefab, level.LevelSize, labyrinthParents[i]);
            labyrinthInfo.LevelMatrix = matrix;

            // set UI button color
            color.a = selectButtons[i].color.a;
            selectButtons[i].color = color;
            selectButtons[i].gameObject.SetActive(true);

            //set starting offset
            labyrinthInfo.Offset = labyrinthInfo.InitialOffset;//Vector2.zero;
            labyrinthParents[i].position = new Vector3(labyrinthInfo.Offset.x, 0, labyrinthInfo.Offset.y);
        }

        Vector3 mousePos = new Vector3(level.MouseStartPos.x, 0.5f, level.MouseStartPos.y);
        mouse = Instantiate(mousePrefab);
        mouse.transform.position = mousePos;
        Vector3 cheesePos = new Vector3(level.CheesePos.x, 0.5f, level.CheesePos.y);
        cheese = Instantiate(cheesePrefab);
        cheese.transform.position = cheesePos;
    }
    
    public void MoveLabyrinth(int labyrinthIndex, Vector2 direction) {
        if (disallowMovement) { return; }
        LevelInfo currentLevel = levels[currentLevelIndex];
        Vector2 initialOffset = currentLevel.labyrinths[labyrinthIndex].Offset; 
        Vector2 newOffset = initialOffset + direction;
        if (Mathf.Abs(newOffset.x) > currentLevel.maxOffset || Mathf.Abs(newOffset.y) > currentLevel.maxOffset) { 
            audioSource.PlayOneShot(failSound, 0.08f);
            return; 
        }

        audioSource.PlayOneShot(moveSound,1);
        //update visuals
        LabyrinthInfo labyrinthInfo = currentLevel.labyrinths[labyrinthIndex];
        IEnumerator coroutine = MoveLabyrinth(labyrinthParents[labyrinthIndex], labyrinthInfo.Offset, direction, labyrinthMovementTime);
        StartCoroutine(coroutine);
        currentLevel.labyrinths[labyrinthIndex].Offset += direction;
        
        IEnumerator MoveLabyrinth( Transform transform, Vector2 prevPosition, Vector2 offset, float movementTime) {
            // Vector2 prevOffset = levelInfo.labyrinths[labyrinthIndex].Offset;
            disallowMovement = true;
            IEnumerator coroutine = Util.MoveOverTime(transform,prevPosition, offset, movementTime);
            yield return StartCoroutine(coroutine);
            disallowMovement = false;
        }
    }
    
    public void CheckLabyrinthSolved() {
        if (disallowMovement) return;
        LevelInfo currentLevel = levels[currentLevelIndex];
        DFSGrid dfs = new DFSGrid(currentLevel);
        IEnumerator coroutine = MouseTraverseLabyrinth(dfs);
        StartCoroutine(coroutine);
        

        IEnumerator MouseTraverseLabyrinth(DFSGrid dfs) {
            List<Vector2Int> path = dfs.Path;
            disallowMovement = true;
            //mouse traverse labyrinth
            Vector2 currentPos = currentLevel.MouseStartPos;
            foreach (Vector2Int node in path) {
                Vector2 offset =  node - currentPos;
                IEnumerator rotatorCoroutine = Util.RotateOverTime(mouse.transform, GetTargetRotation(offset), mouseMovementTime);
                yield return StartCoroutine(rotatorCoroutine);
                IEnumerator coroutine = Util.MoveOverTime(mouse.transform,currentPos, offset, mouseMovementTime);
                yield return StartCoroutine(coroutine);
                currentPos = node;
            }
            //if lab solved, segue to next level
            if (dfs.Solved) {
                Debug.Log("Segue to next level");
                //shrink cheese
                IEnumerator shrinkCoroutine = Util.ShrinkAway(cheese.transform, 0.6f); 
                yield return StartCoroutine(shrinkCoroutine);
                //fade out level
                yield return StartCoroutine(FadeLevel(true));
                //segue to next level
                ResetLevel();
                if (currentLevelIndex+1 == levels.Length) {
                    //game complete
                    StartCoroutine(Util.LerpAlpha(gameCompleteImage, 0.5f, 0, 1));
                } else {
                    InitializeLevel(currentLevelIndex+1);
                    yield return new WaitForSeconds(0.3f);
                    //fade level back in
                    yield return StartCoroutine(FadeLevel(false));
                    disallowMovement = false;
                }
            } else {
                disallowMovement = false;
            }
        }

        IEnumerator FadeLevel(bool fadeout) {
            float startAlpha = fadeout ? 1 : 0;
            float targetAlpha = fadeout ? 0 : 1;
            float duration = 0.5f;
            foreach (RawImage button in selectButtons) {
                StartCoroutine(Util.LerpAlpha(button, duration, startAlpha, targetAlpha));
            }
            StartCoroutine(Util.LerpAlpha(playButton, duration, startAlpha, targetAlpha));
            if (!fadeout) { StartCoroutine(Util.LerpAlpha(hintImage, duration, startAlpha, targetAlpha)); }
            yield return StartCoroutine(Util.LerpAlpha(labyrinthDisplayImage, duration, startAlpha, targetAlpha));
        }

        float GetTargetRotation(Vector2 direction) {
            if (direction.x == 0 && direction.y == 1) { return 0f;}
            if (direction.x == 1 && direction.y == 0) { return 90f;}
            if (direction.x == 0 && direction.y == -1) { return 180f;}
            if (direction.x == -1 && direction.y == 0) { return 270f;}
            Debug.LogError("direction: "+direction+" not expected");
            return 0;
        }
    }

    private void ResetLevel() {
        Destroy(mouse);
        Destroy(cheese);
        //destroy all the tiles in the labyrinth
        foreach (Transform lab in labyrinthParents)
        {
            lab.position = Vector3.zero;
            for (int i = 0; i < lab.childCount; i++)
            {
                Transform child = lab.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }

    IEnumerator ShowHint(Texture hint) {
        if (hint == null) {
            hintImage.gameObject.SetActive(false);
        } else {
            hintImage.gameObject.SetActive(true);
            hintImage.texture = hint;
            // hintImage.SetNativeSize();
            yield return new WaitForSeconds(1f);
            StartCoroutine(Util.LerpAlpha(hintImage, 0.5f, 1, 0));
        }
    }
}
