﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace BeatGame.Utility.Physics
{
    public class ECSRaycast : MonoBehaviour
    {
        public static RaycastHit Raycast(float3 fromPosition, float3 toPosition, uint layerMask)
        {
            var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();

            var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            RaycastInput raycastInput = new RaycastInput
            {
                Start = fromPosition,
                End = toPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = layerMask,
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(raycastInput, out RaycastHit hit))
            {
                return hit;
            }

            return hit;
        }

        public static RaycastHit Raycast(float3 fromPosition, float3 toPosition)
        {
            return Raycast(fromPosition, toPosition, ~0u);
        }

        public static void RaycastAll(float3 fromPosition, float3 toPosition, uint layerMask, ref NativeList<RaycastHit> raycastHits)
        {
            var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();

            var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            RaycastInput raycastInput = new RaycastInput
            {
                Start = fromPosition,
                End = toPosition,
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = layerMask,
                    GroupIndex = 0
                }
            };

            collisionWorld.CastRay(raycastInput, ref raycastHits);
        }

        public static void RaycastAll(float3 fromPosition, float3 toPosition, ref NativeList<RaycastHit> raycastHits)
        {
            RaycastAll(fromPosition, toPosition, ~0u, ref raycastHits);
        }
    }
}