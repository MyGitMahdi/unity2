using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class SphereContRot : MonoBehaviour
{
    BluetoothHelper bluetoothHelper;
    Vector3 startPosition;
    Quaternion startRotation;

    // Use this for initialization
    void Start()
    {
        try
        {
            bluetoothHelper = BluetoothHelper.GetInstance();
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnDataReceived += OnDataReceived;

            // Replace with your Arduino device name
            bluetoothHelper.setDeviceName("AmelTech");

            // Connect to the Arduino device
            bluetoothHelper.Connect();

            startPosition = transform.position;
            startRotation = transform.rotation;
        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new position based on roll and pitch values
        float rollAngle = transform.rotation.eulerAngles.z;
        float pitchAngle = transform.rotation.eulerAngles.x;

        float x = Mathf.Sin(Mathf.Deg2Rad * rollAngle);
        float y = Mathf.Sin(Mathf.Deg2Rad * pitchAngle);
        float z = Mathf.Cos(Mathf.Deg2Rad * rollAngle) * Mathf.Cos(Mathf.Deg2Rad * pitchAngle);

        Vector3 newPosition = startPosition + new Vector3(x, y, z);

        // Move the sphere to the new position
        transform.position = newPosition;
    }

    void OnConnected()
    {
        // Start listening for incoming data
        bluetoothHelper.StartListening();
    }

    void OnDataReceived()
    {
        string receivedData = bluetoothHelper.Read();
        Debug.Log("Received data: " + receivedData);

        // Parse the received data into roll and pitch values
        string[] angles = receivedData.Split(',');

        if (angles.Length >= 2)
        {
            float roll, pitch;
            if (float.TryParse(angles[0], out roll) && float.TryParse(angles[1], out pitch))
            {
                // Update the rotation of the sphere based on roll and pitch values
                Quaternion newRotation = Quaternion.Euler(pitch, 0f, roll);
                transform.rotation = startRotation * newRotation;
            }
        }
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}
