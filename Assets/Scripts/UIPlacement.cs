using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlacement : MonoBehaviour
{
    [SerializeField]
    private Button MastButton;
    [SerializeField]
    private Button HemocytoblastButton;
    [SerializeField]
    private Button DendriticButton;
    [SerializeField]
    private Button BButton;
    [SerializeField]
    private Button TButton;
    [SerializeField]
    private Button NKButton;
    [SerializeField]
    private Button NeutrophilButton;
    [SerializeField]
    private Button MacrophageButton;

    [SerializeField]
    private Button CancelButton;

    [SerializeField]
    private MapManager manager;


    // Start is called before the first frame update
    void Start()
    {
        MastButton.onClick.AddListener(delegate { Selection(6); });
        HemocytoblastButton.onClick.AddListener(delegate { Selection(5); });
        DendriticButton.onClick.AddListener(delegate { Selection(4); });
        BButton.onClick.AddListener(delegate { Selection(1); });
        TButton.onClick.AddListener(delegate { Selection(2); });
        NKButton.onClick.AddListener(delegate { Selection(3); });
        NeutrophilButton.onClick.AddListener(delegate { Selection(-1); });
        MacrophageButton.onClick.AddListener(delegate { Selection(-2); });

    }

    void Selection(int selection)
    {
        CancelButton.onClick.AddListener(delegate { manager.CancelPlace(); });
        manager.RequestPlace(selection);
    }

}
