using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRoom
{
    public CRoom()
    {
        _roomType = CGlobal.ERoomType._empty;
    }

    private CGlobal.ERoomType _roomType;

    public CGlobal.ERoomType RoomType
    {
        get { return _roomType; }
        set { _roomType = value; }
    }

    public override string ToString()
    {
        return _roomType.ToString();
    }
}
