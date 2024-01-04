using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace TGB.SniperVsRunner
{
    public class FindTarget : MonoBehaviour
    {
        public GameObject targetsParent;
        private List<Transform> targets;
        private NavMeshAgent agent;
        Vector3 destination;
        bool isReady = false;

        // Start is called before the first frame update
        public void PrepareTargets()
        {
            agent = GetComponent<NavMeshAgent>();

            targets = new List<Transform>();

            foreach (Transform childCharacter in targetsParent.transform)
            {
                targets.Add(childCharacter);
                foreach (Transform childChildCharacter in childCharacter)
                {
                    targets.Add(childChildCharacter);
                }
            }

            selectNextTarget();
            isReady = true;
        }

        private void selectNextTarget()
        {
            int nextTargetIndex = (int)(targets.Count * Random.value);

            agent.SetDestination(targets[nextTargetIndex].position);

            destination = agent.destination;
        }

        // Update is called once per frame
        void Update()
        {
            if (isReady)
            { 
                if (Vector3.Distance(destination, GetComponent<Transform>().position) < 1.0f)
                {
                    selectNextTarget();
                }
            }
        }
    }
}
