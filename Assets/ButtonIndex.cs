using UnityEngine;

public class ButtonIndex : MonoBehaviour
{
    public static ButtonIndex instance {  get; private set; }


    public int index;
    private void Awake()
    {
        if (instance == null) 
            {
                instance = this;
            }
    }

   
       

}
