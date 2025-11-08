using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoreBridge.Modules.Area.Forms;

public sealed class OverlayForm : Form
{
    private readonly Pen _pen = new(Color.White, 2f);
    private Rectangle? _rectangle;
    private Point _startMousePos;
    private readonly Timer _fadeInTimer;
    private readonly Timer _fadeOutTimer;
    private double _currentOpacity;
    private readonly double _targetOpacity = 0.5;

    public OverlayForm()
    {
        BackColor = Color.Black;
        TransparencyKey = Color.Black;
        FormBorderStyle = FormBorderStyle.None;
        Bounds = Screen.PrimaryScreen.Bounds;
        AllowTransparency = true;
        ShowInTaskbar = false;
        TopMost = true;
        Cursor = Cursors.Cross;
        DoubleBuffered = true;
        Opacity = 0;

        _fadeInTimer = new Timer();
        _fadeInTimer.Interval = 16;
        _fadeInTimer.Tick += OnFadeInTick;

        _fadeOutTimer = new Timer();
        _fadeOutTimer.Interval = 16;
        _fadeOutTimer.Tick += OnFadeOutTick;

        MouseDown += OnMouseDown;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x08000000;
            return cp;
        }
    }

    public event EventHandler<Rectangle> AreaSelected;
    public event EventHandler<bool> Hidden;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        TransparencyKey = Color.Red;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        StartFadeIn();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fadeInTimer?.Stop();
            _fadeInTimer?.Dispose();
            _fadeOutTimer?.Stop();
            _fadeOutTimer?.Dispose();
            _pen?.Dispose();
            MouseDown -= OnMouseDown;
            MouseMove -= OnMouseMove;
            MouseUp -= OnMouseUp;
        }

        base.Dispose(disposing);
    }

    private new void Hide()
    {
        Hidden?.Invoke(this, true);
        base.Hide();
    }

    private void StartFadeIn()
    {
        _fadeInTimer.Start();
    }

    private void OnFadeInTick(object sender, EventArgs e)
    {
        _currentOpacity += 0.15;

        if (_currentOpacity >= _targetOpacity)
        {
            _currentOpacity = _targetOpacity;
            _fadeInTimer.Stop();
        }

        Opacity = _currentOpacity;
    }

    private void StartFadeOut()
    {
        _fadeInTimer.Stop();
        _currentOpacity = Opacity;
        _fadeOutTimer.Start();
    }

    private void OnFadeOutTick(object sender, EventArgs e)
    {
        _currentOpacity -= 0.15;

        if (_currentOpacity <= 0)
        {
            _currentOpacity = 0;
            _fadeOutTimer.Stop();
            Hide();
        }

        Opacity = _currentOpacity;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_rectangle != null) e.Graphics.DrawRectangle(_pen, (Rectangle)_rectangle);
        base.OnPaint(e);
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
                _startMousePos = e.Location;
                MouseMove += OnMouseMove;
                MouseUp += OnMouseUp;
                break;
            case MouseButtons.Right:
                StartFadeOut();
                break;
            case MouseButtons.None:
            case MouseButtons.Middle:
            case MouseButtons.XButton1:
            case MouseButtons.XButton2:
            default:
                return;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        _rectangle = GetRectangle(_startMousePos, e.Location);
        Invalidate();
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        StartFadeOut();

        if (e.Button != MouseButtons.Left) return;

        var rectangle = GetRectangle(_startMousePos, e.Location);
        if (rectangle is { Width: > 10, Height: > 10 }) AreaSelected?.Invoke(this, rectangle);
    }

    private static Rectangle GetRectangle(Point point1, Point point2)
    {
        return new Rectangle(
            Math.Min(point1.X, point2.X),
            Math.Min(point1.Y, point2.Y),
            Math.Abs(point1.X - point2.X),
            Math.Abs(point1.Y - point2.Y)
        );
    }
}