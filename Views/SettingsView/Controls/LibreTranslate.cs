using Blish_HUD.Controls;
using LoreBridge.Models;
using LoreBridge.Translation.Translators;
using Microsoft.Xna.Framework;

namespace LoreBridge.Views.SettingsView.Controls;

public sealed class LibreTranslate : FlowPanel
{
    public LibreTranslate(Panel mainPanel, Settings settings)
    {
        Parent = mainPanel;
        Title = "LibreTranslate";
        CanCollapse = true;
        WidthSizingMode = SizingMode.Fill;
        HeightSizingMode = SizingMode.AutoSize;
        FlowDirection = ControlFlowDirection.SingleTopToBottom;
        OuterControlPadding = new Vector2(6, 6);
        ControlPadding = new Vector2(6, 6);
        ShowBorder = true;
        Visible = settings.TranslationTranslator.Value == (int)Translators.LibreTranslate;
        Height = settings.TranslationTranslator.Value == (int)Translators.LibreTranslate ? 1 : 0;

        settings.TranslationTranslator.SettingChanged += (o, e) =>
        {
            Visible = e.NewValue == (int)Translators.LibreTranslate;
            Height = settings.TranslationTranslator.Value == (int)Translators.LibreTranslate ? 1 : 0;
        };

        var urlPanel = new FlowPanel
        {
            Parent = this,
            FlowDirection = ControlFlowDirection.LeftToRight,
            WidthSizingMode = SizingMode.Fill,
            HeightSizingMode = SizingMode.AutoSize,
            ControlPadding = new Vector2(6, 0)
        };

        var urlLabel = new Label
        {
            Parent = urlPanel,
            Text = "API URL",
            ShowShadow = true,
            Height = 28,
            Width = 180
        };

        var urlInput = new TextBox
        {
            Parent = urlPanel,
            Text = settings.TranslationLibreTranslateUrl.Value
        };

        urlInput.InputFocusChanged += (o, e) =>
        {
            if (!e.Value)
                settings.TranslationLibreTranslateUrl.Value = urlInput.Text;
        };
    }
}