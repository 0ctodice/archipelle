///////////////////////////////////////////////////////////////////////////
//
// ------------------------------------------------------------------------
//   _____      _            _ _
//  |  _  |    | |          | (_)
//  | |/' | ___| |_ ___   __| |_  ___ ___
//  |  /| |/ __| __/ _ \ / _` | |/ __/ _ \
//  \ |_/ / (__| || (_) | (_| | | (_|  __/
//   \___/ \___|\__\___/ \__,_|_|\___\___|
//
// ------------------------------------------------------------------------
//
//  Project for the Weekly Game Jam 264
//  ~ Thomas DUMONT A.K.A 0ctodice
//
// ------------------------------------------------------------------------
//
///////////////////////////////////////////////////////////////////////////
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
