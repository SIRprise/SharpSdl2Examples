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

        //Scene sprites
        private static readonly SDL.SDL_Rect[] _SpriteClips = new SDL.SDL_Rect[4];
        private static readonly LTexture _SpriteSheetTexture = new LTexture();

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
                    //Create renderer for window
                    Renderer = SDL.SDL_CreateRenderer(_Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
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

            //Load sprite sheet texture
            if (_SpriteSheetTexture.LoadFromFile("dots.png") == false)
            {
                Console.WriteLine("Failed to load sprite sheet texture!");
                success = false;
            }
            else
            {
                //Set top left sprite
                _SpriteClips[0].x = 0;
                _SpriteClips[0].y = 0;
                _SpriteClips[0].w = 100;
                _SpriteClips[0].h = 100;

                //Set top right sprite
                _SpriteClips[1].x = 100;
                _SpriteClips[1].y = 0;
                _SpriteClips[1].w = 100;
                _SpriteClips[1].h = 100;

                //Set bottom left sprite
                _SpriteClips[2].x = 0;
                _SpriteClips[2].y = 100;
                _SpriteClips[2].w = 100;
                _SpriteClips[2].h = 100;

                //Set bottom right sprite
                _SpriteClips[3].x = 100;
                _SpriteClips[3].y = 100;
                _SpriteClips[3].w = 100;
                _SpriteClips[3].h = 100;
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded images
            _SpriteSheetTexture.Free();

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
            if (Init() == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                if (LoadMedia() == false)
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

                        //Render top left sprite
                        _SpriteSheetTexture.Render(0, 0, _SpriteClips[0]);

                        //Render top right sprite
                        _SpriteSheetTexture.Render(SCREEN_WIDTH - _SpriteClips[1].w, 0, _SpriteClips[1]);

                        //Render bottom left sprite
                        _SpriteSheetTexture.Render(0, SCREEN_HEIGHT - _SpriteClips[2].h, _SpriteClips[2]);

                        //Render bottom right sprite
                        _SpriteSheetTexture.Render(SCREEN_WIDTH - _SpriteClips[3].w, SCREEN_HEIGHT - _SpriteClips[3].h, _SpriteClips[3]);

                        //_SpriteSheetTexture.render(0, 0, null);

                        //Update screen
                        SDL.SDL_RenderPresent(Renderer);
                    }
                }
            }


            //Free resources and close SDL
            Close();

            //Console.ReadLine();
            return 0;
        }
    }
}