local mod = 'Core'
local title = 'Town'
local type = 'Building'
local description = 'A collectiong of huts'
local pixelsPerUnit = 200
local health = 10
local defenceBonus = 10
local walkable = true
local canBuildUnits = false
local blocksSight = false
local onlyOneAllowed = false
local HeroSpawnPoint = false
local BuildableUnits = {}

function GetAllData()
Building.GetInfo(mod,title,type,description,pixelsPerUnit,health,defenceBonus,walkable,canBuildUnits,blocksSight,onlyOneAllowed,BuildableUnits,HeroSpawnPoint)
end