
--Debug.Log("Starting");
vecCompBool = LM.VectorCompair(GCS.originalPositionOfUnit,LM.NewVector2(-1,-1));
if vecCompBool == false then
	GCS.originalPositionOfUnit = LM.NewVector2(-1,-1)
end

unit = GCS.SelectedUnitPlayScene.GetComponent("UnitController");
unit.UnitMovable = true
--Debug.Log("Point 1");

for k,v in pairs(GCS.UnitPos) do
	LM.ChangeSpriteColor(v,1,1,1);
	unit2 = v.GetComponent("UnitController");
	unit2.MovementController();
	unit2.GetSightTiles();
end
--Debug.Log("Point 2");
for k,v in pairs(GCS.TilePos) do
	LM.ChangeSpriteColor(v,1,1,1);
	LM.ChangeChildSpriteColor(v,1,1,1,1);
	terrain = v.GetComponent("TerrainController");
	terrain.FogOfWarController();
end
--Debug.Log("Finalizing");
GCS.SelectedUnitPlayScene = null
PSCC.AttackButtonSelected = false
PSCC.SetActionButtonsToFalse();


--[[

This is a example script.
When the player clicks the cancel button after they have selected a unit or if they click the unit again then the game would normally run the GCS.CancelActionPlayScene function.
If there is a lua script with that name in the GameLocation/StreamingAssets/LuaCoreScript it will instead run that.

Script that can be lua are as follows:
KillUnitPlayScene
CancelActionPlayScene
WaitActionPlayScene
CaptureActionPlayScene
MoveActionPlayScene
AttackActionPlayScene

Scripts that can be accessed through lua are as follows:

Script Name                             What its called in lua
GameControllerScript					GCS
LuaMethods								LM
MapEditorMenueCamController				MEMCC
PlaySceneCamController					PSCC

--Note-- the names in the methods are just so you know what they are used for, you can call them whatever you want as long as they are the same types
--Note2-- All of these methods can be looked at (insert public git here)
List of methods that can be accessed in GCS are as follows:
CombatCalculator(int AttackersHealth, int AttackersMaxHealth, int AttackersAttackPower,Vector2 DefendersPosition, Int DefendersDefence)
CancelActionPlayScene()
WaitActionPlayScene()
CaptureActionPlayScene()
AttackActionPlayScene()
MoveActionPlayScene()
PlaySceneTurnChanger()
SpriteUpdateActivator()
AddBuildingToDictionary(GameObject BuildingToAdd)

]]--


