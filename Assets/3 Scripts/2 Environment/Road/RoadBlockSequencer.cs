using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockSequencer : RoadBlockSpawner
{
    private int currentIndex = 0;

    protected override RoadBlock GetNextRoadBlock()
    {
        if (currentIndex == roadBlockPrefabs.Count) return null;

        RoadBlock roadBlock = roadBlockPrefabs[currentIndex];
        currentIndex++;

        return roadBlock;
    }
}
