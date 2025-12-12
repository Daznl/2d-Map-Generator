using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LinkManager
{
    public Character LinkedCharacter { get; set; }
    public List<Date> LinkDates { get; set; } = new List<Date>();
    public List<Date> DelinkDates { get; set; } = new List<Date>();
    public bool IsLinked { get; set; }
    public int LinkTime { get; set; }

    public void Link(RaceManager raceManager, Character thisCharacter, Character other)
    {
            IsLinked = true;
            other.LinkManager.IsLinked = true;
            LinkedCharacter = other;
            other.LinkManager.LinkedCharacter = thisCharacter;

            Date currentDate = thisCharacter.GetCurrentDate();
            LinkDates.Add(currentDate);
            other.LinkManager.LinkDates.Add(currentDate);

            MergeLivingSituations(raceManager, thisCharacter, other);

            // Remove both characters from SingleLookingForHome list
            raceManager.SingleLookingForHome.Remove(thisCharacter);
            raceManager.SingleLookingForHome.Remove(other);

            // Create a couple instance and add it to the RaceManager's list
            Couple newCouple = new Couple(thisCharacter, other);
            raceManager.CoupleLookingForHome.Add(newCouple);
    }
    private void MergeLivingSituations(RaceManager raceManager, Character thisCharacter, Character other)
    {
        var thisGroup = thisCharacter.Data.LivingGroup;
        var otherGroup = other.Data.LivingGroup;

        // Determine the group to merge into based on size or randomly if equal
        LivingGroup chosenGroup = ChooseWhoToMove(thisGroup, otherGroup);

        // Merge characters into the chosen group
        if (chosenGroup != thisGroup)
        {
            raceManager.livingTogetherManager.MoveCharacterBetweenGroups(thisCharacter, chosenGroup);
        }
        if (chosenGroup != otherGroup)
        {
            raceManager.livingTogetherManager.MoveCharacterBetweenGroups(other, chosenGroup);
        }
    }

    private LivingGroup ChooseWhoToMove(LivingGroup thisCharacterGroup, LivingGroup otherCharacterGroup)
    {
        LivingGroup ChosenGroup;
        // Check if both groups have homes
        bool thisGroupHasHome = thisCharacterGroup.IsHoused;
        bool otherGroupHasHome = otherCharacterGroup.IsHoused;

        if (thisGroupHasHome && otherGroupHasHome)
        {
            // Compare the number of characters in each group
            int sizeThisCharacterGroup = thisCharacterGroup.Members.Count;
            int sizeOtherCharacterGroup = otherCharacterGroup.Members.Count;

            if (sizeThisCharacterGroup == sizeOtherCharacterGroup)
            {
                // Randomly choose between the groups if the size is the same
                ChosenGroup = UnityEngine.Random.value > 0.5f ? thisCharacterGroup : otherCharacterGroup;
            }
            else if (sizeThisCharacterGroup < sizeOtherCharacterGroup)
            {
                ChosenGroup = thisCharacterGroup;
            }
            else
            {
                ChosenGroup = otherCharacterGroup;
            }
        }
        else if (thisGroupHasHome && !otherGroupHasHome)
        {
            // If only this group has a home, prefer moving into this group
            ChosenGroup = thisCharacterGroup;
        }
        else if (!thisGroupHasHome && otherGroupHasHome)
        {
            // If only the other group has a home, prefer moving into the other group
            ChosenGroup = otherCharacterGroup;
        }
        else
        {
            // If neither group has a home, randomly choose between the two
            ChosenGroup = UnityEngine.Random.value > 0.5f ? thisCharacterGroup : otherCharacterGroup;
        }

        return ChosenGroup;
    }
    
}