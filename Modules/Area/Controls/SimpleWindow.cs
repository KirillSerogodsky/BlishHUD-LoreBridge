using Blish_HUD;
using Blish_HUD.Controls;
using Glide;
using Microsoft.Xna.Framework;

namespace LoreBridge.Modules.AreaTranslation.Controls;

public abstract class SimpleWindow : Container, IWindow
{
    private readonly Tween _animFade;
    private bool _canClose = true;
    private bool _canCloseWithEscape = true;
    private double _lastWindowInteract;
    private bool _topMost;

    protected SimpleWindow()
    {
        Opacity = 0f;
        Visible = false;

        _zIndex = Screen.WINDOW_BASEZINDEX;

        ClipsBounds = false;

        _animFade = Animation.Tweener.Tween(this, new { Opacity = 1f }, 0.2f).Repeat().Reflect();
        _animFade.Pause();
        _animFade.OnComplete(() =>
        {
            _animFade.Pause();
            if (_opacity <= 0) Visible = false;
        });
    }

    public override int ZIndex
    {
        get => _zIndex + WindowBase2.GetZIndex(this);
        set => SetProperty(ref _zIndex, value);
    }

    public bool CanClose
    {
        get => _canClose;
        set => SetProperty(ref _canClose, value);
    }

    public bool CanCloseWithEscape
    {
        get => _canCloseWithEscape;
        set => SetProperty(ref _canCloseWithEscape, value);
    }

    public bool TopMost
    {
        get => _topMost;
        set => SetProperty(ref _topMost, value);
    }

    public override void Show()
    {
        BringWindowToFront();

        if (Visible) return;

        Location = new Point(MathHelper.Clamp(_location.X, 0, GameService.Graphics.SpriteScreen.Width - 64),
            MathHelper.Clamp(_location.Y, 0, GameService.Graphics.SpriteScreen.Height - 64));

        Opacity = 0;
        Visible = true;

        _animFade.Resume();
    }

    public override void Hide()
    {
        if (!Visible) return;

        _animFade.Resume();
    }

    double IWindow.LastInteraction => _lastWindowInteract;

    public void BringWindowToFront()
    {
        _lastWindowInteract = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds;
    }

    public void ToggleWindow()
    {
        if (Visible)
            Hide();
        else
            Show();
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);
        CalculateWindow();
    }

    private void CalculateWindow()
    {
        ContentRegion = new Rectangle(0, 0, Width, Height);
    }
}