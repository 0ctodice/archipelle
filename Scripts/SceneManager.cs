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
using System.Threading.Tasks;

public class SceneManager : Node2D
{
    private enum FADESTATE
    {
        IN,
        OUT,
        MAP,
        NXT,
        GO,
        CUR,
        WAIT
    }
    [Export] private string[] _Scenes;
    private Node2D _CurrentScene;
    private Node2D _CurrentSceneNode;
    private Area2D _CurrentChest;
    private Control _CurrentMap;
    private Node2D _SceneManager;
    private ColorRect _ColorRect;
    private KinematicBody2D _Player;
    private AudioStreamPlayer _ChestSound;
    private AudioStreamPlayer _GameOverSound;
    private AudioStreamPlayer _Music;
    private Tween _Tween;
    private Timer _Timer;
    private FADESTATE _State = FADESTATE.OUT;
    private int _currentSceneID = 0;
    public override void _Ready()
    {
        _CurrentSceneNode = GetNode<Node2D>("CurrentScene");
        _SceneManager = GetNode<Node2D>("SceneManager");
        _ColorRect = _SceneManager.GetNode<Control>("Control").GetNode<ColorRect>("ColorRect");
        _Tween = _SceneManager.GetNode<Tween>("Tween");
        _ChestSound = _SceneManager.GetNode<Node>("SoundPlayer").GetNode<AudioStreamPlayer>("Chest");
        _GameOverSound = _SceneManager.GetNode<Node>("SoundPlayer").GetNode<AudioStreamPlayer>("GameOver");
        _Music = _SceneManager.GetNode<Node>("SoundPlayer").GetNode<AudioStreamPlayer>("Music");
        _Timer = _SceneManager.GetNode<Timer>("Timer");
        _Timer.Connect("timeout", this, "Timeout");
        LoadFirstScene();
    }

    private void Timeout()
    {
        _Timer.Stop();
        switch (_State)
        {
            case FADESTATE.OUT:
                FadeOut();
                break;
            case FADESTATE.IN:
                FadeIn();
                break;
            case FADESTATE.MAP:
                _State = FADESTATE.WAIT;
                _CurrentMap.Call("ShowMap");
                break;
            case FADESTATE.NXT:
                _State = FADESTATE.OUT;
                LoadAScene((_currentSceneID + 1) % _Scenes.Length);
                break;
            case FADESTATE.GO:
                FadeInGameOver();
                break;
            case FADESTATE.CUR:
                _State = FADESTATE.OUT;
                LoadAScene(_currentSceneID);
                break;
            case FADESTATE.WAIT:
                _State = FADESTATE.IN;
                _Timer.Start(1f);
                break;
        }
    }

    private void LoadFirstScene()
    {
        _CurrentScene = (Node2D)(Load<PackedScene>(_Scenes[_currentSceneID])).Instance();
        _CurrentSceneNode.AddChild(_CurrentScene);
        _CurrentChest = _CurrentScene.GetNode<Area2D>("Chest");
        _Player = _CurrentScene.GetNode<KinematicBody2D>("Player");
        _CurrentMap = _CurrentScene.GetNode<Control>("MapInterface");
        _CurrentChest.Connect("body_exited", this, "OnPlayerFind");
        _Player.Connect("renamed", this, "OnGameOver");
        Timeout();
    }
    private void LoadAScene(int index)
    {
        if (index != 10)
        {
            _CurrentScene.QueueFree();
            _CurrentSceneNode.GetChild(0).QueueFree();
            _currentSceneID = index;
            _CurrentScene = (Node2D)(Load<PackedScene>(_Scenes[_currentSceneID])).Instance();
            _CurrentSceneNode.AddChild(_CurrentScene);
            _CurrentChest = _CurrentScene.GetNode<Area2D>("Chest");
            _Player = _CurrentScene.GetNode<KinematicBody2D>("Player");
            _CurrentMap = _CurrentScene.GetNode<Control>("MapInterface");
            _CurrentChest.Connect("body_exited", this, "OnPlayerFind");
            _Player.Connect("renamed", this, "OnGameOver");
        }
        else
        {
            _currentSceneID = index;
            _CurrentScene = (Node2D)(Load<PackedScene>(_Scenes[_currentSceneID])).Instance();
            _CurrentSceneNode.AddChild(_CurrentScene);
        }

        Timeout();
    }

