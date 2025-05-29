using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Methods : MonoBehaviour
{
    public int spawnsCount = 0;
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
    public int DoFunction(Method method, List<int> parameters, string colorstr)
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
        if (x >= main.large || x < 0 || y >= main.large || y < 0)
        {
            main.log.text = "ERROR!!!! SPAWN OUT OF CANVAS";
            interpreter.error = true;
            return;
        }
        spawnsCount++;
        if (spawnsCount > 1)
        {
            main.log.text = "ERROR!!!! THERE CAN ONLY BE ONE SPAWN";
            interpreter.error = true;
            return;
        }
        main.x = x;
        main.y = y;
        pointer.transform.position = new Vector3(main.cellScale * x, -main.cellScale * y, 0);
    }
    public void Color(string color)
    {
        if (Enum.TryParse(color, out CellColor cellcolor)) main.actualColor = cellcolor;
        else
        {
            main.log.text = "ERROR!!!! \"" + color + "\" IS NOT A DEFINED COLOR";
            interpreter.error = true;
            return;
        }
    }
    public void Size(int x)
    {
        if (x <= 0)
        {
            main.log.text = "ERROR!!!! INCORRECT BRUSH SIZE ASSIGNMENT";
            interpreter.error = true;
            return;
        }
        if (x % 2 == 0) main.actualBrushSize = x - 1;
        else main.actualBrushSize = x;
    }
    public void DrawLine(int dirX, int dirY, int distance)
    {
        //normalizer
        dirX = dirX > 0 ? 1 : (dirX < 0 ? -1 : 0);
        dirY = dirY > 0 ? 1 : (dirY < 0 ? -1 : 0);

        if (distance <= 0) return;
        if (distance >= main.actualBrushSize - 1)
        {
            Segment(dirX, dirY, distance, main.actualBrushSize);
            DrawLine(dirX, dirY, distance-1);
        }
        else
        {
            int localsize = (distance % 2 == 0) ? distance + 1 : distance;
            Segment(dirX, dirY, distance, localsize);
            DrawLine(dirX, dirY, distance-1);
        }
    }
    public void Segment(int dirX, int dirY, int distance, int side) // method for DrawLine()
    {
        for (int f = 0; f < side; f++)
        {
            for (int c = 0; c < side; c++)
            {
                int currentX = main.x + (dirX * c);
                int currentY = main.y + (dirY * f);

                if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;

                if (main.actualColor != CellColor.Transparent) main.cellsBoard[currentX, currentY].color = main.actualColor;
            }
        }
        // pointer in the limits
        main.x = Mathf.Clamp(main.x + (dirX * distance), 0, main.large - 1);
        main.y = Mathf.Clamp(main.y + (dirY * distance), 0, main.large - 1);

        // move the pointer
        pointer.transform.position = new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);
        //Vector3.MoveTowards(pointer.transform.position, new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0), 1);
        Refresh();
        //StartCoroutine(Delay());
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(main.delay);
    }
    public void Refresh()
    {
        for (int f = 0; f < main.large; f++)
        {
            for (int c = 0; c < main.large; c++)
            {
                main.board[f, c].GetComponent<SpriteRenderer>().sprite = main.CellFolder.transform.GetChild((int)main.cellsBoard[f, c].color).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    public void DrawCircle(int dirX, int dirY, int radius)
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
        return main.x;
    }
    public  int GetActualY()
    {
        return main.y;
    }
    public  int GetCanvasSize()
    {
        return main.large;
    }
    public  int GetColorCount(string color, int x1, int y1, int x2, int y2)
    {
        return 0;
    }
    public  int IsBrushColor(string color)
    {
        if (Enum.TryParse(color, out CellColor cellcolor))
        {
            if (cellcolor == main.actualColor) return 1;
            else return 0;
        }
        else return 0;
    }
    public  int IsBrushSize(int size)
    {
        if (size == main.actualBrushSize) return 1;
        else return 0;
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


