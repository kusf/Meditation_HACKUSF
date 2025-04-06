using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public partial class Main : Node
{
	private Texture wallTexture;
	private MeshInstance3D wallInstance;
	private double deltaTime;
	private const float PromptRefreshTime = 5f;
	private string[] options;

	public override void _Ready()
	{
		string pngDir = "C:\\Contents\\Projects\\Hackathons\\Meditation\\frontend\\Images\\wallTexture.png";
		Image image = Image.LoadFromFile(pngDir);
		StandardMaterial3D newTextureMaterial = new StandardMaterial3D();
		newTextureMaterial.AlbedoTexture = ImageTexture.CreateFromImage(image);
		GetChild<MeshInstance3D>(4).SetSurfaceOverrideMaterial(0, newTextureMaterial);
		options = new string[4];
	}

	public override void _Process(double delta)
	{
		deltaTime += delta;
		if (deltaTime >= PromptRefreshTime)
		{
			//run first prompt
			//Json.ParseString(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\DataBridge\\DataBridge.json"));

			JObject jobject = JObject.Parse(File.ReadAllText("C:\\Contents\\Projects\\Hackathons\\Meditation\\DataBridge\\DataBridge.json"));
			dynamic dynamicObj = JsonConvert.DeserializeObject(jobject.ToString());
			if (dynamicObj != null)
			{
				string promptText = dynamicObj["text"];
				options = dynamicObj["options"];
			}
			deltaTime -= PromptRefreshTime;
		}
	}

	private void SetUpPromptBridge(string option)
	{
		Dictionary<string, object> jSonWritesDict = new Dictionary<string, object>();
		jSonWritesDict.Add("prompt", option);
		jSonWritesDict.Add("id", Random.Shared.Next(0, 9999 + 1));
		File.WriteAllText("C:\\Contents\\Projects\\Hackathons\\Meditation\\DataBridge\\DataBridge.json", JObject.FromObject(jSonWritesDict).ToString());
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
