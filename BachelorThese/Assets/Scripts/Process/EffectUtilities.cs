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
    public static void ColorTag(GameObject word, WordInfo.WordTag tag)
    {
        foreach (Image image in word.GetComponentsInChildren<Image>())
            image.color = tag.tagColor;
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
                        if (WordUtilities.CheckIfWordIsUsed(WordUtilities.WordInfoToString(wordInfos), wordInfos.Length, isFillerWord))
                        {
                            ColorAWord(text, wordInfos, ReferenceManager.instance.interactedColor);
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
    /// Lerp between different alpha values for an image
    /// </summary>
    /// <returns></returns>
    /*IEnumerator AlphaWave(Image img)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        Color color = img.color;
        float alpha = 0;
        float highAlpha = color.a;

        while (true)
        {

            yield return delay;
        }
    }*/
}
