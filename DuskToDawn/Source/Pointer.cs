using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pointer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		gameObject.transform.Rotate(new Vector3(0, 2, 0));
    }
}
