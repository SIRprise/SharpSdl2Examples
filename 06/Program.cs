using System;
using System.Globalization;
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
        private static IntPtr _ScreenSurface = IntPtr.Zero;
        private static IntPtr _PngSurface = IntPtr.Zero;

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
                    //Initialize PNG loading
                    var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG;
                    if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                    {
                        Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Get window surface
                        _ScreenSurface = SDL.SDL_GetWindowSurface(_Window);
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
            _PngSurface = LoadSurface("loaded.png");
            if (_PngSurface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load PNG image!");
                success = false;
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded image
            SDL.SDL_FreeSurface(_PngSurface);
            _PngSurface = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

        private static IntPtr LoadSurface(string path)
        {
            //The final optimized image
            var optimizedSurface = IntPtr.Zero;

            //Load image at specified path
            var loadedSurface = SDL_image.IMG_Load(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                return optimizedSurface;
            }

            var s = Marshal.PtrToStructure<SDL.SDL_Surface>(_ScreenSurface);

            //Convert surface to screen format
            optimizedSurface = SDL.SDL_ConvertSurface(loadedSurface, s.format, 0);
            if (optimizedSurface == IntPtr.Zero)
                Console.WriteLine("Unable to optimize image {0}! SDL Error: {1}", path, SDL.SDL_GetError());

            //Get rid of old loaded surface
            SDL.SDL_FreeSurface(loadedSurface);

            return optimizedSurface;
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

                        SDL.SDL_BlitSurface(_PngSurface, IntPtr.Zero, _ScreenSurface, IntPtr.Zero);

                        //Update the surface
                        SDL.SDL_UpdateWindowSurface(_Window);
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
