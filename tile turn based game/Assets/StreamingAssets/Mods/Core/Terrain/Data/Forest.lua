local mod = 'Core'
local title = 'Forest'
local type = 'Terrain'
local description = 'Forest with lots of trees'
local pixelsPerUnit = 64
local DefenceBonus = 10
local Wheight = 3
local BlocksSight = true
local IdleAnimations = false
local Overlays = false
local Connectable = false
local Walkable = true

function GetAllData()
Terrain.GetInfo(mod,title,type,description,pixelsPerUnit,DefenceBonus,Wheight,BlocksSight,IdleAnimations,Overlays,Connectable,Walkable)
end