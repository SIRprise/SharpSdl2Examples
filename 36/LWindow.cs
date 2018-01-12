using System;
using System.Runtime.InteropServices;
using SDL2;

namespace SdlExample
{
    //Texture wrapper class
    class LWindow
    {
        //Window data
        private IntPtr mWindow;
        private IntPtr mRenderer;
        private int mWindowID;

        //Window dimensions
        private int mWidth;
        private int mHeight;

        //Window focus
        private bool mMouseFocus;
        private bool mKeyboardFocus;
        private bool mFullScreen;
        private bool mMinimized;
        private bool mShown;


        public LWindow()
        {
            //Initialize non-existant window
            mWindow = IntPtr.Zero;
            mRenderer = IntPtr.Zero;

            mMouseFocus = false;
            mKeyboardFocus = false;
            mFullScreen = false;
            mShown = false;
            mWindowID = -1;

            mWidth = 0;
            mHeight = 0;
        }

        public bool init()
        {
            //Create window
            mWindow = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                Program.SCREEN_WIDTH, Program.SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            if (mWindow != IntPtr.Zero)
            {
                mMouseFocus = true;
                mKeyboardFocus = true;
                mWidth = Program.SCREEN_WIDTH;
                mHeight = Program.SCREEN_HEIGHT;

                //Create renderer for window
                var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                mRenderer = SDL.SDL_CreateRenderer(mWindow, -1, renderFlags);
                if (mRenderer == IntPtr.Zero)
                {
                    Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                    SDL.SDL_DestroyWindow(mWindow);
                    mWindow = IntPtr.Zero;
                }
                else
                {
                    //Initialize renderer color
                    SDL.SDL_SetRenderDrawColor(mRenderer, 0xFF, 0xFF, 0xFF, 0xFF);

                    //Grab window identifier
                    mWindowID = (int)SDL.SDL_GetWindowID(mWindow);

                    //Flag as opened
                    mShown = true;
                }
            }
            else
            {
                Console.WriteLine("Window could not be created! SDL Error: {0}", SDL.SDL_GetError());
            }

            return mWindow != IntPtr.Zero && mRenderer != IntPtr.Zero;
        }

        public void handleEvent(SDL.SDL_Event e)
        {
            //If an event was detected for this window
            if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT && e.window.windowID == mWindowID)
            {
                //Caption update flag
                bool updateCaption = false;

                switch (e.window.windowEvent)
                {
                    //Window appeared
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                        mShown = true;
                        break;

                    //Window disappeared
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                        mShown = false;
                        break;

                    //Get new dimensions and repaint
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                        mWidth = e.window.data1;
                        mHeight = e.window.data2;
                        SDL.SDL_RenderPresent(mRenderer);
                        break;

                    //Repaint on expose
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                        SDL.SDL_RenderPresent(mRenderer);
                        break;

                    //Mouse enter
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                        mMouseFocus = true;
                        updateCaption = true;
                        break;

                    //Mouse exit
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                        mMouseFocus = false;
                        updateCaption = true;
                        break;

                    //Keyboard focus gained
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                        mKeyboardFocus = true;
                        updateCaption = true;
                        break;

                    //Keyboard focus lost
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                        mKeyboardFocus = false;
                        updateCaption = true;
                        break;

                    //Window minimized
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                        mMinimized = true;
                        break;

                    //Window maxized
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
                        mMinimized = false;
                        break;

                    //Window restored
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
                        mMinimized = false;
                        break;

                    //Hide on close
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                        SDL.SDL_HideWindow(mWindow);
                        break;
                }

                //Update window caption with new data
                if (updateCaption)
                {
                    string caption = "SDL Tutorial - ID: " + mWindowID + " MouseFocus:" + (mMouseFocus ? "On" : "Off") + " KeyboardFocus:" + (mKeyboardFocus ? "On" : "Off");
                    SDL.SDL_SetWindowTitle(mWindow, caption);
                }
            }
        }

        public void focus()
        {
            //Restore window if needed
            if (!mShown)
            {
                SDL.SDL_ShowWindow(mWindow);
            }

            //Move window forward
            SDL.SDL_RaiseWindow(mWindow);
        }

        public void render()
        {
            if (!mMinimized)
            {
                //Clear screen
                SDL.SDL_SetRenderDrawColor(mRenderer, 0xFF, 0xFF, 0xFF, 0xFF);
                SDL.SDL_RenderClear(mRenderer);

                //Update screen
                SDL.SDL_RenderPresent(mRenderer);
            }
        }


        public void free()
        {
            if (mWindow != IntPtr.Zero)
            {
                SDL.SDL_DestroyWindow(mWindow);
            }

            mMouseFocus = false;
            mKeyboardFocus = false;
            mWidth = 0;
            mHeight = 0;
        }

        public int getWidth()
        {
            return mWidth;
        }

        public int getHeight()
        {
            return mHeight;
        }

        public bool hasMouseFocus()
        {
            return mMouseFocus;
        }

        public bool hasKeyboardFocus()
        {
            return mKeyboardFocus;
        }

        public bool isMinimized()
        {
            return mMinimized;
        }

        public bool isShown()
        {
            return mShown;
        }
    }
}