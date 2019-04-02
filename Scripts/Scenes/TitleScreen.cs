using Godot;
using System;

public class TitleScreen : Control
{
    [Export]
	private bool UseTitleScreen = true;

	[Export]
	private PackedScene StartScene;

	[Export]
	private float shaderWaveConst = 0.1f;

	[Export]
	private float ShaderWaveSpeed = 5f;

	private bool passToShader = true;

	// Refs
	private ShaderMaterial Shader;

	// ================================================================

    public override void _Ready()
    {
        if (!UseTitleScreen)
			Controller.SceneGoto(StartScene);

		Shader = (ShaderMaterial)GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("Shader").Material;
    }


	public override void _Process(float delta)
	{
		if (passToShader)
		{
			Shader.SetShaderParam("waveConst", shaderWaveConst);
			Shader.SetShaderParam("speed", ShaderWaveSpeed);
		}
	}

	// ================================================================


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

	// ================================================================

	private void AnimationFinished(string anim_name)
	{
		if (anim_name == "Fadein")
		{
			passToShader = false;
			GetNode<CanvasLayer>("CanvasLayer2").GetNode<ColorRect>("Fadein").QueueFree();
			GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("Shader").QueueFree();
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Fadein 2");
		}
	}

}
