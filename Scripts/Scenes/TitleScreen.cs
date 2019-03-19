using Godot;
using System;

public class TitleScreen : Control
{
    [Export]
	private bool UseTitleScreen = true;

	[Export]
	private PackedScene StartScene;

	// ================================================================

    public override void _Ready()
    {
        if (!UseTitleScreen)
			Controller.SceneGoto(StartScene);
    }


	public void ButtonHover()
	{
		Controller.PlaySystemSound(Controller.Sound.HOVER);
	}


	public void ClickNewGame()
	{
		Controller.PlaySystemSound(Controller.Sound.SELECT);
		Player.EnableCamera(true);
		Controller.SceneGoto(StartScene);
		Player.Main.Position = new Vector2(0, 185);
	}
}
