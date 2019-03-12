using Godot;
using System;

public class Player : KinematicBody2D
{
	private static Player inst;
    private static Player Main { get { return inst; } }

	Player()
	{
		inst = this;
	}

	// ================================================================

	[Export]
	private SpriteFrames debugSpriteFrames;

	[Export(PropertyHint.File, "*.txt")]
	private string debugDialogueFile = string.Empty;

	// ================================================================

	private Vector2 motion = new Vector2(0, 0);
	private Vector2 face = new Vector2(1, 1);
	private bool walking = false;

	public enum ST {MOVE, NO_INPUT};
	private ST state = ST.MOVE;

	private const float WalkSpeed = 180f;

	// ================================================================

	public static ST State { get { return Player.Main.state; } set { Player.Main.state = value; } }

	// ================================================================

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
		int lMove = Input.IsActionPressed("move_left") ? 1 : 0;
		int rMove = Input.IsActionPressed("move_right") ? 1 : 0;
		motion.x = rMove - lMove;

		int uMove = Input.IsActionPressed("move_up") ? 1 : 0;
		int dMove = Input.IsActionPressed("move_down") ? 1 : 0;
		motion.y = dMove - uMove;

		// Sprite facing direction
		if (Input.IsActionJustPressed("move_up"))
			face.y = -1;

		if (Input.IsActionJustPressed("move_down"))
			face.y = 1;

		if (Input.IsActionJustPressed("move_left"))
			face.x = -1;

		if (Input.IsActionJustPressed("move_right"))
			face.x = 1;

		// Debug
		if (Input.IsActionJustPressed("debug_1"))
			Controller.Dialogue(debugDialogueFile, 0, "Neftali", "#ff0000",  debugSpriteFrames, "Not Neftali", "#880000", debugSpriteFrames);
	}
}
