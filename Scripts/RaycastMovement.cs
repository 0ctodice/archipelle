using Godot;
using static Godot.GD;

public class RaycastMovement : RayCast2D
{
    [Export] int _RayCastSize = 16;
    public override void _Process(float delta)
    {
        CastTo = GetInput();
    }

    private Vector2 GetInput()
    {
        if (Input.IsActionJustPressed("ui_down"))
        {
            return Vector2.Down * _RayCastSize;
        }
        else if (Input.IsActionJustPressed("ui_up"))
        {
            return Vector2.Up * _RayCastSize;
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            return Vector2.Right * _RayCastSize;
        }
        else if (Input.IsActionJustPressed("ui_left"))
        {
            return Vector2.Left * _RayCastSize;
        }
        else
        {
            return CastTo;
        }
    }

}