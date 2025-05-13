using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class MainScript : MonoBehaviour
{
    public GameObject[,] board;
    public Cells[,] cellsBoard;
    public GameObject pointer;
    public GameObject CellFolder;
    public CellColor actualColor;
    public bool refresh = false;
    public int rows;
    public int cols;
    public float cellHeight;
    public float cellWidth;

    void Start()
    {
        rows = 20;
        cols = 20;
        //Cambiando el tamaño de las celdas y del puntero
        cellHeight = 5 / (float)rows;
        cellWidth = 5 / (float)cols;
        float minScale = cellHeight < cellWidth ? cellHeight : cellWidth;

        CellFolder.transform.GetChild(0).localScale = new Vector3(cellWidth, cellHeight, 1);
        pointer.transform.localScale = new Vector3(pointer.transform.localScale.x * minScale, pointer.transform.localScale.y * minScale, 1);

        board = new GameObject[rows, cols];
        cellsBoard = new Cells[rows, cols];

        actualColor = CellColor.White;
        for (int f = 0; f < rows; f++)
        {
            for (int c = 0; c < cols; c++)
            {
                cellsBoard[f, c] = new Cells();
                board[f, c] = Instantiate(CellFolder.transform.GetChild(0).gameObject, new Vector3(cellWidth * 2 * c, -cellHeight * 2 * f, 1), Quaternion.identity);
            }
        }
    }
    public void Refresh()
    {
        for (int f = 0; f < rows; f++)
        {
            for (int c = 0; c < cols; c++)
            {
                board[f, c].GetComponent<SpriteRenderer>().sprite = CellFolder.transform.GetChild((int)cellsBoard[f, c].color).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    public void PointerMove(int x, int y)
    {
        pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, new Vector3(x, y, 0), 10f);
    }

    void Update()
    {
        int x = (int)pointer.transform.position.x;
        int y = (int)pointer.transform.position.y;
        if (refresh)
        {
            refresh = false;
            Refresh();
        }
        if (Input.GetKeyDown("down")) PointerMove(x, y - 1);
        if (Input.GetKeyDown("up")) PointerMove(x, y + 1);
        if (Input.GetKeyDown("left")) PointerMove(x - 1, y);
        if (Input.GetKeyDown("right")) PointerMove(x + 1, y);
    }
}
