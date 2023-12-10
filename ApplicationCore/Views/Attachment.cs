﻿using Infrastructure.Views;

namespace ApplicationCore.Views;
public class AttachmentViewModel : BaseRecordView
{
	public int PostId { get; set; }

	public string? PostType { get; set; }

	public string? Type { get; set; }

	public string? Path { get; set; }

	public string? PreviewPath { get; set; }

	public string? Name { get; set; }

	public string? Title { get; set; }


	public int Width { get; set; }

	public int Height { get; set; }


}
