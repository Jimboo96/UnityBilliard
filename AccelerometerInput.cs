using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class AccelerometerInput : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM8", 9600);
    public GameObject gameBall;
    public Renderer rend;
    public Collider m_Collider;
    private Rigidbody rb;
    public Transform ball;
    public float speed;
    public static float x { get; set; }
    //public static float y { get; set; }
    //public static float z { get; set; }

    void Start()
    {
        gameBall = GameObject.Find("gameBall");
        ball = GameObject.Find("gameBall").transform;
        rend = GetComponent<Renderer>();
        m_Collider = GetComponent<Collider>();
        rend.enabled = true; 
        rb = GetComponent<Rigidbody>();
        transform.position = new Vector3((gameBall.transform.position.x - 10), (gameBall.transform.position.y + 1), gameBall.transform.position.z);
        stream.ReadTimeout = 50;
        stream.Open();

        if (stream.IsOpen == true)
        {
            StartCoroutine
            (
                AsynchronousReadFromArduino
                ((string s) => Debug.Log(s),     // Callback
                    () => Debug.LogError("Error!"), // Error callback
                    10000f                          // Timeout (milliseconds)
             ));
        }
        else
        {
            Debug.Log("Not open");
        }
    }

    void Update()
    {
        //Reset position near the gameBall
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector3((gameBall.transform.position.x - 10),(gameBall.transform.position.y + 1), gameBall.transform.position.z);
            Quaternion target = Quaternion.Euler(0, 0, 250);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, 1);
        }
        //Rotate around the gameBall
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 move = new Vector3(0, 1, 0);
            transform.RotateAround(gameBall.transform.position, move, 2);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 move = new Vector3(0, -1, 0);
            transform.RotateAround(gameBall.transform.position, move, 2);
        }
    }

    private void FixedUpdate()
    {
        FollowTargetWithRotation(gameBall.transform, gameBall.transform.position.x, (speed*2));
    }

    void FollowTargetWithRotation(Transform target, float distanceToStop, float speed)
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        transform.LookAt(target);
        transform.Rotate(0, 90, 90);
        rb.AddRelativeForce((new Vector3(0.0f,-x,moveHorizontal) * speed), ForceMode.Force);
    }

    void OnCollisionEnter(Collision col)
    {      
        if (col.gameObject.name == "gameBall")
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);
        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
                x = float.Parse(dataString);
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
}

