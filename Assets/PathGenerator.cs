using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    System.Random prng;
    private string layer;

    Mesh mesh;
    Mesh envMesh;

    private GameObject[] landmarks;
    private int seed;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    List<(Vector3 start, Vector3 end)> paths;
    List<Polygon> pathPolys;
    IDictionary<Vector3, Vector3> neighbors;
    IDictionary<Vector3, int> outerEnds;
    IDictionary<Vector3, (int a, int ai)> Intersections;
    List<(Vector3 v, int a)> pathEnds;
    
    int numOfLandmarks = 6;
    int maxLength = 1500;
    int totalLength = 0;
    private int pathWidth = 6;
    private float pathScale = 0.5f;
    int vertIndex, triVertIndex, triIndex, verticesCount;
    Vector3 spawn = new Vector3(20, 8, 20);

    List<Vector3> landMarkCoords;
    Material mat;

    public void Initialize(string layer, GameObject[] landmarks, int seed, Material mat)
    {
        this.layer = layer;
        this.landmarks = landmarks;
        this.seed = seed;
        this.prng = new System.Random(seed);
        this.paths = new List<(Vector3, Vector3)>();
        this.pathPolys = new List<Polygon>();
        this.neighbors = new Dictionary<Vector3, Vector3>();
        this.outerEnds = new Dictionary<Vector3, int>();
        this.Intersections = new Dictionary<Vector3, (int a, int ai)>();
        this.pathEnds = new List<(Vector3 v, int a)>();
        this.landMarkCoords = new List<Vector3>();
        this.mat = mat;
    }

    void AddLandMarkCoords(Vector3 vertex)
    {
        (int a, int ai) = Intersections[vertex];
        int fullAng = a + ai/2;
        Vector3 move = new Vector3(Mathf.Cos(fullAng * Mathf.Deg2Rad)*10, 0, Mathf.Sin(fullAng * Mathf.Deg2Rad)*10);
        landMarkCoords.Add(vertex + move);
        System.Console.WriteLine("vertex: {0} move: {1} vertex + move: {2} a {3} ai {4} fullAng {5}", vertex, move, vertex + move, a, ai, fullAng);
    }

    private void GeneratePolygon(Vector3 start, Vector3 end, int startAngle, int totalEdges, int edgesToDraw) 
    {
            int baseLength = 30;
            Polygon polygon = new Polygon();
            polygon.Initialize(totalEdges);

            int innerAngle = ((totalEdges-2)*180) / totalEdges;
            int outerAngle = 180 - innerAngle;
            int range = outerAngle / 4;
            int lenrange = 20;
            
            int randAngle = 0, angleInc = 50, randLength = 0, length = 40;

            int angle = startAngle;
            Vector3 oldpos = start, newpos = start;
            polygon.addVertex(start);

            for (int i = 0; i < edgesToDraw-1; i++) {

                oldpos = newpos;
                newpos += new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad)*length, 0, Mathf.Sin(angle * Mathf.Deg2Rad)*length);
                
                paths.Add((oldpos, newpos));
                polygon.addVertex(newpos);

                neighbors.Add(newpos, oldpos);
                outerEnds.Add(newpos, angle % 360);
                pathEnds.Add((newpos, angle % 360));

                if (!Intersections.ContainsKey(oldpos))
                    Intersections.Add(oldpos, (angle % 360, 180-angleInc % 360));

                totalLength += length;
                System.Console.WriteLine("path length: {0} length: {1}", totalLength, length);
        
                if (i % 2 == 0) {
                    randAngle = prng.Next(0, range);
                    randLength = prng.Next(0, lenrange);

                    angleInc = (outerAngle + randAngle);
                    length = baseLength + randLength;
                } else{
                    angleInc = (outerAngle - randAngle);
                    length = baseLength + lenrange - randLength;
                }
                angle += angleInc;
            }

            Vector3 newEnd = end;
            paths.Add((newpos, newEnd));

            int newAngle = (int)(Mathf.Atan2(end.z - newpos.z, end.x - newpos.x) * Mathf.Rad2Deg);
            int oldAngle = Intersections[oldpos].a-180;
            Intersections.Add(newpos, (newAngle, oldAngle-newAngle));

            totalLength += (int)Vector3.Distance(newpos, newEnd);
            System.Console.WriteLine("path length: {0} length: {1} newAngle: {2} oldAngle: {3}", totalLength, length, newAngle, oldAngle);

            if (start == end) {
                neighbors.Add(end, newpos);
                pathEnds.Add((end, angle));
                Intersections[end] = (startAngle, newAngle+180-startAngle);
                //polygon.print();
                return;
            }

            AddLandMarkCoords(start);
            AddLandMarkCoords(end);

            outerEnds.Remove(start);
            outerEnds.Remove(end);

            for (int i = 0; i <= totalEdges-edgesToDraw; i++) {
                polygon.addVertex(newEnd);
                newpos = newEnd;
                newEnd = neighbors[newEnd];

                neighbors[newpos] = oldpos;

                oldpos = newpos;
            }
            
            //polygon.print();
    }

    void GenerateExtraEndpoints(Vector3 start)
    {
        int length = 0, randLength = 0, j = -1, baseLength = 30, lenrange = 2*(maxLength - numOfLandmarks*baseLength - totalLength)/numOfLandmarks;

        foreach(KeyValuePair<Vector3, int> entry in outerEnds)
        {
            if (++j % 2 != 0) continue;

            if (j % 4 == 0){
                randLength = prng.Next(0, lenrange);
                length = baseLength + randLength;
            } else
                length = baseLength + lenrange - randLength;
            
            Vector3 outerVertex = entry.Key;
            Vector3 normDist = (outerVertex - start).normalized;
            totalLength += length;
            Vector3 newpos = outerVertex + normDist*length;
            paths.Add((outerVertex, newpos));

            AddLandMarkCoords(outerVertex);
            landMarkCoords.Add(newpos + normDist*10);
        }
    }

    public void GenerateLandmarkCoords(Mesh[] meshes, int centerx, int centerz) 
    {
        // + new Vector3(20, 0, -30)
        Vector3 start = meshes[3].vertices[0];
        {    
            int totalEdges = 6;
        
            GeneratePolygon(start, start, 90, totalEdges, totalEdges);
            GeneratePolygon(pathEnds[^1].v, pathEnds[0].v, (180+pathEnds[1].a), totalEdges, totalEdges-1);
            for (int i = 1; i < totalEdges-1; i++) {
                GeneratePolygon(pathEnds[^1].v, pathEnds[i].v, (180+pathEnds[(i+2)%totalEdges].a), totalEdges, totalEdges-2);
            }
            GeneratePolygon(pathEnds[^1].v, pathEnds[totalEdges].v, (180+pathEnds[1].a), totalEdges, totalEdges-3);
            
            GenerateExtraEndpoints(start);
        }
    }

    public void GenerateLandmarks(Mesh[] meshes)
    {
        if (landmarks.Length > 0)
        {    
            for (int j = 0; j < landMarkCoords.Count; j++)
            {
                Vector3 vertice = landMarkCoords[j];
                GameObject objectToSpawn = landmarks[0];
                objectToSpawn.layer = LayerMask.NameToLayer(layer);


                vertice.y -= 3;
                Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
    }

    void getVerticesCount(int i)
    {

        foreach ((Vector3 first, Vector3 second) in paths) 
        {
            Vector3 dist = new Vector3(second.x - first.x, 0, second.z - first.z);
            Vector3 normDist = Vector3.Normalize(dist)*pathScale;

            if (dist.x != 0.0f)
                verticesCount += (int)(dist.x/normDist.x);
            else
                verticesCount += (int)(dist.z/normDist.z);

            //System.Console.WriteLine("first: {0} second: {1} dist {2} normDistx {3} normDistz {4} verticesCount {5}", first, second, dist, normDist.x, normDist.z, verticesCount);
        }
    }

    public void GeneratePaths(Mesh[] meshes)
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //for (int i = 0; i < meshes.Length; i++)
        int i = 0;
        {    
            vertIndex = triVertIndex = triIndex = verticesCount = 0;
            envMesh = meshes[i];
            getVerticesCount(i);

            vertices = new Vector3[(verticesCount+paths.Count) * (pathWidth+1)];
            colors = new Color[vertices.Length];
            triangles = new int[verticesCount * pathWidth * 6];

            foreach ((Vector3 first, Vector3 second) in paths) 
            {
                int len = addPathVertices(first, second);
                addPathTriangles(len);
                triVertIndex += pathWidth+1;
            }
            
            UpdateMesh();
            createMeshGO();
        }
        System.Console.WriteLine("vertsize: {0} trisize: {1} verticesCount: {2}", vertices.Length, triangles.Length, verticesCount);
    }

    int addPathVertices(Vector3 s, Vector3 f)
    {
        Vector3 pos = s;
        Vector3 dist = new Vector3(f.x - s.x, 0, f.z - s.z);
        Vector3 normDist = dist.normalized*pathScale;
        int layerMask = 1 << 8;

        Vector3 perp = new Vector3(dist.z, 0, -dist.x).normalized;
        Color brown = new Color(0.4f, 0.3f, 0.0f, 0.5f);

        int len = (int)(dist.z/normDist.z);

        if (dist.x != 0.0f)
            len = (int)(dist.x/normDist.x);
        Vector3 vertex, heightV, adjustedV, start;

        for (int i = 0; i <= len; i++) 
        {
            start = pos - (pathWidth/2)*perp*pathScale;
            RaycastHit hit;

            for (int j = 0; j <= pathWidth; j++)
            {
                adjustedV = start + j*perp*pathScale + Vector3.up*30;

                if (Physics.Linecast(adjustedV, adjustedV + Vector3.down*50, out hit, layerMask)) {
                    heightV = hit.point;/*
                    if (j==0)
                    {Debug.DrawRay(adjustedV, Vector3.down*hit.distance, Color.green, 5000f);
                    System.Console.WriteLine("hit: {0} collider {3} adjustedV + Vector3.down: {1} start + j*perp*pathScale: {2}", hit.point, adjustedV + Vector3.down*50, start + j*perp*pathScale, hit.collider);
                }*/
                } else {
                    heightV = envMesh.vertices[(int)(adjustedV.x)+201*(int)(adjustedV.z)];
                    System.Console.WriteLine("did not hit: {0} adjustedV + Vector3.down: {1} start + j*perp*pathScale: {2}", hit.point, adjustedV + Vector3.down*50, start + j*perp*pathScale);
                }

                if (j % pathWidth == 0){
                    vertex = new Vector3(adjustedV.x, heightV.y+0.025f, adjustedV.z);
                    colors[vertIndex] = brown;
                } else {
                    vertex = new Vector3(adjustedV.x, heightV.y+0.1f, adjustedV.z);
                    colors[vertIndex] = brown;
                }
                vertices[vertIndex++] = vertex;
                //System.Console.WriteLine("i: {0} j: {1} s: {2} f: {3} dist: {4} vertex: {5} pos: {6} vertIndex: {7} ", i, j, s, f, dist, vertex, pos, vertIndex);
                //System.Console.WriteLine("vertsize: {0} trisize: {1} len: {2} verticesCount: {3} normDist {4} normDist.x {5} normDist.z {5} dist.normalized {6}", vertices.Length, triangles.Length, len, verticesCount, normDist, normDist.x, normDist.z, dist.normalized.x);
            }
            //System.Console.WriteLine("i: {0} s: {1} f: {2} dist: {3} pos: {4} vertIndex: {5} ", i, s, f, dist, pos, vertIndex);
            //System.Console.WriteLine("vertsize: {0} trisize: {1} len: {2} verticesCount: {3} normDist {4}", vertices.Length, triangles.Length, len, verticesCount, normDist);

            pos += normDist;
        }

        return len;
    }

    void addPathTriangles(int len)
    {
        // Go to next row
        for (int z = 0; z < len; z++)
        {
            // fill row
            for (int x = 0; x < pathWidth; x++)
            {
                triangles[triIndex + 0] = triVertIndex + 0;
                triangles[triIndex + 1] = triVertIndex + pathWidth + 1;
                triangles[triIndex + 2] = triVertIndex + 1;
                triangles[triIndex + 3] = triVertIndex + 1;
                triangles[triIndex + 4] = triVertIndex + pathWidth + 1;
                triangles[triIndex + 5] = triVertIndex + pathWidth + 2;

                //System.Console.WriteLine("triIndex + 0: {0} triIndex + 1: {1} triIndex + 2 {2} triIndex + 3 {3} triIndex + 4 {4} triIndex + 5 {5}, pathWidth {6}", triangles[triIndex + 0], triangles[triIndex + 1], triangles[triIndex + 2], triangles[triIndex + 3], triangles[triIndex + 4], triangles[triIndex + 5], pathWidth);
                //System.Console.WriteLine("vert: {0} triIndex: {1} len: {2} vertIndex: {3} pathWidth: {4} vertsize: {5} trisize: {6} vert count {7}", triVertIndex, triIndex, len, vertIndex, pathWidth, vertices.Length, triangles.Length, verticesCount);
                //System.Console.WriteLine("vert + pathWidth + 1: {0} vert + 1: {1} vert + pathWidth + 2: {2} vert: {3}", vertices[triVertIndex + pathWidth + 1], vertices[triVertIndex + 1], vertices[triVertIndex + pathWidth + 2], vertices[triVertIndex]);
                triVertIndex++;
                triIndex += 6;
            }
            triVertIndex++;
        }
    }
/*
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        for (int i = 0; i < vertices.Length; i += 20)
            Gizmos.DrawSphere(vertices[i], 0.5f);
    }
*/
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    void createMeshGO()
    {
        GameObject go = new GameObject("Mesh");

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().material = mat;
        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
        go.GetComponent<MeshCollider>().enabled = true;

        go.layer = LayerMask.NameToLayer(layer);
    }
    private class Polygon
    {
        Vector3[] polVertices;
        int i = 0;
        float area;
        int edges;

        public void Initialize(int edges)
        {
            this.edges = edges;
            this.polVertices = new Vector3[edges];
        }

        public void addVertex(Vector3 vertex)
        {
            if (i < edges)
                polVertices[i++] = vertex;
        }

        public void print()
        {
            for (int j = 0; j < i; j++)
                System.Console.WriteLine("j: {0} vertex: {1}", j, polVertices[j]);
        }
    }
}
