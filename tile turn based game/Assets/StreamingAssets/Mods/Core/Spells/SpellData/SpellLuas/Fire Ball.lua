local cost = 5
local damage = 5

function CastSpell(Target,Caster)
Cunit = Caster.GetComponent("UnitController");
  if Cunit.Mana >= cost;
	unit = Target.GetComponent("UnitController");
	dead = unit.DoDamage(damage);
	if dead
		GCS.KillUnitPlayScene(Target);
		unit.StartDeadAmination();
		Cunit.KilledEnemyUnit(unit);
	else
		GCS.UpdateUnitHealthText(Target.transform.position);
	end
	for k,v in pairs(GCS.UnitPos)
		LM.ChangeSpriteColor(v,1,1,1);
	end
	GCS.WaitActionPlayScene();
	PSCC.AttackButtonSelected = false;
	PSCC.SetActionButtonsToFalse();
	PSCC.HideOrShowSaveButton(false);
  end
end