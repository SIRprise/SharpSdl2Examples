using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SDL2;

namespace SdlExample
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //Number of data integers
        private const int TOTAL_DATA = 10;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr gRenderer = IntPtr.Zero;

        //Globally used font
        public static IntPtr gFont = IntPtr.Zero;

        //Scene textures
        private static LTexture gPromptTextTexture = new LTexture();
        private static LTexture[] gDataTextures = new LTexture[TOTAL_DATA];

        //Data points
        private static int[] gData = new int[TOTAL_DATA];

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
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
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

                        //Initialize SDL_ttf
                        if (SDL_ttf.TTF_Init() == -1)
                        {
                            Console.WriteLine("SDL_ttf could not initialize! SDL_ttf Error: {0}", SDL.SDL_GetError());
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

            //Text rendering color
            SDL.SDL_Color textColor = new SDL.SDL_Color { a = 0xFF };
            SDL.SDL_Color highlightColor = new SDL.SDL_Color { r = 0xFF, a = 0xFF };

            //Open the font
            gFont = SDL_ttf.TTF_OpenFont("lazy.ttf", 28);
            if (gFont == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load lazy font! SDL_ttf Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                if (!gPromptTextTexture.loadFromRenderedText("Enter Data:", textColor))
                {
                    Console.WriteLine("Failed to render prompt text!");
                    success = false;
                }
            }

            var existsFile = File.Exists("nums.bin");
            if (existsFile == false)
            {
                //Create file for writing
                using (var bw = new BinaryWriter(File.OpenWrite("nums.bin"), Encoding.Default))
                {
                    //Initialize data
                    for (int i = 0; i < TOTAL_DATA; ++i)
                    {
                        gData[i] = i + 1;
                        bw.Write(gData[i]);
                    }
                }

                Console.WriteLine("New file created!");
            }
            //File exists
            else
            {
                //Load data
                Console.WriteLine("Reading file...!");
                using (var bw = new BinaryReader(File.OpenRead("nums.bin"), Encoding.Default))
                {
                    //Initialize data
                    for (int i = 0; i < TOTAL_DATA; ++i)
                    {
                        gData[i] = bw.ReadInt32();
                    }
                }
            }

            //Initialize data textures
            gDataTextures[0] = new LTexture();
            gDataTextures[0].loadFromRenderedText(gData[0].ToString(), highlightColor);
            for (int i = 1; i < TOTAL_DATA; ++i)
            {
                gDataTextures[i] = new LTexture();
                gDataTextures[i].loadFromRenderedText(gData[i].ToString(), textColor);
            }

            return success;
        }

        private static void close()
        {
            //Open data for writing
            //Create file for writing
            using (var bw = new BinaryWriter(File.OpenWrite("nums.bin"), Encoding.Default))
            {
                //Initialize data
                for (int i = 0; i < TOTAL_DATA; ++i)
                {
                    bw.Write(gData[i]);
                }
            }

            //Free loaded images
            gPromptTextTexture.free();
            for (int i = 0; i < TOTAL_DATA; ++i)
            {
                gDataTextures[i].free();
            }

            //Free global font
            SDL_ttf.TTF_CloseFont(gFont);
            gFont = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(gRenderer);
            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;
            gRenderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_ttf.TTF_Quit();
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

                    //Set text color as black
                    SDL.SDL_Color textColor = new SDL.SDL_Color { a = 255 };
                    SDL.SDL_Color highlightColor = new SDL.SDL_Color { r = 0xFF, a = 0xFF };

                    //Current input point
                    int currentData = 0;

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
                                switch (e.key.keysym.sym)
                                {
                                    //Previous data entry
                                    case SDL.SDL_Keycode.SDLK_UP:
                                        //Rerender previous entry input point
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), textColor);
                                        --currentData;
                                        if (currentData < 0)
                                        {
                                            currentData = TOTAL_DATA - 1;
                                        }

                                        //Rerender current entry input point
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), highlightColor);
                                        break;

                                    //Next data entry
                                    case SDL.SDL_Keycode.SDLK_DOWN:
                                        //Rerender previous entry input point
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), textColor);
                                        ++currentData;
                                        if (currentData == TOTAL_DATA)
                                        {
                                            currentData = 0;
                                        }

                                        //Rerender current entry input point
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), highlightColor);
                                        break;

                                    //Decrement input point
                                    case SDL.SDL_Keycode.SDLK_LEFT:
                                        --gData[currentData];
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), highlightColor);
                                        break;

                                    //Increment input point
                                    case SDL.SDL_Keycode.SDLK_RIGHT:
                                        ++gData[currentData];
                                        gDataTextures[currentData].loadFromRenderedText(gData[currentData].ToString(), highlightColor);
                                        break;
                                }
                            }

                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Render text textures
                        gPromptTextTexture.render((SCREEN_WIDTH - gPromptTextTexture.getWidth()) / 2, 0);
                        for (int i = 0; i < TOTAL_DATA; ++i)
                        {
                            gDataTextures[i].render((SCREEN_WIDTH - gDataTextures[i].getWidth()) / 2, gPromptTextTexture.getHeight() + gDataTextures[0].getHeight() * i);
                        }

                        //Update screen
                        SDL.SDL_RenderPresent(gRenderer);
                    }

                    //Disable text input
                    SDL.SDL_StopTextInput();
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
