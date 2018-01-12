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
        private static IntPtr _Renderer = IntPtr.Zero;

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
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");

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
                    _Renderer = SDL.SDL_CreateRenderer(_Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
                    if (_Renderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                    }
                }
            }

            return success;
        }


        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Nothing to load
            return success;
        }

        private static void Close()
        {
            //Destroy window
            SDL.SDL_DestroyRenderer(_Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            _Renderer = IntPtr.Zero;

            //Quit SDL subsystems
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
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(_Renderer);

                        //Render red filled quad
                        var fillRect = new SDL.SDL_Rect { x = SCREEN_WIDTH / 4, y = SCREEN_HEIGHT / 4, w = SCREEN_WIDTH / 2, h = SCREEN_HEIGHT / 2 };
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0xFF, 0x00, 0x00, 0xFF);
                        SDL.SDL_RenderFillRect(_Renderer, ref fillRect);

                        //Render green outlined quad
                        var outlineRect = new SDL.SDL_Rect { x = SCREEN_WIDTH / 6, y = SCREEN_HEIGHT / 6, w = SCREEN_WIDTH * 2 / 3, h = SCREEN_HEIGHT * 2 / 3 };
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0x00, 0xFF, 0x00, 0xFF);
                        SDL.SDL_RenderDrawRect(_Renderer, ref outlineRect);

                        //Draw blue horizontal line
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0x00, 0x00, 0xFF, 0xFF);
                        SDL.SDL_RenderDrawLine(_Renderer, 0, SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT / 2);

                        //Draw vertical line of yellow dots
                        SDL.SDL_SetRenderDrawColor(_Renderer, 0xFF, 0xFF, 0x00, 0xFF);
                        for (int i = 0; i < SCREEN_HEIGHT; i += 4)
                        {
                            SDL.SDL_RenderDrawPoint(_Renderer, SCREEN_WIDTH / 2, i);
                        }

                        //Update screen
                        SDL.SDL_RenderPresent(_Renderer);
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