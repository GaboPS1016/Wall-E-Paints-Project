using UnityEngine;

public class Pointer : MonoBehaviour
{
    public MainScript mainS;
    public GameObject pointer;
    public void PointerMove(int x, int y)
    {
        pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, new Vector3(x, y, 0), 10f);
    }
    void Start()
    {
        pointer.transform.localScale = new Vector3((float)(pointer.transform.localScale.x * mainS.cellScale), (float)(pointer.transform.localScale.y * mainS.cellScale), 1);
    }
    void Update()
    {
        int x = (int)pointer.transform.position.x;
        int y = (int)pointer.transform.position.y;

        if (Input.GetKeyDown("down")) PointerMove(x, y - (int)(pointer.transform.localScale.x * mainS.cellScale * 2));
        if (Input.GetKeyDown("up")) PointerMove(x, y + (int)(pointer.transform.localScale.x * mainS.cellScale * 2));
        if (Input.GetKeyDown("left")) PointerMove(x - (int)(pointer.transform.localScale.x * mainS.cellScale * 2), y);
        if (Input.GetKeyDown("right")) PointerMove(x + (int)(pointer.transform.localScale.x * mainS.cellScale * 2), y);
    }
}
