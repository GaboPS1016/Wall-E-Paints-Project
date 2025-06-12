using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;

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
            main.log.text = "ERROR!!!! SPAWN FUERA DEL CANVAS";
            interpreter.error = true;
            return;
        }
        spawnsCount++;
        if (spawnsCount > 1)
        {
            main.log.text = "ERROR!!!! SOLO PUEDE HABER UN SPAWN";
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
            main.log.text = "ERROR!!!! \"" + color + "\" NO ES UN COLOR DEFINIDO";
            interpreter.error = true;
            return;
        }
    }
    public void Size(int x)
    {
        if (x <= 0)
        {
            main.log.text = "ERROR!!!! TAMAÑO INCORRECTO DE LA BROCHA";
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
            main.log.text = "ERROR!!!! DIBUJANDO FUERA DEL CANVAS";
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
            main.Refresh();
            return;
        }
        
        // move the pointer
        pointer.transform.position = new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);
        main.Refresh();
        if (distance <= 1) return;
        DrawLine(dirX, dirY, distance - 1);
    }
    public void DrawCircle(int dirX, int dirY, int radius)
    {
        if (radius < 0)
        {
            main.log.text = "ERROR!!!! EL RADIO DEL CÍRCULO NO PUEDE SER NEGATIVO";
            interpreter.error = true;
            return;
        }
        if (radius == 0)
        {
            if (main.actualColor != CellColor.Transparent) main.cellsBoard[main.y, main.x].color = main.actualColor;
            return;
        }
        //normalizer
        dirX = dirX > 0 ? 1 : (dirX < 0 ? -1 : 0);
        dirY = dirY > 0 ? 1 : (dirY < 0 ? -1 : 0);
        //center
        int centerX = main.x + radius * dirX;
        int centerY = main.y + radius * dirY;

        main.x = Mathf.Clamp(centerX, 0, main.large - 1);
        main.y = Mathf.Clamp(centerY, 0, main.large - 1);

        if (centerX < 0 || centerX >= main.large || centerY < 0 || centerY >= main.large)
        {
            main.log.text = "ERROR!!!! EL CENTRO DEL CÍRCULO ESTÁ FUERA DEL CANVAS";
            interpreter.error = true;
            return;
        }
        main.x = centerX;
        main.y = centerY;

        // move the pointer
        pointer.transform.position = new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);

        if (main.actualColor == CellColor.Transparent) return;

        //converting to diagonal radius
        radius = (int)(radius * 1.5f);

        //Bresenham's algorithm
        int x = 0;
        int y = radius;
        int d = 3 - 2 * radius;

        while (x <= y)
        {
            AddCirclePoints(centerX, centerY, x, y);

            if (d < 0)
            {
                d = d + 4 * x + 6;
            }
            else
            {
                d = d + 4 * (x - y) + 10;
                y--;
            }
            x++;
        }
        main.Refresh();
    }
    public void AddCirclePoints(int centerX, int centerY, int x, int y)
    {
        //four simetrical octancts
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                for (int brushW = -main.actualBrushSize / 2; brushW <= main.actualBrushSize / 2; brushW++)
                {
                    for (int brushH = -main.actualBrushSize / 2; brushH <= main.actualBrushSize / 2; brushH++)
                    {
                        int px = centerX + x * i + brushW;
                        int py = centerY + y * j + brushH;
                        if (px < 0 || px >= main.large || py < 0 || py >= main.large) continue;
                        main.cellsBoard[py, px].color = main.actualColor;
                    }
                }
            }
        }
        //other four inverted octants
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                for (int brushW = -main.actualBrushSize / 2; brushW <= main.actualBrushSize / 2; brushW++)
                {
                    for (int brushH = -main.actualBrushSize / 2; brushH <= main.actualBrushSize / 2; brushH++)
                    {
                        int px = centerX + y * i + brushW;
                        int py = centerY + x * j + brushH;
                        if (px < 0 || px >= main.large || py < 0 || py >= main.large) continue;
                        main.cellsBoard[py, px].color = main.actualColor;
                    }
                }
            }
        }
    }
    public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {
        if (distance < 0 || width < 0 || height < 0)
        {
            main.log.text = "ERROR!!!! LOS PARÁMETROS DEL RECTÁNGULO DEBEN SER POSITIVOS";
            interpreter.error = true;
            return;
        }
        //normalizer
        dirX = dirX > 0 ? 1 : (dirX < 0 ? -1 : 0);
        dirY = dirY > 0 ? 1 : (dirY < 0 ? -1 : 0);
        //center
        int centerX = main.x + distance * dirX;
        int centerY = main.y + distance * dirY;

        main.x = Mathf.Clamp(centerX, 0, main.large - 1);
        main.y = Mathf.Clamp(centerY, 0, main.large - 1);

        // move the pointer
        pointer.transform.position = new Vector3(main.cellScale * main.x, -main.cellScale * main.y, 0);

        if (centerX < 0 || centerX >= main.large || centerY < 0 || centerY >= main.large)
        {
            main.log.text = "ERROR!!!! EL CENTRO DEL RECTÁNGULO ESTÁ FUERA DEL CANVAS";
            interpreter.error = true;
            return;
        }

        if (width == 0 || height == 0) return;
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        int semiWidth = width / 2 + 1;
        int semiHeight = height / 2 + 1;
        //horizontal sides
        for (int sign = -1; sign <= 1; sign += 2)
        {
            for (int brushW = -main.actualBrushSize / 2; brushW <= main.actualBrushSize / 2; brushW++)
            {
                for (int brushH = -main.actualBrushSize / 2; brushH <= main.actualBrushSize / 2; brushH++)
                {
                    for (int i = -semiWidth; i <= semiWidth; i++)
                    {
                        int currentX = centerX + i + brushH;
                        int currentY = centerY + (semiHeight + brushW) * sign;
                        if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                        if (main.actualColor != CellColor.Transparent) main.cellsBoard[currentY, currentX].color = main.actualColor;
                    }
                }
            }
        }
        //vertical sides
        for (int sign = -1; sign <= 1; sign += 2)
        {
            for (int brushW = -main.actualBrushSize / 2; brushW <= main.actualBrushSize / 2; brushW++)
            {
                for (int brushH = -main.actualBrushSize / 2; brushH <= main.actualBrushSize / 2; brushH++)
                {
                    for (int i = -semiHeight; i <= semiHeight; i++)
                    {
                        int currentX = centerX + (semiWidth + brushW) * sign;
                        int currentY = centerY + i + brushH;
                        if (currentX < 0 || currentX >= main.large || currentY < 0 || currentY >= main.large) continue;
                        if (main.actualColor != CellColor.Transparent) main.cellsBoard[currentY, currentX].color = main.actualColor;
                    }
                }
            }
        }
        main.Refresh();
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
        if (Enum.TryParse(color, out CellColor cellcolor))
        {
            if (x1 < 0 || x1 >= main.large || x2 < 0 || x2 >= main.large || y1 < 0 || y1 >= main.large || y2 < 0 || y2 >= main.large) return 0;
            int cont = 0;
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    if (main.cellsBoard[y, x].color == cellcolor) cont++;
                }
            }
            return cont;
        }
        else
        {
            main.log.text = "ERROR!!!! \"" + color + "\" NO ES UN COLOR EXISTENTE";
            interpreter.error = true;
            return 0;
        }
    }
    public  int IsBrushColor(string color)
    {
        if (Enum.TryParse(color, out CellColor cellcolor))
        {
            if (cellcolor == main.actualColor) return 1;
            else return 0;
        }
        else
        {
            main.log.text = "ERROR!!!! \"" + color + "\" NO ES UN COLOR EXISTENTE";
            interpreter.error = true;
            return 0;
        }
    }
    public  int IsBrushSize(int size)
    {
        if (size == main.actualBrushSize) return 1;
        else return 0;
    }
    public int IsCanvasColor(string color, int vertical, int horizontal)
    {
        if (Enum.TryParse(color, out CellColor cellcolor))
        {
            int x = main.x + vertical;
            int y = main.y + horizontal;
            if (x < 0 || x >= main.large || y < 0 || y >= main.large) return 0;
            if (main.cellsBoard[y, x].color == cellcolor) return 1;
            return 0;
        }
        else
        {
            main.log.text = "ERROR!!!! \"" + color + "\" NO ES UN COLOR EXISTENTE";
            interpreter.error = true;
            return 0;
        } 
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


