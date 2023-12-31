using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class VisitorTokensAndInfo
{
    long seed = -1;
    int nbBeforeExplode = -1;
    long nbSoFar = -1;
    GameObject ourGameObject;

    public long tokenId { get => seed; set => seed = value; }
    public int NbBeforeExplode { get => nbBeforeExplode; set => nbBeforeExplode = value; }
    public long NbSoFar { get => nbSoFar; set => nbSoFar = value; }
    public GameObject OurGameObject { get => ourGameObject; set => ourGameObject = value; }

    
    internal bool isReturningFromVault = false;
    internal bool isNew = false;
    internal bool refusedNewEntry = false; // Already in our box
    internal bool exploding = false;
    internal ManageToken ourManageToken;
}
