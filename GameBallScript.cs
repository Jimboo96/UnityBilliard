using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBallScript : MonoBehaviour {
    public AccelerometerInput accelerometer;
    private Rigidbody rb;
    public float speed;
    public GameObject billiardStick;
    public Renderer billiardStickRenderer;
    public Collider m_Collider;
    public Rigidbody billiardStickRB;

    void Start () {
        rb = GetComponent<Rigidbody>();
        billiardStick = GameObject.Find("stick");
        accelerometer = billiardStick.GetComponent<AccelerometerInput>();
        billiardStickRenderer = accelerometer.GetComponent<Renderer>();
        m_Collider = accelerometer.GetComponent<Collider>();
        billiardStickRB = accelerometer.GetComponent<Rigidbody>();
    }

	void FixedUpdate () {
        speed = rb.velocity.magnitude;
        if (speed < 0.1)
        {
            billiardStickRenderer.enabled = true;
            m_Collider.enabled = true;
            billiardStickRB.constraints = RigidbodyConstraints.None;
        }
        else
        {
            billiardStickRenderer.enabled = false;
            m_Collider.enabled = false;
        }
    }
}
