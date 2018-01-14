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

        //The window we'll be rendering to
        private static IntPtr _Window = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr Renderer = IntPtr.Zero;

        //Scene texture
        private static readonly LTexture _PromptTexture = new LTexture();

        //The music that will be played
        private static IntPtr _Music = IntPtr.Zero;

        //The sound effects that will be used
        private static IntPtr _Scratch = IntPtr.Zero;
        private static IntPtr _High = IntPtr.Zero;
        private static IntPtr _Medium = IntPtr.Zero;
        private static IntPtr _Low = IntPtr.Zero;


        private static bool Init()
        {
            //Initialization flag
            bool success = true;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
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

                        //Initialize SDL_mixer
                        if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) < 0)
                        {
                            Console.WriteLine("SDL_mixer could not initialize! SDL_mixer Error: {0}", SDL.SDL_GetError());
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

            if (!_PromptTexture.LoadFromFile("prompt.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load music
            _Music = SDL_mixer.Mix_LoadMUS("beat.wav");
            if (_Music == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            //Load sound effects
            _Scratch = SDL_mixer.Mix_LoadWAV("scratch.wav");
            if (_Scratch == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            _High = SDL_mixer.Mix_LoadWAV("high.wav");
            if (_High == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            _Medium = SDL_mixer.Mix_LoadWAV("medium.wav");
            if (_Medium == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            _Low = SDL_mixer.Mix_LoadWAV("low.wav");
            if (_Low == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            return success;
        }


        private static void Close()
        {
            //Free loaded images
            _PromptTexture.Free();

            //Free the sound effects
            SDL_mixer.Mix_FreeChunk(_Scratch);
            SDL_mixer.Mix_FreeChunk(_High);
            SDL_mixer.Mix_FreeChunk(_Medium);
            SDL_mixer.Mix_FreeChunk(_Low);
            _Scratch = IntPtr.Zero;
            _High = IntPtr.Zero;
            _Medium = IntPtr.Zero;
            _Low = IntPtr.Zero;

            //Free the music
            SDL_mixer.Mix_FreeMusic(_Music);
            _Music = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_mixer.Mix_Quit();
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
                            {
                                quit = true;
                            }
                            //Handle key press
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                switch (e.key.keysym.sym)
                                {
                                    //Play high sound effect
                                    case SDL.SDL_Keycode.SDLK_1:
                                        SDL_mixer.Mix_PlayChannel(-1, _High, 0);
                                        break;

                                    //Play medium sound effect
                                    case SDL.SDL_Keycode.SDLK_2:
                                        SDL_mixer.Mix_PlayChannel(-1, _Medium, 0);
                                        break;

                                    //Play low sound effect
                                    case SDL.SDL_Keycode.SDLK_3:
                                        SDL_mixer.Mix_PlayChannel(-1, _Low, 0);
                                        break;

                                    //Play scratch sound effect
                                    case SDL.SDL_Keycode.SDLK_4:
                                        SDL_mixer.Mix_PlayChannel(-1, _Scratch, 0);
                                        break;

                                    case SDL.SDL_Keycode.SDLK_9:
                                        //If there is no music playing
                                        if (SDL_mixer.Mix_PlayingMusic() == 0)
                                        {
                                            //Play the music
                                            SDL_mixer.Mix_PlayMusic(_Music, -1);
                                        }
                                        //If music is being played
                                        else
                                        {
                                            //If the music is paused
                                            if (SDL_mixer.Mix_PausedMusic() == 1)
                                            {
                                                //Resume the music
                                                SDL_mixer.Mix_ResumeMusic();
                                            }
                                            //If the music is playing
                                            else
                                            {
                                                //Pause the music
                                                SDL_mixer.Mix_PauseMusic();
                                            }
                                        }
                                        break;

                                    case SDL.SDL_Keycode.SDLK_0:
                                        //Stop the music
                                        SDL_mixer.Mix_HaltMusic();
                                        break;
                                }
                            }
                        }

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(Renderer);

                        //Render prompt
                        _PromptTexture.Render(0, 0);

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
