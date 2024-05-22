using System.Collections.Generic;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace StifferVehicles
{
    public partial class StiffnessSystem : GameSystemBase
    {
        private static ILog Logger;

        private EntityQuery query;

        //private UIUpdateState uiUpdateState;
        private PrefabSystem prefabSystem;
        public static StiffnessSystem Instance { get; private set; }
        public static Dictionary<string, SwayingData> VanillaStiffnesses;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            Enabled = true;
            Logger = Mod.log;

            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<CarData>(),
                ]
            };
            query = GetEntityQuery(desc);
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode == GameMode.MainMenu)
            {
                UpdateEntities();
            }
        }

        public void UpdateEntities()
        {
            bool isVanillaStiffnessesEmpty = VanillaStiffnesses == null || VanillaStiffnesses.Count == 0;
            if (isVanillaStiffnessesEmpty)
            {
                VanillaStiffnesses = new Dictionary<string, SwayingData>();
            }
            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<CarData>(),
                ]
            };
            query = GetEntityQuery(desc);
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (EntityManager.HasComponent<SwayingData>(entity))
                {
                    var prefabName = prefabSystem.GetPrefabName(entity);
                    var data = EntityManager.GetComponentData<SwayingData>(entity);
                    if (isVanillaStiffnessesEmpty)
                    {
                        var dataCopy = new SwayingData()
                        {
                            m_VelocityFactors = data.m_VelocityFactors,
                            m_SpringFactors = data.m_SpringFactors,
                            m_DampingFactors = data.m_DampingFactors,
                            m_MaxPosition = data.m_MaxPosition
                        };
                        VanillaStiffnesses.Add(prefabName, dataCopy);
                    }
                    data.m_SpringFactors = VanillaStiffnesses[prefabName].m_SpringFactors * Setting.Instance.SpringModifier;
                    data.m_DampingFactors = VanillaStiffnesses[prefabName].m_DampingFactors / Setting.Instance.DampingModifier;
                    data.m_MaxPosition = VanillaStiffnesses[prefabName].m_MaxPosition / Setting.Instance.MaxPosition;
                    EntityManager.SetComponentData(entity, data);
                    Logger.Info($"Updated stiffness for {prefabName}: Spring {data.m_SpringFactors}, Damping {data.m_DampingFactors}, MaxPosition {data.m_MaxPosition}");
                }
            }
        }

        protected override void OnUpdate()
        {

        }
    }

}