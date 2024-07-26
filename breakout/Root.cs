using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MessagePack;
using RingEngine.Runtime;

[MessagePackObject(keyAsPropertyName: true)]
public class BreakoutMessage
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Consumable
    {
        public string name;
        public int rest_times;
        public bool transformed;
    }

    public Dictionary<string, Consumable> player_consumables = [];
    public string selected_skill;
    public string[] selected_policy;
    public string current_level;
    public int player_max_health { get; set; }
    public int player_init_ammo;
    public Dictionary<string, string> level_result;

    public Godot.Collections.Dictionary AsDict()
    {
        var dict = new Godot.Collections.Dictionary()
        {
            { "selected_skill", selected_skill },
            { "selected_policy", selected_policy },
            { "current_level", current_level },
            { "player_max_health", player_max_health },
            { "player_init_ammo", player_init_ammo },
        };
        var d = new Godot.Collections.Dictionary();
        foreach (var (key, value) in player_consumables)
        {
            d[key] = new Godot.Collections.Dictionary()
            {
                { "rest_times", value.rest_times },
                { "transformed", value.transformed },
            };
        }
        dict["player_consumables"] = d;
        return dict;
    }

    public static BreakoutMessage FromDict(Godot.Collections.Dictionary dict)
    {
        var message = new BreakoutMessage
        {
            level_result = dict["level_result"].AsGodotDictionary<string, string>().ToDictionary(),
            player_consumables = []
        };
        foreach (var (name, value) in dict["player_consumables"].AsGodotDictionary())
        {
            var d = value.AsGodotDictionary();
            message.player_consumables[name.AsString()] = new Consumable
            {
                name = name.AsString(),
                rest_times = d["rest_times"].AsInt32(),
                transformed = d["transformed"].AsBool(),
            };
        }
        return message;
    }
}

public partial class Root : Node, ISubRuntime
{
    public string RuntimeName => "Breakout";

    public void GetMessage(string runtimeName, object message)
    {
        var data = MessagePackSerializer.Deserialize<BreakoutMessage>(
            MessagePackSerializer.ConvertFromJson(message as string)
        );
        var game = GD.Load<PackedScene>("res://breakout/scenes/breakout/breakout.tscn")
            .Instantiate();
        AddChild(game);
        GetNode("/root/BreakoutManager").Call("receive_init_message", data.AsDict());
    }

    public void EndGame(Godot.Collections.Dictionary data)
    {
        GetParent<Runtime>()
            .SwitchRuntime(
                this,
                "AVG",
                MessagePackSerializer.SerializeToJson(BreakoutMessage.FromDict(data))
            );
    }
}
