local cost = 1
local damage = 1

function CastSpell(Target,Caster)
Cunit = Caster.GetComponent("UnitController");
  if Cunit.HClass.Mana >= cost then
	unit = Target.GetComponent("UnitController");
	dead = unit.DoDamage(damage);
	if dead then
		GCS.KillUnitPlayScene(Target);
		unit.StartDeadAmination();
		Cunit.KilledEnemyUnit(unit);
	else
		GCS.UpdateUnitHealthText(unit.position);
	end
	for k,v in pairs(GCS.UnitPos) do
		LM.ChangeSpriteColor(v,1,1,1);
	end
  end
end