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

    public GameObject beltMesh;
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

    /// <summary>
    /// 反转
    /// </summary>
    public void SwitchDir()
    {
        Vector3 localScale = beltMesh.transform.localScale;
        localScale.x = -localScale.x;
        // 重新设置localScale
        beltMesh.transform.localScale = localScale;
    }

    
    /// <summary>
    /// 开关
    /// </summary>
    public void OnOffMove()
    {
        playableDirector.enabled = false;
    }
    
    
}
