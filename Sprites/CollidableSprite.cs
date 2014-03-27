using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// An abstract definition of an collidable sprite
    /// </summary>
    abstract class CollidableSprite : Sprite
    {
        static readonly int ROTATION;
        readonly int iNegXNoncollide, iPosXNoncollide, iNegYNoncollide, iPosYNoncollide, iCollidableWidth, iCollidableDepth;
        protected override float Rotation { get { return ROTATION; } set { base.Rotation = ROTATION; } }
        internal override int Width
        {
            get { return width + iNegXNoncollide + iPosXNoncollide; }
            set
            {
                width = value - (iNegXNoncollide + iPosXNoncollide) <= 0 ? 1 : (value - (iNegXNoncollide + iPosXNoncollide) > iCollidableWidth ?
                iCollidableWidth : value - (iNegXNoncollide + iPosXNoncollide));
            }
        }
        internal override int Depth
        {
            get { return depth + iNegYNoncollide + iPosYNoncollide; }
            set
            {
                depth = value - (iNegYNoncollide + iPosYNoncollide) <= 0 ? 1 : (value - (iNegYNoncollide + iPosYNoncollide) > iCollidableDepth ?
                iCollidableDepth : value - (iNegYNoncollide + iPosYNoncollide));
            }
        }
        internal int CollidableWidth { get { return (int)(width * scale); } set { width = (value / scale) <= 0 ? 1 : ((value / scale) > iCollidableWidth ? iCollidableWidth : (int)(value / scale)); } }
        internal int CollidableDepth { get { return (int)(depth * scale); } set { depth = (value / scale) <= 0 ? 1 : ((value / scale) > iCollidableDepth ? iCollidableDepth : (int)(value / scale)); } }
        protected Vector2 CollidableOrigin { get { return new Vector2(CollidableWidth / 2, CollidableDepth / 2); } }
        internal Rectangle Collidable { get { return new Rectangle((int)(Position.X - CollidableOrigin.X), (int)(Position.Y - CollidableOrigin.Y), CollidableWidth, CollidableDepth); } }
        internal int Radius { get { return (int)Math.Sqrt((CollidableWidth * CollidableWidth) + (CollidableDepth * CollidableDepth)); } }
        internal int CollidableNegX { get { return X - (CollidableWidth / 2); } }
        internal int CollidablePosX { get { return X + (CollidableWidth / 2); } }
        internal int CollidableNegY { get { return Y - (CollidableDepth / 2); } }
        internal int CollidablePosY { get { return Y + (CollidableDepth / 2); } }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY) :
            base(_texture)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes) :
            base(_texture, _frames, _modes)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position) :
            base(_texture, _frames, _modes, _position)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position, int _width, int _depth) :
            base(_texture, _frames, _modes, _position, _width, _depth)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        /// <param name="_scale">Scale of sprite</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position, int _width, int _depth, float _scale) :
            base(_texture, _frames, _modes, _position, _width, _depth, _scale)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        /// <param name="_scale">Scale of sprite</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position, int _width, int _depth, float _scale) :
            base(_texture, _position, _width, _depth, _scale)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position, int _width, int _depth) :
            base(_texture, _position, _width, _depth)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Constructor for CollidableSprite
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_position">Position of sprite center</param>
        public CollidableSprite(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position) :
            base (_texture, _position)
        {
            iNegXNoncollide = _negX;
            iPosXNoncollide = _posX;
            iNegYNoncollide = _negY;
            iPosXNoncollide = _posY;
            iCollidableWidth = iWidth - (iNegXNoncollide + iPosXNoncollide);
            iCollidableDepth = iDepth - (iNegYNoncollide + iPosYNoncollide);
            width -= iNegXNoncollide + iPosXNoncollide;
            depth -= iNegYNoncollide + iPosYNoncollide;
        }

        /// <summary>
        /// Detects if a collision occurs with the specified CollidableSprite and takes appropriate action
        /// </summary>
        /// <param name="_sprite">Specified CollidableSprite</param>
        public abstract void Collision(CollidableSprite _sprite);
    }
}
