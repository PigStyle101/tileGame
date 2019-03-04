using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class snaptogrid : MonoBehaviour {

    public float gridSideLength = 1.0f;
    private Color startColor;
    

    private void Start()
    {
        
    }

    void Update()
    {

        // we don't want to force things into a grid during play mode
        if (Application.isPlaying) { return; }

        transform.localPosition = GetSnappedPosition(transform.localPosition);
    }

    public Vector3 GetGridPosition()
    {
        return GetSnappedPosition(transform.position);
    }

    public Vector3 GetSnappedPosition(Vector3 position)
    {

        // not fatal in the Editor, but just better not to divide by 0 if we can avoid it
        if (gridSideLength == 0) { return position; }

        Vector3 gridPosition = new Vector3(
            gridSideLength * Mathf.Round(position.x / gridSideLength),
            gridSideLength * Mathf.Round(position.y / gridSideLength),
            gridSideLength * Mathf.Round(position.z / gridSideLength)
        );
        return gridPosition;
    }

   
}
