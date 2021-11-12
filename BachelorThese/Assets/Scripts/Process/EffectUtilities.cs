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
    /// <summary>
    /// color a singular tag in it's tag color or any other color
    /// </summary>
    /// <param name="word"></param>
    /// <param name="tag"></param>
    public static void ColorTag(GameObject word, string tagName)
    {
        Color tagColor = WordUtilities.MatchColorToTag(tagName);
        foreach (Image image in word.GetComponentsInChildren<Image>())
            image.color = tagColor;
    }
    public static void ColorTag(GameObject word, Color color)
    {
        foreach (Image image in word.GetComponentsInChildren<Image>())
            image.color = color;
    }
    /// <summary>
    /// put in a Color Gradient here with FIVE (5) Colors. empty colors will be ignored
    /// </summary>
    /// <param name="word"></param>
    /// <param name="colorGradient"></param>
    /// <returns></returns>
    public static IEnumerator ColorTagGradient(GameObject word, Color[] colorGradient, float time)
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
                    ColorTag(word, currentColor);
                }
            }
            yield return delay;
        }
        ColorTag(word, calculatedColorGradient[4]);
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
    public static void ColorAWord(Word word, Color color)
    {
        if (word.relatedText != null)
        {
            ColorAWord(word.relatedText, new TMP_WordInfo[1] { word.relatedWordInfo }, color);
        }
    }
    /// <summary>
    /// Take all colorable texts, check if they are active and color them
    /// </summary>
    public static void ReColorAllInteractableWords()
    {
        foreach (TMP_Text text in ReferenceManager.instance.interactableTextList)
        {
            text.ForceMeshUpdate();
            if (text.isActiveAndEnabled)
            {
                for (int i = 0; i < text.textInfo.wordCount; i++)
                {
                    TMP_WordInfo wordInfo = text.textInfo.wordInfo[i];
                    if (WordLookupReader.instance.CheckForWord(wordInfo, out TMP_WordInfo[] wordInfos, out bool isFillerWord))
                    {
                        if (WordUtilities.CheckIfWordIsUsed(WordUtilities.WordInfoToString(wordInfos), wordInfos.Length, isFillerWord, out bool cantBeSaved))
                        {
                            if (!cantBeSaved)
                                ColorAWord(text, wordInfos, ReferenceManager.instance.interactedColor);
                            else
                                ColorAWord(text, wordInfos, ReferenceManager.instance.inListColor);
                        }
                        else
                            ColorAWord(text, wordInfos, ReferenceManager.instance.interactableColor);
                    }
                }
            }
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

}
public class RefBool
{
   public bool refBool = false;
}
