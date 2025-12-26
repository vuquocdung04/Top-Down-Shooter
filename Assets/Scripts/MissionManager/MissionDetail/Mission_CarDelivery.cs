using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Car Delivery Mission", menuName = "Missions/Car Delivery Mission")]

public class Mission_CarDelivery : Mission
{
    private bool carWasDelivered;
    public override void StartMission()
    {
        carWasDelivered = false;
        MissionObject_CarToDeliver.OnCarDelivery += CarDeliveryCompleted;
        
        Car[] cars = FindObjectsOfType<Car>();

        foreach (var car in cars)
        {
            car.AddComponent<MissionObject_CarToDeliver>();
        }
        
    }

    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }

    private void CarDeliveryCompleted()
    {
        carWasDelivered = true;
        MissionObject_CarToDeliver.OnCarDelivery -= CarDeliveryCompleted;
    }
}