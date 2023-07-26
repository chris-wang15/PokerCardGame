using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager uniqueInstance;
    private static readonly object locker = new object();

    private DataManager()
    {
        // loadData();
    }

    public static DataManager GetInstance()
    {
        lock (locker)
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new DataManager();
            }
        }

        return uniqueInstance;
    }

    public bool InitAiControl(int id)
    {
        return id != 1;
    }
}
