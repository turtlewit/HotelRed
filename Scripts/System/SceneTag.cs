using Godot;
using System;

public class SceneTag : Label
{
    [Export]
	private string sceneName = string.Empty;

	[Export]
	private bool displayName = true;

	[Export]
	private AudioStream sceneMusic;

	[Export]
	private int cameraLimitRight;

	[Export]
	private int cameraLimitBottom;

	// ================================================================

	public int CameraLimitRight { get { return cameraLimitRight; } }
	public int CameraLimitBottom { get { return cameraLimitBottom; } }
}
