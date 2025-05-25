using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class MainScript : MonoBehaviour
{
    public TMP_Text input;
    public GameObject[,] board;
    public Cells[,] cellsBoard;
    public GameObject CellFolder;
    public CellColor actualColor;
    public bool refresh = false;
    public int large;
    public float cellScale;

    void Start()
    {
        large = 20;
        //Cambiando el tamaño de las celdas
        cellScale = 10 / (float)large;
        CellFolder.transform.GetChild(0).localScale = new Vector3(cellScale, cellScale, 1);

        board = new GameObject[large, large];
        cellsBoard = new Cells[large, large];

        actualColor = CellColor.White;
        for (int f = 0; f < large; f++)
        {
            for (int c = 0; c < large; c++)
            {
                cellsBoard[f, c] = new Cells();
                board[f, c] = Instantiate(CellFolder.transform.GetChild(0).gameObject, new Vector3(cellScale * c, -cellScale * f, 1), Quaternion.identity);
            }
        }
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
    

    void Update()
    {
        if (refresh)
        {
            refresh = false;
            Refresh();
        }
    }
}
