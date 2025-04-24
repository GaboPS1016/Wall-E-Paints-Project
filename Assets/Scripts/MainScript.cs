using UnityEngine;
using CellsName;
public class MainScript : MonoBehaviour
{
    public GameObject[,] board;
    public Cells[,] cellsBoard;
    public GameObject CellFolder;
    public Cells cells;
    public bool Change = false;
    public int rows;
    public int cols;
    void Start()
    {
        rows = 10;
        cols = 10;

        board = new GameObject[rows, cols];
        cellsBoard = new Cells[rows, cols];

        for (int f = 0; f < rows; f++)
        {
            for (int c = 0; c < cols; c++)
            {
                cellsBoard[f,c] = ScriptableObject.CreateInstance<Cells>();
                board[f,c] = Instantiate(CellFolder.transform.GetChild(0).gameObject, new Vector3(c, -f, 1), Quaternion.identity);
            }
        }
    }
    public void Refresh()
    {
        for (int f = 0; f < rows; f++)
        {
            for (int c = 0; c < cols; c++)
            {
                board[f,c].GetComponent<SpriteRenderer>().sprite = CellFolder.transform.GetChild((int)cellsBoard[f,c].color).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
    void Update()
    {
        
    }
}
