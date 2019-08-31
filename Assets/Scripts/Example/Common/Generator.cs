
using UnityEngine;
using UnityEditor;

public class MapGenerator : ScriptableWizard
{
    public GameObject floor;
    public GameObject block;

    public int width;
    public int height;

    [MenuItem("GameObject/Create Map Wizard")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<MapGenerator>("Create Map", "Create");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        GameObject parent = new GameObject("SquareHolder");
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Random.value < .85f)
                {
                    GameObject.Instantiate(floor, parent.transform).transform.position = new Vector2(i, j);
                }
                else
                {
                    GameObject.Instantiate(block, parent.transform).transform.position = new Vector2(i, j);
                }
            }
        }
    }
}
