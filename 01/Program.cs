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

        static int Main(string[] args)
        {
            //The window we'll be rendering to
            IntPtr window = IntPtr.Zero;

            //The surface contained by the window
            SDL.SDL_Surface screenSurface;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
            }
            else
            {
                //Create window
                window = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (window == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                }
                else
                {
                    //Get window surface
                    var myscreenSurface = SDL.SDL_GetWindowSurface(window);
                    screenSurface = Marshal.PtrToStructure<SDL.SDL_Surface>(myscreenSurface);

                    //Fill the surface white
                    SDL.SDL_FillRect(myscreenSurface, IntPtr.Zero, SDL.SDL_MapRGB(screenSurface.format, 0xFF, 0xFF, 0xFF));

                    //Update the surface
                    SDL.SDL_UpdateWindowSurface(window);

                    //Wait two seconds
                    SDL.SDL_Delay(5000);
                }
            }

            //Destroy window
            SDL.SDL_DestroyWindow(window);

            //Quit SDL subsystems
            SDL.SDL_Quit();

            return 0;
        }
    }
}
