using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private int selectedButtonIndex;
    [SerializeField] GameManager gameManager;
    [SerializeField] Button[] buttons;

    public void OnButtonPress(int buttonIndex) {
        selectedButtonIndex = buttonIndex;
        buttons[buttonIndex].Select();
        foreach (Button button in buttons)
        {
            if (button.ButtonIndex != buttonIndex) {
                button.Deselect();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttons[0].Select();
        selectedButtonIndex = buttons[0].ButtonIndex;
    }

    // Update is called once per frame
    void Update()
    {
        //check for wasd and arrow keys
        Vector2 direction = Vector2.zero;
        direction.y = Math.Sign(Input.GetAxis("Horizontal"));
        if (direction.y != 0) {
            gameManager.MoveLabyrinth(selectedButtonIndex, direction);
            return;
        }
        direction.x = -Math.Sign(Input.GetAxis("Vertical"));
        if (direction.x != 0) {
            gameManager.MoveLabyrinth(selectedButtonIndex, direction);
        }

        //check for 1,2,3 which activate the buttons
        if (Input.GetAxis("Button1") > 0)       { OnButtonPress(0); }
        else if (Input.GetAxis("Button2") > 0)  { OnButtonPress(1); }
        else if (Input.GetAxis("Button3") > 0)  { OnButtonPress(2); }
    }
}
