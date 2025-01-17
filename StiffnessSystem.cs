using System;
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
            UpdateEntities();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode == GameMode.MainMenu)
            {
                //UpdateEntities();
            }
        }

        public void UpdateEntities()
        {
            if (VanillaStiffnesses == null || VanillaStiffnesses.Count == 0)
                VanillaStiffnesses = new Dictionary<string, SwayingData>();
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
                try
                {
                    if (EntityManager.HasComponent<SwayingData>(entity))
                    {
                        var prefabName = prefabSystem.GetPrefabName(entity);
                        var data = EntityManager.GetComponentData<SwayingData>(entity);
                        if (!VanillaStiffnesses.ContainsKey(prefabName))
                        {
                            var dataCopy = new SwayingData()
                            {
                                m_VelocityFactors = data.m_VelocityFactors,
                                m_SpringFactors = data.m_SpringFactors,
                                m_DampingFactors = data.m_DampingFactors,
                                m_MaxPosition = data.m_MaxPosition
                            };
                            Logger.Debug(
                                $"Saving vanilla stiffness for {prefabName}: Spring {data.m_SpringFactors}, Damping {data.m_DampingFactors}, MaxPosition {data.m_MaxPosition}");
                            VanillaStiffnesses.Add(prefabName, dataCopy);
                        }

                        data.m_MaxPosition = VanillaStiffnesses[prefabName].m_MaxPosition / Setting.Instance.StiffnessModifier;
                        data.m_DampingFactors = VanillaStiffnesses[prefabName].m_DampingFactors * Setting.Instance.DampingModifier;
                        EntityManager.SetComponentData(entity, data);
                        Logger.Debug(
                            $"Updated stiffness for {prefabName}: Spring {data.m_SpringFactors}, Damping {data.m_DampingFactors}, MaxPosition {data.m_MaxPosition}");
                    }
                }
                catch (Exception x)
                {
                    Logger.Error("Error updating stiffness: " + x.Message);
                }
            }
        }

        protected override void OnUpdate()
        {

        }
    }

}