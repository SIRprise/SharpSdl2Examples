using SDL2;

namespace _17
{
    //Texture wrapper class
    class LButton
    {

        //Initializes variables
        public LButton()
        {
            mPosition.x = 0;
            mPosition.y = 0;

            mCurrentSprite = Program.LButtonSprite.BUTTON_SPRITE_MOUSE_OUT;
        }

        public void setPosition(int x, int y)
        {
            mPosition.x = x;
            mPosition.y = y;
        }

        public void handleEvent(SDL.SDL_Event e)
        {
            //If mouse event happened
            if (e.type == SDL.SDL_EventType.SDL_MOUSEMOTION || e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN || e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
            {
                //Get mouse position
                int x, y;
                SDL.SDL_GetMouseState(out x, out y);

                //Check if mouse is in button
                bool inside = true;

                //Mouse is left of the button
                if (x < mPosition.x)
                {
                    inside = false;
                }
                //Mouse is right of the button
                else if (x > mPosition.x + Program.BUTTON_WIDTH)
                {
                    inside = false;
                }
                //Mouse above the button
                else if (y < mPosition.y)
                {
                    inside = false;
                }
                //Mouse below the button
                else if (y > mPosition.y + Program.BUTTON_HEIGHT)
                {
                    inside = false;
                }

                //Mouse is outside button
                if (!inside)
                {
                    mCurrentSprite = Program.LButtonSprite.BUTTON_SPRITE_MOUSE_OUT;
                }
                //Mouse is inside button
                else
                {
                    //Set mouse over sprite
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            mCurrentSprite = Program.LButtonSprite.BUTTON_SPRITE_MOUSE_OVER_MOTION;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            mCurrentSprite = Program.LButtonSprite.BUTTON_SPRITE_MOUSE_DOWN;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            mCurrentSprite = Program.LButtonSprite.BUTTON_SPRITE_MOUSE_UP;
                            break;
                    }
                }
            }
        }

        public void render()
        {
            //Show current button sprite
            Program.gButtonSpriteSheetTexture.render(mPosition.x, mPosition.y, Program.gSpriteClips[(int)mCurrentSprite]);
        }


        //Top left position
        SDL.SDL_Point mPosition;

        //Currently used global sprite
        Program.LButtonSprite mCurrentSprite;
    };

}