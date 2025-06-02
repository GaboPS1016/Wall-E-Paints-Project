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
    public int currentBrush;
    public int rectSize;
    public bool resize = true;
    public CellColor colorToFill;
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
                resize = true;
                currentBrush = main.actualBrushSize;
                break;
            case Method.DrawCircle:
                DrawCircle(parameters[0], parameters[1], parameters[2]);
                break;
            case Method.DrawRectangle:
                DrawRectangle(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
                break;
            case Method.Fill:
                if (main.actualColor == CellColor.Transparent) break;
                colorToFill = main.cellsBoard[main.y, main.x].color;
                Fill(main.x, main.y);
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
        currentBrush = main.actualBrushSize;
        rectSize = main.actualBrushSize;
    }
    public void DrawLine(int dirX, int dirY, int distance)
    {
        bool indexout = false;
        //normalizer
        dirX = dirX > 0 ? 1 : (dirX < 0 ? -1 : 0);
        dirY = dirY > 0 ? 1 : (dirY < 0 ? -1 : 0);

        //large of side of the square points
        if (dirX == 0 && dirY == 0) return;
        if (distance <= 0) return;

        

        //diagonal
        if (dirX != 0 && dirY != 0)
        {
            if (distance < currentBrush - 1) currentBrush = (distance % 2 == 0) ? distance + 1 : distance;
            for (int f = 0; f < currentBrush; f++)
            {
                for (int c = 0; c < currentBrush; c++)
                {
                    //pixel to paint
                    int currentX = main.x + (dirX * c);
                    int currentY = main.y + (dirY * f);

                    //checking limits
                    if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                    //painting
                    if (main.actualColor != CellColor.Transparent) main.cellsBoard[currentY, currentX].color = main.actualColor;
                }
            }
        }
        //horizontal or vertical
        else
        {   
            if (resize)
            {
                resize = false;
                if (main.actualBrushSize > distance) rectSize = distance - distance % 2;
                else rectSize = main.actualBrushSize;
            }
            for (int i = -rectSize / 2; i <= rectSize / 2; i++)
            {
                //pixel to paint
                int currentX = main.x;
                int currentY = main.y;
                if (dirX == 0) currentX += i;
                if (dirY == 0) currentY += i;

                //checking limits
                if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                //painting
                if (main.actualColor != CellColor.Transparent) main.cellsBoard[currentY, currentX].color = main.actualColor;
            }
        }
        
        
        // pointer in the limits
        if (main.x + dirX < 0 || main.x + dirX >= main.large || main.y + dirY < 0 || main.y + dirY >= main.large)
        {
            indexout = true;
            main.log.text = "ERROR!!!! DRAWING OUT OF CANVAS";
            interpreter.error = true;
        }

        main.x = Mathf.Clamp(main.x + dirX, 0, main.large - 1);
        main.y = Mathf.Clamp(main.y + dirY, 0, main.large - 1);

        //last pixel
        if (!indexout && distance == 1 && main.actualColor != CellColor.Transparent)
        {
            if (dirX == 0 || dirY == 0)
            {
                for (int i = -rectSize / 2; i <= rectSize / 2; i++)
                {
                    //pixel to paint
                    int currentX = main.x;
                    int currentY = main.y;
                    if (dirX == 0) currentX += i;
                    if (dirY == 0) currentY += i;

                    //checking limits
                    if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                    //painting
                    main.cellsBoard[currentY, currentX].color = main.actualColor;
                }
                
            }
            else main.cellsBoard[main.y, main.x].color = main.actualColor;
        }    
        if (indexout)
        {
            indexout = false;
            Refresh();
            return;
        }
        
        // move the pointer
        pointer.transform.position = new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);
        //Vector3.MoveTowards(pointer.transform.position, new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0), 1);
        //new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);
        Refresh();
        if (distance <= 1) return;
        DrawLine(dirX, dirY, distance - 1);
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
    public void Fill(int x, int y)
    {
        main.cellsBoard[y, x].color = main.actualColor;
        for (int f = -1; f <= 1; f++)
        {
            for (int c = -1; c <= 1; c++)
            {
                if (f == 0 && c == 0) continue;
                if (f != 0 && c != 0) continue;

                int currentX = x + c;
                int currentY = y + f;
                //checking limits
                if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                if (main.cellsBoard[currentY, currentX].color != colorToFill) continue;
                Fill(currentX, currentY);
            }
        }
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


