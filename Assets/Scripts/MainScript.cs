using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class MainScript : MonoBehaviour
{
    public Parser parser;
    public Interpreter interpreter;
    public Methods methods;
    public TMP_Text log;
    public TMP_Text input;
    public static string logText;
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
    public static int x;
    public static int y;

    void Start()
    {
        logText = log.text;
        large = 50;
        //Changing the scale of the cells
        cellScale = 10 / (float)large;
        CellFolder.transform.GetChild(0).localScale = new Vector3(cellScale, cellScale, 1);
        // Changing the scale of the pointer
        methods.pointer.transform.localScale = new Vector3((float)(cellScale / 20), (float)(cellScale / 20), 1);

        board = new GameObject[large, large];
        cellsBoard = new Cells[large, large];

        actualColor = CellColor.White;
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
    void EnumerateRowsAndColumns()
    {
        float separation = 0.25f;
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
            colNumber.transform.position = new Vector3(cellScale * col + cellScale/2, separation, 0);
        }
        
        // rows numbers
        for (int row = 0; row < large; row++)
        {
            GameObject rowNumber = Instantiate(numberPrefab, nums.transform);
            rowNumber.GetComponent<TMP_Text>().text = row.ToString();
            rowNumber.transform.position = new Vector3(-separation, -cellScale * row - cellScale/2, 0);
        }
    }
    public void Refresh()
    {
        logText = "";
        interpreter.error = false;
        methods.spawnsCount = 0;

        List<string> tokens = parser.Parsing(input.text);
        interpreter.MainInterpreter(tokens);
        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                board[f, c].GetComponent<SpriteRenderer>().sprite = CellFolder.transform.GetChild((int)cellsBoard[f, c].color).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
}
