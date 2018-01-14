using SDL2;

namespace SdlExample
{
    //Texture wrapper class
    public class LButton
    {
        public enum LButtonSprite
        {
            BUTTON_SPRITE_MOUSE_OUT = 0,
            BUTTON_SPRITE_MOUSE_OVER_MOTION = 1,
            BUTTON_SPRITE_MOUSE_DOWN = 2,
            BUTTON_SPRITE_MOUSE_UP = 3,
            BUTTON_SPRITE_TOTAL = 4
        }

        //Top left position
        private SDL.SDL_Point _Position;

        //Currently used global sprite
        private LButtonSprite _CurrentSprite;

        //Initializes variables
        public LButton()
        {
            _Position.x = 0;
            _Position.y = 0;

            _CurrentSprite = LButtonSprite.BUTTON_SPRITE_MOUSE_OUT;
        }

        public void SetPosition(int x, int y)
        {
            _Position.x = x;
            _Position.y = y;
        }

        public void HandleEvent(SDL.SDL_Event e)
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
                if (x < _Position.x)
                {
                    inside = false;
                }
                //Mouse is right of the button
                else if (x > _Position.x + Program.BUTTON_WIDTH)
                {
                    inside = false;
                }
                //Mouse above the button
                else if (y < _Position.y)
                {
                    inside = false;
                }
                //Mouse below the button
                else if (y > _Position.y + Program.BUTTON_HEIGHT)
                {
                    inside = false;
                }

                //Mouse is outside button
                if (!inside)
                {
                    _CurrentSprite = LButtonSprite.BUTTON_SPRITE_MOUSE_OUT;
                }
                //Mouse is inside button
                else
                {
                    //Set mouse over sprite
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            _CurrentSprite = LButtonSprite.BUTTON_SPRITE_MOUSE_OVER_MOTION;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            _CurrentSprite = LButtonSprite.BUTTON_SPRITE_MOUSE_DOWN;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            _CurrentSprite = LButtonSprite.BUTTON_SPRITE_MOUSE_UP;
                            break;
                    }
                }
            }
        }

        public void Render()
        {
            //Show current button sprite
            Program.ButtonSpriteSheetTexture.Render(_Position.x, _Position.y, Program.SpriteClips[(int)_CurrentSprite]);
        }

    }

}