using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RimStreetLine
{
    public StreetLine Line { get; private set; }
    public float AngularPosOnRim { get; private set; }
    public bool AngularPosCalculatedFromA { get; private set; }

    public RimStreetLine(StreetLine line)
    {
        Line = line;
        AngularPosCalculatedFromA = (Line.AOnRim);
        AngularPosOnRim = (Line.AOnRim) ? -Vector2.SignedAngle(-Vector2.up, Line.A) : -Vector2.SignedAngle(-Vector2.up, Line.B);
    }

    public RimStreetLine(StreetLine line, bool calcAngleFromA)
    {
        Line = line;
        AngularPosCalculatedFromA = calcAngleFromA;
        AngularPosOnRim = (calcAngleFromA) ? -Vector2.SignedAngle(-Vector2.up, Line.A) : -Vector2.SignedAngle(-Vector2.up, Line.B);
    }
}
