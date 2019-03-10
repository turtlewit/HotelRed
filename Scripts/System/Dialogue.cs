using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Dialogue : Node2D
{
	[Signal]
	public delegate void text_ended();

	// ================================================================

	[Export]
	private DynamicFont font;

	// ================================================================

	// Text storage
    private List<string> text = new List<string>();
	private int textPage = 0;

	// File info
	private string sourceFile;
	private int textSet;

	// Display info
	private int textSize = 16;
	//private int charSpacing = 10;

	private int disp = -1;
	private bool roll = false;
	private float interval = 0.03f;

	private bool secondClient = false;
	private string leftClientName;
	private string rightClientName;
	private Color leftClientColor;
	private Color rightClientColor;
	private SpriteFrames leftClientPortrait;
	private SpriteFrames rightClientPortrait;
	private bool talkSide = false;

	// Input
	private bool allowAdvance = false;
	private bool buffer = false;

	private bool started = false;
	private bool finished = false;

	// Timing
	private float t = 0f;
	private bool pause = false;

	private Node2D client;

	// Constants
	private const int LineEnd = 52;
	private const int LineSpacing = 32;

	private const int TextLeft = 168;
	private const int TextTop = 266;
	private const int NametagTop = 248;


	private Regex wordRegex = new Regex(@"\b\w*\b");

	//private DynamicFont font = GD.Load("res://Fonts/FontDialogue.tres") as DynamicFont;

	// Modifiers
	private enum Modifier {NORMAL, RED, GREEN, BLUE, SHAKE, WAVE};

	// Refs
	private AnimatedSprite Indicator;
	private AnimatedSprite PortraitSpriteLeft;
	private AnimatedSprite PortraitSpriteRight;
	private Sprite PortraitFrameLeft;
	private Sprite PortraitFrameRight;

	private AudioStreamPlayer SoundStart;
	private AudioStreamPlayer SoundType;
	private AudioStreamPlayer SoundAdvance;
	private AudioStreamPlayer SoundEnd;

	private Timer TimerStart;
	private Timer TimerRollText;
	private Timer TimerSound;
	private Timer TimerWaitShort;
	private Timer TimerWaitLong;
	private Timer TimerBuffer;
	private Timer TimerEnd;

	// ================================================================

	public bool SecondClient { set { secondClient = value; } }
	public string LeftClientName { set { leftClientName = value; } }
	public string RightClientName { set { rightClientName = value; } }
	public Color LeftClientColor { set { leftClientColor = value; } }
	public Color RightClientColor { set { rightClientColor = value; } }
	public SpriteFrames LeftClientPortrait { set { leftClientPortrait = value; } }
	public SpriteFrames RightClientPortrait { set { rightClientPortrait = value; } }
	public string SourceFile { set { sourceFile = value;} }
	public int TextSet { set { textSet = value; } }
	public Node2D Client { set { client = value; } }
	public bool Buffer { set { buffer = value; } }

	// ================================================================

    public override void _Ready()
    {
        // Refs
		Indicator = GetNode<AnimatedSprite>("CanvasLayer/Indicator");
		PortraitSpriteLeft = GetNode<AnimatedSprite>("CanvasLayer/BorderLeft/PortraitLeft");
		PortraitSpriteRight = GetNode<AnimatedSprite>("CanvasLayer/BorderRight/PortraitRight");
		PortraitFrameLeft = GetNode<Sprite>("CanvasLayer/BorderLeft");
		PortraitFrameRight = GetNode<Sprite>("CanvasLayer/BorderRight");

		SoundStart = GetNode<AudioStreamPlayer>("SoundStart");
		SoundStart = GetNode<AudioStreamPlayer>("SoundType");
		SoundStart = GetNode<AudioStreamPlayer>("SoundAdvance");
		SoundStart = GetNode<AudioStreamPlayer>("SoundEnd");

		TimerStart = GetNode<Timer>("TimerStart");
		TimerRollText = GetNode<Timer>("TimerRollText");
		TimerSound = GetNode<Timer>("TimerSound");
		TimerWaitShort = GetNode<Timer>("TimerWaitShort");
		TimerWaitLong = GetNode<Timer>("TimerWaitLong");
		TimerBuffer = GetNode<Timer>("TimerBuffer");
		TimerEnd = GetNode<Timer>("TimerEnd");

		// Setup
		Indicator.Hide();
		SoundStart.Play();

    }


	public override void _Process(float delta)
	{
		t += 60f * delta;

		// stuff

		Update();
	}


	public override void _Draw()
	{
		if (disp >= 0)
		{
			// Draw nametag
			DrawString(font, new Vector2(TextLeft, NametagTop), talkSide ? rightClientName : leftClientName, talkSide ? rightClientColor : leftClientColor);

			// Draw dialogue text
			Modifier modifier = Modifier.NORMAL;

			int i = 0;
			int space = 0;
			int length = 0;
			int line = 0;
			float charSpacing = 0f;

			while (i < disp + 1)
			{
				// Skip spaces
				if (text[textPage][i] != ' ')
					length++;

				// Line breaks
				//GD.Print(i - 1);
				if (i > 0 && text[textPage][i - 1] == ' ' && space + length + wordRegex.Match(text[textPage], space).Length > LineEnd)
				{
					space = 0;
					length = 0;
					line++;
				}
				
				// Escape tokens
				switch (text[textPage][i])
				{
					case '#':
					{
						space = 0;
						length = 0;
						line++;
						break;
					}

					case '|':
					{
						if (!pause)
						{
							// stuff
						}

						break;
					}

					case '{':
					{
						if (!pause)
						{
							// stuff
						}

						break;
					}

					case '$':
					{
						switch (text[textPage][i + 1])
						{
							case '0':
								modifier = Modifier.NORMAL;
								break;
							case 'r':
								modifier = Modifier.RED;
								break;
							case 'g':
								modifier = Modifier.GREEN;
								break;
							case 's':
								modifier = Modifier.SHAKE;
								break;
							case 'w':
								modifier = Modifier.WAVE;
								break;
							default:
								modifier = Modifier.NORMAL;
								break;
						}

						i++;
						space--;
						break;
					}

					default:
					{
						font.Size = textSize;
						switch (modifier)
						{
							case Modifier.NORMAL:
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(),"", new Color(1f, 1f, 1f));
								break;
							case Modifier.RED:
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), "", new Color(1f, 0, 0));
								break;
							case Modifier.BLUE:
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), "", new Color(0, 1f, 1f));
								break;
							case Modifier.SHAKE:
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing + Mathf.RoundToInt((float)GD.RandRange(-1d, 1d)), TextTop + (LineSpacing * line) + Mathf.RoundToInt((float)GD.RandRange(-1d, 1d))), text[textPage][i].ToString(), "", new Color(1f, 1f, 1f));
								break;
							case Modifier.WAVE:
							{
								float so = (2f * t) + (i * 3);
								double shift = Math.Sin(so * Math.PI * (1f / 60f)) * 3f;
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing, TextTop + (LineSpacing * line) + (float)shift), text[textPage][i].ToString(), "", new Color(1f, 1f, 1f));
								break;
							}

							default:
								charSpacing += DrawChar(font, new Vector2(TextLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), "", new Color(1f, 1f, 1f));
								break;
						}

						break;
					}
				}

				i++;
				space++;
			}
		}
	}

	// ================================================================

	private void Start()
	{
		// Load text
		LoadTextFromFile(sourceFile);
		textPage = 0;
		disp = 0;

		// Set up portraits
		PortraitSpriteLeft.Frames = leftClientPortrait;

		if (rightClientPortrait != null)
			PortraitSpriteRight.Frames = rightClientPortrait;
		else
		{
			PortraitFrameRight.Hide();
			PortraitSpriteRight.Hide();
		}

		font.Size = textSize;
		TimerRollText.SetWaitTime(interval);
		TimerRollText.Start();
		PlayTextSound();
		TimerSound.Start();
		started = true;
	}


	private void LoadTextFromFile(string path)
	{
		File file = new File();

		try
		{
			file.Open(path, (int)File.ModeFlags.Read);
			int currentIndex = -1;
			//var currentSet = new List<string>();
			bool read = false;

			while (!file.EofReached())
			{
				string line = file.GetLine();
				if (line[0] == '[')
					currentIndex = line[1].ToString().ToInt();
				
				if (line[0] == '}' && read)
				{
					read = false;
					break;
				}

				if (read)
					text.Add(line.StripEdges());
				
				if (line[0] == '{' && currentIndex == textSet)
					read = true;
			}
		}
		finally
		{
			file.Close();
		}
	}


	private void RollText()
	{
		disp++;
		if (disp >= text[textPage].Length - 1)
		{
			roll = false;
			// indicator
			TimerRollText.Stop();
			TimerSound.Stop();
			// client set talking
			allowAdvance = true;
		}
	}


	private void PlayTextSound()
	{

	}


	private void AdvancePage()
	{
		if (textPage + 1 < text.Count)
		{
			SoundAdvance.Play();
			Indicator.Hide();
			textPage++;
			disp = -1;
			// client set talking
			TimerRollText.SetWaitTime(interval);
			TimerRollText.Start();
			PlayTextSound();
			TimerSound.Start();
			allowAdvance = false;
			roll = true;
		}
		else
		{
			SoundEnd.Play();
			Indicator.Hide();
			// AnimationPlayer stuff
			finished = true;
			TimerEnd.Start();
		}
	}


	private void ResumeText()
	{
		TimerRollText.Start();
		PlayTextSound();
		TimerSound.Start();
		pause = false;
		roll = true;
	}


	private void ResetBuffer()
	{
		buffer = false;
	}


	private void Finish()
	{
		Controller.Main.KeyboardLock = Controller.KB.MOVE;
		// client interact sprite
		EmitSignal("text_ended");
	}
}
