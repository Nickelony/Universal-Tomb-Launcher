using System;
using System.Runtime.InteropServices;

namespace UniversalTombLauncher.Native
{
	internal struct INPUT
	{
		public uint Type;
		public MOUSEKEYBDHARDWAREINPUT Data;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct MOUSEKEYBDHARDWAREINPUT
	{
		[FieldOffset(0)]
		public MOUSEINPUT Mouse;
	}

	internal struct MOUSEINPUT
	{
		public int X;
		public int Y;
		public uint MouseData;
		public uint Flags;
		public uint Time;
		public IntPtr ExtraInfo;
	}

	internal unsafe struct WindowCompositionAttributeData
	{
		public WCA Attribute;
		public void* Data;
		public int DataLength;
	}

	internal struct AccentPolicy
	{
		public ACCENT AccentState;
		public uint AccentFlags;
		public uint GradientColor;
		public uint AnimationId;
	}
}
