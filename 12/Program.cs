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

        //Scene texture
        private static readonly LTexture _ModulatedTexture = new LTexture();

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


        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load texture
            if (_ModulatedTexture.LoadFromFile("colors.png") == false)
            {
                Console.WriteLine("Failed to load sprite sheet texture!");
                success = false;
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded images
            _ModulatedTexture.Free();

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

                    //Modulation components
                    byte r = 255;
                    byte g = 255;
                    byte b = 255;

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
                            {
                                quit = true;
                            }
                            //On keypress change rgb values
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                switch (e.key.keysym.sym)
                                {
                                    //Increase red
                                    case SDL.SDL_Keycode.SDLK_q:
                                        r += 32;
                                        break;

                                    //Increase green
                                    case SDL.SDL_Keycode.SDLK_w:
                                        g += 32;
                                        break;

                                    //Increase blue
                                    case SDL.SDL_Keycode.SDLK_e:
                                        b += 32;
                                        break;

                                    //Decrease red
                                    case SDL.SDL_Keycode.SDLK_a:
                                        r -= 32;
                                        break;

                                    //Decrease green
                                    case SDL.SDL_Keycode.SDLK_s:
                                        g -= 32;
                                        break;

                                    //Decrease blue
                                    case SDL.SDL_Keycode.SDLK_d:
                                        b -= 32;
                                        break;
                                }
                            }
                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(Renderer);

                        //Modulate and render texture
                        _ModulatedTexture.SetColor(r, g, b);
                        _ModulatedTexture.Render(0, 0);

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