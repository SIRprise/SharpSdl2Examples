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

        //Window dimensions
        private int mWidth;
        private int mHeight;

        //Window focus
        private bool mMouseFocus;
        private bool mKeyboardFocus;
        private bool mFullScreen;
        private bool mMinimized;

        public LWindow()
        {
            //Initialize non-existant window
            mWindow = IntPtr.Zero;
            mMouseFocus = false;
            mKeyboardFocus = false;
            mFullScreen = false;
            mMinimized = false;
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
            }

            return mWindow != IntPtr.Zero;
        }

        public IntPtr createRenderer()
        {
            var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
            return SDL.SDL_CreateRenderer(mWindow, -1, renderFlags);
        }

        public void handleEvent(SDL.SDL_Event e)
        {
            //Window event occured
            if (e.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
            {
                //Caption update flag
                bool updateCaption = false;

                switch (e.window.windowEvent)
                {
                    //Get new dimensions and repaint on window size change
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                        mWidth = e.window.data1;
                        mHeight = e.window.data2;
                        SDL.SDL_RenderPresent(Program.gRenderer);
                        break;

                    //Repaint on exposure
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                        SDL.SDL_RenderPresent(Program.gRenderer);
                        break;

                    //Mouse entered window
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                        mMouseFocus = true;
                        updateCaption = true;
                        break;
			
                    //Mouse left window
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                        mMouseFocus = false;
                        updateCaption = true;
                        break;

                    //Window has keyboard focus
                    case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                        mKeyboardFocus = true;
                        updateCaption = true;
                        break;

                    //Window lost keyboard focus
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
                }

                //Update window caption with new data
                if( updateCaption )
                {
                    string caption = "SDL Tutorial - MouseFocus:" + (mMouseFocus ? "On" : "Off") + " KeyboardFocus:" + ((mKeyboardFocus) ? "On" : "Off");
                    SDL.SDL_SetWindowTitle(mWindow, caption);
                }
            }
            //Enter exit full screen on return key
            else if( e.type == SDL.SDL_EventType.SDL_KEYDOWN && e.key.keysym.sym == SDL.SDL_Keycode.SDLK_RETURN)
            {
                if( mFullScreen )
                {
                    SDL.SDL_SetWindowFullscreen(mWindow, (uint)SDL.SDL_bool.SDL_FALSE);
                    mFullScreen = false;
                }
                else
                {

                    SDL.SDL_SetWindowFullscreen(mWindow, (uint)SDL.SDL_bool.SDL_TRUE);
                    mFullScreen = true;
                    mMinimized = false;
                }
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

    }
}