using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.EventSystems.EventTrigger;

namespace XyphireLethalWonderworld
{
    internal class DogToy : ThrowableItem
    {
        bool isPlaying = false;
        float timer = 0f;
        public float maxTimer = 10f;
        bool changedMoon = false;
        SpawnableEnemyWithRarity? enemySaved;
        int enemiesSpawned = 0;
        public int enemiesToSpawn = 2;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            transform.gameObject.GetComponent<Animator>().SetTrigger("Squish");
            base.ItemActivate(used, buttonDown);
            isPlaying = true;
        }

        public override void Start()
        {
            base.Start();
            if (IsServer && !StartOfRound.Instance.inShipPhase)
            {
                foreach (var enemy in RoundManager.Instance.currentLevel.OutsideEnemies)
                {
                    if (enemy.enemyType.enemyName == "MouthDog")
                    {
                        enemySaved = enemy;
                        enemy.enemyType.MaxCount += 3;
                        RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount *= 2;
                        enemy.rarity *= 10;
                        changedMoon = true;
                        break;
                    }
                }
                if (enemySaved == null)
                {
                    foreach (var moon in GameObject.FindObjectOfType<Terminal>().moonsCatalogueList)
                    {
                        foreach (var enemy in moon.OutsideEnemies)
                        {
                            if (enemy.enemyType.enemyName == "MouthDog")
                            {
                                enemySaved = enemy;
                                break;
                            }
                        }
                        if (enemySaved != null)
                        {
                            break;
                        }
                    }
                }

            }
        }

        public static NetworkObjectReference Spawn(SpawnableEnemyWithRarity enemy, Vector3 position, float yRot = 0f)
        {
            GameObject gameObject = Instantiate(enemy.enemyType.enemyPrefab, position, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
            RoundManager.Instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
            return new NetworkObjectReference(gameObject);
        }

        public override void Update()
        {
            base.Update();
            if(isPlaying)
            {
                timer += Time.deltaTime;
                if(timer > maxTimer || playerHeldBy != null)
                {
                    isPlaying = false;
                    timer = 0f;
                    return;
                }

                RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, 1f, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
            }

            if (IsServer && enemySaved != null && TimeOfDay.Instance.currentDayTime > 780 && UnityEngine.Random.Range(0f,1f) > 0.99 && enemiesSpawned < enemiesToSpawn)
            {
                Spawn(enemySaved, RoundManager.Instance.outsideAINodes[UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length-1)].transform.position);
                enemiesSpawned++;
            }

            if (IsServer && enemySaved != null && changedMoon && StartOfRound.Instance.shipIsLeaving)
            {
                enemySaved.enemyType.MaxCount -= 3;
                RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount /= 2;
                enemySaved.rarity /= 10;
                changedMoon = false;
            }
        }

        public override void OnHitGround()
        {
            transform.gameObject.GetComponent<Animator>().SetTrigger("Squish");
            base.OnHitGround();
            base.ItemActivate(true, true);
            isPlaying = true;
        }

        public override void EquipItem()
        {
            base.EquipItem();
            playerHeldBy.equippedUsableItemQE = true;
        }

        public override void ItemInteractLeftRight(bool right)
        {
            base.ItemInteractLeftRight(right);
            if (!right && IsOwner)
            {
                base.DiscardHeld();
            }
        }

        public override void PocketItem()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.equippedUsableItemQE = false;
            }
            base.PocketItem();
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.equippedUsableItemQE = false;
            }
            base.DiscardItem();
        }

        public override void OnNetworkDespawn()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.equippedUsableItemQE = false;
            }
            base.OnNetworkDespawn();
        }
    }
}
