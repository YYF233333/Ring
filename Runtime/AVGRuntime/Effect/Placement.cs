namespace RingEngine.Runtime.AVGRuntime.Effect;

using System;
using Godot;

public class Placement
{
    public float x;
    public float y;
    public float scale;

    public Vector2 Position => new(x, y);

    public Placement(double x, double y, double scale)
    {
        this.x = (float)x;
        this.y = (float)y;
        this.scale = (float)scale;
    }

    public static Placement BG => new(0.0, 0.0, 1.0);

    public override bool Equals(object obj) =>
        obj is Placement placement
        && this.x == placement.x
        && this.y == placement.y
        && this.scale == placement.scale;

    public override int GetHashCode() => HashCode.Combine(this.x, this.y, this.scale);
}
