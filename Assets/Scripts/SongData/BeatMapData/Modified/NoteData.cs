﻿using Unity.Entities;
using Unity.Mathematics;

public struct NoteData
{
    public double Time { get; set; }

    public TransformData TransformData { get; set; }

    public int Type { get; set; }

    public int CutDirection { get; set; }

    public float3 Color { get; set; }
}
