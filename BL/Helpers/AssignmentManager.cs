using DalApi;
namespace Helpers;

internal static class AssignmentManager
{
    lock (AdminManager.BlMutex) //stage 7
    {
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); 

}
