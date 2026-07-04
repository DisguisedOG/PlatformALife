using Godot;
using System;

public static class AssetValidator
{
    public static bool IsMultipleOfGrid(float value, float gridSize)
    {
        return Mathf.IsZeroApprox(value % gridSize);
    }

    public static void ValidateWorldPosition(Node2D node, float gridSize = 32.0f)
    {
        if (node == null)
        {
            return;
        }

        if (!IsMultipleOfGrid(node.GlobalPosition.X, gridSize) || !IsMultipleOfGrid(node.GlobalPosition.Y, gridSize))
        {
            GD.PrintErr($"{node.Name} global position is not tile-aligned: {node.GlobalPosition}. Expected multiples of {gridSize}.");
        }
    }

    public static void ValidatePortal(Portal portal)
    {
        if (portal == null)
        {
            return;
        }

        var shapeNode = portal.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        if (shapeNode == null)
        {
            GD.PrintErr("Portal asset missing CollisionShape2D.");
            return;
        }

        if (shapeNode.Shape is RectangleShape2D rect)
        {
            float width = rect.Size.X;
            float height = rect.Size.Y;
            if (!IsMultipleOfGrid(width, 32.0f) || !IsMultipleOfGrid(height, 32.0f))
            {
                GD.PrintErr($"Portal collider is not grid-aligned: {width}x{height}. Expected multiples of 32.");
            }
        }
        else
        {
            GD.PrintErr("Portal collision shape is not a RectangleShape2D.");
        }

        ValidateWorldPosition(portal);
    }
}
