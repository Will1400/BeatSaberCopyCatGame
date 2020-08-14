﻿using BeatGame.Data;
using BeatGame.Logic.Managers;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ObstacleSpawningSystem : SystemBase
{
    public NativeList<ObstacleData> obstacles;

    // Needs to be here to run the system
    EntityQuery defaultQuery;

    protected override void OnCreate()
    {
        obstacles = new NativeList<ObstacleData>(Allocator.Persistent);

        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });
    }

    protected override void OnDestroy()
    {
        obstacles.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
        {
            SpawnNeededNotes();
        }
    }

    void SpawnNeededNotes()
    {
        if (obstacles.IsCreated == false)
            return;

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        var job = new SpawnJob
        {
            CommandBuffer = commandBuffer,
            Obstacles = obstacles,
            CurrentBeat = GameManager.Instance.CurrentBeat,
            LastBeat = GameManager.Instance.LastBeat,
            HalfJumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration,
            JumpDistance = CurrentSongDataManager.Instance.SongSpawningInfo.JumpDistance,
            Entity = EntityPrefabManager.Instance.GetEntityPrefab("Wall"),
        };
        job.Schedule().Complete();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }

    [BurstCompile]
    struct SpawnJob : IJob
    {
        public EntityCommandBuffer CommandBuffer;
        public NativeList<ObstacleData> Obstacles;
        public double HalfJumpDuration;
        public double CurrentBeat;
        public double LastBeat;
        public float JumpDistance;
        public Entity Entity;

        public void Execute()
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                var obstacle = Obstacles[i];

                if (obstacle.Time - HalfJumpDuration <= CurrentBeat && obstacle.Time - HalfJumpDuration >= LastBeat)
                {
                    var entity = CommandBuffer.Instantiate(Entity);
                    CommandBuffer.RemoveComponent<Prefab>(entity);
                    float4x4 scale = new float4x4
                    {
                        c0 = new float4(obstacle.TransformData.Scale.x, 0, 0, 0),
                        c1 = new float4(0, obstacle.TransformData.Scale.y, 0, 0),
                        c2 = new float4(0, 0, obstacle.TransformData.Scale.z, 0),
                        c3 = new float4(0, 0, 0, 1)
                    };

                    CommandBuffer.SetComponent(entity, new DestroyOnBeat { Beat = (float)CurrentBeat });

                    CommandBuffer.SetComponent(entity, new Translation { Value = obstacle.TransformData.Position + new float3(0, 0, JumpDistance) });

                    CommandBuffer.SetComponent(entity, new CompositeScale
                    {
                        Value = scale
                    });

                    CommandBuffer.SetComponent(entity, new Rotation { Value = quaternion.Euler(obstacle.TransformData.LocalRotation) });
                }

            }
        }
    }
}
