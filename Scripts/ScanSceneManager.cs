using System;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using UnityEngine.UI;

public class ScanSceneManager : MonoBehaviour
{
    BluetoothHelper bluetoothHelper;

    public Text statusText;
    public GameObject sphere;

    string receivedMessage;

    bool isScanning = false;
    List<BluetoothDevice> nearbyDevicesList = new List<BluetoothDevice>();

    void Start()
    {
        try
        {
            BluetoothHelper.BLE = true;  // Use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance();

            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived;
            bluetoothHelper.OnScanEnded += OnScanEnded;

            BluetoothHelperCharacteristic characteristic = new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214");
            characteristic.setService("19B10000-E8F2-537E-4F6C-D104768A1214");
            bluetoothHelper.setTxCharacteristic(characteristic);
            bluetoothHelper.setRxCharacteristic(characteristic);

            bluetoothHelper.setFixedLengthBasedStream(10);

            sphere.GetComponent<Renderer>().material.color = Color.black;
            statusText.text = "Click 'Connect' to start scanning";
        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
            sphere.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log(ex.ToString());
            statusText.text = ex.Message;
        }
    }

    void OnMessageReceived()
    {
        receivedMessage = bluetoothHelper.Read();
        statusText.text = receivedMessage;
        Debug.Log(System.DateTime.Now.Second);
    }

    void OnScanEnded(LinkedList<BluetoothDevice> nearbyDevices)
    {
        nearbyDevicesList.Clear();
        statusText.text = "Found " + nearbyDevices.Count + " AmelTech";

        foreach (BluetoothDevice device in nearbyDevices)
        {
            nearbyDevicesList.Add(device);
        }

        isScanning = false;
    }

    void Update()
    {
        if (!bluetoothHelper.IsBluetoothEnabled())
        {
            bluetoothHelper.EnableBluetooth(true);
        }
    }

    void OnConnected()
    {
        statusText.text = "Connected to AmelTech";
        sphere.GetComponent<Renderer>().material.color = Color.green;
        try
        {
            bluetoothHelper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnConnectionFailed()
    {
        sphere.GetComponent<Renderer>().material.color = Color.red;
        statusText.text = "Connection Failed";
    }

    public void ConnectButtonClicked()
    {
        isScanning = true;
        statusText.text = "Scanning...";
        bluetoothHelper.ScanNearbyDevices();
    }

    public void DisconnectButtonClicked()
    {
        bluetoothHelper.Disconnect();
        sphere.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void SendTextButtonClicked()
    {
        bluetoothHelper.SendData(new byte[] { 0, 1, 2, 3, 4 });
    }

    void OnGUI()
    {
        if (isScanning)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Scanning...");
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Connect"))
            {
                ConnectButtonClicked();
            }
        }

        if (bluetoothHelper != null && bluetoothHelper.isConnected())
        {
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                DisconnectButtonClicked();
            }

            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Send text"))
            {
                SendTextButtonClicked();
            }
        }

        if (isScanning && nearbyDevicesList.Count > 0)
        {
            string devicesText = "Nearby Devices:\n";
            foreach (BluetoothDevice device in nearbyDevicesList)
            {
                devicesText += device.DeviceName + "\n";
            }
            GUI.TextArea(new Rect(Screen.width / 2 - Screen.width / 10, 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), devicesText);
        }
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}
