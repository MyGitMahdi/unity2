﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class Demo : MonoBehaviour
{
    private BluetoothHelper helper;
    private bool isScanning;
    private bool isConnecting;

    private string data;
    private string tmp;
    private LinkedList<BluetoothDevice> devices;

    // Start is called before the first frame update
    void Start()
    {
        data = "";
        tmp = "";
        try
        {
            BluetoothHelper.BLE = true;
            helper = BluetoothHelper.GetInstance();
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.OnScanEnded += OnScanEnded;
            helper.OnDataReceived += OnDataReceived;
            helper.setTerminatorBasedStream("\n"); // Every message ends with a new line character
        }
        catch (Exception)
        {
            Debug.LogError("Failed to initialize BluetoothHelper.");
        }
    }

    void OnDataReceived()
    {
        data += "\n<" + helper.Read();
    }

    void OnScanEnded(LinkedList<BluetoothDevice> devices)
    {
        this.isScanning = false;
        this.devices = devices;
    }

    void OnConnected()
    {
        isConnecting = false;
    }

    void OnConnectionFailed()
    {
        isConnecting = false;
    }

    void OnGUI()
    {
        if (helper == null)
            return;

        if (!helper.isConnected() && !isScanning && !isConnecting)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Start Scanning"))
            {
                isScanning = helper.ScanNearbyDevices();
            }

            if (devices != null && devices.First != null)
            {
                DrawDeviceButtons();
            }
        }
        else if (!helper.isConnected() && isScanning)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Scanning...");
        }
        else if (helper.isConnected())
        {
            GUI.TextArea(new Rect(Screen.width / 4, 2 * Screen.height / 10, Screen.width / 2, 7 * Screen.height / 10), data);
            tmp = GUI.TextField(new Rect(Screen.width / 4, Screen.height / 10, Screen.width / 2, Screen.height / 10 - 10), tmp);

            if (GUI.Button(new Rect(3 * Screen.width / 4 + 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Send"))
            {
                helper.SendData(tmp);
                data += "\n>" + tmp;
                tmp = "";
            }

            if (GUI.Button(new Rect(3 * Screen.width / 4 + 10, 8 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                helper.Disconnect();
            }
        }
    }

    private void DrawDeviceButtons()
    {
        LinkedListNode<BluetoothDevice> node = devices.First;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string bluetoothName = node.Value.DeviceName;

                if (bluetoothName == "AmelTech")
                {
                    if (GUI.Button(new Rect((j + 1) * Screen.width / 5 + 5, (i + 2) * Screen.height / 10 + 5, Screen.width / 5 - 10, Screen.height / 10 - 10), bluetoothName))
                    {
                        helper.setDeviceName(bluetoothName);

                        try
                        {
                            helper.Connect();
                            isConnecting = true;
                        }
                        catch (Exception)
                        {
                            Debug.LogError("Failed to connect to the selected Bluetooth device.");
                            isConnecting = false;
                        }
                    }
                }

                node = node.Next;

                if (node == null)
                    return;
            }
        }
    }

    void OnDestroy()
    {
        if (helper != null)
            helper.Disconnect();
    }
}
