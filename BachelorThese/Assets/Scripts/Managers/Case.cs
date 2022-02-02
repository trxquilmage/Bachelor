using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Case : MonoBehaviour
{
    //inheriting classes fill these values in on InitializeValues()
    [HideInInspector] public GameObject caseObject;
    [HideInInspector] public Image journalImage;
    [HideInInspector] public GameObject listingParent;
    [HideInInspector] public TMP_Text contentCount;
    [HideInInspector] public WordInfo.Origin origin;
    [HideInInspector] public Scrollbar scrollbar;
    [HideInInspector] public int maxContentAmount;

    [HideInInspector] public GameObject bubbleReplacement;
    [HideInInspector]
    public BubbleData[] contents
    {
        get { return GetContents(); }
        set
        {
            SetContents(value);
        }
    }
    [HideInInspector] public BubbleData[] Contents;
    [HideInInspector] public bool automaticallyOpen;
    [HideInInspector] public float caseScreenHeight;
    [HideInInspector] public float bubbleScreenHeight;

    [HideInInspector] public ReferenceManager refM;
    // Start is called before the first frame update
    #region VIRTUAL
    public virtual void Initialize()
    {
        InitializeValues();
        caseScreenHeight = listingParent.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        FillArrayWithContents();
        UpdateContentCount();
    }
    public virtual void Awake()
    {

    }
    /// <summary>
    /// Called before the Functions that are called in Start()
    /// </summary>
    public virtual void InitializeValues()
    {
        refM = ReferenceManager.instance;
    }
    /// <summary>
    /// As the Classes inheriting from BubbleData need a Constructor, we cant just create an empty array
    /// </summary>
    public virtual void FillArrayWithContents()
    {
        contents = new BubbleData[maxContentAmount];
        for (int i = 0; i < maxContentAmount; i++)
        {
            contents[i] = new BubbleData();
        }
    }
    /// <summary>
    /// Open case manually through UI. will not close on AutomaticOpenCase(false)
    /// </summary>
    public virtual void ManuallyOpenCase()
    {
        caseObject.SetActive(!caseObject.activeInHierarchy);
        ReloadContents();
        ResetScrollbar();
    }
    /// <summary>
    /// Reloads the Case and respawns it's content
    /// </summary>
    public virtual void ReloadContents()
    {
        if (listingParent != null)
        {
            foreach (Bubble data in listingParent.GetComponentsInChildren<Bubble>())
            {
                if (data.gameObject != listingParent)
                {
                    Destroy(data.gameObject);
                }
            }
            SpawnContents();
            UpdateContentCount();
            StartCoroutine(RescaleScrollbar());
        }
    }
    /// <summary>
    /// Spawn all bubbles, that are in contents right now into the case
    /// </summary>
    public virtual void SpawnContents()
    {
        RearrangeContents();
        // Spawn words again
        foreach (BubbleData data in contents)
        {
            if (data.name != null)
            {
                SpawnBubbleInCase(data);
            }
        }
    }
    public virtual BubbleData[] GetContents()
    {
        return Contents;
    }
    public virtual void SetContents(BubbleData[] value)
    {
        Contents = value;
    }
    /// <summary>
    /// Updates the UI Element, that shows how many Bubbles there are in the Case right now
    /// </summary>
    public virtual void UpdateContentCount()
    {
        contentCount.text =
            GetContentCount().ToString() + "<b>/" + maxContentAmount.ToString() + "</b>";
    }
    public virtual void TryToSaveTheBubble(Bubble bubble)
    {
        BubbleData data = bubble.data;
        bubble.data.origin = origin;
        if (CheckIfCanSaveBubble(data.name, out int index, out bool bubbleIsAlreadyInList, out bool caseIsFull))
        {
            SaveBubbleDataInContents(index, data);
            CheckIfANewTagIsIncludedAndAddButton(data.tag);
        }
        else
        {
            if (bubbleIsAlreadyInList)
                UIManager.instance.BlendInUI(refM.warningWordAlreadyInList, 3);
            else if (caseIsFull)
                UIManager.instance.BlendInUI(refM.warningCaseFull, 3);
            WordUtilities.ReturnWordIntoText(bubble);
        }

        ReloadContents();
        EffectUtilities.ReColorAllInteractableWords();
    }
    /// <summary>
    /// Delete currentWord out of the Case
    /// </summary>
    public virtual void DeleteOutOfCase()
    {
        BubbleData currentData = WordClickManager.instance.currentWord.GetComponent<Bubble>().data;
        WordCaseManager.instance.overrideTag = currentData.tag;
        BubbleData[] currentContents = contents;
        int deleteInt = -1;
        int i = 0;
        foreach (BubbleData contentData in currentContents)
        {
            if (contentData.name == currentData.name)
            {
                deleteInt = i;
                break;
            }
            i++;
        }
        if (deleteInt > -1)
        {
            string tagToDelete = currentContents[deleteInt].tag;
            currentContents[deleteInt] = new BubbleData();
            CheckIfATagIsEmptyDueToDelete(tagToDelete);
        }
        else
            Debug.Log("The bubble to delete couldnt be found " + currentData.name);
        contents = currentContents;

        ReloadContents();
        RescaleScrollbar();
        DestroyReplacement();
        UpdateContentCount();
    }
    /// <summary>
    /// Create a less visible version of the bubble in the case to indicate its position
    /// </summary>
    /// <param name="word"></param>
    public virtual void SpawnReplacement(Bubble bubble)
    {
        bubbleReplacement = SpawnBubbleInCase(bubble.data);
        Color color = WordUtilities.MatchColorToTag(bubble.data.name, bubble.data.subtag);
        color.a = 0.3f;
        foreach (Image img in bubbleReplacement.GetComponentsInChildren<Image>())
        {
            img.color = color;
        }
    }
    protected virtual void CheckIfANewTagIsIncludedAndAddButton(string tagName)
    {
    }
    protected virtual void CheckIfATagIsEmptyDueToDelete(string tagName)
    {
    }
    #region SCROLLBAR
    public virtual void ChangeScrollbarValue(float scrollValue)
    {
        scrollbar.value -= (scrollValue * 0.001f);
    }
    /// <summary>
    /// Call when the scrollbar is scrolled. moves the words up & down. resets on reload
    /// </summary>
    public virtual void ScrollThroughBubbles()
    {
        if (bubbleScreenHeight > 0)
        {
            float value = scrollbar.value;
            float posY = Mathf.Lerp(0, bubbleScreenHeight, value);
            listingParent.transform.localPosition = new Vector2(listingParent.transform.localPosition.x, posY);
        }
    }
    /// <summary>
    /// Reset the Scrollbar to value = 0, so that we dont get stuck when we delete an item
    /// </summary>
    public virtual void ResetScrollbar()
    {
        ReferenceManager.instance.bubbleScrollbar.value = 0;
    }
    /// <summary>
    /// Takes the ScrollBar and re-scales it according to the current amount of words
    /// </summary>
    /// <param name="bubbleCount"></param>
    public virtual IEnumerator RescaleScrollbar()
    {
        yield return new WaitForEndOfFrame();
        
        UpdateScrollbarBubbleHeight();

        float addedHeight = Mathf.Clamp(bubbleScreenHeight, 0, refM.bubbleScreenHeightMaxSize);
        float scrollbarSize = WordUtilities.Remap(addedHeight, 0, refM.bubbleScreenHeightMaxSize, 1, 0.05f);
        scrollbar.size = scrollbarSize;
    }
    /// <summary>
    /// Take the height of all bubbles that are protrayed at the moment and subtract them by the screen height
    /// </summary>
    public virtual void UpdateScrollbarBubbleHeight()
    {
        float spacing = listingParent.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing;
        bubbleScreenHeight = spacing;

        RectTransform currentRectTransform;
        for (int i = 0; i < listingParent.transform.childCount; i++)
        {
            currentRectTransform = listingParent.transform.GetChild(i).GetComponent<RectTransform>();
            bubbleScreenHeight += currentRectTransform.sizeDelta.y;
            bubbleScreenHeight += spacing;
        }
        bubbleScreenHeight -= caseScreenHeight;
    }
    #endregion
    #endregion
    #region NOT VIRTUAL
    protected void SaveBubbleDataInContents(int index, BubbleData data)
    {
        contents[index] = data;
    }

    /// <summary>
    /// counts how many contents there are in the list right now
    /// </summary>
    public int GetContentCount()
    {
        int count = 0;
        foreach (BubbleData data in contents)
        {
            if (data.name != null)
                count++;
        }
        return count;
    }
    /// <summary>
    /// Checks if the given bubble data exists and Updates it from the given value
    /// </summary>
    /// <param name="data"></param>
    public virtual void UpdateBubbleData(BubbleData data)
    {
        GetBubbleData(data, out int index);
        if (index != -1)
        {
            contents[index] = data;
        }
    }
    public virtual void RearrangeContents()
    {
        List<BubbleData> favorites = new List<BubbleData>();
        List<BubbleData> nonFavorites = new List<BubbleData>();

        foreach (BubbleData data in contents)
        {
            if (data.isFavorite)
                favorites.Add(data);
            else
                nonFavorites.Add(data);
        }
        favorites.AddRange(nonFavorites);
        contents = favorites.ToArray();
    }
    public virtual void RearrangeContents(ref List<BubbleData> bubbleDataList)
    {
        List<BubbleData> favorites = new List<BubbleData>();
        List<BubbleData> nonFavorites = new List<BubbleData>();

        foreach (BubbleData data in bubbleDataList)
        {
            if (data.isFavorite)
                favorites.Add(data);
            else
                nonFavorites.Add(data);
        }
        favorites.AddRange(nonFavorites);
        bubbleDataList = favorites;
    }
    /// <summary>
    /// Checks if the given bubble data exists and returns the value and it's index
    /// If there is no value, the return indey is -1
    /// </summary>
    /// <param name="data"></param>
    public BubbleData GetBubbleData(BubbleData data, out int index)
    {
        BubbleData returnData = null;

        index = -1;
        int i = -1;
        foreach (BubbleData contentData in contents)
        {
            i++;
            if (contentData.name == data.name)
            {
                returnData = contentData;
                index = i;
            }
        }
        return returnData;
    }
    public BubbleData GetBubbleData(string dataName, out int index)
    {
        BubbleData data = new BubbleData() { name = dataName };
        data = GetBubbleData(data, out index);
        return data;
    }

    /// <summary>
    /// Opens case and memorizes, if it was open before. If not, it will close on false.
    /// </summary>
    /// <param name="open"></param>
    public void AutomaticOpenCase(bool open)
    {
        if (open)
        {
            if (caseObject.activeInHierarchy)
                automaticallyOpen = true;
            caseObject.SetActive(true);
        }
        else
            if (!automaticallyOpen)
                caseObject.SetActive(false);
        ResetScrollbar();
    }
    /// <summary>
    /// Take the bubbledata and spawn a bubble in the correct case
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual GameObject SpawnBubbleInCase(BubbleData data)
    {
        GameObject bubble = WordUtilities.CreateWord(data, Vector3.zero, new TMP_WordInfo(), Vector2.zero, origin, true);
        bubble.transform.SetParent(listingParent.transform);

        return bubble;
    }

    /// <summary>
    /// takes the name of a bubble and checks if it fits into the contents array
    /// </summary>
    /// <param name="name"></param>
    /// <param name="saveAtIndex"></param>
    /// <returns></returns>
    public bool CheckIfCanSaveBubble(string name, out int saveAtIndex, out bool bubbleIsAlreadyInList, out bool caseIsFull, string overrideTag = null)
    {
        WordCaseManager.instance.overrideTag = overrideTag;
        BubbleData[] currentContents = contents;

        saveAtIndex = -1;
        bubbleIsAlreadyInList = (ReferenceManager.instance.duplicateWords) ? false : CheckIfBubbleInList(name, currentContents);
        caseIsFull = true;

        if (!bubbleIsAlreadyInList)
        {
            for (int i = 0; i < maxContentAmount; i++)
            {
                if (currentContents != null) //can be null because of the wordcase?
                {
                    if (currentContents[i].name == null)
                    {
                        saveAtIndex = i;
                        caseIsFull = false;
                        break;
                    }
                }
            }
            if (!caseIsFull)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Checks, if the bubble that is supposed to be saved is already in the list
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public bool CheckIfBubbleInList(string name, BubbleData[] currentContents)
    {
        bool inList = false;
        foreach (BubbleData data in currentContents)
        {
            if (data.name != null && data.name == name)
            {
                inList = true;
            }
        }
        return inList;
    }

    /// <summary>
    /// Destroy the wordReplacement, if there is one
    /// </summary>
    public void DestroyReplacement()
    {
        if (bubbleReplacement != null)
        {
            Destroy(bubbleReplacement);
            bubbleReplacement = null;
        }
    }
    #endregion
}
