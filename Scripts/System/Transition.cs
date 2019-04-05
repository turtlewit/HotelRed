using Godot;
using System;

public class Transition : Area2D
{
	[Export]
	private PackedScene targetScene;

	[Export]
	private Vector2 targetPosition;

    [Export]
	private Direction targetDirection;

	[Export]
	private bool walk;

	// Refs
	private Timer TimerFadeOut;
	private Timer TimerTransition;
	private Timer TimerFadeIn;

	// ================================================================

	private enum Direction {Up, Down, Left, Right};

	// ================================================================

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TimerFadeOut = GetNode<Timer>("TimerFadeOut");
		TimerTransition = GetNode<Timer>("TimerTransition");
		TimerFadeIn = GetNode<Timer>("TimerFadeIn");
    }

	// ================================================================

	private void BodyEntered(PhysicsBody2D body)
	{
		if (!Player.Teleporting && body.IsInGroup("Player"))
		{
			Player.Teleporting = true;
			Player.State = Player.ST.NO_INPUT;
			Player.Walking = true;
			Player.MotionOverride = true;
			Player.MotionOverrideVec = new Vector2(0, -1);
			Player.WalkSpeedOverride = 150f;

			switch (targetDirection)
			{
				case Direction.Up:
					Player.Face = Player.SpriteDirection.UP;
					break;
				case Direction.Down:
					Player.Face = Player.SpriteDirection.DOWN;
					break;
				case Direction.Left:
					Player.Face = Player.SpriteDirection.LEFT;
					break;
				case Direction.Right:
					Player.Face = Player.SpriteDirection.RIGHT;
					break;
				default:
					Player.Face = Player.SpriteDirection.UP;
					break;
			}

			TimerFadeOut.Start();
		}
	}


	private void StartFadeOut()
	{
		Controller.Fade(false, false, 0.25f);

		TimerTransition.Start();
	}


	private void ChangeScenes()
	{
		Controller.SceneGoto(targetScene);
		
		/* if (walk)
		{
			switch (targetDirection)
				{
					case Direction.Up:
					{
						float x = Player.Main.Position.x;
						Player.Main.Position = new Vector2(x, targetPosition.y);
						break;
					}
					case Direction.Down:
					{
						float x = Player.Main.Position.x;
						Player.Main.Position = new Vector2(x, targetPosition.y);
						break;
					}
					case Direction.Left:
					{
						float y = Player.Main.Position.y;
						Player.Main.Position = new Vector2(targetPosition.x, y);
						break;
					}
					case Direction.Right:
					{
						float y = Player.Main.Position.y;
						Player.Main.Position = new Vector2(targetPosition.x, y);
						break;
					}
					default:
						Player.Main.Position = targetPosition;
						break;
				}
		}
		else */
		
		Player.Main.Position = targetPosition;

		Controller.EndTransition();
	}


	private void StartFadeIn()
	{
		Controller.Fade(true, false, 0.25f);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
