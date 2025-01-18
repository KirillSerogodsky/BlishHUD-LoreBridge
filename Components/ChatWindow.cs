using System;
using System.Collections.Generic;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Glide;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LoreBridge.Components;

public abstract class ChatWindow : Container, IWindow
{
    private const int TitlebarHeight = 26;
    private const int TitlebarVerticalOffset = 23;
    private const int CornerOffset = 3;
    private const int TitleOffset = 44;
    private const int ContentTopOffset = 2;
    private const int Margin = 1;
    private const int ResizeHandleSize = 16;
    private const int MinWindowWidth = 300;
    private const int MinWindowHeight = 210;
    private const int MaxWindowWidth = 610;
    private const int MaxWindowHeight = 532;

    private readonly Tween _animFade;
    private bool _canClose = true;
    private bool _canCloseWithEscape = true;
    private bool _canResize;
    private bool _dragging;
    private bool _resizing;
    private string _title = "No Title";
    private bool _topMost;

    protected ChatWindow()
    {
        Opacity = 0f;
        Visible = false;

        _zIndex = Screen.WINDOW_BASEZINDEX;

        ClipsBounds = false;

        GameService.Input.Mouse.LeftMouseButtonReleased += OnGlobalMouseRelease;

        // TODO: Use window mask when fading windows in and out instead of this lame opacity transition
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
        get => _zIndex + GetZIndex(this);
        set => SetProperty(ref _zIndex, value);
    }

