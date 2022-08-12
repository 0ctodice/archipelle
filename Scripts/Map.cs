using Godot;
using static Godot.GD;

public class Map : Control
{
    private Tween _Tween;
    private KinematicBody2D _Player;
    private Vector2 _OutPosition;
    private Vector2 _InPosition;
    private Vector2 _CurrentPosition;
    private bool _Visible = false;

    public override void _Ready()
    {
        _OutPosition = GetRect().Position;
        _CurrentPosition = GetRect().Position;
        _InPosition = GetRect().Position + new Vector2(0, -200);
        _Tween = GetNode<Tween>("Tween");
        _Player = GetParent().GetNode<KinematicBody2D>("Player");
        _Visible = false;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("map"))
        {
            ShowMap();
        }
    }

    public void ShowMap()
    {
        var nextPosition = _Visible ? _OutPosition : _InPosition;
        _Tween.InterpolateProperty(this, "rect_position", _CurrentPosition, nextPosition, .25f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Visible = !_Visible;
        _CurrentPosition = nextPosition;
        _Player.Set("ShouldMove", !_Visible);
    }
}