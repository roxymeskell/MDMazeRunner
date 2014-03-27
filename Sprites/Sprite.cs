using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// An abstract definition of a simple sprite
    /// </summary>
    abstract class Sprite
    {
        public static SpriteBatch batch;
        public static SpriteBatch Batch { get { return batch; } set { batch = value; } }
        public int ViewNegX { get { return Batch.GraphicsDevice.Viewport.Bounds.Left; } }
        public int ViewPosX { get { return Batch.GraphicsDevice.Viewport.Bounds.Right; } }
        public int ViewNegY { get { return Batch.GraphicsDevice.Viewport.Bounds.Top; } }
        public int ViewPosY { get { return Batch.GraphicsDevice.Viewport.Bounds.Bottom; } }

        protected readonly int iScale, iWidth, iDepth;
        protected Vector2 position, actualPosition;
        protected int width, depth;
        protected float scale, rotation;
        protected SpriteTexture texture;
        internal Vector2 Position { get { return position; } set { actualPosition += value - position; position = value; } }
        internal Vector2 ActualPosition { get { return actualPosition; } set { position += value - actualPosition; actualPosition = value; } }
        internal virtual int Width { get { return width; } set { width = value <= 0 ? 1 : (value > iWidth ? iWidth : value); } }
        internal virtual int Depth { get { return depth; } set { depth = value <= 0 ? 1 : (value > iDepth ? iDepth : value); } }
        internal int ScaledWidth { get { return (int)((Width / iScale) * scale); } set { Width = (int)((value * iScale) / scale); } }
        internal int ScaledDepth { get { return (int)((Depth / iScale) * scale); } set { Depth = (int)((value * iScale) / scale); } }
        protected float Scale { get { return scale; } set { scale = value <= 0 ? 1 : value; } }
        protected virtual float Rotation { get { return rotation; } set { rotation = value; } }
        protected virtual Vector2 TextureOrigin { get { return new Vector2(width / 2, depth / 2); } }
        internal int X { get { return (int)Position.X; } }
        internal int Y { get { return (int)Position.Y; } }
        internal int NegX { get { return X - (ScaledWidth / 2); } }
        internal int PosX { get { return X + (ScaledWidth / 2); } }
        internal int NegY { get { return Y - (ScaledDepth / 2); } }
        internal int PosY { get { return Y + (ScaledDepth / 2); } }
        internal bool Visible
        {
            get
            {
                if (Batch != null)
                    return ((NegX > ViewNegX && NegX < ViewPosX) || (PosX > ViewNegX && PosX < ViewPosX)) &&
                           ((NegY > ViewNegY && NegY < ViewPosY) || (PosY > ViewNegX && PosY < ViewPosY));
                else
                    return false;
            }
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        public Sprite(Texture2D _texture)
        {
            texture = new SpriteTexture(_texture, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = new Vector2(0, 0);
            actualPosition = new Vector2(X, Y);
            width = iWidth;
            depth = iDepth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        public Sprite(Texture2D _texture, int _frames, int _modes)
        {
            texture = new SpriteTexture(_texture, _frames, _modes, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = new Vector2(0, 0);
            actualPosition = new Vector2(X, Y);
            width = iWidth;
            depth = iDepth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        public Sprite(Texture2D _texture, int _frames, int _modes, Vector2 _position)
        {
            texture = new SpriteTexture(_texture, _frames, _modes, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            width = iWidth;
            depth = iDepth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Sprite(Texture2D _texture, int _frames, int _modes, Vector2 _position, int _width, int _depth)
        {
            texture = new SpriteTexture(_texture, _frames, _modes, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            Width = _width;
            Depth = _depth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        /// <param name="_scale">Scale of sprite</param>
        public Sprite(Texture2D _texture, int _frames, int _modes, Vector2 _position, int _width, int _depth, float _scale)
        {
            texture = new SpriteTexture(_texture, _frames, _modes, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            Width = _width;
            Depth = _depth;
            Scale = _scale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        /// <param name="_scale">Scale of sprite</param>
        public Sprite(Texture2D _texture, Vector2 _position, int _width, int _depth, float _scale)
        {
            texture = new SpriteTexture(_texture, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            Width = _width;
            Depth = _depth;
            Scale = _scale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Sprite(Texture2D _texture, Vector2 _position, int _width, int _depth)
        {
            texture = new SpriteTexture(_texture, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            Width = _width;
            Depth = _depth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Constructor for Sprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        public Sprite(Texture2D _texture, Vector2 _position)
        {
            texture = new SpriteTexture(_texture, this);
            iScale = 1;
            iWidth = texture.Width;
            iDepth = texture.Depth;
            position = _position;
            actualPosition = new Vector2(X, Y);
            width = iWidth;
            depth = iDepth;
            scale = iScale;
            rotation = 0;
        }

        /// <summary>
        /// Sets the layer the sprite is to be drawn in
        /// </summary>
        protected virtual void SetLayer()
        {
            texture.Layer = position.Y / ViewPosY;
        }

        /// <summary>
        /// Sets the layer the sprite is to be drawn in
        /// </summary>
        /// <param name="_batch">SpriteBatch objects used for information when deciding layer</param>
        protected virtual void SetLayer(SpriteBatch _batch)
        {
            texture.Layer = position.Y / _batch.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="_batch">SpriteBatch object</param>
        public void Draw()
        {
            if (Visible)
            {
                SetLayer();
                texture.Draw();
            }
        }

        /// <summary>
        /// Draws the sprite
        /// </summary>
        /// <param name="_batch">SpriteBatch object</param>
        public void Draw(SpriteBatch _batch)
        {
            Rectangle _bounds = _batch.GraphicsDevice.Viewport.Bounds;
            if (((NegX > _bounds.Left && NegX < _bounds.Right) || (PosX > _bounds.Left && PosX < _bounds.Right)) &&
                       ((NegY > _bounds.Top && NegY < _bounds.Bottom) || (PosY > _bounds.Top && PosY < _bounds.Bottom)))
            {
                SetLayer(_batch);
                texture.Draw(_batch);
            }
        }

        /// <summary>
        /// Adjusts position to be relative to a specified point
        /// </summary>
        /// <param name="_point">Specified point</param>
        public virtual void AdjustPosition(Vector2 _point)
        {
            //Adjusts position to current world position
            //Not nessecarily correct but it doesn't matter because it is only wrong when character is not viewable
            position = actualPosition + _point;
        }

        /// <summary>
        /// Adjusts position to be relative to the upper left coordinates of an object
        /// positioned at _position and with the origin _origin
        /// Adjusts to be relative to point = (_position - _origin)
        /// </summary>
        /// <param name="_position">Position of object</param>
        /// <param name="_origin">Origin of object</param>
        public virtual void AdjustPosition(Vector2 _position, Vector2 _origin)
        {
            //Adjusts position to current world position
            //Not nessecarily correct but it doesn't matter because it is only wrong when character is not viewable
            position = actualPosition + _position - _origin;
        }

        /// <summary>
        /// Updates the sprite during the game
        /// </summary>
        /// <param name="_time">GameTime object</param>
        public void Update(GameTime _time);

        /// <summary>
        /// A sprite texture object that can have multiple modes and animation frames
        /// Different rows in the texture grid are considered modes
        /// Different columns in the texture grid are considered columns
        /// </summary>
        internal class SpriteTexture
        {
            readonly Texture2D TEXTURE_GRID;
            readonly int FRAMES, MODES, WIDTH, DEPTH;
            readonly Sprite SPRITE;
            int frame, mode;
            Color drawColor;
            SpriteEffects effects;
            float layer;
            internal int Width { get { return WIDTH; } }
            internal int Depth { get { return DEPTH; } }
            internal int Frame { get { return frame; } set { frame = value % FRAMES; } }
            internal int Mode { get { return mode; } set { mode = value % MODES; } }
            internal Color DrawColor { get { return drawColor; } set { drawColor = value; } }
            internal SpriteEffects Effects { get { return effects; } set { effects = value; } }
            internal float Layer { get { return layer; } set { layer = value < 0 ? 0 : (value > 1 ? 1 : value); } }
            protected Rectangle Source { get { return new Rectangle((WIDTH * frame) - WIDTH + SPRITE.Width, (DEPTH * mode) - DEPTH + SPRITE.Depth, SPRITE.Width, SPRITE.Depth); } }

            /// <summary>
            /// Constructor for a SpriteTexture object with only one mode and animation frame
            /// </summary>
            /// <param name="_textureGrid">Texture grid</param>
            /// <param name="_sprite">Sprite using the SpriteTexture</param>
            internal SpriteTexture(Texture2D _textureGrid, Sprite _sprite)
            {
                TEXTURE_GRID = _textureGrid;
                SPRITE = _sprite;
                FRAMES = 1;
                MODES = 1;
                WIDTH = TEXTURE_GRID.Width / FRAMES;
                DEPTH = TEXTURE_GRID.Height / FRAMES;
                frame = 0;
                mode = 0;
                drawColor = Color.White;
                effects = SpriteEffects.None;
                layer = 0;
            }

            /// <summary>
            /// Constructor for a SpriteTexture object with multiple modes and animation frames
            /// </summary>
            /// <param name="_textureGrid">Texture grid</param>
            /// <param name="_frames">Animation frames in the SpriteTexture</param>
            /// <param name="_modes">Modes in the SpriteTexture</param>
            /// <param name="_sprite">Sprite using the SpriteTexture</param>
            internal SpriteTexture(Texture2D _textureGrid, int _frames, int _modes, Sprite _sprite)
            {
                TEXTURE_GRID = _textureGrid;
                SPRITE = _sprite;
                FRAMES = _frames;
                MODES = _modes;
                WIDTH = TEXTURE_GRID.Width / FRAMES;
                DEPTH = TEXTURE_GRID.Height / FRAMES;
                frame = 0;
                mode = 0;
                drawColor = Color.White;
                effects = SpriteEffects.None;
                layer = 0;
            }

            /// <summary>
            /// Draws the texture using static SpriteBatch object
            /// </summary>
            internal void Draw()
            {
                Batch.Draw(TEXTURE_GRID, SPRITE.Position, Source, drawColor, SPRITE.Rotation, SPRITE.TextureOrigin, SPRITE.Scale, effects, layer);
            }

            /// <summary>
            /// Draws the texture using given SpriteBatch object
            /// </summary>
            /// <param name="_batch">SpriteBatch object</param>
            internal void Draw(SpriteBatch _batch)
            {
                _batch.Draw(TEXTURE_GRID, SPRITE.Position, Source, drawColor, SPRITE.Rotation, SPRITE.TextureOrigin, SPRITE.Scale, effects, layer); 
            }

            /// <summary>
            /// Checks if texture is currently on last animation frame
            /// </summary>
            /// <returns>True if currently on last frame</returns>
            protected bool LastFrame()
            {
                return frame >= FRAMES - 1;
            }

            /// <summary>
            /// Increments the animation frame
            /// If last frame, loops to first frame
            /// </summary>
            public void NextFrame()
            {
                    Frame++;
            }

            /// <summary>
            /// Increments the animation frame
            /// If on last frame does not increment
            /// </summary>
            public void NextFrameNoLoop()
            {
                if (!LastFrame())
                    Frame++;
            }
        }
    }
}
