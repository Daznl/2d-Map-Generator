using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FamilyRelations
{
    public List<Character> Parents { get; set; } = new List<Character>();
    public List<Character> Children { get; set; } = new List<Character>();
    public List<Character> Siblings { get; set; } = new List<Character>();
    public List<Character> StepSiblings { get; set; } = new List<Character>();

    public void AddSiblings(Character newCharacter, Character parent1, Character parent2)
    {
        if (parent1 != null && parent2 != null)
        {
            var commonChildren = parent1.FamilyRelations.Children.Intersect(parent2.FamilyRelations.Children).ToList();
            foreach (var sibling in commonChildren)
            {
                if (sibling != newCharacter)
                {
                    newCharacter.FamilyRelations.Siblings.Add(sibling);
                    sibling.FamilyRelations.Siblings.Add(newCharacter);
                }
            }
        }
    }

    public void AddStepSiblings(Character newCharacter, Character parent1, Character parent2)
    {
        if (parent1 != null && parent2 != null)
        {
            var parent1UniqueChildren = parent1.FamilyRelations.Children.Except(parent2.FamilyRelations.Children).ToList();
            var parent2UniqueChildren = parent2.FamilyRelations.Children.Except(parent1.FamilyRelations.Children).ToList();


            var stepSiblings = parent1UniqueChildren.Union(parent2UniqueChildren).ToList();

            foreach (var stepSibling in stepSiblings)
            {
                if (stepSibling != newCharacter)
                {
                    newCharacter.FamilyRelations.StepSiblings.Add(stepSibling);
                    stepSibling.FamilyRelations.StepSiblings.Add(newCharacter);

                    // Debugging: Print when a step sibling is added
                    Debug.Log($"Adding Step Sibling: {newCharacter.Data.name} <-> {stepSibling.Data.name}");
                }
            }

            // Debugging: Check for characters not in both lists
            var notInBothLists = parent1UniqueChildren.Concat(parent2UniqueChildren).Except(parent1UniqueChildren.Intersect(parent2UniqueChildren)).ToList();
            foreach (var character in notInBothLists)
            {
                // Debug information about the character
                Debug.LogError($"Character Not In Both Lists: {character.Data.name}, Parent1: {parent1.Data.name}, Parent2: {parent2.Data.name}");
            }
        }
    }

    public bool IsSibling(Character other)
    {
        return Siblings.Contains(other);
    }
}