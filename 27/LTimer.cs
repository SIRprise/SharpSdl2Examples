using System;
using System.Runtime.InteropServices;
using SDL2;

namespace _27
{
    //Texture wrapper class
    class LTimer
    {

        //Initializes variables
        public LTimer()
        {
            //Initialize the variables
            mStartTicks = 0;
            mPausedTicks = 0;

            mPaused = false;
            mStarted = false;
        }

        public void start()
        {
            //Start the timer
            mStarted = true;

            //Unpause the timer
            mPaused = false;

            //Get the current clock time
            mStartTicks = SDL.SDL_GetTicks();
            mPausedTicks = 0;
        }

        public void stop()
        {
            //Stop the timer
            mStarted = false;

            //Unpause the timer
            mPaused = false;

            //Clear tick variables
            mStartTicks = 0;
            mPausedTicks = 0;
        }

        public void pause()
        {
            //If the timer is running and isn't already paused
            if (mStarted && !mPaused)
            {
                //Pause the timer
                mPaused = true;

                //Calculate the paused ticks
                mPausedTicks = SDL.SDL_GetTicks() - mStartTicks;
                mStartTicks = 0;
            }
        }

        public void unpause()
        {
            //If the timer is running and paused
            if (mStarted && mPaused)
            {
                //Unpause the timer
                mPaused = false;

                //Reset the starting ticks
                mStartTicks = SDL.SDL_GetTicks() - mPausedTicks;

                //Reset the paused ticks
                mPausedTicks = 0;
            }
        }


        public uint getTicks()
        {
            //The actual timer time
            uint time = 0;

            //If the timer is running
            if (mStarted)
            {
                //If the timer is paused
                if (mPaused)
                {
                    //Return the number of ticks when the timer was paused
                    time = mPausedTicks;
                }
                else
                {
                    //Return the current time minus the start time
                    time = SDL.SDL_GetTicks() - mStartTicks;
                }
            }

            return time;
        }

        public bool isStarted()
        {
            //Timer is running and paused or unpaused
            return mStarted;
        }

        public bool isPaused()
        {
            //Timer is running and paused
            return mPaused && mStarted;
        }


        //The clock time when the timer started
        private uint mStartTicks;

        //The ticks stored when the timer was paused
        private uint mPausedTicks;

        //The timer status
        private bool mPaused;
        private bool mStarted;
    };

}