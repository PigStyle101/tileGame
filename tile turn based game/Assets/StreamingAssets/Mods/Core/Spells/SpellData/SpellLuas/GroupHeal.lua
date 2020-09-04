local cost = 5
local heal = 5
local directions {}

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
	if Cunit.Mana >= cost then
		for k,v in pairs(directions)
			if GCS.UnitPos.ContainsKey(Cunit.Position + v) then
				unit = GCS.UnitPos[Cunit.Position + v].GetComponent("UnitController");
				if unit.Team == Cunit.Team then
					unit.DoHeal(heal);
					GCS.UpdateUnitHealthText(unit.Position);
					for k,v in pairs(GCS.UnitPos)
						LM.ChangeSpriteColor(v,1,1,1);
					end
					GCS.WaitActionPlayScene();
					PSCC.AttackButtonSelected = false;
					PSCC.SetActionButtonsToFalse();
					PSCC.HideOrShowSaveButton(false);
				end
			end
		end
	end
end

