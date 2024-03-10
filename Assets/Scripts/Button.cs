using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private int buttonIndex;
    public int ButtonIndex { get { return buttonIndex; }}
    private bool isSelected = false;

    [Header("Textures")]
    [SerializeField] private Texture2D onDeselectedTex;
    [SerializeField] private Texture2D onDeselectedHoverTex;
    [SerializeField] private Texture2D onSelectedTex;
    [SerializeField] private Texture2D onSelectedHoverTex;
    // Start is called before the first frame update
    private void Awake() {

        rawImage = gameObject.GetComponent<RawImage>();
        Debug.Assert(rawImage != null);
        rawImage.texture = onDeselectedTex;
    }

    private void OnMouseEnter() {
        if (isSelected) {
            rawImage.texture = onSelectedHoverTex;
        } else {
            rawImage.texture = onDeselectedHoverTex;
        }
    }
    private void OnMouseExit() {
        if (!isSelected) {
            rawImage.texture = onDeselectedTex;
        } else {
            rawImage.texture = onSelectedTex;
        }
    }
    private void OnMouseUpAsButton() {
        inputManager.OnButtonPress(buttonIndex);
        Select();
    }

    public void Deselect() {
        rawImage.texture = onDeselectedTex;
        isSelected = false;
    }
    public void Select() {
        rawImage.texture = onSelectedHoverTex;
        isSelected = true;
    }
}
