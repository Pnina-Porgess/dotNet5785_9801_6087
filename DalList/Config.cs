
using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{

    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextCallId++; }


    internal const int startAssignmentId = 2000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextAssignmentId++;
    }


    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; 
    } = DateTime.Now;


    public static TimeSpan RiskRange { get; internal set; }
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void Reset()
    {
        nextAssignmentId = startAssignmentId;
        nextCallId = startCallId;
        Clock = DateTime.Now;
        RiskRange=TimeSpan.Zero;
    }
}

