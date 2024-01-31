using UnityEngine;
using SQLite4Unity3d;

public class AllTableScript : MonoBehaviour
{
    // All Tables In This Script
    // Mention the class in OnCreateNewTable() Function of DataBasePanelScript Script;
}

public class HomePageTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string DailyBonusAmt { get; set; }
    public string HealthActivityPerct { get; set; }
    public string HappyActivityPerct { get; set; }
    public string HealthActivityCost { get; set; }
    public string HappyActivityCost { get; set; }
    public string ElectionNames { get; set; }
    public string ElectionAgeReq { get; set; }
    public string ElectionResptReq { get; set; }
    public string ElectionCampFund { get; set; }
    public string ElectionReward { get; set; }
    public string ElectedSalary { get; set; }
}


public class ResearchTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string rsrchName { get; set; }
    public string rsrchImgPath { get; set; }
    public string rsrchDscpt { get; set; }
    public string rsrchCostL1 { get; set; }
    public string rsrchCostL2 { get; set; }
    public string rsrchCostL3 { get; set; }
    public string rsrchReqResptL1 { get; set; }
    public string rsrchReqResptL2 { get; set; }
    public string rsrchReqResptL3 { get; set; }
}


public class BusinessTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string BizNameL1 { get; set; }
    public string BizNameL2 { get; set; }
    public string BizNameL3 { get; set; }
    public string BizImgPathL1 { get; set; }
    public string BizImgPathL2 { get; set; }
    public string BizImgPathL3 { get; set; }
    public string BizCostL1 { get; set; }
    public string BizCostL2 { get; set; }
    public string BizCostL3 { get; set; }
    public string BizRevenueL1 { get; set; }
    public string BizRevenueL2 { get; set; }
    public string BizRevenueL3 { get; set; }
    public string BizResaleL1 { get; set; }
    public string BizResaleL2 { get; set; }
    public string BizResaleL3 { get; set; }
}


public class LifeStyleTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string LsRespect { get; set; }
    public string LsPartName { get; set; }
    public string LsHomeName { get; set; }
    public string LsTransName { get; set; }
    public string LsPartImgPath { get; set; }
    public string LsHomeImgPath { get; set; }
    public string LsTransImgPath { get; set; }
    public string LsPartExpense { get; set; }
    public string LsHomeExpense { get; set; }
    public string LsTransExpense { get; set; }
    public string LsHomeCost { get; set; }
    public string LsTransCost { get; set; }
}


public class TradeTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string TradeName { get; set; }
    public string TradePrice { get; set; }

}


public class GeneralTable
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Property { get; set; }
    public string GameState { get; set; }
}

