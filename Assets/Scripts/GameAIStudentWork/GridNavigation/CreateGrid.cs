// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse {

    public class CreateGrid
    {

        // Please change this string to your name
        public const string StudentAuthorName = "Sebastian Brumm";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if point p is inside (or on edge) the polygon defined by pts (CCW winding). False, otherwise
        static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {
            return CG.InPoly1(pts, p) != CG.PointPolygonIntersectionType.Outside;
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        static int Convert(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        // IsPointInsideBoundingBox(): Determines whether a point (Vector2Int:p) is On/Inside a bounding box (such as a grid cell) defined by
        // minCellBounds and maxCellBounds (both Vector2Int's).
        // Returns true if the point is ON/INSIDE the cell and false otherwise
        // This method should return true if the point p is on one of the edges of the cell.
        // This is more efficient than PointInsidePolygon() for an equivalent dimension poly
        // Preconditions: minCellBounds <= maxCellBounds, per dimension
        static bool IsPointInsideAxisAlignedBoundingBox(Vector2Int minCellBounds, Vector2Int maxCellBounds, Vector2Int p)
        {
            if (minCellBounds.x <= p.x && p.x <= maxCellBounds.x)
            {
                if (minCellBounds.y <= p.y && p.y <= maxCellBounds.y)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }




        // IsRangeOverlapping(): Determines if the range (inclusive) from min1 to max1 overlaps the range (inclusive) from min2 to max2.
        // The ranges are considered to overlap if one or more values is within the range of both.
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1 AND min2 <= max2
        static bool IsRangeOverlapping(int min1, int max1, int min2, int max2)
        {
            if (min1 <= min2 && min2 <= max1)
            {
                return true;
            }
            else if (min1 <= max2 && max2 <= max1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // IsAxisAlignedBouningBoxOverlapping(): Determines if the AABBs defined by min1,max1 and min2,max2 overlap or touch
        // Returns true if overlap, false otherwise.
        // Preconditions: min1 <= max1, per dimension. min2 <= max2 per dimension
        static bool IsAxisAlignedBoundingBoxOverlapping(Vector2Int min1, Vector2Int max1, Vector2Int min2, Vector2Int max2)
        {

            if (IsRangeOverlapping(min1.x, max1.x, min2.x, max2.x) && IsRangeOverlapping(min1.y, max1.y, min2.y, max2.y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        // IsTraversable(): returns true if the grid is traversable from grid[x,y] in the direction dir, false otherwise.
        // The grid boundaries are not traversable. If the grid position x,y is itself not traversable but the grid cell in direction
        // dir is traversable, the function will return false.
        // returns false if the grid is null, grid rank is not 2 dimensional, or any dimension of grid is zero length
        // returns false if x,y is out of range
        // Note: public methods are autograded
        public static bool IsTraversable(bool[,] grid, int x, int y, TraverseDirection dir)
        {

            if (grid == null)
            {
                return false;
            }
            else if (grid.Rank != 2)
            {
                return false;
            }
            else if (grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
            {
                return false;
            }
            else if (grid.GetLength(0) < x || grid.GetLength(1) < y)
            {
                return false;
            }
            else
            {
                if (grid[x, y] == false)
                {
                    return false;
                }
                else if (grid.GetLength(0) == (x + 1) && (dir == TraverseDirection.Right || dir == TraverseDirection.UpRight || dir == TraverseDirection.DownRight))
                {
                    return false;
                }
                else if (x == 0 && (dir == TraverseDirection.Left || dir == TraverseDirection.UpLeft || dir == TraverseDirection.DownLeft))
                {
                    return false;
                }
                else if (grid.GetLength(1) == (y + 1) && (dir == TraverseDirection.Up || dir == TraverseDirection.UpRight || dir == TraverseDirection.UpLeft))
                {
                    return false;
                }
                else if (y == 0 && (dir == TraverseDirection.Down || dir == TraverseDirection.DownLeft || dir == TraverseDirection.DownRight))
                {
                    return false;
                }
                else
                {
                    if (dir == TraverseDirection.Left && grid[x - 1, y] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.Right && grid[x + 1, y] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.Down && grid[x, y - 1] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.Up && grid[x, y + 1] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.UpRight && grid[x + 1, y + 1] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.DownRight && grid[x + 1, y - 1] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.UpLeft && grid[x - 1, y + 1] == true)
                    {
                        return true;
                    }
                    else if (dir == TraverseDirection.DownLeft && grid[x - 1, y - 1] == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }


        // Create(): Creates a grid lattice discretized space for navigation.
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // cellWidth: target cell width (of a grid cell) in world dimensions
        // obstacles: a list of collider obstacles
        // grid: an array of bools. A cell is true if navigable, false otherwise
        //    Example: grid[x_pos, y_pos]

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth,
            List<Polygon> obstacles,
            out bool[,] grid
            )
        {
            // ignoring the obstacles for this limited demo; 
            // Marks cells of the grid untraversable if geometry intersects interior!
            // Carefully consider all possible geometry interactions

            // also ignoring the world boundary defined by canvasOrigin and canvasWidth and canvasHeight


            grid = new bool[Convert(canvasWidth/cellWidth)/1000, Convert(canvasHeight / cellWidth) / 1000];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = true;
                }
            }
            List<Vector2Int[]> converted = new List<Vector2Int[]>();
            foreach (Polygon obstacle in obstacles)
            {
                converted.Add(obstacle.getIntegerPoints());
            }
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Vector2Int cellMin1 = new Vector2Int(Convert(canvasOrigin.x) + Convert(cellWidth*i) + 1, Convert(canvasOrigin.y) + Convert(cellWidth * j) + 1);
                    Vector2Int cellMax1 = new Vector2Int(Convert(canvasOrigin.x) + Convert(cellWidth * (i+1)) - 1, Convert(canvasOrigin.y) + Convert(cellWidth * (j+1)) - 1);
                    Vector2Int cellMin2 = new Vector2Int(Convert(canvasOrigin.x) + Convert(cellWidth * i) + 1, Convert(canvasOrigin.y) + Convert(cellWidth * (j + 1)) - 1);
                    Vector2Int cellMax2 = new Vector2Int(Convert(canvasOrigin.x) + Convert(cellWidth * (i + 1)) - 1, Convert(canvasOrigin.y) + Convert(cellWidth * j) + 1);
                    foreach (Vector2Int[] obstacle in converted)
                    {
                        for (int k = 0; k < obstacle.GetLength(0); k++)
                        {
                            int point = k - 1;
                            if (k == 0)
                            {
                                point = obstacle.GetLength(0) - 1;
                            }
                            if (IsPointInsideAxisAlignedBoundingBox(cellMin1, cellMax1, obstacle[k]) || IsPointInsidePolygon(obstacle, cellMin1) || IsPointInsidePolygon(obstacle, cellMax1) || IsPointInsidePolygon(obstacle, cellMin2) || IsPointInsidePolygon(obstacle, cellMax2))
                            {
                                grid[i, j] = false;
                                break;
                            }
                            else if (Intersects(cellMin1, cellMin2, obstacle[k], obstacle[point]) || Intersects(cellMin2, cellMax1, obstacle[k], obstacle[point]) || Intersects(cellMax1, cellMax2, obstacle[k], obstacle[point]) || Intersects(cellMax2, cellMin1, obstacle[k], obstacle[point]))
                            {
                                grid[i, j] = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

    }

}