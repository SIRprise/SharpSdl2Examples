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
        private static IntPtr _StretchedSurface = IntPtr.Zero;

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

                    SDL.SDL_Rect stretchRect;
                    stretchRect.x = 0;
                    stretchRect.y = 0;
                    stretchRect.w = SCREEN_WIDTH;
                    stretchRect.h = SCREEN_HEIGHT;

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
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                //Select surfaces based on key press
                                switch (e.key.keysym.sym)
                                {
                                    case SDL.SDL_Keycode.SDLK_UP:
                                        stretchRect.h--;
                                        break;

                                    case SDL.SDL_Keycode.SDLK_DOWN:
                                        stretchRect.h++;
                                        break;

                                    case SDL.SDL_Keycode.SDLK_LEFT:
                                        stretchRect.w--;
                                        break;

                                    case SDL.SDL_Keycode.SDLK_RIGHT:
                                        stretchRect.w++;
                                        break;
                                }
                            }
                        }

                        SDL.SDL_BlitScaled(_StretchedSurface, IntPtr.Zero, _ScreenSurface, ref stretchRect);

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

        private static void Close()
        {
            //Free loaded image
            SDL.SDL_FreeSurface(_StretchedSurface);
            _StretchedSurface = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;

            //Quit SDL subsystems
            SDL.SDL_Quit();
        }

        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load stretching surface
            _StretchedSurface = LoadSurface("stretch.bmp");
            if (_StretchedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load stretching image!");
                success = false;
            }

            return success;
        }

        private static IntPtr LoadSurface(string path)
        {
            //The final optimized image
            var optimizedSurface = IntPtr.Zero;
            
            //Load image at specified path
            var loadedSurface = SDL.SDL_LoadBMP(path);
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
                    //Get window surface
                    _ScreenSurface = SDL.SDL_GetWindowSurface(_Window);
                }
            }

            return success;
        }
    }
}
