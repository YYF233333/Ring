namespace RingEngine.Runtime.Effect;

public class Placement
{
    public float x;
    public float y;
    public float scale;

    public Placement(double x, double y, double scale)
    {
        this.x = (float)x;
        this.y = (float)y;
        this.scale = (float)scale;
    }

    public static Placement BG => new(0.0, 0.0, 1.0);
}
