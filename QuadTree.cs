using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MDMazeRunner
{
    /// <summary>
    /// A class that handles collision dectection between CollidableSprite objects
    /// </summary>
    class QuadTree
    {
        Node root;
        readonly Rectangle bounds;

        /// <summary>
        /// Constructor for QuadTree
        /// </summary>
        /// <param name="_bounds">Viewable boundaries (possibly unneeded)</param>
        /// <param name="_sprites">List of CollidableSprite objects</param>
        /// <param name="_texture">Background of QuadTree (UNNEEDED)</param>
        public QuadTree(Rectangle _bounds, List<Sprites.CollidableSprite> _sprites)
        {
            bounds = _bounds;
            root = Node.CreateRootNode(bounds.Width, bounds.Height, _sprites);
        }

        /// <summary>
        /// Resets tree using a new list of CollidableSprite objects
        /// </summary>
        /// <param name="_sprites">List of CollidableSprite objects</param>
        public void Reset(List<Sprites.CollidableSprite> _sprites)
        {
            root = Node.CreateRootNode(bounds.Width, bounds.Height, _sprites);
        }

        /// <summary>
        /// Adds a sprite to the QuadTree
        /// </summary>
        /// <param name="_sprite">Sprite to add</param>
        public void Add(Sprites.CollidableSprite _sprite)
        {
            root.Add(_sprite);
        }

        /// <summary>
        /// Adds multiple sprites to the QuadTree
        /// </summary>
        /// <param name="_sprite">List of sprites to add</param>
        public void Add(List <Sprites.CollidableSprite> _sprites)
        {
            root.Add(_sprites);
        }

        /// <summary>
        /// Method used to update QuadTree object according to the positions of its CollidableSprite objects
        /// Also checks for collisions
        /// </summary>
        public void Update()
        {
            //Update tree
            root.Update();

            //Check for sprite collisions
            root.CheckCollisions();
        }

        /// <summary>
        /// Draws all CollidableSprite objects in QuadTree
        /// </summary>
        /// <param name="_batch">Spritebatch</param>
        public void Draw(SpriteBatch _batch)
        {
            root.Draw(_batch);
        }

        /// <summary>
        /// A class defining the nodes of a QuadTree object
        /// </summary>
        class Node
        {
            readonly int width, height;
            readonly Node parent;
            readonly Vector2 centerPoint;
            Node[,] subsections;
            Sprites.CollidableSprite[] sprites;
            public int W { get { return width; } }
            public int H { get { return height; } }
            public int CenterX { get { return (int)centerPoint.X; } }
            public int CenterY { get { return (int)centerPoint.Y; } }
            public int NegX { get { return (int)centerPoint.X - width / 2; } }
            public int PosX { get { return (int)centerPoint.X + width / 2; } }
            public int NegY { get { return (int)centerPoint.Y - height / 2; } }
            public int PosY { get { return (int)centerPoint.Y + height / 2; } }
            /// <summary>
            /// Array of CollidableSprite objects contained in Node
            /// </summary>
            public Sprites.CollidableSprite[] Sprites { get { return sprites; } }
            /// <summary>
            /// Array of CollidableSprite objects contained in subsections of Node
            /// </summary>
            public Sprites.CollidableSprite[] SubSprites
            {
                get
                {
                    if (subsections != null)
                    {
                        List<Sprites.CollidableSprite> _subSprites = new List<Sprites.CollidableSprite>();
                        for (int a = 0; a < 2; a++)
                            for (int b = 0; b < 2; b++)
                                if (subsections[a, b].AllSprites != null)
                                    _subSprites.AddRange(subsections[a, b].AllSprites);
                        return _subSprites.ToArray();
                    }
                    return null;
                }
            }
            /// <summary>
            /// Array of CollidableSprite objects contained in Node and subsections of Node
            /// </summary>
            public Sprites.CollidableSprite[] AllSprites
            {
                get
                {
                    List<Sprites.CollidableSprite> _allSprites = new List<Sprites.CollidableSprite>();
                    if (Sprites != null)
                        _allSprites.AddRange(Sprites);
                    if (SubSprites != null)
                        _allSprites.AddRange(SubSprites);
                    return _allSprites.ToArray();
                }
            }
            public Sprites.CollidableSprite[] LeftwardSprites
            {
                get
                {
                    List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();

                    if (sprites != null)
                        _sprites.AddRange(sprites);

                    if (subsections != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            _sprites.AddRange(subsections[0, i].LeftwardSprites);
                        }
                    }
                    return _sprites.ToArray();
                }
            }
            public Sprites.CollidableSprite[] RightwardSprites
            {
                get
                {
                    List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();

                    if (sprites != null)
                        _sprites.AddRange(sprites);

                    if (subsections != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            _sprites.AddRange(subsections[1, i].RightwardSprites);
                        }
                    }
                    return _sprites.ToArray();
                }
            }
            public Sprites.CollidableSprite[] UpwardSprites
            {
                get
                {
                    List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();

                    if (sprites != null)
                        _sprites.AddRange(sprites);

                    if (subsections != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            _sprites.AddRange(subsections[i, 0].UpwardSprites);
                        }
                    }
                    return _sprites.ToArray();
                }
            }
            public Sprites.CollidableSprite[] DownwardSprites
            {
                get
                {
                    List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();

                    if (sprites != null)
                        _sprites.AddRange(sprites);

                    if (subsections != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            _sprites.AddRange(subsections[i, 1].DownwardSprites);
                        }
                    }
                    return _sprites.ToArray();
                }
            }
            public Rectangle Bounds { get { return new Rectangle(NegX, NegY, W, H); } }

            /// <summary>
            /// Constructor, used to create root node
            /// </summary>
            /// <param name="_w">Width of node</param>
            /// <param name="_h">Height of node</param>
            /// <param name="_sprites">Sprites in node</param>
            private Node(int _w, int _h, Sprites.CollidableSprite[] _sprites)
            {
                width = _w;
                height = _h;
                parent = null;
                centerPoint = new Vector2(width / 2, height / 2);

                subsections = null;

                sprites = null;

                PlaceSprites(new List<Sprites.CollidableSprite>(_sprites));

                if (subsections != null)
                    Console.WriteLine("Root has subsections");
                else
                    Console.WriteLine("Root does not have subsections");
            }

            /// <summary>
            /// Constructor, used to create root node
            /// </summary>
            /// <param name="_w">Width of node</param>
            /// <param name="_h">Height of node</param>
            /// <param name="_sprite">Sprite in node</param>
            private Node(int _w, int _h, Sprites.CollidableSprite _sprite)
            {
                width = _w;
                height = _h;
                parent = null;
                centerPoint = new Vector2(width / 2, height / 2);

                subsections = null;

                sprites = new Sprites.CollidableSprite[] { _sprite };
            }

            /// <summary>
            /// Constructor, used for creating root node
            /// </summary>
            /// <param name="_w">Width of node</param>
            /// <param name="_h">Height of node</param>
            private Node(int _w, int _h)
            {
                width = _w;
                height = _h;
                parent = null;
                centerPoint = new Vector2(width / 2, height / 2);
                subsections = null;
                sprites = null;
            }

            /// <summary>
            /// Constructor, used to create child nodes
            /// </summary>
            /// <param name="_parent">Parent of node</param>
            /// <param name="_cX">Center X coordinate of node</param>
            /// <param name="_cY">Center Y coordinate of node</param>
            internal Node(Node _parent, int _cX, int _cY)
            {
                parent = _parent;
                width = parent.W / 2;
                height = parent.H / 2;
                centerPoint = new Vector2(_cX, _cY);

                subsections = null;

                sprites = null;
            }

            /// <summary>
            /// Recursively checks for collisions between sprites only in the current node
            /// and sprites in the current node and all subsections
            /// </summary>
            internal void CheckCollisions()
            {
                /*if (sprites != null)
                    foreach (Sprites.CollidableSprite _s in sprites)
                        foreach (Sprites.CollidableSprite _c in AllSprites)
                            if (_s != _c)
                                if (Vector2.Distance(_s.Position, _c.Position) <= (_s.Radius + _c.Radius))
                                    if (_s.Collidable.Intersects(_c.Collidable))
                                        _s.Collision(_c);*/

                if (sprites != null)
                    for (int _s = 0; _s < Sprites.Length; _s++)
                        for (int _a = _s + 1; _a < AllSprites.Length; _a++)
                            if (Vector2.Distance(Sprites[_s].Position, AllSprites[_a].Position) <= (Sprites[_s].Radius + AllSprites[_a].Radius))
                                if (Sprites[_s].Collidable.Intersects(AllSprites[_a].Collidable))
                                    Sprites[_s].Collision(AllSprites[_a]);

                //Check collisions in all subsections if they exist
                if (subsections != null)
                {
                    foreach (Node _n in subsections)
                        _n.CheckCollisions();
                }
            }

            /// <summary>
            /// Adds a sprite to the QuadTree, meant to invoked by root node
            /// </summary>
            /// <param name="_sprite">Sprite to add</param>
            internal void Add(Sprites.CollidableSprite _sprite)
            {
                List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();
                _sprites.Add(_sprite);
                Add(_sprites);
            }

            /// <summary>
            /// Adds multiple sprites to the QuadTree, meant to be invoked by root node
            /// </summary>
            /// <param name="_sprites">List of sprites to add</param>
            internal void Add(List<Sprites.CollidableSprite> _sprites)
            {
                PlaceSprites(_sprites);
                if (_sprites.Count > 0)
                {
                }
            }

            /// <summary>
            /// Update method called using the root node
            /// Calls a private update method on itself which then recursively calls private update method on child nodes
            /// All sprites both completely and partially contained in root node should be kept
            /// Otherwise, sprites will be thrown out and deleted
            /// </summary>
            internal void Update()
            {
                List<Sprites.CollidableSprite> _sprites = new List<Sprites.CollidableSprite>();

                Update(_sprites);

                //Error if sprites left in _sprites
                if (_sprites.Count > 0)
                {
                    if (sprites != null)
                        _sprites.AddRange(sprites);
                    sprites = _sprites.ToArray();
                }
            }

            /// <summary>
            /// Private update method that resurively calls update method on all child nodes
            /// Keeps CollidableSprite objects in a List object and places them
            /// Uses same List object throughout recursions
            /// </summary>
            /// <param name="_sprites">List object containing CollidableSprite objects</param>
            private void Update(List<Sprites.CollidableSprite> _sprites)
            {
                //Calls update on each subsection
                if (subsections != null)
                {
                    foreach (Node _s in subsections)
                        _s.Update(_sprites);
                }

                //Check if node currently has sprites, act accordingly
                if (sprites != null)
                {
                    List<Sprites.CollidableSprite> _retainedSprites = new List<Sprites.CollidableSprite>();
                    foreach (Sprites.CollidableSprite _s in sprites)
                    {
                        //If a sprite is contained in current node and not possible to contain in child node,
                        //add sprite to retained sprites. o Else, add to passed in sprite list
                        if (InNode(_s) && !InSubsection(_s))
                            _retainedSprites.Add(_s);
                        else
                            _sprites.Add(_s);
                    }
                    //Keep retained sprites in current cell
                    sprites = _retainedSprites.ToArray();
                }

                //Place, or attempt to place, passed in sprites
                PlaceSprites(_sprites);

                //Check for subsections
                if (SubSprites != null)
                {
                    //In no sprites in subsections, delete subsections
                    if (SubSprites.Length == 0)
                    {
                        subsections = null;
                    }
                    else
                    {
                        //If only one sprite in subsection, and no sprites in current node, add sprite to current node and delete subsections
                        if (SubSprites.Length == 1 && sprites == null)
                        {
                            sprites = SubSprites;
                            subsections = null;
                        }
                    }
                }
            }

            /// <summary>
            /// Checks if the collidable portion of a sprite is contained inside current node
            /// </summary>
            /// <param name="_sprite">Sprite to check</param>
            /// <returns>True if sprite is contained inside current node</returns>
            private bool InNode(Sprites.CollidableSprite _sprite)
            {
                return _sprite.CollidableNegX > NegX &&
                       _sprite.CollidablePosX < PosX &&
                       _sprite.CollidableNegY > NegY &&
                       _sprite.CollidablePosY < PosY;
            }

            /// <summary>
            /// Checks if the collide portion of a sprite is contained inside a subsection of current node
            /// </summary>
            /// <param name="_sprite">Sprite to check</param>
            /// <returns>True if sprite is contained inside a subsection of current node</returns>
            private bool InSubsection(Sprites.CollidableSprite _sprite)
            {
                return (_sprite.CollidablePosX < CenterX || _sprite.CollidableNegX > CenterX) &&
                       (_sprite.CollidablePosY < CenterY || _sprite.CollidableNegY > CenterY);
            }

            /// <summary>
            /// Checks if any visible portion of a sprite intersects current node
            /// </summary>
            /// <param name="_sprite">Sprite to check</param>
            /// <returns>True if sprite is intersects current node</returns>
            private bool IntersectsNode(Sprites.CollidableSprite _sprite)
            {
                return ((_sprite.NegX > NegX && _sprite.NegX < PosX) || (_sprite.PosX > NegX && _sprite.PosX < PosX)) &&
                       ((_sprite.NegY > NegY && _sprite.NegY < PosY) || (_sprite.PosY > NegX && _sprite.PosY < PosY));
            }

            /// <summary>
            /// Places sprites into nodes according to their positions
            /// </summary>
            /// <param name="_sprites">Sprites to be placed</param>
            private void PlaceSprites(List<Sprites.CollidableSprite> _sprites)
            {
                //Return if _sprites is empty
                if (_sprites.Count == 0)
                    return;

                List<Sprites.CollidableSprite> _inNode = new List<Sprites.CollidableSprite>(_sprites.FindAll(s => InNode(s)));
                _sprites.RemoveAll(s => InNode(s));
                List<Sprites.CollidableSprite> _inSub = new List<Sprites.CollidableSprite>(_inNode.FindAll(s => InSubsection(s)));
                _inNode.RemoveAll(s => InSubsection(s));

                //If has no parent and _sprites is not empty, add all sprites intersecting node to _inNode
                //Then remove intercecting sprites from _sprites
                if (parent == null && _sprites.Count > 0)
                {
                    _inNode.AddRange(_sprites.FindAll( s => IntersectsNode(s)));
                    _sprites.RemoveAll( s => IntersectsNode(s));
                }
                

                //Check if needing to place sprites into subsections
                if ((_inSub.Count + _inNode.Count) > 1 && _inSub.Count > 0)
                {
                    //Check for existance of subsections
                    if (subsections != null)
                    {
                        foreach (Node _n in subsections)
                            _n.PlaceSprites(_inSub);
                    }
                    else
                    {
                        CreateSubsections(_inSub);
                    }
                }

                //Move all sprites remaining in _inSub to _inSection
                _inNode.AddRange(_inSub);

                //Check and return if _inSection is empty
                if (_inNode.Count == 0)
                    return;

                //Check if currently sprites in node, if so, add to _inSection
                if (sprites != null)
                    _inNode.AddRange(sprites);

                //Get array from _inNode if _inNode.Count > 0, otherwise sprites = null
                if (_inNode.Count > 0)
                    sprites = _inNode.ToArray();
                else
                    sprites = null;
            }

            /// <summary>
            /// Creates subsections for a Node and puts sprites in subsections
            /// </summary>
            /// <param name="_sprites">Puts sprites in subsections</param>
            private void CreateSubsections(List<Sprites.CollidableSprite> _sprites)
            {
                subsections = new Node[2, 2];
                /*for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                    {
                        subsections[x, y] = new Node(this, CenterX + ((x == 0 ? -1 : 1) * W / 4), CenterY + ((y == 0 ? -1 : 1) * H / 4));
                        subsections[x, y].PlaceSprites(_sprites);
                    }*/
                subsections[0, 0] = new Node(this, CenterX - (W / 4), CenterY - (H / 4));
                subsections[0, 0].PlaceSprites(_sprites);
                subsections[1, 0] = new Node(this, CenterX + (W / 4), CenterY - (H / 4));
                subsections[1, 0].PlaceSprites(_sprites);
                subsections[0, 1] = new Node(this, CenterX + (W / 4), CenterY + (H / 4));
                subsections[0, 1].PlaceSprites(_sprites);
                subsections[1, 1] = new Node(this, CenterX - (W / 4), CenterY + (H / 4));
                subsections[1, 1].PlaceSprites(_sprites);
            }

            /// <summary>
            /// Draw each sprite in subsection
            /// </summary>
            /// <param name="_batch">SpriteBatch</param>
            internal void Draw(SpriteBatch _batch)
            {
                //_batch.Draw(texture, Bounds, null, BG);
                if (subsections != null)
                    foreach (Node _s in subsections)
                        _s.Draw(_batch);
            }

            /// <summary>
            /// Static method to create a root node
            /// </summary>
            /// <param name="_w">Width of root node</param>
            /// <param name="_h">Height of root node</param>
            /// <param name="_sprites">Sprites in root node</param>
            /// <returns>New node</returns>
            internal static Node CreateRootNode(int _w, int _h, List<Sprites.CollidableSprite> _sprites)
            {
                if (_sprites.Count == 1)
                    return new Node(_w, _h, _sprites.First());
                if (_sprites.Count > 1)
                    return new Node(_w, _h, _sprites.ToArray());
                return new Node(_w, _h);
            }
        }
    }
}
