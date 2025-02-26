using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class dddd : MonoBehaviour
{
    struct player
    {
        public playercomponent playerComponent;

        public player(Rigidbody r) => playerComponent = new playercomponent(r);
    }
    public struct playercomponent
    {
        public Rigidbody rb;
        public playercomponent(Rigidbody r) => rb = r;
    }

    struct SplitRaycast
    {
        public RaycastHit hit;
        public Vector3 point;

        public SplitRaycast(RaycastHit h)
        {
            hit = h;
            point = hit.point;
        }
    }

    void Start()
    {
        Rigidbody GetRig(string name)
        {
            GameObject obj(string n) => GameObject.Find(n);
            return obj(name).GetComponent<Rigidbody>();
        }
        string GetName(Transform obj = null)
        {
            if (obj == null) obj = transform;
            return transform.name;
        }

        Vector3 AddDir(List<(Vector3 dir, Vector3 axis)> diraxis)
        {
            Vector3 somme = Vect0();
            for (int i = 0; i < GetLength(ref diraxis); i++)
            {
                somme += MultiplyDirAxis(diraxis[i].dir, diraxis[i].axis);
            }
            int GetLength(ref List<(Vector3, Vector3)> list)
            {
                int l = 0;
                while (l < list.ToArray().Length) l++;
                return l;
            }
            return somme;
            Vector3 Vect0() => new Vector3(0, 0, 0);
            Vector3 MultiplyDirAxis(Vector3 dir, Vector3 axis)
            {
                return RemoveZ(dir) * GetAxis(axis);

                Vector3 RemoveZ(Vector3 dir)
                {
                    return new Vector3(dir.x, 0, dir.z);
                }
                float GetAxis(Vector3 axis)
                {
                    if (Axis(axis) == "null") return 0;
                    return Input.GetAxis(Axis(axis));

                    string Axis(Vector3 axis)
                    {
                        switch ((axis.x, axis.y, axis.z))
                        {
                            case ((1, 0, 0)):
                                return "Horizontal";
                            case ((0, 0, 1)):
                                return "Vertical";
                        }
                        return "null";
                    }
                }
            }
        }
        Vector3 MultiplyDirForce(Vector3 dir, float force) => dir * force;
        Vector3 CreateVector3((int x, int y, int z) v) => new Vector3(v.x, v.y, v.z);
        Vector3 GetSelfDir(string axis)
        {
            switch(axis)
            {
                case "forward": return transform.forward;
                case "right": return transform.right;
            }
            return Vector3.zero;
        }
        SplitRaycast VerticalRaycast(GameObject startObj, bool up = false)
        {
            RaycastHit hit = CreateEmptyRaycast();
            Physics.Raycast(objCenter(startObj.transform), VerticalDir(up), out hit);
            return new SplitRaycast(hit);

            RaycastHit CreateEmptyRaycast() => new SplitRaycast().hit;
            Vector3 Up() => new Vector3(0, 1, 0);
            Vector3 objCenter(Transform startObj) => startObj.position;
            Vector3 VerticalDir(bool up)
            {
                if (up) return Up();
                return -Up();
            }
        }
        (float x, float y, float z) SeparateVector3(Vector3 v) => (v.x, v.y, v.z);
        bool maxDist((float x1, float y1, float z1) p1, (float x2, float y2, float z2) p2, float maxDist) => Vector3.Distance(new Vector3(p1.x1, p1.y1, p1.z1), new Vector3(p2.x2, p2.y2, p2.z2)) > maxDist;
        Vector3 down(bool self = false)
        {
            if (self) return -transform.up;
            return -Vector3.up;
        }

        player prb = new player(GetRig(GetName()));

        prb.playerComponent.rb.velocity += MultiplyDirForce(AddDir(new List<(Vector3, Vector3)> { (GetSelfDir("forward"), CreateVector3((1, 0, 0)) ), 
                                                                                                  (GetSelfDir("right"), CreateVector3((1, 0, 0)) )} ),
                                                                                                  2.0f);

        SplitRaycast groundedhit = VerticalRaycast(gameObject, true);

        if (maxDist(SeparateVector3(groundedhit.point), SeparateVector3(transform.position), 2.0f))
            transform.Translate(down() * 2.0f);
    }

    public class codenormal : MonoBehaviour
    {
        Rigidbody rb;
        void Start()
        {
            rb.velocity += (Input.GetAxis("Horizontal") * transform.forward + Input.GetAxis("Vertical") * transform.right) * 2.0f;

            if (!Physics.Raycast(transform.position, -Vector3.up, 2))
                transform.Translate(Vector3.down * 2.0f);
        }
    }
}
