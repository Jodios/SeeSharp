using Godot;
using Godot.Collections;

/**
    Will be saving the data within games memory
    as i don't want to write a file. Don't think the browser would 
    allow that either.
    Refering to: https://docs.godotengine.org/en/stable/tutorials/io/saving_games.html 
**/
public partial class SaveGameUtils : Node
{

    public static Dictionary<string, Dictionary<string, Variant>> Session = new();

    private Global global;

    public override void _Ready()
    {
        global = GetNode<Global>("/root/Global");
    }

    // TERRIBLE :D 
    public void SaveGame()
    {
        Viewport root = GetTree().Root;
        var currentScene = root.GetChild(root.GetChildCount() - 1);
        SceneTree tree = currentScene.GetTree();

        Array nodes = new();
        string parent = "";
        foreach (Node saveNode in tree.GetNodesInGroup(Global.Persist))
        {
            if (string.IsNullOrEmpty(saveNode.SceneFilePath))
            {
                GD.Print($"persistent node '{saveNode.Name}' is not an instanced scene, skipped");
                continue;
            }

            if (!saveNode.HasMethod("Save"))
            {
                GD.Print($"persistent node '{saveNode.Name}' is missing a Save() function, skipped");
                continue;
            }

            Dictionary<string, Variant> nodeData =
                (Dictionary<string, Variant>)saveNode.Call("Save");

            parent = nodeData["Parent"].ToString();
            if (!Session.ContainsKey(parent))
            {
                Session[parent] = new();
            }

            if (saveNode.IsInGroup(Global.Player))
            {
                Session["Player"] = nodeData;
            }
            else
            {
                nodes.Add(nodeData);
            }

        }
        Session[parent]["Nodes"] = nodes;
    }

    public void LoadGame()
    {
        Viewport root = GetTree().Root;
        var currentScene = root.GetChild(root.GetChildCount() - 1);
        SceneTree tree = currentScene.GetTree();
        string parent = currentScene.GetPath();

        if (global.TargetPathToSpawnIn is not null)
        {
            foreach (LittleFella littleFella in tree.GetNodesInGroup(Global.Player))
            {
                Pathway spawnPoint = (Pathway)littleFella.GetParent().GetNode(global.TargetPathToSpawnIn);
                littleFella.Position = spawnPoint.SpawnPoint.Position;
                foreach (var (key, val) in Session["Player"])
                {
                    if (key == "Filename" || key == "Parent" || key == "Position")
                    {
                        continue;
                    }
                    littleFella.Set(key, val);
                }
            }
        }

        if (!Session.ContainsKey(parent)) return;

        // Clearing state
        foreach (Node saveNode in tree.GetNodesInGroup(Global.Persist))
        {
            if (saveNode is LittleFella) continue;
            saveNode.QueueFree();
        }

        Array nodes = (Array)Session[parent]["Nodes"];
        foreach (Dictionary<string, Variant> nodeData in nodes)
        {
            PackedScene newNodeScene = GD.Load<PackedScene>(nodeData["Filename"].ToString());
            Node2D newNode = newNodeScene.Instantiate<Node2D>();
            newNode.Position = (Vector2)nodeData["Position"];
            foreach (var (key, val) in nodeData)
            {
                if (key == "Filename" || key == "Parent" || key == "Position")
                {
                    continue;
                }
                newNode.Set(key, val);
            }
            tree.Root.GetNode(nodeData["Parent"].ToString()).AddChild(newNode);
        }
    }

}