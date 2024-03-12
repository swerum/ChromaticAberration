using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RGB { 
    RED,
    GREEN,
    BLUE,
    YELLOW,
    CYAN,
    MAGENTA,
    WHITE,
    BLACK,
    NONE
}

public static class ColorUtil 
{
    private static Color yellow = new Color(1f, 1f, 0f);
    private static Color magenta = new Color(1f, 0f, 1f);
    private static Color cyan = new Color(0f, 1f, 1f);
    private static Color white = new Color(1f, 1f, 1f);
    public static Color GetColorFromRGB(RGB rgb) {
        switch (rgb) {
            case RGB.RED:       return Color.red;
            case RGB.GREEN:     return Color.green;
            case RGB.BLUE:      return Color.blue;
            case RGB.YELLOW:    return yellow;
            case RGB.CYAN:      return cyan;
            case RGB.MAGENTA:   return magenta;
            case RGB.WHITE:     return white;
            case RGB.BLACK:      return Color.black;
            default:
                Debug.LogError("There is no RGB values '"+rgb+"'");
                return Color.black;
        }
    }

    public static RGB GetRGBFromColor(Color color) {
        color.a = 1;
        if (color == Color.red)     { return RGB.RED; }
        if (color == Color.green)   { return RGB.GREEN; }
        if (color == Color.blue)    { return RGB.BLUE; }
        if (color == yellow)  { return RGB.YELLOW; }
        if (color == cyan)    { return RGB.CYAN; }
        if (color == magenta) { return RGB.MAGENTA; }
        if (color == white)   { return RGB.WHITE; }
        if (color == Color.black)   { return RGB.BLACK; }
        return RGB.NONE;
    }

    public static RGB AddColors(RGB c1, RGB c2) {
        Color composite = GetColorFromRGB(c1)+ GetColorFromRGB(c2);
        return GetRGBFromColor(composite);
        
    }
}
