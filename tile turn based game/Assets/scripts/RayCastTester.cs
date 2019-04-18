using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTester : MonoBehaviour
{
    public string TagFilter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RayCastTesterMethod();
    }

    public void RayCastTesterMethod()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.transform.tag == TagFilter)
                {
                    Debug.Log(hit.transform.name);
                }
            }
        }
    }
}
