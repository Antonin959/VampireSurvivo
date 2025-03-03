using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gen : MonoBehaviour
{
    public TMP_Text WaveText;
    public GameObject tile;
    public EnemieScript enemie;
    public int thepath = 0;
    int spawnpathindex = 0;

    Dictionary<Vector3, int> tiles = new Dictionary<Vector3, int>();

    public LineRenderer linePrefab;

    class P
    {
        public List<Vector3> path;
        public Vector3 pos;
        public bool active;

        public LineRenderer line;
        public List<(Transform t, float Y, bool b)> tile;

        public P(Vector3 p, LineRenderer l)
        {
            path = new List<Vector3>() { p };
            tile = new List<(Transform, float, bool)>();
            pos = p;
            active = true;
            line = l;
        }
        public P(List<Vector3> pa, Vector3 p, LineRenderer l)
        {
            path = pa;
            tile = new List<(Transform, float, bool)>();
            pos = p;
            active = true;
            line = l;
        }
        public void AddLinePos(Vector3 newpos)
        {
            Debug.Log(line);
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, new Vector3(newpos.x, 3.05f, newpos.z));

            Mesh mesh = new Mesh();
            line.BakeMesh(mesh, true);
            Vector3[] vert = mesh.vertices;
            for (int j = 0; j < vert.Length; j++)
                vert[j] = Quaternion.Euler(-90, 0, 0) * vert[j];
            mesh.vertices = vert;
            mesh.RecalculateBounds();

            line.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    List<P> p = new List<P>();

    class EnnemieGroup
    {
        public float groupTimer;
        public int enIndex;

        public EnnemieGroup(float timer, int EnIndex)
        {
            groupTimer = timer;
            enIndex = EnIndex;
        }
    }
    int ennemieGroupIndex = 0, wave = 0;
    float gtimer = 0, wtimer = 0;
    readonly List<(float waveTimer, List<(float groupTimer, int enIndex)> group)> enemieWave = new List<(float, List<(float, int)>)>()
    {
        (6.0f, new List<(float, int)>() { (0.0f, 1), (1.0f, 1), (1.0f, 1)}),
        (10.0f, new List<(float, int)>() { (0.0f, 1), (1.0f, 1), (1.0f, 1)}),
        (10.0f, new List<(float, int)>() { (0.0f, 2), (1.0f, 2), (1.0f, 1)}),
        (10.0f, new List<(float, int)>() { (0.0f, 1), (1.0f, 2), (1.0f, 2)}),
        (10.0f, new List<(float, int)>() { (0.0f, 1), (0.2f, 1), (0.2f, 1) ,(0.2f, 1) ,(1.0f, 1) ,(0.2f, 1), (0.2f, 1), (0.2f, 1)}),
        (10.0f, new List<(float, int)>() { (0.0f, 2), (1.0f, 2), (1.0f, 2) ,(1.0f, 2) ,(1.0f, 2) ,(1.0f, 2)}),
        (10.0f, new List<(float, int)>() { (0.0f, 2), (1.0f, 2), (1.0f, 1)}),
        (10.0f, new List<(float, int)>() { (0.0f, 1), (1.0f, 2), (1.0f, 2)}),
        (10.0f, new List<(float, int)>() { (0.0f, 2), (1.0f, 2), (1.0f, 2) ,(1.0f, 2) ,(1.0f, 2) ,(1.0f, 2)}),
        (10.0f, new List<(float, int)>() { (0.0f, 2), (1.0f, 2), (1.0f, 2) ,(1.0f, 2) ,(1.0f, 2) ,(1.0f, 2)}),

    };

    void Start()
    {
        p.Add(new P(new Vector3(0, 0, 0), Instantiate(linePrefab)));
        Debug.Log(p[0].line + " " + linePrefab);
        Debug.Log(p[0].line + "  " + linePrefab, p[0].line);

        StartCoroutine(Generation(8));
    }
    void Update()
    {
        WaveText.text = "Wave " + wave; 

        for (int i = 0; i < p.Count; i++)
        {
            for (int t = 0; t < p[i].tile.Count; t++)
            {
                if (!p[i].tile[t].b)
                {
                    p[i].tile[t].t.position = new Vector3(p[i].tile[t].t.position.x, p[i].tile[t].Y, p[i].tile[t].t.position.z);
                    continue;
                }

                if (p[i].tile[t].t.position.y <= p[i].tile[t].Y)
                    p[i].tile[t] = (p[i].tile[t].t, p[i].tile[t].Y, false);

                if (p[i].tile[t].t)
                    p[i].tile[t].t.position -= new Vector3(0, p[i].tile[t].t.position.y * 0.8f * Time.deltaTime + 0.2f, 0);
            }
        }

        if (wave < enemieWave.Count)
        {
            (float timer, List<(float groupTimer, int e)> group) twave = enemieWave[wave];

            if (wtimer >= twave.timer)
            {
                if (ennemieGroupIndex < twave.group.Count)
                {
                    (float groupTimer, int e) group = twave.group[ennemieGroupIndex];

                    gtimer += Time.deltaTime;
                    if (gtimer >= group.groupTimer)
                    {
                        for (int i = 0; i < p.Count; i++)
                        {
                            EnemieScript e = Instantiate(enemie, new Vector3(0, 1, 0), Quaternion.identity);
                            e.path = p[i].path;
                            e.speed = group.e == 1 ? 2 : 6;
                            e.GetComponent<Renderer>().material.color = group.e == 1 ? Color.red : new Color(0.6f, 0, 0);

                        }
                        ennemieGroupIndex++;
                        gtimer = 0;
                    }
                }
                else
                {
                    wave++;
                    wtimer = 0;
                    ennemieGroupIndex = 0;
                    StartCoroutine(Generation(8));
                }
            }
            else
                wtimer += Time.deltaTime;
        }
    }

    IEnumerator Generation(int amout)
    {
        for (int i = 0; i < amout; i++)
        {
            Generate();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Generate()
    {
        int prepCount = p.Count;

        int i = Random.Range(0, p.Count);
        if (!p[i].active)
            return;
        List<Vector3> nearpos = new List<Vector3>();

        TryAdd(new Vector3(1, 0, 0));
        TryAdd(new Vector3(-1, 0, 0));
        TryAdd(new Vector3(0, 0, 1));
        TryAdd(new Vector3(0, 0, -1));

        void TryAdd(Vector3 offset)
        {
            if (!tiles.ContainsKey(p[i].pos + offset))
                nearpos.Add(offset);
        }

        if (nearpos.Count <= 0)
        {
            p[i].active = false;
            return;
        }

        int index = Random.Range(0, nearpos.Count);
        Vector3 offset = nearpos[index];

        nearpos.RemoveAt(index);

        if (nearpos.Count > 0)
        {
            float rand = Random.Range(0f, 100f);
            Vector3 newpos = p[i].pos + nearpos[Random.Range(0, nearpos.Count)];

            if (rand >= 98.0f)
            {
                List<Vector3> newlist = new List<Vector3>();

                for (int j = 0; j < p[i].path.Count; j++)
                    newlist.Add(p[i].path[j]);
                newlist.Add(newpos);

                P pp = new P(newlist, newpos, Instantiate(linePrefab));

                p.Add(pp);
                Debug.LogWarning(pp.line);

                //p[p.Count - 1].line = Instantiate(linePrefab);

                p[p.Count - 1].AddLinePos(p[i].line.GetPosition(p[i].line.positionCount - 1));

                newtile(p[p.Count - 1].pos, p.Count - 1, 1, new Color32(121, 191, 101, 255));
            }
            else if (rand <= 80.0f)
            {
                const float diff = 0.5f;
                float offsetdiff = 0;
                Color32 color = new Color32(121, 191, 101, 255);

                float yrand = Random.Range(0f, 100f);
                if (yrand < 50)
                    offsetdiff = 0;
                else if (yrand < 80)
                    offsetdiff = 1;
                else
                {
                    offsetdiff = 3;
                    color = Color.grey;
                }

                newtile(newpos, i, 1 + offsetdiff * diff, color);
            }
        }

        p[i].pos = p[i].pos + offset;
        p[i].path.Add(p[i].pos);

        Debug.Log(i);

        newtile(p[i].pos, i, 1, new Color32(121, 191, 101, 255));
        p[i].AddLinePos(p[i].pos * 4 + new Vector3(0, 0.05f, 0));


    }
    public float r;

    void newtile(Vector3 pos, int index, float Y, Color32 color)
    {
        if (!tiles.ContainsKey(pos))
        {
            p[index].tile.Add((Instantiate(tile, pos * 4 + new Vector3(0, 0 + 10, 0), Quaternion.identity).transform, Y, true));
            p[index].tile[p[index].tile.Count - 1].t.GetComponent<Renderer>().material.color = color;
            tiles.Add(pos, 0);
        }

    }

    /*
    void OnDrawGizmos()
    {
        for (int i = 0; i < p.Count; i++)
        {
            float y = 0;

            if (i == 0) { Gizmos.color = Color.red; y = 0.5f; }
            else if (i == 1) { Gizmos.color = Color.yellow; y = 1.0f; }
            else { Gizmos.color = Color.white; y = 1.5f; }

            for (int j = 0; j < p[i].path.Count; j++)
            {
                Gizmos.DrawSphere(p[i].path[j]*4, 0.4f);
            }
        }
    }*/
}
