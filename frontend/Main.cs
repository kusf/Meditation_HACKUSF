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
	private Process cmdApp;
	private double deltaTime;
	private const float PromptRefreshTime = 5f;
	private string[] options;
	private int oldID = -1;

	private string GetDataBridgePath()
	{
		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "DataBridge.json" };
		return Path.Combine(pathPieces);
	}

	private string GetPromptBridgePath()
	{
		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "DataBridge", "PromptBridge.json" };
		return Path.Combine(pathPieces);
	}

	private string GetBackendPath()
	{
		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "backend", "Meditation.py" };
		return Path.Combine(pathPieces);
	}

	public override void _Ready()
	{
		//string strCmdText;
		//strCmdText = "C:\\Contents\\Projects\\Hackathons\\Meditation\\venv\\Scripts\\python.exe C:/Contents/Projects/Hackathons/Meditation/backend/Meditation.py";
		//cmdApp = Process.Start("CMD.exe", strCmdText);

		string[] pathPieces = { "C:", "Contents", "Projects", "Hackathons", "Meditation", "frontend", "Images", "wallTexture.png" };
		string finalPath = Path.Combine(pathPieces);
		Image image = Image.LoadFromFile(finalPath);
		StandardMaterial3D newTextureMaterial = new StandardMaterial3D();
		newTextureMaterial.AlbedoTexture = ImageTexture.CreateFromImage(image);
		GetChild<MeshInstance3D>(4).SetSurfaceOverrideMaterial(0, newTextureMaterial);
		panelControls = new Control[3] { GetNode("MainUI/UI/MainUIControl/TwoPanel") as Control, GetNode("MainUI/UI/MainUIControl/ThreePanel") as Control, GetNode("MainUI/UI/MainUIControl/FourPanel") as Control };
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
			JObject jobject = JObject.Parse(File.ReadAllText(GetPromptBridgePath()));
			dynamic dynamicObj = JsonConvert.DeserializeObject(jobject.ToString());
			if (dynamicObj == null)
				return;
			if (dynamicObj["id"] == oldID)		//error checks
				return;

			oldID = dynamicObj["id"];
			string promptText = dynamicObj["text"];     //data extraction
			int validIndex = 0;
			for (int i = 0; i < 4; i++)
			{
				try
				{
					options[i] = dynamicObj["options"][i];
					validIndex++;
				}
				catch (Exception e)
				{
					validIndex = i - 1;
					break;
				}
			}
			validIndex = Math.Clamp(validIndex, 0, panelControls.Length - 1);
			for (int i = 0; i < panelControls.Length; i++)
				panelControls[i].Visible = false;
			for (int i = 0; i < panelControls[validIndex].GetChildCount(); i++)
			{
				Button button = panelControls[validIndex].GetChild(i) as Button;
				button.Text = options[i];
			}
			panelControls[validIndex].Visible = true;
		}
	}

	public override void _ExitTree()
	{
		//cmdApp.Close();
	}

	private void SetUpPromptBridge(string option)
	{
		for (int i = 0; i < panelControls.Length; i++)
			panelControls[i].Visible = false;
		
		Dictionary<string, object> jSonWritesDict = new Dictionary<string, object>();
		jSonWritesDict.Add("prompt", option);
		jSonWritesDict.Add("id", Random.Shared.Next(0, 9999 + 1));
		File.WriteAllText(GetPromptBridgePath(), JObject.FromObject(jSonWritesDict).ToString());
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
