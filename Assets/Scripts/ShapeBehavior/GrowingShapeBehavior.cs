using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingShapeBehavior : ShapeBehavior
{
    Vector3 originalScalel;
    float duration;

    public void Initialize(Shape shape, float duration)
    {
        originalScalel = shape.transform.localScale;
        this.duration = duration;
        shape.transform.localScale = Vector3.zero;
    }
    public override ShapeBehaviorType BehaviorType
    {
        get
        {
            return ShapeBehaviorType.Growing;
        }
    }

    public override bool GameUpdate(Shape shape)
    {
        if (shape.Age < duration)
        {
            float s = shape.Age / duration;
            shape.transform.localScale = s * originalScalel;
            return true;
        }
        shape.transform.localScale = originalScalel;
        return false;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(originalScalel);
        writer.Write(duration);
    }
    public override void Load(GameDataReader reader)
    {
        originalScalel = reader.ReadVector3();
        duration = reader.ReadFloat();
    }

    public override void Recycle()
    {
		ShapeBehaviorPool<GrowingShapeBehavior>.Reclaim(this);
    }

}
