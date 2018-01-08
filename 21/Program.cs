using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _21
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

        //Scene texture
        private static LTexture gPromptTexture = new LTexture();

        //The music that will be played
        private static IntPtr gMusic = IntPtr.Zero;

        //The sound effects that will be used
        private static IntPtr gScratch = IntPtr.Zero;
        private static IntPtr gHigh = IntPtr.Zero;
        private static IntPtr gMedium = IntPtr.Zero;
        private static IntPtr gLow = IntPtr.Zero;


        private static bool init()
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


        static bool loadMedia()
        {
            //Loading success flag
            bool success = true;

            if (!gPromptTexture.loadFromFile("prompt.png"))
            {
                Console.WriteLine("Failed to load!");
                success = false;
            }

            //Load music
            gMusic = SDL_mixer.Mix_LoadMUS("beat.wav");
            if (gMusic == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            //Load sound effects
            gScratch = SDL_mixer.Mix_LoadWAV("scratch.wav");
            if (gScratch == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            gHigh = SDL_mixer.Mix_LoadWAV("high.wav");
            if (gHigh == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            gMedium = SDL_mixer.Mix_LoadWAV("medium.wav");
            if (gMedium == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            gLow = SDL_mixer.Mix_LoadWAV("low.wav");
            if (gLow == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load! {0}", SDL.SDL_GetError());
                success = false;
            }

            return success;
        }


        private static void close()
        {
            //Free loaded images
            gPromptTexture.free();

            //Free the sound effects
            SDL_mixer.Mix_FreeChunk(gScratch);
            SDL_mixer.Mix_FreeChunk(gHigh);
            SDL_mixer.Mix_FreeChunk(gMedium);
            SDL_mixer.Mix_FreeChunk(gLow);
            gScratch = IntPtr.Zero;
            gHigh = IntPtr.Zero;
            gMedium = IntPtr.Zero;
            gLow = IntPtr.Zero;

            //Free the music
            SDL_mixer.Mix_FreeMusic(gMusic);
            gMusic = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(gRenderer);
            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;
            gRenderer = IntPtr.Zero;

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
                            //Handle key press
                            else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                            {
                                switch (e.key.keysym.sym)
                                {
                                    //Play high sound effect
                                    case SDL.SDL_Keycode.SDLK_1:
                                        SDL_mixer.Mix_PlayChannel(-1, gHigh, 0);
                                        break;

                                    //Play medium sound effect
                                    case SDL.SDL_Keycode.SDLK_2:
                                        SDL_mixer.Mix_PlayChannel(-1, gMedium, 0);
                                        break;

                                    //Play low sound effect
                                    case SDL.SDL_Keycode.SDLK_3:
                                        SDL_mixer.Mix_PlayChannel(-1, gLow, 0);
                                        break;

                                    //Play scratch sound effect
                                    case SDL.SDL_Keycode.SDLK_4:
                                        SDL_mixer.Mix_PlayChannel(-1, gScratch, 0);
                                        break;

                                    case SDL.SDL_Keycode.SDLK_9:
                                        //If there is no music playing
                                        if (SDL_mixer.Mix_PlayingMusic() == 0)
                                        {
                                            //Play the music
                                            SDL_mixer.Mix_PlayMusic(gMusic, -1);
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
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Render prompt
                        gPromptTexture.render(0, 0);

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
