using System;
using System.Windows.Forms;
using Blish_HUD;
using Blish_HUD.Controls;
using Glide;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Control = Blish_HUD.Controls.Control;
using MouseEventArgs = Blish_HUD.Input.MouseEventArgs;
using Panel = Blish_HUD.Controls.Panel;

namespace LoreBridge.Components;

public class Scrollbar2 : Control
{
    private const int CONTROL_WIDTH = 12;
    private const int MIN_LENGTH = 32;
    private const int CAP_SLACK = 6;
    private const int SCROLL_ARROW = 50;
    private const int SCROLL_CONT_ARROW = 10;
    private const int SCROLL_CONT_TRACK = 15;
    private const int SCROLL_WHEEL = 30;

    private Container _associatedContainer;
    private Rectangle _barBounds;

    private int _containerLowestContent;
    private Rectangle _downArrowBounds;

    private double _lastClickTime;

    private int _scrollbarHeight = MIN_LENGTH;

    private double _scrollbarPercent = 1.0;

    private float _scrollDistance;

    private ClickFocus _scrollFocus;

    private int _scrollingOffset;

    private float _targetScrollDistance;

    private Tween _targetScrollDistanceAnim;
    private Rectangle _trackBounds;

    private Rectangle _upArrowBounds;

    public Scrollbar2(Container container)
    {
        _associatedContainer = container;

        _upArrowBounds = Rectangle.Empty;
        _downArrowBounds = Rectangle.Empty;
        _barBounds = Rectangle.Empty;
        _trackBounds = Rectangle.Empty;

        Width = CONTROL_WIDTH;

        Input.Mouse.LeftMouseButtonReleased += MouseOnLeftMouseButtonReleased;
        _associatedContainer.MouseWheelScrolled += HandleWheelScroll;
    }

    private ClickFocus ScrollFocus
    {
        get => _scrollFocus;
        set
        {
            _scrollFocus = value;
            HandleClickScroll(true);
        }
    }

    private float TargetScrollDistance
    {
        get
        {
            if (_targetScrollDistanceAnim == null) return _scrollDistance;

            return _targetScrollDistance;
        }
        set
        {
            var aVal = MathHelper.Clamp(value, 0f, 1f);
            if (_associatedContainer != null && _targetScrollDistance != aVal) _targetScrollDistance = aVal;
        }
    }

    public float ScrollDistance
    {
        get => _scrollDistance;
        set
        {
            if (SetProperty(ref _scrollDistance, MathHelper.Clamp(value, 0f, 1f), true))
                _targetScrollDistance = _scrollDistance;

            UpdateAssocContainer();
        }
    }

    private int ScrollbarHeight
    {
        get => _scrollbarHeight;
        set
        {
            if (!SetProperty(ref _scrollbarHeight, value, true)) return;

            // Reclamps the scrolling content
            RecalculateScrollbarSize();
            UpdateAssocContainer();
        }
    }

    public Container AssociatedContainer
    {
        get => _associatedContainer;
        set => SetProperty(ref _associatedContainer, value);
    }

    private int _containerContentDiff => _containerLowestContent - _associatedContainer.ContentRegion.Height;
    private int TrackLength => _size.Y - _textureUpArrow.Height - _textureDownArrow.Height;

    protected override void DisposeControl()
    {
        base.DisposeControl();

        Input.Mouse.LeftMouseButtonReleased -= MouseOnLeftMouseButtonReleased;
        _associatedContainer.MouseWheelScrolled -= HandleWheelScroll;
    }

    protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
    {
        base.OnLeftMouseButtonPressed(e);

        ScrollFocus = GetScrollFocus(Input.Mouse.Position - AbsoluteBounds.Location);
        _lastClickTime = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds;
    }

    private void MouseOnLeftMouseButtonReleased(object sender, MouseEventArgs e)
    {
        ScrollFocus = ClickFocus.None;
    }

    protected override void OnMouseWheelScrolled(MouseEventArgs e)
    {
        HandleWheelScroll(this, e);

        base.OnMouseWheelScrolled(e);
    }

