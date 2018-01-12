using System;
using System.Globalization;
using System.Threading;
using SDL2;

namespace SdlExample
{
    class Program
    {
        //Screen dimension constants
        public const int SCREEN_WIDTH = 640;

        public const int SCREEN_HEIGHT = 480;
        
        //Total windows
        private const int TOTAL_WINDOWS = 3;

        //Our custom window
        private static LWindow[] gWindows = new LWindow[TOTAL_WINDOWS];

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
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                {
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");
                }

                //Create window
                gWindows[0] = new LWindow();
                if (!gWindows[0].init())
                {
                    Console.WriteLine("Window 0 could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
            }

            return success;
        }


        private static void close()
        {
            //Destroy windows
            for (int i = 0; i < TOTAL_WINDOWS; ++i)
            {
                gWindows[i].free();
            }

            //Quit SDL subsystems
            SDL.SDL_Quit();
        }

        static int Main(string[] args)
        {
            SDL.SDL_SetHint(SDL.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //Start up SDL and create window
            var success = init();
            if (success == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                //Initialize the rest of the windows
                for (int i = 1; i < TOTAL_WINDOWS; ++i)
                {
                    gWindows[i] = new LWindow();
                    gWindows[i].init();
                }

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

                        //Handle window events
                        for (int i = 0; i < TOTAL_WINDOWS; ++i)
                        {
                            gWindows[i].handleEvent(e);
                        }

                        //Pull up window
                        if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                        {
                            switch (e.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_1:
                                    gWindows[0].focus();
                                    break;

                                case SDL.SDL_Keycode.SDLK_2:
                                    gWindows[1].focus();
                                    break;

                                case SDL.SDL_Keycode.SDLK_3:
                                    gWindows[2].focus();
                                    break;
                            }
                        }
                    }

                    //Update all windows
                    for (int i = 0; i < TOTAL_WINDOWS; ++i)
                    {
                        gWindows[i].render();
                    }

                    //Check all windows
                    bool allWindowsClosed = true;
                    for (int i = 0; i < TOTAL_WINDOWS; ++i)
                    {
                        if (gWindows[i].isShown())
                        {
                            allWindowsClosed = false;
                            break;
                        }
                    }

                    //Application closed all windows
                    if (allWindowsClosed)
                    {
                        quit = true;
                    }
                }
            }

            //Free resources and close SDL
            close();

            if (success == false)
                Console.ReadLine();

            return 0;
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
