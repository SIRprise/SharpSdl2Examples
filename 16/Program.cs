using System;
using System.Globalization;
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

        //Globally used font
        public static IntPtr Font = IntPtr.Zero;

        //Rendered texture
        private static readonly LTexture _TextTexture = new LTexture();

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


        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Open the font
            Font = SDL_ttf.TTF_OpenFont("lazy.ttf", 28);
            if (Font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load lazy font! SDL_ttf Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                //Render text
                var textColor = new SDL.SDL_Color();
                if (!_TextTexture.LoadFromRenderedText("The quick brown fox jumps over the lazy dog", textColor))
                {
                    Console.WriteLine("Failed to render text texture!");
                    success = false;
                }
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded images
            _TextTexture.Free();

            //Free global font
            SDL_ttf.TTF_CloseFont(Font);
            Font = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;

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

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(Renderer);

                        //Render current frame
                        _TextTexture.Render((SCREEN_WIDTH - _TextTexture.GetWidth()) / 2, (SCREEN_HEIGHT - _TextTexture.GetHeight()) / 2);

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