    /// <summary>
    ///     The text shown at the top of the window.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value, true);
    }

    /// <summary>
    ///     If <c>true</c>, allows the window to be resized by dragging the bottom right corner.
    ///     <br /><br />Default: <c>false</c>
    /// </summary>
    public bool CanResize
    {
        get => _canResize;
        set => SetProperty(ref _canResize, value);
    }

    /// <summary>
    ///     Indicates if the window is actively being dragged.
    /// </summary>
    public bool Dragging
    {
        get => _dragging;
        private set => SetProperty(ref _dragging, value);
    }

    /// <summary>
    ///     Indicates if the window is actively being resized.
    /// </summary>
    public bool Resizing
    {
        get => _resizing;
        private set => SetProperty(ref _resizing, value);
    }

    /// <summary>
    ///     If <c>true</c>, draws an X icon on the window's titlebar and allows the user to close it by pressing it.
    ///     <br /><br />Default: <c>true</c>
    /// </summary>
    public bool CanClose
    {
        get => _canClose;
        set => SetProperty(ref _canClose, value);
    }

    /// <summary>
    ///     If <c>true</c>, the window will close when the user presses the escape key.
    ///     <see cref="CanClose" /> must also be set to <c>true</c>.
    ///     <br /><br />Default: <c>true</c>
    /// </summary>
    public bool CanCloseWithEscape
    {
        get => _canCloseWithEscape;
        set => SetProperty(ref _canCloseWithEscape, value);
    }

    /// <summary>
    ///     If <c>true</c>, this window will show on top of all other windows, regardless of which one had focus last.
    ///     <br /><br />Default: <c>false</c>
    /// </summary>
    public bool TopMost
    {
        get => _topMost;
        set => SetProperty(ref _topMost, value);
    }

    public override void UpdateContainer(GameTime gameTime)
    {
        if (Dragging)
        {
            var nOffset = Input.Mouse.Position - _dragStart;
            Location += nOffset;

            _dragStart = Input.Mouse.Position;
        }
        else if (Resizing)
        {
            var nOffset = Input.Mouse.Position - _dragStart;
            Size = HandleWindowResize(_resizeStart + nOffset);
        }
    }

    protected override void DisposeControl()
    {
        GameService.Input.Mouse.LeftMouseButtonReleased -= OnGlobalMouseRelease;

        base.DisposeControl();
    }

    #region Paint

    public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        PaintWindowBackground(spriteBatch);
        PaintTitleBar(spriteBatch);
    }

    public override void PaintAfterChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        PaintTitleText(spriteBatch);
        PaintExitButton(spriteBatch);
        PaintCorner(spriteBatch);
    }

    private void PaintCorner(SpriteBatch spriteBatch)
    {
        if (CanResize)
            spriteBatch.DrawOnCtrl(this,
                MouseOverResizeHandle || Resizing
                    ? _textureWindowResizableCornerActive
                    : _textureWindowResizableCorner,
                ResizeHandleBounds);
        else
            spriteBatch.DrawOnCtrl(this, _textureWindowCorner, ResizeHandleBounds);
    }

    private void PaintWindowBackground(SpriteBatch spriteBatch)
    {
        var scaledWidth = (int)(BackgroundDestinationBounds.Width * 0.9);
        var scaledHeight = (int)(BackgroundDestinationBounds.Height * 0.9);
        var height = _textureContentBackground.Height - scaledHeight;
        var bottomPoint = height < 0 ? 0 : height;
        spriteBatch.DrawOnCtrl(
            this,
            _textureContentBackground,
            BackgroundDestinationBounds,
            new Rectangle(
                0,
                bottomPoint,
                scaledWidth,
                scaledHeight
            )
        );
    }

    private Rectangle CalculateIntersection(Rectangle rect1, Rectangle rect2)
    {
        var x1 = Math.Max(rect1.X, rect2.X);
        var y1 = Math.Max(rect1.Y, rect2.Y);
        var x2 = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width);
        var y2 = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height);

        if (x2 > x1 && y2 > y1) return new Rectangle(x1, y1, x2 - x1, y2 - y1);

        return Rectangle.Empty;
    }

    private void PaintTitleBar(SpriteBatch spriteBatch)
    {
        if (MouseOver && MouseOverTitleBar)
        {
            // TODO
        }

        // Draw left texture
        var maxOffset = 52;

        var leftTextureOffset = Math.Max(0, maxOffset - (MaxWindowWidth - TitleBarBounds.Width));
        if (leftTextureOffset > 0)
        {
            var destinationRectFirst = new Rectangle(
                _leftTitleBarDrawBounds.X,
                _leftTitleBarDrawBounds.Y,
                leftTextureOffset,
                _textureTitleBarLeft.Height
            );
            var sourceRectFirst = new Rectangle(
                0,
                0,
                leftTextureOffset,
                _textureTitleBarLeft.Height
            );
            spriteBatch.DrawOnCtrl(
                this,
                _textureTitleBarLeft,
                destinationRectFirst,
                sourceRectFirst
            );
        }

        var rightTextureOffset = Math.Max(0, maxOffset - (TitleBarBounds.Width - _textureTitleBarLeft.Width));
        var destinationRect = new Rectangle(
            TitleBarBounds.X + leftTextureOffset,
            _leftTitleBarDrawBounds.Y,
            Math.Min(_textureTitleBarLeft.Width - rightTextureOffset, TitleBarBounds.Width),
            _textureTitleBarLeft.Height
        );
        var sourceRect = new Rectangle(
            rightTextureOffset,
            0,
            destinationRect.Width,
            _textureTitleBarLeft.Height
        );
        spriteBatch.DrawOnCtrl(
            this,
            _textureTitleBarLeft,
            destinationRect,
            sourceRect
        );

        // Draw right corner
        spriteBatch.DrawOnCtrl(
            this,
            _textureTitleBarRight,
            _rightTitleBarDrawBounds,
            null,
            Color.Black,
            MathHelper.ToRadians(270),
            new Vector2(_textureTitleBarRight.Height, 0)
        );

        // Draw divider
        var dividerScaledWidth = (int)(_textureTitleBarDivider.Width * 1.11);
        var dividerScaledHeight = (int)(_textureTitleBarDivider.Height * 1.3);
        var targetRect = new Rectangle(
            0,
            TitleBarBounds.Bottom - dividerScaledHeight + 8,
            dividerScaledWidth,
            dividerScaledHeight
        );
        var clipRect = new Rectangle(0, 0, TitleBarBounds.Width, TitleBarBounds.Height);
        var visibleRect = CalculateIntersection(targetRect, clipRect);
        var scaleX = _textureTitleBarDivider.Width / (float)targetRect.Width;
        var scaleY = _textureTitleBarDivider.Height / (float)targetRect.Height;
        var dividerSourceRect = new Rectangle(
            (int)((visibleRect.X - targetRect.X) * scaleX),
            (int)((visibleRect.Y - targetRect.Y) * scaleY),
            (int)(visibleRect.Width * scaleX),
            (int)(visibleRect.Height * scaleY)
        );
        spriteBatch.DrawOnCtrl(
            this,
            _textureTitleBarDivider,
            visibleRect,
            dividerSourceRect,
            Color.White * 0.6f
        );
    }

    private void PaintTitleText(SpriteBatch spriteBatch)
    {
        if (!string.IsNullOrWhiteSpace(Title))
            spriteBatch.DrawStringOnCtrl(this, Title, Content.DefaultFont16,
                _leftTitleBarDrawBounds.OffsetBy(TitleOffset, 2), Color.White);
    }

    private void PaintExitButton(SpriteBatch spriteBatch)
    {
        if (CanClose)
            spriteBatch.DrawOnCtrl(this, MouseOverExitButton
                    ? TextureExitButtonActive
                    : TextureExitButton,
                ExitButtonBounds);
    }

    #endregion

    #region Load Static

    private static readonly Texture2D TextureExitButton = Content.GetTexture("button-exit");
    private static readonly Texture2D TextureExitButtonActive = Content.GetTexture("button-exit-active");

    #endregion

    #region Textures

    private readonly AsyncTexture2D _textureContentBackground = AsyncTexture2D.FromAssetId(155139);
    private readonly AsyncTexture2D _textureTitleBarLeft = AsyncTexture2D.FromAssetId(155147);
    private readonly AsyncTexture2D _textureTitleBarRight = AsyncTexture2D.FromAssetId(156009);

    private readonly AsyncTexture2D _textureTitleBarDivider = AsyncTexture2D.FromAssetId(156052);

    // private readonly AsyncTexture2D _textureTitleBarLeftActive = AsyncTexture2D.FromAssetId(155147);
    // private readonly AsyncTexture2D _textureTitleBarRightActive = AsyncTexture2D.FromAssetId(156009);
    private readonly AsyncTexture2D _textureWindowCorner = AsyncTexture2D.FromAssetId(156008);
    private readonly AsyncTexture2D _textureWindowResizableCorner = AsyncTexture2D.FromAssetId(156009);
    private readonly AsyncTexture2D _textureWindowResizableCornerActive = AsyncTexture2D.FromAssetId(156010);

    #endregion

    #region Static Window Management

    public static IEnumerable<IWindow> GetWindows()
    {
        return GameService.Graphics.SpriteScreen.GetChildrenOfType<IWindow>();
    }

    /// <summary>
    ///     Returns the calculated zindex offset.  This should be added to the base zindex (typically
    ///     <see cref="Screen.WINDOW_BASEZINDEX" />) and returned as the zindex.
    /// </summary>
    public static int GetZIndex(IWindow thisWindow)
    {
        var windows = GetWindows().ToArray();

        if (!windows.Contains(thisWindow))
            throw new InvalidOperationException(
                $"{nameof(thisWindow)} must be a direct child of GameService.Graphics.SpriteScreen before ZIndex can automatically be calculated.");

        return Screen.WINDOW_BASEZINDEX + windows.OrderBy(window => window.TopMost)
            .ThenBy(window => window.LastInteraction)
            .TakeWhile(window => window != thisWindow)
            .Count();
    }

    /// <summary>
    ///     Gets or sets the active window. Returns null if no window is visible.
    /// </summary>
    public static IWindow ActiveWindow
    {
        get => GetWindows().Where(w => w.Visible).OrderByDescending(GetZIndex).FirstOrDefault();
        set => value.BringWindowToFront();
    }

    #endregion

    #region Show & Hide

    /// <summary>
    ///     Shows the window if it is hidden.
    ///     Hides the window if it is currently showing.
    /// </summary>
    public void ToggleWindow()
    {
        if (Visible)
            Hide();
        else
            Show();
    }

    /// <summary>
    ///     Shows the window.
    /// </summary>
    public override void Show()
    {
        BringWindowToFront();

        if (Visible)
            return;

        Opacity = 0.0f;
        Visible = true;
        _animFade.Resume();
    }

    /// <summary>
    ///     Hides the window.
    /// </summary>
    public override void Hide()
    {
        if (!Visible) return;

        Dragging = false;
        _animFade.Resume();
        Content.PlaySoundEffectByName(@"window-close");
    }

    #endregion

    #region Implementation Properties

    private bool _showSideBar;

    protected bool ShowSideBar
    {
        get => _showSideBar;
        set => SetProperty(ref _showSideBar, value);
    }

    private int _sideBarHeight = 100;

    protected int SideBarHeight
    {
        get => _sideBarHeight;
        set => SetProperty(ref _sideBarHeight, value, true);
    }

    private double _lastWindowInteract;
    double IWindow.LastInteraction => _lastWindowInteract;

    #endregion

    #region Window Regions

    // Mouse regions

    protected Rectangle TitleBarBounds { get; private set; } = Rectangle.Empty;
    protected Rectangle ExitButtonBounds { get; private set; } = Rectangle.Empty;
    protected Rectangle ResizeHandleBounds { get; private set; } = Rectangle.Empty;
    protected Rectangle BackgroundDestinationBounds { get; private set; } = Rectangle.Empty;

    // Draw regions

    private Rectangle _leftTitleBarDrawBounds = Rectangle.Empty;
    private Rectangle _rightTitleBarDrawBounds = Rectangle.Empty;

    public override void RecalculateLayout()
    {
        // Title bar bounds
        _rightTitleBarDrawBounds = new Rectangle(
            TitleBarBounds.Width - _textureTitleBarRight.Width + CornerOffset * 2 - 1,
            TitleBarBounds.Y - CornerOffset,
            _textureTitleBarRight.Width,
            _textureTitleBarRight.Height);

        // The left bar could end up too long, so we shrink its width down some to avoid drawing too far into the right titlebar
        _leftTitleBarDrawBounds = new Rectangle(TitleBarBounds.Location.X,
            TitleBarBounds.Location.Y - TitlebarVerticalOffset,
            _textureTitleBarLeft.Width,
            _textureTitleBarLeft.Height);

        // Exit button bounds
        ExitButtonBounds = new Rectangle(
            _rightTitleBarDrawBounds.Right - Margin * 2 - TextureExitButton.Width,
            _rightTitleBarDrawBounds.Y,
            TextureExitButton.Width,
            TextureExitButton.Height);

        // Corner bounds
        ResizeHandleBounds = new Rectangle(Width - _textureWindowCorner.Width + CornerOffset,
            Height - _textureWindowCorner.Height + CornerOffset * 2,
            _textureWindowCorner.Width,
            _textureWindowCorner.Height);
    }

    #endregion

    #region Window States

    protected bool MouseOverTitleBar { get; private set; }
    protected bool MouseOverExitButton { get; private set; }
    protected bool MouseOverResizeHandle { get; private set; }

    private Point _dragStart = Point.Zero;
    private Point _resizeStart = Point.Zero;

    protected override void OnMouseMoved(MouseEventArgs e)
    {
        ResetMouseRegionStates();

        if (RelativeMousePosition.Y < TitleBarBounds.Bottom)
        {
            if (ExitButtonBounds.Contains(RelativeMousePosition))
                MouseOverExitButton = true;
            else
                MouseOverTitleBar = true;
        }
        else if (_canResize
                 && ResizeHandleBounds.Contains(RelativeMousePosition)
                 && RelativeMousePosition.X > ResizeHandleBounds.Right - ResizeHandleSize
                 && RelativeMousePosition.Y > ResizeHandleBounds.Bottom - ResizeHandleSize)
        {
            MouseOverResizeHandle = true;
        }

        base.OnMouseMoved(e);
    }

    private void OnGlobalMouseRelease(object sender, MouseEventArgs e)
    {
        if (Visible)
        {
            Dragging = false;
            Resizing = false;
        }
    }

    protected override void OnMouseLeft(MouseEventArgs e)
    {
        ResetMouseRegionStates();
        base.OnMouseLeft(e);
    }

    protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
    {
        BringWindowToFront();

        if (MouseOverTitleBar)
        {
            Dragging = true;
            _dragStart = Input.Mouse.Position;
        }
        else if (MouseOverResizeHandle)
        {
            Resizing = true;
            _resizeStart = Size;
            _dragStart = Input.Mouse.Position;
        }
        else if (MouseOverExitButton && CanClose)
        {
            Hide();
        }

        base.OnLeftMouseButtonPressed(e);
    }

    private void ResetMouseRegionStates()
    {
        MouseOverTitleBar = false;
        MouseOverExitButton = false;
        MouseOverResizeHandle = false;
    }

    /// <summary>
    ///     Modifies the window size as it's being resized.
    ///     Override to lock the window size at specific intervals or implement other resize behaviors.
    /// </summary>
    protected virtual Point HandleWindowResize(Point newSize)
    {
        return new Point(
            MathHelper.Clamp(newSize.X, MinWindowWidth, MaxWindowWidth),
            MathHelper.Clamp(newSize.Y, MinWindowHeight, MaxWindowHeight));
    }

    public void BringWindowToFront()
    {
        _lastWindowInteract = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds;
    }

    #endregion

    #region Window Construction

    protected override void OnResized(ResizedEventArgs e)
    {
        CalculateWindow();

        base.OnResized(e);
    }

    private void CalculateWindow()
    {
        ContentRegion = new Rectangle(
            0,
            TitlebarHeight + ContentTopOffset,
            Width,
            Height - TitlebarHeight - ContentTopOffset);

        TitleBarBounds = new Rectangle(0, 0, Size.X, TitlebarHeight + 2);

        var drawWidth = ContentRegion.Width + ContentRegion.X;
        var drawHeight = ContentRegion.Height + ContentRegion.Y - TitlebarHeight;
        BackgroundDestinationBounds = new Rectangle(0,
            TitlebarHeight,
            drawWidth,
            drawHeight);
    }

    #endregion
}