using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MDMazeRunner.Sprites
{
    /// <summary>
    /// An abstract definition for a collidable sprite that can be interacted with
    /// </summary>
    abstract class Functional : CollidableSprite
    {
        /// <summary>
        /// Constructor for Functional
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY) :
            base(_texture, _negX, _posX, _negY, _posY)
        {
        }

        /// <summary>
        /// Constructor for Functional
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes) :
            base(_texture, _negX, _posX, _negY, _posY, _frames, _modes)
        {
        }

        /// <summary>
        /// Constructor for Functional
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_frames">Animation frames</param>
        /// <param name="_modes">Different modes</param>
        /// <param name="_position">Position of sprite center</param>
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position) :
            base(_texture, _negX, _posX, _negY, _posY, _frames, _modes, _position)
        {
        }

        /// <summary>
        /// Constructor for Functional
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
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position, int _width, int _depth) :
            base(_texture, _negX, _posX, _negY, _posY, _frames, _modes, _position, _width, _depth)
        {
        }

        /// <summary>
        /// Constructor for Functional
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
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, int _frames, int _modes, Vector2 _position, int _width, int _depth, float _scale) :
            base(_texture, _negX, _posX, _negY, _posY, _frames, _modes, _position, _width, _depth, _scale)
        {
        }

        /// <summary>
        /// Constructor for Functional
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
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position, int _width, int _depth, float _scale) :
            base(_texture, _negX, _posX, _negY, _posY, _position, _width, _depth, _scale)
        {
        }

        /// <summary>
        /// Constructor for Functional
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_position">Position of sprite center</param>
        /// <param name="_width">Width of sprite</param>
        /// <param name="_depth">Depth of sprite</param>
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position, int _width, int _depth) :
            base(_texture, _negX, _posX, _negY, _posY, _position, _width, _depth)
        {
        }

        /// <summary>
        /// Constructor for Functional
        /// </summary>
        /// <param name="_texture">Texture for sprite</param>
        /// <param name="_negX">Scale in negative X direction that is noncollidable</param>
        /// <param name="_posX">Scale in positive X direction that is noncollidable</param>
        /// <param name="_negY">Scale in negative Y direction that is noncollidable</param>
        /// <param name="_posY">Scale in positive Y direction that is noncollidable</param>
        /// <param name="_position">Position of sprite center</param>
        public Functional(Texture2D _texture, int _negX, int _posX, int _negY, int _posY, Vector2 _position) :
            base(_texture, _negX, _posX, _negY, _posY, _position)
        {
        }

        public override void Collision(CollidableSprite _sprite)
        {
            if (_sprite is Character)
                ((Character)_sprite).FunctionalCollision(this);
        }
    }
}

