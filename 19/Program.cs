using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _19
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //Analog joystick dead zone
        private const int JOYSTICK_DEAD_ZONE = 8000;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr gRenderer = IntPtr.Zero;

        //Scene textures
        private static LTexture gArrowTexture = new LTexture();
        
        //Game Controller 1 handler
        private static IntPtr gGameController = IntPtr.Zero;

        
        private static bool init()
        {
            //Initialization flag
            bool success = true;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_JOYSTICK) < 0)
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

                //Check for joysticks
                if (SDL.SDL_NumJoysticks() < 1)
                {
                    Console.WriteLine("Warning: No joysticks connected!");
                }
                else
                {
                    //Load joystick
                    gGameController = SDL.SDL_JoystickOpen(0);
                    if (gGameController == IntPtr.Zero)
                    {
                        Console.WriteLine("Warning: Unable to open game controller! SDL Error: {0}", SDL.SDL_GetError());
                    }
                }

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
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                    gRenderer = SDL.SDL_CreateRenderer(gWindow, -1, renderFlags);
                    if (gRenderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);

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


        static bool loadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load press texture
            if (!gArrowTexture.loadFromFile("arrow.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            return success;
        }

        private static void close()
        {
            //Free loaded images
            gArrowTexture.free();
            
            //Close game controller
            SDL.SDL_JoystickClose(gGameController);
            gGameController = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(gRenderer);
            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;
            gRenderer = IntPtr.Zero;

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
            var success = init();
            if (success == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                success = loadMedia();
                if (success == false)
                {
                    Console.WriteLine("Failed to load media!");
                }
                else
                {
                    //Main loop flag
                    bool quit = false;

                    //Event handler
                    SDL.SDL_Event e;

                    //Normalized direction
                    int xDir = 0;
                    int yDir = 0;

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
                            else if (e.type == SDL.SDL_EventType.SDL_JOYAXISMOTION)
                            {
                                //Motion on controller 0
                                if (e.jaxis.which == 0)
                                {
                                    //X axis motion
                                    if (e.jaxis.axis == 0)
                                    {
                                        //Left of dead zone
                                        if (e.jaxis.axisValue < -JOYSTICK_DEAD_ZONE)
                                        {
                                            xDir = -1;
                                        }
                                        //Right of dead zone
                                        else if (e.jaxis.axisValue > JOYSTICK_DEAD_ZONE)
                                        {
                                            xDir = 1;
                                        }
                                        else
                                        {
                                            xDir = 0;
                                        }
                                    }
                                    //Y axis motion
                                    else if (e.jaxis.axis == 1)
                                    {
                                        //Below of dead zone
                                        if (e.jaxis.axisValue < -JOYSTICK_DEAD_ZONE)
                                        {
                                            yDir = -1;
                                        }
                                        //Above of dead zone
                                        else if (e.jaxis.axisValue > JOYSTICK_DEAD_ZONE)
                                        {
                                            yDir = 1;
                                        }
                                        else
                                        {
                                            yDir = 0;
                                        }
                                    }
                                }
                            }
                        }


                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Calculate angle
                        double joystickAngle = Math.Atan2(yDir, xDir) * (180.0 / Math.PI);

                        //Correct angle
                        if (xDir == 0 && yDir == 0)
                        {
                            joystickAngle = 0;
                        }

                        //Render joystick 8 way angle
                        gArrowTexture.render((SCREEN_WIDTH - gArrowTexture.getWidth()) / 2, (SCREEN_HEIGHT - gArrowTexture.getHeight()) / 2, null, joystickAngle);
                        
                        //Update screen
                        SDL.SDL_RenderPresent(gRenderer);
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
