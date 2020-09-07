local cost = 1
local heal = 5
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
	Debug.Log("Starting Casting");
	Cunit = Caster.GetComponent("UnitController");
	if Cunit.HClass.Mana >= cost then
		Debug.Log("Starting 1st if");
		for k,v in pairs(directions) do
			Debug.Log("Starting for if");
			if GCS.UnitPos.ContainsKey(Cunit.Position + v) then    ----something is wronge here, it flages for a nil value
				Debug.Log("Starting 2nd if");
				unit = GCS.UnitPos[Cunit.Position + v].GetComponent("UnitController");
				if unit.Team == Cunit.Team then
					Debug.Log("Starting 3rd if");
					unit.DoHeal(heal);
					GCS.UpdateUnitHealthText(unit.Position);
					for k,v in pairs(GCS.UnitPos) do
						LM.ChangeSpriteColor(v,1,1,1);
					end
					GCS.WaitActionPlayScene();
					PSCC.AttackButtonSelected = false;
					PSCC.SetActionButtonsToFalse();
					PSCC.HideOrShowSaveButton(false);
				end
			end
		end
	else 
	Debug.Log("not enough mana");
	end
end

