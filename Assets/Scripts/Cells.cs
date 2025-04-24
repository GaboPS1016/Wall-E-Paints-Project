using UnityEngine;

namespace CellsName
{
    public class Cells : ScriptableObject
    {
        public float height {get; set;}
        public float width {get; set;}
        public CellColor color {get; set;}
        public enum CellColor { White, Black, Blue, Red, Yellow, Green, Purple, Orange, Transparent };
        public Cells()
        {
            height = 1;
            width = 1;
            color = CellColor.White;
        }
    }
}


