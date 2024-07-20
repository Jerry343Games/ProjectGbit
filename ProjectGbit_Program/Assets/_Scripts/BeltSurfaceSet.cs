using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class BeltSurfaceSet : MonoBehaviour
{

    public Material defaultMat;
    public Material highLightMat;

    public Renderer beltRender;
    public Renderer shelftRender;


    public PlayableDirector playableDirector;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 选中
    /// </summary>
    public void BeSelected()
    {
        beltRender.material = highLightMat;
        shelftRender.material = highLightMat;
    }

    /// <summary>
    /// 退出选中
    /// </summary>
    public void ExitSelected()
    {
        beltRender.material=defaultMat;
        shelftRender.material = defaultMat;
    }

    public void SwitchDir()
    {
        
    }

    public void OffMove()
    {
        playableDirector.enabled = false;
    }
    
    
}
