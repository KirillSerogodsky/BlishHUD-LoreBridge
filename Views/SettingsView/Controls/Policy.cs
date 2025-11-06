using Blish_HUD.Controls;
using LoreBridge.Models;
using LoreBridge.Utils;

namespace LoreBridge.Views.SettingsView.Controls;

public class Policy
{
    private readonly StandardButton _button;
    
    public Policy(Panel mainPanel, Settings settings)
    {
        _button = new StandardButton
        {
            Parent = mainPanel,
            Text = "Accept the policy for using the module",
            Width = 260,
        };
        _button.Click += async (_, _) =>
        {
            _button.Enabled = false;
            var result = await PolicyConfirmation.ShowPolicyConfirmationAsync();
            if (result)
            {
                settings.ContentUsePolicyConfirmation.Value = true;
            }
            _button.Enabled = true;
        };
    }

    public void Dispose()
    {
        _button.Dispose();
    }
}