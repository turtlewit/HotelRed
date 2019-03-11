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
			Controller.Main.SceneGoto(StartScene);
    }


	public void ClickNewGame()
	{
		Player.Main.EnableCamera(true);
		Controller.Main.SceneGoto(StartScene);
	}
}
