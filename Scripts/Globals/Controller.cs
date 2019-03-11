using Godot;
using System;
using System.Collections.Generic;

public class Controller : Node
{
	private static Controller inst;
    public static Controller Main { get { return inst; } }

	Controller()
	{
		inst = this;
	}

	// ================================================================

	private Dictionary<string, int> flag = new Dictionary<string, int>()
	{
		// Put flags here
	};

	public enum KB {MOVE, MENU, TALK};
	private KB keyboardLock = KB.MOVE;

	// Refs
	private PackedScene DialogueRef = GD.Load<PackedScene>("res://Instances/System/Dialogue.tscn");
	private PackedScene SoundBurstRef = GD.Load<PackedScene>("res://Instances/System/SoundBurst.tscn");

	// ================================================================
 
	public KB KeyboardLock { get; set; }

	// ================================================================

    public override void _Ready()
    {
        
    }

	// ================================================================

	public int Flag(string flag)
	{
		return this.flag[flag];
	}

	public void SetFlag(string flag, int value)
	{
		this.flag[flag] = value;
	}

	public void SceneGoto(PackedScene targetScene)
	{
		SceneGotoPre();
		GetTree().ChangeSceneTo(targetScene);
		SceneGotoPost();
	}


	public void PlaySoundBurst(AudioStream sound, float volume = 0f, float pitch = 0f)
	{
		var sb = SoundBurstRef.Instance() as AudioStreamPlayer;
		sb.Stream = sound;
		sb.VolumeDb = volume;
		sb.PitchScale = pitch;
		GetTree().GetRoot().AddChild(sb);
		sb.Play();
	}


	public void Dialogue(string sourceFile, int dialogueSet, string leftClientName, string leftClientColor, SpriteFrames leftClientPortrait, string rightClientName = "NULL", string rightClientColor = "#ffffff", SpriteFrames rightClientPortrait = null)
	{
		var dlg = DialogueRef.Instance() as Dialogue;
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

		GetTree().GetRoot().AddChild(dlg);
	}

	// ================================================================

	private void SceneGotoPre()
	{

	}


	private void SceneGotoPost()
	{
		
	}
}
