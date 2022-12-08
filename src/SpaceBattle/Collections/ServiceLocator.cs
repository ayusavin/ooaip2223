namespace SpaceBattle.Collections;
using SpaceBattle.Base;

// Description:
//      Implementation of global service locator
public static class ServiceLocator
{
    static IDictionary<string, IStrategy> services = new Dictionary<string, IStrategy>();

    // Description:
    //      Locates the instance with provided service key, call it and
    //      returns result, converted to ReturnType
    //  Params:
    //      serviceKey: service name, uniquely identifying a specific service
    //  Return:
    //      Returns a generic casted instant of a service with the given serviceKey
    public static ReturnType Locate<ReturnType>(string serviceKey, params object[] argv)
    {
        return (ReturnType)services[serviceKey].Run();
    }

    // Description:
    //      Register new service with provided service key
    //  Params:
    //      instance: the service instance which can later be called with 
    //                Locate method
    //      serviceKey: service name, uniquely identifying a specific service
    public static void Register(string serviceKey, IStrategy service)
    {
        services.Add(serviceKey, service);
    }
}