    public void FadeIn()
    {
        _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 0), Color.Color8(42, 42, 42, 255), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Timer.Start(1f);
        _State = FADESTATE.NXT;
    }

    public void FadeInGameOver()
    {
        _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 0), Color.Color8(42, 42, 42, 255), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Timer.Start(1f);
        _State = FADESTATE.CUR;
    }

    public void FadeOut()
    {
        _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 255), Color.Color8(42, 42, 42, 0), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Timer.Start(1f);
        _State = FADESTATE.MAP;
    }

    public void OnPlayerFind(Node body)
    {
        _CurrentChest.Visible = true;
        _ChestSound.Play();
        _Player.Set("ShouldMove", false);
        Timeout();
    }

    public void OnGameOver()
    {
        if (!_CurrentChest.Visible)
        {
            _State = FADESTATE.GO;
            _GameOverSound.Play();
            Timeout();
        }
    }

    // private async void LoadFirstScene()
    // {
    //     _CurrentScene = (Node2D)(Load<PackedScene>(_Scenes[_currentSceneID])).Instance();
    //     _CurrentSceneNode.AddChild(_CurrentScene);
    //     _CurrentChest = _CurrentScene.GetNode<Area2D>("Chest");
    //     _Player = _CurrentScene.GetNode<KinematicBody2D>("Player");
    //     _CurrentMap = _CurrentScene.GetNode<Control>("MapInterface");
    //     _CurrentChest.Connect("body_exited", this, "OnPlayerFind");
    //     _Player.Connect("renamed", this, "OnGameOver");
    //     await FadeOut();
    //     _CurrentMap.Call("ShowMap");
    // }
    // private async void LoadAScene(int index)
    // {
    //     await FadeIn();
    //     _CurrentScene.QueueFree();
    //     _CurrentSceneNode.GetChild(0).QueueFree();
    //     _currentSceneID = index;
    //     _CurrentScene = (Node2D)(Load<PackedScene>(_Scenes[_currentSceneID])).Instance();
    //     _CurrentSceneNode.AddChild(_CurrentScene);
    //     _CurrentChest = _CurrentScene.GetNode<Area2D>("Chest");
    //     _Player = _CurrentScene.GetNode<KinematicBody2D>("Player");
    //     _CurrentMap = _CurrentScene.GetNode<Control>("MapInterface");
    //     _CurrentChest.Connect("body_exited", this, "OnPlayerFind");
    //     _Player.Connect("renamed", this, "OnGameOver");
    //     await FadeOut();
    //     _CurrentMap.Call("ShowMap");
    // }

    // public async Task FadeIn()
    // {
    //     await Task.Delay(1000);
    //     _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 0), Color.Color8(42, 42, 42, 255), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
    //     _Tween.Start();
    //     await Task.Delay(1000);
    // }

    // public async Task FadeOut()
    // {
    //     await Task.Delay(1000);
    //     _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 255), Color.Color8(42, 42, 42, 0), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
    //     _Tween.Start();
    //     await Task.Delay(1000);
    // }

    // public async Task PlayChestSound()
    // {
    //     _ChestSound.Play();
    //     await Task.Delay(1000);
    // }

    // public async Task PlayGameOverSound()
    // {
    //     _GameOverSound.Play();
    //     await Task.Delay(1000);
    // }

    // public async void OnPlayerFind(Node body)
    // {
    //     _CurrentChest.Visible = true;
    //     _Music.StreamPaused = true;
    //     await PlayChestSound();
    //     _Music.StreamPaused = false;
    //     _Player.Set("ShouldMove", false);
    //     LoadAScene((_currentSceneID + 1) % _Scenes.Length);
    // }

    // public async void OnGameOver()
    // {
    //     if (!_CurrentChest.Visible)
    //     {
    //         _Music.StreamPaused = true;
    //         await PlayGameOverSound();
    //         _Music.StreamPaused = false;
    //         LoadAScene(_currentSceneID);
    //     }
    // }
}
