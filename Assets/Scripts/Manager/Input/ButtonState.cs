using System;
using UnityEngine;

public struct ButtonState
{
    public bool Held;
    public uint DownFrame;
    public uint UpFrame;
    public float HoldTime;
    public double LastDownTime;
    public double LastUpTime;
    public bool ConsumedDown;
    public double BufferedUntil;
    public bool WasBufferedConsume;

    public void SetDown(float bufferDuration)
    {
        Held = true;
        DownFrame = (uint)Time.frameCount;
        HoldTime = 0f;
        LastDownTime = Time.unscaledTimeAsDouble;
        ConsumedDown = false;
        WasBufferedConsume = false;

        BufferedUntil = bufferDuration > 0f ? 
            Time.unscaledTimeAsDouble + bufferDuration : 
            0;
    }

    public void SetUp()
    {
        Held = false;
        UpFrame = (uint)Time.frameCount;
        LastUpTime = Time.unscaledTimeAsDouble;
    }

    public void TickHold()
    {
        if (Held) HoldTime += Time.unscaledDeltaTime;
    }

    public bool IsDownThisFrame() => DownFrame == (uint)Time.frameCount;
    public bool IsUpThisFrame()   => UpFrame == (uint)Time.frameCount;

    public bool IsBuffered() => Time.unscaledTimeAsDouble <= BufferedUntil && !ConsumedDown;

}
