using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZaraEngine.Diseases.Stages;
using ZaraEngine.Diseases.Stages.Fluent;
using ZaraEngine.Injuries;
using ZaraEngine.Inventory;

namespace ZaraEngine.Diseases.Treatment
{
    public class ApplianceTimedTreatmentNode
    {

        private readonly List<ApplianceTimedTreatment> _treatments = new List<ApplianceTimedTreatment>();

        private bool _isOverallHealingStarted;

        private readonly DiseaseLevels _treatedLevel;

        public ApplianceTimedTreatmentNode(DiseaseLevels treatedLevel, params ApplianceTimedTreatment[] treatments)
        {
            _treatedLevel = treatedLevel;

            foreach (var t in treatments)
            {
                t.SetTreatedLevel(treatedLevel);
            }

            _treatments.AddRange(treatments);

            foreach (var treatment in treatments)
            {
                treatment.IsNodePart = true;
            }
        }

        public bool OnApplianceTaken(IGameController gc, ApplianceInfo applianceInfo, ActiveDisease disease)
        {
            var treatedStage = disease.TreatedStage;

            if (treatedStage == null)
                treatedStage = disease.GetActiveStage(gc.WorldTime.Value);

            if (treatedStage == null)
                return false;

            var isTreatedLevel = treatedStage.Level == _treatedLevel;
            var isApplied = _treatments.Any(treatment => treatment.OnApplianceTaken(gc, applianceInfo, disease));

            if (isTreatedLevel)
            {
                if (_treatments.All(x => x.IsFinished))
                {
                    _isOverallHealingStarted = true;

                    disease.Invert();

                    disease.DeclareDiseaseTreated();

                    Events.NotifyAll(l => l.DiseaseHealed(disease.Disease));

                    return true;
                }

                if (_treatments.All(x => x.IsStarted) && !_isOverallHealingStarted)
                {
                    _isOverallHealingStarted = true;

                    disease.Invert();

                    Events.NotifyAll(l => l.DiseaseTreatmentStarted(disease.Disease));

                    return true;
                }

                if (isApplied && _isOverallHealingStarted)
                {
                    Events.NotifyAll(l => l.DiseaseHealingContinued(disease.Disease));
                }
            }

            return isApplied;
        }

        public void Check(ActiveDisease disease, IGameController gc)
        {
            var treatedStage = disease.TreatedStage;

            if (treatedStage == null)
                treatedStage = disease.GetActiveStage(gc.WorldTime.Value);

            if (treatedStage == null)
                return;

            var isTreatedLevel = treatedStage.Level == _treatedLevel;

            _treatments.ForEach(x => x.Check(disease, gc));

            if (isTreatedLevel)
            {
                if (_treatments.Any(x => x.IsFailed))
                {
                    _treatments.ForEach(x => x.IsFailed = false);

                    Events.NotifyAll(l => l.DiseaseStartProgressing(disease.Disease));
                }
            }
        }

        public void Reset()
        {
            _treatments.ForEach(x => x.Reset());
        }

    }
}
