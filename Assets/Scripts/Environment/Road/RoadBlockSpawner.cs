using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;

    [SerializeField]
    private List<RoadBlock> roadBlockPrefabs = new List<RoadBlock>();

    [SerializeField]
    private int trigerGap = 90;     // trigerGap is gap between end line and limit line;

    private int currentIndex = 0;
    private int currentLimit = 0;

    [Header("Preset")]
    [SerializeField]
    private RoadBlock presetedRoadBlock;

    private RoadBlock currentRoadBlock;
    private RoadBlock previousRoadBlock;

    private void Awake()
    {
        if (!presetedRoadBlock)
            Debug.LogError("Please input your preseted RoadBlock.");

        currentRoadBlock = presetedRoadBlock;

        currentLimit = -trigerGap;
    }

    private void Update()
    {
        if (targetObject.transform.position.z >= currentLimit)
            SpawnRoadBlock();
    }

    private void SpawnRoadBlock()
    {
        if (previousRoadBlock)
            previousRoadBlock.gameObject.SetActive(false);

        currentIndex++;
        currentLimit = 100 * currentIndex - trigerGap;

        RoadBlock prefab = roadBlockPrefabs[Random.Range(0, roadBlockPrefabs.Count)];
        Vector3 pos = new Vector3(0, 0, 100 * currentIndex);
        RoadBlock nextRoadBlock = Instantiate<RoadBlock>(prefab, pos, Quaternion.identity);

        currentRoadBlock.SetNextRoadBlock(nextRoadBlock);
        previousRoadBlock = currentRoadBlock;
        currentRoadBlock = nextRoadBlock;
    }
}
