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
        private static IntPtr gCurrentSurface = IntPtr.Zero; 

        //The images that correspond to a keypress
        private static IntPtr[] gKeyPressSurfaces = new IntPtr[6];

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

                    //Set default current surface
                    gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DEFAULT];

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
                            
                            //User presses a key
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                //Select surfaces based on key press
                                switch (e.key.keysym.sym)
                                {
                                    case SDL.SDL_Keycode.SDLK_UP:
                                        gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_UP];
                                        break;

                                    case SDL.SDL_Keycode.SDLK_DOWN:
                                        gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DOWN];
                                        break;

                                    case SDL.SDL_Keycode.SDLK_LEFT:
                                        gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_LEFT];
                                        break;

                                    case SDL.SDL_Keycode.SDLK_RIGHT:
                                        gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_RIGHT];
                                        break;

                                    default:
                                        gCurrentSurface = gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DEFAULT];
                                        break;
                                }
                            }
                        }

                        //Apply the current image
                        SDL.SDL_BlitSurface(gCurrentSurface, IntPtr.Zero, gScreenSurface, IntPtr.Zero);

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
            //Deallocate surfaces
            for (int i = 0; i < gKeyPressSurfaces.Length; ++i)
            {
                SDL.SDL_FreeSurface(gKeyPressSurfaces[i]);
                gKeyPressSurfaces[i] = IntPtr.Zero;
            }

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

            //Load default surface
            gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DEFAULT] = loadSurface("press.bmp");
            if (gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DEFAULT] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load default image!");
                success = false;
            }

            //Load up surface
            gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_UP] = loadSurface("up.bmp");
            if (gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_UP] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load up image!");
                success = false;
            }

            //Load down surface
            gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DOWN] = loadSurface("down.bmp");
            if (gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_DOWN] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load down image!");
                success = false;
            }

            //Load left surface
            gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_LEFT] = loadSurface("left.bmp");
            if (gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_LEFT] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load left image!");
                success = false;
            }

            //Load right surface
            gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_RIGHT] = loadSurface("right.bmp");
            if (gKeyPressSurfaces[(int)KeyPressSurfaces.KEY_PRESS_SURFACE_RIGHT] == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load right image!");
                success = false;
            }

            return success;
        }

        private static IntPtr loadSurface(string path)
        {
            //Load image at specified path
            IntPtr loadedSurface = SDL.SDL_LoadBMP(path);
            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
            }


            return loadedSurface;
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
