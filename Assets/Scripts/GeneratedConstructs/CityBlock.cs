using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock
{
    public List<Street> Bounds { get; private set; }
    public bool OnRim { get; private set; }
    //private Vector3 rawCenter3D;
    public Vector3 Center3D { get; private set; }
    public Vector2 Center { get { return Center3D.UnShiftToV2(); } }


    public CityBlock(List<StreetTraversal> traversedBounds, bool hasRimSkip)
    {
        Bounds = new List<Street>();
        OnRim = hasRimSkip;
        Vector3 sum = Vector3.zero;
        int count = 0;
        //since the alg for finding CityBlocks runs clockwise
        //the corners for calculating the center will always be
        //on the right when the street was traversed from A to B,
        //and on the left when traversed from B to A

        //we only add the first corner from any street in order of traversal
        //since they repeat between one street and the next, except for
        //the situation when there is a rim skip
        for (int i = 0; i < traversedBounds.Count; i++)
        {
            Bounds.Add(traversedBounds[i].Street);
            sum += (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.CornerAR : traversedBounds[i].Street.CornerBL;
            count++;
            bool rimSkipAhead = (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.Line.BOnRim : traversedBounds[i].Street.Line.AOnRim;
            if (rimSkipAhead)
            {
                sum += (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.CornerBR : traversedBounds[i].Street.CornerAL;
                count++;
            }
        }
        Center3D = sum / count;

        ///To figure out the center we need the correct corners of streets which
        ///are the bounds of this CityBlock.
        ///The correct corners are the closer ones from the pairs of corners situated
        ///on ends of the street (A1 A2 and B1 B2), but to know which ones are the closer ones
        ///we need some other point from which to get the distance from. This is why rawCenter3D
        ///exists, which is a center computed from the points of the StreetLine that sits at the basis
        ///of every Street object. This rawCenter3D will usually be pretty close to the actual Center,
        ///sometimes even equal, but it can also not be, because streets have different widths. That is why
        ///the alg even needs to bother with finding the corners of streets - to account for how thick Streets are.


    }

}
