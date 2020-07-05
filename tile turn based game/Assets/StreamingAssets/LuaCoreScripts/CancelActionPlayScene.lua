--GCS.PrintDebug("Start");
com = GCS.VectorCompair(GCS.originalPositionOfUnit.x,GCS.originalPositionOfUnit.y,-1,-1);
GCS.PrintDebug(GCS.originalPositionOfUnit);
if com == false then
	GCS.originalPositionOfUnit = GCS.ChangeVector2(-1,-1)
end
GCS.PrintDebug(GCS.SelectedUnitPlayScene.name);
unit = GCS.GetUnitController(GCS.SelectedUnitPlayScene);
unit.UnitMovable = true
--GCS.PrintDebug("Starting 1st loop");
for k,v in pairs(GCS.UnitPos) do
	GCS.ChangeSpriteColor(v,1,1,1);
	unit2 = GCS.GetUnitController(v);
	unit2.MovementController();
	unit2.GetSightTiles();
end
--GCS.PrintDebug("Starting 2nd loop");
for k,v in pairs(GCS.TilePos) do
	GCS.ChangeSpriteColor(v,1,1,1);
	GCS.ChangeFOWSpriteColor(v,1,1,1);
	terrain = GCS.GetTerrainController(v);
	terrain.FogOfWarController();
end
--GCS.PrintDebug("Finalizing");
GCS.SelectedUnitPlayScene = null
PSCC.AttackButtonSelected = false
PSCC.SetActionButtonsToFalse();