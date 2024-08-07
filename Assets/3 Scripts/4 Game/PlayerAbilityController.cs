using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    /*
    [SerializeField]
    private List<PlayerAbilityQueue> abilityQueues;
    [SerializeField]
    private List<PlayerAbilityQueue> abilityQueuesTrash = new List<PlayerAbilityQueue>();
    private Dictionary<Ability, PlayerAbilityQueue> seletedAbilities = new Dictionary<Ability, PlayerAbilityQueue>();

    [SerializeField]
    private int displayCount = 3;
    private bool display = false;
    private float displayTimer = 0;

    [Header("[UI]")]
    [SerializeField]
    private GameObject canvasAbility;

    [System.Serializable]
    public class PlayerAbilityQueue
    {
        public string title;
        public List<Ability> abilities;
        public bool isRemovable = true;

        public bool IsUsable()
        {
            return abilities.Count > 0;
        }

        public Ability GetFirstAbility()
        {
            Ability ability = abilities[0];
            return ability;
        }

        public void SelectItem(Ability ability)
        {
            if (!isRemovable) return;
            abilities.Remove(ability);
        }

        public void StopDisplayAll()
        {
            foreach (Ability ability in abilities)
            {
                ability.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        MonsterSpawnController.onClearWaveEvent.AddListener(StartDisplay);

        canvasAbility.gameObject.SetActive(false);

        foreach (PlayerAbilityQueue queue in abilityQueues)
        {
            queue.StopDisplayAll();
        }
    }

    private void Update()
    {
        if (!display) return;

        displayTimer -= Time.deltaTime;

        if (displayTimer <= 0)
            StopDisplay();
    }

    public void StartDisplay(float displayTime)
    {
        displayTimer = displayTime;
        StartDisplayAbility();
    }

    public void OnClickAbility(Ability ability)
    {
        PlayerAbilityQueue queue = seletedAbilities[ability];
        queue.SelectItem(ability);

        StopDisplay();

        MonsterSpawnController.SkipWaveDeltaTime();
    }

    public void StopDisplay()
    {
        displayTimer = 0;
        StopDisplayAbility();
    }

    private void StartDisplayAbility()
    {
        if (display) return;

        display = true;
        canvasAbility.gameObject.SetActive(true);
        seletedAbilities.Clear();

        List<int> indexList = new List<int>();
        for (int i = 0; i < abilityQueues.Count; i++)
        {
            PlayerAbilityQueue queue = abilityQueues[i];

            if (!queue.IsUsable())
            {
                abilityQueues.Remove(queue);
                abilityQueuesTrash.Add(queue);
                continue;
            }

            indexList.Add(i);
        }

        int selectCount = 0;
        while (indexList.Count > 0)
        {
            int idx = indexList[Random.Range(0, indexList.Count)];
            PlayerAbilityQueue queue = abilityQueues[idx];

            Ability ability = queue.GetFirstAbility();
            ability.onClickAbilityEvent.AddListener(OnClickAbility);
            ability.gameObject.SetActive(true);
            seletedAbilities.Add(ability, queue);
            selectCount++;

            indexList.Remove(idx);

            if (selectCount >= displayCount)
                break;
        }
    }

    private void StopDisplayAbility()
    {
        print("stop");

        if (!display) return;

        display = false;
        canvasAbility.gameObject.SetActive(false);

        foreach (Ability ability in seletedAbilities.Keys)
        {
            ability.onClickAbilityEvent.RemoveListener(OnClickAbility);
            ability.gameObject.SetActive(false);
        }

        seletedAbilities.Clear();
    }
    */
}