using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDL2;

namespace SdlExample
{
    //Texture wrapper class
    public class Dot
    {
        //The dimensions of the dot
        const int DOT_WIDTH = 20;
        const int DOT_HEIGHT = 20;

        //Maximum axis velocity of the dot
        const int DOT_VEL = 1;

        //Initializes the variables
        public Dot(int x, int y)
        {
            //Initialize the offsets
            mPosX = x;
            mPosY = y;

            //Create the necessary SDL_Rects
            //mColliders.resize(11);
            mColliders = new SDL.SDL_Rect[11];

            //Initialize the velocity
            mVelX = 0;
            mVelY = 0;

            //Initialize the collision boxes' width and height
            mColliders[0].w = 6;
            mColliders[0].h = 1;

            mColliders[1].w = 10;
            mColliders[1].h = 1;

            mColliders[2].w = 14;
            mColliders[2].h = 1;

            mColliders[3].w = 16;
            mColliders[3].h = 2;

            mColliders[4].w = 18;
            mColliders[4].h = 2;

            mColliders[5].w = 20;
            mColliders[5].h = 6;

            mColliders[6].w = 18;
            mColliders[6].h = 2;

            mColliders[7].w = 16;
            mColliders[7].h = 2;

            mColliders[8].w = 14;
            mColliders[8].h = 1;

            mColliders[9].w = 10;
            mColliders[9].h = 1;

            mColliders[10].w = 6;
            mColliders[10].h = 1;

            //Initialize colliders relative to position
            shiftColliders();
        }

        private void shiftColliders()
        {
            //The row offset
            int r = 0;

            //Go through the dot's collision boxes
            for (int set = 0; set < mColliders.Length; ++set)
            {
                //Center the collision box
                mColliders[set].x = mPosX + (DOT_WIDTH - mColliders[set].w) / 2;

                //Set the collision box at its row offset
                mColliders[set].y = mPosY + r;

                //Move the row offset down the height of the collision box
                r += mColliders[set].h;
            }
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
        public void move(SDL.SDL_Rect[] otherColliders)
        {
            //Move the dot left or right
            mPosX += mVelX;
            shiftColliders();

            //If the dot collided or went too far to the left or right
            if ((mPosX < 0) || (mPosX + DOT_WIDTH > Program.SCREEN_WIDTH) || checkCollision(mColliders, otherColliders))
            {
                //Move back
                mPosX -= mVelX;
                shiftColliders();
            }

            //Move the dot up or down
            mPosY += mVelY;
            shiftColliders();

            //If the dot collided or went too far up or down
            if ((mPosY < 0) || (mPosY + DOT_HEIGHT > Program.SCREEN_HEIGHT) || checkCollision(mColliders, otherColliders))
            {
                //Move back
                mPosY -= mVelY;
                shiftColliders();
            }
        }

        private bool checkCollision(SDL.SDL_Rect[] a, SDL.SDL_Rect[] b)
        {
            //The sides of the rectangles
            int leftA, leftB;
            int rightA, rightB;
            int topA, topB;
            int bottomA, bottomB;

            //Go through the A boxes
            for (int Abox = 0; Abox < a.Length; Abox++)
            {
                //Calculate the sides of rect A
                leftA = a[Abox].x;
                rightA = a[Abox].x + a[Abox].w;
                topA = a[Abox].y;
                bottomA = a[Abox].y + a[Abox].h;

                //Go through the B boxes
                for (int Bbox = 0; Bbox < b.Length; Bbox++)
                {
                    //Calculate the sides of rect B
                    leftB = b[Bbox].x;
                    rightB = b[Bbox].x + b[Bbox].w;
                    topB = b[Bbox].y;
                    bottomB = b[Bbox].y + b[Bbox].h;

                    //If no sides from A are outside of B
                    if (((bottomA <= topB) || (topA >= bottomB) || (rightA <= leftB) || (leftA >= rightB)) == false)
                    {
                        //A collision is detected
                        return true;
                    }
                }
            }

            //If neither set of collision boxes touched
            return false;
        }


        //Shows the dot on the screen
        public void render()
        {
            //Show the dot
            Program.gDotTexture.render(mPosX, mPosY);
        }

        //The X and Y offsets of the dot
        private int mPosX, mPosY;

        //The velocity of the dot
        private int mVelX, mVelY;

        //Dot's collision box
        private SDL.SDL_Rect mCollider;

        //private List<SDL.SDL_Rect> mColliders = new List<SDL.SDL_Rect>();
        private SDL.SDL_Rect[] mColliders;

        public SDL.SDL_Rect[] getColliders()
        {
            return mColliders;
        }
    }

}