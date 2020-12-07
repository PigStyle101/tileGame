local cost = 1
local damage = 1
local directions = {}

directions["N"] = LM.NewVector2(1,0);
directions["NE"] = LM.NewVector2(1,1);
directions["E"] = LM.NewVector2(0,1);
directions["SE"] = LM.NewVector2(-1,1);
directions["S"] = LM.NewVector2(-1,0);
directions["SW"] = LM.NewVector2(-1,-1);
directions["W"] = LM.NewVector2(0,-1);
directions["NW"] = LM.NewVector2(1,-1);

function CastSpell(Target,Caster)
	Cunit = Caster.GetComponent("UnitController");
	if Cunit.HClass.Mana >= cost then
		for k,v in pairs(directions) do
			if LM.DictionaryContainsKey(GCS.UnitPos,Cunit.Position + v) then
				unit = GCS.UnitPos[Cunit.Position + v].GetComponent("UnitController");
				dead = unit.DoDamage(damage);
				if dead then
					GCS.KillUnitPlayScene(GCS.UnitPos[unit.Position]);
					unit.StartDeadAmination();
					Cunit.KilledEnemyUnit(unit);
				else
					GCS.UpdateUnitHealthText(unit.Position);
				end
				for k,v in pairs(GCS.UnitPos) do
					LM.ChangeSpriteColor(v,1,1,1);
				end
			end
		end
		dead = Cunit.DoDamage(damage);
		if dead then
			GCS.KillUnitPlayScene(GCS.UnitPos[Cunit.Position]);
			unit.StartDeadAmination();
			Cunit.KilledEnemyUnit(Cunit);
		else
			GCS.UpdateUnitHealthText(Cunit.Position);
		end
	end
end