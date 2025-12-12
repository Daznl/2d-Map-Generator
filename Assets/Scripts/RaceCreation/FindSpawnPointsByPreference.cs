using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MapArrayScript;
using static RacePreference;

public class FindSpawnPointsByPreference : MonoBehaviour
{
    public SpawnPointFilterFunctions S;
    public GameManager gameManager;

    public void initialize(GameManager inGameManager)
    {
        gameManager = inGameManager;
        S = new SpawnPointFilterFunctions();
        S.Initialize(inGameManager);
        
    }
    public struct LandTypeScores
    {
        public int Lowland;
        public int Land;
        public int Hill;
        public int Sand;
        public int Mountain;
        public int Highland;
        public int Peak;
        public int DryLand;
        public int Swamp;
        public int Swamplowland;

        // Constructor
        public LandTypeScores(int lowland, int land, int hill, int sand, int mountain, int highland, int peak, int dryLand, int swamp, int swamplowland)
        {
            Lowland = lowland;
            Land = land;
            Hill = hill;
            Sand = sand;
            Mountain = mountain;
            Highland = highland;
            Peak = peak;
            DryLand = dryLand;
            Swamp = swamp;
            Swamplowland = swamplowland;
        }
    }
    public struct ResourceScores
    {
        public float MainFoodMultiplier;
        public float BonusFoodMultiplier;
        public float MainWoodMultiplier;
        public float BonusWoodMultiplier;
        public float MainOreMultiplier;
        public float BonusOreMultiplier;
        public float LuxuryResourceMultiplier;

        public ResourceScores(float mainFood, float bonusFood, float mainWood, float bonusWood,
                              float mainOre, float bonusOre, float luxuryResource)
        {
            MainFoodMultiplier = mainFood;
            BonusFoodMultiplier = bonusFood;
            MainWoodMultiplier = mainWood;
            BonusWoodMultiplier = bonusWood;
            MainOreMultiplier = mainOre;
            BonusOreMultiplier = bonusOre;
            LuxuryResourceMultiplier = luxuryResource;
        }
    }
    public struct WaterFeatureScores
    {
        public int RiverScore;
        public int LakeScore;
        public int CoastScore;

        public WaterFeatureScores(int riverScore, int lakeScore, int coastScore)
        {
            RiverScore = riverScore;
            LakeScore = lakeScore;
            CoastScore = coastScore;
        }
    }

    private int CalculateInitialScore(GenericCoordinate block, LandTypeScores landScores, WaterFeatureScores waterScores)
    {
        int score = 0;
        score += S.CalculateWaterFeatureScore(block, waterScores);
        score += S.ScoreForLandType(gameManager.LoadedWorld.worldBlockTypesArray[block.x, block.y], landScores);
        return score;
    }

    public List<CoordinateWithAmount> FindSpawnPoints(HashSet<GenericCoordinate> territory, LandTypeScores landScores, ResourceScores resourceScores, WaterFeatureScores waterScores)
    {
        List<CoordinateWithAmount> spawnPoints = territory.Select(block =>
            new CoordinateWithAmount(block.x, block.y, CalculateInitialScore(block, landScores, waterScores))
        ).ToList();

        // Only keep top 50 for detailed evaluation
        var topSpawnPoints = spawnPoints.OrderByDescending(sp => sp.amount).Take(50).ToList();

        // Apply detailed resource scoring
        topSpawnPoints.ForEach(point =>
            point.amount += S.ScoreResourcesAroundBlock(new GenericCoordinate(point.x, point.y), resourceScores)
        );

        // Narrow down to top 10 after resource scoring
        var finalTopSpawnPoints = topSpawnPoints.OrderByDescending(sp => sp.amount).Take(5).ToList();

        return finalTopSpawnPoints;
    }


    public List<CoordinateWithAmount> FindSpawnPointsForLowland(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores lowlandScores = new LandTypeScores(
            lowland: 100, land: 80, hill: 50, sand: 40, mountain: 10, highland: 20, peak: 5, dryLand: 0, swamp: 20, swamplowland: 30);

        ResourceScores resourceScores = new ResourceScores(
            mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);

        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, lowlandScores, resourceScores, waterScores);
    }

    public List<CoordinateWithAmount> FindSpawnPointsForLand(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores landScores = new LandTypeScores(
            lowland: 80, land: 100, hill: 80, sand: 30, mountain: 20, highland: 20, peak: 5, dryLand: 20, swamp: 20, swamplowland: 20);

        ResourceScores resourceScores = new ResourceScores(
            mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);

        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, landScores, resourceScores, waterScores);
    }

    public List<CoordinateWithAmount> FindSpawnPointsForHighland(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores landScores = new LandTypeScores(
            lowland: 10, land: 50, hill: 60, sand: 40, mountain: 80, highland: 100, peak: 50, dryLand: 0, swamp: 0, swamplowland: 0);

        ResourceScores resourceScores = new ResourceScores(
            mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);

        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, landScores, resourceScores, waterScores);
    }

    public List<CoordinateWithAmount> FindSpawnPointsForMountain(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores landScores = new LandTypeScores(
            lowland: 0, land: 40, hill: 50, sand: 10, mountain: 100, highland: 60, peak: 80, dryLand: 20, swamp: 20, swamplowland: 10);

        ResourceScores resourceScores = new ResourceScores(
            mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);

        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, landScores, resourceScores, waterScores);
    }

    public List<CoordinateWithAmount> FindSpawnPointsForDesert(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores landScores = new LandTypeScores(
            lowland: 80, land: 60, hill: 50, sand: 100, mountain: 10, highland: 50, peak: 5, dryLand: 100, swamp: 20, swamplowland: 30);

        ResourceScores resourceScores = new ResourceScores(
           mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);
        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, landScores, resourceScores, waterScores);
    }

    public List<CoordinateWithAmount> FindSpawnPointsForSwamp(HashSet<GenericCoordinate> territory)
    {
        LandTypeScores landScores = new LandTypeScores(
            lowland: 40, land: 20, hill: 10, sand: 5, mountain: 10, highland: 20, peak: 5, dryLand: 0, swamp: 100, swamplowland: 80);

        ResourceScores resourceScores = new ResourceScores(
            mainFood: 100, bonusFood: 50, mainWood: 50, bonusWood: 25, mainOre: 50, bonusOre: 25, luxuryResource: 25);

        WaterFeatureScores waterScores = new WaterFeatureScores(
            riverScore: 100, lakeScore: 50, coastScore: 15);

        return FindSpawnPoints(territory, landScores, resourceScores, waterScores);
    }
}
