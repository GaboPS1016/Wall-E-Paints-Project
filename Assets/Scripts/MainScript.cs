using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using System.Collections;
using System.Linq;
using System;
public class MainScript : MonoBehaviour
{
    public Parser parser;
    public Interpreter interpreter;
    public Methods methods;
    public TMP_Text log;
    public TMP_InputField input;
    public TMP_InputField inputDim;
    public GameObject[,] board;
    public Cells[,] cellsBoard;
    public GameObject CellFolder;
    public GameObject nums;
    public GameObject numberPrefab;
    public CellColor actualColor;
    public int actualBrushSize;
    public bool refresh = false;
    public int large;
    public float cellScale;
    public float delay;
    public int x;
    public int y;

    void Start()
    {
        large = 20;
        delay = 0.5f;
        //Changing the scale of the cells
        cellScale = 10 / (float)large;
        CellFolder.transform.GetChild(0).localScale = new Vector3(cellScale, cellScale, 1);
        // Changing the scale of the pointer
        methods.pointer.transform.localScale = new Vector3((float)(cellScale / 20), (float)(cellScale / 20), 1);

        board = new GameObject[large, large];
        cellsBoard = new Cells[large, large];

        actualColor = CellColor.Transparent;
        actualBrushSize = 1;
        methods.currentBrush = actualBrushSize;

        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                cellsBoard[f, c] = new Cells();
                board[f, c] = Instantiate(CellFolder.transform.GetChild(0).gameObject, new Vector3(cellScale * c, -cellScale * f, 1), Quaternion.identity);
            }
        }
        EnumerateRowsAndColumns();
    }
    void EnumerateRowsAndColumns()
    {
        float separation = 0.25f;
        Vector3 numscale = numberPrefab.transform.localScale;
        if (large > 25) numberPrefab.transform.localScale = new Vector3(numscale.x * cellScale * 2, numscale.y * cellScale * 2, 1);
        // delete old numbers
        foreach (Transform child in nums.transform)
        {
            Destroy(child.gameObject);
        }

        // cols numbers
        for (int col = 0; col < large; col++)
        {
            GameObject colNumber = Instantiate(numberPrefab, nums.transform);
            colNumber.GetComponent<TMP_Text>().text = col.ToString();
            colNumber.transform.position = new Vector3(cellScale * col + cellScale / 2, separation, 0);
        }

        // rows numbers
        for (int row = 0; row < large; row++)
        {
            GameObject rowNumber = Instantiate(numberPrefab, nums.transform);
            rowNumber.GetComponent<TMP_Text>().text = row.ToString();
            rowNumber.transform.position = new Vector3(-separation, -cellScale * row - cellScale / 2, 0);
        }
        numberPrefab.transform.localScale = new Vector3(numscale.x, numscale.y, 1);
    }
    public void Redimension()
    {
        int large2 = 20;
        log.text = "";
        interpreter.error = false;
        methods.spawnsCount = 0;
        interpreter.numVars.Clear();
        interpreter.boolVars.Clear();

        //Catching input
        string cleanInput = inputDim.text.Trim();
        if (string.IsNullOrEmpty(cleanInput)) large2 = 20;
        else
        {
            try
            {
                large2 = int.Parse(cleanInput);
                if (large2 <= 0)
                {
                    log.text = "ERROR!!!! INCORRECT DIMENSION NUMBER: " + large2;
                    return;
                }
                if (large2 > 200) large2 = 200;
            }
            catch (System.Exception)
            {
                log.text = "ERROR!!!! INCORRECT DIMENSION INPUT: " + large2;
                return;
            }
        }

        //Destroy actual board
        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                Destroy(board[f, c]);
            }
        }
        large = large2;
        //Changing the scale of the cells
        cellScale = 10 / (float)large;
        CellFolder.transform.GetChild(0).localScale = new Vector3(cellScale, cellScale, 1);
        // Changing the scale of the pointer
        methods.pointer.transform.localScale = new Vector3((float)(cellScale / 20), (float)(cellScale / 20), 1);

        board = new GameObject[large, large];
        cellsBoard = new Cells[large, large];

        actualColor = CellColor.Transparent;
        actualBrushSize = 1;

        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                cellsBoard[f, c] = new Cells();
                board[f, c] = Instantiate(CellFolder.transform.GetChild(0).gameObject, new Vector3(cellScale * c, -cellScale * f, 1), Quaternion.identity);
            }
        }
        EnumerateRowsAndColumns();
    }
    public void Compile()
    {
        Reset();

        List<string> tokens = parser.Parsing(input.text);
        interpreter.MainInterpreter(tokens);
        
        Refresh();
    }
    public void Refresh()
    {
        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                board[f, c].GetComponent<SpriteRenderer>().sprite = CellFolder.transform.GetChild((int)cellsBoard[f, c].color).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    public void Reset()
    {
        log.text = "";
        interpreter.error = false;
        methods.spawnsCount = 0;
        actualBrushSize = 1;
        methods.currentBrush = 1;
        actualColor = CellColor.Transparent;
        x = 0;
        y = 0;
        interpreter.numVars.Clear();
        interpreter.boolVars.Clear();
        methods.pointer.transform.position = new Vector3(-30, 0, 0);

        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                cellsBoard[f, c].color = CellColor.White;
                board[f, c].GetComponent<SpriteRenderer>().sprite = CellFolder.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    public void Save()
    {
        string content = input.text;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Archivo PW", ".pw"));
        FileBrowser.SetDefaultFilter(".pw");

        FileBrowser.ShowSaveDialog(
            onSuccess: paths =>
            {
                string path = paths[0];
                try
                {
                    File.WriteAllText(path, content);
                    log.text = "Archivo guardado correctamente en: " + path;
                }
                catch (Exception ex)
                {
                    log.text = "Error al guardar archivo: " + ex.Message;
                }
            },
            onCancel: () =>
            {
                log.text = "Cancelado por el usuario";
            },
            pickMode: FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: null,
            title: "Guardar como",
            saveButtonText: "Guardar"
        );
    }
    public void Load()
    {
        FileBrowser.ShowLoadDialog(
            onSuccess: paths =>
            {
                string path = paths[0];
                try
                {
                    input.text = File.ReadAllText(path);
                    log.text = "Archivo cargado correctamente";
                }
                catch (Exception ex)
                {
                    log.text = "Error al cargar archivo: " + ex.Message;
                }
            },
            onCancel: () =>
            {
                log.text = "Cancelado por el usuario";
            },
            pickMode: FileBrowser.PickMode.Files,
            allowMultiSelection: false,
            initialPath: null,
            title: "Cargar",
            loadButtonText: "Cargar"
        );
    }
}
