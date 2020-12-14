using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GailType
{
    JumpGail
}

public class Gail : MonoBehaviour
{
    [SerializeField] GailType gailType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GailType GetGailType()
    {
        return gailType;
    }

}
