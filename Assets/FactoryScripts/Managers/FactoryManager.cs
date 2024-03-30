using System.Collections.Generic;

public class FactoryManager : Singleton<FactoryManager>
{
    List<Factory> registeredFactories;

    private void Awake()
    {
        registeredFactories = new List<Factory>();
    }

    public void RegisterFactory(Factory factory)
    {
        registeredFactories.Add(factory);
    }
}