﻿// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameAICourse
{


    public class CreateNavMesh
    {

        public static string StudentAuthorName = "Sebastian Brumm";



        // Helper method provided to help you implement this file. Leave as is.
        // Converts Vector2 to Vector2Int with default factor for computational geometry (1000)
        static public Vector2Int Convert(Vector2 v)
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
        // Returns true if point p is inside (but not on edge) the polygon defined by pts (CCW winding). False, otherwise
        public static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {

            return CG.InPoly1(pts, p) == CG.PointPolygonIntersectionType.Inside;

        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if there is at least one intersection between A and a polygon in polys
        public static bool IntersectsConvexPolygons(Polygon A, List<Polygon> polys)
        {
            return CG.IntersectionConvexPolygons(A, polys);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Tests to see if AB is an edge in a list of polys
        public static bool IsLineSegmentInPolygons(Vector2Int A, Vector2Int B, List<Polygon> polys)
        {
            return CG.IsLineSegmentInPolygons(A, B, polys);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Tests if abc are collinear
        static public bool IsCollinear(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            return CG.Collinear(a, b, c);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Tests if the polygon winding is CCW
        static public bool IsCCW(Vector2Int[] poly)
        {
            return CG.Ccw(poly);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Tests if C is between A and B (first tests if C is collinear with A and B
        // and then whether C is on the line between A and B
        static public bool Between(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            return CG.Between(a, b, c);
        }

        static public bool PolyOnEdge(Vector2Int a, Vector2Int b, List<Polygon> polys)
        {
            List<Vector2Int[]> converted = new List<Vector2Int[]>();
            foreach (Polygon obstacle in polys)
            {
                converted.Add(obstacle.getIntegerPoints());
            }

            foreach (Vector2Int[] obstacle in converted)
            {
                foreach (Vector2Int v in obstacle)
                {
                    if (v.Equals(a) || v.Equals(b))
                    {
                        continue;
                    }
                    else if (Between(a, b, v))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static public bool EnclosesPoly(Polygon tri, List<Polygon> polys)
        {
            List<Vector2Int[]> converted = new List<Vector2Int[]>();
            foreach (Polygon obstacle in polys)
            {
                converted.Add(obstacle.getIntegerPoints());
            }

            foreach (Vector2Int[] obstacle in converted)
            {
                foreach (Vector2Int v in obstacle)
                {
                    if (IsPointInsidePolygon(tri.getIntegerPoints(), v))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Tests if AB intersects with the interior of any poly of polys (touching the outside of a poly does not
        // count an intersection)
        public static bool InteriorIntersectionLineSegmentWithPolygons(Vector2Int A, Vector2Int B, List<Polygon> polys)
        {
            return CG.InteriorIntersectionLineSegmentWithPolygons(A, B, polys);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Merges two polygons into one across a common edge AB/BA
        public static Polygon MergePolygons(Polygon poly1, Polygon poly2, Vector2Int A, Vector2Int B)
        {
            return Utils.MergePolygons(poly1, poly2, A, B);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Tests if a poly is convex
        static public bool IsConvex(Vector2Int[] poly)
        {
            return CG.CheckForConvexity(poly);
        }



        // Create(): Creates a navmesh and path graph (associated with navmesh) 
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // obstacles: a list of Polygons that are obstacles in the scene
        // agentRadius: the radius of the agent
        // offsetObst: out param of the complex expanded obstacles for visualization purposes
        // origTriangles: out param of the triangles that are used for navmesh generation
        //          These triangles are passed out for validation and visualization.
        // navmeshPolygons: out param of the convex polygons of the navmesh (list). 
        //          These polys are passed out for validation and visualization
        // adjPolys: out param of type AdjacentPolygons. These are used validation
        // pathNodes: a list of graph nodes (either centered on portal edges or navmesh polygon, depending
        //                        on assignment requirements)
        // pathEdges: graph adjacency list for each node. Cooresponding index of pathNodes match
        //      a node with its edge list. All nodes must have an edge list (no null list)
        //      entries in each edge list are indices into pathNodes
        // 
        public static void Create(
        Vector2 canvasOrigin, float canvasWidth, float canvasHeight,
            List<Polygon> obstacles, float agentRadius,
            out List<Polygon> offsetObst,
            out List<Polygon> origTriangles,
            out List<Polygon> navmeshPolygons,
            out AdjacentPolygons adjPolys,
            out List<Vector2> pathNodes,
            out List<List<int>> pathEdges
            )
        {

            // Some basic initialization
            pathEdges = new List<List<int>>();
            pathNodes = new List<Vector2>();

            origTriangles = new List<Polygon>();
            navmeshPolygons = null;

            // This is a special dictionary for tracking polygons that share
            // edges. It is going to be used to determine which triangles can be merged
            // into larger convex polygons. Later, it will also be used for generating
            // the pathNetwork on top of the navmesh
            adjPolys = new AdjacentPolygons();

            // Holds the complex set of polys representing obstacle boundaries
            // Any time you need to test against obstacles, use offsetObstPolys
            // instead of obstacles
            List<Polygon> offsetObstPolys;

            // This creates a complex set of polygons representing the obstacle boundaries.
            // It's built with a 3rd party library called Clipper. In addition
            // to finding the union of obstacle boundaries, and clipping against the canvas, 
            // it also performs expansion for agentOffset
            Utils.GenerateOffsetNavSpace(canvasOrigin, canvasWidth, canvasHeight,
               agentRadius, obstacles, out offsetObstPolys);


            List<Polygon> tmp = new List<Polygon>(offsetObstPolys);

            // We currently don't support holes, but we can remove them. Holes
            // might form from union of polys, or (more rarely) expansion of concave polys.
            // There could be a hole with another poly inside, possibly repeating recursively.
            // In this case, removing holes will leave overlapping polys, but this shouldn't have
            // any bad effect other than wasted computation.
            foreach (var p in tmp)
            {
                if (!IsCCW(p.getIntegerPoints()))
                {
                    Debug.Log("*** Removed a hole from obstacles! ***");
                    offsetObstPolys.Remove(p);
                }
            }

            offsetObst = offsetObstPolys; // out param for viz

            // Obtain all the vertices that are going to be used to form our triangles
            List<Vector2Int> obstacleVertices;
            Utils.AllVerticesFromPolygons(offsetObstPolys, out obstacleVertices);

            // Let's also add the four corners of the canvas (with offset)
            var A = canvasOrigin + new Vector2(agentRadius, agentRadius);
            var B = canvasOrigin + new Vector2(0f, canvasHeight) + new Vector2(agentRadius, -agentRadius);
            var C = canvasOrigin + new Vector2(canvasWidth, canvasHeight) + new Vector2(-agentRadius, -agentRadius);
            var D = canvasOrigin + new Vector2(canvasWidth, 0f) + new Vector2(-agentRadius, agentRadius);

            var Ai = Convert(A);
            var Bi = Convert(B);
            var Ci = Convert(C);
            var Di = Convert(D);

            obstacleVertices.Add(Ai);
            obstacleVertices.Add(Bi);
            obstacleVertices.Add(Ci);
            obstacleVertices.Add(Di);


            // ******************** PHASE 0 - Change your name string ************************
            // TODO set your name above

            //********************* PHASE I - Brute force triangle formation *****************

            // In this phase, some scaffolding is provided for you. Your goal to to produce
            // triangles that will serve as the foundation of your navmesh. You will use
            // a brute force method of evaluating all combinations of three vertices to see
            // if a valid triangle is formed. This includes checking for degenerate triangles, 
            // triangles that intersect obstacle boundaries, and triangles that intersect
            // triangles you already made. There is also a special test to see if triangles
            // break adjacency (described later).

            // Iterate through combinations of obstacle vertices that can form triangle
            // candidates.
            var obstVertCount = obstacleVertices.Count;
            for (int i = 0; i < obstVertCount - 2; ++i)
            {

                for (int j = i + 1; j < obstVertCount - 1; ++j)
                {

                    for (int k = j + 1; k < obstVertCount; ++k)
                    {
                        // These are vertices for a candidate triangle
                        // that we hope to form
                        var V1 = obstacleVertices[i];
                        var V2 = obstacleVertices[j];
                        var V3 = obstacleVertices[k];

                        // TODO This inner loop involves tasks for you to implement

                        // TODO first lets check if the candidate triangle
                        // is NOT degenerate. Use IsCollinear(), if
                        // it is then just call continue to go to the next tri

                        if (IsCollinear(V1, V2, V3))
                        {
                            continue;
                        }

                        // TODO The next part is potentially a little tricky to understand,
                        // but easy to implement. Many of the edges of the triangles
                        // you form will be adjacent to obstacles. 
                        // The problem is that greedy triangle formation
                        // can make triangles that are "too big" and block adjacencies
                        // from forming because navmesh poly adjacency can only occur via a 
                        // common edge (not coincident edges with different vertices).
                        // What you need to do is first determine which of the 3 tri edges
                        // are edges of an obstacle polygon via IsLineSegmentInPolygons().
                        //
                        // *** Make sure you use offsetObstPolys any time you need to check
                        // against obstacles ***
                        //
                        // Be sure to store these IsLineSegmentInPolygons() test results in vars 
                        // since the test is expensive and you need the info later.
                        // After that, each tri edge that is NOT a line/edge in a poly
                        // should be checked further to see if there are any obstacle vertices
                        // that are ON the tri edge and BETWEEN the start and end point.
                        // You need to test against all obstacleVertices EXCEPT your two triangle
                        // edge endpoints. You will probably want to write a helper method
                        // to do this separately with the three candidate triangle edges.
                        // Use Between() to test each obstacle vertex against the candidate
                        // triangle edge. This test is important to get right because
                        // it will stop triangles from forming that block adjacencies from forming.
                        // If there is a vertex Between(), then "continue" to the next Triangle.
                        // (Note: that if a tri edge is true for IsLineSegmentInPolygons() that it
                        // can still be valid. It's just impossible for the Between() test
                        // to fail. So we skip that step for efficiency.)

                        bool v1v2 = false;
                        bool v2v3 = false;
                        bool v3v1 = false;

                        if (IsLineSegmentInPolygons(V1, V2, offsetObstPolys))
                        {
                            v1v2 = true;
                        }
                        else
                        {
                            if (PolyOnEdge(V1,V2, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        if (IsLineSegmentInPolygons(V2, V3, offsetObstPolys))
                        {
                            v2v3 = true;
                        }
                        else
                        {
                            if (PolyOnEdge(V2, V3, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        if (IsLineSegmentInPolygons(V3, V1, offsetObstPolys))
                        {
                            v3v1 = true;
                        }
                        else
                        {
                            if (PolyOnEdge(V3, V1, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        // TODO If the tri candidate has gotten this far, now create
                        // a new Polygon from your tri points. Also, we need to make sure
                        // all tris are consistent ordering. So call IsCCW(). If it's 
                        // NOT then call tri.Reverse() to fix it.

                        Vector2Int[] triArray = new Vector2Int[3];
                        triArray[0] = V1;
                        triArray[1] = V2;
                        triArray[2] = V3;
                        if (!IsCCW(triArray))
                        {
                            System.Array.Reverse(triArray);
                        }

                        Polygon tri = new Polygon();
                        tri.SetIntegerPoints(triArray);

                        // TODO Next, check if your new tri overlaps the other tris you
                        // have added so far. You will be adding valid tris to origTriangles.
                        // So, Use IntersectsConvexPolygons()
                        // If there is an overlap then call continue. Note that IntersectsConvexPolygons
                        // will not return true if the triangles are only touching.

                        if (IntersectsConvexPolygons(tri, origTriangles))
                        {
                            continue;
                        }

                        // TODO After that, you want to see if your new tri encloses any
                        // obstacleVertices. Use IsPointInsidePolygon() to accomplish this.
                        // If you get a hit, call continue to pass on the tri.
                        // THEN, you need to check for the possibility that the tri
                        // is exactly an obstacle polygon (of offsetObstPolys). triPoly.Equals() can be
                        // used. You can check out the implementation to see that it
                        // correctly compares any vertex ordering of the same winding.
                        // NOTE both of these are very rare tests to be successful.
                        // You can temporarily skip it and come back later if you want.

                        if (EnclosesPoly(tri, offsetObstPolys))
                        {
                            continue;
                        }

                        bool isObst = false;

                        foreach (Polygon obst in offsetObstPolys)
                        {
                            if (tri.Equals(obst))
                            {
                                isObst = true;
                                break;
                            }
                        }

                        if (isObst)
                        {
                            continue;
                        }

                        // TODO you now want to see if your new tri edges intersect
                        // with any of the obstacle edges. However, we can avoid 
                        // testing a tri edge that is exactly the same as an obstacle edge for
                        // performance.
                        // So use your saved results from IsLineSegmentInPolygons() (above) to 
                        // determine whether you should then call
                        // InteriorIntersectionLineSegmentWithPolygons(). If this test intersects,
                        // this skip the tri by calling continue.

                        if (!v1v2)
                        {
                            if (InteriorIntersectionLineSegmentWithPolygons(V1, V2, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        if (!v2v3)
                        {
                            if (InteriorIntersectionLineSegmentWithPolygons(V2, V3, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        if (!v3v1)
                        {
                            if (InteriorIntersectionLineSegmentWithPolygons(V3, V1, offsetObstPolys))
                            {
                                continue;
                            }
                        }

                        // TODO If the triangle has survived this far, add it to 
                        // origTriangles.
                        // Also, add it to the adjPolys dictionary with AddPolygon() (not
                        // Add()). Internally, AddPolygon() is fairly complicated
                        // as it tracks shared edges between polys

                        origTriangles.Add(tri);
                        adjPolys.AddPolygon(tri);

                    } // for
                } // for
            } // for

            // Priming the navmeshPolygons for next steps, and also allow visualization
            navmeshPolygons = new List<Polygon>(origTriangles);

            // TODO If you completed all of the triangle generation above, 
            // you can just return from the Create() method here to test what you have
            // accomplished so far. The originalTriangles
            // will be visualized as translucent yellow polys. Since they are translucent,
            // any accidental tri overlaps will be a darker shade of yellow. (Useful
            // for debugging.)
            // Also, navmeshPolygons is initially just the tris. Those are visualized 
            // as a blue outline. Note that the blue lineweight is very thin for better 
            // debugging of small polys


            // ********************* PHASE II - Merge Triangles *****************************
            // 
            // This phase involves merging triangles into larger convex polygons for the sake
            // of efficiency. If you like, you can temporarily skip to phase 3 and come back
            // later.
            // 
            // TODO Next up, you need to merge triangles into larger convex polygons where
            // possible. The greedy strategy you will use involves examining adjacent
            // tris and seeing if they can be merged into one new convex tri.
            // 
            // At the beginning of this process, you should make a copy of adjPolys. Continue
            // reading below to see why. You can SHALLOW copy like this: 
            // newAdjPolys = new AdjacentPolygons(adjPolys);
            // 
            // Iterate through adjPolys.Keys (type:CommonPolygonEdge) and get the value 
            // (type:CommonPolygons) for each key. This structure identifies only one polygon
            // if the edge is a boundary (.IsBarrier), but otherwise .AB and .BA references 
            // the adjacent polys. You can also get the .CommonEdge (with vertices .A and .B).
            // (The AB/BA refers to orientation of the common edge AB within each poly 
            // relative to the winding of the polygon.)
            // If you have two polygons AB and BA (NOT .IsBarrier), then use 
            // MergePolygons() to create a new polygon candidate. You need to 
            // check IsConvex() to decide if it's valid. 
            // If it is valid, then you need to remove the common edge (and merged polys)
            // from your adjPolys dictionary and also add the new, larger convex poly. 
            // And further, you need all the other common edges of the two old merged polys 
            // to be updated with the merged version.
            // You actually want to perform the dictionary operations on "newAdjPolys" that
            // you created above. This is because you never want to add/remove items
            // to a data structure that you are iterating over. A slightly more efficient
            // alternative would be to make dedicated add and delete lists and apply them
            // after enumeration is complete.
            // The removal of a common edge can be accomplished with newAdjPolys.Remove().
            // You can add the new merged polygon and update all old poly references with
            // a single method call:
            // AddPolygon(Polygon p, Polygon replacePolyA, Polygon replacePolyB)
            // Similar to the updates to newAdjPolys, you also want to remove old polys
            // and add the new poly to navMeshPolygons.
            // When your loop is finished, don't forget to set adjPolys to newAdjPolys.
            // If you don't do the last step then it won't appear that you have done any merging.

            int merges = 1;

            while (merges != 0)
            {
                AdjacentPolygons newAdjPolys = new AdjacentPolygons(adjPolys);
                merges = 0;

                foreach (CommonPolygonEdge edge in adjPolys.Keys)
                {
                    if (adjPolys[edge].AB != null && adjPolys[edge].BA != null && !adjPolys[edge].IsBarrier)
                    {
                        Polygon cand = MergePolygons(adjPolys[edge].AB, adjPolys[edge].BA, adjPolys[edge].CommonEdge.A, adjPolys[edge].CommonEdge.B);
                        if (!IsConvex(cand.getIntegerPoints()))
                        {
                            continue;
                        }

                        newAdjPolys.Remove(edge);
                        newAdjPolys.AddPolygon(cand, adjPolys[edge].AB, adjPolys[edge].BA);
                        navmeshPolygons.Remove(adjPolys[edge].AB);
                        navmeshPolygons.Remove(adjPolys[edge].BA);
                        navmeshPolygons.Add(cand);
                        merges++;
                    }
                }

                adjPolys = new AdjacentPolygons(newAdjPolys);
            }

            // TODO At this point you can visualize a single pass of the merging (e.g. test your
            // code). After that, wrap it all in a loop that tries successive passes of
            // merges, tracking how many successful merges occur. Your loop should terminate
            // when no merges are successful. Given that we only make a shallow copy,
            // a single pass through will create convex polygons possibly larger than 4 sides.
            // It is possibly impossible for more than one pass to be needed.
            // Be sure you see the blue lines disappear along edges where mergers occured.



            // *********************** PHASE 3 - Path Network from NavMesh *********************

            // The last step is to create a Path Graph from your navMesh
            // This will involve iterating over the keys of adjPolys so you can get the 
            // CommonPolygons values.
            //
            // Issues you need to address are:
            // 1.) Calculate midpoints of each portal edge to be your pathNodes
            // 2.) Implement a method for mapping Polygons each to a list of CommonPolygonEdges,
            //      and mapping CommonPolygonEdges to path node indexes (ints)
            //
            // For 1.), You can add the end points of the edges together and divide by 2.
            // 2.) is a bit more challenging. I recommend the use of Dictionaries for the mappings.
            //  You may benefit from a two-pass approach, iterating over the adjPolys.

            Dictionary<Polygon, List<CommonPolygonEdge>> polygonToEdges = new Dictionary<Polygon, List<CommonPolygonEdge>>();
            Dictionary<CommonPolygonEdge, int> edgeToNodeIndex = new Dictionary<CommonPolygonEdge, int>();
            int m = 0;

            foreach (CommonPolygonEdge edge in adjPolys.Keys)
            {
                if (!adjPolys[edge].IsBarrier)
                {
                    Vector2Int node = new Vector2Int((adjPolys[edge].CommonEdge.A.x + adjPolys[edge].CommonEdge.B.x) / 2, (adjPolys[edge].CommonEdge.A.y + adjPolys[edge].CommonEdge.B.y) / 2);
                    pathNodes.Add(ConvertToFloat(node));

                    pathEdges.Add(new List<int>());
                    edgeToNodeIndex[edge] = m;
                    m++;
                    if (polygonToEdges.ContainsKey(adjPolys[edge].AB))
                    {
                        polygonToEdges[adjPolys[edge].AB].Add(edge);
                    }
                    else
                    {
                        polygonToEdges[adjPolys[edge].AB] = new List<CommonPolygonEdge>();
                        polygonToEdges[adjPolys[edge].AB].Add(edge);
                    }

                    if (polygonToEdges.ContainsKey(adjPolys[edge].BA))
                    {
                        polygonToEdges[adjPolys[edge].BA].Add(edge);
                    }
                    else
                    {
                        polygonToEdges[adjPolys[edge].BA] = new List<CommonPolygonEdge>();
                        polygonToEdges[adjPolys[edge].BA].Add(edge);
                    }
                }
            }

            foreach (Polygon poly in polygonToEdges.Keys)
            {
                for (int n = 0; n < polygonToEdges[poly].Count; n++)
                {
                    if (n == polygonToEdges[poly].Count - 1)
                    {
                        pathEdges[edgeToNodeIndex[polygonToEdges[poly][n]]].Add(edgeToNodeIndex[polygonToEdges[poly][0]]);
                        pathEdges[edgeToNodeIndex[polygonToEdges[poly][0]]].Add(edgeToNodeIndex[polygonToEdges[poly][n]]);
                    }
                    else
                    {
                        pathEdges[edgeToNodeIndex[polygonToEdges[poly][n]]].Add(edgeToNodeIndex[polygonToEdges[poly][n+1]]);
                        pathEdges[edgeToNodeIndex[polygonToEdges[poly][n+1]]].Add(edgeToNodeIndex[polygonToEdges[poly][n]]);
                    }
                }
            }

            // ***************************** FINAL **********************************************
            // Once you have completed everything, you will probably find that the code
            // is very slow. It can be sped up a good bit by creating hashtables of common calculations.
            // This is not required though. 
            //
            // Also, there are better ways to triangulate that perform better and give
            // better quality triangles (not long and skinny but closer to equilateral).
            // However, you don't need to worry about implementing this.
            // 

        } // Create()


    }

}