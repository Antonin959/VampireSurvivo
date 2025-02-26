using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class playerrscript : MonoBehaviour
{
    Vector3 basepos;
    Quaternion baserot;

    public Transform drawings;
    public GameObject rangeSphere, enemie;
    public MeshFilter placepreview = null;
    public GameObject[] Towers;

    float speed = 5.0f;
    int previewtower = 0;

    Transform LinkTowerObject = null;


    void Start()
    {
        basepos = transform.position;
        baserot = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Instantiate(enemie);
        /*
        if (!placepreview.gameObject.activeSelf)
        {*/
            Mov();
            TowerEdit();
        //}
        //else
        //{
            if (previewtower > 0 && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.tag == "normal")
            {
                Debug.Log(hit.transform.name);

                placepreview.transform.position = hit.point + new Vector3(0, 0.5f, 0);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Instantiate(Towers[previewtower], hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);

                    placepreview.gameObject.SetActive(false);
                }
            }
            else
                placepreview.transform.position = new Vector3(0, -100, 0);
        //}
        
    }

    void Mov()
    {
        basepos += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.CompareTag("tower"))
            {
                LinkTowerObject = hit.transform;
                Debug.Log(LinkTowerObject.gameObject, LinkTowerObject);
                LinkTowerObject = hit.transform.GetChild(0).GetChild(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            LinkTowerObject = null;
        }

        if (LinkTowerObject != null)
        {
            transform.position = LinkTowerObject.position;
            transform.rotation = LinkTowerObject.rotation;
        }
        else
        {
            transform.position = basepos;
            transform.rotation = baserot;
        }
    }

    void TowerEdit()
    {
        for (int i = 0; i < drawings.childCount; i++)
        {
            Destroy(drawings.GetChild(i).gameObject);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.transform.gameObject.tag == "tower")
            {
                Instantiate(rangeSphere, hit.transform.position, Quaternion.identity, drawings).transform.localScale = hit.transform.GetComponent<TowerManager>().range * new Vector3(1, 1, 1);
            }
        }
    }

    public void SpawTower(int tower)
    {
        placepreview.gameObject.SetActive(true);
        placepreview.mesh = Towers[tower].GetComponent<MeshFilter>().sharedMesh;
        previewtower = tower;
    }
    
}