using Godot;
using static Godot.GD;
using System.Threading.Tasks;

public class SplashScreen : Node2D
{
    private Sprite _Logo;
    private Tween _Tween;
    private ColorRect _ColorRect;
    private Timer _Timer;
    private enum FADESTATE
    {
        IN,
        OUT,
        NXT
    }
    private FADESTATE _State = FADESTATE.OUT;

    public override void _Ready()
    {
        _Logo = GetNode<Sprite>("Logo");
        _Tween = GetNode<Tween>("Tween");
        _ColorRect = GetNode<Control>("Control").GetNode<ColorRect>("ColorRect");
        _Timer = GetNode<Timer>("Timer");
        _Timer.Connect("timeout", this, "LoadScenes");
        LoadScenes();
    }

    public void LoadScenes()
    {
        switch (_State)
        {
            case FADESTATE.OUT:
                FadeOut();
                break;
            case FADESTATE.IN:
                FadeIn();
                break;
            case FADESTATE.NXT:
                GetTree().ChangeScene("res://Scenes/Scenes.tscn");
                break;
        }
    }

    public void FadeIn()
    {
        _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 0), Color.Color8(42, 42, 42, 255), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Timer.Start(1f);
        _State = FADESTATE.NXT;
    }

    public void FadeOut()
    {
        _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 255), Color.Color8(42, 42, 42, 0), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Tween.InterpolateProperty(_Logo, "scale", _Logo.Scale, _Logo.Scale * 1.3f, 5f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
        _Tween.Start();
        _Timer.Start(2f);
        _State = FADESTATE.IN;
    }

    // public async void LoadScenes()
    // {
    //     await FadeOut();
    //     await FadeIn();
    //     GetTree().ChangeScene("res://Scenes/Scenes.tscn");
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
    //     _Tween.InterpolateProperty(_ColorRect, "color", Color.Color8(42, 42, 42, 255), Color.Color8(42, 42, 42, 0), 1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
    //     _Tween.Start();
    //     _Tween.InterpolateProperty(_Logo, "scale", _Logo.Scale, _Logo.Scale * 1.3f, 5f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
    //     _Tween.Start();
    //     await Task.Delay(1000);
    // }
}