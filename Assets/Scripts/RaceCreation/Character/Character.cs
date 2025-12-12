using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Android;
using UnityEngine.Analytics;
using System.Diagnostics;
using UnityEngine.Animations;

public class Character
{
    public CharacterData Data;
    public FamilyRelations FamilyRelations;
    public LinkManager LinkManager;
    public TimeCounter timeCounter;

    public Character(RaceManager raceManager, Character parent1, Character parent2, TimeCounter timeCounterInstance, Gender? specifiedGender = null)
    {
        string race = raceManager.raceProperties.RaceName;
        this.timeCounter = timeCounterInstance;

        // Initialize CharacterData first
        this.Data = new CharacterData(
            GenerateFullName(),
            specifiedGender ?? (Gender)UnityEngine.Random.Range(0, 2),
            race,
            GetCurrentDate(),
            true, // This could be adjusted based on actual homelessness status after living group assignment
            null, // Initial home, adjusted later if needed
            null // Initialize LivingWith directly based on character situation
        );

        // Initialize other properties
        this.FamilyRelations = new FamilyRelations();
        this.LinkManager = new LinkManager();
        this.LinkManager.IsLinked = false;
        this.LinkManager.LinkTime = 0;
        this.Data.lifeSpan = UnityEngine.Random.Range(raceManager.raceProperties.LifeSpan - 10, raceManager.raceProperties.LifeSpan + 10);
        this.LinkManager.LinkDates = new List<Date>();
        this.LinkManager.DelinkDates = new List<Date>();

        //if the character is not a starting character
        if (parent1 != null && parent2 != null)
        {
            // Now it's safe to use Data property
            FamilyRelations.Parents.Add(parent1);
            FamilyRelations.Parents.Add(parent2);

            FamilyRelations.AddSiblings(this, parent1, parent2);
            FamilyRelations.AddStepSiblings(this, parent1, parent2);
            // Add the new character to the parent's living group, if it exists
            raceManager.livingTogetherManager.AddCharacterToGroup(this, parent1.Data.LivingGroup);

            //if there are parents add the character to those parents
            parent1?.FamilyRelations.Children.Add(this);
            parent2?.FamilyRelations.Children.Add(this);
        }
        //is a starting character
        else
        {
            Town initialTown = raceManager.buildingManager.StartingTown;
            // For characters without parents, create a new living group
            var newGroup = new LivingGroup(new List<Character> { this }, false);
            raceManager.livingTogetherManager.AddLivingGroup(newGroup);
            // Since it's a new group, directly assign the character's LivingGroup to this new group
            this.Data.LivingGroup = newGroup;
        }  
    }

    
    private string GenerateFullName()
    {
        string firstName = TextFileFunctions.GetRandomLineFromTextFile("TextFile/CharacterFirstNames");
        string lastName = TextFileFunctions.GetRandomLineFromTextFile("TextFile/CharacterLastNames");
        return $"{firstName} {lastName}";
    }

    public Date GetCurrentDate()
    {
        int currentYear = timeCounter.currentYear;
        int daysInYear = timeCounter.daysInYear;
        Date currentDate = new Date(currentYear, timeCounter.totalDays % daysInYear);

        return currentDate;
    }    
}
