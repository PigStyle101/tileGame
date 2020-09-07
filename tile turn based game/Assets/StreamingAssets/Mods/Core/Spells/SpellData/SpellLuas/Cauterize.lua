local cost = 5
local heal = 5

function CastSpell(Target,Caster)
Cunit = Caster.GetComponent("UnitController");
  if Cunit.Mana >= cost;
	unit = Target.GetComponent("UnitController");
	unit.DoHeal(heal);
	GCS.UpdateUnitHealthText(Target.transform.position);
	for k,v in pairs(GCS.UnitPos)
		LM.ChangeSpriteColor(v,1,1,1);
	end
	GCS.WaitActionPlayScene();
	PSCC.AttackButtonSelected = false;
	PSCC.SetActionButtonsToFalse();
	PSCC.HideOrShowSaveButton(false);
  end
end