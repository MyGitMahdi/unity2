using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class BLEServicesManager : MonoBehaviour
{
    private BluetoothHelper bluetoothHelper;
    private float timer;

    void Start()
    {
        timer = 0;
        try
        {
            BluetoothHelper.BLE = true;  // Use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance("AmelTech");
            bluetoothHelper.OnConnected += () =>
            {
                Debug.Log("Connected");
                sendData();
            };
            bluetoothHelper.OnConnectionFailed += () =>
            {
                Debug.Log("Connection failed");
            };
            bluetoothHelper.OnScanEnded += OnScanEnded;
            bluetoothHelper.OnServiceNotFound += (serviceName) =>
            {
                Debug.Log(serviceName);
            };
            bluetoothHelper.OnCharacteristicNotFound += (serviceName, characteristicName) =>
            {
                Debug.Log(characteristicName);
            };
            bluetoothHelper.OnCharacteristicChanged += (value, characteristic) =>
            {
                Debug.Log(characteristic.getName());
                Debug.Log(System.Text.Encoding.ASCII.GetString(value));
            };

            BluetoothHelperService service = new BluetoothHelperService("19B10000-E8F2-537E-4F6C-D104768A1214");
            service.addCharacteristic(new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214"));
            bluetoothHelper.Subscribe(service);
            bluetoothHelper.ScanNearbyDevices();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void OnScanEnded(LinkedList<BluetoothDevice> devices)
    {
        if (devices.Count == 0)
        {
            bluetoothHelper.ScanNearbyDevices();
            return;
        }

        try
        {
            bluetoothHelper.setDeviceName("AmelTech");
            bluetoothHelper.Connect();
            Debug.Log("Connecting");
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

    void Update()
    {
        if (bluetoothHelper == null)
            return;
        if (!bluetoothHelper.isConnected())
            return;
        timer += Time.deltaTime;

        if (timer < 5)
            return;
        timer = 0;
        sendData();
    }

    void sendData()
    {
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214");
        ch.setService("19B10000-E8F2-537E-4F6C-D104768A1214");
        bluetoothHelper.WriteCharacteristic(ch, "10001000");
    }
}
