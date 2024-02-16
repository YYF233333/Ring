namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;

public class Placement
{
    public float x;
    public float scale;

    public Placement(double x, double scale)
    {
        this.x = (float)x;
        this.scale = (float)scale;
    }

    public static Placement BG => new(0.0, 1.0);
}
