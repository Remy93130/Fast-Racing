using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownTrack : MonoBehaviour
{

   /* private string selectedChoice;
    public string SelectedChoice { get => selectedChoice; }*/


    // Start is called before the first frame update
    void Start()
    {
        var dropdown  = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        Object[] items = Resources.LoadAll("Prefabs/Tracks", typeof(GameObject));

        foreach(var item in items)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = item.name });
        }

      //  DropdownItemSelected(dropdown);
        //dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }

 /*   void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        selectedChoice = dropdown.options[index].text;
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
