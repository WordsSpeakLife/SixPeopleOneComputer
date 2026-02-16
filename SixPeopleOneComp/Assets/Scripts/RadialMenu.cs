using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class RadialMenu : MonoBehaviour
{
    [SerializeField] GameObject EntryPrefab;
    List<RadialMenuEntry> Entries;
    [SerializeField] List<Sprite> Icons;
    [Range(1,8)][SerializeField] int entriesCount;

    [SerializeField] float Radius = 300f;
    void Start()
    {
        Entries = new List<RadialMenuEntry>();
    }

    void AddEntry(string pLabel, Sprite pIcon)
    {
        GameObject entry = Instantiate(EntryPrefab, transform);

        RadialMenuEntry radialEntry = entry.GetComponent<RadialMenuEntry>();
        radialEntry.SetLabel(pLabel);
        radialEntry.SetIcon(pIcon);

        Entries.Add(radialEntry);
    }

    public void Open()
    {
        if(GameManager.instance.menuActive == GameManager.instance.weaponRadialMenu)
        {
            updateEntries();
            Rearrange();
        }
        //else
        //{
        //    Close();
        //}
    }


    public void updateEntries()
    {
        //if (Entries.Count < GameManager.instance.player.GetComponent<PlayerController>().weaponList.Count)
        //{
        //    Entries.Clear();
            for (int i = 0; i < entriesCount; i++)
            {
                AddEntry("Button" + i.ToString(), Icons[i]);
            }
        //}
        
    }

    //public void Close()
    //{
    //    Entries.RemoveRange(0, Entries.Count);
    //}

    void Rearrange()
    {
        float radiansOfSep = (Mathf.PI * 2) / entriesCount;
        for(int i = 0; i < entriesCount; i++)
        {
            float x = Mathf.Sin(radiansOfSep * i) * Radius;
            float y = Mathf.Cos(radiansOfSep * i) * Radius;

            Entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }
}
