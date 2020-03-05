local mod = 'Core'
local title = 'MainBase'
local type = 'Building'
local description = 'Capture this to win'
local pixelsPerUnit = 200
local health = 10
local defenceBonus = 30
local walkable = true
local canBuildUnits = false
local blocksSight = false
local onlyOneAllowed = true
local HeroSpawnPoint = true
local BuildableUnits = {}

function GetAllData()
Building.GetInfo(mod,title,type,description,pixelsPerUnit,health,defenceBonus,walkable,canBuildUnits,blocksSight,onlyOneAllowed,BuildableUnits,HeroSpawnPoint)
end