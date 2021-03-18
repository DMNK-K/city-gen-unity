using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCorner
{
    public Vector3 Position { get; private set; }
    public List<Street> ConnectedTo { get; private set; }
    public List<bool> ConnectedOnA { get; private set; }
    public List<bool> ConnectedOnLeft { get; private set; }

    public IntersectionCorner(Vector3 pos)
    {
        Position = pos;
    }

    public void FillConnectionData(List<Street> possibleConnected)
    {
        ConnectedTo = new List<Street>();
        ConnectedOnA = new List<bool>();
        ConnectedOnLeft = new List<bool>();
        for (int i = 0; i < possibleConnected.Count; i++)
        {
            if (possibleConnected[i].CornerAL.Similar(Position))
            {
                ConnectedTo.Add(possibleConnected[i]);
                ConnectedOnA.Add(true);
                ConnectedOnLeft.Add(true);
            }
            else if (possibleConnected[i].CornerAR.Similar(Position))
            {
                ConnectedTo.Add(possibleConnected[i]);
                ConnectedOnA.Add(true);
                ConnectedOnLeft.Add(false);
            }
            else if (possibleConnected[i].CornerBL.Similar(Position))
            {
                ConnectedTo.Add(possibleConnected[i]);
                ConnectedOnA.Add(false);
                ConnectedOnLeft.Add(true);
            }
            else if (possibleConnected[i].CornerBR.Similar(Position))
            {
                ConnectedTo.Add(possibleConnected[i]);
                ConnectedOnA.Add(false);
                ConnectedOnLeft.Add(false);
            }
        }
    }

    //public Vector3 GetSidewalkOffset(int i)
    //{
    //    //return Conne
    //}
}
