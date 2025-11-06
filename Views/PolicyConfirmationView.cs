using System.Diagnostics;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using View = Blish_HUD.Graphics.UI.View;

namespace LoreBridge.Views;

internal class PolicyConfirmationView : View
{
    public TaskCompletionSource<bool> ResultTask { get; } = new();
    private Container _buildPanel;

    protected override void Build(Container buildPanel)
    {
        _buildPanel = buildPanel;

        var container = new Panel()
        {
            Parent = buildPanel,
            Title = "CONTENT USE POLICY AGREEMENT",
            ShowBorder = true,
            BackgroundTexture = GameService.Content.GetTexture("tooltip"),
            Size = buildPanel.Size,
            ClipsBounds = false,
            HeightSizingMode = SizingMode.AutoSize
        };

        var builder = new FormattedLabelBuilder()
            .CreatePart("The LoreBridge module provides translation functionality which may violate ",
                b => b.SetFontSize(ContentService.FontSize.Size16))
            .CreatePart("ArenaNet's Content Use Policy",
                b => b.SetFontSize(ContentService.FontSize.Size16).MakeBold().SetTextColor(Color.Red))
            .CreatePart(".",
                b => b.SetFontSize(ContentService.FontSize.Size16))
            .CreatePart("\n\nPlease read ", b => b.SetFontSize(ContentService.FontSize.Size16))
            .CreatePart("Content Use Policy",
                b => b.SetFontSize(ContentService.FontSize.Size16).MakeBold().SetTextColor(Color.Cyan)
                    .SetLink(OpenPolicyLink))
            .CreatePart(" before accepting.\n\n", b => b.SetFontSize(ContentService.FontSize.Size16));

        var text = builder
            .SetWidth(buildPanel.Width - 30)
            .AutoSizeHeight()
            .Wrap()
            .Build();
        text.Location = new Point(15, 15);
        text.Parent = container;

        var checkBox = new Checkbox()
        {
            Parent = container,
            Text = "I have read and understand the risks",
            Location = new Point(15, text.Bottom + 40),
            Width = 200,
            Checked = false
        };

        var acceptButton = new StandardButton
        {
            Parent = container,
            Text = "Accept",
            Width = 100,
            Location = new Point((container.Width - 210) / 2, checkBox.Bottom + 20),
            Enabled = false
        };
        acceptButton.Click += (_, _) => Close(true);

        var declineButton = new StandardButton
        {
            Parent = container,
            Text = "Decline",
            Width = 100,
            Location = new Point(acceptButton.Right + 10, checkBox.Bottom + 20)
        };
        declineButton.Click += (_, _) => Close(false);

        checkBox.CheckedChanged += (s, e) => { acceptButton.Enabled = checkBox.Checked; };
    }

    private static void OpenPolicyLink()
    {
        Process.Start("https://www.arena.net/en/legal/content-terms-of-use/");
    }

    private void Close(bool result)
    {
        ResultTask.TrySetResult(result);
        _buildPanel.Dispose();
    }
}