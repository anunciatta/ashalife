using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character currentCharacter;
    public List<Character> allCharacters = new();

    public string userName;

    void Awake()
    {

    }

    void OnDestroy()
    {

    }

    public void SetCurrentCharater(Character character)
    {
        currentCharacter = character;
    }

    public void AddNewCharacter(Character newCharacter)
    {
        allCharacters.Add(newCharacter);
    }
}





[Serializable]
public struct AvatarConfig
{
    public int eyeIndex, beardIndex, hairIndex, headIndex, eyebrowsIndex, earsIndex, mouthIndex, makeupIndex, accessoriesIndex, classesIndex;

    public int skinColorIndex, eyesColorIndex, makeupColorIndex, hairColorIndex, beardColorIndex;

    public AvatarConfig(int classesIndex, int eyeIndex, int beardIndex, int hairIndex, int headIndex, int eyebrowsIndex, int earsIndex, int mouthIndex, int makeupIndex, int accessoriesIndex, int skinColorIndex, int eyesColorIndex, int makeupColorIndex, int hairColorIndex, int beardColorIndex)
    {
        this.eyeIndex = eyeIndex;
        this.beardIndex = beardIndex;
        this.hairIndex = hairIndex;
        this.classesIndex = classesIndex;
        this.headIndex = headIndex;
        this.eyebrowsIndex = eyebrowsIndex;
        this.earsIndex = earsIndex;
        this.mouthIndex = mouthIndex;
        this.accessoriesIndex = accessoriesIndex;
        this.makeupIndex = makeupIndex;

        this.skinColorIndex = skinColorIndex;
        this.eyesColorIndex = eyesColorIndex;
        this.makeupColorIndex = makeupColorIndex;
        this.beardColorIndex = beardColorIndex;
        this.hairColorIndex = hairColorIndex;
    }
}

[Serializable]
public struct EquippedItens
{
    public string armor;
    public string shield;
    public string helmet;
    public string wings;
    public string jewelry;
    public string weapon;
}


