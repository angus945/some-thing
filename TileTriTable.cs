using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileTriTable
{
    float lerp;

    public Vector3[][] triVertices;

    Vector3 Vertices(float x, float y) => new Vector3(x, y);

    Vector3 VerticesUpLeft { get => Vertices(-.5f, .5f); }
    Vector3 VerticesUpRight { get => Vertices(.5f, .5f); }
    Vector3 VerticesDownLeft { get => Vertices(-.5f, -.5f); }
    Vector3 VerticesDownRight { get => Vertices(.5f, -.5f); }

    Vector3 VerticesLerpUpLeft { get => Vertices(-lerp, .5f); }
    Vector3 VerticesLerpUpRight { get => Vertices(lerp, .5f); }

    Vector3 VerticesLerpDwonLeft { get => Vertices(-lerp, -.5f); }
    Vector3 VerticesLerpDwonRight { get => Vertices(lerp, -.5f); }

    Vector3 VerticesLerpLeftDown { get => Vertices(-.5f, -lerp); }
    Vector3 VerticesLerpLeftUp { get => Vertices(-.5f, lerp); }

    Vector3 VerticesLerpRightUp { get => Vertices(.5f, lerp); }
    Vector3 VerticesLerpRightDown { get => Vertices(.5f, -lerp); }


    public TileTriTable(float lerp) : this()
    {
        this.lerp = lerp;

        triVertices = new Vector3[][]
        {
/*00000*/  new Vector3[] {  }, 
/*00001*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft }, 
/*00010*/  new Vector3[] { VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp }, 
/*00011*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp },
/*00100*/  new Vector3[] { VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight }, 
/*00101*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight }, 
/*00110*/  new Vector3[] { VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight }, 
/*00111*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight }, 
/*01000*/  new Vector3[] { VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft }, 
/*01001*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft },
/*01010*/  new Vector3[] { VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft},
/*01011*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft},
/*01100*/  new Vector3[] { VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft},
/*01101*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft},
/*01110*/  new Vector3[] { VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft},
/*01111*/  new Vector3[] { VerticesLerpLeftUp, VerticesUpLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightDown, VerticesDownRight, VerticesDownLeft, VerticesLerpLeftDown, VerticesLerpDwonLeft },
           
/*10000*/  new Vector3[] { VerticesLerpLeftDown, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpLeftDown, VerticesLerpUpLeft, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesLerpDwonLeft, VerticesLerpUpRight, VerticesLerpDwonRight , VerticesLerpDwonRight , VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightUp, VerticesLerpRightDown     },
/*10001*/  new Vector3[] { VerticesLerpLeftDown, VerticesUpLeft, VerticesLerpUpRight, VerticesLerpLeftDown, VerticesLerpUpRight, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpUpRight,  VerticesLerpDwonRight, VerticesLerpDwonRight, VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightUp, VerticesLerpRightDown },
/*10010*/  new Vector3[] { VerticesLerpLeftDown, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpLeftDown, VerticesLerpUpLeft, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpUpLeft, VerticesLerpDwonRight, VerticesLerpDwonRight, VerticesLerpUpLeft, VerticesLerpRightDown, VerticesLerpUpLeft, VerticesUpRight, VerticesLerpRightDown },
/*10011*/  new Vector3[] { VerticesLerpLeftDown, VerticesUpLeft, VerticesUpRight, VerticesLerpLeftDown, VerticesUpRight, VerticesLerpRightDown, VerticesLerpLeftDown, VerticesLerpRightDown, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpRightDown, VerticesLerpDwonRight },
/*10100*/  new Vector3[] { VerticesLerpLeftDown, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpLeftDown, VerticesLerpUpLeft, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesLerpDwonLeft, VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpDwonLeft, VerticesLerpRightUp, VerticesDownRight },
/*10101*/  new Vector3[] { VerticesLerpLeftDown, VerticesUpLeft, VerticesLerpUpRight, VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpLeftDown, VerticesLerpLeftDown, VerticesLerpRightUp, VerticesLerpDwonLeft, VerticesLerpDwonLeft, VerticesLerpRightUp, VerticesDownRight },
/*10110*/  new Vector3[] { VerticesLerpDwonLeft, VerticesLerpUpLeft, VerticesDownRight, VerticesDownRight, VerticesLerpUpLeft, VerticesUpRight,VerticesLerpLeftDown, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpLeftDown, VerticesLerpUpLeft, VerticesLerpDwonLeft },
/*10111*/  new Vector3[] { VerticesLerpDwonLeft, VerticesLerpLeftDown, VerticesDownRight, VerticesDownRight, VerticesLerpLeftDown, VerticesUpLeft, VerticesUpLeft, VerticesUpRight, VerticesDownRight },
/*11000*/  new Vector3[] { VerticesDownLeft, VerticesLerpLeftUp, VerticesLerpDwonRight, VerticesLerpDwonRight, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesLerpDwonRight, VerticesLerpDwonRight, VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightUp, VerticesLerpRightDown  },
/*11001*/  new Vector3[] { VerticesDownLeft, VerticesUpLeft, VerticesLerpUpRight, VerticesLerpUpRight, VerticesLerpDwonRight, VerticesDownLeft, VerticesLerpDwonRight , VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpDwonRight, VerticesLerpRightUp, VerticesLerpRightDown },
/*11010*/  new Vector3[] { VerticesDownLeft, VerticesLerpLeftUp, VerticesLerpDwonRight, VerticesLerpDwonRight, VerticesLerpLeftUp, VerticesLerpUpLeft,VerticesLerpUpLeft, VerticesLerpRightDown, VerticesLerpDwonRight, VerticesLerpUpLeft, VerticesUpRight, VerticesLerpRightDown },
/*11011*/  new Vector3[] { VerticesDownLeft, VerticesUpLeft, VerticesUpRight, VerticesUpRight, VerticesLerpDwonRight, VerticesDownLeft, VerticesLerpDwonRight, VerticesUpRight, VerticesLerpRightDown },
/*11100*/  new Vector3[] { VerticesDownLeft, VerticesLerpLeftUp, VerticesDownRight, VerticesDownRight, VerticesLerpLeftUp, VerticesLerpRightUp, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpUpRight, VerticesLerpUpRight, VerticesLerpRightUp, VerticesLerpLeftUp },
/*11101*/  new Vector3[] { VerticesDownLeft, VerticesUpLeft, VerticesDownRight, VerticesDownRight, VerticesUpLeft, VerticesLerpUpRight, VerticesLerpUpRight, VerticesLerpRightUp, VerticesDownRight },
/*11110*/  new Vector3[] { VerticesDownLeft, VerticesLerpLeftUp, VerticesLerpUpLeft, VerticesLerpUpLeft, VerticesUpRight, VerticesDownLeft, VerticesDownLeft,VerticesUpRight, VerticesDownRight  },
/*11111*/  new Vector3[] { VerticesDownLeft, VerticesUpLeft, VerticesDownRight, VerticesDownRight, VerticesUpLeft, VerticesUpRight },

        };
    }
}
