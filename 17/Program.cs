using System;
using System.Globalization;
using System.Threading;
using SDL2;

namespace SdlExample
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //Button constants
        public const int BUTTON_WIDTH = 300;

        public const int BUTTON_HEIGHT = 200;
        private const int TOTAL_BUTTONS = 4;


        //The window we'll be rendering to
        private static IntPtr _Window = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr Renderer = IntPtr.Zero;

        //Rendered texture
        public static LTexture ButtonSpriteSheetTexture = new LTexture();

        //Mouse button sprites
        public static SDL.SDL_Rect[] SpriteClips = new SDL.SDL_Rect[4];

        //Buttons objects
        private static readonly LButton[] _Buttons = new LButton[TOTAL_BUTTONS];

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
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                {
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");
                }

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
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                    Renderer = SDL.SDL_CreateRenderer(_Window, -1, renderFlags);
                    if (Renderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);

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


        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Load sprites
            if (!ButtonSpriteSheetTexture.LoadFromFile("button.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }
            else
            {
                //Set sprites
                for (int i = 0; i < 4; ++i)
                {
                    SpriteClips[i].x = 0;
                    SpriteClips[i].y = i * 200;
                    SpriteClips[i].w = BUTTON_WIDTH;
                    SpriteClips[i].h = BUTTON_HEIGHT;
                }

                //Set buttons in corners
                _Buttons[0] = new LButton();
                _Buttons[0].SetPosition(0, 0);
                _Buttons[1] = new LButton();
                _Buttons[1].SetPosition(SCREEN_WIDTH - BUTTON_WIDTH, 0);
                _Buttons[2] = new LButton();
                _Buttons[2].SetPosition(0, SCREEN_HEIGHT - BUTTON_HEIGHT);
                _Buttons[3] = new LButton();
                _Buttons[3].SetPosition(SCREEN_WIDTH - BUTTON_WIDTH, SCREEN_HEIGHT - BUTTON_HEIGHT);
            }

            return success;
        }

        private static void Close()
        {
            //Free loaded images
            ButtonSpriteSheetTexture.Free();

            //Destroy window
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;

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
            var success = Init();
            if (success == false)
            {
                Console.WriteLine("Failed to initialize!");
            }
            else
            {
                //Load media
                success = LoadMedia();
                if (success == false)
                {
                    Console.WriteLine("Failed to load media!");
                }
                else
                {
                    //Main loop flag
                    bool quit = false;

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
                                quit = true;

                            //Handle button events
                            for (int i = 0; i < TOTAL_BUTTONS; ++i)
                            {
                                _Buttons[i].HandleEvent(e);
                            }
                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(Renderer);

                        //Render buttons
                        for (int i = 0; i < TOTAL_BUTTONS; ++i)
                        {
                            _Buttons[i].Render();
                        }

                        //Update screen
                        SDL.SDL_RenderPresent(Renderer);
                    }
                }
            }


            //Free resources and close SDL
            Close();

            if (success == false)
                Console.ReadLine();

            return 0;
        }
    }
}
