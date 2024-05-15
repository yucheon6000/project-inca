using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject targetCar;

    [SerializeField]
    private List<RoadBlock> roadBlockPrefabs = new List<RoadBlock>();

    [SerializeField]
    private int trigerGap = 490;     // trigerGap is gap between end line and limit line;

    [Header("Preset")]
    [SerializeField]
    private RoadBlock presetedRoadBlock;

    private RoadBlock currentRoadBlock;
    private RoadBlock previousRoadBlock;

    private Vector3Int roadBlockMap = new Vector3Int();

    private Vector3 RoadBlockSize => RoadBlock.road_block_size;

    private void Awake()
    {
        if (!presetedRoadBlock)
            Debug.LogError("Please input your preseted RoadBlock.");

        currentRoadBlock = presetedRoadBlock;
    }

    private void Update()
    {
        if (currentRoadBlock.OnTriggerEnterCar(targetCar.transform.position, trigerGap))
        {
            SpawnRoadBlock();
        }
    }

    private void SpawnRoadBlock()
    {
        if (previousRoadBlock)
            previousRoadBlock.gameObject.SetActive(false);

        RoadBlock prefab = roadBlockPrefabs[Random.Range(0, roadBlockPrefabs.Count)];

        Vector3 pos = currentRoadBlock.transform.position;

        if (currentRoadBlock.CurrentDirection == RoadBlockDirection.North && currentRoadBlock.NextDirection == RoadBlockDirection.North) pos.z += RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.North && currentRoadBlock.NextDirection == RoadBlockDirection.East) pos.x += RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.North && currentRoadBlock.NextDirection == RoadBlockDirection.South) pos.z -= RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.North && currentRoadBlock.NextDirection == RoadBlockDirection.West) pos.x -= RoadBlockSize.x;

        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.South && currentRoadBlock.NextDirection == RoadBlockDirection.North) pos.z -= RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.South && currentRoadBlock.NextDirection == RoadBlockDirection.East) pos.x -= RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.South && currentRoadBlock.NextDirection == RoadBlockDirection.South) pos.z += RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.South && currentRoadBlock.NextDirection == RoadBlockDirection.West) pos.x += RoadBlockSize.x;

        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.East && currentRoadBlock.NextDirection == RoadBlockDirection.North) pos.x += RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.East && currentRoadBlock.NextDirection == RoadBlockDirection.East) pos.z -= RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.East && currentRoadBlock.NextDirection == RoadBlockDirection.South) pos.x -= RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.East && currentRoadBlock.NextDirection == RoadBlockDirection.West) pos.z += RoadBlockSize.z;

        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.West && currentRoadBlock.NextDirection == RoadBlockDirection.North) pos.x -= RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.West && currentRoadBlock.NextDirection == RoadBlockDirection.East) pos.z += RoadBlockSize.z;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.West && currentRoadBlock.NextDirection == RoadBlockDirection.South) pos.x += RoadBlockSize.x;
        else if (currentRoadBlock.CurrentDirection == RoadBlockDirection.West && currentRoadBlock.NextDirection == RoadBlockDirection.West) pos.z -= RoadBlockSize.z;

        RoadBlockDirection nextRoadBlockDirection = RoadBlockDirection.North;

        int angle = (int)currentRoadBlock.CurrentDirection + (int)currentRoadBlock.NextDirection;
        angle = (int)(angle % 360);

        if (angle == 0) nextRoadBlockDirection = RoadBlockDirection.North;
        if (angle == 90) nextRoadBlockDirection = RoadBlockDirection.East;
        if (angle == 180) nextRoadBlockDirection = RoadBlockDirection.South;
        if (angle == 270) nextRoadBlockDirection = RoadBlockDirection.West;

        RoadBlock nextRoadBlock = Instantiate<RoadBlock>(prefab, pos, Quaternion.identity);
        nextRoadBlock.Setup(nextRoadBlockDirection);

        currentRoadBlock.SetNextRoadBlock(nextRoadBlock);
        previousRoadBlock = currentRoadBlock;
        currentRoadBlock = nextRoadBlock;
    }
}
