using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

public partial class Main : Node
{
	private Texture wallTexture;
	private MeshInstance3D wallInstance;
	private Control[] panelControls;
	private Node3D modelsContainer;
	private Process cmdApp;
	private AudioStreamPlayer3D voicePlayer;
	private Node musicPlayers;
	public ShaderMaterial shader;
	public ColorRect fadeObj;
	private double deltaTime;
	private const float PromptRefreshTime = 5f;
	private string[] options;
	private int oldID = -1;
	private double shaderTime;
	private int displayIndex;
	private bool clickedButton = true;
	private FadeState fadeState;
	private double fadeTimer = 0;
	private const float FadeTime = 5f;

	private enum FadeState
	{
		None,
		FadeIn,		//frame comes in
		FadeOut		//frame goes to black
	}

	private string[] bones = new string[17] {
		"nose",
		"left_shoulder",
		"right_shoulder",
		"left_elbow",
		"right_elbow",
		"left_wrist",
		"right_wrist",
		"left_index",
		"right_index",
		"left_hip",
		"right_hip",
		"left_knee",
		"right_knee",
		"left_ankle",
		"right_ankle",
		"left_foot_index",
		"right_foot_index"
	};

	private string GetDataBridgePath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/DataBridge.json";

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "DataBridge.json" };
		return Path.Combine(pathPieces);
	}

	private string GetPromptBridgePath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/PromptBridge.json";

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "PromptBridge.json" };
		return Path.Combine(pathPieces);
	}

	private string GetGeneratedImagePath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/backend/image.png";

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "image.png" };
		return Path.Combine(pathPieces);
	}

	private string GetGeneratedVoicePath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/voice.wav";

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "voice.wav" };
		return Path.Combine(pathPieces);
	}

	private string GetBackendPath()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/backend/Meditation.py";

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "backend", "Meditation.py" };
		return Path.Combine(pathPieces);
	}

	public override void _Ready()
	{
		//string strCmdText;
		//strCmdText = GetBackendPath();
		//cmdApp = Process.Start("CMD.exe", strCmdText);

		voicePlayer = GetNode<AudioStreamPlayer3D>("VoicePlayer");
		musicPlayers = GetNode<Node>("MusicPlayers");
		panelControls = new Control[3] { GetNode("MainUI/UI/MainUIControl/TwoPanel") as Control, GetNode("MainUI/UI/MainUIControl/ThreePanel") as Control, GetNode("MainUI/UI/MainUIControl/FourPanel") as Control };
		modelsContainer = GetNode<Node3D>("ModelsContainer");
		options = new string[4];
		fadeObj = GetNode<ColorRect>("MainUI/UI/FadeObj");
		shader = GetNode<ColorRect>("ShaderMaterialContainer").Material as ShaderMaterial;
	}

	public override void _Process(double delta)
	{
		shaderTime += delta;
		shader.SetShaderParameter("imageOffset", 24f * (float)Math.Sin(shaderTime));
		shader.SetShaderParameter("imageStretch", 2f * Math.Abs((float)Math.Cos(shaderTime) + 0.05f));
		shader.SetShaderParameter("magnifier", ((float)Math.Sin(shaderTime) / 2f) + 1f);
		if (shaderTime >= 360f)
			shaderTime -= 360f;
		
		deltaTime += delta;
		if (deltaTime >= PromptRefreshTime)
		{
			//run first prompt
			//Json.ParseString(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "", "DataBridge", "DataBridge.json"));
			deltaTime -= PromptRefreshTime;
			JObject dataBridgeData = JObject.Parse(File.ReadAllText(GetDataBridgePath()));
			dynamic dataBridgeObj = JsonConvert.DeserializeObject(dataBridgeData.ToString());
			if (dataBridgeObj == null)
				return;
			if (dataBridgeObj["id"] == oldID)      //error checks
			{
				GD.Print("Waiting for new JSON...");
				return;
			}

			GD.Print("Reading new JSON");
			oldID = dataBridgeObj["id"];
			string promptText = dataBridgeObj["text"];     //data extraction
			int intensity = dataBridgeObj["intensity"];
			displayIndex = 0;
			for (int i = 0; i < 4; i++)
			{
				try
				{
					options[i] = dataBridgeObj["options"][i];
					displayIndex++;
				}
				catch (Exception e)
				{
					displayIndex = i - 1;
					break;
				}
			}
			displayIndex = Math.Clamp(displayIndex, 0, panelControls.Length - 1);
			for (int i = 0; i < panelControls.Length; i++)
				panelControls[i].Visible = false;
			for (int i = 0; i < panelControls[displayIndex].GetChildCount(); i++)
			{
				Button button = panelControls[displayIndex].GetChild(i) as Button;
				button.Text = options[i];
			}
			Image image = Image.LoadFromFile(GetGeneratedImagePath());
			StandardMaterial3D newTextureMaterial = new StandardMaterial3D();
			newTextureMaterial.AlbedoTexture = ImageTexture.CreateFromImage(image);
			newTextureMaterial.NextPass = shader;
			GetNode<MeshInstance3D>("BackgroundWall").MaterialOverlay = newTextureMaterial;
			AudioStreamPlayer ambiencePlayer = musicPlayers.GetChild(intensity - 1) as AudioStreamPlayer;
			ambiencePlayer.VolumeDb = -8f;
			ambiencePlayer.Play();
			voicePlayer.Stream = AudioStreamWav.LoadFromFile(GetGeneratedVoicePath());
			voicePlayer.Play();
			(modelsContainer.GetChild(Random.Shared.Next(0, 3 + 1)) as Node3D).Visible = true;
			clickedButton = false;
			fadeState = FadeState.FadeIn;
		}
		if (!voicePlayer.Playing && !clickedButton)
			panelControls[displayIndex].Visible = true;

		if (fadeState == FadeState.None)
			fadeTimer = 0;
		else if (fadeState == FadeState.FadeIn)
		{
			fadeTimer += delta;
			fadeObj.Color = new Color(fadeObj.Color, 1f - ((float)fadeTimer / FadeTime));
			if (fadeTimer >= FadeTime)
			{
				fadeState = FadeState.None;
				fadeTimer = 0;
			}
		}
		else
		{
			fadeTimer += delta;
			fadeObj.Color = new Color(fadeObj.Color, (float)fadeTimer / FadeTime);
			if (fadeTimer >= FadeTime)
			{
				for (int i = 0; i < modelsContainer.GetChildCount(); i++)
					(modelsContainer.GetChild(i) as Node3D).Visible = false;
				fadeState = FadeState.None;
				fadeTimer = 0;
			}
		}
	}

	private void SetUpPromptBridge(string option)
	{
		for (int i = 0; i < panelControls.Length; i++)
			panelControls[i].Visible = false;
		
		Dictionary<string, object> jSonWritesDict = new Dictionary<string, object>() {
			{ "prompt", option },
			{ "id", Random.Shared.Next(0, 9999 + 1) }
		};
		File.WriteAllText(GetPromptBridgePath(), JObject.FromObject(jSonWritesDict).ToString());
		fadeState = FadeState.FadeOut;
		clickedButton = true;
	}

	private void OnOption1Pressed()
	{
		SetUpPromptBridge(options[0]);
	}

	private void OnOption2Pressed()
	{
		SetUpPromptBridge(options[1]);
	}

	private void OnOption3Pressed()
	{
		SetUpPromptBridge(options[2]);
	}

	private void OnOption4Pressed()
	{
		SetUpPromptBridge(options[3]);
	}
}
