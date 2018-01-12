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
        private static IntPtr _Texture = IntPtr.Zero;

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

            //Load stretching surface
            _Texture = LoadTexture("texture.png");
            if (_Texture == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load texture image!");
                success = false;
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded image
            SDL.SDL_DestroyTexture(_Texture);
            _Texture = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(_Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            _Renderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

        private static IntPtr LoadTexture(string path)
        {
            //The final optimized image
            var newTexture = IntPtr.Zero;

            //Load image at specified path
            var loadedSurface = SDL_image.IMG_Load(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
            }
            else
            {
                //Create texture from surface pixels
                newTexture = SDL.SDL_CreateTextureFromSurface(_Renderer, loadedSurface);
                if (newTexture == IntPtr.Zero)
                    Console.WriteLine("Unable to create texture from {0}! SDL Error: {1}", path, SDL.SDL_GetError());

                //Get rid of old loaded surface
                SDL.SDL_FreeSurface(loadedSurface);
            }

            return newTexture;
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
                        SDL.SDL_RenderClear(_Renderer);

                        //Render texture to screen
                        SDL.SDL_RenderCopy(_Renderer, _Texture, IntPtr.Zero, IntPtr.Zero);

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