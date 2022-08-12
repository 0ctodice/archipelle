using Godot;
using static Godot.GD;

public class Player : KinematicBody2D
{
    [Export] private int _GridCellSize = 16;
    [Export] public int _DiggingLimit = 16;
    public bool ShouldMove = false;
    private RayCast2D _BoatCast2D;
    private RayCast2D _ColliderNorth;
    private RayCast2D _ColliderSouth;
    private RayCast2D _ColliderEast;
    private RayCast2D _ColliderWest;
    private RayCast2D _GetOutBoatObstacleCast2D;
    private AnimatedSprite _AnimatedSprite;
    private Area2D _HoleCollider;
    private Texture _PirateTexture;
    private Texture _BoatTexture;
    private StaticBody2D _Boat;
    private PackedScene _Hole;
    private AudioStreamPlayer _StepSound;
    private bool _InBoat = false;
    public int _DiggingCount = 0;

    public override void _Ready()
    {
        _BoatCast2D = GetNode<RayCast2D>("BoatCast2D");
        _GetOutBoatObstacleCast2D = GetNode<RayCast2D>("GetOutBoatObstacleCast2D");
        _AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _HoleCollider = GetNode<Area2D>("Area2D");
        _PirateTexture = Load<Texture>("res://Assets/player.png");
        _BoatTexture = Load<Texture>("res://Assets/boat.png");
        _Hole = Load<PackedScene>("res://Prefabs/Hole.tscn");
        _StepSound = GetTree().Root.GetChild(0).GetNode<Node2D>("SceneManager").GetNode<Node>("SoundPlayer").GetNode<AudioStreamPlayer>("Step");
        var rayCastCollider = GetNode<Node2D>("RayCastCollider");
        _ColliderNorth = rayCastCollider.GetNode<RayCast2D>("North");
        _ColliderSouth = rayCastCollider.GetNode<RayCast2D>("South");
        _ColliderEast = rayCastCollider.GetNode<RayCast2D>("East");
        _ColliderWest = rayCastCollider.GetNode<RayCast2D>("West");
        _AnimatedSprite.Play("Ground");
    }
    public override void _PhysicsProcess(float delta)
    {
        if (ShouldMove)
        {
            var input = GetInput();
            if (input != Vector2.Zero && !_InBoat && _HoleCollider.GetOverlappingBodies().Count == 0)
            {
                Dig(input, _GridCellSize);
            }
            else
            {
                MoveAndCollide(input * _GridCellSize);
            }

            // if (input != Vector2.Zero && !_InBoat && _HoleCollider.GetOverlappingBodies().Count == 0)
            // {
            //     var hole = (StaticBody2D)_Hole.Instance();
            //     hole.Position = Position;
            //     hole.ZIndex = -1;
            //     MoveAndCollide(input * _GridCellSize);
            //     GetParent().AddChild(hole);
            //     _DiggingCount++;
            //     _StepSound.Play();
            // }
            // else
            // {
            //     MoveAndCollide(input * _GridCellSize);
            // }

            if (_BoatCast2D.IsColliding() && Input.IsActionJustPressed("ui_select"))
            {
                if (!_InBoat)
                {
                    _Boat = (StaticBody2D)_BoatCast2D.GetCollider();
                    getInBoat();
                }
                else
                {
                    if (!_GetOutBoatObstacleCast2D.IsColliding()) getOutBoat();
                }
            }

            if (_DiggingCount >= _DiggingLimit)
            {
                ShouldMove = false;
                Name = "DEAD";
            }
        }
    }

    private void Dig(Vector2 input, int value)
    {
        var hole = (StaticBody2D)_Hole.Instance();
        hole.Position = Position;
        hole.ZIndex = -1;
        MoveAndCollide(input * value);
        GetParent().AddChild(hole);
        _DiggingCount++;
        _StepSound.Play();
    }


    private void getInBoat()
    {
        _Boat.GetNode<Sprite>("Sprite").Visible = false;
        SetRayCastColliderCollisionMask(2, false);
        _BoatCast2D.SetCollisionMaskBit(3, false);
        Dig(_BoatCast2D.CastTo, 1);
        _AnimatedSprite.Play("Boat");
        _BoatCast2D.SetCollisionMaskBit(0, true);
        SetRayCastColliderCollisionMask(0, true);
        _InBoat = true;
    }

    private void SetRayCastColliderCollisionMask(int bite, bool value)
    {
        _ColliderNorth.SetCollisionMaskBit(bite, value);
        _ColliderSouth.SetCollisionMaskBit(bite, value);
        _ColliderEast.SetCollisionMaskBit(bite, value);
        _ColliderWest.SetCollisionMaskBit(bite, value);
    }

    private void getOutBoat()
    {
        _Boat.Position = Position;
        _Boat.GetNode<Sprite>("Sprite").Visible = true;
        _BoatCast2D.SetCollisionMaskBit(0, false);
        SetRayCastColliderCollisionMask(0, false);
        MoveAndCollide(_BoatCast2D.CastTo);
        _AnimatedSprite.Play("Ground");
        SetRayCastColliderCollisionMask(2, true);
        _BoatCast2D.SetCollisionMaskBit(3, true);
        _InBoat = false;
    }

    private Vector2 GetInput()
    {
        if (Input.IsActionJustPressed("ui_right"))
        {
            _AnimatedSprite.FlipH = false;
            if (!_ColliderEast.IsColliding()) return new Vector2(1, 0);
        }
        else if (Input.IsActionJustPressed("ui_left"))
        {
            _AnimatedSprite.FlipH = true;
            if (!_ColliderWest.IsColliding()) return new Vector2(-1, 0);
        }
        else if (Input.IsActionJustPressed("ui_down") && !_ColliderSouth.IsColliding()) return new Vector2(0, 1);
        else if (Input.IsActionJustPressed("ui_up") && !_ColliderNorth.IsColliding()) return new Vector2(0, -1);

        return new Vector2(0, 0);
    }
}