using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher
{
	public sealed class Configuration
	{
		public ACCENT WindowAccent { get; set; } = ACCENT.ENABLE_ACRYLICBLURBEHIND;

		public GradientFlow TopBar_GradientFlow { get; set; } = GradientFlow.TopToBottom;
		public string TopBar_GradientStartColor { get; set; } = "#404040";
		public int TopBar_GradientStartAlpha { get; set; } = 192;
		public string TopBar_GradientEndColor { get; set; } = "#202020";
		public int TopBar_GradientEndAlpha { get; set; } = 192;

		public GradientFlow BottomBar_GradientFlow { get; set; } = GradientFlow.TopToBottom;
		public string BottomBar_GradientStartColor { get; set; } = "#404040";
		public int BottomBar_GradientStartAlpha { get; set; } = 192;
		public string BottomBar_GradientEndColor { get; set; } = "#202020";
		public int BottomBar_GradientEndAlpha { get; set; } = 192;

		public string FontColor { get; set; } = ColorTranslator.ToHtml(Color.White);

		public int DisplayTimeMilliseconds { get; set; } = 1500;

		public string DefaultPath => Directory.Exists("Engine") ? @"Engine\splash.xml" : "splash.xml";

		public Configuration Load(Stream stream)
			=> new XmlSerializer(GetType()).Deserialize(stream) as Configuration;

		public Configuration Load(string filePath)
		{
			try
			{
				using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					return Load(stream);
			}
			catch
			{
				return new Configuration();
			}
		}

		public Configuration Load()
			=> Load(DefaultPath);
	}
}
