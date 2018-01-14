using System;
using System.Globalization;
using System.Linq;
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
        private static IntPtr _Window = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr Renderer = IntPtr.Zero;

        //Scene textures
        private static readonly LTexture _PressTexture = new LTexture();

        private static readonly LTexture _UpTexture = new LTexture();
        private static readonly LTexture _DownTexture = new LTexture();
        private static readonly LTexture _LeftTexture = new LTexture();
        private static readonly LTexture _RightTexture = new LTexture();

        private static bool Init()
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
                _Window = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                    SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (_Window == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                    Renderer = SDL.SDL_CreateRenderer(_Window, -1, renderFlags);
                    if (Renderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);

                        //Initialize PNG loading
                        var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG;
                        if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                        {
                            Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
                            success = false;
                        }
                    }
                }
            }

            return success;
        }


        private static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load press texture
            if (!_PressTexture.LoadFromFile("press.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load up texture
            if (!_UpTexture.LoadFromFile("up.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load down texture
            if (!_DownTexture.LoadFromFile("down.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load left texture
            if (!_LeftTexture.LoadFromFile("left.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load right texture
            if (!_RightTexture.LoadFromFile("right.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            return success;
        }


        private static void Close()
        {
            //Free loaded images
            _PressTexture.Free();
            _UpTexture.Free();
            _DownTexture.Free();
            _LeftTexture.Free();
            _RightTexture.Free();

            //Destroy window
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

        static int Main(string[] args)
        {
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Start up SDL and create window
            var success = Init();
            if (success == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                success = LoadMedia();
                if (success == false)
                {
                    Console.WriteLine("Failed to load media!");
                }
                else
                {
                    //Main loop flag
                    bool quit = false;

                    //While application is running
                    while (!quit)
                    {
                        //Event handler
                        SDL.SDL_Event e;

                        //Handle events on queue
                        while (SDL.SDL_PollEvent(out e) != 0)
                        {
                            //User requests quit
                            if (e.type == SDL.SDL_EventType.SDL_QUIT)
                                quit = true;
                        }

                        //Set texture based on current keystate
                        int size;
                        var currentKeyStatesPtr = SDL.SDL_GetKeyboardState(out size);
                        var currentKeyStates = new byte[size];
                        Marshal.Copy(currentKeyStatesPtr, currentKeyStates, 0, currentKeyStates.Length);

                        if (currentKeyStates.Any(a => a != 0))
                        {
                            Console.Write("KeyState arrays: ");

                            int index = -1;
                            foreach (var keyState in currentKeyStates)
                            {
                                index++;
                                if (keyState == 0)
                                    continue;

                                Console.Write("{0};", (SDL.SDL_Scancode)index);
                            }

                            Console.WriteLine();
                        }

                        //Current rendered texture
                        LTexture currentTexture;

                        if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] != 0)
                        {
                            currentTexture = _UpTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] != 0)
                        {
                            currentTexture = _DownTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] != 0)
                        {
                            currentTexture = _LeftTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] != 0)
                        {
                            currentTexture = _RightTexture;
                        }
                        else
                        {
                            currentTexture = _PressTexture;
                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(Renderer);

                        //Render current texture
                        currentTexture.Render(0, 0);

                        //Update screen
                        SDL.SDL_RenderPresent(Renderer);
                    }
                }
            }


            //Free resources and close SDL
            Close();

            if (success == false)
                Console.ReadLine();

            return 0;
        }
    }
}