    private void HandleWheelScroll(object sender, MouseEventArgs e)
    {
        // Don't scroll if the scrollbar isn't visible
        if (!Visible || _scrollbarPercent > 0.99) return;

        // Avoid scrolling nested panels
        var ctrl = (Control)sender;
        while (ctrl != _associatedContainer && ctrl != null)
        {
            if (ctrl is Panel) return;
            ctrl = ctrl.Parent;
        }

        if (GameService.Input.Mouse.State.ScrollWheelValue == 0) return;

        float normalScroll = Math.Sign(GameService.Input.Mouse.State.ScrollWheelValue);
        ScrollAnimated((int)normalScroll * -SCROLL_WHEEL * SystemInformation.MouseWheelScrollLines);
    }

    private ClickFocus GetScrollFocus(Point mousePos)
    {
        return mousePos switch
        {
            var point when _trackBounds.Contains(point) && !_barBounds.Contains(point) && _barBounds.Y < point.Y =>
                ClickFocus.AboveBar,
            var point when _trackBounds.Contains(point) && !_barBounds.Contains(point) && _barBounds.Y > point.Y =>
                ClickFocus.BelowBar,
            var point when _barBounds.Contains(point) => ClickFocus.Bar,
            var point when _upArrowBounds.Contains(point) => ClickFocus.UpArrow,
            var point when _downArrowBounds.Contains(point) => ClickFocus.DownArrow,
            _ => ClickFocus.None
        };
    }

    private void HandleClickScroll(bool clicked)
    {
        Action<int> scroll = pixels =>
            ScrollDistance = (_containerContentDiff * ScrollDistance + pixels) / _containerContentDiff;
        Func<bool, Action<int>> getScrollAction = c => c ? ScrollAnimated : scroll;

        var relMousePos = Input.Mouse.Position - AbsoluteBounds.Location;

        if (ScrollFocus == ClickFocus.None)
        {
        }
        else if (ScrollFocus == ClickFocus.BelowBar)
        {
            if (GetScrollFocus(relMousePos) == ClickFocus.BelowBar)
                getScrollAction(clicked)(clicked ? -ScrollbarHeight : -SCROLL_CONT_TRACK);
        }
        else if (ScrollFocus == ClickFocus.AboveBar)
        {
            if (GetScrollFocus(relMousePos) == ClickFocus.AboveBar)
                getScrollAction(clicked)(clicked ? ScrollbarHeight : SCROLL_CONT_TRACK);
        }
        else if (ScrollFocus == ClickFocus.UpArrow)
        {
            getScrollAction(clicked)(clicked ? -SCROLL_ARROW : -SCROLL_CONT_ARROW);
        }
        else if (ScrollFocus == ClickFocus.DownArrow)
        {
            getScrollAction(clicked)(clicked ? SCROLL_ARROW : SCROLL_CONT_ARROW);
        }
        else if (ScrollFocus == ClickFocus.Bar)
        {
            if (clicked)
                _scrollingOffset = relMousePos.Y - _barBounds.Y;

            relMousePos = relMousePos - new Point(0, _scrollingOffset) - _trackBounds.Location;
            ScrollDistance = relMousePos.Y / (float)(TrackLength - ScrollbarHeight);
            TargetScrollDistance = ScrollDistance;
        }
    }

    private void ScrollAnimated(int pixels)
    {
        TargetScrollDistance = (_containerContentDiff * ScrollDistance + pixels) / _containerContentDiff;
        _targetScrollDistanceAnim = Animation.Tweener
            .Tween(this, new { ScrollDistance = TargetScrollDistance }, 0f, overwrite: true).Ease(Ease.QuadOut);
    }

    protected override CaptureType CapturesInput()
    {
        return CaptureType.Mouse | CaptureType.MouseWheel;
    }

    private void UpdateAssocContainer()
    {
        AssociatedContainer.VerticalScrollOffset =
            (int)Math.Floor((_containerLowestContent - AssociatedContainer.ContentRegion.Height) * ScrollDistance);
    }

    public override void DoUpdate(GameTime gameTime)
    {
        base.DoUpdate(gameTime);

        var timeDiff = gameTime.TotalGameTime.TotalMilliseconds - _lastClickTime;

        if (ScrollFocus == ClickFocus.Bar)
            HandleClickScroll(false);
        else if (timeDiff > 200)
            HandleClickScroll(false);

        Invalidate();
    }

