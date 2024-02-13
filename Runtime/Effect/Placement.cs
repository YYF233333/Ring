namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;
using Godot;

public record Placement
{
    public float scale { get; set; }
    public Vector2 position { get; set; }
}

public static class Placements
{
    static Dictionary<string, Placement> table = new()
    {
        {"farleft", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"farmiddle", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"farright", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"left", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"middle", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"right", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"nearleft", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"nearmiddle", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } },
        {"nearright", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } }
    };

    public static Placement BG = new() { scale = 1.0f, position = new Vector2(0.0f, 0.0f) };

    public static Placement Get(string name)
    {
        return table[name];
    }

}
