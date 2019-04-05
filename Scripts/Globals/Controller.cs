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

	private Dictionary<string, bool> flag = new Dictionary<string, bool>()
	{
		// Put flags here
	};

	public enum Sound {HOVER, SELECT};

	/* public struct DialogueParams
	{
		string sourceFile;
		int dialogueSet;
		string leftClientName;
		
	} */

	// Refs
	private PackedScene DialogueRef = GD.Load<PackedScene>("res://Instances/System/Dialogue.tscn");
	private PackedScene SoundBurstRef = GD.Load<PackedScene>("res://Instances/System/SoundBurst.tscn");

	private Timer TimerEndTransitionRef;
	private Timer TimerEndTransition2Ref;

	// Sounds
	private AudioStreamPlayer SoundSysHover;
	private AudioStreamPlayer SoundSysSelect;

	// ================================================================

    public override void _Ready()
    {
        // Refs
		SoundSysHover = GetNode<AudioStreamPlayer>("SoundSysHover");
		SoundSysSelect = GetNode<AudioStreamPlayer>("SoundSysSelect");

		TimerEndTransitionRef = GetNode<Timer>("TimerEndTransition");
		TimerEndTransition2Ref = GetNode<Timer>("TimerEndTransition2");
    
		GD.Randomize();
		
	}


	/* public override void _Process(float delta)
	{

	}*/

	// ================================================================

	public static bool Flag(string flag)
	{
		return Controller.Main.flag[flag];
	}


	public static void SetFlag(string flag, bool value)
	{
		Controller.Main.flag[flag] = value;
	}


	public static SceneTag GetSceneTag()
	{
		return Controller.Main.GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag");
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


	public static void Dialogue(string sourceFile, int dialogueSet, string leftClientName, string leftClientColor, SpriteFrames leftClientPortrait, string rightClientName = "NULL", string rightClientColor = "#ffffff", SpriteFrames rightClientPortrait = null, bool restoreMovement = true, Node signalConnection = null, string signalMethod = "")
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

		dlg.RestoreMovement = restoreMovement;

		if (rightClientPortrait != null)
			dlg.LineEnd = 248;
		else
		{
			dlg.TextLeft += 35;

			// Shift box positions (start)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 0, new Vector2(335, 424));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 1, new Vector2(335, 284));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 0, new Vector2(83, 368));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 1, new Vector2(83, 228));
		
			// Shift box positions (finish)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 0, new Vector2(335, 284));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 1, new Vector2(335, 424));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 0, new Vector2(83, 228));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 1, new Vector2(83, 368));
		}
			
		dlg.Position = new Vector2(Player.GetCamera().GetCameraScreenCenter().x - 300, Player.GetCamera().GetCameraScreenCenter().y - 180);
		Controller.Main.GetTree().GetRoot().AddChild(dlg);
		dlg.InitializePortraits();

		if (signalConnection != null && signalMethod != "")
			dlg.Connect("text_ended", signalConnection, signalMethod);
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


	public static void EndTransition()
	{
		Controller.Main.TimerEndTransitionRef.Start();
		Fade(true, false, 0.25f);
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


	private void EndTransition2()
	{
		Controller.Main.TimerEndTransition2Ref.Start();
	}


	private void EndTransition3()
	{
		Player.MotionOverrideVec = new Vector2(0, 0);
		Player.MotionOverride = false;
		Player.Motion = new Vector2(0, 0);
		Player.Walking = false;
		Player.State = Player.ST.MOVE;
		Player.Teleporting = false;
	}
}
