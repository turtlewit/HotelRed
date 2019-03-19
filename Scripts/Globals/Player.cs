using Godot;
using System;

public class Player : KinematicBody2D
{
	private static Player inst;
    public static Player Main { get { return inst; } }

	Player()
	{
		inst = this;
	}

	// ================================================================

	[Export]
	private SpriteFrames debugSpriteFrames;

	[Export]
	private SpriteFrames debugSpriteFrames2;

	[Export(PropertyHint.File, "*.txt")]
	private string debugDialogueFile = string.Empty;

	// ================================================================

	private Vector2 motion = new Vector2(0, 0);
	private Vector2 face = new Vector2(1, 1);
	private bool walking = false;

	private string[] stepSounds = {"SoundStep1", "SoundStep2", "SoundStep3", "SoundStep4", "SoundStep5"};
	private float sound = -1;

	public enum ST {MOVE, NO_INPUT};
	private ST state = ST.MOVE;

	private const float WalkSpeed = 180f;

	// Refs
	private AnimatedSprite Spr;
	private Timer TimerStepSound;

	// ================================================================

	public static ST State { get { return Player.Main.state; } set { Player.Main.state = value; } }

	// ================================================================

	public override void _Ready()
	{
		// Refs
		Spr = GetNode<AnimatedSprite>("Sprite");
		TimerStepSound = GetNode<Timer>("TimerStepSound");
	}

	public override void _Process(float delta)
	{
		if (walking)
			sound += 1 * delta;
		else
			sound = -1;

		if (walking && TimerStepSound.IsStopped())
		{
			FootstepSound();
			TimerStepSound.SetWaitTime((1f / 7f) * 2f);
			TimerStepSound.Start();
		}
		else if (!walking)
			TimerStepSound.Stop();
	}

	public override void _PhysicsProcess(float delta)
	{
		switch (state)
		{
			case ST.MOVE:
			{
				Movement();
				walking = motion.x != 0 || motion.y != 0;
				break;
			}

			case ST.NO_INPUT:
			{
				motion = Vector2.Zero;
				break;
			}
		}

		Animation();

		motion = MoveAndSlide(motion * WalkSpeed * delta * 60f);
	}

	// ================================================================

	public static Camera2D GetCamera()
	{
		return Player.Main.GetNode<Camera2D>("Camera");
	}


	public static void EnableCamera(bool enable)
	{
		Player.Main.GetNode<Camera2D>("Camera").Current = enable;
	}

	// ================================================================

	private void Movement()
	{
		// Movement direction
		Vector2 previous = new Vector2(motion.x, motion.y);

		int lMove = Input.IsActionPressed("move_left") ? 1 : 0;
		int rMove = Input.IsActionPressed("move_right") ? 1 : 0;
		motion.x = rMove - lMove;

		int uMove = Input.IsActionPressed("move_up") ? 1 : 0;
		int dMove = Input.IsActionPressed("move_down") ? 1 : 0;
		motion.y = dMove - uMove;

		ChangeFace(previous);

		// Debug
		if (Input.IsActionJustPressed("debug_1"))
			Controller.Dialogue(debugDialogueFile, 0, "Ravia", "#2391ef",  debugSpriteFrames2, "Neftali", "#ff0000", debugSpriteFrames);
	}


	private void ChangeFace(Vector2 previous)  // REFACTOR THIS AWFULNESS LATER
	{
		if (previous.x != motion.x)
		{
			if (motion.x != 0)
			{
				if (previous.y == motion.y && motion.y == 0)
				{
					face.x = Mathf.Sign(motion.x);
					face.y = 0;
				}
				else
				{
					face.x = 0;
					if (motion.y != 0)
						face.y = Mathf.Sign(motion.y);
				}
			}
			else if (motion.y != 0)
			{
				face.y = Mathf.Sign(motion.y);
				face.x = 0;
			}
		}
		else if (previous.y != motion.y)
		{
			if (motion.y != 0)
			{
				if (previous.x == motion.x && motion.x == 0)
				{
					face.y = Mathf.Sign(motion.y);
					face.x = 0;
				}
				else
				{
					face.y = 0;
					if (motion.x != 0)
						face.x = Mathf.Sign(motion.x);
				}
			}
			else if (motion.x != 0)
			{
				face.x = Mathf.Sign(motion.x);
				face.y = 0;
			}
		}
	}


	private void Animation()
	{
		if (face == Vector2.Up)
		{
			Spr.Play(walking ? "walkup" : "up");
		}
		else if (face == Vector2.Down)
		{
			Spr.Play(walking ? "walkdown" : "down");
		}
		else if (face == Vector2.Left)
		{
			Spr.Play(walking ? "walkleft" : "left");
		}
		else if (face == Vector2.Right)
		{
			Spr.Play(walking ? "walkright" : "right");
		}
	}


	private void FootstepSound()
	{
		Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSounds)).Stream, volume: -4f, pitch: (float)GD.RandRange(0.9, 1.1));
	}
}
