using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using SDL2;

namespace _10
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
            public void render(int x, int y)
            {
                //Set rendering space and render to screen
                SDL.SDL_Rect renderQuad = new SDL.SDL_Rect { x = x, y = y, w = mWidth, h = mHeight };
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

        //Scene textures
        private static LTexture gFooTexture = new LTexture();
        private static LTexture gBackgroundTexture = new LTexture();


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

            //Load Foo' texture
            if (!gFooTexture.loadFromFile("foo.png"))
            {
                Console.WriteLine("Failed to load Foo' texture image!");
                success = false;
            }

            //Load background texture
            if (!gBackgroundTexture.loadFromFile("background.png"))
            {
                Console.WriteLine("Failed to load background texture image!");
                success = false;
            }

            return success;
        }

        private static void close()
        {
            //Free loaded images
            gFooTexture.free();
            gBackgroundTexture.free();

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

                        //Render background texture to screen
                        gBackgroundTexture.render(0, 0);

                        //Render Foo' to the screen
                        gFooTexture.render(240, 190);

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
