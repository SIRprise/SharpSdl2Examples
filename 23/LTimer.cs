using SDL2;

namespace SdlExample
{
    //Texture wrapper class
    public class LTimer
    {
        //The clock time when the timer started
        private uint _StartTicks;

        //The ticks stored when the timer was paused
        private uint _PausedTicks;

        //The timer status
        private bool _Paused;
        private bool _Started;

        //Initializes variables
        public LTimer()
        {
            //Initialize the variables
            _StartTicks = 0;
            _PausedTicks = 0;

            _Paused = false;
            _Started = false;
        }

        public void Start()
        {
            //Start the timer
            _Started = true;

            //Unpause the timer
            _Paused = false;

            //Get the current clock time
            _StartTicks = SDL.SDL_GetTicks();
            _PausedTicks = 0;
        }

        public void Stop()
        {
            //Stop the timer
            _Started = false;

            //Unpause the timer
            _Paused = false;

            //Clear tick variables
            _StartTicks = 0;
            _PausedTicks = 0;
        }

        public void Pause()
        {
            //If the timer is running and isn't already paused
            if (_Started && !_Paused)
            {
                //Pause the timer
                _Paused = true;

                //Calculate the paused ticks
                _PausedTicks = SDL.SDL_GetTicks() - _StartTicks;
                _StartTicks = 0;
            }
        }

        public void Unpause()
        {
            //If the timer is running and paused
            if (_Started && _Paused)
            {
                //Unpause the timer
                _Paused = false;

                //Reset the starting ticks
                _StartTicks = SDL.SDL_GetTicks() - _PausedTicks;

                //Reset the paused ticks
                _PausedTicks = 0;
            }
        }


        public uint GetTicks()
        {
            //The actual timer time
            uint time = 0;

            //If the timer is running
            if (_Started)
            {
                //If the timer is paused
                if (_Paused)
                {
                    //Return the number of ticks when the timer was paused
                    time = _PausedTicks;
                }
                else
                {
                    //Return the current time minus the start time
                    time = SDL.SDL_GetTicks() - _StartTicks;
                }
            }

            return time;
        }

        public bool IsStarted()
        {
            //Timer is running and paused or unpaused
            return _Started;
        }

        public bool IsPaused()
        {
            //Timer is running and paused
            return _Paused && _Started;
        }
    }
}