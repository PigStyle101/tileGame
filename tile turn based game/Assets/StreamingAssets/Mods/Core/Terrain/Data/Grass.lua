local mod = 'Core'
local title = 'Grass'
local type = 'Terrain'
local description = 'A field of grass'
local pixelsPerUnit = 64
local DefenceBonus = 0
local Wheight = 2
local BlocksSight = false
local IdleAnimations = false
local Overlays = false
local Connectable = false
local Walkable = true

function GetAllData()
Terrain.GetInfo(mod,title,type,description,pixelsPerUnit,DefenceBonus,Wheight,BlocksSight,IdleAnimations,Overlays,Connectable,Walkable)
end