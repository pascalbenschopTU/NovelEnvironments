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
    int size = 0;
    private int pathWidth = 8;
    private float pathScale = 0.4f;
    int vertIndex = 0, triVertIndex = 0, triIndex = 0, verticesCount = 0;
    Vector3 spawn;

    List<Vector3> landMarkCoords;
    Material mat;

    public void Initialize(string layer, GameObject[] landmarks, int seed, Material mat, int size)
    {
        this.layer = layer;
        this.landmarks = landmarks;
        this.seed = seed;
        this.size = size;
        this.maxLength = size*4;
        this.prng = new System.Random(seed);
        this.paths = new List<(Vector3, Vector3)>();
        this.pathPolys = new List<Polygon>();
        this.neighbors = new Dictionary<Vector3, Vector3>();
        this.outerEnds = new Dictionary<Vector3, int>();
        this.Intersections = new Dictionary<Vector3, (int a, int ai)>();
        this.pathEnds = new List<(Vector3 v, int a)>();
        this.landMarkCoords = new List<Vector3>();
        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        this.mat = mat;
    }

    void AddLandMarkCoords(Vector3 vertex)
    {
        (int a, int ai) = Intersections[vertex];
        float fullAng = (a + ai/2) * Mathf.Deg2Rad;
        Vector3 move = new Vector3(Mathf.Cos(fullAng)*10, 0, Mathf.Sin(fullAng)*10);
        Vector3 height = getHeigthVertex(vertex + move);

        landMarkCoords.Add(height);
        //System.Console.WriteLine("vertex: {0} move: {1} height: {2} a {3} ai {4} fullAng {5}", vertex, move, height, a, ai, fullAng);
    }

    public Vector3 getSpawn()
    {
        return spawn;
    }

    public List<(Vector3 start, Vector3 end)> getPaths()
    {
        return paths;
    }

    private void GeneratePolygon(Vector3 start, Vector3 end, int startAngle, int totalEdges, int edgesToDraw) 
    {
        Polygon polygon = new Polygon();
        polygon.Initialize(totalEdges);

        int innerAngle = ((totalEdges-2)*180) / totalEdges;
        int outerAngle = 180 - innerAngle;
        int range = outerAngle / 4;
        int baseLength = (int)(size/12);
        int lenrange = (int)(size/20);
        
        int randAngle = 0, angleInc = 50, randLength = 0, length = baseLength+(int)(lenrange/2);

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
            //System.Console.WriteLine("path length: {0} length: {1}", totalLength, length);
    
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
        //System.Console.WriteLine("path length: {0} length: {1} newAngle: {2} oldAngle: {3}", totalLength, length, newAngle, oldAngle);

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
        int j = -1, length = 0, baseLength = (int)(size/10), randLength = 0, lenrange = 2*(maxLength - numOfLandmarks*baseLength - totalLength)/numOfLandmarks;

        foreach(Vector3 outerVertex in outerEnds.Keys)
        {
            if (++j % 2 != 0) continue;

            if (j % 4 == 0){
                randLength = prng.Next(0, lenrange);
                length = baseLength + randLength;
            } else
                length = baseLength + lenrange - randLength;
            
            Vector3 normDist = (outerVertex - start).normalized;
            Vector3 newpos = outerVertex + normDist*length;
            totalLength += length;

            paths.Add((outerVertex, newpos));

            AddLandMarkCoords(outerVertex);
            landMarkCoords.Add(getHeigthVertex(newpos + normDist*5));
        }
    }

    public void GenerateLandmarkCoords(Mesh[] meshes, int centerx, int centerz) 
    {
        this.spawn = getHeigthVertex(new Vector3((int)(size/12), 0, -(int)(size/20))) + Vector3.up*1.5f;
        Vector3 start = spawn;

        int totalEdges = 6;
    
        GeneratePolygon(start, start, 90, totalEdges, totalEdges);
        GeneratePolygon(pathEnds[^1].v, pathEnds[0].v, (180+pathEnds[1].a), totalEdges, totalEdges-1);
        for (int i = 1; i < totalEdges-1; i++) {
            GeneratePolygon(pathEnds[^1].v, pathEnds[i].v, (180+pathEnds[(i+2)%totalEdges].a), totalEdges, totalEdges-2);
        }
        GeneratePolygon(pathEnds[^1].v, pathEnds[totalEdges].v, (180+pathEnds[1].a), totalEdges, totalEdges-3);
        
        GenerateExtraEndpoints(start);
    }

    public void GenerateLandmarks(Mesh[] meshes)
    {
        if (landmarks.Length > 0)
        {    
            for (int j = 0; j < landMarkCoords.Count; j++)
            {
                Vector3 vertice = landMarkCoords[j];
                GameObject objectToSpawn = landmarks[j % landmarks.Length];
                objectToSpawn.layer = LayerMask.NameToLayer(layer);

                vertice.y -= 3;
//                Instantiate(objectToSpawn, vertice, Quaternion.Euler(new Vector3(-90, 0, 0)));
                
                objectToSpawn.AddComponent<AreaTracker>();

                Instantiate(objectToSpawn, vertice, Quaternion.identity);

            }
        }
    }

    public void GeneratePaths(Mesh[] meshes)
    {
        foreach ((Vector3 first, Vector3 second) in paths) 
            verticesCount += getStepAndLength(first, second).len;

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
        //System.Console.WriteLine("vertsize: {0} trisize: {1} verticesCount: {2}", vertices.Length, triangles.Length, verticesCount);
    }

    Vector3 getHeigthVertex(Vector3 vertex)
    {
        Vector3 heightV = vertex + Vector3.up*100;
        RaycastHit hit;

        if (Physics.Linecast(heightV, heightV + Vector3.down*150, out hit)) {
            heightV = hit.point;/*
            if (j==0)
            {Debug.DrawRay(adjustedV, Vector3.down*hit.distance, Color.green, 5000f);
            System.Console.WriteLine("hit: {0} collider {3} adjustedV + Vector3.down: {1} start + j*perp*pathScale: {2}", hit.point, adjustedV + Vector3.down*50, start + j*perp*pathScale, hit.collider);
        */
        } else
            return vertex;
        
        return heightV;
    }

    (Vector3 v, int len) getStepAndLength(Vector3 s, Vector3 f)
    {
        Vector3 dist = new Vector3(f.x - s.x, 0, f.z - s.z);
        Vector3 normDist = dist.normalized*pathScale;

        Vector3 perp = new Vector3(dist.z, 0, -dist.x).normalized*pathScale;

        int len = (int)((dist.z+normDist.z)/normDist.z);

        if (dist.x != 0.0f)
            len = (int)((dist.x+normDist.x)/normDist.x);
        
        return (normDist, len);
    }

    int addPathVertices(Vector3 s, Vector3 f)
    {
        Vector3 pos = s;
        (Vector3 step, int len) = getStepAndLength(s, f);

        Vector3 perp = new Vector3(step.z, 0, -step.x);
        Color brown = new Color(0.4f, 0.3f, 0.0f, 0.5f);

        Vector3 heightV, start;

        for (int i = 0; i <= len; i++) 
        {
            start = pos - (pathWidth/2)*perp;

            for (int j = 0; j <= pathWidth; j++)
            {
                heightV = getHeigthVertex(start + j*perp);

                if (j % pathWidth == 0 || i % len == 0){
                    heightV += Vector3.up*0.02f;
                    colors[vertIndex] = brown;
                } else {
                    heightV += Vector3.up*0.15f;
                    colors[vertIndex] = brown;
                }

                vertices[vertIndex++] = heightV;
            }

            pos += step;
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
                
                triVertIndex++;
                triIndex += 6;
            }
            triVertIndex++;
        }
    }

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
        GameObject go = new GameObject("PathMesh");

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
