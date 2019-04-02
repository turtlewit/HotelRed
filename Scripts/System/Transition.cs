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

	// ================================================================

	private enum Direction {Up, Down, Left, Right};

	// ================================================================

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

	// ================================================================

	private void BodyEntered(PhysicsBody2D body)
	{
		if (body.IsInGroup("Player"))
		{
			Controller.SceneGoto(targetScene);

			Player.Main.Position = targetPosition;
			switch (targetDirection)
			{
				case Direction.Up:
				{
					break;
				}
				case Direction.Down:
				{
					break;
				}

				case Direction.Left:
				{
					break;
				}

				case Direction.Right:
				{
					break;
				}
			}
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
