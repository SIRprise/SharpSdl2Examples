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
        public const int DOT_WIDTH = 20;
        public const int DOT_HEIGHT = 20;

        //Maximum axis velocity of the dot
        const int DOT_VEL = 1;

        //Initializes the variables
        public Dot(int x, int y)
        {
            //Initialize the offsets
            mPosX = x;
            mPosY = y;

            //Set collision circle size
            mCollider.r = DOT_WIDTH / 2;

            //Initialize the velocity
            mVelX = 0;
            mVelY = 0;

            //Move collider relative to the circle
            shiftColliders();
        }

        private void shiftColliders()
        {
            //Align collider to center of dot
            mCollider.x = mPosX;
            mCollider.y = mPosY;
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
        public void move(SDL.SDL_Rect square, Circle circle)
        {
            //Move the dot left or right
            mPosX += mVelX;
            shiftColliders();

            //If the dot collided or went too far to the left or right
            if ((mPosX - mCollider.r < 0) || (mPosX + mCollider.r > Program.SCREEN_WIDTH) || checkCollision(mCollider, square) || checkCollision(mCollider, circle))
            {
                //Move back
                mPosX -= mVelX;
                shiftColliders();
            }

            //Move the dot up or down
            mPosY += mVelY;
            shiftColliders();

            //If the dot collided or went too far up or down
            if ((mPosY - mCollider.r < 0) || (mPosY + mCollider.r > Program.SCREEN_HEIGHT) || checkCollision(mCollider, square) || checkCollision(mCollider, circle))
            {
                //Move back
                mPosY -= mVelY;
                shiftColliders();
            }
        }

        private bool checkCollision(Circle a, Circle b)
        {
            //Calculate total radius squared
            int totalRadiusSquared = a.r + b.r;
            totalRadiusSquared = totalRadiusSquared * totalRadiusSquared;

            //If the distance between the centers of the circles is less than the sum of their radii
            if (distanceSquared(a.x, a.y, b.x, b.y) < (totalRadiusSquared))
            {
                //The circles have collided
                return true;
            }

            //If not
            return false;
        }

        private bool checkCollision(Circle a, SDL.SDL_Rect b)
        {
            //Closest point on collision box
            int cX, cY;

            //Find closest x offset
            if (a.x < b.x)
            {
                cX = b.x;
            }
            else if (a.x > b.x + b.w)
            {
                cX = b.x + b.w;
            }
            else
            {
                cX = a.x;
            }

            //Find closest y offset
            if (a.y < b.y)
            {
                cY = b.y;
            }
            else if (a.y > b.y + b.h)
            {
                cY = b.y + b.h;
            }
            else
            {
                cY = a.y;
            }

            //If the closest point is inside the circle
            if (distanceSquared(a.x, a.y, cX, cY) < a.r * a.r)
            {
                //This box and the circle have collided
                return true;
            }

            //If the shapes have not collided
            return false;
        }

        private double distanceSquared(int x1, int y1, int x2, int y2)
        {
            int deltaX = x2 - x1;
            int deltaY = y2 - y1;
            return deltaX * deltaX + deltaY * deltaY;
        }

        //Shows the dot on the screen
        public void render()
        {
            //Show the dot
            Program.gDotTexture.render(mPosX - mCollider.r, mPosY - mCollider.r);
        }

        //The X and Y offsets of the dot
        private int mPosX, mPosY;

        //The velocity of the dot
        private int mVelX, mVelY;

        //Dot's collision box
        private Circle mCollider;

        public Circle getCollider()
        {
            return mCollider;
        }
    }


    public struct Circle
    {
        public int x, y;
        public int r;
    };

}