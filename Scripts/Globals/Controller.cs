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

	public enum Sound {HOVER, SELECT};

	// Refs
	private PackedScene DialogueRef = GD.Load<PackedScene>("res://Instances/System/Dialogue.tscn");
	private PackedScene SoundBurstRef = GD.Load<PackedScene>("res://Instances/System/SoundBurst.tscn");

	// Sounds
	private AudioStreamPlayer SoundSysHover;
	private AudioStreamPlayer SoundSysSelect;

	// ================================================================

    public override void _Ready()
    {
        // Refs
		SoundSysHover = GetNode<AudioStreamPlayer>("SoundSysHover");
		SoundSysSelect = GetNode<AudioStreamPlayer>("SoundSysSelect");
    }


	public override void _Process(float delta)
	{

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


	public static void PlaySoundBurst(AudioStream sound, float volume = 0f, float pitch = 1f)
	{
		var sb = Controller.Main.SoundBurstRef.Instance() as AudioStreamPlayer;
		sb.Stream = sound;
		sb.VolumeDb = volume;
		sb.PitchScale = pitch;
		Controller.Main.GetTree().GetRoot().AddChild(sb);
		sb.Play();
	}


	public static void PlaySystemSound(Sound sound)
	{
		switch (sound)
		{
			case Controller.Sound.HOVER:
				PlaySoundBurst(Controller.Main.SoundSysHover.Stream, volume: -8, pitch: 1.1f);
				break;
			case Controller.Sound.SELECT:
				PlaySoundBurst(Controller.Main.SoundSysSelect.Stream);
				break;
			
		}
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
			dlg.LineEnd = 248;

		dlg.Position = new Vector2(Player.GetCamera().GetCameraScreenCenter().x - 300, Player.GetCamera().GetCameraScreenCenter().y - 180);
		Controller.Main.GetTree().GetRoot().AddChild(dlg);
		dlg.InitializePortraits();
	}


	public static void Fade(bool fadein, bool white, float time)
	{
		Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1f / time;
		if (white)
			Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").Play(fadein ? "Fadein white" : "Fadeout white");
		else
			Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").Play(fadein ? "Fadein black" : "Fadeout black");
	}


	public static void PlayMusic(AudioStream music)
	{
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Stream = music;
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Play();
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
