using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Yarn.Unity;
using System;
using System.Text.RegularExpressions;

public static class EffectUtilities
{
    public static void ColorAllChildrenOfAnObject(GameObject parent, string tagName, string subtagName)
    {
        Color tagColor = WordUtilities.MatchColorToTag(tagName, subtagName);
        ColorAllChildrenOfAnObject(parent, tagColor);
    }
    public static void ColorAllChildrenOfAnObject(GameObject parent, Color color)
    {
        if (parent == null)
            return;

        foreach (Image image in parent.GetComponentsInChildren<Image>())
            image.color = color;
    }
    public static void ColorAllChildrenOfAnObject(GameObject parent, Color color, bool ignoreStar)
    {
        if (parent == null)
            return;

        foreach (Image image in parent.GetComponentsInChildren<Image>())
            if (image.tag != "DontColorImage")
                image.color = color;
    }
    public static void ColorAllChildrenOfAnObject(GameObject parent, string tagName, string subtagName, bool ignoreStar)
    {
        if (!ignoreStar)
        {
            ColorAllChildrenOfAnObject(parent, tagName, subtagName);
            return;
        }

        ColorAllChildrenOfAnObject(parent, WordUtilities.MatchColorToTag(tagName, subtagName), ignoreStar);
    }
    public static void ColorAnObject(GameObject gameObject, Color color)
    {
        if (gameObject == null || !gameObject.TryGetComponent<Image>(out Image image))
            return;

        image.color = color;
    }
    /// <summary>
    /// take a game object and color all its images in a gradient for "time"-seconds
    /// put in a Color Gradient here with FIVE (5) Colors. empty colors will be ignored
    /// </summary>
    /// <param name="word"></param>
    /// <param name="colorGradient"></param>
    /// <returns></returns>
    public static IEnumerator ColorObjectAndChildrenInGradient(GameObject word, Color[] colorGradient, float time)
    {
        Color[] calculatedColorGradient = ReadColorGradient(colorGradient);
        Color currentColor;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        float timer = 0;
        float t;
        int index;
        while (timer < time)
        {
            timer += Time.deltaTime;
            if (word != null)
            {
                index = Mathf.FloorToInt(timer / (time / 5)); //returns the current int we are starting from

                if (index < 4)
                {
                    t = WordUtilities.Remap(timer, index * (time / 5), (index + 1) * (time / 5), 0, 1);
                    currentColor = Color.Lerp(calculatedColorGradient[index], calculatedColorGradient[index + 1], t);
                    ColorAllChildrenOfAnObject(word, currentColor);
                }
            }
            yield return delay;
        }
        ColorAllChildrenOfAnObject(word, calculatedColorGradient[4]);
    }
    public static IEnumerator ColorSingularObjectInGradient(GameObject word, Color[] colorGradient, float time)
    {
        Color[] calculatedColorGradient = ReadColorGradient(colorGradient);
        Color currentColor;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        float timer = 0;
        float t;
        int index;
        while (timer < time)
        {
            timer += Time.deltaTime;
            if (word != null)
            {
                index = Mathf.FloorToInt(timer / (time / 5)); //returns the current int we are starting from

                if (index < 4)
                {
                    t = WordUtilities.Remap(timer, index * (time / 5), (index + 1) * (time / 5), 0, 1);
                    currentColor = Color.Lerp(calculatedColorGradient[index], calculatedColorGradient[index + 1], t);
                    ColorAnObject(word, currentColor);
                }
            }
            yield return delay;
        }
        ColorAnObject(word, calculatedColorGradient[4]);
    }
    /// <summary>
    /// color the given word in the text in the given color
    /// </summary>
    /// <param name="text"></param>
    /// <param name="wordInfos"></param>
    /// <param name="color"></param>
    public static void ColorAWord(TMP_Text text, TMP_WordInfo[] wordInfos, Color32 color)
    {
        foreach (TMP_WordInfo wordInfo in wordInfos)
        {
            for (int i = wordInfo.firstCharacterIndex; i <= wordInfo.lastCharacterIndex; i++)
            {
                TMP_CharacterInfo charInfo = text.textInfo.characterInfo[i];
                int vertexIndex = charInfo.vertexIndex;
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
                Color32[] vertexColors = text.textInfo.meshInfo[meshIndex].colors32;

                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;

                // Update Geometry
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

    }
    public static void ColorAWord(Bubble word, Color color)
    {
        if (word.originalText != null)
        {
            ColorAWord(word.originalText, new TMP_WordInfo[1] { word.originalWordInfo }, color);
        }
    }
    /// <summary>
    /// Take all colorable texts, check if they are active and color them
    /// </summary>
    public static void ReColorAllInteractableWords()
    {
        if (ReferenceManager.instance.highlightedWordsEnabled)
        {
            foreach (TMP_Text text in ReferenceManager.instance.interactableTextList)
            {
                text.ForceMeshUpdate();
                if (text.isActiveAndEnabled)
                {
                    for (int i = 0; i < text.textInfo.wordCount; i++)
                    {
                        TMP_WordInfo wordInfo = text.textInfo.wordInfo[i];
                        FindCorrectWordColorAndColorWord(wordInfo, text);
                    }
                }
            }
        }
    }

    public static void FindCorrectWordColorAndColorWord(TMP_WordInfo wordInfo, TMP_Text text)
    {
        if (WordLookupReader.instance.CheckForWord(wordInfo, out TMP_WordInfo[] wordInfos, out bool isFillerWord))
        {
            if (WordUtilities.IsWordInADataBank(WordUtilities.WordInfoToString(wordInfos), isFillerWord, out bool wordAlreadyInWordCase, out bool wordCaseIsFull))
            {
                if (wordAlreadyInWordCase)
                    ColorAWord(text, wordInfos, ReferenceManager.instance.inListColor);
                else if (wordCaseIsFull)
                    ColorAWord(text, wordInfos, ReferenceManager.instance.listFullColor);
                else
                    ColorAWord(text, wordInfos, ReferenceManager.instance.interactedColor);
            }
            else
                ColorAWord(text, wordInfos, ReferenceManager.instance.interactableColor);
        }
    }

    /// <summary>
    /// Takes in a Gradient and mixes in the colors in between the given ones
    /// </summary>
    /// <param name="colorGradient"></param>
    /// <returns></returns>
    static Color[] ReadColorGradient(Color[] colorGradient)
    {
        Color emptyColor = new Color();
        int lastIndex = 0;
        int nextIndex = 0;
        Color nextColor = new Color();
        Color[] calculatedColorGradient = new Color[5] { new Color(), new Color(), new Color(), new Color(), new Color() };

        for (int i = 0; i < 5; i++)
        {
            if (colorGradient[i] == emptyColor) //is empty 
            {
                // get the next index
                for (int j = i; j < 5; j++)
                {
                    if (colorGradient[j] != emptyColor)
                    {
                        nextIndex = j;
                        break;
                    }
                }
                if (nextIndex == 0)
                    nextIndex = 4;
                nextColor = Color.Lerp(colorGradient[lastIndex], colorGradient[nextIndex], WordUtilities.Remap(i, lastIndex, nextIndex, 0, 1));
                calculatedColorGradient[i] = nextColor;
            }
            else
            {
                lastIndex = i;
                calculatedColorGradient[i] = colorGradient[i];
            }
        }
        return calculatedColorGradient;
    }
    /// <summary>
    /// Takes Two Colors and Compares them. If they are equal, return true
    /// </summary>
    /// <param name="colorA"></param>
    /// <param name="colorB"></param>
    /// <returns></returns>
    public static bool CompareColor32(Color32 colorA, Color32 colorB)
    {
        if (colorA.r == colorB.r)
        {
            if (colorA.g == colorB.g)
            {
                if (colorA.b == colorB.b)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool CompareColor(Color colorA, Color colorB)
    {
        if (colorA.r == colorB.r)
        {
            if (colorA.g == colorB.g)
            {
                if (colorA.b == colorB.b)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// Take a color and return a grey-ish version of it
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color32 ColorTintGrey(Color32 color)
    {
        return Color.Lerp(color, ReferenceManager.instance.shadowButtonColor, 0.2f);
    }
    public static Color32 ColorTintWhite(Color32 color)
    {
        return Color.Lerp(color, Color.white, 0.2f);
    }
    /// <summary>
    /// Lerp between different alpha values for an image
    /// </summary>
    /// <returns></returns>
    public static IEnumerator AlphaWave(Image img, RefBool checkBool)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        checkBool.refBool = true;

        Color color = img.color;
        float alpha;
        float t;
        float timer = 0;
        while (checkBool.refBool)
        {
            timer += Time.deltaTime;
            t = WordUtilities.Remap(Mathf.Sin(timer * 2.8f), -1, 1, 0, 1.3f);
            t = Mathf.Clamp01(t);
            alpha = Mathf.Lerp(0.3f, 1, t);
            color.a = alpha;
            img.color = color;
            yield return delay;
        }
    }
    /// <summary>
    /// Take an object and shake it as feedback
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="shakeTime"></param>
    /// <returns></returns>
    public static IEnumerator ShakeNo(GameObject obj, float shakeTime)
    {
        if (!WordUtilities.ArrayContains(UIManager.instance.activeEffects, obj))
        {
            RefBool activeEffect = new RefBool() { refBool = true, refObject = obj };
            WordUtilities.AddToArray(UIManager.instance.activeEffects, activeEffect);
            WaitForEndOfFrame delay = new WaitForEndOfFrame();

            RectTransform rT = obj.GetComponent<RectTransform>();
            Vector2 startPos = rT.localPosition;
            Vector2 targetPosRight = startPos + Vector2.right * 4 + Vector2.up * 3;
            Vector2 targetPosLeft = startPos + Vector2.left * 4 + Vector2.down * 3;

            float t;
            float timer = 0;
            while (timer < shakeTime)
            {
                timer += Time.deltaTime;
                if (timer < shakeTime / 4)
                    t = WordUtilities.Remap(timer, 0, shakeTime / 4, 0.5f, 1);
                else if (timer < shakeTime * 3 / 4)
                    t = WordUtilities.Remap(timer, shakeTime / 4, shakeTime * 3 / 4, 1, 0);
                else
                    t = WordUtilities.Remap(timer, shakeTime * 3 / 4, shakeTime, 0, 0.5f);
                rT.localPosition = Vector2.Lerp(targetPosLeft, targetPosRight, t);
                yield return delay;
            }
            rT.localPosition = startPos;
            WordUtilities.RemoveFromArray(UIManager.instance.activeEffects, activeEffect);
        }
    }
}
public class RefBool
{
    public bool refBool = false;
    public GameObject refObject = null;
}
