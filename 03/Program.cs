using System;
using System.Runtime.InteropServices;
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

        //The image we will load and show on the screen
        private static IntPtr gXOut = IntPtr.Zero;

        static int Main(string[] args)
        {
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
                        }

                        //Apply the image
                        SDL.SDL_BlitSurface(gXOut, IntPtr.Zero, gScreenSurface, IntPtr.Zero);

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
            //Deallocate surface
            SDL.SDL_FreeSurface(gXOut);
            gXOut = IntPtr.Zero;

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

            //Load splash image
            gXOut = SDL.SDL_LoadBMP("x.bmp");
            if (gXOut == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", "x.bmp", SDL.SDL_GetError());
                success = false;
            }

            return success;
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
    }
}
