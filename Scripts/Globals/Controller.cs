using Godot;
using System;
using System.Collections.Generic;

public class Controller : Node
{
	private static Controller inst;
    private static Controller Main { get { return inst; } }

	Controller()
	{
		inst = this;
	}

	// ================================================================

	private Dictionary<string, int> flag = new Dictionary<string, int>()
	{
		// Put flags here
	};

	// Refs
	private PackedScene DialogueRef = GD.Load<PackedScene>("res://Instances/System/Dialogue.tscn");
	private PackedScene SoundBurstRef = GD.Load<PackedScene>("res://Instances/System/SoundBurst.tscn");

	// ================================================================

    public override void _Ready()
    {
        
    }


	public override void _Process(float delta)
	{
		//GD.Print(keyboardLock);
	}

	// ================================================================

	public static int Flag(string flag)
	{
		return Controller.Main.flag[flag];
	}


	public static void SetFlag(string flag, int value)
	{
		Controller.Main.flag[flag] = value;
	}


	public static void SceneGoto(PackedScene targetScene)
	{
		Controller.Main.SceneGotoPre();
		Controller.Main.GetTree().ChangeSceneTo(targetScene);
		Controller.Main.GetNode<Timer>("TimerSceneGoto").Start();
	}


	public static void PlaySoundBurst(AudioStream sound, float volume = 0f, float pitch = 0f)
	{
		var sb = Controller.Main.SoundBurstRef.Instance() as AudioStreamPlayer;
		sb.Stream = sound;
		sb.VolumeDb = volume;
		sb.PitchScale = pitch;
		Controller.Main.GetTree().GetRoot().AddChild(sb);
		sb.Play();
	}


	public static void Dialogue(string sourceFile, int dialogueSet, string leftClientName, string leftClientColor, SpriteFrames leftClientPortrait, string rightClientName = "NULL", string rightClientColor = "#ffffff", SpriteFrames rightClientPortrait = null)
	{
		Player.State = Player.ST.NO_INPUT;
		var dlg = Controller.Main.DialogueRef.Instance() as Dialogue;
		dlg.SourceFile = sourceFile;
		dlg.TextSet = dialogueSet;
		dlg.SecondClient = rightClientPortrait != null;

		dlg.LeftClientName = leftClientName;
		dlg.LeftClientColor = new Color(leftClientColor);
		dlg.LeftClientPortrait = leftClientPortrait;

		dlg.RightClientName = rightClientName;
		dlg.RightClientColor = new Color(rightClientColor);
		dlg.RightClientPortrait = rightClientPortrait;

		if (rightClientPortrait != null)
			dlg.LineEnd = 258;

		//dlg.Position = new Vector2(Player.GetCamera().Position.x + 300, Player.GetCamera().GlobalPosition.y + 180);
		//GD.Print(Player.GetCamera().GlobalPosition);
		GD.Print(Player.GetCamera().GetCameraScreenCenter());
		dlg.Position = new Vector2(Player.GetCamera().GetCameraScreenCenter().x - 300, Player.GetCamera().GetCameraScreenCenter().y - 180);
		Controller.Main.GetTree().GetRoot().AddChild(dlg);
		//Player.GetCamera().AddChild(dlg);
		dlg.InitializePortraits();
	}

	// ================================================================

	private void SceneGotoPre()
	{

	}


	private void SceneGotoPost()
	{
		Player.GetCamera().LimitRight = GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag").CameraLimitRight;
		Player.GetCamera().LimitBottom = GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag").CameraLimitBottom;
	}
}
