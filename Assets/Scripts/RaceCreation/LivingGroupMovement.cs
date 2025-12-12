using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static LivingTogetherManager;

public class LivingGroupMovement
{
    public void MoveGroupsIntoHomes(RaceManager raceManager)
    {
        var sortedGroups = SortGroupsByType(raceManager);

        MoveInHomelessGroup(raceManager, sortedGroups, raceManager.buildingManager.StartingTown);
        MoveCoupleIntoHome(raceManager, raceManager.buildingManager.StartingTown);
        MoveSinglesIntoHomes(raceManager, raceManager.buildingManager.StartingTown);
    }

    public void MoveInHomelessGroup(RaceManager raceManager, List<LivingGroup> sortedGroups, Town town)
    {
        Building availableHome = town.UnoccupiedHouse.FirstOrDefault();
        if (availableHome != null)
        {
            foreach (var group in sortedGroups)
            {
                // Only consider unhoused groups
                if (!group.IsHoused)
                {
                    // Associate the group with the building
                    availableHome.SetOccupantGroup(group);

                    // Find the town containing the available home and mark the home as occupied
                    town.OccupyHouse(availableHome);

                    // Since a group has been housed, break the loop to re-evaluate remaining groups next cycle
                    break;
                }
            }
        }
    }
    public void MoveCoupleIntoHome(RaceManager raceManager, Town town)
    {
        Building availableHome = town.UnoccupiedHouse.FirstOrDefault();
        if (availableHome != null)
        {
            if (raceManager.CoupleLookingForHome.Any())
            {
                foreach (var couple in raceManager.CoupleLookingForHome.ToList())
                {
                    // Retrieve all descendants of the couple
                    var descendants = GetDescendants(couple.characters);

                    // Find the current living group of the couple
                    var currentLivingGroup = couple.characters.First().Data.LivingGroup;

                    // Filter the current living group for members that are either part of the couple or their descendants
                    var membersToMove = currentLivingGroup.Members.Where(member => couple.characters.Contains(member) || descendants.Contains(member)).ToList();

                    MoveCharactersIntoHome(raceManager, membersToMove, availableHome, town);

                    // Remove the couple from the CoupleLookingForHome list
                    raceManager.CoupleLookingForHome.Remove(couple);
                }
            }
        }
    }

    public void MoveSinglesIntoHomes(RaceManager raceManager, Town town)
    {
        HandleSinglesAlreadyAloneInHouse(raceManager);

        Building availableHome = town.UnoccupiedHouse.FirstOrDefault();
        if (availableHome != null)
        {
            if (raceManager.SingleLookingForHome.Any())
            {
                foreach (var single in raceManager.SingleLookingForHome.ToList()) // Use ToList to safely modify the collection while iterating
                {

                    List<Character> singleCharacterList = new List<Character>();
                    singleCharacterList.Add(single);

                    MoveCharactersIntoHome(raceManager, singleCharacterList, availableHome, town);

                    // Remove the single from the SingleLookingForHome list
                    raceManager.SingleLookingForHome.Remove(single);
                }
            }
        }
    }

    public void MoveCharactersIntoHome(RaceManager raceManager, List<Character> list, Building home, Town town)
    {
        // Create a new LivingGroup for the members to move and associate it with the available home
        var newLivingGroup = new LivingGroup(list, true, home);
        raceManager.livingTogetherManager.AddLivingGroup(newLivingGroup);

        // Associate the new living group with the available home
        home.SetOccupantGroup(newLivingGroup);

        town.OccupyHouse(home);

        // Move each member to the new living group, ensuring they are removed from any previous group
        foreach (var member in list)
        {
            raceManager.livingTogetherManager.MoveCharacterBetweenGroups(member, newLivingGroup);
        }
    }

    public void HandleSinglesAlreadyAloneInHouse(RaceManager raceManager)
    {
        foreach (var single in raceManager.SingleLookingForHome.ToList()) // Use ToList to safely modify the collection while iterating
        {
            // Check if the single is already housed by themselves
            if (single.Data.LivingGroup != null && single.Data.LivingGroup.Members.Count == 1 && single.Data.LivingGroup.IsHoused)
            {
                // Single is already housed by themselves, remove them from the singles list and skip to the next single
                raceManager.SingleLookingForHome.Remove(single);
                continue; // Skip the rest of the loop and move to the next single
            }
        }
    }

    public List<Character> GetDescendants(List<Character> couple)
    {
        var descendants = new List<Character>();
        var queue = new Queue<Character>(couple);

        while (queue.Count > 0)
        {
            var currentCharacter = queue.Dequeue();
            descendants.Add(currentCharacter);

            foreach (var child in currentCharacter.FamilyRelations.Children)
            {
                queue.Enqueue(child);
            }
        }

        return descendants;
    }
    public List<LivingGroup> SortGroupsByType(RaceManager raceManager)
    {
        // Assuming LivingTogetherManager is accessible via raceManager
        var livingGroups = raceManager.livingTogetherManager.LivingGroups;

        // Sort by IsHoused status first (false comes before true), then by any other criteria
        var sortedGroups = livingGroups.OrderBy(group => group.IsHoused).ToList();

        return sortedGroups;
    }

    public void UpdateHouseOccupancyStatus(RaceManager raceManager, Town town)
    {
/*        foreach (var town in raceManager.buildingManager.Towns)
        {*/
            foreach (var building in raceManager.buildingManager.StartingTown.OccupiedHouse.ToList())
            {
                if (building.OccupantGroup == null || building.OccupantGroup.Members.Count == 0)
                {
                    // The building is now unoccupied, so move it to the unoccupied list
                    town.UnoccupiedHouse.Add(building);
                    town.OccupiedHouse.Remove(building);

                    // If the building was previously associated with a living group, update that group's status
                    if (building.OccupantGroup != null)
                    {
                        building.OccupantGroup.IsHoused = false;
                        building.OccupantGroup.CurrentHome = null;
                        building.ClearOccupantGroup(); // Clear the association
                    }
                }
            }

    }

}
