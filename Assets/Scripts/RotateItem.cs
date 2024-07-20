using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour
{
    private Vector3 Spin = new Vector3(0, 30, 0);

    // Update is called once per frame
    void Update()
    {
        //Continuous rotate the GameObject around y-axis
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (Spin * Time.deltaTime));
    }
}
