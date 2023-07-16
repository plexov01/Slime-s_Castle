using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Renderer bgRenderer;
    public bool scroolBG = false;

    void Update()
    {
        if (scroolBG)
        {
            bgRenderer.material.mainTextureOffset += new Vector2(0, -speed * Time.deltaTime);
        }
    }
}
