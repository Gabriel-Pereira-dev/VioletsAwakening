using System.Collections;
using System.Collections.Generic;
using Projectiles;
using StateMachineNamespace;
using UnityEngine;

namespace Behaviors.LichBoss.States
{
    public class AttackSuper : State
    {
        private LichBossController controller;
        private LichBossHelper helper;

        private float endAttackCooldown;
        public AttackSuper(LichBossController controller) : base("AttackSuper")
        {
            this.controller = controller;
            helper = this.controller.helper;
        }

        public override void Enter()
        {
            base.Enter();

            // Set variables
            endAttackCooldown = controller.attackSuperDuration;
            controller.thisAnimator.SetTrigger("tAttackSuper");
            float firstAttackDelay = controller.attackSuperDelay;
            float delayStep = (controller.attackSuperDuration - firstAttackDelay) / (controller.attackSuperCount);
            Debug.Log("DelayStep: "+ delayStep);
            for (int i = 0; i <= controller.attackSuperCount - 1; i++)
            {
                var delay = controller.attackSuperDelay + delayStep * i;
                
                Debug.Log(i + " - " + delay);
                // Schedule AttackSuper
                helper.StartStateCoroutine(ScheduleAttackSuper(delay));
            }
            
            
        }

        public override void Exit()
        {
            base.Exit();
            helper.ClearStateCoroutines();
        }

        public override void Update()
        {
            base.Update();

            if ((endAttackCooldown -= Time.deltaTime) <= 0)
            {
                controller.stateMachine.ChangeState(controller.idleState);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        private IEnumerator ScheduleAttackSuper(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            Debug.Log("Atacou SUPER");
            
            //Get prefab
            List<GameObject> prefabs = new List<GameObject>(){controller.fireballPrefab,controller.energyballPrefab};
            var prefab =prefabs[Random.Range(0, prefabs.Count)];
            
            //Create Object
            var spawnTransform = controller.staffTop;
            var rotation = spawnTransform.rotation;
            var position = spawnTransform.position;
            var projectile = Object.Instantiate(prefab, position,
                rotation);
            
            // Populate Projectile Collision
            var projectileCollision = projectile.GetComponent<ProjectileCollision>();
            projectileCollision.attacker = controller.gameObject;
            projectileCollision.damage = controller.attackDamage;
            
            //Get Stuff
            var aimOffset = controller.aimOffset;
            var player = GameManager.Instance.player;
            var projectileRigidbody = projectile.GetComponent<Rigidbody>();
            
            //Apply Impulse
            var vectorToPlayer = ((player.transform.position + aimOffset) - position).normalized;
            var forceVector = rotation * Vector3.forward;
            forceVector = new Vector3(forceVector.x, vectorToPlayer.y, forceVector.z);
            forceVector *= controller.attackSuperImpulse;
            projectileRigidbody.AddForce(forceVector,ForceMode.Impulse);

            
            
            Object.Destroy(projectile,30f);
        }
    }
}
