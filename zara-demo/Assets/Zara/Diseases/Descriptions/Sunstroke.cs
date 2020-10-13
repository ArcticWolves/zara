using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZaraEngine.Diseases.Stages;
using ZaraEngine.Diseases.Stages.Fluent;

namespace ZaraEngine.Diseases
{
    public class Sunstroke : DiseaseDefinitionBase
    {

        public Sunstroke()
        {
            Name = "Sunstroke";
            IsDynamic = true;

            Stages = new List<DiseaseStage>(new[]
            {
                StageBuilder.NewStage().WithLevelOfSeriousness(DiseaseLevels.InitialStage)
                    .SelfHealChance(20)
                    .Vitals
                    .WithTargetBodyTemperature(37.1f)
                    .WillReachTargetsInHours(1)
                    .AndLastForHours(3)
                    .AdditionalEffects
                    .WithLowChanceOfSneeze()
                    .NoDisorders()
                    .NoDrains()
                    .Treatment
                    .WithConsumable((gc, cunsumable, disease) =>
                    {
                        return false;
                    })
                    .AndWithoutSpecialItems()
                    .Build()
            });
        }

        public override void Check(ActiveDisease disease, IGameController gc)
        {
            
        }

    }
}
