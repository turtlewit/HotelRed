using Godot;
using System;
using System.Collections.Generic;

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

	[Export(PropertyHint.File, "*.txt")]
	private string debugDialogueFile2 = string.Empty;

	// ================================================================

	// Movement
	private Vector2 motion = new Vector2(0, 0);
	//private Vector2 face = new Vector2(1, 1);
	private SpriteDirection face = SpriteDirection.DOWN;
	private bool walking = false;
	private bool teleporting = false;

	private bool motionOverride = false;
	private Vector2 motionOverrideVec = new Vector2(0, 0);
	private float walkSpeedOverride = 180f;

	// Sprite sets 
	public enum SpriteSet {NORMAL, PAPER};
	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};
	private SpriteSet currentSpriteSet = SpriteSet.NORMAL;

	private Queue<string> keyQueue = new Queue<string>();

	private static readonly string[] spriteSetNormal = {"up", "down", "left", "right"};
	private static readonly string[] spriteSetNormalWalk = {"walkup", "walkdown", "walkleft", "walkright"};
	private static readonly string[] spriteSetPaper = {"up", "down_paper", "left", "right_paper"};
	private static readonly string[] spriteSetPaperWalk = {"walkup", "walkdown_paper", "walkleft", "walkright_paper"};

	// Step sounds
	private static readonly string[] stepSounds = {"SoundStep1", "SoundStep2", "SoundStep3", "SoundStep4", "SoundStep5"};
	private float sound = -1;

	// States
	public enum ST {MOVE, NO_INPUT};
	private ST state = ST.MOVE;

	// Constants
	private const float WalkSpeed = 180f;

	// Refs
	private AnimatedSprite Spr;
	private Timer TimerStepSound;

	// ================================================================

	public static ST State { get { return Player.Main.state; } set { Player.Main.state = value; } }
	public static Vector2 Motion { get { return Player.Main.motion; } set { Player.Main.motion = value; } }
	public static bool MotionOverride { get { return Player.Main.motionOverride; } set { Player.Main.motionOverride = value; } }
	public static Vector2 MotionOverrideVec { get { return Player.Main.motionOverrideVec; } set { Player.Main.motionOverrideVec = value; } }
	public static float WalkSpeedOverride { get { return Player.Main.walkSpeedOverride; } set {Player.Main.walkSpeedOverride = value; } }
	public static SpriteDirection Face { get { return Player.Main.face; } set { Player.Main.face = value; } }
	public static bool Walking { get { return Player.Main.walking; } set { Player.Main.walking = value; } }
	public static SpriteSet CurrentSpriteSet { get { return Player.Main.currentSpriteSet; } set { Player.Main.currentSpriteSet = value; } }

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
		ZIndex = (int)Position.y;

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

		if (!motionOverride)
			motion = MoveAndSlide(motion * WalkSpeed * delta * 60f);
		else
			motion = MoveAndSlide(motionOverrideVec * walkSpeedOverride * delta * 60f);
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


	/* public static void StopWalking()
	{
		Player.Main.walking = false;
	} */

	// ================================================================

	private void Movement()
	{
		// Movement direction
		//Vector2 previous = new Vector2(motion.x, motion.y);

		int lMove = Input.IsActionPressed("move_left") ? 1 : 0;
		int rMove = Input.IsActionPressed("move_right") ? 1 : 0;
		motion.x = rMove - lMove;

		int uMove = Input.IsActionPressed("move_up") ? 1 : 0;
		int dMove = Input.IsActionPressed("move_down") ? 1 : 0;
		motion.y = dMove - uMove;

		//ChangeFace(previous);
		if (Input.IsActionJustReleased("move_up") || Input.IsActionJustReleased("move_down") || Input.IsActionJustReleased("move_left") || Input.IsActionJustReleased("move_right"))
			ChangeFace();
		
		/* if (Input.IsActionJustPressed("move_up"))
			face = SpriteDirection.UP;

		if (Input.IsActionJustPressed("move_down"))
			face = SpriteDirection.DOWN;

		if (Input.IsActionJustPressed("move_left"))
			face = SpriteDirection.LEFT;

		if (Input.IsActionJustPressed("move_right"))
			face = SpriteDirection.RIGHT; */

		// Debug
		if (Input.IsActionJustPressed("debug_1"))
			//Controller.Dialogue(debugDialogueFile, 0, "Ravia", "#2391ef",  debugSpriteFrames2, "Neftali", "#ff0000", debugSpriteFrames);
			Controller.Dialogue(debugDialogueFile2, 0, "Ravia", "#2391ef",  debugSpriteFrames2);
	
		if (Input.IsActionJustPressed("debug_2"))
			Controller.Fade(false, false, 1);

		if (Input.IsActionJustPressed("debug_3"))
			Controller.Fade(true, false, 1);
	}


	private void ChangeFace()  // REFACTOR THIS AWFULNESS LATER
	//private void ChangeFace()
	{
		/* if (previous.x != motion.x)
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
		}*/

		/* switch (previous)
		{
			case SpriteDirection.UP:
			{
				if (motion.y > 0)
					face = SpriteDirection.
			}
		}*/

		if (motion.y > 0)
			face = motion.x < 0 ? SpriteDirection.LEFT : motion.x > 0 ? SpriteDirection.RIGHT : SpriteDirection.DOWN;
		else if (motion.y < 0)
			face = motion.x < 0 ? SpriteDirection.LEFT : motion.x > 0 ? SpriteDirection.RIGHT : SpriteDirection.UP;
	}


	private void Animation()
	{
		/* if (face == Vector2.Up)
		{
			Spr.Play(GetSprite(currentSpriteSet, SpriteDirection.UP, walking));
		}
		else if (face == Vector2.Down)
		{
			Spr.Play(GetSprite(currentSpriteSet, SpriteDirection.DOWN, walking));
		}
		else if (face == Vector2.Left)
		{
			Spr.Play(GetSprite(currentSpriteSet, SpriteDirection.LEFT, walking));
		}
		else if (face == Vector2.Right)
		{
			Spr.Play(GetSprite(currentSpriteSet, SpriteDirection.RIGHT, walking));
		}*/

		Spr.Play(GetSprite(currentSpriteSet, face, walking));
	}


	private void FootstepSound()
	{
		Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSounds)).Stream, volume: -4f, pitch: (float)GD.RandRange(0.9, 1.1));
	}


	private string GetSprite(SpriteSet set, SpriteDirection direction, bool walking)
	{
		switch (set)
		{
			case SpriteSet.NORMAL:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];

			case SpriteSet.PAPER:
				return walking ? spriteSetPaperWalk[(int)direction] : spriteSetPaper[(int)direction];
			
			default:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];
		}
	}
}
