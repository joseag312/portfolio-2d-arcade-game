using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class MenuStore : Control
{
    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public HUDDialogSystem HUD;
    [Export] public BuyPowerUpContainer BuyPowerUpContainer;
    [Export] public UpgradePowerUpContainer UpgradePowerUpContainer;
    [Export] public UpgradeShipContainer UpgradeShipContainer;
    [Export] public AssignContainerBox AssignContainerBox;
    [Export] public Button ReturnButton;
    [Export] public Button EquipButton;
    [Export] public Button BuyButton;
    [Export] public Button UpgradeButton;

    private string _selectedWeaponKey = "";

    public override void _Ready()
    {
        G.MS.PlayTrack(Music.FADE);
        WireGrid();
        WireSpecials();
        WireCards();
        WireButtons();

        if (!G.GS.IsDialogSeen("ArmoryIntro"))
        {
            _ = ArmoryIntroDialog();
        }
    }

    private void WireGrid()
    {
        foreach (var storeItem in GetTree().GetNodesInGroup("store_item_panels"))
        {
            if (storeItem is StoreItemPanel panel)
            {
                panel.StoreItemClicked += OnStoreItemClicked;
            }
        }
    }

    private void WireSpecials()
    {
        foreach (var specialItem in GetTree().GetNodesInGroup("special_item_buttons"))
        {
            if (specialItem is SpecialUpgradeButton button)
            {
                button.SpecialItemClicked += OnSpecialItemClicked;
            }
        }
    }

    private void WireCards()
    {
        BuyPowerUpContainer.PurchaseMessage += OnPurchaseMessage;
        UpgradePowerUpContainer.UpgradeMessage += OnUpgradeMessage;
        UpgradeShipContainer.UpgradeMessage += OnUpgradeMessage;
    }

    private void WireButtons()
    {
        EquipButton.Pressed += OnEquipPressed;
        BuyButton.Pressed += OnBuyPressed;
        UpgradeButton.Pressed += OnUpgradePressed;
        ReturnButton.Pressed += OnReturnPressed;
    }

    private void OnSpecialItemClicked(string specialItemKey)
    {
        G.SFX.Play(SFX.CLICK);
        if ("Ship".Equals(specialItemKey))
        {
            UpgradePowerUpContainer.Visible = false;
            BuyPowerUpContainer.Visible = false;
            UpgradeShipContainer.Visible = true;
        }
        else
        {
            _selectedWeaponKey = specialItemKey;
            BuyPowerUpContainer.Visible = false;
            UpgradeShipContainer.Visible = false;
            UpgradePowerUpContainer.LoadContainer(_selectedWeaponKey);
            UpgradePowerUpContainer.Visible = true;
        }
        EquipButton.Visible = false;
        BuyButton.Visible = false;
        UpgradeButton.Visible = false;
    }

    private void OnStoreItemClicked(string storeItemKey)
    {
        G.SFX.Play(SFX.CLICK);
        _selectedWeaponKey = storeItemKey;
        EquipButton.Visible = true;
        BuyButton.Visible = true;
        UpgradeButton.Visible = true;
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.Visible = false;
        BuyPowerUpContainer.LoadContainer(_selectedWeaponKey);
        BuyPowerUpContainer.Visible = true;
    }

    private void OnPurchaseMessage(string message)
    {
        _ = HUD.PopUpMessage(Char.STORE, Mood.STORE.Default, message);
    }

    private void OnUpgradeMessage(string message)
    {
        _ = HUD.PopUpMessage(Char.STORE, Mood.STORE.Default, message);
    }

    private void OnEquipPressed()
    {
        G.SFX.Play(SFX.CLICK);
        AssignContainerBox.OpenFor(_selectedWeaponKey);
    }

    private void OnBuyPressed()
    {
        G.SFX.Play(SFX.CLICK);
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.Visible = false;
        BuyPowerUpContainer.LoadContainer(_selectedWeaponKey);
        BuyPowerUpContainer.Visible = true;
    }

    private void OnUpgradePressed()
    {
        G.SFX.Play(SFX.CLICK);
        BuyPowerUpContainer.Visible = false;
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.LoadContainer(_selectedWeaponKey);
        UpgradePowerUpContainer.Visible = true;
    }

    private async void OnReturnPressed()
    {
        G.SFX.Play(SFX.CLICK);
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuLevelsScene);
    }

    public async Task ArmoryIntroDialog()
    {
        await Task.Delay(300);
        await HUD.FirstMessage(Char.STORE, Mood.STORE.Default, "Ah, rookie! Welcome, welcome!");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "I handle the armory for Spin God… with a small fee, of course.");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "Unlock a weapon, and I’ll make it stronger. Shinier too, if you’ve got the money.");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "Remember to equip my new creation! If you want to use it, of course...");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "Survival, rookie... Upgrade your health and your basic laser.");
        await HUD.Message(Char.STORE, Mood.STORE.Default, "Nothing’s free, kid. Not survival, not style.");
        await HUD.LastMessage(Char.STORE, Mood.STORE.Default, "Take a look around… and remember, no refunds.");
        G.GS.MarkDialogSeen("ArmoryIntro");
        G.GS.Save();
    }
}
