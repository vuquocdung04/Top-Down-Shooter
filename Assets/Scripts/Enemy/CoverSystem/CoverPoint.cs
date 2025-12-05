using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public bool occupied = false; // chiem linh
    
    public void SetOccupied(bool occupied) => this.occupied =  occupied;
}