// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse
{

    public class CreatePathNetwork
    {

        public const string StudentAuthorName = "Sebastian Brumm";




        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        public static Vector2Int ConvertToInt(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        public static int ConvertToInt(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2Int converted to Vector2 according to default scaling factor (1000)
        public static Vector2 ConvertToFloat(Vector2Int v)
        {
            float f = 1f / (float)CG.FloatToIntFactor;
            return new Vector2(v.x * f, v.y * f);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns int converted to float according to default scaling factor (1000)
        public static float ConvertToFloat(int v)
        {
            float f = 1f / (float)CG.FloatToIntFactor;
            return v * f;
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2Int point, Vector2Int lineStart, Vector2Int lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }


        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Determines if a point is inside/on a CCW polygon and if so returns true. False otherwise.
        public static bool IsPointInPolygon(Vector2Int[] polyPts, Vector2Int point)
        {
            return CG.PointPolygonIntersectionType.Outside != CG.InPoly1(polyPts, point);
        }

        // Returns true iff p is strictly to the left of the directed
        // line through a to b.
        // You can use this method to determine if 3 adjacent CCW-ordered
        // vertices of a polygon form a convex or concave angle

        public static bool Left(Vector2Int a, Vector2Int b, Vector2Int p)
        {
            return CG.Left(a, b, p);
        }

        // Vector2 version of above
        public static bool Left(Vector2 a, Vector2 b, Vector2 p)
        {
            return CG.Left(CG.Convert(a), CG.Convert(b), CG.Convert(p));
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
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        //Student code to build the path network from the given pathNodes and Obstacles
        //Obstacles - List of obstacles on the plane
        //agentRadius - the radius of the traversing agent
        //minPoVDist AND maxPoVDist - used for Points of Visibility (see assignment doc)
        //pathNodes - ref parameter that contains the pathNodes to connect (or return if pathNetworkMode is set to PointsOfVisibility)
        //pathEdges - out parameter that will contain the edges you build.
        //  Edges cannot intersect with obstacles or boundaries. Edges must be at least agentRadius distance
        //  from all obstacle/boundary line segments. No self edges, duplicate edges. No null lists (but can be empty)
        //pathNetworkMode - enum that specifies PathNetwork type (see assignment doc)

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight,
            List<Polygon> obstacles, float agentRadius, float minPoVDist, float maxPoVDist, ref List<Vector2> pathNodes, out List<List<int>> pathEdges,
            PathNetworkMode pathNetworkMode)
        {

            //STUDENT CODE HERE

            pathEdges = new List<List<int>>(pathNodes.Count);

            for (int i = 0; i < pathNodes.Count; ++i)
            {
                pathEdges.Add(new List<int>());
            }

            Vector2 topLeft = new Vector2(canvasOrigin.x, canvasOrigin.y + canvasHeight);
            Vector2 topRight = new Vector2(canvasOrigin.x + canvasWidth, canvasOrigin.y + canvasHeight);
            Vector2 bottomRight = new Vector2(canvasOrigin.x + canvasWidth, canvasOrigin.y);
            List<Vector2Int[]> converted = new List<Vector2Int[]>();
            foreach (Polygon obstacle in obstacles)
            {
                converted.Add(obstacle.getIntegerPoints());
            }

            for (int i = 0; i < pathNodes.Count; ++i)
            {
                if (DistanceToLineSegment(pathNodes[i], canvasOrigin, topLeft) < agentRadius)
                {
                    continue;
                }
                if (DistanceToLineSegment(pathNodes[i], topLeft, topRight) < agentRadius)
                {
                    continue;
                }
                if (DistanceToLineSegment(pathNodes[i], topRight, bottomRight) < agentRadius)
                {
                    continue;
                }
                if (DistanceToLineSegment(pathNodes[i], canvasOrigin, bottomRight) < agentRadius)
                {
                    continue;
                }
                if (!IsPointInsideAxisAlignedBoundingBox(ConvertToInt(canvasOrigin), ConvertToInt(topRight), ConvertToInt(pathNodes[i])))
                {
                    continue;
                }
                Boolean inObject = false;
                foreach (Vector2Int[] obstacle in converted)
                {
                    if (IsPointInPolygon(obstacle, ConvertToInt(pathNodes[i])))
                    {
                        inObject = true;
                        break;
                    }
                }
                if (inObject)
                {
                    continue;
                }

                for (int j = 0; j < pathNodes.Count; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (!IsPointInsideAxisAlignedBoundingBox(ConvertToInt(canvasOrigin), ConvertToInt(topRight), ConvertToInt(pathNodes[j])))
                    {
                        continue;
                    }
                    if (DistanceToLineSegment(pathNodes[j], canvasOrigin, topLeft) < agentRadius)
                    {
                        continue;
                    }
                    if (DistanceToLineSegment(pathNodes[j], topLeft, topRight) < agentRadius)
                    {
                        continue;
                    }
                    if (DistanceToLineSegment(pathNodes[j], topRight, bottomRight) < agentRadius)
                    {
                        continue;
                    }
                    if (DistanceToLineSegment(pathNodes[j], canvasOrigin, bottomRight) < agentRadius)
                    {
                        continue;
                    }
                    inObject = false;
                    foreach (Vector2Int[] obstacle in converted)
                    {
                        if (IsPointInPolygon(obstacle, ConvertToInt(pathNodes[j])))
                        {
                            inObject = true;
                            break;
                        }
                        for (int k = 0; k < obstacle.GetLength(0); k++)
                        {
                            int point = k - 1;
                            if (k == 0)
                            {
                                point = obstacle.GetLength(0) - 1;
                            }
                            if (DistanceToLineSegment(pathNodes[i], ConvertToFloat(obstacle[k]), ConvertToFloat(obstacle[point])) < agentRadius)
                            {
                                inObject = true;
                                break;
                            }
                        }
                    }
                    if (inObject)
                    {
                        continue;
                    }

                    bool intersectsObj = false;
                    foreach (Vector2Int[] obstacle in converted)
                    {
                        for (int k = 0; k < obstacle.GetLength(0); k++)
                        {
                            int point = k - 1;
                            if (k == 0)
                            {
                                point = obstacle.GetLength(0) - 1;
                            }
                            if (Intersects(ConvertToInt(pathNodes[i]), ConvertToInt(pathNodes[j]), obstacle[k], obstacle[point]))
                            {
                                intersectsObj = true;
                                break;
                            }
                            else if (DistanceToLineSegment(pathNodes[j], ConvertToFloat(obstacle[k]), ConvertToFloat(obstacle[point])) < agentRadius)
                            {
                                intersectsObj = true;
                                break;
                            }
                            else if (DistanceToLineSegment(ConvertToFloat(obstacle[k]), pathNodes[i], pathNodes[j]) < agentRadius)
                            {
                                intersectsObj = true;
                                break;
                            }
                        }
                        if (intersectsObj)
                        {
                            break;
                        }
                    }
                    if (intersectsObj)
                    {
                        continue;
                    }

                    pathEdges[i].Add(j);
                }
            }

            // END STUDENT CODE

        }


    }

}