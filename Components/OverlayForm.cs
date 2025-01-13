using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoreBridge.Components;

public sealed class OverlayForm : Form
{
    private readonly Pen _pen = new(Color.White, 2f);
    private Rectangle? _rectangle;

    private Point _startMousePos;
    // public event EventHandler<bool> OnCanceled;

    public OverlayForm()
    {
        BackColor = Color.Black;
        TransparencyKey = Color.Black;
        Opacity = 0.5;
        FormBorderStyle = FormBorderStyle.None;
        Bounds = Screen.PrimaryScreen.Bounds;
        AllowTransparency = true;
        ShowInTaskbar = false;
        TopMost = true;
        Cursor = Cursors.Cross;
        DoubleBuffered = true;

        MouseDown += OnMouseDown;
        // KeyDown += OnKeyDown;
    }

    public event EventHandler<Rectangle> AreaSelected;

    /* private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Reset();
            OnCanceled.Invoke(this, true);
        }
    } */

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        TransparencyKey = Color.Red;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_rectangle != null) e.Graphics.DrawRectangle(_pen, (Rectangle)_rectangle);
        ;

        base.OnPaint(e);
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        _startMousePos = e.Location;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        _rectangle = GetRectangle(_startMousePos, e.Location);
        Invalidate();
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        Reset();
        var rectangle = GetRectangle(_startMousePos, e.Location);
        if (rectangle.Width > 10 && rectangle.Height > 10) AreaSelected?.Invoke(this, rectangle);
        // OnCanceled.Invoke(this, true);
    }

    private void Reset()
    {
        _rectangle = null;
        Invalidate();
        Update();

        MouseMove -= OnMouseMove;
        MouseUp -= OnMouseUp;
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