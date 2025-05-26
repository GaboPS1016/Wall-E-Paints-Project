using UnityEngine;
using System;
using System.Collections.Generic;

public class Methods : MonoBehaviour
{
    public int spawnsCount;
    public Interpreter interpreter;
    public GameObject pointer;
    public MainScript main;
    public void DoAction(Method method, List<int> parameters, string colorstr)
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
    public  int DoFunction(Method method, List<int> parameters, string colorstr)
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
    public void Spawn(int x, int y)
    {
        if (x > main.large || x < 0 || y > main.large || y < 0)
        {
            MainScript.logText = "ERROR!!!! SPAWN OUT OF CANVAS";
            interpreter.error = true;
            return;
        }
        spawnsCount++;
        if (spawnsCount > 1)
        {
            MainScript.logText = "ERROR!!!! THERE CAN ONLY BE ONE SPAWN";
            interpreter.error = true;
            return;
        }
        MainScript.logText = "Empezaste en " + x + ", " + y + "\n";
        MainScript.x = x;
        MainScript.y = y;
        pointer.transform.position = new Vector3(main.cellScale * x, -main.cellScale * y, 0);
    }
    public  void Color(string color)
    {

    }
    public  void Size(int x)
    {

    }
    public  void DrawLine(int dirX, int dirY, int distance)
    {

    }
    public  void DrawCircle(int dirX, int dirY, int radius)
    {

    }
    public  void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {

    }
    public  void Fill()
    {

    }
    public  int GetActualX()
    {
        return 0;
    }
    public  int GetActualY()
    {
        return 0;
    }
    public  int GetCanvasSize()
    {
        return 0;
    }
    public  int GetColorCount(string color, int x1, int y1, int x2, int y2)
    {
        return 0;
    }
    public  int IsBrushColor(string color)
    {
        return 0;
    }
    public  int IsBrushSize(int size)
    {
        return 0;
    }
    public  int IsCanvasColor(string color, int vertical, int horizontal)
    {
        return 0;
    }
    public  Dictionary<Method, int> ParametersDictionary = new()
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


