using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

public partial class Main : Node
{
	private Texture wallTexture;
	private MeshInstance3D wallInstance;
	private Control[] panelControls;
	private Node3D modelContainers;
	private Node3D mainModel;
	private Process cmdApp;
	private AudioStreamPlayer3D voicePlayer;
	private AudioStreamPlayer3D musicPlayer;
	public ShaderMaterial shader;
	private double deltaTime;
	private const float PromptRefreshTime = 5f;
	private string[] options;
	private int oldID = -1;
	private double shaderTime;
	private int displayIndex;
	private bool clickedButton = false;

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
		// string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "DataBridge.json" };
		// return Path.Combine(pathPieces);
		return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/DataBridge.json";
	}

	private string GetPromptBridgePath()
	{
		// string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "PromptBridge.json" };
		// return Path.Combine(pathPieces);
		return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/PromptBridge.json";
	}

	private string GetGeneratedImagePath()
	{
		// string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "image.png" };
		// return Path.Combine(pathPieces);
		return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/backend/image.png";
	}

	private string GetGeneratedVoicePath()
	{
		// string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "voice.wav" };
		// return Path.Combine(pathPieces);
		return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/DataBridge/voice.wav";
	}

	private string GetBackendPath()
	{
		// string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "backend", "Meditation.py" };
		// return Path.Combine(pathPieces);
		return "/Users/cool/Documents/GitHub/Meditation_HACKUSF/backend/Meditation.py";
	}

	public override void _Ready()
	{
		//string strCmdText;
		//strCmdText = "C:\\Contents\\Projects\\Hackathons\\Meditation\\venv\\Scripts\\python.exe C:/Contents/Projects/Hackathons/Meditation/backend/Meditation.py";
		//cmdApp = Process.Start("CMD.exe", strCmdText);
		voicePlayer = GetNode<AudioStreamPlayer3D>("VoicePlayer");
		musicPlayer = GetNode<AudioStreamPlayer3D>("MusicPlayer");
		panelControls = new Control[3] { GetNode("MainUI/UI/MainUIControl/TwoPanel") as Control, GetNode("MainUI/UI/MainUIControl/ThreePanel") as Control, GetNode("MainUI/UI/MainUIControl/FourPanel") as Control };
		modelContainers = GetNode<Node3D>("ModelContainers");
		mainModel = GetNode<Node3D>("LowPolyModel");
		options = new string[4];
	}

	public override void _Process(double delta)
	{
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
			GetChild<MeshInstance3D>(4).SetSurfaceOverrideMaterial(0, newTextureMaterial);
			(musicPlayer.GetChild(intensity - 1) as AudioStreamPlayer).Play();
			voicePlayer.Stream = AudioStreamWav.LoadFromFile(GetGeneratedVoicePath());
			voicePlayer.Play();
			mainModel = InstanceFromId(modelContainers.GetChild(Random.Shared.Next(0, 3 + 1)).GetInstanceId()) as Node3D;
			clickedButton = false;
		}
		if (!voicePlayer.Playing && !clickedButton)
			panelControls[displayIndex].Visible = true;
	}

	private void SetUpPromptBridge(string option)
	{
		for (int i = 0; i < panelControls.Length; i++)
			panelControls[i].Visible = false;
		
		Dictionary<string, object> jSonWritesDict = new Dictionary<string, object>();
		jSonWritesDict.Add("prompt", option);
		jSonWritesDict.Add("id", Random.Shared.Next(0, 9999 + 1));
		File.WriteAllText(GetPromptBridgePath(), JObject.FromObject(jSonWritesDict).ToString());
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
