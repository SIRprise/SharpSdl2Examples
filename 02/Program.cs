using System;
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

        //The image we will load and show on the screen
        private static IntPtr _HelloWorld = IntPtr.Zero;

        static int Main(string[] args)
        {
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
                    //Apply the image
                    SDL.SDL_BlitSurface(_HelloWorld, IntPtr.Zero, _ScreenSurface, IntPtr.Zero);
                    
                    //Update the surface
                    SDL.SDL_UpdateWindowSurface(_Window);
                    
                    //Wait two seconds
                    SDL.SDL_Delay(2000);
                }
            }

            //Free resources and close SDL
            Close();

            return 0;
        }

        private static void Close()
        {
            //Deallocate surface
            SDL.SDL_FreeSurface(_HelloWorld);
            _HelloWorld = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;

            //Quit SDL subsystems
            SDL.SDL_Quit();
        }

        private static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load splash image
            _HelloWorld = SDL.SDL_LoadBMP("hello_world.bmp");
            if (_HelloWorld == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", "hello_world.bmp", SDL.SDL_GetError());
                success = false;
            }

            return success;
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
