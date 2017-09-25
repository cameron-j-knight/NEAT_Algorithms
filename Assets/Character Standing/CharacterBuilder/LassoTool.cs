using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LassoTool : MonoBehaviour {
    Vector3[] edgeVerts;
    Vector3[] vertices;
    int[] tris;
    int pointSpacing = 2;
    Vector3 mousePos;
    List<Vector3> edges = new List<Vector3>();
    int maxDist = 10;
    List<GameObject> Lines = new List<GameObject>();
    public GameObject LassoLine;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Array.Clear(edges, 0, edges.Length);
            edges.Clear();
            SaveEdge();

        }
        if (Input.GetMouseButton(0))
        {
            if ((mousePos - Input.mousePosition).sqrMagnitude >= pointSpacing * pointSpacing)
            {
                SaveEdge();

            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            edges.Add(Vector3.zero);
            tris = CreateTris();
           StartCoroutine( CreateMesh(edges.ToArray(), tris));


            foreach (GameObject Line in Lines.ToArray())
            {
                Destroy(Line);

            }

        }
    }

    void SaveEdge()
    {
        mousePos = Input.mousePosition;
        edges.Add( Camera.main.ScreenToWorldPoint( new Vector3(mousePos.x, mousePos.y, maxDist)));
        DrawLasso();
    }

    int[] CreateTris()
    {

        List<int> trisList = new List<int>();
        for (var i = 0; i < edges.Count; i++)
        {

            trisList.Add(i);
            trisList.Add((i + 1) % (edges.Count - 1));
            trisList.Add(edges.Count - 1);
        }
        return trisList.ToArray();
    }

    IEnumerator CreateMesh(Vector3[] verts, int[] tris)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;







        MeshCollider col = (MeshCollider)gameObject.AddComponent(typeof(MeshCollider));
        //col.convex = true;

        col.isTrigger = false;
        col.sharedMesh = mesh;
        Rigidbody rb = (Rigidbody)gameObject.AddComponent(typeof(Rigidbody));
        rb.isKinematic = true;
        rb.position = transform.position;
        yield return new WaitForFixedUpdate();

        //Destroy(rb);
        //Destroy(col);
        //Destroy(mesh);
        Camera.main.GetComponent<CameraRotate>().Run = true;

        this.enabled  = (false);
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
    }


    void DrawLasso()
    {
        foreach(GameObject Line in Lines.ToArray())
        {
            Destroy(Line);

        }
        Lines.Clear();
        if (edges.ToArray().Length > 1)
        {


            for (int i = 1; i < edges.ToArray().Length + 1; i++)
            {
                if (i == edges.ToArray().Length)
                {
                    Vector3 linePos = new Vector3(edges[0].x + edges[i - 1].x, edges[0].y + edges[i - 1].y, edges[0].z + edges[i - 1].z) * 0.5f;
                    linePos = Vector3.Lerp (linePos, Camera.main.transform.position, .9f);
                    GameObject thisLine = Instantiate(LassoLine, linePos, Quaternion.identity) as GameObject;
                    thisLine.transform.parent = this.transform;
                    thisLine.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((edges[0].y - edges[i - 1].y), (edges[0].x - edges[i - 1].x)) * (360 / (2 * Mathf.PI)));
                    thisLine.transform.localScale = (Vector3.right * (.1f * (Vector3.Distance(edges[0], edges[i - 1])))) + new Vector3(0, .05f, .05f);

                    Lines.Add(thisLine);

                }
                else
                {
                    Vector3 linePos = new Vector3(edges[i].x + edges[i - 1].x, edges[i].y + edges[i - 1].y, edges[i].z + edges[i - 1].z) * 0.5f;
                    linePos = Vector3.Lerp(linePos, Camera.main.transform.position, .9f);

                    GameObject thisLine = Instantiate(LassoLine, linePos, Quaternion.identity) as GameObject;
                    thisLine.transform.parent = this.transform;
                    thisLine.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((edges[i].y - edges[i - 1].y), (edges[i].x - edges[i - 1].x)) * (360 / (2 * Mathf.PI)));
                    thisLine.transform.localScale = (Vector3.right * (.1f * (Vector3.Distance(edges[i], edges[i - 1])))) + new Vector3(0, .05f, .05f);
                    Lines.Add(thisLine);


                }
            }
       
        
    }



}
}
