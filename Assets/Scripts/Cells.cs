public class Cells
{
    public CellColor color { get; set; }
    public Cells()
    {
        color = CellColor.White;
    }
}
public enum CellColor { White, Black, Blue, Red, Yellow, Green, Purple, Orange, Transparent };