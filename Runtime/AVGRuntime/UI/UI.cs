using Godot;

public partial class UI : Control
{
	public TextureRect ChapterNameBack => GetNode<TextureRect>("./ChapterNameBack");

	public string ChapterName
	{
		get => GetNode<Label>("./ChapterNameBack/ChapterName").Text;
		set => GetNode<Label>("./ChapterNameBack/ChapterName").Text = value;
	}

	public RichTextLabel TextBox =>
		GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");

	public string CharacterName
	{
		get =>
			GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox")
				.Text.Trim(['【', '】']);
		set
		{
			var node = GetNode<RichTextLabel>(
				"./TextBoxBack/MarginContainer/MarginContainer2/TextBox"
			);
			if (string.IsNullOrEmpty(value.TrimStart()))
			{
				node.Text = "";
			}
			else
			{
				node.Text = "【" + value + "】";
			}
		}
	}

	public PackedScene Serialize()
	{
		var scene = new PackedScene();
		scene.Pack(this);
		return scene;
	}
}
