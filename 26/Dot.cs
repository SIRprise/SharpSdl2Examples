using System;
using System.Runtime.InteropServices;
using SDL2;

namespace _26
{
    //Texture wrapper class
    class Dot
    {
        //The dimensions of the dot
        const int DOT_WIDTH = 20;
        const int DOT_HEIGHT = 20;

        //Maximum axis velocity of the dot
        const int DOT_VEL = 10;

        //Initializes the variables
        public Dot()
        {

        }

        //Takes key presses and adjusts the dot's velocity
        public void handleEvent(SDL.SDL_Event e)
        {
            //If a key was pressed
            if (e.type == SDL.SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0)
            {
                //Adjust the velocity
                switch (e.key.keysym.sym)
                {
                    case SDL.SDL_Keycode.SDLK_UP: mVelY -= DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_DOWN: mVelY += DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_LEFT: mVelX -= DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_RIGHT: mVelX += DOT_VEL; break;
                }
            }
            //If a key was released
            else if (e.type == SDL.SDL_EventType.SDL_KEYUP && e.key.repeat == 0)
            {
                //Adjust the velocity
                switch (e.key.keysym.sym)
                {
                    case SDL.SDL_Keycode.SDLK_UP: mVelY += DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_DOWN: mVelY -= DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_LEFT: mVelX += DOT_VEL; break;
                    case SDL.SDL_Keycode.SDLK_RIGHT: mVelX -= DOT_VEL; break;
                }
            }
        }

        //Moves the dot
        public void move()
        {
            //var a = string.Format("mPosX:{0};mVelX:{1};mPosY:{2};mVelY:{3}", mPosX, mVelX, mPosY, mVelY);
            //Console.WriteLine(a);

            //Move the dot left or right
            mPosX += mVelX;

            //If the dot went too far to the left or right
            if ((mPosX < 0) || (mPosX + DOT_WIDTH > Program.SCREEN_WIDTH))
            {
                //Move back
                mPosX -= mVelX;
            }

            //Move the dot up or down
            mPosY += mVelY;

            //If the dot went too far up or down
            if ((mPosY < 0) || (mPosY + DOT_HEIGHT > Program.SCREEN_HEIGHT))
            {
                //Move back
                mPosY -= mVelY;
            }

            //Console.WriteLine("mPosX:{0};mVelX:{1};mPosY:{2};mVelY:{3}", mPosX, mVelX, mPosY, mVelY);
        }

        //Shows the dot on the screen
        public void render()
        {
            //Show the dot
            Program.gDotTexture.render(mPosX, mPosY);
        }

        //The X and Y offsets of the dot
        int mPosX, mPosY;

        //The velocity of the dot
        int mVelX, mVelY;
    }

}