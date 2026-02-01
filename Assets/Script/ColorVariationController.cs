using UnityEngine;

public class ColorVariationController : MonoBehaviour
{
    public Color[] RandomColors;
    public Renderer Mesh;
    public int MaterialIndex;
    
    void Start()
    {
        Color color = RandomColors[Random.Range(0, RandomColors.Length)];
        Mesh.materials[MaterialIndex].SetColor("_Base_Color", color);
    }

}
