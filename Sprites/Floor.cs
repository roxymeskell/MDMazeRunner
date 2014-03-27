using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// A class representing noncollidable floor sprites
    /// </summary>
    class Floor : Sprite
    {
        static Texture2D _texture_;
        static int? _frames_;
        static int? _modes_;
        static int? _cellScale_;
        public static Texture2D Texture
        {
            get { return _texture_ == null ? new Texture2D(Batch.GraphicsDevice, CellScale * Frames, CellScale * Modes) : _texture_; }
            set { _texture_ = (value.Width / Frames >= CellScale && value.Height / Modes >= CellScale) ? value : Texture; }
        }
        public static int Frames { get { return _frames_.HasValue ? _frames_.Value : 1; } set { _frames_ = value > 0 ? value : 1; } }
        public static int Modes { get { return _modes_.HasValue ? _modes_.Value : 1; } set { _modes_ = value > 0 ? value : 1; } }
        public static int CellScale { get { return _cellScale_.HasValue ? _cellScale_.Value : 4; } set { _cellScale_ = value > 3 ? value : 4; } }

        /// <summary>
        /// Constructor for Floor
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_cellScale">Cell scale in pixels</param>
        public Floor(Texture2D _texture, Vector2 _position, int _cellScale) :
            base(_texture, _position, _cellScale, _cellScale)
        {
        }

        /// <summary>
        /// Constructor for Floor
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_cellScale">Cell scale in pixels</param>
        public Floor(Texture2D _texture, int _frames, int _modes, Vector2 _position, int _cellScale) :
            base(_texture, _frames, _modes, _position, _cellScale, _cellScale)
        {
        }

        /// <summary>
        /// Constructor for Floor
        /// </summary>
        /// <param name="_position">Position of sprite center</param>
        public Floor(Vector2 _position) :
            base(Texture, Frames, Modes, _position, CellScale, CellScale)
        {
        }


        /// <summary>
        /// Sets the layer the sprite is to be drawn in
        /// </summary>
        protected override void SetLayer()
        {
            texture.Layer = 0;
        }

        /// <summary>
        /// Sets the layer the sprite is to be drawn in
        /// </summary>
        /// <param name="_batch">SpriteBatch objects used for information when deciding layer</param>
        protected override void SetLayer(SpriteBatch _batch)
        {
            texture.Layer = 0;
        }
    }
}
