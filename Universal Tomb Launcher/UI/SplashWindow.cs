using System;
using System.Diagnostics;
using System.IO;
using SDL;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.UI
{
	/// <summary>
	/// Cross-platform splash screen implemented using SDL3 and SDL3_ttf.
	/// </summary>
	internal static class SplashWindow
	{
		private static readonly int[] ValidSplashWidths = { 1024, 768, 512, 384 };
		private static readonly int[] ValidSplashHeights = { 512, 384, 256 };

		/// <summary>
		/// Displays the splash window until timeout or user interaction.
		/// </summary>
		public static unsafe SplashResult Show(bool isPreviewMode = false, string? overrideMessage = null)
		{
			var config = new Configuration().Load();

			string splashImagePath = Directory.Exists("Engine")
				? Path.Combine("Engine", "splash.bmp")
				: "splash.bmp";

			if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS))
				return SplashResult.Launch;

			bool ttfInitialized = SDL3_ttf.TTF_Init();

			SDL_Window* window = null;
			SDL_Renderer* renderer = null;
			SDL_Texture* imageTexture = null;
			SDL_Texture* textTexture = null;
			TTF_Font* font = null;

			try
			{
				bool hasImage = false;
				int imageWidth = 0;
				int imageHeight = 0;

				if (File.Exists(splashImagePath))
				{
					SDL_Surface* splashSurface = SDL3.SDL_LoadBMP(splashImagePath);
					if (splashSurface != null)
					{
						bool validWidth = Array.Exists(ValidSplashWidths, w => w == splashSurface->w);
						bool validHeight = Array.Exists(ValidSplashHeights, h => h == splashSurface->h);

						if (validWidth && validHeight)
						{
							imageWidth = splashSurface->w;
							imageHeight = splashSurface->h;
							hasImage = true;
						}
						else
						{
							SDL3.SDL_DestroySurface(splashSurface);
							splashSurface = null;
						}
					}

					// Surface will be converted to texture once renderer is created
				}

				int topBarHeight = hasImage ? 26 : 0;
				int bottomBarHeight = 75;
				int totalWidth = hasImage ? imageWidth : 384;
				int totalHeight = topBarHeight + (hasImage ? imageHeight : 0) + bottomBarHeight;

				var winFlags = SDL_WindowFlags.SDL_WINDOW_BORDERLESS | SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP;
				window = SDL3.SDL_CreateWindow("Universal Tomb Launcher", totalWidth, totalHeight, winFlags);
				if (window == null)
					return SplashResult.Launch;

				SDL3.SDL_SetWindowPosition(window, (int)SDL3.SDL_WINDOWPOS_CENTERED, (int)SDL3.SDL_WINDOWPOS_CENTERED);
				SDL3.SDL_SetWindowOpacity(window, 0.0f);

				renderer = SDL3.SDL_CreateRenderer(window, (byte*)null);
				if (renderer == null)
					return SplashResult.Launch;

				SDL3.SDL_SetRenderDrawBlendMode(renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

				// Create image texture if available
				if (hasImage && File.Exists(splashImagePath))
				{
					SDL_Surface* surf = SDL3.SDL_LoadBMP(splashImagePath);
					if (surf != null)
					{
						imageTexture = SDL3.SDL_CreateTextureFromSurface(renderer, surf);
						SDL3.SDL_DestroySurface(surf);
					}
				}

				// Determine message text
				string messageText = isPreviewMode
					? "Press ESC to EXIT preview mode..."
					: overrideMessage ?? "Press CTRL to show the SETUP dialog...";

				// Font loading and text texture creation
				int textW = 0;
				int textH = 0;
				ColorRgba fontColor = ColorRgba.FromHex(config.FontColor);

				if (ttfInitialized)
				{
					string? fontPath = FontHelper.FindSystemFontPath();
					if (fontPath != null)
					{
						font = SDL3_ttf.TTF_OpenFont(fontPath, 20.0f);
						if (font != null)
						{
							var sdlColor = new SDL_Color
							{
								r = fontColor.R,
								g = fontColor.G,
								b = fontColor.B,
								a = fontColor.A
							};

							SDL_Surface* textSurface = SDL3_ttf.TTF_RenderText_Blended(font, messageText, UIntPtr.Zero, sdlColor);
							if (textSurface != null)
							{
								textW = textSurface->w;
								textH = textSurface->h;
								textTexture = SDL3.SDL_CreateTextureFromSurface(renderer, textSurface);
								SDL3.SDL_DestroySurface(textSurface);
							}
						}
					}
				}

				// Parse colors for top and bottom gradient bars
				ColorRgba topStart = ColorRgba.FromHex(config.TopBar_GradientStartColor, (byte)config.TopBar_GradientStartAlpha);
				ColorRgba topEnd = ColorRgba.FromHex(config.TopBar_GradientEndColor, (byte)config.TopBar_GradientEndAlpha);
				ColorRgba bottomStart = ColorRgba.FromHex(config.BottomBar_GradientStartColor, (byte)config.BottomBar_GradientStartAlpha);
				ColorRgba bottomEnd = ColorRgba.FromHex(config.BottomBar_GradientEndColor, (byte)config.BottomBar_GradientEndAlpha);

				float currentOpacity = 0.0f;
				long startTimestamp = Stopwatch.GetTimestamp();

				// Event and animation loop
				while (true)
				{
					SDL_Event e;
					while (SDL3.SDL_PollEvent(&e))
					{
						if (e.type == (uint)SDL_EventType.SDL_EVENT_KEY_DOWN)
						{
							if (!isPreviewMode && (e.key.key == SDL_Keycode.SDLK_LCTRL || e.key.key == SDL_Keycode.SDLK_RCTRL))
								return SplashResult.Setup;

							if (isPreviewMode && e.key.key == SDL_Keycode.SDLK_ESCAPE)
								return SplashResult.Exit;
						}
						else if (e.type == (uint)SDL_EventType.SDL_EVENT_QUIT)
						{
							return SplashResult.Exit;
						}
					}

					// Update fade-in animation
					if (currentOpacity < 1.0f)
					{
						currentOpacity = Math.Min(1.0f, currentOpacity + 0.1f);
						SDL3.SDL_SetWindowOpacity(window, currentOpacity);
					}

					// Check timeout
					if (!isPreviewMode)
					{
						double elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
						if (elapsedMs >= config.DisplayTimeMilliseconds)
							return SplashResult.Launch;
					}

					// Render frame
					SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
					SDL3.SDL_RenderClear(renderer);

					// Render splash image
					if (hasImage && imageTexture != null)
					{
						var imgRect = new SDL_FRect
						{
							x = 0,
							y = topBarHeight,
							w = totalWidth,
							h = imageHeight
						};
						SDL3.SDL_RenderTexture(renderer, imageTexture, null, &imgRect);
					}

					// Render top gradient bar
					if (topBarHeight > 0)
					{
						DrawGradient(renderer, 0, 0, totalWidth, topBarHeight, topStart, topEnd, config.TopBar_GradientFlow);
					}

					// Render bottom gradient bar
					float bottomY = topBarHeight + (hasImage ? imageHeight : 0);
					DrawGradient(renderer, 0, bottomY, totalWidth, bottomBarHeight, bottomStart, bottomEnd, config.BottomBar_GradientFlow);

					// Render text
					if (textTexture != null)
					{
						float textX = (totalWidth - textW) / 2.0f;
						float textY = bottomY + (bottomBarHeight - textH) / 2.0f;
						var textRect = new SDL_FRect
						{
							x = textX,
							y = textY,
							w = textW,
							h = textH
						};
						SDL3.SDL_RenderTexture(renderer, textTexture, null, &textRect);
					}
					else
					{
						// Fallback: built-in SDL debug text
						float debugX = Math.Max(10.0f, (totalWidth - (messageText.Length * 8)) / 2.0f);
						float debugY = bottomY + (bottomBarHeight - 16) / 2.0f;
						SDL3.SDL_SetRenderScale(renderer, 1.0f, 1.0f);
						SDL3.SDL_SetRenderDrawColor(renderer, fontColor.R, fontColor.G, fontColor.B, fontColor.A);
						SDL3.SDL_RenderDebugText(renderer, debugX, debugY, messageText);
					}

					SDL3.SDL_RenderPresent(renderer);
					SDL3.SDL_Delay(16); // Target ~60 FPS
				}
			}
			finally
			{
				if (textTexture != null)
					SDL3.SDL_DestroyTexture(textTexture);

				if (imageTexture != null)
					SDL3.SDL_DestroyTexture(imageTexture);

				if (font != null)
					SDL3_ttf.TTF_CloseFont(font);

				if (renderer != null)
					SDL3.SDL_DestroyRenderer(renderer);

				if (window != null)
					SDL3.SDL_DestroyWindow(window);

				if (ttfInitialized)
					SDL3_ttf.TTF_Quit();

				SDL3.SDL_Quit();
			}
		}

		/// <summary>
		/// Draws a hardware-accelerated linear gradient rectangle using SDL_RenderGeometry.
		/// </summary>
		private static unsafe void DrawGradient(
			SDL_Renderer* renderer,
			float x,
			float y,
			float width,
			float height,
			ColorRgba startColor,
			ColorRgba endColor,
			GradientFlow flow)
		{
			var cStart = new SDL_FColor
			{
				r = startColor.R / 255.0f,
				g = startColor.G / 255.0f,
				b = startColor.B / 255.0f,
				a = startColor.A / 255.0f
			};

			var cEnd = new SDL_FColor
			{
				r = endColor.R / 255.0f,
				g = endColor.G / 255.0f,
				b = endColor.B / 255.0f,
				a = endColor.A / 255.0f
			};

			SDL_FColor cTopLeft, cTopRight, cBottomRight, cBottomLeft;

			switch (flow)
			{
				case GradientFlow.BottomToTop:
					cTopLeft = cEnd;
					cTopRight = cEnd;
					cBottomRight = cStart;
					cBottomLeft = cStart;
					break;

				case GradientFlow.LeftToRight:
					cTopLeft = cStart;
					cTopRight = cEnd;
					cBottomRight = cEnd;
					cBottomLeft = cStart;
					break;

				case GradientFlow.RightToLeft:
					cTopLeft = cEnd;
					cTopRight = cStart;
					cBottomRight = cStart;
					cBottomLeft = cEnd;
					break;

				case GradientFlow.TopToBottom:
				default:
					cTopLeft = cStart;
					cTopRight = cStart;
					cBottomRight = cEnd;
					cBottomLeft = cEnd;
					break;
			}

			var vertices = stackalloc SDL_Vertex[4];
			vertices[0] = new SDL_Vertex { position = new SDL_FPoint { x = x, y = y }, color = cTopLeft };
			vertices[1] = new SDL_Vertex { position = new SDL_FPoint { x = x + width, y = y }, color = cTopRight };
			vertices[2] = new SDL_Vertex { position = new SDL_FPoint { x = x + width, y = y + height }, color = cBottomRight };
			vertices[3] = new SDL_Vertex { position = new SDL_FPoint { x = x, y = y + height }, color = cBottomLeft };

			var indices = stackalloc int[6] { 0, 1, 2, 0, 2, 3 };

			SDL3.SDL_RenderGeometry(renderer, null, vertices, 4, indices, 6);
		}
	}
}
