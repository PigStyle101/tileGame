local mod = 'Core'
local title = 'Barracks'
local type = 'Building'
local description = 'A unit production building'
local pixelsPerUnit = 200
local health = 10
local defenceBonus = 20
local walkable = true
local canBuildUnits = true
local blocksSight = false
local onlyOneAllowed = false
local HeroSpawnPoint = false
local BuildableUnits = {'Archer','Soldier','Militia','Mage'}

function GetAllData()
Building.GetInfo(mod,title,type,description,pixelsPerUnit,health,defenceBonus,walkable,canBuildUnits,blocksSight,onlyOneAllowed,BuildableUnits,HeroSpawnPoint)
end