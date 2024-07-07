using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private void Start()
    {
        char test = 'a';
        byte[] data = BitConverter.GetBytes(test);

        char test2 = BitConverter.ToChar(data);
        string debug = "";
        for (int i = 0; i < data.Length; i++)
        {
            debug += data[i].ToString();
        }
        Debug.Log(debug);
        Debug.Log(test2);

        for (int i = 0; i < 4; i++)
        {
            //data = BitConverter.ToChar(data);
        }
    }
}