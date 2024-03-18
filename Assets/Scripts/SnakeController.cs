using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float bodySpeed;
    [SerializeField] private float steerSpeed;
    [SerializeField] private GameObject bodyPrefab;
    public GameManager gameManager;

    private int gap = 10;
    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    public StickController MoveStick;
    public AudioSource audioSource;
    public AudioClip foodSound;
     private Gyroscope gyro;
    private bool gyroSupported;
    private float gyroSensitivity = 5.0f; 
    private float gyroSmoothFactor = 0.9f; 
    private Queue<float> gyroHistory = new Queue<float>(); 
    private int gyroHistoryLength = 10;

    void Start()
    {

        
        InvokeRepeating("UpdatePositionHistory", 0f, 0.01f);
        bigSnake();
       bigSnake();

         gyroSupported = SystemInfo.supportsGyroscope;
        bool useGyro = PlayerPrefs.GetInt("UseGyroControl", 0) == 1;
        if (useGyro && SystemInfo.supportsGyroscope)
        {
            gyroSupported = true;
            gyro = Input.gyro;
            gyro.enabled = true;
        }
        else
        {
            gyroSupported = false;
        }


    }
    void Awake()
    {
        if (MoveStick != null)
        {
            MoveStick.StickChanged += MoveStick_StickChanged;
        }
    }

    private Vector2 MoveStickPos = Vector2.zero;

    private void MoveStick_StickChanged(object sender, StickEventArgs e)
    {
        MoveStickPos = e.Position;
    }
    void Update()
{
     if (gyroSupported)
        {
            float gyroInput = gyro.rotationRate.y * gyroSensitivity;

            
            gyroHistory.Enqueue(gyroInput);
            if (gyroHistory.Count > gyroHistoryLength)
            {
                gyroHistory.Dequeue();
            }

            float smoothedInput = 0f;
            foreach (float input in gyroHistory)
            {
                smoothedInput += input;
            }
            smoothedInput /= gyroHistory.Count;

           
            transform.Rotate(Vector3.up * smoothedInput * steerSpeed * Time.deltaTime);
        }
    else
    {
        float h = Mathf.Abs(MoveStickPos.x) > Mathf.Abs(Input.GetAxis("Horizontal")) ? MoveStickPos.x : Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * h * steerSpeed * Time.deltaTime);
    }
    transform.position += transform.forward * moveSpeed * Time.deltaTime;
    UpdateBodyParts();
}

    private void UpdateBodyParts()
    {
        int index = 0;
        foreach (GameObject body in bodyParts)
        {
            Vector3 point = positionHistory[Math.Min(index * gap, positionHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * bodySpeed * Time.deltaTime;

            body.transform.LookAt(point);

            index++;
        }
    }


    void UpdatePositionHistory()
    {
        Debug.Log("UpdatePositionHistory");
       
        positionHistory.Insert(0, transform.position);

       
        if (positionHistory.Count > 500)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }
    private void bigSnake()
    {
        GameObject body = Instantiate(bodyPrefab);
        bodyParts.Add(body);
        audioSource.PlayOneShot(foodSound);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            gameManager.EndGame();
        }
        if (other.CompareTag("food"))
        {
            bigSnake();
            Destroy(other.gameObject);
            gameManager.AddScore(1);
            gameManager.GenerateFood();
        }
    }

}
