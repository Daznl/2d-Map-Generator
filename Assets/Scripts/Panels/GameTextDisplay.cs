using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class GameTextDisplay : MonoBehaviour
{
    public RaceDataHolder raceDataHolder;
    public int selectedRaceIndex;
    public TimeCounter timeCounter;
    public LastActionPerformed lastActionPerformed = LastActionPerformed.None;
    private Text displayText;
    public enum LastActionPerformed
    {
        None,
        ShowCharacterList,
        ShowRandomCharacterDetails
    }

    void Start()
    {
        displayText = GetComponent<Text>();

        // Initialize selectedRaceIndex with -1 (no race selected)
        selectedRaceIndex = -1;

        // Subscribe to the TimeCounter's yearly event
        //timeCounter.OnYearIncremented += UpdateCharacterList;
        timeCounter.OnYearIncremented += UpdateRandomCharacterDetails;
    }

    void OnDestroy()
    {
        // Unsubscribe from the TimeCounter's yearly event
        timeCounter.OnYearIncremented -= UpdateRandomCharacterDetails;
    }
    public void ShowRaceManagerCharacters(int index)
    {
        selectedRaceIndex = index;
        lastActionPerformed = LastActionPerformed.ShowCharacterList;
    }

    public void DisplayCharacterList()
    {
        displayText.text = GetCharacterListString();
    }

    private void UpdateCharacterList()
    {
        if (lastActionPerformed == LastActionPerformed.ShowCharacterList)
        {
            displayText.text = GetCharacterListString();
        }
    }

    private void UpdateRandomCharacterDetails()
    {
        switch (lastActionPerformed)
        {
            case LastActionPerformed.ShowCharacterList:
                UpdateCharacterList();
                break;
            case LastActionPerformed.ShowRandomCharacterDetails:
                ShowRandomCharacterDetails();
                break;
                // ... add more cases for other actions as needed
        }
    }

    public string GetRandomCharacterDetails(RaceManager raceManager)
    {
        if (raceManager.aliveCharacters.Characters.Count + raceManager.deadCharacters.Characters.Count == 0)
        {
            return "No characters available.";
        }

        //create a list of all the characters dead and alive
        List<Character> allCharacters = new List<Character>();
        allCharacters.AddRange(raceManager.aliveCharacters.Characters);
        allCharacters.AddRange(raceManager.deadCharacters.Characters);

        Character randomCharacter = allCharacters[Random.Range(0, allCharacters.Count)];
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Name: {randomCharacter.Data.name}");
        sb.AppendLine($"Gender: {randomCharacter.Data.gender}");
        sb.AppendLine($"Age: {randomCharacter.Data.age}");

        bool isAlive = raceManager.aliveCharacters.Characters.Contains(randomCharacter);
        sb.AppendLine($"Is Alive: {isAlive}");

        // Convert LinkDates list to a string
        string linkDatesStr = string.Join(", ", randomCharacter.LinkManager.LinkDates.Select(date => date.ToString()));
        sb.AppendLine($"Link Dates: {linkDatesStr}");

        sb.AppendLine($"BirthDate: <color=#FFFFFF>{randomCharacter.Data.birthDate.ToString()}</color>");
        sb.AppendLine($"deathDate: <color=#FFFFFF>{randomCharacter.Data.deathDate.ToString()}</color>");

        // Add parent names
        string parentNames = string.Join(", ", randomCharacter.FamilyRelations.Parents.Select(p => p.Data.name));
        sb.AppendLine($"<color=yellow>Parents:</color> {parentNames}");

        // Add sibling names
        string siblingNames = string.Join(", ", randomCharacter.FamilyRelations.Siblings.Select(s => s.Data.name));
        sb.AppendLine($"<color=red>Siblings:</color> {siblingNames}");

        // Add sibling names
        string stepSiblingNames = string.Join(", ", randomCharacter.FamilyRelations.StepSiblings.Select(s => s.Data.name));
        sb.AppendLine($"<color=red>StepSiblings:</color> {stepSiblingNames}");

        // Add child names
        string childNames = string.Join(", ", randomCharacter.FamilyRelations.Children.Select(c => c.Data.name));
        sb.AppendLine($"<color=orange>Children:</color> {childNames}");

        // Add linked character name
        if (randomCharacter.LinkManager.LinkedCharacter != null)
        {
            sb.AppendLine($"<color=lightblue>Linked Character:</color> {randomCharacter.LinkManager.LinkedCharacter.Data.name}");
        }
        else
        {
            sb.AppendLine("Linked Character: None");
        }

        if (randomCharacter.Data.job != null)
        {
            sb.AppendLine($"<color=green>Job:</color> {randomCharacter.Data.job.JobName}");
        }
        else
        {
            sb.AppendLine("Job: Unemployed");
        }

        if (randomCharacter.Data.LivingGroup != null && randomCharacter.Data.LivingGroup.Members.Any())
        {
            // Access the members of the LivingGroup and join their names for display
            string livingWithNames = string.Join(", ", randomCharacter.Data.LivingGroup.Members.Select(member => member.Data.name));
            sb.AppendLine($"<color=purple>Living With:</color> {livingWithNames}");
        }
        else
        {
            sb.AppendLine("Living With: Alone");
        }

        // ... add any other properties you want to display

        return sb.ToString();
    }
    public void ShowRandomCharacterDetails()
    {
        RaceManager currentRaceManager = raceDataHolder.GetCurrentRaceManager();
        string randomCharacterDetails = GetRandomCharacterDetails(currentRaceManager);
        displayText.text = randomCharacterDetails;
        lastActionPerformed = LastActionPerformed.ShowRandomCharacterDetails;
    }

    private string GetCharacterListString()
    {
        RaceManager raceManager = raceDataHolder.raceManagersList[selectedRaceIndex];
        // Check if raceDataHolder or raceManagersList is null
        if (raceDataHolder == null || raceDataHolder.raceManagersList == null)
        {
            Debug.LogError("RaceDataHolder or raceManagersList is null.");
            return "Initialization error: Data holder or list is null.";
        }

        if (selectedRaceIndex >= 0 && selectedRaceIndex < raceDataHolder.raceManagersList.Count)
        {
            if (raceManager.aliveCharacters.Characters.Count == 0)
            {
                return "No character in list.";
            }
            

            List<Character> allCharacters = new List<Character>(raceManager.aliveCharacters.Characters);
            allCharacters.AddRange(raceManager.deadCharacters.Characters);

            string characterList = "";

            foreach (Character character in allCharacters)
            {
                bool isAlive = raceManager.aliveCharacters.Characters.Contains(character);
                string statusText = isAlive ? "<color=white>Alive</color>" : "<color=black>Dead</color>";

                bool isAdult = raceManager.aliveCharacters.Adults.Contains(character) || raceManager.deadCharacters.Adults.Contains(character);
                string ageStatus = isAdult ? "<color=yellow>Adult</color>" : "<color=orange>Child</color>";

                string genderColor = character.Data.gender == Gender.Male ? "<color=blue>Male</color>" : "<color=red>Female</color>";

                characterList += $"{character.Data.name}, {character.Data.age}, {genderColor}, {ageStatus}, Birthdate: {character.Data.birthDate.ToString()}, {statusText}\n";
            }
            return characterList;
        }
        else
        {
            return "No race selected.";
        }
    }
}