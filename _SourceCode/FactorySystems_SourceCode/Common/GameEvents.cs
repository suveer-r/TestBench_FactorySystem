public static class GameEvents
{
    public delegate void FactoryClicked(Factory factory);
    public static FactoryClicked OnFactoryClicked;

    public delegate bool CreateOrUpgradeFactory(Factory factory);
    public static CreateOrUpgradeFactory OnFactoryCreatedOrUpgraded;
}