using Godot;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingEngine.Runtime.Effect
{
    public record Placement
    {
        public float scale { get; set; }
        public Vector2 position { get; set; }
    }

    public static class Placements
    {
        static Dictionary<string, Placement> table = new Dictionary<string, Placement>
        {
            {"farleft", new Placement { scale = 0.5f, position = new Vector2(0.0f, 0.0f) } }
        };

        public static Placement Get(string name)
        {
            return table[name];
        }

    }

}
