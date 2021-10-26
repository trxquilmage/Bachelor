using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordInfo
{
    // Lists all Info needed to Work with the words
    #region WordTags
    public enum WordTags
    {
        Location, Item, Name, None, All
    }
    #region Location 
    public enum Location
    {
        Position, GoodThing, BadThing
    }
    #region Position
    public enum Position
    {
        east, west, north, south
    }
    #endregion
    #endregion
    #region Item
    #endregion
    #region Name
    #endregion
    #endregion

    public enum Origin
    {
        Dialogue, WordCase
    }


}
