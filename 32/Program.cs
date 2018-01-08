using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace SdlExample
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr gRenderer = IntPtr.Zero;

        //Globally used font
        public static IntPtr gFont = IntPtr.Zero;

        //Scene textures
        private static LTexture gPromptTextTexture = new LTexture();
        private static LTexture gInputTextTexture = new LTexture();


        private static bool init()
        {
            //Initialization flag
            bool success = true;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                {
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");
                }

                //Create window
                gWindow = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                    SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (gWindow == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
                    gRenderer = SDL.SDL_CreateRenderer(gWindow, -1, renderFlags);
                    if (gRenderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);

                        //Initialize PNG loading
                        var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG;
                        if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                        {
                            Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
                            success = false;
                        }

                        //Initialize SDL_ttf
                        if (SDL_ttf.TTF_Init() == -1)
                        {
                            Console.WriteLine("SDL_ttf could not initialize! SDL_ttf Error: {0}", SDL.SDL_GetError());
                            success = false;
                        }
                    }
                }
            }

            return success;
        }


        static bool loadMedia()
        {
            //Loading success flag
            bool success = true;

            //Open the font
            gFont = SDL_ttf.TTF_OpenFont("lazy.ttf", 28);
            if (gFont == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load lazy font! SDL_ttf Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                SDL.SDL_Color textColor = new SDL.SDL_Color { a = 0xFF };
                if (!gPromptTextTexture.loadFromRenderedText("Enter Text:", textColor))
                {
                    Console.WriteLine("Failed to render prompt text!");
                    success = false;
                }
            }

            return success;
        }

        private static void close()
        {
            //Free loaded images
            gPromptTextTexture.free();
            gInputTextTexture.free();

            //Free global font
            SDL_ttf.TTF_CloseFont(gFont);
            gFont = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(gRenderer);
            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;
            gRenderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

        static int Main(string[] args)
        {
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Start up SDL and create window
            var success = init();
            if (success == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                success = loadMedia();
                if (success == false)
                {
                    Console.WriteLine("Failed to load media!");
                }
                else
                {
                    //Main loop flag
                    bool quit = false;

                    //Event handler
                    SDL.SDL_Event e;

                    //Set text color as black
                    SDL.SDL_Color textColor = new SDL.SDL_Color{ a = 255 };

                    //The current input text. 
                    string inputText = "Some text";
                    gInputTextTexture.loadFromRenderedText(inputText, textColor);

                    //Enable text input
                    SDL.SDL_StartTextInput();

                    //While application is running
                    while (!quit)
                    {
                        //The rerender text flag
                        bool renderText = false;

                        //Handle events on queue
                        while (SDL.SDL_PollEvent(out e) != 0)
                        {
                            //User requests quit
                            if (e.type == SDL.SDL_EventType.SDL_QUIT)
                            {
                                quit = true;
                            }

                            //Special key input
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                //Handle backspace
                                if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_BACKSPACE && inputText.Length > 0)
                                {
                                    //lop off character
                                    //inputText.pop_back();
                                    inputText = inputText.Substring(0, inputText.Length - 1);
                                    renderText = true;
                                }
                                //Handle copy
                                else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_c && (SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_CTRL) > 0)
                                {
                                    SDL.SDL_SetClipboardText(inputText);
                                }
                                //Handle paste
                                else if (e.key.keysym.sym == SDL.SDL_Keycode.SDLK_v && (SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_CTRL) > 0)
                                {
                                    inputText = SDL.SDL_GetClipboardText();
                                    renderText = true;
                                }
                            }
                            //Special text input event
                            else if (e.type == SDL.SDL_EventType.SDL_TEXTINPUT)
                            {
                                unsafe
                                {
                                    //Not copy or pasting
                                    if (!((e.text.text[0] == 'c' || e.text.text[0] == 'C') && (e.text.text[0] == 'v' || e.text.text[0] == 'V') &&
                                          (SDL.SDL_GetModState() & SDL.SDL_Keymod.KMOD_CTRL) > 0))
                                    {
                                        //Append character
                                        inputText += (char)*e.text.text;
                                        renderText = true;
                                    }
                                }
                            }
                        }

                        //Rerender text if needed
                        if (renderText)
                        {
                            //Text is not empty
                            if (inputText != "")
                            {
                                //Render new text
                                gInputTextTexture.loadFromRenderedText(inputText, textColor);
                            }
                            //Text is empty
                            else
                            {
                                //Render space texture
                                gInputTextTexture.loadFromRenderedText(" ", textColor);
                            }
                        } 

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Render text textures
                        gPromptTextTexture.render((SCREEN_WIDTH - gPromptTextTexture.getWidth()) / 2, 0);
                        gInputTextTexture.render((SCREEN_WIDTH - gInputTextTexture.getWidth()) / 2, gPromptTextTexture.getHeight());

                        //Update screen
                        SDL.SDL_RenderPresent(gRenderer);
                    }

                    //Disable text input
                    SDL.SDL_StopTextInput();
                }
            }


            //Free resources and close SDL
            close();

            if (success == false)
                Console.ReadLine();

            return 0;
        }








        //Key press surfaces constants
        public enum KeyPressSurfaces
        {
            KEY_PRESS_SURFACE_DEFAULT,
            KEY_PRESS_SURFACE_UP,
            KEY_PRESS_SURFACE_DOWN,
            KEY_PRESS_SURFACE_LEFT,
            KEY_PRESS_SURFACE_RIGHT,
            KEY_PRESS_SURFACE_TOTAL
        };

    }

}
