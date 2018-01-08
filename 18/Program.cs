using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _18
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr gRenderer = IntPtr.Zero;

        //Scene textures
        private static LTexture gPressTexture = new LTexture();
        private static LTexture gUpTexture = new LTexture();
        private static LTexture gDownTexture = new LTexture();
        private static LTexture gLeftTexture = new LTexture();
        private static LTexture gRightTexture = new LTexture();

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
            if (!gPressTexture.loadFromFile("press.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load up texture
            if (!gUpTexture.loadFromFile("up.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load down texture
            if (!gDownTexture.loadFromFile("down.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load left texture
            if (!gLeftTexture.loadFromFile("left.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load right texture
            if (!gRightTexture.loadFromFile("right.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            return success;
        }

        private static void close()
        {
            //Free loaded images
            gPressTexture.free();
            gUpTexture.free();
            gDownTexture.free();
            gLeftTexture.free();
            gRightTexture.free();

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

                    //Current rendered texture
                    LTexture currentTexture;

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

                        //Set texture based on current keystate
                        int size;
                        var currentKeyStatesPtr = SDL.SDL_GetKeyboardState(out size);
                        var currentKeyStates = new byte[size];
                        Marshal.Copy(currentKeyStatesPtr, currentKeyStates, 0, currentKeyStates.Length);

                        if (currentKeyStates.Any(a => a != 0))
                        {
                            Console.Write("KeyState arrays: ");

                            int index = -1;
                            foreach (var keyState in currentKeyStates)
                            {
                                index++;
                                if (keyState == 0)
                                    continue;

                                Console.Write("{0};", (SDL.SDL_Scancode)index);
                            }

                            Console.WriteLine();
                        }

                        if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] != 0)
                        {
                            currentTexture = gUpTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] != 0)
                        {
                            currentTexture = gDownTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] != 0)
                        {
                            currentTexture = gLeftTexture;
                        }
                        else if (currentKeyStates[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] != 0)
                        {
                            currentTexture = gRightTexture;
                        }
                        else
                        {
                            currentTexture = gPressTexture;
                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Render current texture
                        currentTexture.render(0, 0);

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
