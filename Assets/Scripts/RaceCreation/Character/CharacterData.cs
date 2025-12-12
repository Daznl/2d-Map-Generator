using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterData
{
    public Gender gender { get; set; }
    public string name { get; set; }
    public string race { get; set; }
    public Date birthDate { get; set; }
    public Date deathDate { get; set; }
    public int age { get; set; }
    public int lifeSpan { get; set; }

    public Job job { get; set; }

    public LivingGroup LivingGroup { get; set; }

    public CharacterData(string _name, Gender _gender, string _race, Date _birthDate, bool _homeless, Building _currentHome = null, LivingGroup _livingGroup = null)
    {
        name = _name;
        gender = _gender;
        race = _race;
        birthDate = _birthDate;
        deathDate = new Date(); // Assuming a default value is suitable
        age = 0; // This could be calculated based on the birthDate and the current date
        job = null;
        LivingGroup = _livingGroup;
    }
}
