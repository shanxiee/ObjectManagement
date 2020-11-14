using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataWriter {
    BinaryWriter writer;

    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Writer(float value)
    {
        writer.Write(value);
    }

    public void Writer(int value)
    {
        writer.Write(value);
    }

    public void Writer(Quaternion value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
        writer.Write(value.w);
    }

    public void Writer(Vector3 value)
    {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public void Writer(Color value)
    {
        writer.Write(value.r);
        writer.Write(value.g);
        writer.Write(value.b);
        writer.Write(value.a);
    }

}
