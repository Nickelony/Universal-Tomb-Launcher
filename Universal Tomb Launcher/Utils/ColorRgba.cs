using System;

namespace UniversalTombLauncher.Utils
{
	public readonly struct ColorRgba
	{
		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }

		public ColorRgba(byte r, byte g, byte b, byte a = 255)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public static ColorRgba FromHex(string? hex, byte alpha = 255)
		{
			if (string.IsNullOrWhiteSpace(hex))
				return new ColorRgba(255, 255, 255, alpha);

			hex = hex.Trim().TrimStart('#');

			try
			{
				if (hex.Length == 6)
				{
					byte r = Convert.ToByte(hex.Substring(0, 2), 16);
					byte g = Convert.ToByte(hex.Substring(2, 2), 16);
					byte b = Convert.ToByte(hex.Substring(4, 2), 16);
					return new ColorRgba(r, g, b, alpha);
				}
				else if (hex.Length == 8)
				{
					byte a = Convert.ToByte(hex.Substring(0, 2), 16);
					byte r = Convert.ToByte(hex.Substring(2, 2), 16);
					byte g = Convert.ToByte(hex.Substring(4, 2), 16);
					byte b = Convert.ToByte(hex.Substring(6, 2), 16);
					return new ColorRgba(r, g, b, a);
				}
				else if (hex.Length == 3)
				{
					byte r = Convert.ToByte(new string(hex[0], 2), 16);
					byte g = Convert.ToByte(new string(hex[1], 2), 16);
					byte b = Convert.ToByte(new string(hex[2], 2), 16);
					return new ColorRgba(r, g, b, alpha);
				}
			}
			catch
			{
				// Fallback to white on format error
			}

			return new ColorRgba(255, 255, 255, alpha);
		}
	}
}
