using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Case : MonoBehaviour
{
    //inheriting classes fill these values in on InitializeValues()
    [HideInInspector] public GameObject caseObject;
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
        ReloadContents(true);
        ResetScrollbar();
    }
    /// <summary>
    /// Reloads the Case and respawns it's content
    /// </summary>
    public virtual void ReloadContents(bool resetScrollbar)
    {
        // Delete all possibly active bubbles
        foreach (Bubble data in listingParent.GetComponentsInChildren<Bubble>())
        {
            if (data.gameObject != listingParent)
            {
                Destroy(data.gameObject);
            }
        }
        SpawnContents();
        UpdateContentCount();
        StartCoroutine(RescaleScrollbar(resetScrollbar));
        DestroyReplacement();
    }
    /// <summary>
    /// Spawn all bubbles, that are in contents right now into the case
    /// </summary>
    public virtual void SpawnContents()
    {
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
    public virtual void SaveBubble(Bubble bubble)
    {
        BubbleData data = bubble.data;
        if (CheckIfCanSaveBubble(data.name, out int index))
        {
            contents[index] = data;
        }
        else
        {
            //put the bubble back into the dialogue
            WordUtilities.ReturnWordIntoText(bubble);
        }
        //Reload
        ReloadContents(false);
        StartCoroutine(RescaleScrollbar(false));
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
            currentContents[deleteInt] = new BubbleData();
        }
        else
            Debug.Log("The bubble to delete couldnt be found " + currentData.name);
        contents = currentContents;
        ReloadContents(true);
    }
    #region SCROLLBAR
    /// <summary>
    /// Call when the scrollbar is scrolled. moves the words up & down. resets on reload
    /// </summary>
    public virtual void ScrollThroughBubbles()
    {
        if (bubbleScreenHeight > 0)
        {
            float value = scrollbar.value;
            float posY = Mathf.Lerp(0, bubbleScreenHeight, value);
            listingParent.transform.localPosition = new Vector2(0, posY);
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
    public virtual IEnumerator RescaleScrollbar(bool resetScrollbar)
    {
        //The old names arent deleted until the frame is over
        yield return new WaitForEndOfFrame();

        UpdateBubbleHeight();

        //if the value is smaller than what fits the canvas, it is irrelevant
        float addedHeight = Mathf.Clamp(bubbleScreenHeight, 0, refM.bubbleScreenHeightMaxSize);

        //between biggest size (1) and smallest we want (0.05f)
        float scrollbarSize = WordUtilities.Remap(addedHeight, 0, refM.bubbleScreenHeightMaxSize, 1, 0.05f);
        scrollbar.size = scrollbarSize;

        //if required, reset scrollbar
        if (resetScrollbar)
        {
            ResetScrollbar();
        }
    }
    /// <summary>
    /// Take the height of all bubbles that are protrayed at the moment and subtract them by the screen height
    /// </summary>
    public virtual void UpdateBubbleHeight()
    {
        float spacing = listingParent.GetComponent<HorizontalOrVerticalLayoutGroup>().spacing;
        bubbleScreenHeight = spacing;
        foreach (RectTransform rT in listingParent.GetComponentsInChildren<RectTransform>())
        {
            if (rT.gameObject.TryGetComponent<Bubble>(out Bubble word) && rT.gameObject != null)
            {
                bubbleScreenHeight += rT.sizeDelta.y;
                bubbleScreenHeight += spacing;
            }
        }
        bubbleScreenHeight -= caseScreenHeight;
    }
    #endregion
    #endregion
    #region NOT VIRTUAL
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
    public void UpdateBubbleData(BubbleData data)
    {
        GetBubbleData(data, out int index);
        if (index != -1)
        {
            contents[index] = data;
            if (this is WordCaseManager)
                Debug.Log(((WordData)contents[index]).currentParent);
        }
        else
            Debug.Log("failed");
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
        // open the case
        if (open)
        {
            if (caseObject.activeInHierarchy)
                automaticallyOpen = true;
            caseObject.SetActive(true);
        }
        //close the case
        else
        {
            if (!automaticallyOpen)
                caseObject.SetActive(false);
        }
        ResetScrollbar();
    }
    /// <summary>
    /// Take the bubbledata and spawn a bubble in the correct case
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public GameObject SpawnBubbleInCase(BubbleData data)
    {
        GameObject bubble = WordUtilities.CreateWord(data, Vector2.zero, new TMP_WordInfo(), Vector2.zero, origin, true);
        bubble.transform.SetParent(listingParent.transform);
        return bubble;
    }
    /// <summary>
    /// Saves bubbleData into the content list
    /// </summary>
    /// <param name="wordItem"></param>

    /// <summary>
    /// takes the name of a bubble and checks if it fits into the contents array
    /// </summary>
    /// <param name="name"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckIfCanSaveBubble(string name, out int index, string overrideTag = null)
    {
        WordCaseManager.instance.overrideTag = overrideTag;
        BubbleData[] currentContents = contents;
        index = -1;
        bool inList;
        if (ReferenceManager.instance.duplicateWords)
            inList = false;
        else
            inList = CheckIfBubbleInList(name, currentContents);

        if (!inList)
        {
            bool foundASpot = false;
            for (int i = 0; i < maxContentAmount; i++)
            {
                if (currentContents != null) //can be null because of the wordcase?
                {
                    if (currentContents[i].name == null)
                    {
                        index = i;
                        foundASpot = true;
                        break;
                    }
                }
            }
            if (foundASpot)
                return true;
            return false;
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
    /// Create a less visible version of the bubble in the case to indicate its position
    /// </summary>
    /// <param name="word"></param>
    public void SpawnReplacement(Bubble bubble)
    {
        bubbleReplacement = SpawnBubbleInCase(bubble.data);
        Color color = WordUtilities.MatchColorToTag(bubble.data.name);
        color.a = 0.3f;
        foreach (Image img in bubbleReplacement.GetComponentsInChildren<Image>())
        {
            img.color = color;
        }
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
