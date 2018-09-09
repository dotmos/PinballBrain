using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System;

public class AsyncSerial : IDisposable { 
    public SerialPort serialPort;

    private Thread portThread;
    private Queue outputQueue; // To serial
    private Queue inputQueue; // From serial

    private string portName;
    private int baudRate;
    private int readTimeout = 100;
    
    private int queueLenght = 100;

    public bool threadRunning;

    
    public AsyncSerial(string portName, int baudRate, int readTimeout, int QueueLenght) : this(portName, baudRate) {
        this.readTimeout = readTimeout;
        this.queueLenght = QueueLenght;
    }
    
    public AsyncSerial(string portName, int baudRate) {
        this.portName = portName;
        this.baudRate = baudRate;

        Setup();
    }

    void Setup() {
        serialPort = new SerialPort(this.portName, this.baudRate);
        serialPort.ReadTimeout = this.readTimeout;
        serialPort.DtrEnable = true;
        serialPort.RtsEnable = true;
        serialPort.Open();
        if (!serialPort.IsOpen) {
            Debug.LogError("Could not open " + portName);
        } else {
            Debug.Log("Opened " + portName);
        }

        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());

        threadRunning = true;
        portThread = new Thread(Loop);
        portThread.Start();
    }

    void StopThread() {
        lock (this) {
            threadRunning = false;
        }
    }
     
    bool IsAlive() {
        lock (this) {
            return threadRunning;
        }
    }

    public bool HasData() {
        if(inputQueue.Count == 0) {
            return false;
        } else {
            return true;
        }
    }

    /// <summary>
    /// Read the oldest data from the serial port
    /// </summary>
    /// <returns></returns>
    public byte[] Read() {
        if (inputQueue.Count == 0) {
            return null;
        } else {
            return (byte[])inputQueue.Dequeue();
        }
    }

    //Write to the serial port
    public void Write(byte[] dataToSend) {
        outputQueue.Enqueue(dataToSend);
    }

    void Loop() {
        while (IsAlive()) {
            // Read
            object dataComingFromDevice = ReadProtocol();
            if (dataComingFromDevice != null) {
                if (inputQueue.Count < queueLenght) {
                    inputQueue.Enqueue(dataComingFromDevice);
                }
            }
            // Send
            if (outputQueue.Count != 0) {
                byte[] dataToSend = (byte[])outputQueue.Dequeue();
                SendProtocol(dataToSend);
            }
        }

        serialPort.Close();
        serialPort.Dispose();
    }

    public void Dispose() {
        StopThread();
    }

    //public abstract string ReadProtocol(); // with Alpha 0.1 Overide by the protocol wrmhlThread_ReadLines to be used as reading protocol.
    byte[] ReadProtocol() {
        //return serialPort.ReadLine();
        
        int bytesToRead = serialPort.BytesToRead;
        if(bytesToRead > 0) {
            byte[] bytes = new byte[bytesToRead];
            serialPort.Read(bytes, 0, bytesToRead);
            return bytes;
        } else {
            return null;
        }
    }

    //public abstract void SendProtocol(object message); // with Alpha 0.1 Overide by the protocol wormhlThread_ReadLines to be used as reading protocol.
    void SendProtocol(byte[] data) {
        //serialPort.WriteLine((string)message);
        serialPort.Write(data, 0, data.Length);
    }
}