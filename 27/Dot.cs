using System;
using System.Runtime.InteropServices;
using SDL2;

namespace _27
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
            //Initialize the offsets
            mPosX = 0;
            mPosY = 0;

            //Set collision box dimension
            mCollider.w = DOT_WIDTH;
            mCollider.h = DOT_HEIGHT;

            //Initialize the velocity
            mVelX = 0;
            mVelY = 0;
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
        public void move(SDL.SDL_Rect wall)
        {
            //Move the dot left or right
            mPosX += mVelX;
            mCollider.x = mPosX;

            //If the dot collided or went too far to the left or right
            if ((mPosX < 0) || (mPosX + DOT_WIDTH > Program.SCREEN_WIDTH) || checkCollision(mCollider, wall))
            {
                //Move back
                mPosX -= mVelX;
                mCollider.x = mPosX;
            }

            //Move the dot up or down
            mPosY += mVelY;
            mCollider.y = mPosY;

            //If the dot collided or went too far up or down
            if ((mPosY < 0) || (mPosY + DOT_HEIGHT > Program.SCREEN_HEIGHT) || checkCollision(mCollider, wall))
            {
                //Move back
                mPosY -= mVelY;
                mCollider.y = mPosY;
            }
        }

        private bool checkCollision(SDL.SDL_Rect a, SDL.SDL_Rect b)
        {
            //The sides of the rectangles
            int leftA, leftB;
            int rightA, rightB;
            int topA, topB;
            int bottomA, bottomB;

            //Calculate the sides of rect A
            leftA = a.x;
            rightA = a.x + a.w;
            topA = a.y;
            bottomA = a.y + a.h;

            //Calculate the sides of rect B
            leftB = b.x;
            rightB = b.x + b.w;
            topB = b.y;
            bottomB = b.y + b.h;

            //If any of the sides from A are outside of B
            if (bottomA <= topB)
            {
                return false;
            }

            if (topA >= bottomB)
            {
                return false;
            }

            if (rightA <= leftB)
            {
                return false;
            }

            if (leftA >= rightB)
            {
                return false;
            }

            //If none of the sides from A are outside B
            return true;
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

        //Dot's collision box
        SDL.SDL_Rect mCollider;
    }

}