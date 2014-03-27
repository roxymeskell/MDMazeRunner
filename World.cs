using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MDMazeRunner.Maze_Generation;
using MDMazeRunner.Sprites;

namespace MDMazeRunner
{
    /// <summary>
    /// A static class defining the visiable world of the maze
    /// Keeps track of all bounds, floors, and other sprites
    /// </summary>
    static class World
    {
        static readonly int X = 0, Y = 1, Z = 2;
        static readonly int MIN_INTERIOR_SCALE = 3, MIN_BOUND_SCALE = 1, MIN_PLAYER_SCALE = 1;
        static List<TileSprite> grid;
        static Vector2 worldPosition, worldOrigin, finalPosition;
        static Texture2D[] floorTextures, boundTextures, ascendingTextures, descendingTextures;
        static int? playerScale; //In pixels (player.Width)
        static int? interiorScale, boundScale, openingScale; //Relative to playerScale (player.Width)
        static int[] centerCell, currDimensions, centerTile;
        static int cellScale, tileSize, cellSize;

        /// <summary>
        /// Current X dimension
        /// </summary>
        public static int DimensionX { get { return currDimensions[X]; } }
        /// <summary>
        /// Current Y dimension
        /// </summary>
        public static int DimensionY { get { return currDimensions[Y]; } }
        /// <summary>
        /// Current Z dimension
        /// </summary>
        public static int DimensionZ { get { return currDimensions[Z]; } }

        public static Texture2D[] Floors
        {
            set
            {
                if (floorTextures == null)
                    floorTextures = new Texture2D[Maze.Dimensions];
                else
                    Array.Resize(ref floorTextures, Maze.Dimensions);
                floorTextures.Initialize();
                Array.Copy(value, floorTextures, value.Length < floorTextures.Length ? value.Length : floorTextures.Length);
            }
        }
        public static Texture2D[] Bounds
        {
            set
            {
                if (boundTextures == null)
                    boundTextures = new Texture2D[Maze.Dimensions];
                else
                    Array.Resize(ref boundTextures, Maze.Dimensions);
                boundTextures.Initialize();
                Array.Copy(value, boundTextures, value.Length < boundTextures.Length ? value.Length : boundTextures.Length);
            }
        }
        public static Texture2D[] Ascendings
        {
            set
            {
                if (ascendingTextures == null)
                    ascendingTextures = new Texture2D[Maze.Dimensions];
                else
                    Array.Resize(ref ascendingTextures, Maze.Dimensions);
                ascendingTextures.Initialize();
                Array.Copy(value, ascendingTextures, value.Length < ascendingTextures.Length ? value.Length : ascendingTextures.Length);
            }
        }
        public static Texture2D[] Descendings
        {
            set
            {
                if (descendingTextures == null)
                    descendingTextures = new Texture2D[Maze.Dimensions];
                else
                    Array.Resize(ref ascendingTextures, Maze.Dimensions);
                descendingTextures.Initialize();
                Array.Copy(value, descendingTextures, value.Length < descendingTextures.Length ? value.Length : descendingTextures.Length);
            }
        }
        public static Texture2D Floor { get { return floorTextures[CurrentDimensions[X]]; } }
        public static Texture2D Bound { get { return boundTextures[CurrentDimensions[Y]]; } }
        public static Texture2D Ascending { get { return ascendingTextures[CurrentDimensions[Z]]; } }
        public static Texture2D Descending { get { return descendingTextures[CurrentDimensions[Z]]; } }
        
        public static int PlayerScale
        {
            get { return playerScale.HasValue ? playerScale.Value : MIN_PLAYER_SCALE; }
            set { playerScale = value > 0 ? value : PlayerScale; }
        }
        public static int InteriorScale
        {
            get { return interiorScale.HasValue ? interiorScale.Value * PlayerScale : MIN_INTERIOR_SCALE * PlayerScale; }
            set
            {
                interiorScale = value / PlayerScale < MIN_INTERIOR_SCALE && value > OpeningScale * 2 ? InteriorScale / PlayerScale:
                    ((value / PlayerScale) % 2 != 1 ? (value / PlayerScale) + 1 : (value / PlayerScale));
            }
        }
        public static int BoundScale
        {
            get { return boundScale.HasValue ? boundScale.Value * PlayerScale : MIN_BOUND_SCALE * PlayerScale; }
            set { boundScale = value / PlayerScale < MIN_BOUND_SCALE ? BoundScale / PlayerScale : value / PlayerScale; }
        }
        public static int OpeningScale
        {
            get { return openingScale.HasValue ? openingScale.Value : (int)Math.Floor((double)InteriorScale / 2) * PlayerScale; }
            set { interiorScale = value / PlayerScale > (int)Math.Floor((double)InteriorScale / 2) ? OpeningScale / PlayerScale :  value / PlayerScale; }
        }
        public static int CellScale { get { return InteriorScale + BoundScale; } }
        public static Vector2 WorldPosition { get { return worldPosition == null ? Vector2.Zero : worldPosition; } set { worldPosition = value; } }
        public static Vector2 WorldOrigin { get { return worldOrigin == null ? Vector2.Zero : worldOrigin; } set { worldOrigin = value; } }
        //The point (0, 0) for the World, the upper left corner coordinates
        public static Vector2 World0Point { get { return WorldPosition - WorldOrigin; } }
        public static int[] CurrentDimensions
        {
            get
            {
                if (currDimensions == null)
                    currDimensions = new int[] { X, Y, Z };
                int[] _currD = new int[] { X, Y, Z };
                Array.Copy(currDimensions.Distinct().ToArray(), _currD, currDimensions.Distinct().ToArray().Length < _currD.Length ?
                    currDimensions.Distinct().ToArray().Length : _currD.Length);
                return _currD;
            }
            set
            {
                if (currDimensions == null)
                    currDimensions = new int[] { X, Y, Z };
                if (value == null)
                    return;
                Array.Copy(value.Distinct().ToArray(), currDimensions, value.Distinct().ToArray().Length < currDimensions.Length ?
                    value.Distinct().ToArray().Length : currDimensions.Length);
            }
        }
        public static int[] CenterCell
        {
            get
            {
                if (centerCell == null)
                {
                    centerCell = new int[Maze.Dimensions];
                    centerCell.Initialize();
                }
                return centerCell;
            }
        }

        public static void Initialize()
        {
        }

    }
}
