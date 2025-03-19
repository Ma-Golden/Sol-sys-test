using UnityEngine;


public abstract class PostProcEffect : ScriptableObject
{
    protected Material material;

    public virtual Material GetMaterial()
    {
        return null; 
    }

    public virtual void ReleaseBuffers()
    {
    }

    public abstract void Render(RenderTexture source, RenderTexture destination);

    public virtual void Awake_ScriptableObject()
    {

    }

}