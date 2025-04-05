using Godot;
using System;
using System.Xml.Linq;

public partial class Main : Node
{
	private Texture wallTexture;
	private MeshInstance3D wallInstance;

	public override void _Ready()
	{
		string pngDir = "C:\\Contents\\Projects\\Hackathons\\Meditation\\frontend\\Images\\wallTexture.png";
		Image image = Image.LoadFromFile(pngDir);
		StandardMaterial3D newTextureMaterial = new StandardMaterial3D();
		newTextureMaterial.AlbedoTexture = ImageTexture.CreateFromImage(image);
		GetChild<MeshInstance3D>(4).SetSurfaceOverrideMaterial(0, newTextureMaterial);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
