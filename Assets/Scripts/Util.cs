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
    NONE
}

public static class Util 
{
    public static Color GetColorFromRGB(RGB rgb) {
        switch (rgb) {
            case RGB.RED:       return Color.red;
            case RGB.GREEN:     return Color.green;
            case RGB.BLUE:      return Color.blue;
            case RGB.YELLOW:    return Color.yellow;
            case RGB.CYAN:      return Color.cyan;
            case RGB.MAGENTA:   return Color.magenta;
            case RGB.WHITE:     return Color.white;
            default:
                Debug.LogError("There is no RGB values '"+rgb+"'");
                return Color.black;
        }
    }

    public static RGB GetRGBFromColor(Color color) {
        if (color == Color.red)     { return RGB.RED; }
        if (color == Color.green)   { return RGB.GREEN; }
        if (color == Color.blue)    { return RGB.BLUE; }
        if (color == Color.yellow)  { return RGB.YELLOW; }
        if (color == Color.cyan)    { return RGB.CYAN; }
        if (color == Color.magenta) { return RGB.MAGENTA; }
        if (color == Color.white)   { return RGB.WHITE; }
        return RGB.NONE;
    }

    public static RGB AddColors(RGB c1, RGB c2) {
        Color composite = GetColorFromRGB(c1)+ GetColorFromRGB(c2);
        return GetRGBFromColor(composite);
        
    }
}