    public override void RecalculateLayout()
    {
        RecalculateScrollbarSize();

        _upArrowBounds = new Rectangle(Width / 2 - _textureUpArrow.Width / 2, 0, _textureUpArrow.Width,
            _textureUpArrow.Height);
        _downArrowBounds = new Rectangle(Width / 2 - _textureDownArrow.Width / 2, Height - _textureDownArrow.Height,
            _textureDownArrow.Width, _textureDownArrow.Height);
        _barBounds = new Rectangle(Width / 2 - _textureBar.Width / 2,
            (int)(ScrollDistance * (TrackLength - ScrollbarHeight)) + _textureUpArrow.Height, _textureBar.Width,
            ScrollbarHeight);
        _trackBounds = new Rectangle(Width / 2 - _textureTrack.Width / 2, _upArrowBounds.Bottom, _textureTrack.Width,
            TrackLength);
    }

    private void RecalculateScrollbarSize()
    {
        if (_associatedContainer == null) return;

        var tempContainerChidlren = _associatedContainer.Children.ToArray();

        _containerLowestContent = 0;

        for (var i = 0; i < tempContainerChidlren.Length; i++)
        {
            ref var child = ref tempContainerChidlren[i];

            if (child.Visible) _containerLowestContent = Math.Max(_containerLowestContent, child.Bottom);
        }

        _containerLowestContent = Math.Max(_containerLowestContent, _associatedContainer.ContentRegion.Height);

        _scrollbarPercent = _associatedContainer.ContentRegion.Height / (double)_containerLowestContent;

        ScrollbarHeight = (int)Math.Max(Math.Floor(TrackLength * _scrollbarPercent) - 1, MIN_LENGTH);

        UpdateAssocContainer();
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        // Don't show the scrollbar if there is nothing to scroll
        if (_scrollbarPercent > 0.99) return;
        
        spriteBatch.DrawOnCtrl(this, _textureTrack, _trackBounds);

        var drawTint = (ScrollFocus == ClickFocus.None && MouseOver) ||
                       (_associatedContainer != null && _associatedContainer.MouseOver)
            ? Color.White
            : ContentService.Colors.Darkened(0.6f);

        drawTint = ScrollFocus != ClickFocus.None
            ? ContentService.Colors.Darkened(0.9f)
            : drawTint;

        spriteBatch.DrawOnCtrl(this, _textureUpArrow, _upArrowBounds, drawTint);
        spriteBatch.DrawOnCtrl(this, _textureDownArrow, _downArrowBounds, drawTint);

        spriteBatch.DrawOnCtrl(this, _textureBar, _barBounds, drawTint);
        spriteBatch.DrawOnCtrl(this, _textureTopCap,
            new Rectangle(Width / 2 - _textureTopCap.Width / 2, _barBounds.Top - CAP_SLACK, _textureTopCap.Width,
                _textureTopCap.Height));
        spriteBatch.DrawOnCtrl(this, _textureBottomCap,
            new Rectangle(Width / 2 - _textureBottomCap.Width / 2,
                _barBounds.Bottom - _textureBottomCap.Height + CAP_SLACK, _textureBottomCap.Width,
                _textureBottomCap.Height));
        spriteBatch.DrawOnCtrl(this, _textureThumb,
            new Rectangle(Width / 2 - _textureThumb.Width / 2,
                _barBounds.Top + (ScrollbarHeight / 2 - _textureThumb.Height / 2), _textureThumb.Width,
                _textureThumb.Height), drawTint);
    }

    private enum ClickFocus
    {
        None,
        UpArrow,
        DownArrow,
        AboveBar,
        BelowBar,
        Bar
    }

    #region Load Static

    private static readonly TextureRegion2D _textureTrack =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-track");

    private static readonly TextureRegion2D _textureUpArrow =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-arrow-up");

    private static readonly TextureRegion2D _textureDownArrow =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-arrow-down");

    private static readonly TextureRegion2D _textureBar =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-bar-active");

    private static readonly TextureRegion2D _textureThumb =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-thumb");

    private static readonly TextureRegion2D _textureTopCap =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-cap-top");

    private static readonly TextureRegion2D _textureBottomCap =
        Blish_HUD.Controls.Resources.Control.TextureAtlasControl.GetRegion("scrollbar/sb-cap-bottom");

    #endregion
}