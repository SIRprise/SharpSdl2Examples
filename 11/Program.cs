using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _12
{
    class Program
    {
        //Texture wrapper class
        class LTexture
        {

            //Initializes variables
            public LTexture()
            {
                //Initialize
                mTexture = IntPtr.Zero;
                mWidth = 0;
                mHeight = 0;
            }

            //Deallocates memory
            ~LTexture()
            {
                free();
            }

            //Loads image at specified path
            public bool loadFromFile(string path)
            {
                //Get rid of preexisting texture
                free();

                //The final texture
                IntPtr newTexture = IntPtr.Zero;

                //Load image at specified path
                IntPtr loadedSurface = SDL_image.IMG_Load(path);
                if (loadedSurface == IntPtr.Zero)
                {
                    Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                }
                else
                {
                    var s = Marshal.PtrToStructure<SDL.SDL_Surface>(loadedSurface);

                    //Color key image
                    SDL.SDL_SetColorKey(loadedSurface, (int)SDL.SDL_bool.SDL_TRUE, SDL.SDL_MapRGB(s.format, 0, 0xFF, 0xFF));

                    //Create texture from surface pixels
                    newTexture = SDL.SDL_CreateTextureFromSurface(gRenderer, loadedSurface);
                    if (newTexture == IntPtr.Zero)
                    {
                        Console.WriteLine("Unable to create texture from {0}! SDL Error: {1}", path, SDL.SDL_GetError());
                    }
                    else
                    {
                        //Get image dimensions
                        mWidth = s.w;
                        mHeight = s.h;
                    }

                    //Get rid of old loaded surface
                    SDL.SDL_FreeSurface(loadedSurface);
                }

                //Return success
                mTexture = newTexture;
                return mTexture != IntPtr.Zero;
            }

            //Deallocates texture
            public void free()
            {
                //Free texture if it exists
                if (mTexture != IntPtr.Zero)
                {
                    SDL.SDL_DestroyTexture(mTexture);
                    mTexture = IntPtr.Zero;
                    mWidth = 0;
                    mHeight = 0;
                }
            }

            //Renders texture at given point
            public void render(int x, int y, SDL.SDL_Rect? clip)
            {
                //Set rendering space and render to screen
                SDL.SDL_Rect renderQuad = new SDL.SDL_Rect { x = x, y = y, w = mWidth, h = mHeight };

                //Set clip rendering dimensions
                if (clip != null)
                {
                    renderQuad.w = clip.Value.w;
                    renderQuad.h = clip.Value.h;

                    var myClip = clip.Value;

                    SDL.SDL_RenderCopy(gRenderer, mTexture, ref myClip, ref renderQuad);
                    return;
                }

                SDL.SDL_RenderCopy(gRenderer, mTexture, IntPtr.Zero, ref renderQuad);
            }

            //Gets image dimensions
            public int getWidth()
            {
                return mWidth;
            }

            public int getHeight()
            {
                return mHeight;
            }


            //The actual hardware texture
            private IntPtr mTexture;

            //Image dimensions
            private int mWidth;

            private int mHeight;
        };

        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;

        private const int SCREEN_HEIGHT = 480;

        //The window we'll be rendering to
        private static IntPtr gWindow = IntPtr.Zero;

        //The surface contained by the window
        private static IntPtr gRenderer = IntPtr.Zero;

        //Scene sprites
        private static SDL.SDL_Rect[] gSpriteClips = new SDL.SDL_Rect[4];
        private static LTexture gSpriteSheetTexture = new LTexture();


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
                    //Create renderer for window
                    gRenderer = SDL.SDL_CreateRenderer(gWindow, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
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

            //Load sprite sheet texture
            if (!gSpriteSheetTexture.loadFromFile("dots.png"))
            {
                Console.WriteLine("Failed to load sprite sheet texture!");
                success = false;
            }
            else
            {
                //Set top left sprite
                gSpriteClips[0].x = 0;
                gSpriteClips[0].y = 0;
                gSpriteClips[0].w = 100;
                gSpriteClips[0].h = 100;

                //Set top right sprite
                gSpriteClips[1].x = 100;
                gSpriteClips[1].y = 0;
                gSpriteClips[1].w = 100;
                gSpriteClips[1].h = 100;

                //Set bottom left sprite
                gSpriteClips[2].x = 0;
                gSpriteClips[2].y = 100;
                gSpriteClips[2].w = 100;
                gSpriteClips[2].h = 100;

                //Set bottom right sprite
                gSpriteClips[3].x = 100;
                gSpriteClips[3].y = 100;
                gSpriteClips[3].w = 100;
                gSpriteClips[3].h = 100;
            }

            return success;
        }

        private static void close()
        {
            //Free loaded images
            gSpriteSheetTexture.free();

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

                        //Clear screen
                        SDL.SDL_SetRenderDrawColor(gRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                        SDL.SDL_RenderClear(gRenderer);

                        //Render top left sprite
                        gSpriteSheetTexture.render(0, 0, gSpriteClips[0]);

                        //Render top right sprite
                        gSpriteSheetTexture.render(SCREEN_WIDTH - gSpriteClips[1].w, 0, gSpriteClips[1]);

                        //Render bottom left sprite
                        gSpriteSheetTexture.render(0, SCREEN_HEIGHT - gSpriteClips[2].h, gSpriteClips[2]);

                        //Render bottom right sprite
                        gSpriteSheetTexture.render(SCREEN_WIDTH - gSpriteClips[3].w, SCREEN_HEIGHT - gSpriteClips[3].h, gSpriteClips[3]);

                        //gSpriteSheetTexture.render(0, 0, null);

                        //Update screen
                        SDL.SDL_RenderPresent(gRenderer);
                    }
                }
            }


            //Free resources and close SDL
            close();

            //Console.ReadLine();
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
