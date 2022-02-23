using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ReconAndDiscovery.Triggers;

public class RectActionTrigger : ActionTrigger
{
    private CellRect rect;

    public CellRect Rect
    {
        get => rect;
        set
        {
            rect = value;
            if (Spawned)
            {
                rect.ClipInsideMap(Map);
            }
        }
    }

    public override ICollection<IntVec3> Cells => rect.Cells.ToList();
}