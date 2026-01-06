using UnityEngine;

public class ZonaTrigger : MonoBehaviour
{
    [SerializeField] private int _zonaID;

    private void OnTriggerEnter(Collider other)
    {
        Pikmin pikmin = other.GetComponent<Pikmin>();
        if( pikmin != null)
        {
            pikmin.SetZona(_zonaID);
        }
    }
}
