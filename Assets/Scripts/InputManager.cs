using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private RGB selectedColor;
    [SerializeField] GameManager gameManager;
    [SerializeField] Button[] buttons;

    public void OnButtonPress(RGB buttonColor) {
        selectedColor = buttonColor;
        foreach (Button button in buttons)
        {
            if (button.ButtonColor != buttonColor) {
                button.Deselect();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttons[0].Select();
        selectedColor = buttons[0].ButtonColor;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Vector2.zero;
        direction.x = -Math.Sign(Input.GetAxis("Horizontal"));
        if (direction.x != 0) {
            gameManager.MoveLabyrinth(selectedColor, direction);
            return;
        }
        direction.y = -Math.Sign(Input.GetAxis("Vertical"));
        if (direction.y != 0) {
            gameManager.MoveLabyrinth(selectedColor, direction);
        }
    }
}
