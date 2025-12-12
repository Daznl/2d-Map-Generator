using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LivingTogetherManager
{
    public List<LivingGroup> LivingGroups { get; private set; } = new List<LivingGroup>();

    public void Initilise()
    {
        LivingGroups = new List<LivingGroup>();
    }

    
    public void AddLivingGroup(LivingGroup group)
    {
        LivingGroups.Add(group);

        // Update each character's LivingWith reference to point to this new group's member list
        foreach (var character in group.Members)
        {
            character.Data.LivingGroup = group;
        }
    }

    public void AddCharacterToGroup(Character character, LivingGroup group)
    {
        if (group == null)
        {
            Debug.LogError("Attempted to add a character to a null group.");
            return;
        }

        if (!group.Members.Contains(character))
        {
            group.Members.Add(character);
            // Ensure character's LivingGroup points to the exact group
            character.Data.LivingGroup = group;
            // Optionally update character's housing status and current home
        }
        else
        {
            Debug.LogError("Character already part of the group.");
        }
    }

    public void RemoveCharacterFromGroup(Character character, LivingGroup group)
    {
        if (group.Members.Contains(character))
        {
            group.Members.Remove(character);
            // Optionally handle updating the character's LivingWith reference, housing status, and home
            character.Data.LivingGroup = null;
        }
        UpdateLivingGroupAfterRemoval(group);
    }

    public void MoveCharacterBetweenGroups(Character character, LivingGroup newGroup)
    {
        // Find the character's current living group
        //var currentGroup = LivingGroups.FirstOrDefault(g => g.Members.Contains(character));
        LivingGroup currentGroup = character.Data.LivingGroup;
        // Remove the character from their current group, if it exists and is different from the new group
        if (currentGroup != null && currentGroup != newGroup)
        {
            currentGroup.Members.Remove(character);
            // Update the current group after removal to possibly remove it if empty
            UpdateLivingGroupAfterRemoval(currentGroup);
        }

        // Add the character to the new group, ensuring they're not already a member
        if (!newGroup.Members.Contains(character))
        {
            newGroup.Members.Add(character);
        }
        // Update the character's LivingWith reference to the new group's member list
        character.Data.LivingGroup = newGroup;
    }

    public void UpdateLivingGroupAfterRemoval(LivingGroup group)
    {
        // Updated to accept a LivingGroup
        if (group.Members.Count == 0)
        {
            LivingGroups.Remove(group);
        }
    }

}
