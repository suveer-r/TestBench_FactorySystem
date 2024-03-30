using UnityEngine;

[CreateAssetMenu(fileName = "Factory_Config_Level_", menuName = "FactorySystem/FactoryConfig", order = 0)]
public class FactoryConfig : ScriptableObject
{
    public int RateOfGemProduction;
    public int CostToBuildFactory;
}
