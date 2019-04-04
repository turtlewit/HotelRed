using Godot;
using System;

public class Event_test : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile;

	[Export]
	private SpriteFrames raviaFrames;

	[Export]
	private SpriteFrames neftaliFrames;

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

    public void Event_Test1()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 0, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}
}
