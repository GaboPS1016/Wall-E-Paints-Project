using UnityEngine;
using System;
using System.Collections.Generic;

public class Methods : MonoBehaviour
{
    public static void DoAction(Method method, List<int> parameters, string colorstr)
    {
        switch (method)
        {
            case Method.Spawn:
                Spawn(parameters[0], parameters[1]);
                break;
            case Method.Color:
                Color(colorstr);
                break;
            case Method.Size:
                Size(parameters[0]);
                break;
            case Method.DrawLine:
                DrawLine(parameters[0], parameters[1], parameters[2]);
                break;
            case Method.DrawCircle:
                DrawCircle(parameters[0], parameters[1], parameters[2]);
                break;
            case Method.DrawRectangle:
                DrawRectangle(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                break;
        }
    }
    public static int DoFunction(Method method, List<int> parameters, string colorstr)
    {
        switch (method)
        {
            case Method.GetActualX:
                return GetActualX();
            case Method.GetActualY:
                return GetActualY();
            case Method.GetCanvasSize:
                return GetCanvasSize();
            case Method.GetColorCount:
                return GetColorCount(colorstr, parameters[0], parameters[1], parameters[2], parameters[3]);
            case Method.IsBrushColor:
                return IsBrushColor(colorstr);
            case Method.IsBrushSize:
                return IsBrushSize(parameters[0]);
            case Method.IsCanvasColor:
                return IsCanvasColor(colorstr, parameters[0], parameters[1]);
        }
        return 0;
    }
    public static void Spawn(int x, int y)
    {
        Console.WriteLine("Empezaste en " + x + ", " + y);
    }
    public static void Color(string color)
    {

    }
    public static void Size(int x)
    {

    }
    public static void DrawLine(int dirX, int dirY, int distance)
    {

    }
    public static void DrawCircle(int dirX, int dirY, int radius)
    {

    }
    public static void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {

    }
    public static void Fill()
    {

    }
    public static int GetActualX()
    {
        return 0;
    }
    public static int GetActualY()
    {
        return 0;
    }
    public static int GetCanvasSize()
    {
        return 0;
    }
    public static int GetColorCount(string color, int x1, int y1, int x2, int y2)
    {
        return 0;
    }
    public static int IsBrushColor(string color)
    {
        return 0;
    }
    public static int IsBrushSize(int size)
    {
        return 0;
    }
    public static int IsCanvasColor(string color, int vertical, int horizontal)
    {
        return 0;
    }
    public static Dictionary<Method, int> ParametersDictionary = new()
    {
        { Method.Spawn, 2},
        { Method.Color, 1},
        { Method.Size, 1},
        { Method.DrawLine, 3},
        { Method.DrawCircle, 3},
        { Method.DrawRectangle, 5},
        { Method.Fill, 0},
        { Method.GetActualX, 0},
        { Method.GetActualY, 0},
        { Method.GetCanvasSize, 0},
        { Method.GetColorCount, 5},
        { Method.IsBrushColor, 1},
        { Method.IsBrushSize, 1},
        { Method.IsCanvasColor, 3}
    };
}

public enum Method { Spawn, Color, Size, DrawLine, DrawCircle, DrawRectangle, Fill, GetActualX, GetActualY, GetCanvasSize, GetColorCount, IsBrushColor, IsBrushSize, IsCanvasColor}


