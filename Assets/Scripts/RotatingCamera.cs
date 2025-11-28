using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCamera : MonoBehaviour
{
    public float rotationTime = 0.2f;
    private Transform player;
    private bool isRotation=false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position;
        Rotate();
    }
    public void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.Q) &&!isRotation)
        {
            StartCoroutine(RotateAround(-45, rotationTime));
        }
        if (Input.GetKeyDown(KeyCode.E) &&!isRotation)
        {
            StartCoroutine(RotateAround(45, rotationTime));
        }
    }
    IEnumerator RotateAround(float angle,float time)
    {
        //旋转完成需要的帧数
        float number = 60 * time;
        float nextAngle = angle / number;
        isRotation = true;
        for(int i = 0; i < number; i++) 
        {
            transform.Rotate(new Vector3(0, 0, nextAngle));
            yield return new WaitForFixedUpdate();
        }
        isRotation = false;
        
    }
}
