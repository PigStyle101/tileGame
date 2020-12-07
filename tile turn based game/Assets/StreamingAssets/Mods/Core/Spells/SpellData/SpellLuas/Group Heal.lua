local cost = 1
local heal = 1
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
				unit.DoHeal(heal);
				GCS.UpdateUnitHealthText(unit.Position);
				for k,v in pairs(GCS.UnitPos) do
					LM.ChangeSpriteColor(v,1,1,1);
				end
			end
		end
		Cunit.DoHeal(heal);
		GCS.UpdateUnitHealthText(Cunit.Position);
	else 
	Debug.Log("not enough mana");
	end
end

