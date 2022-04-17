using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_WaypointLine
{
    private static readonly Vector3 vec3Offset = new Vector3(0, UX_Tile.LIFT_DIST, 0);

    private readonly LineRenderer[] mLineRenderer = new LineRenderer[9];
    // [board count][clone count]
    private readonly Coord[][] mBoardOffsets;
    private int mBoardID = 0;

    public Vector3[] Positions {
        set {
            for (int i = 0; i < mBoardOffsets[mBoardID].Length; i++) {
                Vector3[] positions = new Vector3[value.Length];
                for (int j = 0; j < positions.Length; j++) {
                    positions[j] = value[j] + mBoardOffsets[mBoardID][i].ToVec3() + vec3Offset;
                }
                mLineRenderer[i].positionCount = positions.Length;
                mLineRenderer[i].SetPositions(positions);
            }
        }
    }
    public int BoardID { set => mBoardID = value; }
    public bool Active {
        set {
            foreach (LineRenderer clone in mLineRenderer) {
                clone.gameObject.SetActive(value);
            }
        }
    }
    public Material Mat { get => mLineRenderer[0].material; }

    public UX_WaypointLine(
        Coord[][] boardOffsets,
        LineRenderer baseLineRenderer,
        Transform waypointsParent,
        string name) {

        mBoardOffsets = boardOffsets;

        for (int i = 0; i < mLineRenderer.Length; i++) {
            mLineRenderer[i] =
                Object.Instantiate(baseLineRenderer.gameObject, waypointsParent).GetComponent<LineRenderer>();
            if (i == 0) mLineRenderer[i].name = name;
            else mLineRenderer[i].name = name + " - Clone " + Util.DirToString(i - 1);
            mLineRenderer[i].gameObject.SetActive(false);
        }
    }
}
