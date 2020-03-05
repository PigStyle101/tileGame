local mod = 'Core'
local title = 'Water'
local type = 'Terrain'
local description = 'Deep water'
local pixelsPerUnit = 64
local DefenceBonus = 0
local Wheight = 3
local BlocksSight = true
local IdleAnimations = false
local Overlays = true
local Connectable = false
local Walkable = false

function GetAllData()
Terrain.GetInfo(mod,title,type,description,pixelsPerUnit,DefenceBonus,Wheight,BlocksSight,IdleAnimations,Overlays,Connectable,Walkable)
end