using UnityEngine;
using System.Collections;

public class EventObject {

    private string eType;
    private Hashtable table;

    public EventObject(string type)
    {
        this.eType = type;
        table = new Hashtable();
    }

    public void AddParameters(int key, string value)
    {
        table.Add(key, value);
    }

    public string GetEventType()
    {
        return eType;
    }

    public Hashtable GetParameters()
    {
        return table;
    }

}
