using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static BuildingManager;
using static ResourceManager;

public class CharacterFunctions : MonoBehaviour
{
    public void SpawnInitialCharacters(RaceManager raceManager, TimeCounter timeCounter)
    {
        SpawnCharacters(raceManager, timeCounter, 5, null, null, Gender.Male);
        SpawnCharacters(raceManager, timeCounter, 5, null, null, Gender.Female);
        Debug.Log("spawned initial Characters");

        // Assuming initial characters are considered homeless and part of a larger homeless group
        //List<Character> initialHomelessGroup = new List<Character>(raceManager.Characters);

        CreateInitialHomes(raceManager);
    }

    private void CreateInitialHomes(RaceManager raceManager)
    {
        // Assuming the first town in the list is the initial spawn point town
        Town initialTown = raceManager.buildingManager.StartingTown;
        for (int i = 0; i < 5; i++)
        {
            Building newHome = new Building(Building.BuildingFunction.House); // Corrected constructor usage
            initialTown.AddUnoccupiedHouse(newHome); // Correct method call on the initialTown object
        }
        //Debug.Log($"Created 5 unoccupied homes in {initialTown.Name}");
    }

    public void SpawnCharacters(RaceManager raceManager, TimeCounter timeCounter, int numCharacters, Character parent1, Character parent2, Gender? specifiedGender = null)
    {
        for (int i = 0; i < numCharacters; i++)
        {
            Character newCharacter = new Character(raceManager, parent1, parent2, timeCounter, specifiedGender);
            
            raceManager.aliveCharacters.Characters.Add(newCharacter);
            raceManager.aliveCharacters.Children.Add(newCharacter);
            raceManager.aliveCharacters.NotWorkers.Add(newCharacter);
        }
    }

    public void UpdateAdultThings(RaceManager raceManager)
    {
        List<Character> AdultCopy = new List<Character>(raceManager.aliveCharacters.Adults);
        foreach (Character character in AdultCopy)
        {
            if (character.LinkManager.IsLinked)
            {
                //ProcessDeLinking(character, raceManager.raceProperties);
                ProcessBirth(character, raceManager);
            }
            else
            {
                ProcessLinking(character, raceManager);
            }
        }
    }

    public void ProcessBirth(Character character, RaceManager raceManager)
    {
        if (CanGiveBirth(raceManager, character))
        {
            /*int numChildren = UnityEngine.Random.Range(1, raceManager.raceProperties.MaximumChildAmount + 1);*/
            SpawnCharacters(raceManager, raceManager.timeCounter, 1, character, character.LinkManager.LinkedCharacter, null);
        }
    }

    public void ProcessLinking(Character character, RaceManager raceManager)
    {
        // Use Unity's Random.Range to check for a 10% chance
        if (Random.Range(0, 1000) < 2) // 10% chance
        {
            Character closestCharacter = FindAcceptableCharacter(raceManager, character);
            if (closestCharacter != null)
            {
                character.LinkManager.Link(raceManager, character, closestCharacter);
               // Debug.Log(character.Data.race + " Linked");
            }
        }
    }

    /*private void ProcessDeLinking(Character character, RaceProperties raceProperties)
    {
        if (Random.Range(1, raceProperties.LinkStaticChance) <= raceProperties.DeLinkingChance)
        {
            //Debug.Log(character.race + " DeLinked");
            character.LinkManager.Delink(character);
        }
    } */

