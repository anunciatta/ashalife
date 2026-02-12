using TMPro;
using UnityEngine;

public class StatusController : MonoBehaviour
{
   [SerializeField] private StatusSlider healthSlider;
   [SerializeField] private StatusSlider energySlider;
   [SerializeField] private StatusSlider experienceSlider;

   private void Awake()
   {
      Bus<OnCharacterLoad>.OnEvent += OnCharacterLoad;

      Bus<OnHealthChange>.OnEvent += OnHealthChange;
      Bus<OnExperienceChange>.OnEvent += OnExperienceChange;
      Bus<OnEnergyChange>.OnEvent += OnEnergyChange;

      Bus<OnHealthMaxValue>.OnEvent += OnHealthMaxValue;
      Bus<OnExperienceMaxValue>.OnEvent += OnExperienceMaxValue;
      Bus<OnEnergyMaxValue>.OnEvent += OnEnergyMaxValue;
   }

   private void OnDestroy()
   {
      Bus<OnCharacterLoad>.OnEvent -= OnCharacterLoad;
      Bus<OnHealthChange>.OnEvent -= OnHealthChange;
      Bus<OnExperienceChange>.OnEvent -= OnExperienceChange;
      Bus<OnEnergyChange>.OnEvent -= OnEnergyChange;
      Bus<OnHealthMaxValue>.OnEvent -= OnHealthMaxValue;
      Bus<OnExperienceMaxValue>.OnEvent -= OnExperienceMaxValue;
      Bus<OnEnergyMaxValue>.OnEvent -= OnEnergyMaxValue;
   }

   private void OnCharacterLoad(OnCharacterLoad data)
   {
      healthSlider.SetMaxValue(data.Character.statuses[(int)healthSlider.status].maxValue);
      energySlider.SetMaxValue(data.Character.statuses[(int)energySlider.status].maxValue + References.Instance.inventory.GetAllEquippedItensModifiers(data.Character)[2]);
      experienceSlider.SetMaxValue(References.Instance.experienceConfigurations.GetXpForNextLevel(data.Character.level));

      healthSlider.SetCurrentValue(data.Character.statuses[(int)healthSlider.status].currentValue);
      energySlider.SetCurrentValue(data.Character.statuses[(int)energySlider.status].currentValue);
      experienceSlider.SetCurrentValue(data.Character.statuses[(int)experienceSlider.status].currentValue);
   }

   private void OnEnergyChange(OnEnergyChange data)
   {
      energySlider.SetCurrentValue(data.NewValue);
   }

   private void OnHealthChange(OnHealthChange data)
   {
      healthSlider.SetCurrentValue(data.NewValue);
      //Check for death
   }

  
   private void OnExperienceChange(OnExperienceChange data)
   {
      experienceSlider.SetCurrentValue(data.NewValue);
      //Check for level up
   }

   private void OnHealthMaxValue(OnHealthMaxValue data)
   {
      healthSlider.SetMaxValue(data.Value);
   }

   private void OnEnergyMaxValue(OnEnergyMaxValue data)
   {
      energySlider.SetMaxValue(data.Value);
   }

   private void OnExperienceMaxValue(OnExperienceMaxValue data)
   {
      experienceSlider.SetMaxValue(data.Value);
   }

}


