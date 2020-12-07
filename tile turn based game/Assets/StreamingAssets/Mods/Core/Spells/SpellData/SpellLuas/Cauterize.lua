local cost = 1
local heal = 1

function CastSpell(Target,Caster)
Cunit = Caster.GetComponent("UnitController");
  if Cunit.HClass.Mana >= cost then
	unit = Target.GetComponent("UnitController");
	unit.DoHeal(heal);
	GCS.UpdateUnitHealthText(unit.position);
	for k,v in pairs(GCS.UnitPos) do
		LM.ChangeSpriteColor(v,1,1,1);
	end
  end
end