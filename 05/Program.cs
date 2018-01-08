using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _01
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        private static IntPtr gScreenSurface = IntPtr.Zero;
        private static IntPtr gStretchedSurface = IntPtr.Zero;
        private static SDL.SDL_Rect stretchRect = new SDL.SDL_Rect();

        static int Main(string[] args)
        {
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Start up SDL and create window
            if (!init())
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                if (!loadMedia())
                {
                    Console.WriteLine("Failed to load media!");
                }
                else
                {
                    //Main loop flag
                    bool quit = false;

                    //Event handler
                    SDL.SDL_Event e;

                    stretchRect.x = 0;
                    stretchRect.y = 0;
                    stretchRect.w = SCREEN_WIDTH;
                    stretchRect.h = SCREEN_HEIGHT;


                    //While application is running
                    while (!quit)
                    {
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

                        SDL.SDL_BlitScaled(gStretchedSurface, IntPtr.Zero, gScreenSurface, ref stretchRect);

                        //Update the surface
                        SDL.SDL_UpdateWindowSurface(gWindow);
                    }
                }
            }

            //Free resources and close SDL
            close();

            //Console.ReadLine();
            return 0;
        }

        private static void close()
        {
            //Free loaded image
            SDL.SDL_FreeSurface(gStretchedSurface);
            gStretchedSurface = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;

            //Quit SDL subsystems
            SDL.SDL_Quit();
        }

        static bool loadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load stretching surface
            gStretchedSurface = loadSurface("stretch.bmp");
            if (gStretchedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load stretching image!");
                success = false;
            }


            return success;
        }

        private static IntPtr loadSurface(string path)
        {
            //The final optimized image
            IntPtr optimizedSurface = IntPtr.Zero;

            //Load image at specified path
            IntPtr loadedSurface = SDL.SDL_LoadBMP(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
            }
            else
            {
                var s = Marshal.PtrToStructure<SDL.SDL_Surface>(gScreenSurface);

                //Convert surface to screen format
                optimizedSurface = SDL.SDL_ConvertSurface(loadedSurface, s.format, 0);
                if (optimizedSurface == IntPtr.Zero)
                {
                    Console.WriteLine("Unable to optimize image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                }

                //Get rid of old loaded surface
                SDL.SDL_FreeSurface(loadedSurface);
            }

            return optimizedSurface;
        }



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
                    //Get window surface
                    gScreenSurface = SDL.SDL_GetWindowSurface(gWindow);
                }
            }

            return success;
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
