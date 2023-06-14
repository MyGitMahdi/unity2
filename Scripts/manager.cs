using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using System;
using System.Text;

public class manager : MonoBehaviour {

    BluetoothHelper bluetoothHelper;
    string deviceName = "AmelTech"; // Replace with your Bluetooth device name

    public Text text;

    public GameObject sphere;

    string received_message;

    void Start () {
        try
        {
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived;

            bluetoothHelper.setTerminatorBasedStream("\n");

            LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();

            Debug.Log(ds);
        }
        catch (Exception ex)
        {
            Debug.Log (ex.Message);
            text.text = ex.Message;
        }
    }

    IEnumerator blinkSphere()
    {
        sphere.GetComponent<Renderer>().material.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        sphere.GetComponent<Renderer>().material.color = Color.green;
    }

    void OnMessageReceived()
    {
        received_message = bluetoothHelper.Read();
        Debug.Log(received_message);
        text.text = received_message;
    }

    void OnConnected()
    {
        sphere.GetComponent<Renderer>().material.color = Color.green;
        try{
            bluetoothHelper.StartListening ();
        }catch(Exception ex){
            Debug.Log(ex.Message);
        }
    }

    void OnConnectionFailed()
    {
        sphere.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("Connection Failed");
    }

    void OnGUI()
    {
        if(bluetoothHelper!=null)
            bluetoothHelper.DrawGUI();
        else 
            return;

        if(!bluetoothHelper.isConnected())
        {
            if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height/10, Screen.width/5, Screen.height/10), "Connect"))
            {
                if(bluetoothHelper.isDevicePaired())
                    bluetoothHelper.Connect ();
                else
                    sphere.GetComponent<Renderer>().material.color = Color.magenta;
            }
        }

        if(bluetoothHelper.isConnected())
        {
            if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height - 2*Screen.height/10, Screen.width/5, Screen.height/10), "Disconnect"))
            {
                bluetoothHelper.Disconnect ();
                sphere.GetComponent<Renderer>().material.color = Color.blue;
            }

            if(GUI.Button(new Rect(Screen.width/2-Screen.width/10, Screen.height/10, Screen.width/5, Screen.height/10), "Send text"))
            {
                bluetoothHelper.SendData(new Byte[] {0, 0, 85, 0, 85});
            }
        }
    }

    void OnDestroy()
    {
        if(bluetoothHelper!=null)
            bluetoothHelper.Disconnect ();
    }
}
