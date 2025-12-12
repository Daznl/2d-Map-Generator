using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacePreference : MonoBehaviour
{
    public enum RaceLandPreference
    {
        Lowland, Land, Highland, Mountain, Desert, Swamp
    }

    public Dictionary<RaceLandPreference, Dictionary<MapArrayScript.Blocktype, int>> LandPreferences =
        new Dictionary<RaceLandPreference, Dictionary<MapArrayScript.Blocktype, int>>()
    {
        {
            RaceLandPreference.Lowland,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Lowland, 20 },
                { MapArrayScript.Blocktype.Land, 10 }
            }
        },
        {
            RaceLandPreference.Land,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Land, 20 },
                { MapArrayScript.Blocktype.Lowland, 10 },
                { MapArrayScript.Blocktype.Hill, 10 }
            }
        },
        {
            RaceLandPreference.Highland,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Highland, 20 },
                { MapArrayScript.Blocktype.Hill, 10 },
                { MapArrayScript.Blocktype.Mountain, 10 },
                { MapArrayScript.Blocktype.Peak, 5 }
            }
        },
        {
            RaceLandPreference.Mountain,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Highland, 5 },
                { MapArrayScript.Blocktype.Mountain, 20 },
                { MapArrayScript.Blocktype.Peak, 30 }
            }
        },
        {
            RaceLandPreference.Desert,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Sand, 20 },
                { MapArrayScript.Blocktype.DryLand, 100 }
            }
        },
        {
            RaceLandPreference.Swamp,
            new Dictionary<MapArrayScript.Blocktype, int>
            {
                { MapArrayScript.Blocktype.Swamp, 20 }
            }
        },
    };

}
