using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoardPin : MonoBehaviour
{
    Vector3 boardTransform = new Vector3(0, 0, 0);
    Vector3 boardScale = new Vector3(4.545f, 4.545f, 3.0f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //pin the board to the centre. that's it.
        gameObject.transform.position = boardTransform;
        gameObject.transform.localScale = boardScale;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