    public Character FindAcceptableCharacter(RaceManager raceManager, Character currentCharacter)
    {
        Character closestCharacter = null;
        float closestAgeDifference = Mathf.Infinity;

        // Iterate through all adult characters to find the closest match in age, regardless of gender or linkage status
        foreach (Character otherCharacter in raceManager.aliveCharacters.Adults)
        {
            bool isFamilyMember = currentCharacter.FamilyRelations.Parents.Contains(otherCharacter) ||
                              currentCharacter.FamilyRelations.Siblings.Contains(otherCharacter) ;
            // Ensure it's not the same character
            if (otherCharacter != currentCharacter && !isFamilyMember && !otherCharacter.LinkManager.IsLinked)
            {
                // Calculate age difference
                float ageDifference = Mathf.Abs(otherCharacter.Data.age - currentCharacter.Data.age);

                // Update the closestCharacter if this character is the closest so far
                if (ageDifference < closestAgeDifference)
                {
                    closestAgeDifference = ageDifference;
                    closestCharacter = otherCharacter;
                }
            }
        }

        return closestCharacter;
    }

    public bool CanGiveBirth(RaceManager raceManager, Character character)
    {
        if (character.LinkManager.IsLinked
            && character.Data.gender == Gender.Female
            && character.LinkManager.LinkTime >= raceManager.raceProperties.TimeBeforeCanGiveBirth
            && Random.Range(1, raceManager.raceProperties.BirthStaticChance * raceManager.birthModifier) <= raceManager.raceProperties.BirthChance
            && character.FamilyRelations.Children.Count < raceManager.raceProperties.MaximumChildAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //iterate through the list of children to determine if they are now an adult
    public void UpdateAdultStatus(RaceManager raceManager)
    {
        List<Character> newlyAdults = new List<Character>();

        foreach (Character character in raceManager.aliveCharacters.Children)
        {
                // Check if the child has reached the birthing age to become an adult
                if (character.Data.age >= raceManager.raceProperties.BirthingAge)
                {
                    newlyAdults.Add(character); // Add to temporary list for processing after the loop
                }
        }

        // Process the transition of newly adults from children to adults
        foreach (Character adult in newlyAdults)
        {
            raceManager.aliveCharacters.Children.Remove(adult); // Remove from the children list
            raceManager.aliveCharacters.Adults.Add(adult); // Add to the adults list
            raceManager.SingleLookingForHome.Add(adult);
        }
    }

    public void UpdateWorkingStatus(RaceManager raceManager)
    {
        foreach (Character character in raceManager.aliveCharacters.NotWorkers.ToList())
        {
            // Check if the NotWorker has reached the working age
            if (character.Data.age >= raceManager.raceProperties.WorkingAge)
            {
                // Directly modify the original list during iteration over its copy
                raceManager.aliveCharacters.NotWorkers.Remove(character);
                raceManager.aliveCharacters.Workers.Add(character);
                raceManager.aliveCharacters.UnEmployed.Add(character);
            }
        }
    }

    public void IncrementAge(RaceManager raceManager)
    {
        foreach (Character character in raceManager.aliveCharacters.Characters)
        {
            character.Data.age++;
            if (character.LinkManager.IsLinked)
            {
                character.LinkManager.LinkTime++;
            }
        }
        UpdateCounterText(raceManager);
    }


    public void HandleCharacterDeath(RaceManager raceManager)
    {
        List<Character> charactersCopy = new List<Character>(raceManager.aliveCharacters.Characters);
        foreach (Character character in charactersCopy)
        {
            ProcessRandomDeath(raceManager, character);
            ProcessNaturalDeath(raceManager, character);
        }
    }


    private void ProcessRandomDeath(RaceManager raceManager, Character character)
    {
        int deathRoll = Random.Range(0, raceManager.raceProperties.RandomDeathStaticChance);
        if (deathRoll <= raceManager.raceProperties.RandomDeathChance * raceManager.deathModifier)
        {
            ProcessDeath(raceManager, character); // Assuming this method handles the consequences of death
            //Debug.Log($"{character.Data.name} Randomly Died");
        }
    }

    private void ProcessNaturalDeath(RaceManager raceManager, Character character)
    {

        if (character.Data.age >= character.Data.lifeSpan && Random.Range(0f, 1f) <= 0.0002f)
        {
            ProcessDeath(raceManager, character); ;
            //Debug.Log($"{character.Data.name} Died Naturally");
        }
    }
    public void ProcessDeath(RaceManager raceManager, Character character)
    {
        UpdateAliveAndDeadLists(raceManager, character);

        RecordDeathDate(character);

        UpdateLivingGroups(raceManager, character);

        MoveAdultsOrChildrenToDead(raceManager, character);

        UpdateWorkRelatedGroups(raceManager, character);

        UpdateHomeSeekerGroups(raceManager, character);

    }

    private void UpdateAliveAndDeadLists(RaceManager raceManager, Character character)
    {
        raceManager.aliveCharacters.Characters.Remove(character);
        raceManager.deadCharacters.Characters.Add(character);
    }
    private void RecordDeathDate(Character character)
    {
        int currentYear = character.timeCounter.currentYear;
        int daysInYear = character.timeCounter.daysInYear;
        character.Data.deathDate = new Date(currentYear, character.timeCounter.totalDays % daysInYear);
    }

    private void UpdateLivingGroups(RaceManager raceManager, Character character)
    {
        var livingGroup = character.Data.LivingGroup;
        if (livingGroup != null)
        {
            // Remove the character from the living group
            raceManager.livingTogetherManager.RemoveCharacterFromGroup(character, livingGroup);

            // Check if the living group needs to be disbanded or updated
            raceManager.livingTogetherManager.UpdateLivingGroupAfterRemoval(livingGroup);
        }
    }

    private void MoveAdultsOrChildrenToDead(RaceManager raceManager, Character character)
    {
        // Remove the character from the appropriate list based on their adult status
        if (raceManager.aliveCharacters.Adults.Contains(character)
            && raceManager.aliveCharacters.Children.Contains(character))
        {
            Debug.LogError("SHiettt");
        }
        else if (raceManager.aliveCharacters.Adults.Contains(character))
        {
            raceManager.aliveCharacters.Adults.Remove(character);
            raceManager.deadCharacters.Adults.Add(character);
        }
        else if (raceManager.aliveCharacters.Children.Contains(character))
        {
            raceManager.aliveCharacters.Children.Remove(character);
            raceManager.deadCharacters.Children.Add(character);
        }
        else
        {
            Debug.LogError("Character was not part adult or child list");
        }
    }

    private void UpdateWorkRelatedGroups(RaceManager raceManager, Character character)
    {
        // Directly handle the job associated with the character, if any.
        if (character.Data.job != null)
        {
            Job currentJob = character.Data.job;

            raceManager.jobManager.UnassignJob(raceManager, character, currentJob);
        }

        // Remove the character from the workers or not-workers list, as appropriate.
        if (raceManager.aliveCharacters.Workers.Contains(character))
        {
            raceManager.aliveCharacters.Workers.Remove(character);
            // Remove the character from employed or unemployed lists based on their current status.
            if (raceManager.aliveCharacters.Employed.Contains(character))
            {
                raceManager.aliveCharacters.Employed.Remove(character);
            }
            else if (raceManager.aliveCharacters.UnEmployed.Contains(character))
            {
                raceManager.aliveCharacters.UnEmployed.Remove(character);
            }
        }
        else if (raceManager.aliveCharacters.NotWorkers.Contains(character))
        {
            raceManager.aliveCharacters.NotWorkers.Remove(character);
        }

        // Optionally, handle any additional cleanup or notifications needed due to the character's job status change.
    }

    private void UpdateHomeSeekerGroups(RaceManager raceManager, Character character)
    {
        // Remove the character from the SingleLookingForHome list if they are in it
        if (raceManager.SingleLookingForHome.Contains(character))
        {
            raceManager.SingleLookingForHome.Remove(character);
        }

        var couplesWithCharacter = raceManager.CoupleLookingForHome.Where(couple => couple.characters.Contains(character)).ToList();

        foreach (var couple in couplesWithCharacter)
        {
            // If you decide to remove the deceased character from the couple
            couple.characters.Remove(character);

            // If a couple should be disbanded when one dies (optional, based on your game logic):
            // Check if the couple should be removed entirely (e.g., if there are no characters left or if your logic requires couples to always be pairs)
            if (couple.characters.Count < 2)
            {
                raceManager.CoupleLookingForHome.Remove(couple);
                // Handle any additional cleanup or reclassification of the surviving character
                // For example, moving them to SingleLookingForHome if they're not part of a couple anymore
                if (couple.characters.Count == 1)
                {
                    Character survivingCharacter = couple.characters.First();
                    if (!raceManager.SingleLookingForHome.Contains(survivingCharacter))
                    {
                        raceManager.SingleLookingForHome.Add(survivingCharacter);
                    }
                }
            }
        }
    } 
    public int CountHomelessGroups(List<LivingGroup> livingGroups)
    {
        int homelessCount = 0;

        foreach (LivingGroup group in livingGroups)
        {
            if (!group.IsHoused)
            {
                homelessCount++;
            }
        }

        return homelessCount;
    }

    public void UpdateCounterText(RaceManager raceManager)
    {
        int totalCharacters = raceManager.aliveCharacters.Characters.Count + raceManager.deadCharacters.Characters.Count;

        int totalAlive = raceManager.aliveCharacters.Characters.Count;
        int totalAliveAdults = raceManager.aliveCharacters.Adults.Count;
        int totalAliveChildren = raceManager.aliveCharacters.Children.Count;

        int totalDead = raceManager.deadCharacters.Characters.Count;
        int totalDeadAdults = raceManager.deadCharacters.Adults.Count;
        int totalDeadChildren = raceManager.deadCharacters.Children.Count;

        int man = raceManager.aliveCharacters.Adults.Count(c => c.Data.gender == Gender.Male);
        int woman = raceManager.aliveCharacters.Adults.Count(c => c.Data.gender == Gender.Female);

        int childMale = raceManager.aliveCharacters.Children.Count(c => c.Data.gender == Gender.Male);
        int childFemale = raceManager.aliveCharacters.Children.Count(c => c.Data.gender == Gender.Female);

        int totalUnLinked = raceManager.aliveCharacters.Adults.Count(c => !c.LinkManager.IsLinked);
        int totalLinked = raceManager.aliveCharacters.Adults.Count(c => c.LinkManager.IsLinked);
        int totalParents = raceManager.aliveCharacters.Adults.Count(c => c.LinkManager.IsLinked && c.FamilyRelations.Children.Count > 0);       

        // Additional statistics
        int totalWorkers = raceManager.aliveCharacters.Workers.Count;
        int totalNotWorkers = raceManager.aliveCharacters.NotWorkers.Count;

        int totalJobs = raceManager.jobManager.Jobs.Count;
        int totalActiveJobs = raceManager.jobManager.activeJobs.Count;
        int totalAvailableJobs = raceManager.jobManager.availableJobs.Count;

        int employed = raceManager.aliveCharacters.Employed.Count;
        int unEmployed = raceManager.aliveCharacters.UnEmployed.Count;

        float deathModifier = raceManager.deathModifier;
        float birthModifier = raceManager.birthModifier;
        float foodConsumptionModifier = raceManager.foodConsumptionModifier;

        // Food resources
        float food = raceManager.resourceManager.resourceStruct.Food;
        float foodProductionRate = raceManager.resourceManager.resourceStruct.FoodProductionRate;
        float foodConsumptionRate = raceManager.resourceManager.resourceStruct.FoodConsumptionRate;

        // Wood resources
        float wood = raceManager.resourceManager.resourceStruct.Wood;
        float woodProductionRate = raceManager.resourceManager.resourceStruct.WoodProductionRate;
        float woodConsumptionRate = raceManager.resourceManager.resourceStruct.WoodConsumptionRate;

        // Ore resources
        float ore = raceManager.resourceManager.resourceStruct.Ore;
        float oreProductionRate = raceManager.resourceManager.resourceStruct.OreProductionRate;
        float oreConsumptionRate = raceManager.resourceManager.resourceStruct.OreConsumptionRate;

        // Bonus Food resources
        float bonusFood = raceManager.resourceManager.resourceStruct.BonusFood;
        float bonusFoodProductionRate = raceManager.resourceManager.resourceStruct.BonusFoodProductionRate;
        float bonusFoodConsumptionRate = raceManager.resourceManager.resourceStruct.BonusFoodConsumptionRate;

        // Bonus Wood resources
        float bonusWood = raceManager.resourceManager.resourceStruct.BonusWood;
        float bonusWoodProductionRate = raceManager.resourceManager.resourceStruct.BonusWoodProductionRate;
        float bonusWoodConsumptionRate = raceManager.resourceManager.resourceStruct.BonusWoodConsumptionRate;

        // Bonus Ore resources
        float bonusOre = raceManager.resourceManager.resourceStruct.BonusOre;
        float bonusOreProductionRate = raceManager.resourceManager.resourceStruct.BonusOreProductionRate;
        float bonusOreConsumptionRate = raceManager.resourceManager.resourceStruct.BonusOreConsumptionRate;

        // Luxury Resource
        float luxuryResource = raceManager.resourceManager.resourceStruct.LuxuryResource;
        float luxuryResourceProductionRate = raceManager.resourceManager.resourceStruct.LuxuryResourceProductionRate;
        float luxuryResourceConsumptionRate = raceManager.resourceManager.resourceStruct.LuxuryResourceConsumptionRate;

        int totalUnoccupiedHomes = raceManager.buildingManager.StartingTown.UnoccupiedHouse.Count;
        int totalOccupiedHomes = raceManager.buildingManager.StartingTown.OccupiedHouse.Count;

        int totalHomelessGroups = CountHomelessGroups(raceManager.livingTogetherManager.LivingGroups);
        int totalCouplesLookingForHomes = raceManager.CoupleLookingForHome.Count;
        int singleLookingForHomesForHomes = raceManager.SingleLookingForHome.Count;
        //

        int totalConstructionsInProgress = raceManager.buildingManager.StartingTown.constructionsInProgress.Count;
        int totalConstructionsJobsActive = raceManager.buildingManager.StartingTown.constructionsInProgress.Sum(construction => construction.jobs.Count);


        string totalText = "<color=#000000>Total Characters:</color> <color=red>" + totalCharacters + "</color>\n" +
                           
                           
                           "<color=#000000>Total Alive:</color> <color=lime>" + totalAlive + "</color>\n" +
                           "<color=#000000>Total Adults:</color> <color=yellow>" + totalAliveAdults + "</color>\n" +
                           "<color=#000000>Total Children:</color> <color=orange>" + totalAliveChildren + "</color>\n" +
                           "<color=#000000>Total Man:</color> <color=yellow>" + man + "</color>\n" +
                           "<color=#000000>Total Women:</color> <color=orange>" + woman + "</color>\n\n" +

                           "<color=#000000>Total Dead:</color> <color=#FFFFFF>" + totalDead + "</color>\n" +
                           "<color=#000000>Total Adults:</color> <color=yellow>" + totalDeadAdults + "</color>\n" +
                           "<color=#000000>Total Children:</color> <color=orange>" + totalDeadChildren + "</color>\n\n" +

                           "<color=#000000>Total Not Linked:</color> <color=lightblue>" + totalUnLinked + "</color>\n" +
                           "<color=#000000>Total Linked:</color> <color=lightblue>" + totalLinked + "</color>\n" +
                           "<color=#000000>Total Parents:</color> <color=purple>" + totalParents + "</color>\n\n" +

                           "<color=#000000>Workers:</color> <color=green>" + totalWorkers + "</color>\n" +
                           "<color=#000000>NotWorkers:</color> <color=green>" + totalNotWorkers + "</color>\n" +

                           "<color=#000000>Jobs:</color> <color=yellow>" + totalJobs + "</color>\n" +
                           "<color=#000000>Active Jobs:</color> <color=yellow>" + totalActiveJobs + "</color>\n" +
                           "<color=#000000>Available Jobs:</color> <color=orange>" + totalAvailableJobs + "</color>\n" +

                           "<color=#000000>Employed:</color> <color=orange>" + employed + "</color>\n" +
                           "<color=#000000>UnEmployed:</color> <color=orange>" + unEmployed + "</color>\n\n" +

                           "<color=#000000>DeathModifier:</color> <color=lime>" + deathModifier + "</color>\n" +
                           "<color=#000000>BirthModifier:</color> <color=lime>" + birthModifier + "</color>\n" +
                           "<color=#000000>FoodConsumptionModifier:</color> <color=lime>" + foodConsumptionModifier + "</color>\n\n" +

                           "<color=#000000>Unoccupied Homes:</color> <color=color=#FFFFFF>" + totalUnoccupiedHomes + "</color>\n" +
                           "<color=#000000>Occupied Homes:</color> <color=color=#FFFFFF>" + totalOccupiedHomes + "</color>\n\n" +

                           "<color=#000000>Homeless Groups:</color> <color=#FFFFFF>" + totalHomelessGroups + "</color>\n" +
                           "<color=#000000>Couple Groups:</color> <color=#FFFFFF>" + totalCouplesLookingForHomes + "</color>\n" +
                           "<color=#000000>Single Groups:</color> <color=red>" + singleLookingForHomesForHomes + "</color>\n\n"+

                           "<color=#000000>Food:</color> <color=lime>" + food + "</color>\n" +
                           "<color=#000000>Wood:</color> <color=lime>" + wood + "</color>\n" +
                           "<color=#000000>Ore:</color> <color=lime>" + ore + "</color>\n" +
                           "<color=#000000>Bonus Food:</color> <color=lime>" + bonusFood + "</color>\n" +
                           "<color=#000000>Bonus Wood:</color> <color=lime>" + bonusWood + "</color>\n" +
                           "<color=#000000>Bonus Ore:</color> <color=lime>" + bonusOre + "</color>\n" +
                           "<color=#000000>Luxury Resource:</color> <color=lime>" + luxuryResource + "</color>\n\n" +

                           "<color=#000000>Production:Consumption</color>\n" +
                           "<color=#000000>Food:</color> <color=lime>" + foodProductionRate + "</color> : <color=red>" + foodConsumptionRate + "</color>\n" +
                           "<color=#000000>Wood:</color> <color=lime>" + woodProductionRate + "</color> : <color=red>" + woodConsumptionRate + "</color>\n" +
                           "<color=#000000>Ore:</color> <color=lime>" + oreProductionRate + "</color> : <color=red>" + oreConsumptionRate + "</color>\n" +
                           "<color=#000000>Bonus Food:</color> <color=lime>" + bonusFoodProductionRate + "</color> : <color=red>" + bonusFoodConsumptionRate + "</color>\n" +
                           "<color=#000000>Bonus Wood:</color> <color=lime>" + bonusWoodProductionRate + "</color> : <color=red>" + bonusWoodConsumptionRate + "</color>\n" +
                           "<color=#000000>Bonus Ore:</color> <color=lime>" + bonusOreProductionRate + "</color> : <color=red>" + bonusOreConsumptionRate + "</color>\n" +
                           "<color=#000000>Luxury Resource:</color> <color=lime>" + luxuryResourceProductionRate + "</color> : <color=red>" + luxuryResourceConsumptionRate + "</color>\n\n" +

                           "<color=#000000>Total Construction in Progress:</color> <color=yellow>" + totalConstructionsInProgress + "</color>\n" +
                           "<color=#000000>Total Construction jobs Active:</color> <color=yellow>" + totalConstructionsJobsActive + "</color>\n";


        raceManager.totalDisplay.text = totalText;
    }
}
