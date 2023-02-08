using Unity.Netcode;

public struct RoundSpec : INetworkSerializeByMemcpy
{
    public int roundNumber;
    public bool isLast;

    public RoundSpec(bool isLast, int roundNumber)
    {
        this.isLast = isLast;
        this.roundNumber = roundNumber;
    }
}